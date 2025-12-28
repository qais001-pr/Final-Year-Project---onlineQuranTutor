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
                var jsonChild = request.Form["student"];
                if (string.IsNullOrEmpty(jsonChild))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        success = false,
                        message = "Student data is required"
                    });
                }

                StudentDTO child = JsonConvert.DeserializeObject<StudentDTO>(jsonChild);
                var postedFile = request.Files["studentImage"];
                string imagePath = null;

                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(postedFile.FileName);
                    string fileName = postedFile.FileName.ToString() + extension;
                    string folderPath = HttpContext.Current.Server.MapPath("~/UploadedStudentImages/");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    imagePath = Path.Combine(folderPath, fileName);
                    postedFile.SaveAs(imagePath);
                    child.profile = "/UploadedStudentImages/" + fileName.ToString();
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
                    Subject = _context.Subjects.Where(s => s.subjectID == child.subjectID).FirstOrDefault(),
                });
                _context.SaveChanges();
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
    }
}
