using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using webapi.Models.Student;

namespace webapi.Controllers.Student
{
    public class StudentsController : ApiController
    {

        onlineQuranTutorEntities4 _context = new onlineQuranTutorEntities4();
        [HttpPost]
        public HttpResponseMessage addStudent()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var jsonChild = request.Form["data"];
                if (string.IsNullOrEmpty(jsonChild))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        success = false,
                        message = "Student data is required"
                    });
                }

                StudentDTO child = JsonConvert.DeserializeObject<StudentDTO>(jsonChild);
                var postedFile = request.Files["image"];
                string imagePath = null;


                var checkedEmail = _context.Users.Where(u => u.email == child.email).FirstOrDefault();
                if (checkedEmail != null)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        success = false,
                        message = "Email already exists"
                    });
                }

                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(postedFile.FileName);
                    string fileName = postedFile.FileName.ToString();
                    string folderPath = HttpContext.Current.Server.MapPath("~/UploadedStudentImages/");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    imagePath = Path.Combine(folderPath, child.email + fileName);
                    postedFile.SaveAs(imagePath);
                    child.profile = "/UploadedStudentImages/" + child.email + fileName.ToString();
                }

                _context.Users.Add(new User()
                {
                    name = child.name,
                    email = child.email,
                    password = child.password,
                    gender = child.gender,
                    dateOfBirth = child.dateOfBirth,
                    userType = child.userType,
                    country = child.country,
                    city = child.city,
                    timezone = child.timezone,
                    preferred_tutor = child.preferred_tutor,
                    profile = child.profile,
                    Subject = _context.Subjects.Where(s => s.subjectName == child.subject).FirstOrDefault(),
                });
                _context.SaveChanges();
                child.userid = _context.Users.OrderByDescending(u => u.userID).FirstOrDefault().userID;
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    success = true,
                    message = "Student added successfully!",
                    data = child
                });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    success = false,
                    message = "Something went wrong",
                    error = ex.Message
                });
            }
        }


        [HttpPost]
        public HttpResponseMessage addStudentSlots(StudentSlots studentSlot)
        {
            if (studentSlot == null)
            {
                return Request.CreateResponse();
            }
            _context.StudentSlots.Add(new StudentSlot()
            {
                Day = _context.Days.Where(d => d.dayID == studentSlot.dayid).FirstOrDefault(),
                Slot = _context.Slots.Where(s => s.slotID == studentSlot.slotid).FirstOrDefault(),
                User = _context.Users.Where(u => u.userID == studentSlot.studentid).FirstOrDefault(),
            });
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Slot Saved Successfully" });
        }





        [HttpPost]
        public HttpResponseMessage removeStudentSlots(StudentSlots studentSlot)
        {
            if (studentSlot == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var StudentSlots = _context.StudentSlots.Where(s => s.Slot.slotID == studentSlot.slotid && s.Day.dayID == studentSlot.dayid &&
            s.User.userID == studentSlot.studentid).FirstOrDefault();
            if (StudentSlots == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Invalid Request" });
            }

            _context.StudentSlots.Remove(StudentSlots);
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Slot Saved Successfully" });
        }


        [HttpGet]
        public HttpResponseMessage getAvailableTutorByStudentID(int studentID)
        {
            if (studentID <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new { success = false, message = "Invalid student ID." });
            }

            var student = _context.Users
                        .Where(u => u.userID == studentID)
                        .Select(s => new
                        {
                            subjectID = s.Subject.subjectID,
                            preferredGender = s.preferred_tutor
                        })
                        .FirstOrDefault();

            if (student == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound,
                    new { success = false, message = "Student not found." });
            }

            var genderPref = (student.preferredGender ?? "").Trim().ToLower();

            var availableTutors = (from ts in _context.TutorSlots
                                   join tSub in _context.TutorSubjects on ts.User.userID equals tSub.User.userID
                                   join u in _context.Users on ts.User.userID equals u.userID
                                   where
                                       tSub.Subject.subjectID == student.subjectID
                                       &&
                                       (
                                           genderPref == "male" ? u.gender.ToLower() == "male" :
                                           genderPref == "female" ? u.gender.ToLower() == "female" :
                                           true
                                       )
                                   select new
                                   {
                                       TutorID = u.userID,
                                       TutorName = u.name,
                                       TutorGender = u.gender
                                   })
                                   .Distinct()
                                   .ToList();

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                success = true,
                total = availableTutors.Count,
                tutors = availableTutors
            });
        }

    }
}
