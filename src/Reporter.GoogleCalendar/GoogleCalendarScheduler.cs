using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FlexKidsScheduler;
using FlexKidsScheduler.Model;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

namespace Reporter.GoogleCalendar
{
    internal class GoogleCalendarScheduler : IDisposable
    {
        private readonly IGoogleCalendarService calendarService;

        private readonly string googleCalendarId;
        private readonly string googleCalendarAccount;

        public GoogleCalendarScheduler(IDateTimeProvider dateTimeProvider, IFlexKidsConfig flexKidsConfig /*IGoogleCalendarService googleCalendarService*/)
        {

            //       if (googleCalendarService == null)
            //           throw new ArgumentNullException("googleCalendarService");

            //       calendarService = googleCalendarService;

            googleCalendarId = flexKidsConfig.GoogleCalendarId;
            googleCalendarAccount = flexKidsConfig.GoogleCalendarAccount;
           
            //TODO Check if file exists and make it more robust.
            var certificate = new X509Certificate2(@flexKidsConfig.GoogleCalendarKeyFile, "notasecret", X509KeyStorageFlags.Exportable);

            var credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(googleCalendarAccount)
                {
                    Scopes = new[] { CalendarService.Scope.Calendar }
                }.FromCertificate(certificate));

            // Create the service.
            var service = new CalendarService(new BaseClientService.Initializer
            { 
                HttpClientFactory = new Google.Apis.Http.HttpClientFactory(),
                HttpClientInitializer = credential,
                ApplicationName = "FlexKids Rooster by CoenM",
            });

            if (service == null)
            {
                throw new Exception("Cannot create service.");
            }

            calendarService = new GoogleCalendarService(service);


            var calendar = calendarService.GetCalendarById(googleCalendarId);
            if (calendar == null)
            {
                throw new Exception("Cannot find calendar");
            }
        }

        public void MakeEvents(IList<ScheduleDiff> schedule)
        {
            var weeks = schedule.Select(x => x.Schedule.Week).Distinct();

            foreach (var w in weeks)
            {
                /*
                List<string> xxy = new List<string>();
                //xxy.Add("MadeByApplication=Coen");
                xxy.Add("Week=" + w.Year + "-" + w.WeekNr);
                var x = new Google.Apis.Util.Repeatable<string>(xxy);

                

                var request = calendarService.CreateListRequest(calendarId);
                request.MaxResults = 100;
                request.ShowDeleted = false;
                
                //request.SharedExtendedProperty = x;
                request.SharedExtendedProperty = "Week=" + w.Year + "-"+  w.WeekNr;
*/

                var request = calendarService.CreateListRequestForWeek(googleCalendarId, w);

                var result = calendarService.GetEvents(request);
                var allRows = new List<Event>();
                while (result.Items != null)
                {
                    //Add the rows to the final list
                    allRows.AddRange(result.Items);

                    // We will know we are on the last page when the next page token is
                    // null.
                    // If this is the case, break.
                    if (result.NextPageToken == null)
                        break;
                    
                    // Prepare the next page of results
                    request.PageToken = result.NextPageToken;

                    // Execute and process the next page request
                    result = calendarService.GetEvents(request);
                }
                       
                foreach (Event ev in allRows)
                    calendarService.DeleteEvent(googleCalendarId, ev);
            }
            
            // add items to calendar.
            foreach (var item in schedule.Where(x => x.Status == ScheduleStatus.Added).OrderBy(x => x.Start).ThenBy(x => x.Status))
            {
                var extendedProperty = new Event.ExtendedPropertiesData();

                extendedProperty.Shared = new Dictionary<string, string>();
                // x.Shared.Add("MadeByApplication", "Coen");
                extendedProperty.Shared.Add("Week", item.Schedule.Week.Year + "-" + item.Schedule.Week.WeekNr);

                //  queryEvent.SharedExtendedProperty = "EventID=3684";
                var newEvent = new Event
                {
                    Start = new EventDateTime { DateTime = item.Schedule.StartDateTime },
                    End = new EventDateTime { DateTime = item.Schedule.EndDateTime },
                    Description = "Ja, je moet werken! xx coen",
                    Location = item.Schedule.Location,
                    Summary = item.Schedule.Location,


                    ExtendedProperties = extendedProperty
                };
                var e = calendarService.InsertEvent(googleCalendarId, newEvent);
                             
            }

            // xyz@gmail.com -> Gmail Account (default Kalender)
            // var request = service.Events.Create("xyz@gmail.com", newEvent);
        }

        public void Dispose()
        {
        }
    }
}