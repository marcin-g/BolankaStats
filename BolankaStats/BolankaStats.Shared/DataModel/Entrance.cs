using BolankaStats.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BolankaStats.DataModel
{
    class Entrance
    {
        public string UniqueId { get; private set; }
        public DateTime Date { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public string Hour { get; private set; }
        public string DeviceId { get; private set; }
        public int PeopleNumber{ get; private set; }
        public bool Entered{ get; private set; }

        public static int MAX_PEOPLE { get; private set; }

        public Entrance(String UniqueId, DateTime Date, string DeviceId, int PeopleNumber, bool Entered)
        {
            this.UniqueId = UniqueId;
            this.Date = Date;
            this.DeviceId = DeviceId;
            this.PeopleNumber = PeopleNumber;
            this.Entered = Entered;
            this.DayOfWeek = Date.DayOfWeek;
            this.Hour = Date.ToString("HH:mm");
            if (MAX_PEOPLE < PeopleNumber)
            {
                MAX_PEOPLE = PeopleNumber;
            }
        }
        public Entrance(DateTime Date, int PeopleNumber, bool Entered)
        {

            this.DeviceId = CommonHelper.DEVICE_ID;
            this.Date = Date;
            this.PeopleNumber = PeopleNumber;
            this.Entered = Entered;
            this.DayOfWeek = Date.DayOfWeek;
            this.Hour = Date.ToString("HH:mm");
            if (MAX_PEOPLE < PeopleNumber)
            {
                MAX_PEOPLE = PeopleNumber;
            }
        }
    }
}
