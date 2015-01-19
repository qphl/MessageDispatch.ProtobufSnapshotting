namespace core
{
    public interface IConsume<in TMessage>
    {
        void Handle(TMessage message);
    }
}