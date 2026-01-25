using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using webapi.Models.Class;
namespace webapi.Controllers.Classes
{
    public interface IClassesController
    {
        DateTime GetNextDateForDay(DayOfWeek day);
        HttpResponseMessage CreateClassesWeeklySimple(AcceptRequestDTO request);
    }

    public class ClassesController : ApiController, IClassesController
    {

        onlineQuranTutorEntities4 _context = new onlineQuranTutorEntities4();
        public DateTime GetNextDateForDay(DayOfWeek day)
        {
            DateTime today = DateTime.Today;
            int daysUntil = ((int)day - (int)today.DayOfWeek + 7) % 7;
            if (daysUntil == 0) daysUntil = 7;
            return today.AddDays(daysUntil);
        }
        [HttpPost]
        public HttpResponseMessage CreateClassesWeeklySimple(AcceptRequestDTO request)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var student = _context.Users.Where(s => s.userID == request.studentID).FirstOrDefault();


                    var tutor = _context.Users.Where(t => t.userID == request.tutorID).FirstOrDefault();
                    var subject = _context.Subjects.Where(s => s.subjectID == request.subjectID).FirstOrDefault();
                    var studentRequest = _context.StudentTutorRequests
                        .Where(r => r.RequestID == request.requestID).FirstOrDefault();

                    if (student == null || tutor == null || subject == null || studentRequest == null)
                    {
                        return Request.CreateResponse(
                            HttpStatusCode.BadRequest,
                            new { message = "Invalid request data" }
                        );
                    }

                    var lessonPlans = _context.Lessons
                        .Where(l => l.surah.Id == request.surahID
                                 && l.Subject.subjectID == request.subjectID)
                        .Select(l => l.LessonPlan)
                        .Distinct()
                        .OrderBy(lp => lp.lessonPlanID)
                        .ToList();

                    if (!lessonPlans.Any())
                    {
                        return Request.CreateResponse(
                            HttpStatusCode.BadRequest,
                            new { message = "No lesson plans found for this Surah" }
                        );
                    }

                    var matchingSlots = (
                        from ts in _context.TutorSlots
                        join ss in _context.StudentSlots
                        on new { ts.Slot.slotID, ts.Day.dayID }
                        equals new { ss.Slot.slotID, ss.Day.dayID }
                        where ts.User.userID == request.tutorID
                              && ss.User.userID == request.studentID
                              && ts.status == "available"
                        select ts
                    ).ToList();




                    if (!matchingSlots.Any())
                    {
                        return Request.CreateResponse(
                            HttpStatusCode.BadRequest,
                            new { message = "No matching slots available" }
                        );
                    }

                    var slotStartDates = new List<DateTime>();

                    foreach (var slot in matchingSlots)
                    {
                        if (!Enum.TryParse(slot.Day.dayName, true, out DayOfWeek dayOfWeek))
                        {
                            return Request.CreateResponse(
                                HttpStatusCode.BadRequest,
                                new { message = $"Invalid day name: {slot.Day.dayName}" }
                            );
                        }

                        slotStartDates.Add(GetNextDateForDay(dayOfWeek));
                    }

                    var slotWeekCounters =
                        Enumerable.Repeat(0, matchingSlots.Count).ToList();

                    var classesToAdd = new List<Class>();
                    int slotIndex = 0;

                    foreach (var lessonPlan in lessonPlans)
                    {
                        int currentSlotIndex = slotIndex % matchingSlots.Count;
                        var slot = matchingSlots[currentSlotIndex];

                        DateTime classDate = slotStartDates[currentSlotIndex].AddDays(slotWeekCounters[currentSlotIndex] * 7);

                        bool conflictExists = _context.Classes.Any(c =>
                            c.User1.userID == tutor.userID &&
                            c.ClassDate == classDate &&
                            c.Slot.slotID == slot.Slot.slotID
                        );

                        if (conflictExists)
                        {
                            return Request.CreateResponse(
                                HttpStatusCode.Conflict,
                                new { message = $"Scheduling conflict on {classDate:dd-MMM-yyyy}" }
                            );
                        }

                        classesToAdd.Add(new Class
                        {
                            User = student,
                            User1 = tutor,
                            Subject = subject,
                            Slot = slot.Slot,
                            LessonPlan = lessonPlan,
                            StudentTutorRequest = studentRequest,
                            Status = "pending",
                            Corrections = "0",
                            ClassDate = classDate,
                            CreatedAt = DateTime.Now
                        });

                        slotWeekCounters[currentSlotIndex]++;
                        slotIndex++;
                    }

                    _context.Classes.AddRange(classesToAdd);
                    foreach (var slot in matchingSlots)
                        slot.status = "booked";
                    studentRequest.status = "Accepted";

                    _context.SaveChanges();
                    transaction.Commit();

                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        message = "Classes created successfully",
                        totalClasses = classesToAdd.Count
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    return Request.CreateResponse(
                        HttpStatusCode.InternalServerError,
                        new { message = "Something went wrong", error = ex.Message }
                    );
                }
            }
        }
    }
}