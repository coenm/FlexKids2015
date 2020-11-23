using System;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace Reporter.GoogleCalendar
{
    interface IGoogleCalendarService : IDisposable
    {
        Calendar GetCalendarById(string id);

        EventsResource.ListRequest CreateListRequest(string calendarId);

        EventsResource.ListRequest CreateListRequestForWeek(string calendarId, Repository.Model.Week week);

        Events GetEvents(EventsResource.ListRequest listRequest);

        string DeleteEvent(string calendarId, Event calendarEvent);

        Event InsertEvent(string calendarId, Event calendarEvent);
    }
}
