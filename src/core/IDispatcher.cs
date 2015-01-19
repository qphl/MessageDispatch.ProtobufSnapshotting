namespace CR.MessageDispatch.Core
{
    public interface IDispatcher<in TMessage>
    {
        void Dispatch(TMessage message);
    }
}
