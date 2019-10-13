using System;

namespace icalToHtmlToJpg
{
    class Event
    {
        #region Private Fields

        ///  <summary>
        /// The name of the event
        ///  </summary>
        private string name = "";

        ///  <summary>
        /// The location of the event
        ///  </summary>
        private string loc = "";

        ///  <summary>
        /// The start time of the event
        ///  </summary>
        private string start = "";

        ///  <summary>
        /// The end time of the event
        ///  </summary>
        private string end = "";

        ///  <summary>
        /// The day of the event
        ///  </summary>
        private int day = 1;

        ///  <summary>
        /// The month of the event
        ///  </summary>
        private int month = 1;

        #endregion

        #region Public Methods

        ///  <summary>
        ///  Create an event with a name, a start time, a end time and a localisation.
        ///  </summary>
        ///  <param name="name">The name of the event.</param>
        ///  <param name="start">The start time of the event.</param>
        ///  <param name="end">The end time of the event.</param>
        ///  <param name="loc">The localisation of the event.</param>
        ///  <remarks>
        ///  Raw input only !
        ///  </remarks>
        public void CreateEvent(string name, string start, string end, string loc)
        {
            this.name = name;
            this.loc = loc;
            this.start = start;
            this.end = end;
        }


        ///  <summary>
        ///  Clean variables to be ready to extract.
        ///  Also create the day and the month.
        ///  </summary>
        public void CleanEvent()
        {
            /*== Clean the name ==*/
            string[] splittedName = new string[5];
            string[] separator = { ":", " - " };
            splittedName = this.name.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            this.name = splittedName[1];

            /*== Clean the localisation ==*/
            string[] splitted = new string[2];
            splitted = this.loc.Split(':', StringSplitOptions.RemoveEmptyEntries);
            this.loc = splitted[1];

            /*== Clean the start time ==*/
            splitted = this.start.Split(':', StringSplitOptions.RemoveEmptyEntries);
            splitted = splitted[1].Split('T', StringSplitOptions.RemoveEmptyEntries);
            char[] sHours = { splitted[1][0], splitted[1][1] };
            char[] sMins = { splitted[1][2], splitted[1][3] };
            this.start = new string(sHours) + ":" + new string(sMins);

            /*== Create date day ==*/
            char[] dayChar = { splitted[0][6], splitted[0][7] };
            this.day = int.Parse(new string(dayChar));

            /*== Create date month ==*/
            char[] monthChar = { splitted[0][4], splitted[0][5] };
            this.month = int.Parse(new string(monthChar));

            /*== Clean end time ==*/
            splitted = this.end.Split(':', StringSplitOptions.RemoveEmptyEntries);
            splitted = splitted[1].Split('T', StringSplitOptions.RemoveEmptyEntries);
            char[] eHours = { splitted[1][0], splitted[1][1] };
            char[] eMins = { splitted[1][2], splitted[1][3] };
            this.end = new string(eHours) + ":" + new string(eMins);
        }

        #endregion

        #region Gets Fields

        ///  <summary>
        ///  Return the name
        ///  </summary>
        ///  <returns>
        ///  Name of the event
        ///  </returns>
        public string GetName()
        {
            return this.name;
        }

        ///  <summary>
        ///  Return the localisation
        ///  </summary>
        ///  <returns>
        ///  localisation of the event
        ///  </returns>
        public string GetLoc()
        {
            return this.loc;
        }

        ///  <summary>
        ///  Return the start time
        ///  </summary>
        ///  <returns>
        ///  Start time of the event
        ///  </returns>
        public string GetStart()
        {
            return this.start;
        }

        ///  <summary>
        ///  Return the end time
        ///  </summary>
        ///  <returns>
        ///  End time of the event
        ///  </returns>
        public string GetEnd()
        {
            return this.end;
        }

        ///  <summary>
        ///  Return the day
        ///  </summary>
        ///  <returns>
        ///  Day of the event
        ///  </returns>
        public int GetDay()
        {
            return this.day;
        }

        ///  <summary>
        ///  Return the month
        ///  </summary>
        ///  <returns>
        ///  Month of the event
        ///  </returns>
        public int GetMonth()
        {
            return this.month;
        }

        #endregion
    }
}
