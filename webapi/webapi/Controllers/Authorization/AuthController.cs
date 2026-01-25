using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace webapi.Controllers.Authorization
{
    public interface IAuthController
    {
        HttpResponseMessage LoginUser(string email, string password);
        HttpResponseMessage ValidateEmail(string email);
    }
    public class AuthController : ApiController, IAuthController
    {
        private readonly onlineQuranTutorEntities4 _context;

        public AuthController()
        {
            _context = new onlineQuranTutorEntities4();
        }
        [HttpPost]
        [Route("api/auth/login")]
        public HttpResponseMessage LoginUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new { message = "Email and password are required" });
            }

            var user = _context.Users
                .Where(u => u.email.ToLower() == email.ToLower()
                         && u.password == password)
                .Select(u => new
                {
                    u.userID,
                    u.name,
                    u.email,
                    u.userType,
                    u.dateOfBirth,
                    u.country,
                    u.city,
                    u.about,
                    u.gender,
                    u.profile
                })
                .FirstOrDefault();

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized,
                    new { message = "Invalid email or password" });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                message = "Login successful",
                user
            });
        }
        [HttpGet]
        [Route("api/auth/validate-email")]
        public HttpResponseMessage ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new { message = "Email is required" });
            }

            var exists = _context.Users
                .Where(u => u.email.ToLower() == email.ToLower()).FirstOrDefault().userID;

            if (exists < 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound,
                    new { message = "Email not found" });
            }

            return Request.CreateResponse(HttpStatusCode.OK,
                new { userid = exists, message = "Email exists" });
        }
        [HttpPost]
        [Route("api/auth/update-password")]
        public HttpResponseMessage UpdatePassword(int userid, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new { message = "Password is required" });
            }
            if (userid <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new { message = "Valid UserID is required" });
            }
            var user = _context.Users.Where(u => u.userID == userid).FirstOrDefault();
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound,
                    new { message = "User not found" });
            }
            user.password = password;
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK,
                new { message = "Password updated Successfully" });
        }
    }
}
