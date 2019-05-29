using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualServer.DB;
using VirtualServer.Models;

namespace VirtualServer.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetData()
        {
            var data = new DataModel();
            data.DictData = DataProvider.espioProvider.GetDictData();
            data.UsageSecondsFromDB = DataProvider.espioProvider.GetUsageSecondsFromDB();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult UpdateData(string dataForUpdate)
        {
            var dataForUpdateDes = (List<DictDataModel>)JsonConvert.DeserializeObject(dataForUpdate, typeof(List<DictDataModel>));
            var data = DataProvider.espioProvider.UpdateData(dataForUpdateDes);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult CreateRow()
        {
            var data = DataProvider.espioProvider.CreateRow();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}