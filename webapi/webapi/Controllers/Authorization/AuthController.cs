using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using webapi.Models.Authorization;

namespace webapi.Controllers.Authorization
{
    public class AuthController : ApiController
    {
        onlineQuranTutorEntities4 _context = new onlineQuranTutorEntities4();


        [HttpGet]
        public HttpResponseMessage loginUser(Login login)
        {

            if (string.IsNullOrEmpty(login.email) || string.IsNullOrEmpty(login.password))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var user = _context.Users.Where(u => u.email.ToLower() == login.email && u.password.ToLower() == login.password.ToLower()).FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                user,
                message = "Login User Successfull"
            });
        }



        [HttpGet]
        public HttpResponseMessage validateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Invalid Email" });
            }
            var users = _context.Users.Where(u => u.email.ToLower() == email.ToLower()).FirstOrDefault();
            if (users == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Email Not Found" });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { user = users, message = "Email Found Successfully" });
        }


        [HttpPut]
        public HttpResponseMessage updatePassword(UpdatePass update)
        {
            if (string.IsNullOrEmpty(update.userid.ToString()) || string.IsNullOrEmpty(update.password))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Invalid Password or Userid" });
            }
            var users = _context.Users.Where(u => u.userID == update.userid).FirstOrDefault();
            users.password = update.password;
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, new { user = users, message = "Password Updated  Successfully" });
        }
    }
}
