// ------------------------------------------------------
// <copyright file="Mediator.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using MyFinance.Messaging;
using MyFinance.Results;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace MyFinance.Messaging;

/// <summary>
/// High-performance implementation of the <see cref="IMediator"/> interface with pipeline behavior support.
/// This mediator uses compiled expressions, caching, and supports pipeline behaviors for cross-cutting concerns.
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider serviceProvider;

    // High-performance caching for compiled handlers and behaviors - thread-safe
    private static readonly ConcurrentDictionary<Type, HandlerDescriptor> HandlerCache = new();
    private static readonly ConcurrentDictionary<Type, RequestTypeInfo> TypeCache = new();
    private static readonly ConcurrentDictionary<(Type requestType, Type responseType), Type[]> BehaviorTypeCache = new();
    private static readonly ConcurrentDictionary<Type, NotificationHandlerDescriptor> NotificationHandlerCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve handlers.</param>
    public Mediator(IServiceProvider serviceProvider) => this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <inheritdoc />
    public async Task<Result> Send(ICommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        return await SendInternal<Result>(command, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        return await SendInternal<Result<TResponse>>(command, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        return await SendInternal<Result<TResponse>>(query, cancellationToken);
    }

    /// <inheritdoc />
    public async Task Publish(INotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        await PublishInternal(notification, cancellationToken);
    }

    /// <summary>
    /// Helper method to await a task and convert the result to an object.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The task to await.</param>
    /// <returns>An object representing the awaited result.</returns>
    public static async Task<object> AwaitAndConvertToObject<T>(Task<T> task)
    {
        var result = await task;
        return result!;
    }

    /// <summary>
    /// Internal method that handles the pipeline behavior chain and final handler execution.
    /// Uses caching for optimal performance while maintaining pipeline behavior support.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request (command or query).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the handler chain.</returns>
    private async Task<TResponse> SendInternal<TResponse>(IRequest request, CancellationToken cancellationToken)
        where TResponse : IResultBase
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        // Get cached behavior types for this request/response combination
        var behaviorTypes = GetCachedBehaviorTypes(requestType, responseType);
        var behaviors = behaviorTypes.SelectMany(bt => serviceProvider.GetServices(bt)).Reverse().ToArray();

        // Create the final handler delegate using compiled invocation
        RequestHandlerDelegate<TResponse> handler = () => this.CallHandlerOptimized<TResponse>(request, cancellationToken);

        // Build the pipeline chain with optimized behavior invocation
        foreach (var behavior in behaviors)
        {
            var currentHandler = handler;
            var currentBehavior = behavior;
            var behaviorType = behavior!.GetType();

            handler = () =>
            {
                // Use cached compiled invoker for behaviors if available
                var handleMethod = GetCachedBehaviorMethod(behaviorType);
                return (Task<TResponse>)handleMethod.Invoke(currentBehavior, [request, currentHandler, cancellationToken])!;
            };
        }

        // Execute the pipeline
        return await handler();
    }

    /// <summary>
    /// Internal method that handles notification publishing to multiple handlers.
    /// Notifications are published to all registered handlers concurrently for optimal performance.
    /// Uses caching for compiled handler invocation.
    /// </summary>
    /// <param name="notification">The notification to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task PublishInternal(INotification notification, CancellationToken cancellationToken)
    {
        var notificationType = notification.GetType();

        // Get cached notification handler descriptor
        var descriptor = GetOrCreateNotificationHandlerDescriptor(notificationType);

        // Get all registered notification handlers for this notification type
        var handlers = serviceProvider.GetServices(descriptor.HandlerType);

        if (!handlers.Any())
        {
            return; // No handlers registered, which is valid for notifications
        }

        // Create tasks for all handlers and execute them concurrently using compiled invokers
        var tasks = handlers.Select(handler => descriptor.CompiledInvoker(handler!, notification, cancellationToken));

        // Wait for all handlers to complete
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Optimized handler calling with compiled invocation and caching.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the handler.</returns>
    private async Task<TResponse> CallHandlerOptimized<TResponse>(IRequest request, CancellationToken cancellationToken)
        where TResponse : IResultBase
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        // Get or create cached handler descriptor
        var descriptor = GetOrCreateHandlerDescriptor(requestType, responseType);

        var handler = serviceProvider.GetService(descriptor.HandlerType) ?? throw new InvalidOperationException($"No handler registered for request type '{requestType.Name}'.");

        // Use compiled invoker for maximum performance
        var result = await descriptor.CompiledInvoker(handler, request, cancellationToken);
        return (TResponse)result;
    }

    /// <summary>
    /// Gets cached behavior types for the specified request/response combination.
    /// </summary>
    private static Type[] GetCachedBehaviorTypes(Type requestType, Type responseType)
    {
        return BehaviorTypeCache.GetOrAdd((requestType, responseType), _ =>
        {
            var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
            return [behaviorType];
        });
    }

    /// <summary>
    /// Gets cached behavior method for faster invocation.
    /// </summary>
    private static MethodInfo GetCachedBehaviorMethod(Type behaviorType)
    {
        // For behaviors, we still use reflection as they're less performance-critical
        // and the complexity of compiled behavior invocation is high
        var interfaces = behaviorType.GetInterfaces();
        var pipelineInterface = Array.Find(interfaces, i =>
                   i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>))
            ?? throw new InvalidOperationException($"Behavior type '{behaviorType.Name}' does not implement IPipelineBehavior<,>.");
        var method = pipelineInterface.GetMethod("Handle") ?? throw new InvalidOperationException($"Handle method not found on behavior type '{behaviorType.Name}'.");
        return method;
    }

    /// <summary>
    /// Gets or creates a cached handler descriptor for the specified request type.
    /// </summary>
    private static HandlerDescriptor GetOrCreateHandlerDescriptor(Type requestType, Type responseType)
    {
        Func<Type, HandlerDescriptor> valueFactory = _ =>
        {
            var typeInfo = GetRequestTypeInfo(requestType, responseType);
            var handlerType = CreateHandlerType(requestType, typeInfo.ResponseType, typeInfo.IsCommand);
            var handleMethod = GetHandleMethod(handlerType);
            var compiledInvoker = CompileHandlerInvoker(handleMethod, handlerType);

            return new HandlerDescriptor
            {
                HandlerType = handlerType,
                HandleMethod = handleMethod,
                CompiledInvoker = compiledInvoker,
            };
        };

        return HandlerCache.GetOrAdd(requestType, valueFactory);
    }

    /// <summary>
    /// Gets cached request type information.
    /// </summary>
    private static RequestTypeInfo GetRequestTypeInfo(Type requestType, Type responseType)
    {
        Func<Type, RequestTypeInfo> valueFactory = _ =>
        {
            // Determine if this is a command or query by checking interfaces
            var isCommand = Array.Find(requestType.GetInterfaces(), i =>
                i == typeof(ICommand) ||
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>)) != null;

            Type actualResponseType;

            if (responseType == typeof(Result))
            {
                actualResponseType = typeof(void); // No response for simple commands
            }
            else if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                actualResponseType = responseType.GetGenericArguments()[0];
            }
            else
            {
                throw new InvalidOperationException($"Unsupported response type: {responseType.Name}");
            }

            return new RequestTypeInfo
            {
                IsCommand = isCommand,
                ResponseType = actualResponseType,
            };
        };
        return TypeCache.GetOrAdd(requestType, valueFactory);
    }

    /// <summary>
    /// Creates the appropriate handler type based on request and response types.
    /// </summary>
    private static Type CreateHandlerType(Type requestType, Type responseType, bool isCommand)
    {
        if (isCommand)
        {
            return responseType == typeof(void)
                ? typeof(ICommandHandler<>).MakeGenericType(requestType)
                : typeof(ICommandHandler<,>).MakeGenericType(requestType, responseType);
        }
        else
        {
            return typeof(IQueryHandler<,>).MakeGenericType(requestType, responseType);
        }
    }

    /// <summary>
    /// Gets the Handle method from the handler type.
    /// </summary>
    private static MethodInfo GetHandleMethod(Type handlerType)
    {
        var methods = handlerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name == "Handle")
            .ToArray();

        var method = methods.FirstOrDefault() ?? throw new InvalidOperationException($"Handle method not found on handler type '{handlerType.Name}'.");
        return method;
    }

    /// <summary>
    /// Compiles a fast invoker for the handler method using expression trees.
    /// This is much faster than reflection-based invocation.
    /// </summary>
    private static Func<object, object, CancellationToken, Task<object>> CompileHandlerInvoker(MethodInfo method, Type handlerType)
    {
        // Parameters: handler instance, request, cancellationToken
        var handlerParam = Expression.Parameter(typeof(object), "handler");
        var requestParam = Expression.Parameter(typeof(object), "request");
        var cancellationTokenParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

        // Cast handler to correct type
        var handlerCast = Expression.Convert(handlerParam, handlerType);

        // Cast request to correct type
        var requestType = method.GetParameters()[0].ParameterType;
        var requestCast = Expression.Convert(requestParam, requestType);

        // Method call: handler.Handle(request, cancellationToken)
        var methodCall = Expression.Call(handlerCast, method, requestCast, cancellationTokenParam);

        // Handle Task<Result> vs Task<Result<T>> return types
        Expression returnExpression;
        if (method.ReturnType.IsGenericType)
        {
            var taskResultType = method.ReturnType.GetGenericArguments()[0];

            // For Task<Result<T>>, we need to convert the result to object
            var awaitExpression = Expression.Call(
                typeof(Mediator),
                nameof(AwaitAndConvertToObject),
                [taskResultType],
                methodCall);

            returnExpression = awaitExpression;
        }
        else
        {
            // For Task<Result>, convert directly
            var awaitExpression = Expression.Call(
                typeof(Mediator),
                nameof(AwaitAndConvertToObject),
                [typeof(Result)],
                methodCall);

            returnExpression = awaitExpression;
        }

        var lambda = Expression.Lambda<Func<object, object, CancellationToken, Task<object>>>(
            returnExpression,
            handlerParam,
            requestParam,
            cancellationTokenParam);

        return lambda.Compile();
    }

    /// <summary>
    /// Gets or creates a cached notification handler descriptor for the specified notification type.
    /// </summary>
    private static NotificationHandlerDescriptor GetOrCreateNotificationHandlerDescriptor(Type notificationType)
    {
        Func<Type, NotificationHandlerDescriptor> valueFactory = _ =>
                {
                    var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);
                    var handleMethod = handlerType.GetMethod("Handle") ?? throw new InvalidOperationException($"Handle method not found on notification handler type '{handlerType.Name}'.");
                    var compiledInvoker = CompileNotificationHandlerInvoker(handleMethod, handlerType, notificationType);

                    return new NotificationHandlerDescriptor
                    {
                        HandlerType = handlerType,
                        CompiledInvoker = compiledInvoker,
                    };
                };
        return NotificationHandlerCache.GetOrAdd(notificationType, valueFactory);
    }

    /// <summary>
    /// Compiles a fast invoker for the notification handler method using expression trees.
    /// </summary>
    private static Func<object, object, CancellationToken, Task> CompileNotificationHandlerInvoker(MethodInfo method, Type handlerType, Type notificationType)
    {
        // Parameters: handler instance, notification, cancellationToken
        var handlerParam = Expression.Parameter(typeof(object), "handler");
        var notificationParam = Expression.Parameter(typeof(object), "notification");
        var cancellationTokenParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

        // Cast handler to correct type
        var handlerCast = Expression.Convert(handlerParam, handlerType);

        // Cast notification to correct type
        var notificationCast = Expression.Convert(notificationParam, notificationType);

        // Method call: handler.Handle(notification, cancellationToken)
        var methodCall = Expression.Call(handlerCast, method, notificationCast, cancellationTokenParam);

        var lambda = Expression.Lambda<Func<object, object, CancellationToken, Task>>(
            methodCall,
            handlerParam,
            notificationParam,
            cancellationTokenParam);

        return lambda.Compile();
    }

    /// <summary>
    /// Descriptor for cached handler information.
    /// </summary>
    private sealed class HandlerDescriptor
    {
        public required Type HandlerType { get; init; }

        public required MethodInfo HandleMethod { get; init; }

        public required Func<object, object, CancellationToken, Task<object>> CompiledInvoker { get; init; }
    }

    /// <summary>
    /// Information about request types.
    /// </summary>
    private sealed class RequestTypeInfo
    {
        public required bool IsCommand { get; init; }

        public required Type ResponseType { get; init; }
    }

    /// <summary>
    /// Descriptor for cached notification handler information.
    /// </summary>
    private sealed class NotificationHandlerDescriptor
    {
        public required Type HandlerType { get; init; }

        public required Func<object, object, CancellationToken, Task> CompiledInvoker { get; init; }
    }
}