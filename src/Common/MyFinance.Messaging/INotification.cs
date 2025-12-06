// ------------------------------------------------------
// <copyright file="INotification.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

namespace MyFinance.Messaging
{
    /// <summary>
    /// Represents a notification that can be published to multiple handlers.
    /// Notifications are fire-and-forget messages that don't return a value.
    /// </summary>
    public interface INotification;
}