using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace webapi.Controllers.Class
{
    public interface IClassesController
    {
        HttpResponseMessage CreateWeeklyClassesForRequest(int requestID);
    }

    public class ClassesController : ApiController, IClassesController
    {
        [HttpPost]
        public HttpResponseMessage CreateWeeklyClassesForRequest(int requestID)
        {
            if (requestID <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request ID");

            try
            {
                using (var db = new onlineQuranTutorEntities4())
                {
                    var request = db.StudentTutorRequests.FirstOrDefault(r => r.RequestID == requestID);
                    if (request == null)
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Request not found");

                    var lessonPlan = db.LessonPlans
                        .Where(l => db.Lessons.Any(ls => ls.LessonPlan.lessonPlanID == l.lessonPlanID
                                                        && db.Qurans.Any(q => q.ID == ls.Quran.ID
                                                                              && q.surah.Id == request.surah.Id)))
                        .OrderBy(l => l.lessonPlanID)
                        .ToList();

                    if (!lessonPlan.Any())
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "No lesson plan found for this Surah");

                    var tutorSlots = db.TutorSlots
                        .Where(ts => ts.User.userID == request.User1.userID && ts.status == "available")
                        .ToList();

                    if (!tutorSlots.Any())
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "No available slots for this tutor");

                    var studentSlots = db.StudentSlots
                        .Where(ss => ss.User.userID == request.User.userID)
                        .ToList();

                    if (!studentSlots.Any())
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "No available slots for this student");

                    var matchingSlots = (from ts in tutorSlots
                                         join ss in studentSlots
                                         on new { ts.Slot.slotID, ts.Day.dayID } equals new { slotID = ss.Slot.slotID, dayID = ss.Day.dayID }
                                         select ts).OrderBy(ms => ms.Day.dayID).ToList();

                    if (!matchingSlots.Any())
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "No matching slots between student and tutor");
                    int lessonIndex = 0;
                    DateTime currentDate = DateTime.Today;

                    while (lessonIndex < lessonPlan.Count)
                    {
                        foreach (var slot in matchingSlots)
                        {
                            if (lessonIndex >= lessonPlan.Count) break;
                            int daysToAdd = ((slot.Day.dayID - (int)currentDate.DayOfWeek + 7) % 7);
                            if (daysToAdd == 0)
                                daysToAdd = 7;

                            DateTime classDate = currentDate.AddDays(daysToAdd);

                            var newClass = new webapi.Class
                            {
                                Slot = db.Slots.Find(slot.Slot.slotID),
                                User = db.Users.Find(request.User.userID),
                                User1 = db.Users.Find(request.User1.userID),
                                Subject = db.Subjects.Find(request.User1.userID),
                                Corrections = "0",
                                StudentTutorRequest = db.StudentTutorRequests.Find(requestID),
                                LessonPlan = db.LessonPlans.Find(lessonPlan[lessonIndex].lessonPlanID),
                                Status = "Scheduled",
                                ClassDate = classDate,
                                CreatedAt = DateTime.Now
                            };

                            db.Classes.Add(newClass);
                            lessonIndex++;
                        }
                        currentDate = currentDate.AddDays(7);
                    }

                    db.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        success = true,
                        message = "Weekly classes created successfully using matching slots",
                        totalClasses = lessonPlan.Count
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    success = false,
                    message = "Server error",
                    error = ex.Message
                });
            }
        }
    }
}