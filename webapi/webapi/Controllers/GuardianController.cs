using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using webapi.Models;
namespace webapi.Controllers
{
    public class GuardianController : ApiController
    {
        onlineQuranTutorEntities4 _context = new onlineQuranTutorEntities4();
        [HttpPost]
        public HttpResponseMessage addChild()
        {
            var request = HttpContext.Current.Request;

            var json = request.Form["child"];


            if (string.IsNullOrEmpty(json))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Child JSON missing");

            ChildDTO childdata = JsonConvert.DeserializeObject<ChildDTO>(json);

            if (childdata.guardianID <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    statusCode = HttpStatusCode.OK,
                    message = "Guardian ID Error"
                });
            }



            if (request.Files.Count > 0)
            {
                var postedFile = request.Files["image"];

                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    var extension = Path.GetExtension(postedFile.FileName);
                    string fileName = childdata.email + extension;

                    string serverPath = HttpContext.Current.Server.MapPath("~/images/");
                    string fullPath = Path.Combine(serverPath, fileName);

                    if (!Directory.Exists(serverPath))
                        Directory.CreateDirectory(serverPath);

                    postedFile.SaveAs(fullPath);

                    childdata.profilePicture = fileName;
                }
            }









            if (childdata == null ||
                   string.IsNullOrWhiteSpace(childdata.name) ||
                   string.IsNullOrWhiteSpace(childdata.email) ||
                   string.IsNullOrWhiteSpace(childdata.password) ||
                   string.IsNullOrWhiteSpace(childdata.gender) ||
                   childdata.dateOfBirth.ToString() == null ||
                   string.IsNullOrWhiteSpace(childdata.userType) ||
                   string.IsNullOrWhiteSpace(childdata.city) ||
                   string.IsNullOrWhiteSpace(childdata.timezone) ||
                   string.IsNullOrWhiteSpace(childdata.country))
            {
                return Request.CreateResponse(
                    HttpStatusCode.NotAcceptable, new
                    {
                        message = "All fields are required"
                    });
            }
            var child = _context.Users.Where(user => user.email == childdata.email).FirstOrDefault();
            if (child != null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadGateway, new
                    {
                        message = $"Child already exists with this {childdata.email}"
                    });
            }

            _context.Users.Add(new User()
            {
                name = childdata.name,
                email = childdata.email,
                password = childdata.password,
                gender = childdata.gender,
                country = childdata.country,
                timezone = childdata.timezone,
                city = childdata.city,
                dateOfBirth = childdata.dateOfBirth,
                userType = childdata.userType,
                profilePicture = childdata.profilePicture,
                createdAt = DateTime.Now,
            });
            _context.SaveChanges();
            var userdata = from user in _context.Users
                           where user.email == childdata.email
                           select new
                           {
                               user.userID,
                               user.email,
                               user.password,
                               user.gender,
                               user.country,
                               user.userType,
                               user.profilePicture,
                               guardianID = childdata.guardianID,
                           };

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                message = "Guardian login successful"
            });
        }


        [HttpPost]
        public HttpResponseMessage AddchildSubjectsAndpreferred(ChildDTOSubject childdata)
        {
            if (childdata.guardianID <= 0 || childdata.userid <= 0 ||
                string.IsNullOrEmpty(childdata.preferred_teacher) || childdata.subjectid <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {

                });
            }
            var guardian = from u in _context.Users
                           where u.userID == childdata.userid
                           select u.userID;



            _context.Students.Add(new Student()
            {
                preferred_teacher = childdata.preferred_teacher,
                Subject = _context.Subjects.Where(s => s.subjectID == childdata.subjectid).FirstOrDefault(),
                User = _context.Users.Where(u => u.userID == childdata.userid).FirstOrDefault(),
                createdAt = DateTime.Now,
            });
            _context.SaveChanges();
            var studentid = _context.Students.Where(s => s.User.userID == childdata.userid)
                .Select(s => s.studentID).FirstOrDefault();

            _context.Children.Add(new Child()
            {
                studentID = studentid,
                Guardian = _context.Guardians.Where(g => g.guardianID == childdata.guardianID).FirstOrDefault(),
            });
            var result =
                (from st in _context.Students
                join u in _context.Users on st.User.userID equals u.userID
                where u.userID == childdata.userid
                select new
                {
                    st.studentID,
                    u.userID,
                    u.name,
                    u.profilePicture,
                    u.email,
                    u.timezone,
                    u.password,
                    u.city,
                    u.country,
                    u.gender,
                    u.userType,
                }).FirstOrDefault();
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                data = result,
                message = "Child Added Successfully"
            });
        }
    }

    public class ChildDTOSubject
    {
        public int guardianID { get; set; }
        public int userid { get; set; }
        public string preferred_teacher { get; set; }
        public int subjectid { get; set; }

    }

}

