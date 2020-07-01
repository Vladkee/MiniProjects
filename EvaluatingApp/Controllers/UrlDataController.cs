using EvaluatingApp.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EvaluatingApp.Controllers
{
    public class UrlDataController : Controller
    {
        private PerformanceDbContext webDbContext;

        public UrlDataController()
        {
            this.webDbContext = new PerformanceDbContext();
        }
        public ActionResult Historical(string urlId)
        {
            var data = this.webDbContext.Responses.Where(url => url.UrlAddressId == Int32.Parse(urlId));

            List<Dictionary<string, object>> urlData = new List<Dictionary<string, object>>();

            foreach (var item in data)
            {
                urlData.Add(new Dictionary<string, object> 
                {   ["response"] = item.ResponseTime,
                    ["evaluatedDate"] = item.EvaluatedDate.ToString("yyyy/MM/dd") 
                });
            }
            return Json(urlData, JsonRequestBehavior.AllowGet);
        }
    }
}