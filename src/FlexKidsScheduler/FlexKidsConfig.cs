using System;
using System.Configuration;

namespace FlexKidsScheduler
{
    public class FlexKidsConfig : IFlexKidsConfig
    {
        public static readonly IFlexKidsConfig Instance = new FlexKidsConfig();

        private FlexKidsConfig()
        {
        }

        private static T GetConfigProperty<T>(string name) where T : IConvertible
        {
            if (ConfigurationManager.AppSettings[name] != null)
            {
                var s = ConfigurationManager.AppSettings[name].Trim();
                return (T)Convert.ChangeType(s, typeof(T));
            }
                
            throw new ConfigurationErrorsException(String.Format("Cannot find config setting {0}", name));
        }

        public String EmailFrom
        {
            get { return GetConfigProperty<string>("EmailFrom"); }
        }

        public String EmailTo2 
        {
            get { return GetConfigProperty<string>("EmailTo2"); }
        }

        public String EmailToName2
        {
            get { return GetConfigProperty<string>("EmailToName2"); }
        }
        
        public String EmailTo1
        {
            get { return GetConfigProperty<string>("EmailTo1"); }
        }

        public String EmailToName1
        {
            get { return GetConfigProperty<string>("EmailToName1"); }
        }


        public String SmtpHost 
        {
            get { return GetConfigProperty<string>("SmtpHost"); }
        }

        public int SmtpPort
        {
            get { return GetConfigProperty<int>("SmtpPort"); }
        }  

        public String SmtpUsername
        {
            get { return GetConfigProperty<string>("SmtpUsername"); }
        }

        public String SmtpPassword
        {
            get { return GetConfigProperty<string>("SmtpPassword"); }
        }

        public String GoogleCalendarAccount
        {
            get { return GetConfigProperty<string>("GoogleCalendarAccount"); }
        }

        public String GoogleCalendarId
        {
            get { return GetConfigProperty<string>("GoogleCalendarId"); }
        }
       
        public String GoogleCalendarKeyFile
        {
            get { return GetConfigProperty<string>("GoogleCalendarKeyFile"); }
        }
    }
}
