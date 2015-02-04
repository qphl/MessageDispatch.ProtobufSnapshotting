using System.Collections.Generic;

namespace CR.MessageDispatch.Core
{
    public interface ISnapshottingDispatcher<TMessage> : IDispatcher<TMessage>
    {
        IDispatcher<TMessage> InnerDispatcher { get; set; }
        int? LoadCheckpoint();
        IEnumerable<object> LoadObjects();
    }
}