using System;

namespace CR.MessageDispatch.Core
{
    public class OnExceptionSkipOrExitDispatcher<TMessage> : IDispatcher<TMessage>
    {
        private readonly IDispatcher<TMessage> _dispatcher;
        private readonly Func<TMessage, Exception, bool> _shouldExit;
        private readonly Action<TMessage, Exception> _onExitLog;

        public OnExceptionSkipOrExitDispatcher(IDispatcher<TMessage> dispatcher,
            Func<TMessage, Exception, bool> shouldExit, Action<TMessage, Exception> onExitLog = null)
        {
            _dispatcher = dispatcher;
            _shouldExit = shouldExit;
            _onExitLog = onExitLog ?? ((_, __) => { });
        }

        public void Dispatch(TMessage message)
        {
            try
            {
                _dispatcher.Dispatch(message);
            }
            catch (Exception e)
            {
                if (_shouldExit(message, e))
                {
                    _onExitLog(message, e);
                    Environment.Exit(1);
                }
            }
        }
    }

    public class OnExceptionExitDispatcher<TMessage> : OnExceptionSkipOrExitDispatcher<TMessage>
    {
        public OnExceptionExitDispatcher(IDispatcher<TMessage> dispatcher,
            Action<TMessage, Exception> onExitLog = null) : base(dispatcher, (_, __) => true, onExitLog)
        {
        }
    }
}