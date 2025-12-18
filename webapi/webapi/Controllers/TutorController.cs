using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using webapi.Models;

namespace webapi.Controllers
{
    public class TutorController : ApiController
    {
        onlineQuranTutorEntities4 _context = new onlineQuranTutorEntities4();

        [HttpPost]
        public HttpResponseMessage addTutorSlots(TutorSlots tutorSlot)
        {
            if (tutorSlot.tutorID <= 0 || tutorSlot.slotid <= 0 || tutorSlot.dayid <= 0 ||
                string.IsNullOrWhiteSpace(tutorSlot.status))
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest, new
                    {
                        message = "Invalid input data"
                    });
            }

            var tutor = _context.TutorSlots.Where(x =>
                x.Tutor.tutorID == tutorSlot.tutorID &&
                x.Slot.slotID == tutorSlot.slotid &&
                x.Day.dayID == tutorSlot.dayid
            ).FirstOrDefault();

            if (tutor != null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest, new
                    {
                        message = $"A slot already exists for Tutor {tutorSlot.tutorID} on day {tutorSlot.dayid}."
                    });
            }

            _context.TutorSlots.Add(new TutorSlot()
            {
                Tutor = _context.Tutors.Find(tutorSlot.tutorID),
                Day = _context.Days.Find(tutorSlot.dayid),
                Slot = _context.Slots.Find(tutorSlot.slotid),
                status = "available"
            });

            _context.SaveChanges();

            return Request.CreateResponse(
                HttpStatusCode.OK, new
                {
                    message = "Tutor Slots added Successfully"
                });
        }




        [HttpGet]
        public HttpResponseMessage getTutorDetails(int id)
        {
            if (id <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    message = "Invalid tutor ID"
                });
            }

            var tutor = (
                from t in _context.Tutors
                         where t.tutorID == id
                         select new
                         {
                             t.tutorID,
                             t.about,
                             userID = t.User.userID,
                             t.User.name,
                             t.User.email,
                             t.User.country,
                             t.User.city,
                             t.User.timezone,

                             subjects = (from ts in _context.TutorSubjects
                                         where ts.Tutor.tutorID == t.tutorID
                                         select new
                                         {
                                             ts.Subject.subjectID,
                                             ts.Subject.subjectName
                                         }),
                             ratings = (from c in _context.Classes
                                        join r in _context.Reviews on c.classID equals r.Class.classID
                                        where c.Tutor.tutorID == t.tutorID
                                        group r by c.Tutor.tutorID into g
                                        select new
                                        {
                                            AverageRating = g.Average(x => x.Rating),
                                            ReviewCount = g.Count()
                                        }).FirstOrDefault()
                         })
            .FirstOrDefault();


            if (tutor == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    message = "Tutor not found"
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                data = tutor,
                message = "Data collected successfully"
            });
        }


        [HttpGet]
        public HttpResponseMessage getClassesByUsingTutorID(int tutorID)
        {

            if (tutorID <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    message = "Invalid tutor ID"
                });
            }

            var result = (
                from c in _context.Classes
                join Tutor in _context.Tutors on c.Tutor.tutorID equals Tutor.tutorID

                join s in _context.Slots on c.Slot.slotID equals s.slotID

                join d in _context.Days on c.Day.dayID equals d.dayID

                join st in _context.Students on c.Student.studentID equals st.studentID

                join u in _context.Users on st.User.userID equals u.userID

                join sub in _context.Subjects on

                c.Subject.subjectID equals sub.subjectID

                where c.Tutor.tutorID == tutorID
                orderby c.classDate
                select new
                {
                    tutorID = Tutor.tutorID,
                    userID = u.userID,
                    studentName = u.name,
                    StudentTimeZone = u.timezone,
                    gender = u.gender,
                    studentProfilePicture = u.profilePicture,
                    usertype = u.userType,
                    starttime = s.startTime,
                    endtime = s.endTime,
                    dayOfClass = d.dayName,
                    Subject = sub.subjectName,
                    status = c.status,
                    classDate = c.classDate

                }
                ).ToList();
            return Request.CreateResponse(
                HttpStatusCode.OK, new
                {
                    data = result,
                    message = "Data collected successfully"
                });

        }


        [HttpGet]
        public HttpResponseMessage getClassesByusingTutorIDAndDayID(TutorClassesDTO data)
        {

            if (data.tutorID <= 0 || data.dayID <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    message = "Invalid tutor ID or Day ID"
                });
            }

            var result = (
                from c in _context.Classes
                join Tutor in _context.Tutors on c.Tutor.tutorID equals Tutor.tutorID

                join s in _context.Slots on c.Slot.slotID equals s.slotID

                join d in _context.Days on c.Day.dayID equals d.dayID

                join st in _context.Students on c.Student.studentID equals st.studentID

                join u in _context.Users on st.User.userID equals u.userID

                join sub in _context.Subjects on

                c.Subject.subjectID equals sub.subjectID

                where c.Tutor.tutorID == data.tutorID && d.dayID == data.dayID
                orderby c.classDate
                select new
                {
                    tutorID = Tutor.tutorID,
                    userID = u.userID,
                    studentName = u.name,
                    StudentTimeZone = u.timezone,
                    gender = u.gender,
                    studentProfilePicture = u.profilePicture,
                    usertype = u.userType,
                    starttime = s.startTime,
                    endtime = s.endTime,
                    dayOfClass = d.dayName,
                    Subject = sub.subjectName,
                    status = c.status,
                    classDate = c.classDate

                }
                ).ToList();
            return Request.CreateResponse(
                HttpStatusCode.OK, new
                {
                    data = result,
                    message = "Data collected successfully"
                });
        }
    }


    public class TutorClassesDTO
    {
        public int dayID { get; set; }
        public int tutorID { get; set; }
    }
}
