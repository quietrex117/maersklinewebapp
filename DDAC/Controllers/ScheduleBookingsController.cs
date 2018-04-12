using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DDAC.Models;

namespace DDAC.Controllers
{
    public class ScheduleBookingsController : Controller
    {
        private DBDDACEntities3 db = new DBDDACEntities3();

        // GET: ScheduleBookings
        public ActionResult Index()
        {
           
            var scheduleBookings = db.ScheduleBookings.Include(s => s.Schedule).Include(s => s.User);
            var vb = db.Users.Where(a => a.Email == HttpContext.User.Identity.Name).FirstOrDefault();

            var v = scheduleBookings.Where(b => b.UsersId == vb.UsersId).ToList();
            return View(v);
        }

        // GET: ScheduleBookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleBooking scheduleBooking = db.ScheduleBookings.Find(id);
            if (scheduleBooking == null)
            {
                return HttpNotFound();
            }
            return View(scheduleBooking);
        }

        // GET: ScheduleBookings/Create
        public ActionResult Create()
        {
            ViewBag.ScheduleId = new SelectList(db.Schedules, "ScheduleId", "FromLocation");
            ViewBag.UsersId = new SelectList(db.Users, "UsersId", "Username");
            return View();
        }

        // POST: ScheduleBookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingId,Cargo,ContainerType,ScheduleId,UsersId")] ScheduleBooking scheduleBooking)
        {
            if (ModelState.IsValid)
            {
                db.ScheduleBookings.Add(scheduleBooking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ScheduleId = new SelectList(db.Schedules, "ScheduleId", "FromLocation", scheduleBooking.ScheduleId);
            ViewBag.UsersId = new SelectList(db.Users, "UsersId", "Username", scheduleBooking.UsersId);
            return View(scheduleBooking);
        }

        // GET: ScheduleBookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleBooking scheduleBooking = db.ScheduleBookings.Find(id);
            if (scheduleBooking == null)
            {
                return HttpNotFound();
            }
            ViewBag.ScheduleId = new SelectList(db.Schedules, "ScheduleId", "FromLocation", scheduleBooking.ScheduleId);
            ViewBag.UsersId = new SelectList(db.Users, "UsersId", "Username", scheduleBooking.UsersId);
            return View(scheduleBooking);
        }

        // POST: ScheduleBookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingId,Cargo,ContainerType,ScheduleId,UsersId")] ScheduleBooking scheduleBooking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scheduleBooking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ScheduleId = new SelectList(db.Schedules, "ScheduleId", "FromLocation", scheduleBooking.ScheduleId);
            ViewBag.UsersId = new SelectList(db.Users, "UsersId", "Username", scheduleBooking.UsersId);
            return View(scheduleBooking);
        }

        // GET: ScheduleBookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleBooking scheduleBooking = db.ScheduleBookings.Find(id);

            if (scheduleBooking == null)
            {
                return HttpNotFound();
            }
            return View(scheduleBooking);
        }

        // POST: ScheduleBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ScheduleBooking scheduleBooking = db.ScheduleBookings.Find(id);
            db.ScheduleBookings.Remove(scheduleBooking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
