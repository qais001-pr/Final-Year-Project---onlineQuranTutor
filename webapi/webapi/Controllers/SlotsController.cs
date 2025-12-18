using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using webapi.Models;

namespace webapi.Controllers
{
    public class SlotsController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetSlotsWithDay(int studentId)
        {
            try
            {
                using (var db = new onlineQuranTutorEntities4())
                {
                    // 1️⃣ Student + User + Timezone
                    var studentData = (from s in db.Students
                                       join u in db.Users on s.User.userID equals u.userID
                                       where s.studentID == studentId
                                       select new
                                       {
                                           StudentID = s.studentID,
                                           TimeZoneId = u.timezone
                                       }).FirstOrDefault();

                    if (studentData == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Student not found");
                    }

                    TimeZoneInfo studentTimeZone = TimeZoneInfo.FindSystemTimeZoneById(studentData.TimeZoneId);

                    // 2️⃣ Days
                    var days = db.Days.ToList();

                    // 3️⃣ Slots
                    var slots = db.Slots.ToList();

                    // 4️⃣ Build response
                    var response = new List<DaySlotsDto>();

                    foreach (var day in days)
                    {
                        var slotList = slots
                            .Select(slot =>
                            {
                                DateTime utcStart = DateTime.SpecifyKind(DateTime.Today.Add(slot.startTime), DateTimeKind.Utc);
                                DateTime utcEnd = DateTime.SpecifyKind(DateTime.Today.Add(slot.endTime), DateTimeKind.Utc);

                                DateTime localStart = TimeZoneInfo.ConvertTimeFromUtc(utcStart, studentTimeZone);
                                DateTime localEnd = TimeZoneInfo.ConvertTimeFromUtc(utcEnd, studentTimeZone);

                                return new SlotDto
                                {
                                    SlotID = slot.slotID,
                                    StartTime = localStart.ToString("HH:mm"),
                                    EndTime = localEnd.ToString("HH:mm"),
                                    //LocalStartDateTime = localStart // temporary for ordering
                                };
                            })
                            .OrderBy(s => s.StartTime) // order by local start time
                            .Select(s => new SlotDto
                            {
                                SlotID = s.SlotID,
                                StartTime = s.StartTime,
                                EndTime = s.EndTime
                            })
                            .ToList();

                        response.Add(new DaySlotsDto
                        {
                            DayID = day.dayID,
                            DayName = day.dayName,
                            Slots = slotList
                        });
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (TimeZoneNotFoundException)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid timezone configured for student");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetSlotsForTutor(int tutorId)
        {
            try
            {
                using (var db = new onlineQuranTutorEntities4())
                {

                    var tutor = (from t in db.Tutors
                                 join u in db.Users on t.User.userID equals u.userID
                                 where t.tutorID == tutorId
                                 select new { t.tutorID, u.timezone }).FirstOrDefault();

                    if (tutor == null)
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Tutor not found");

                    TimeZoneInfo tutorTimeZone = TimeZoneInfo.FindSystemTimeZoneById(tutor.timezone);
                    
                    
                    var days = (from d in db.Days
                                select d).ToList();

                    var slots = (from sl in db.Slots
                                 select sl).ToList();
                    
                    var response = (from day in days
                                    select new DaySlotsDto
                                    {
                                        DayID = day.dayID,
                                        DayName = day.dayName,
                                        Slots = (from slot in slots
                                                 let utcStart = DateTime.SpecifyKind(DateTime.Today.Add(slot.startTime), DateTimeKind.Utc)
                                                 let utcEnd = DateTime.SpecifyKind(DateTime.Today.Add(slot.endTime), DateTimeKind.Utc)
                                                 let localStart = TimeZoneInfo.ConvertTimeFromUtc(utcStart, tutorTimeZone)
                                                 let localEnd = TimeZoneInfo.ConvertTimeFromUtc(utcEnd, tutorTimeZone)
                                                 orderby localStart
                                                 select new SlotDto
                                                 {
                                                     SlotID = slot.slotID,
                                                     StartTime = localStart.ToString("HH:mm"),
                                                     EndTime = localEnd.ToString("HH:mm")
                                                 }).ToList()
                                    }).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (TimeZoneNotFoundException)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid timezone");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


    }
}
