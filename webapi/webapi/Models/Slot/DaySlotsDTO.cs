using System.Collections.Generic;

namespace webapi.Models.Slot
{
    public class DaySlotsDto
    {
        public int DayID { get; set; }
        public string DayName { get; set; }
        public List<SlotDto> Slots { get; set; }
    }
    public class SlotDto
    {
        public int DayID { get; set; }
        public int SlotID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Status { get; set; }
    }
}