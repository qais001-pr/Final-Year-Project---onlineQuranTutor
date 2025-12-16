using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using webapi.Models;

namespace webapi.Controllers
{
    public class StudentRequestsTutorController : ApiController
    {


        onlineQuranTutorEntities4 _context = new onlineQuranTutorEntities4();

        [HttpPost]
        public HttpResponseMessage studentSendRequestToTutor(StudentRequestsTutorDTO requestdata)
        {


            if (requestdata.tutorid <= 0 || requestdata.studentid <= 0 || requestdata.subjectid <= 0 || requestdata.surahsID <= 0 || requestdata.surahsID > 30)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest, new
                    {
                        message = "Invalid Input"
                    });
            }



            var check = _context.Students.Where(x => x.studentID == requestdata.studentid
                      && x.Subject.subjectID == requestdata.subjectid).FirstOrDefault();

            if (check == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest, new
                    {
                        message = "No student found with this student ID and subject ID."
                    });
            }

            var chectsurahID = _context.surahs.Where(x => x.Id == requestdata.surahsID).FirstOrDefault();

            if (chectsurahID == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest, new
                    {
                        message = "Surah Not Found With this ID"
                    });
            }

            var checkReq = _context.StudentTutorRequests.Where(
                req => req.SurahID == requestdata.surahsID &&
                req.Tutor.tutorID == requestdata.tutorid &&
                req.Student.studentID == requestdata.studentid)
                .FirstOrDefault();



            if (checkReq != null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest, new
                    {
                        message = "Request Already Sent To Tutor."
                    });
            }
            _context.StudentTutorRequests.Add(new StudentTutorRequest()
            {
                Student = _context.Students.Where(s => s.studentID == requestdata.studentid).FirstOrDefault(),
                Tutor = _context.Tutors.Where(t => t.tutorID == requestdata.tutorid).FirstOrDefault(),
                Subject = _context.Subjects.Where(sub => sub.subjectID == requestdata.subjectid).FirstOrDefault(),
                SurahID = requestdata.surahsID,
                status = "pending",
                createdAt = System.DateTime.Now,
                updatedAt = System.DateTime.Now
            });
            _context.SaveChanges();
            return Request.CreateResponse(
                HttpStatusCode.OK, new
                {
                    message = "Request Send Successfully"
                });
        }



        public DateTime GetNextDateForDay(DayOfWeek day)
        {
            DateTime today = DateTime.Today;
            int daysUntil = ((int)day - (int)today.DayOfWeek + 7) % 7;
            if (daysUntil == 0) daysUntil = 7;
            return today.AddDays(daysUntil);
        }








        [HttpPost]
        public HttpResponseMessage CreateClassesWeeklySimple(AcceptRequestFromTutorDTO request)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var student = _context.Students.FirstOrDefault(s => s.studentID == request.studentID);
                    var tutor = _context.Tutors.FirstOrDefault(t => t.tutorID == request.tutorID);
                    var subject = _context.Subjects.FirstOrDefault(s => s.subjectID == request.subjectID);
                    var studentRequest = _context.StudentTutorRequests
                        .FirstOrDefault(r => r.RequestID == request.requestID);

                    if (student == null || tutor == null || subject == null || studentRequest == null)
                    {
                        return Request.CreateResponse(
                            HttpStatusCode.BadRequest,
                            new { message = "Invalid request data" }
                        );
                    }

                    if (studentRequest.status == "Accepted")
                    {
                        return Request.CreateResponse(
                            HttpStatusCode.BadRequest,
                            new { message = "Request already accepted" }
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
                        where ts.Tutor.tutorID == request.tutorID
                              && ss.Student.studentID == request.studentID
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
                            c.Tutor.tutorID == tutor.tutorID &&
                            c.classDate == classDate &&
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
                            Student = student,
                            Tutor = tutor,
                            Subject = subject,
                            Slot = slot.Slot,
                            Day = slot.Day,
                            LessonPlan = lessonPlan,
                            StudentTutorRequest = studentRequest,
                            status = "pending",
                            corrections = 0,
                            classDate = classDate,
                            createdAt = DateTime.Now
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
