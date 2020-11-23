using FlexKidsScheduler;
using System.IO;

namespace FixedFlexKidsConnection
{
    public class FixedFlexKidsConnection : IFlexKidsConnection
    {
        private string GetFileContent(string filename)
        {
            var file = Path.Combine(filename);

            if (File.Exists(filename))
                return File.ReadAllText(file);
            return "";
        }

        public string GetSchedulePage(int id)
        {
            switch (id)
            {
                case 0:
                    return GetFileContent("files/2015-08.html");

                default:
                    return "";
            }
        }

        public string GetAvailableSchedulesPage()
        {
            return GetFileContent("files/index.html");
        }

        public void Dispose()
        {

        }
    }
}
