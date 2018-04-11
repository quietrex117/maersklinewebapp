using DDAC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DDAC.Controllers
{
    public class VesselsController : Controller
    {
        // GET: Vessels
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetVessels()
        {
            using (DBDDACEntities3 dc = new DBDDACEntities3())
            {
                var vessels = dc.Vessels.OrderBy(a => a.VesselName).ToList();
                return Json(new { data = vessels }, JsonRequestBehavior.AllowGet);

            }
        }
    }
}