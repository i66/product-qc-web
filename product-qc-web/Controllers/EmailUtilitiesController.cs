using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace product_qc_web.Controllers
{
    public class EmailUtilitiesController : Controller
    {
        // GET: EmailUtilities
        public ActionResult SendEmail()
        {
            return View();
        }

      
    }
}