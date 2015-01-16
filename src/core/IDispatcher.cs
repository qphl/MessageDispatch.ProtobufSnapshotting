namespace core
{
    public interface IDispatcher<in TMessage>
    {
        void Dispatch(TMessage message);
    }
}
