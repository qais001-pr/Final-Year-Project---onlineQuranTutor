using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI.WebControls;
using webapi.Models;
namespace webapi.Controllers
{
    public class AuthController : ApiController
    {
        onlineQuranTutorEntities4 _context = new onlineQuranTutorEntities4();
        [HttpPost]
        public HttpResponseMessage checkEmail(string email)
        {

            var user = _context.Users.Where(u => u.email == email).FirstOrDefault();
            if (user != null)

                return Request.CreateResponse(
                    HttpStatusCode.OK, new
                    {
                        message = "Email checked successfully"
                    });

            return Request.CreateResponse(
                HttpStatusCode.OK, new
                {
                    message = "Email checked successfully"
                });
        }


        [HttpPut]
        public HttpResponseMessage updatePassword(int userID, string newPassword)
        {
            var user = _context.Users.Where(u => u.userID == userID).FirstOrDefault();
            if (user != null)
            {
                user.password = newPassword;
                _context.SaveChanges();
                return Request.CreateResponse(
                    HttpStatusCode.OK, new
                    {
                        message = "Password updated successfully"
                    });
            }
            return Request.CreateResponse(
                HttpStatusCode.NotFound, new
                {
                    message = "User not found"
                });
        }
        [HttpPost]
        public HttpResponseMessage loginUser(Auth auth)
        {
            if (auth == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    message = "Invalid request"
                });
            }

            var user = _context.Users
                .Where(u => u.email == auth.email && u.password == auth.password)
                .Select(u => new
                {
                    u.userID,
                    u.userType
                })
                .FirstOrDefault();

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    message = "Login Failed"
                });
            }

            switch (user.userType.ToLower())
            {
                case "student":
                    {
                        var result = (from u in _context.Users
                                      join st in _context.Students on u.userID equals st.User.userID
                                      where u.userID == user.userID
                                      select new
                                      {
                                          u.userID,
                                          u.userType,
                                          u.name,
                                          u.email,
                                          u.timezone,
                                          u.country,
                                          u.city,
                                          u.dateOfBirth,
                                          u.createdAt,
                                          st.studentID,
                                          subjectID = st.Subject.subjectID,
                                          st.preferred_teacher
                                      }).FirstOrDefault();

                        return Request.CreateResponse(HttpStatusCode.OK, new { data = result });
                    }

                case "guardian":
                    {
                        var result = (from u in _context.Users
                                      join g in _context.Guardians on u.userID equals g.User.userID
                                      where u.userID == user.userID
                                      select new
                                      {
                                          u.userID,
                                          u.userType,
                                          u.name,
                                          u.email,
                                          u.timezone,
                                          u.country,
                                          u.city,
                                          u.dateOfBirth,
                                          u.createdAt,
                                          g.guardianID,

                                          children = (
                                              from ch in _context.Children
                                              join st in _context.Students on ch.studentID equals st.studentID
                                              join uu in _context.Users on st.User.userID equals uu.userID
                                              where ch.Guardian.guardianID == g.guardianID
                                              select new
                                              {
                                                  uu.userID,
                                                  uu.name,
                                                  uu.email,
                                                  st.studentID,
                                                  st.preferred_teacher
                                              }
                                          ).ToList()
                                      }).FirstOrDefault();

                        return Request.CreateResponse(HttpStatusCode.OK, new { data = result });
                    }

                case "tutor":
                    {
                        var result = (from u in _context.Users
                                      join t in _context.Tutors on u.userID equals t.User.userID
                                      where u.userID == user.userID
                                      select new
                                      {
                                          u.userID,
                                          u.userType,
                                          u.name,
                                          u.email,
                                          u.timezone,
                                          u.country,
                                          u.city,
                                          u.createdAt,
                                          t.tutorID,
                                      }).FirstOrDefault();

                        return Request.CreateResponse(HttpStatusCode.OK, new { data = result });
                    }

                case "child":
                    {
                        var result = (from u in _context.Users
                                      join st in _context.Students on u.userID equals st.User.userID
                                      where u.userID == user.userID
                                      select new
                                      {
                                          u.userID,
                                          u.userType,
                                          u.name,
                                          u.email,
                                          st.studentID,
                                          st.preferred_teacher
                                      }).FirstOrDefault();

                        return Request.CreateResponse(HttpStatusCode.OK, new { data = result });
                    }

                default:
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        message = "Invalid user type"
                    });
            }
        }
    }

}
