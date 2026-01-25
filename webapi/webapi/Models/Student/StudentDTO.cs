using System;

namespace webapi.Models.Student
{
    public class StudentDTO
    {
        public int userid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string gender { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string userType { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string timezone { get; set; }
        public string profile { get; set; }
        public string preferred_tutor { get; set; }
        public string subject { get; set; }
    }
}