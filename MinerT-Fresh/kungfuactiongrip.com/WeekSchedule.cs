using System.Collections.Generic;

namespace MinerT.kungfuactiongrip.com
{
    public class DayOfWeek
    {
        public static DayOfWeek Monday = new DayOfWeek(0);
        public static DayOfWeek Tuesday = new DayOfWeek(1);
        public static DayOfWeek Wednesday = new DayOfWeek(2);
        public static DayOfWeek Thursday = new DayOfWeek(3);
        public static DayOfWeek Friday = new DayOfWeek(4);
        public static DayOfWeek Saturday = new DayOfWeek(5);
        public static DayOfWeek Sunday = new DayOfWeek(6);

        public int GetDayIndex()
        {
            return _pos;
        }

        private int _pos;
        private DayOfWeek(int pos)
        {
            _pos = pos;
        }
    }

    public class WeekSchedule
    {
        public List<DaySchedule> TheSchedules { get; set; }

        public WeekSchedule()
        {
            TheSchedules = new List<DaySchedule>(7);
        }
        public void AddDayOfWeek(DaySchedule ds)
        {
           TheSchedules.Add(ds);
        }

        public DaySchedule GetDayOfWeek(int day)
        {
            // Sunday is end for me ;)
            if (day == 0)
            {
                day = 6;
            }
            else
            {
                // correct for silly sunday 0 index
                day -= 1;
            }

            if (day < TheSchedules.Count)
            {
                return TheSchedules[day];
            }

            return null;
        }

    }
}
