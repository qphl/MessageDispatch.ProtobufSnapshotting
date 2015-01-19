namespace CR.MessageDispatch.Core
{
    public interface IConsume<in TMessage>
    {
        void Handle(TMessage message);
    }
}