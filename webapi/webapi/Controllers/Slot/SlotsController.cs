using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using webapi.Models.Slot;

namespace webapi.Controllers.Slot
{
    public class SlotsController : ApiController
    {
        [HttpGet]
        [Route("api/slots/GetSlotsWithDay")]
        public HttpResponseMessage GetSlotsWithDay(int userid)
        {
            try
            {
                using (var db = new onlineQuranTutorEntities4())
                {
                    var studentData = (
                                       from u in db.Users
                                       where u.userID == userid
                                       select new
                                       {
                                           userid = u.userID,
                                           TimeZone = u.timezone
                                       }).FirstOrDefault();

                    if (studentData == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Student not found");
                    }

                    TimeZoneInfo studentTimeZone = TimeZoneInfo.FindSystemTimeZoneById(studentData.TimeZone);

                    var days = db.Days.ToList();

                    var slots = db.Slots.ToList();

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
                                };
                            })
                            .OrderBy(s => s.StartTime)
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

    }
}
