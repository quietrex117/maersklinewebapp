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
    public class SchedulesController : Controller
    {
        private DBDDACEntities3 db = new DBDDACEntities3();

        
        // GET: Schedules
        public ActionResult Index(string searchBy, string fromLocation, string toLocation)
        {
            //if(searchBy == "Gender")
                var schedules = from c in db.Schedules select c;
                //return View(db.Schedules.Where(x => x.FromLocation == search).ToList()); shipmentDate
                if(!string.IsNullOrEmpty(fromLocation))
                {
                    schedules = schedules.Where(c => c.FromLocation.StartsWith(fromLocation));
                }
                if (!string.IsNullOrEmpty(toLocation))
                {
                    schedules = schedules.Where(c => c.ToLocation.StartsWith(toLocation));
                }
                
                
                //    schedules = schedules.Where(c => c.ShipmentDate.Equals(shipmentDate.ToShortDateString()));
                
            return View(schedules);
            //return View(db.Schedules.Where(x => x.FromLocation.StartsWith(fromLocation)) && db.Schedules.Where(x.ToLocation.StartsWith(toLocation)));
            //&& x.ToLocation.StartsWith(toLocation) || fromLocation == null));

            //return View(db.Schedules.ToList());
        }

        // GET: Schedules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedule schedule = db.Schedules.Find(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }
            return View(schedule);
        }

        // GET: Schedules/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Schedules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ScheduleId,ShipmentDate,FromLocation,ToLocation,Rate,EarliestDeparture,Status,VesselsID")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                db.Schedules.Add(schedule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(schedule);
        }

        // GET: Schedules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedule schedule = db.Schedules.Find(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }
            return View(schedule);
        }

        // GET: Schedules/Selection/5
        [HttpGet]
        public ActionResult Selection(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedule schedule = db.Schedules.Find(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }
            return View(schedule);
        }

        [HttpGet]
        public ActionResult Selection2(int id)
        {
            List<object> myModel = new List<object>();
            var v = db.Schedules.Where(a => a.ScheduleId == id).FirstOrDefault();
            myModel.Add(v.ScheduleId);
            var vb = db.Users.Where(a => a.Email == HttpContext.User.Identity.Name).FirstOrDefault();
            
            ScheduleBooking scheduleBooking = new ScheduleBooking();
            
            scheduleBooking.ScheduleId = v.ScheduleId;
            scheduleBooking.UsersId = vb.UsersId;
            
            //scheduleBooking.Cargo = v.ToLocation;
            //scheduleBooking.ContainerType = v.FromLocation;

            return View(scheduleBooking);
        }

        

        [HttpPost]
        public ActionResult Selection2(ScheduleBooking scheduleBooking)
        {
            if (ModelState.IsValid)
            {
                db.ScheduleBookings.Add(scheduleBooking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(scheduleBooking);
        }


        public ActionResult Index2()
        {
            List<object> myModel = new List<object>();
            myModel.Add(db.ScheduleBookings.ToList());
            myModel.Add(db.Users.ToList());
            myModel.Add(db.Schedules.ToList());
            return View(myModel);
        }

        

        // POST: Schedules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ScheduleId,ShipmentDate,FromLocation,ToLocation,Rate,EarliestDeparture,Status,VesselsID")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(schedule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(schedule);
        }

        // GET: Schedules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedule schedule = db.Schedules.Find(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }
            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Schedule schedule = db.Schedules.Find(id);
            db.Schedules.Remove(schedule);
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

        public ActionResult GetSchedules()
        {

            using (db)
            {
                
                var schedules = db.Schedules.OrderBy(a => a.FromLocation).ToList();
                return Json(new { data = schedules }, JsonRequestBehavior.AllowGet);

            }
        }
    }
}
