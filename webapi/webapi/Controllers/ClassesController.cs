using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using webapi.Models;

namespace webapi.Controllers
{
    public class ClassesController : ApiController
    {
        onlineQuranTutorEntities4 _context = new onlineQuranTutorEntities4();


        [HttpPost]
        public HttpResponseMessage updateClass(UpdateClass classDTO)
        {
            if (string.IsNullOrEmpty(classDTO.status) || classDTO.classID <= 0 || classDTO.corrections < 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Invalid Data" });
            }
            var classData = _context.Classes.Where(c => c.classID == classDTO.classID).FirstOrDefault();
            classData.status = classDTO.status;
            classData.corrections = classDTO.corrections;
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "updated successfully" });
        }


        [HttpGet]
        public HttpResponseMessage getclassesDataByID(int classid)
        {
            if (classid <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new
                    {
                        message = "Invalid Data",
                    });
            }
            var result = (
    from c in _context.Classes
    where c.classID == classid
    select new
    {
        c.classID,
        c.classDate,
        c.status,
        c.corrections,
        Ayats = (
            from l in _context.Lessons
            join q in _context.Qurans on l.Quran.ID equals q.ID
            where l.LessonPlan.lessonPlanID == c.LessonPlan.lessonPlanID
            select new
            {
                q.ID,
                q.AyahText
            }
        ).ToList()
    }
).FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                data = result,
                totalAyat = result.Ayats.Count,
                message = "Data Collected Successfully"
            });
        }
    }
}
