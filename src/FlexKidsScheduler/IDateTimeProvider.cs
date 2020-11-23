using System;

namespace FlexKidsScheduler
{
    public interface IDateTimeProvider
    {
        DateTime Today { get; }
        DateTime Now { get; }
    }
}
