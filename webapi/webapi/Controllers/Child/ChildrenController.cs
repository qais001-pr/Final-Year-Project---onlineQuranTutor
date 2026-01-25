using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using webapi.Models.Child;

namespace webapi.Controllers.Child
{
    public class ChildrenController : ApiController
    {
        onlineQuranTutorEntities4 _context = new onlineQuranTutorEntities4();
        [HttpPost]
        public HttpResponseMessage addChild()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var jsonChild = request.Form["children"];
                if (string.IsNullOrEmpty(jsonChild))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        success = false,
                        message = "Children data is required"
                    });
                }

                ChildDTO child = JsonConvert.DeserializeObject<ChildDTO>(jsonChild);
                var postedFile = request.Files["childImage"];
                string imagePath = null;

                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(postedFile.FileName);
                    string fileName = postedFile.FileName.ToString();
                    string folderPath = HttpContext.Current.Server.MapPath("~/UploadedChildImages/");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    imagePath = Path.Combine(folderPath, child.email + fileName);
                    postedFile.SaveAs(imagePath);
                    child.profile = "/UploadedChildImages/" + child.email + fileName.ToString();
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

                _context.children.Add(new child()
                {
                    User = _context.Users.Where(u => u.email.ToLower() == child.email.ToLower()).FirstOrDefault(),
                    User1 = _context.Users.Where(u => u.userID == child.guardianID).FirstOrDefault(),
                });
                _context.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    success = true,
                    message = "Child added successfully!",
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
