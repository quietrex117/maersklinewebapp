using DDAC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DDAC.Controllers
{
    public class BookingController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetBooking()
        {
            using (DBDDACEntities3 dc = new DBDDACEntities3())
            {
                var booking = dc.ScheduleBookings.OrderBy(a => a.Cargo).ToList();
                return Json(new { data = booking }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Book(int id)
        {
            using (DBDDACEntities3 dc = new DBDDACEntities3())
            {
                var v = dc.Bookings.Where(a => a.BookingID == id).FirstOrDefault();
                return View(v);
            }
        }

        [HttpPost]
        public ActionResult Book(Booking booking)
        {
            bool status = false;
            if (ModelState.IsValid)
            {
                using (DBDDACEntities3 dc = new DBDDACEntities3())
                {
                    dc.Bookings.Add(booking);
                    dc.SaveChanges();
                    status = true;

                }
            }
            return new JsonResult { Data = new { status = status } };
        }
    }
}
