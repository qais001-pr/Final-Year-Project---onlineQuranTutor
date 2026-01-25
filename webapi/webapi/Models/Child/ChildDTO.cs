using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webapi.Models.Child
{
    public class ChildDTO
    {
        public int guardianID { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string gender { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string userType { get; set; }
        public string country { get; set; }
        public string city  { get; set; }
        public string timezone { get; set; }
        public string profile { get; set; }
        public string preferred_tutor { get; set; }
        public string subject { get; set; }
    }
}