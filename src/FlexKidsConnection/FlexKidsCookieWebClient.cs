using System;
using System.Collections.Specialized;
using FlexKidsScheduler;

namespace FlexKidsConnection
{
    public class FlexKidsCookieWebClient : IFlexKidsConnection
    {
        private readonly IWeb web;
        private readonly FlexKidsCookieConfig config;
        private Boolean isLoggedIn;

        public FlexKidsCookieWebClient(IWeb web, FlexKidsCookieConfig config)
        {
            this.web = web;
            this.config = config;
        }

        private void Login()
        {
            var reqparm = new NameValueCollection
            {
                {"username", config.Username},
                {"password", config.Password},
                {"role", "4"},
                {"login", "Log in"}
            };
            web.PostValues(config.HostUrl + "/user/process", reqparm);
//            var responsebytes = webclient.UploadValues(BaseUrl + "/user/process", "POST", reqparm);
//            var responsebody = Encoding.UTF8.GetString(responsebytes);
            
            isLoggedIn = true;
        }
        
        public string GetSchedulePage(int id)
        {
            if(!isLoggedIn)
                Login();

            var urlSchedule = String.Format(config.HostUrl + "/personeel/rooster/week?week={0}", id);
            return web.DownloadPageAsString(urlSchedule);
        }

        public string GetAvailableSchedulesPage()
        {
            if (!isLoggedIn)
                Login();

            return web.DownloadPageAsString(config.HostUrl + "/personeel/rooster/index");
        }

        public void Dispose()
        {
            if(web == null)
                return;

            web.Dispose();
        }
    }
}
