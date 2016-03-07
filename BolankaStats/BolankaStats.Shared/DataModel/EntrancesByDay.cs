using System;
using System.Collections.Generic;
using System.Text;

namespace BolankaStats.DataModel
{
    class EntrancesByDay
    {
        public string DayOfWeek { get; set; }
        public IEnumerable<Entrance> DayEntrances { get; set; }
        public EntrancesByDay(string DayOfWeek, IEnumerable<Entrance> DayEntrances)
        {
            this.DayEntrances = DayEntrances;
            this.DayOfWeek = DayOfWeek;
        }
    }
}
