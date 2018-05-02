// <copyright file="TransactionalDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System.Transactions;

    /// <summary>
    /// Transaction based dispatcher implementation.
    /// </summary>
    /// <typeparam name="TMessage">Message Type</typeparam>
    public class TransactionalDispatcher<TMessage> : IDispatcher<TMessage>
    {
        private readonly IDispatcher<TMessage> _dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalDispatcher{TMessage}"/> class.
        /// </summary>
        /// <param name="dispatcher">Dispatcher of a certain message type.</param>
        public TransactionalDispatcher(IDispatcher<TMessage> dispatcher)
        {
            _dispatcher = dispatcher;
        }

        /// <inheritdoc />
        public void Dispatch(TMessage message)
        {
            using (var transaction = new TransactionScope())
            {
                _dispatcher.Dispatch(message);
                transaction.Complete();
            }
        }
    }
}
