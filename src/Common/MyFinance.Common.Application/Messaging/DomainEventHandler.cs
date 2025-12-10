using System;
using System.Collections.Generic;
using System.Text;
using MyFinance.Common.Domain;

namespace MyFinance.Common.Application.Messaging;

public abstract class DomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    public abstract Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);

    public Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken = default) =>
        Handle((TDomainEvent)domainEvent, cancellationToken);
}
