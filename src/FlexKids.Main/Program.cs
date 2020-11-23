using System;
using System.Configuration;
using System.IO;
using System.Net;
using FlexKidsConnection;
using FlexKidsParser;
using FlexKidsScheduler;
using NLog;
using Reporter.Email;
using Reporter.GoogleCalendar;
using Reporter.Nlog;
using Repository;
using Repository.Mono.Sqlite;
using SimpleInjector;

namespace FlexKids.Main
{
    class Program
    {
        private static readonly Container Container = new Container();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Logger.Info("Starting.. ");

            AcceptAllCertificates();
            Logger.Info("Certificate validation disabled.");
            
            SetupDependencyContainer();
            try
            {
                Container.Verify();
            }
            catch (Exception e)
            {
                Logger.Error("Cannot verify the dependency injection container", e);
                return;
            }
            
            Logger.Info("Dependencies registered");

            var scheduler = Container.GetInstance<Scheduler>();
            scheduler.ScheduleChanged += delegate(object sender, ScheduleChangedArgs changedArgs)
            {
                var allHandlers = Container.GetAllInstances<IReportScheduleChange>();
                foreach (var handler in allHandlers)
                {
                    handler.HandleChange(changedArgs.Diff);
                }
            };

            Logger.Info("Start scheduler");
            scheduler.GetChanges();
            Logger.Info("Finished scheduler");

            scheduler.Dispose();

            Console.WriteLine("END");
            Console.WriteLine(DateTime.Now);
        }

        static private void SetupDependencyContainer()
        {
            Container.RegisterSingle(Sha1Hash.Instance);
            Container.RegisterSingle(DateTimeProvider.Instance);
            Container.RegisterSingle(FlexKidsConfig.Instance);

            Container.Register<IEmailService, EmailService>();
            Container.Register<IKseParser, FlexKidsHtmlParser>();
            RegisterFlexKidsConnection(Container);


            //  container.Register<IScheduleRepositoryFactory, MonoSqliteScheduleRepositoryFactory>();
            Container.Register<IScheduleRepository>(() => new MonoSqliteScheduleRepositoryFactory().CreateScheduleRepository());

            Container.RegisterAll<IReportScheduleChange>(
                typeof(EmailReportScheduleChange),
                typeof(ConsoleReportScheduleChange),
                typeof(CalendarReportScheduleChange));
        }

        static private void RegisterFlexKidsConnection(Container container)
        {
            if(ConfigurationManager.AppSettings["IFlexKidsConnection"] != null &&
               ConfigurationManager.AppSettings["IFlexKidsConnection"] == "FixedFlexKidsConnection.FixedFlexKidsConnection")
            {
                container.Register<IFlexKidsConnection, FixedFlexKidsConnection.FixedFlexKidsConnection>();
            }
            else
            {
                var h = ConfigurationManager.AppSettings["FlexKidsHost"];
                var u = ConfigurationManager.AppSettings["FlexKidsUsername"];
                var p = ConfigurationManager.AppSettings["FlexKidsPassword"];
                var config = new FlexKidsCookieConfig(
                    h,
                    u,
                    p);
                container.RegisterSingle(config);
                container.Register<IWeb, WebClientAdapter>();
                container.Register<IFlexKidsConnection, FlexKidsCookieWebClient>();
            }
        }

        private static void AcceptAllCertificates()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, error) =>
            {
                return true; //cert.GetCertHashString() == "xxxxxxxxxxxxxxxx";
            };
        }

        private static string GetContentByFile(string filename)
        {
            return File.ReadAllText(filename);
        }
    }
}