using System.Collections.Specialized;
using FlexKidsConnection;
using WebHandler;

namespace FlexKids.Main
{
    public class WebClientAdapter : IWeb
    {
        private CookieAwareWebClient webclient;
        
        public WebClientAdapter()
        {
            webclient = new CookieAwareWebClient();
        }

        public byte[] PostValues(string address, NameValueCollection data)
        {
            return webclient.UploadValues(address, "POST", data);
        }

        public string DownloadPageAsString(string address)
        {
            return webclient.DownloadString(address);
        }

        public void Dispose()
        {
            if (webclient != null)
                webclient.Dispose();
            webclient = null;
        }
    }
}