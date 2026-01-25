using System;
using System.Collections.Generic;
using webapi.Models.Subject;

namespace webapi.Models.Tutor
{
    public class TutorDTO
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
        public List<SubjectDTO> subjectList { get; set; }
    }
}