using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using webapi.Models.Student;
using webapi.Models.Tutor;

namespace webapi.Controllers.Tutor
{
    public class TutorController : ApiController
    {
        onlineQuranTutorEntities4 _context = new onlineQuranTutorEntities4();
        [HttpPost]
        public HttpResponseMessage addTutor()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var jsonTutor = request.Form["tutor"];
                if (string.IsNullOrEmpty(jsonTutor))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        success = false,
                        message = "Tutor data is required"
                    });
                }

                TutorDTO tutor = JsonConvert.DeserializeObject<TutorDTO>(jsonTutor);
                if (string.IsNullOrEmpty(tutor.name.Trim()) ||
                    string.IsNullOrEmpty(tutor.email.Trim()) ||
                    string.IsNullOrEmpty(tutor.password.Trim()) ||
                    string.IsNullOrEmpty(tutor.gender.Trim()) ||
                    string.IsNullOrEmpty(tutor.dateOfBirth.ToString()) ||
                    string.IsNullOrEmpty(tutor.userType.Trim()) ||
                    string.IsNullOrEmpty(tutor.country.Trim()) ||
                    string.IsNullOrEmpty(tutor.city.Trim()) ||
                    string.IsNullOrEmpty(tutor.timezone.Trim()) ||
                    tutor.subjectList == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Tutor Data Invalid" });
                }

                var checkTutor = _context.Users.Where(u => u.email.ToLower() == tutor.email.ToLower()).FirstOrDefault();
                if (checkTutor != null)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        success = false,
                        message = "Email Already Exists"
                    });
                }
                var postedFile = request.Files["tutorImage"];
                string imagePath = null;

                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(postedFile.FileName);
                    string fileName = postedFile.FileName.ToString();
                    string folderPath = HttpContext.Current.Server.MapPath("~/UploadedTutorImages/");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    imagePath = Path.Combine(folderPath, tutor.email + fileName);
                    postedFile.SaveAs(imagePath);
                    tutor.profile = "/UploadedTutorImages/" + tutor.email + fileName.ToString();
                }

                _context.Users.Add(new User()
                {
                    name = tutor.name,
                    email = tutor.email,
                    password = tutor.password,
                    gender = tutor.gender,
                    dateOfBirth = tutor.dateOfBirth,
                    userType = tutor.userType,
                    country = tutor.country,
                    city = tutor.city,
                    timezone = tutor.timezone,
                    profile = tutor.profile,
                });
                _context.SaveChanges();

                foreach (var item in tutor.subjectList)
                {
                    _context.TutorSubjects.Add(new TutorSubject()
                    {
                        Subject = _context.Subjects.Where(s => s.subjectName == item.name).FirstOrDefault(),
                        User = _context.Users.Where(u => u.email.ToLower() == tutor.email.ToLower()).FirstOrDefault(),
                    });
                }
                _context.SaveChanges();
                tutor.userid = _context.Users.Where(u => u.email.ToLower() == tutor.email.ToLower()).FirstOrDefault().userID;
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    success = true,
                    message = "Tutor added successfully!",
                    data = tutor
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
        public HttpResponseMessage addTutorSlots(TutorSlots tutorSlot)
        {
            if (tutorSlot == null)
            {
                return Request.CreateResponse();
            }
            _context.TutorSlots.Add(new TutorSlot()
            {
                Day = _context.Days.Where(d => d.dayID == tutorSlot.dayid).FirstOrDefault(),
                Slot = _context.Slots.Where(s => s.slotID == tutorSlot.slotid).FirstOrDefault(),
                User = _context.Users.Where(u => u.userID == tutorSlot.tutorid).FirstOrDefault(),
                status = "available",
            });
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Slot Saved Successfully" });
        }





        [HttpDelete]
        public HttpResponseMessage removeTutorSlots(TutorSlots tutorSlot)
        {
            if (tutorSlot == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var StudentSlots = _context.StudentSlots.Where(s => s.Slot.slotID == tutorSlot.slotid && s.Day.dayID == tutorSlot.dayid &&
            s.User.userID == tutorSlot.tutorid).FirstOrDefault();
            if (StudentSlots == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Invalid Request" });
            }

            _context.StudentSlots.Remove(StudentSlots);
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Slot Saved Successfully" });
        }
    }
}
