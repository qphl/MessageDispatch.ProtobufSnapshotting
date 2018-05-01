// <copyright file="TransactionalDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    public class TransactionalDispatcher<TMessage> : IDispatcher<TMessage>
    {
        private readonly IDispatcher<TMessage> _dispatcher;

        public TransactionalDispatcher(IDispatcher<TMessage> dispatcher)
        {
            _dispatcher = dispatcher;
        }

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
