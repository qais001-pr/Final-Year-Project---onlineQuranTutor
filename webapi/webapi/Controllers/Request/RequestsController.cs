using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using webapi.Models.Request;

namespace webapi.Controllers.Request
{
    public interface IRequestsController
    {
        HttpResponseMessage requestToTutor(RequestDTO data);
    }

    public class RequestsController : ApiController, IRequestsController
    {
        private readonly onlineQuranTutorEntities4 _context = new onlineQuranTutorEntities4();

        [HttpPost]
        public HttpResponseMessage requestToTutor(RequestDTO data)
        {
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new { success = false, message = "Invalid request data." });
            }

            var existingRequest = _context.StudentTutorRequests
                .Where(r => r.Subject.subjectID == data.subjectId
                         && r.User.userID == data.studentId
                         && r.User1.userID == data.tutorId
                         && r.surah.Id == data.surahID)
                .FirstOrDefault();

            if (existingRequest != null)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new { success = false, message = "Request already sent to this tutor." });
            }
            var newRequest = new StudentTutorRequest
            {
                User = _context.Users.Find(data.studentId),
                User1 = _context.Users.Find(data.tutorId),
                Subject = _context.Subjects.Find(data.subjectId),
                surah = _context.surahs.Find(data.surahID),
                status = "Pending",
                createdAt = DateTime.Now,
                updatedAt = DateTime.Now,
            };

            _context.StudentTutorRequests.Add(newRequest);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK,
                new { success = true, message = "Request sent successfully.", request = newRequest });
        }
    }
}
