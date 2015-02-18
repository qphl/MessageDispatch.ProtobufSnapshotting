using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CR.MessageDispatch.Core
{
    public class TransactionalDispatcher<TMessage>:IDispatcher<TMessage>
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
