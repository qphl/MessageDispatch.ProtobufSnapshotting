namespace core
{
    public interface IConsume<in TMessage>
    {
        void Handle(TMessage message);
    }

    public interface IConsume<in TMessage, in TSequenceNumber>
    {
        void Handle(TMessage message, TSequenceNumber sequenceNumber);
    }
}