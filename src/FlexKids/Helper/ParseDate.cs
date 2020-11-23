using System;
using System.Linq;
using NLog;

namespace FlexKidsParser.Helper
{
    public static class ParseDate
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //time = 09:00
        public static DateTime AddStringTimeToDate(DateTime date, string time)
        {
            var split = time.Trim().Split(':');
            if (split.Count() != 2)
                throw new FormatException("Not a valid time format.");

            var hour = 0;
            if (!Int32.TryParse(split[0].Trim(), out hour))
                throw new Exception("No hours found");

            if (hour < 0 || hour > 23)
                throw new Exception("Hours not in range");

            var min = 0;
            if (!Int32.TryParse(split[1].Trim(), out min))
                throw new Exception("No minutes found");

            if (min < 0 || min >= 60)
                throw new Exception("Minutes not in range");

            return new DateTime(date.Year, date.Month, date.Day, hour, min, 0);
        }

        //startEndTime = 09:00-13:30
        //startEndTime = 14:00-17:30
        public static Tuple<DateTime, DateTime> CreateStartEndDateTimeTuple(DateTime date, string startEndTime)
        {
            var splitBothTimes = startEndTime.Trim().Split('-');
            if (splitBothTimes.Count() != 2)
                throw new Exception();

            return new Tuple<DateTime, DateTime>(
                AddStringTimeToDate(date, splitBothTimes[0]), 
                AddStringTimeToDate(date, splitBothTimes[1]));
        }

        public static string RemoveLastCharIfDot(string s)
        {
            if (s.Length == 0)
                return s;

            if (s.Substring(s.Length - 1, 1) == ".")
                return s.Substring(0, s.Length - 1);
            return s;
        }

        public static DateTime StringToDateTime(string input, int year)
        {
            var splitInput = input.Split(' ');
            if (splitInput.Count() != 2)
                throw new FormatException("Not a valid format.");

            var spitDate = splitInput[1].Trim().Split('-');
            if (spitDate.Count() != 2)
                throw new Exception();

            int day;
            if (!Int32.TryParse(spitDate[0].Trim(), out day))
                throw new Exception();

            if (day <= 0)
                throw new Exception();

            if (day > 31)
                throw new Exception();

            var month = 0;
            var monthTxt = RemoveLastCharIfDot(spitDate[1].Trim());
            
            switch (monthTxt)
            {
                case "jan": //unchecked
                    month = 1;
                    break;

                case "feb":
                    month = 2;
                    break;

                case "mrt":
                    month = 3;
                    break;

                case "apr":
                    month = 4;
                    break;

                case "mei":
                    month = 5;
                    break;

                case "jun":
                    month = 6; // unchecked
                    break;

                case "jul":
                    month = 7; // unchecked
                    break;
               
                case "aug":
                    month = 8; // unchecked
                    break;

                case "sep":
                case "sept":
                    month = 9; // unchecked
                    break;

                case "okt":
                    month = 10; // unchecked
                    break;

                case "nov":
                    month = 11; // unchecked
                    break;

                case "dec":
                    month = 12;
                    break;

                default:
                    Logger.Error(monthTxt + "  is not catched");
                    throw new Exception(monthTxt + "  is not catched");
            }

            var result = new DateTime(year, month, day, 0, 0, 0);
            return result;

        } 
    }


    //
    //
    //  CultureInfo myCultureInfo = new CultureInfo("nl-NL");
    //  var dt = new DateTime(2014, 12, 19);
    //                                    
    //                                           
    //      //myCultureInfo.DateTimeFormat.Calendar = new
    //      //DateTime date1 = new DateTime(1890, 9, 10);
    //
    //      var xyz = dt.ToString("ddd dd-MMM", myCultureInfo);
    //
    //
    //
    //      string MyString = "12 Juni 2008";
    //      var strDate = divs[0].InnerText.Trim().Split(' ').Last();
    //
    //      try
    //      {
    //          DateTime MyDateTime = DateTime.ParseExact(strDate.Trim(), "dd-MMM", myCultureInfo);
    //          Console.WriteLine(MyDateTime);
    //      }
    //      catch (FormatException)
    //      {
    //          Console.WriteLine("Unable to parse '{0}'", strDate);
    //      }
    //
    //      //DateTime MyDateTime = DateTime.Parse(strDate, myCultureInfo);
    //      // Console.WriteLine(MyDateTime);
}