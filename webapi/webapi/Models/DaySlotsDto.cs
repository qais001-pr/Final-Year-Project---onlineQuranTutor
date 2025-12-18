using System.Collections.Generic;

namespace webapi.Models
{

    public class DaySlotsDto
    {
        public int DayID { get; set; }
        public string DayName { get; set; }
        public List<SlotDto> Slots { get; set; }
    }
}