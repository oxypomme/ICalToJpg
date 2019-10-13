using System;
using System.IO;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using CoreHtmlToImage;

// TODO 01 : Changement de mois
// TODO 02 : Séparer en fonctions

namespace icalToHtmlToJpg
{
    class ICalToJpg
    {
        #region Private Fields

        private static string[] CalendarUrl { get; set; }

        #endregion

        #region Private Methods

        static void Main(string[] args)
        {
            /*== Get Calendar ==*/
            CalendarUrl = new string[8];
            CalendarUrl[0] = @"https://dptinfo.iutmetz.univ-lorraine.fr/lna/agendas/ical.php?ical=e81e5e310001831"; //1.1
            CalendarUrl[1] = @"https://dptinfo.iutmetz.univ-lorraine.fr/lna/agendas/ical.php?ical=4352c5485001785"; //1.2
            CalendarUrl[2] = @"https://dptinfo.iutmetz.univ-lorraine.fr/lna/agendas/ical.php?ical=329314450001800"; //2.1
            CalendarUrl[3] = @""; //2.2
            CalendarUrl[4] = @""; //3.1
            CalendarUrl[5] = @"https://dptinfo.iutmetz.univ-lorraine.fr/lna/agendas/ical.php?ical=b4a52df5e501843"; //3.2
            CalendarUrl[6] = @"https://dptinfo.iutmetz.univ-lorraine.fr/lna/agendas/ical.php?ical=561e49e97901779"; //4.1
            CalendarUrl[7] = @"https://dptinfo.iutmetz.univ-lorraine.fr/lna/agendas/ical.php?ical=3a1ee9527101771"; //4.2

            var stringICal = new string[CalendarUrl.Length];
            for (int i = 0; i < CalendarUrl.Length; i++)
            {
                try
                {
                    //we download the differents icals
                    using (var wc = new WebClient())
                        stringICal[i] = wc.DownloadString(CalendarUrl[i]);
                }
                catch (ArgumentException)
                {
                    //if no ical is provided, we just ignore it. I already give enough to the promotion, if they don't give me the link, fuck them.
                    continue;
                }
            }

            /*== Create Events ==*/
            List<string[]> listEvents = new List<string[]>();
            List<Event> events = new List<Event>();

            DayOfWeek day = DateTime.Now.DayOfWeek;
            int days = day - DayOfWeek.Monday;
            DateTime start = DateTime.Now.AddDays(-days);
            DateTime end = start.AddDays(6);

            char[] startChar = { start.ToString()[0], start.ToString()[1] };
            char[] endChar = { end.ToString()[0], end.ToString()[1] };

            int idLine = 0;
            int dateStart = int.Parse(new string(startChar));
            int dateEnd = int.Parse(new string(endChar));
            int month = DateTime.Now.Month;

            bool isEndOfCal = false;

            // Split the ICal line by line and add it in a List<>
            listEvents.Add(stringICal[1].Split("\n", StringSplitOptions.RemoveEmptyEntries));

            while (!isEndOfCal)
            {
                for (int j = 0; j < listEvents[idLine].Length; j++)
                {
                    // if it's a new event
                    if (listEvents[idLine][j] == "BEGIN:VEVENT")
                    {
                        // create an event
                        var ev = new Event();
                        ev.CreateEvent(listEvents[idLine][j + 1], //name
                                       listEvents[idLine][j + 2], //start date
                                       listEvents[idLine][j + 3], //end date
                                       listEvents[idLine][j + 4]); //localisation
                        ev.CleanEvent();

                        // if the event is in this week add it in another list
                        int evDate = ev.GetDay();
                        if (dateEnd >= evDate && evDate >= dateStart && ev.GetMonth() == month)
                        {
                            events.Add(ev);
                        }

                    }
                    //if it's the end of the ICal, exit the loop
                    else if (listEvents[idLine][j] == "END:VCALENDAR") { isEndOfCal = true; }
                }
                idLine++; // New line
            }

            /*== Create HTML ==*/

            var body = new StringBuilder();
            var finalHtml = new StringBuilder();

            // Get a list of the dates in the week
            List<int> dates = new List<int>();
            IEnumerable<int> numbers = Enumerable.Range(dateStart, dateEnd - dateStart);
            foreach (int n in numbers)
            {
                dates.Add(n);
            }

            string[] hours = { "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18" };
            string[] minutes = { "00", "15", "30", "45" };

            int id_h = 0;
            int id_m = 0;
            int[] delays = new int[6];

            body.Append("<thead>\n<tr>\n<th></th>\n");
            // Create table head
            int dateHead = 0;
            foreach(int n in numbers)
            {
                bool isActive = n == DateTime.Now.Day;
                var dayHead = new DateTime(DateTime.Now.Year, month, n);
                body.AppendLine(string.Format(@"<th class=""{3}"">{0} {1}/{2}</th>", dayHead.DayOfWeek, dates[dateHead], month, isActive ? "active" : ""));
                dateHead++;
            }
            body.Append("</tr>\n</thead>\n<tbody>\n");

            // For each line (there's 40 lines)
            for (int i = 0; i < 40; i++)
            {
                int dcount = -1;

                // Start of line
                body.AppendLine("<tr>");

                // Each 4 lines add the time
                if (i % 4 == 0)
                {
                    body.AppendLine(string.Format(@"<th rowspan=""4"">{0}h-{1}h", hours[id_h], hours[id_h + 1]));
                    id_h++;
                }

                // Each 4 lines, reset the index for minutes
                if (id_m == 4)
                {
                    id_m = 1;
                }
                else
                {
                    id_m++;
                }

                // For each day
                foreach (var d in dates)
                {
                    dcount++; // ID of the day
                    if (delays[dcount] > 0) { delays[dcount]--; } // Reduce cooldown if exists
                    bool isFound = false;

                    // Check for each event if the time and the date matches
                    foreach (var ev in events)
                    {
                        if (ev.GetStart() == string.Format("{0}:{1}", hours[id_h - 1], minutes[id_m - 1]) && ev.GetDay() == d)
                        {
                            // Translate complete str time to an int with hours only
                            char[] evStartHChar = { ev.GetStart()[0], ev.GetStart()[1] };
                            int evStartHInt = int.Parse(new string(evStartHChar));
                            char[] evEndHChar = { ev.GetEnd()[0], ev.GetEnd()[1] };
                            int evEndHInt = int.Parse(new string(evEndHChar));

                            // Translate complete str time to an int with minutes only
                            char[] evStartMChar = { ev.GetStart()[3], ev.GetStart()[4] };
                            int evStartMInt = int.Parse(new string(evStartMChar));
                            char[] evEndMChar = { ev.GetEnd()[3], ev.GetEnd()[4] };
                            int evEndMInt = int.Parse(new string(evEndMChar));

                            bool isActive = ev.GetDay() == DateTime.Now.Day;

                            // Calc duration and add it to the HTML
                            int delay = (evEndHInt - evStartHInt) * 4 + (evEndMInt - evStartMInt) / 15;
                            body.AppendLine(string.Format(@"<td rowspan=""{0}"" class=""{3}"">{1} <br /> <i>{2}</i></td>", delay, ev.GetName(), ev.GetLoc(), isActive?"active":""));

                            // Remove the event
                            events.Remove(ev);

                            // Insert a cooldown to this day, the cd is the duration of the event
                            isFound = true;
                            delays[dcount] = delay;
                            break;
                        }
                    }
                    // If nothing was found, insert a blank cell
                    if (!isFound && delays[dcount] == 0) { body.AppendLine("<td></td>"); }
                }

                // End of line
                body.AppendLine("</tr>");
            }

            // Download & Merge the pre-generated head and foot with the body previously generated
            string head, foot;
            using (var wc = new WebClient())
                head = string.Format(wc.DownloadString(@"https://raw.githubusercontent.com/oxypomme/ICalToJpg/master/icalToHtmlToJpg/head.html"), dateStart, dateEnd-1, month);
            using (var wc = new WebClient())
                foot = wc.DownloadString(@"https://raw.githubusercontent.com/oxypomme/ICalToJpg/master/icalToHtmlToJpg/foot.html");
            finalHtml.Append(head + body + foot);

            /*== Create JPG/HTML File ==*/
            try
            {
                // jpeg file
                var converter = new HtmlConverter();
                var bytes = converter.FromHtmlString(finalHtml.ToString());
                File.WriteAllBytes("edt12.jpg", bytes);
            }
            catch (Exception)
            {
                // if started on the C: drive (beacause stfu) then create an html file
                using (FileStream fs = new FileStream("edt12.html", FileMode.Create))
                {
                    using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                    {
                        w.Write(finalHtml);
                    }
                }
            }

        }

        #endregion
    }
}
