using System;

namespace FlexKidsScheduler
{
    public interface IFlexKidsConnection : IDisposable
    {
        string GetSchedulePage(int id);

        string GetAvailableSchedulesPage();
    }
}