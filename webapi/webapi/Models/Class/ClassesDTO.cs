using System;

namespace webapi.Models.Class
{
    public class ClassesDTO
    {
        public int ClassID { get; set; }

        public int StudentID { get; set; }

        public int TutorID { get; set; }

        public int SlotID { get; set; }

        public int SubjectID { get; set; }

        public int LessonPlanID { get; set; }

        public int StudentRequestTutorID { get; set; }

        public string Status { get; set; } = "Scheduled";

        public int Corrections { get; set; }

        public DateTime ClassDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}