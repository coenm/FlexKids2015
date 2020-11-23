using System.Collections.Generic;
using System.Linq;
using FlexKidsParser.Helper;
using FlexKidsScheduler.Model;
using HtmlAgilityPack;
using NLog;

namespace FlexKidsParser
{
    internal class ScheduleParser
    {
        private readonly int year;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private HtmlDocument document;
        private readonly string content;

        private HtmlDocument Document
        {
            get
            {
                if(document == null)
                {
                    document = new HtmlDocument();
                    document.LoadHtml(content);
                }
                return document;
            } 
        }

        public ScheduleParser(string content, int year)
        {
            this.year = year;
            this.content = content;
        }


      //        private async Task<HtmlAgilityPack.HtmlDocument> GetDocument(HttpResponseMessage responseMessage)
//        {
////            string result = await responseMessage.Content.ReadAsStringAsync();
////            if (!responseMessage.IsSuccessStatusCode)
////                throw new FileNotFoundException("Unable to retrieve document");
//        }

        public List<ScheduleItem> GetScheduleFromContent()
        {
            var  result = new List<ScheduleItem>();

            var divsIdUrenregistratie = Document.DocumentNode.Descendants()
                .Where(x => (x.IsDiv() && x.IdEquals("urenregistratie")))
                .ToList();

            if (divsIdUrenregistratie.Count() != 1)
            {
                Logger.Error("urenregistratieDiv");
                return null;
            }
            var divIdUrenregistratie = divsIdUrenregistratie.First();


            var tablesIdLocatieWeekoverzicht = divIdUrenregistratie.Descendants()
                .Where(x => x.IdEquals("locatie_weekoverzicht"))
                .ToList();
            if (tablesIdLocatieWeekoverzicht.Count() != 1)
            {
                Logger.Error("tableLocaties");
                return null;
            }
            var tableIdLocatieWeekoverzicht = tablesIdLocatieWeekoverzicht.First();


            // get head (hierin zitten de dagen en de datums)
            var theads = tableIdLocatieWeekoverzicht.Descendants().Where(x => x.IsThead()).ToList();
            if (theads.Count() != 1)
            {
                Logger.Error("theads");
                return null;
            }
            var thead = theads.First();

            //get tr
            var rows = thead.Descendants().Where(x => x.IsTr()).ToList();
            if (rows.Count() != 1)
            {
                Logger.Error("rows");
                return null;
            }
            var row = rows.First();

            //get columns
            var cols = row.Descendants().Where(x => x.IsTh()).ToList();
            if (cols.Count() != 5 + 1) //5 days + first column is info
            {
                Logger.Error("cols");
                return null;
            }

            // first column is nothing..
            // second till 6th is monday till friday
            //            var colMa = cols[1];
            //            var colDi = cols[2];
            //            var colWo = cols[3];
            //            var colDo = cols[4];
            //            var colVr = cols[5];

            //            for (int i = 1; i < 6; i++)
            //            {
            //                divs = cols[i].Descendants().Where(x => x.IsDiv()).ToList();
            //                if (divs.Count() == 2)
            //                {
            //                    Console.WriteLine("--------------------");
            //                    Console.WriteLine(divs[0].InnerText.Trim()); //day/date
            //                    Console.WriteLine(divs[1].InnerText.Trim()); //total hours to work that day
            //                    Console.WriteLine("--------------------");
            //                }
            //            }



            //            var tbodys = tableIdLocatieWeekoverzicht.Descendants().Where(x => x.Name == "tbody").ToList();
            var tbodys = tableIdLocatieWeekoverzicht.ChildNodes.Where(x => x.IsTbody()).ToList();
            var tbody = tbodys.First();

            var trs2 = tbody.ChildNodes.Where(x => x.IsTr()).ToList();

            foreach (var tr in trs2)
            {
                //get columns
                var tds = tr.ChildNodes.Where(x => x.IsTd()).ToList();

                // first column is info
                var infoTd = tds.First();
                // deze heeft 4 divs
                var infoTdDivs = infoTd.ChildNodes.Where(x => x.IsDiv()).ToList();
                //                for (int i = 0; i < infoTdDivs.Count; i++)
                //                {
                //                    Console.WriteLine(infoTdDivs[i].Attributes.First().Value);
                //                    Console.WriteLine(infoTdDivs[i].InnerText);
                //                    Console.WriteLine("+++++++++++++++++++++");
                //                }

                // second till 6th is monday till friday
                //            var colMa = cols[1];
                //            var colDi = cols[2];
                //            var colWo = cols[3];
                //            var colDo = cols[4];
                //            var colVr = cols[5];
                //dagen
                for (int i = 1; i < 6; i++)
                {

                    if (tds[i].HasChildNodes)
                    {
                        // at least one locatieplanning_2colommen en 1 div met totaal
                        // kunnen meerdere locatieplanning_2collomen zijn
                        if (tds[i].ChildNodes.Count(x => x.IsElement()) >= 2)
                        {

                            var locatieplanningen = tds[i].ChildNodes.Where(x => x.IsTable() && x.ClassContains("locatieplanning_2colommen"));
                            foreach (var firstItem in locatieplanningen)
                            {
                                

                            //table
                            //var firstItem = tds[i].ChildNodes.First(x => x.IsElement());
                            //firstItem.Class() = "";
                            if (firstItem.IsTable() && firstItem.ClassContains("locatieplanning_2colommen"))
                            {
                                /*
                                <table class="locatieplanning_2colommen dienst" width="100%">
                                    <tr>
                                        <td class="locatieplanning_naam bold" colspan=2>
                                            Dienst
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="left">09:00-18:30</td>
                                        <td class="right">(09:00)</td>
                                    </tr>
                                </table> 
                                */
                                if (firstItem.ChildNodes.Count(x => x.IsElement()) == 2)
                                {
                                    var firstRow = firstItem.ChildNodes.First(x => x.IsElement());
                                    var lastRow = firstItem.ChildNodes.Last(x => x.IsElement());

                                    if (lastRow.ChildNodes.Count(x => x.IsElement()) == 2)
                                    {
                                        var firstTd = lastRow.ChildNodes.First(x => x.IsElement()); //<td class="left">09:00-18:30</td>
                                        var lastTd = lastRow.ChildNodes.Last(x => x.IsElement()); //<td class="right">(09:00)</td> 

                                        var times = firstTd.InnerText.Trim(); //ie. 09:00-18:00
                                        var divs = cols[i].Descendants().Where(x => x.IsDiv()).ToList();
                                        var dateString = divs[0].InnerText.Trim();

                                        var locationString = infoTdDivs[3].InnerText;

                                        var dateWithoutTime = ParseDate.StringToDateTime(dateString, year);
                                        var startEndDateTimeTuple = ParseDate.CreateStartEndDateTimeTuple(dateWithoutTime, times);

                                        result.Add(new ScheduleItem()
                                        {
                                            Start = startEndDateTimeTuple.Item1,
                                            End = startEndDateTimeTuple.Item2,
                                            Location = locationString
                                        });
                                    }
                                }
                            }
                            }
                        }

                        //                        var divs2 = tds[i].Descendants().Where(x => x.IsDiv()).ToList();
                        //                        if (divs2.Count() == 2)
                        //                        {
                        //                            Console.WriteLine("--------------------");
                        //                            Console.WriteLine(divs2[0].InnerText.Trim());
                        //                            Console.WriteLine(divs2[1].InnerText.Trim());
                        //                            Console.WriteLine("--------------------");
                        //                        }
                    }
                }
            }

            return result;
        }
 
    }
}
