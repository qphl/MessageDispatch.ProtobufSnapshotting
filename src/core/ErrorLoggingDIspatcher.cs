using System;

namespace CR.MessageDispatch.Core
{
    public class ErrorLoggingDispatcher<TMessage> : IDispatcher<TMessage>
    {
        private readonly IDispatcher<TMessage> _dispatcher;
        private readonly Action<TMessage, Exception> _exceptionLogAction;

        public ErrorLoggingDispatcher(IDispatcher<TMessage> dispatcher, Action<TMessage, Exception> exceptionLogAction)
        {
            _dispatcher = dispatcher;
            _exceptionLogAction = exceptionLogAction;
        }

        public void Dispatch(TMessage message)
        {
            try
            {
                _dispatcher.Dispatch(message);
            }
            catch (Exception e)
            {
                _exceptionLogAction(message, e);
                throw;
            }
        }
    }
}