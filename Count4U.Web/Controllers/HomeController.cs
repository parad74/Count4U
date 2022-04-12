using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Count4U.Web.Models;
using Newtonsoft.Json;

namespace Count4U.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Planogram()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Data(GetData[] dataToSend)
        {
            foreach (GetData data in dataToSend)
            {
                data.done++;

                if (data.done == 100)
                {
                    data.done = 0;
                }
            }

            return new JsonResult(){Data = dataToSend};
        }
    }
}
