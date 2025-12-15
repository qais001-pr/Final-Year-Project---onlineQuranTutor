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
    public class UsersController : ApiController
    {
        onlineQuranTutorEntities4 _context = new onlineQuranTutorEntities4();
        [HttpPost]
        public HttpResponseMessage addUser()
        {
            var request = HttpContext.Current.Request;

            var json = request.Form["user"];


            if (string.IsNullOrEmpty(json))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Student JSON missing");

            SignUpUser user = JsonConvert.DeserializeObject<SignUpUser>(json);





            if (request.Files.Count > 0)
            {
                var postedFile = request.Files["image"];

                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    var extension = Path.GetExtension(postedFile.FileName);
                    string fileName = user.email + extension;

                    string serverPath = HttpContext.Current.Server.MapPath("~/images/");
                    string fullPath = Path.Combine(serverPath, fileName);

                    if (!Directory.Exists(serverPath))
                        Directory.CreateDirectory(serverPath);

                    postedFile.SaveAs(fullPath);

                    user.ProfilePicture = fileName;
                }
            }







            if (user == null ||
                string.IsNullOrWhiteSpace(user.name) ||
                string.IsNullOrWhiteSpace(user.email) ||
                string.IsNullOrWhiteSpace(user.password) ||
                string.IsNullOrWhiteSpace(user.gender) ||
                user.dateOfBirth == DateTime.MinValue ||
                string.IsNullOrWhiteSpace(user.userType) ||
                string.IsNullOrWhiteSpace(user.city) ||
                string.IsNullOrWhiteSpace(user.timezone) ||
                string.IsNullOrWhiteSpace(user.country))
            {
                return Request.CreateResponse(
                    HttpStatusCode.NotAcceptable, new
                    {
                        message = "All fields are required"
                    });
            }

            var checkUser = _context.Users.Where(u => u.email == user.email).FirstOrDefault();
            if (checkUser != null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.NotAcceptable, new
                    {
                        message = $"User with email {user.email} already exists"
                    });
            }
            var newUser = new User
            {
                name = user.name,
                email = user.email,
                password = user.password,
                gender = user.gender,
                userType = user.userType,
                dateOfBirth = user.dateOfBirth,
                city = user.city,
                timezone = user.timezone,
                country = user.country,
                profilePicture = user.ProfilePicture,
                createdAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
            if (user.userType.Equals("Guardian"))
            {
                var us = _context.Users.FirstOrDefault(u => u.email == newUser.email);
                _context.Guardians.Add(new Guardian
                {
                    User = us
                });
                _context.SaveChanges();
                var registereduser = from u in _context.Users
                                     where u.email == newUser.email
                                     join g in _context.Guardians on u.userID equals g.User.userID
                                     select new
                                     {
                                         g.guardianID,
                                         u.userID,
                                         u.name,
                                         u.gender,
                                         u.dateOfBirth,
                                         u.email,
                                         u.password,
                                         u.country,
                                         u.city,
                                         u.timezone,
                                         u.profilePicture,
                                     };
                return Request.CreateResponse(
                    HttpStatusCode.OK, new
                    {

                        message = "Guardian added successfully",
                        data = registereduser,
                    });
            }
            if (user.userType.Equals("Tutor") &&
                user.tutorSubjects != null &&
                user.tutorSubjects.Count > 0)
            {
                var tutor = new Tutor
                {
                    User = newUser,
                    about = ""
                };

                _context.Tutors.Add(tutor);
                _context.SaveChanges();
                foreach (var subject in user.tutorSubjects)
                {
                    var subj = _context.Subjects.FirstOrDefault(s => s.subjectID == subject.subjectid);
                    if (subj != null)
                    {
                        _context.TutorSubjects.Add(new TutorSubject
                        {
                            Tutor = tutor,
                            Subject = subj
                        });
                    }
                }

                _context.SaveChanges();
                var registereduser = from u in _context.Users
                                     where u.email == newUser.email
                                     join t in _context.Tutors on u.userID equals t.User.userID
                                     select new
                                     {
                                         u.userID,
                                         t.tutorID,
                                         u.name,
                                         u.gender,
                                         u.dateOfBirth,
                                         u.email,
                                         u.password,
                                         u.country,
                                         u.city,
                                         u.timezone,
                                         u.profilePicture

                                     };
                return Request.CreateResponse(
                    HttpStatusCode.OK, new
                    {
                        message = "Tutor added successfully",
                        data = registereduser,
                    });
            }
            if (user.userType.Equals("Student"))
            {
                var us = _context.Users.FirstOrDefault(u => u.email == newUser.email);
                _context.SaveChanges();
                var registereduser = from u in _context.Users
                                     where u.email == newUser.email
                                     select new
                                     {
                                         u.userID,
                                         u.name,
                                         u.gender,
                                         u.dateOfBirth,
                                         u.email,
                                         u.password,
                                         u.country,
                                         u.city,
                                         u.timezone,
                                         u.profilePicture,
                                     };
                return Request.CreateResponse(
                    HttpStatusCode.OK, new
                    {

                        message = "Student added successfully",
                        data = registereduser,
                    });
            }
            return Request.CreateResponse(
                HttpStatusCode.OK, new
                {
                    message = "User added successfully"
                });
        }


    }
}


