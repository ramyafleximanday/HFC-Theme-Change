

using Excel;
using IEM.Areas.IFAMS.Models;
using IEM.Areas.MASTERS.Models;
using IEM.Common;
using IEM.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net;
using IEM.Areas.FLEXISPEND.Models;
using ClosedXML.Excel;
using Newtonsoft.Json;
using IEM.App_Start;
using IEM.Areas.EOW.Models;


namespace IEM.Areas.ASMS.Controllers
{
    public class TDSSpecialRateCheckerSummaryController : Controller
    {
        //
        // GET: /ASMS/TDSSpecialRateCheckerSummary/

        private fsIreposiroty objModel;
        ErrorLog objErrorLog = new ErrorLog();
        CmnFunctions objCmnFunctions = new CmnFunctions();
        FlexispendDataModel objd = new FlexispendDataModel();

        public ActionResult Index()
        {
            return View();
        }

        public TDSSpecialRateCheckerSummaryController()
            : this(new FlexispendDataModel())
        {

        }

        public TDSSpecialRateCheckerSummaryController(fsIreposiroty objM)
        {
            objModel = objM;
        }

        [HttpGet]
        public ActionResult TDSSpecialRateCheckerSummary()
        {

            return View();
        }

        //[HttpGet]
        //public PartialViewResult _GetTDSFileList(string straction)
        //{
        //    try
        //    {

        //        List<FS_GSTRModel> lstGetEmployee = new List<FS_GSTRModel>();
        //        TempData["action"] = straction.ToString();
        //        return PartialView(lstGetEmployee);
        //    }
        //    catch (Exception ex)
        //    {
        //        objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
        //        return PartialView();
        //    }
        //}

        //Chekcer
        [HttpGet]
        public PartialViewResult _GetTDSUploadDetails(string straction, int id)
        {
            try
            {

                ViewBag.headervalue = id;
                Session["headerid"] = id;
                List<FS_GSTRModel> lstGetEmployee = new List<FS_GSTRModel>();
                TempData["action"] = straction.ToString();
                return PartialView(lstGetEmployee);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return PartialView();
            }
        }

        [HttpPost]
        public JsonResult TDSApproveRejected(string remarks, string id)
        {
           
            string action = "Approve";
            string headerid = Session["headerid"].ToString();
            string success = objModel.TDSApproveRejectedUpload(remarks, headerid, action);

            return Json(success, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TDSRejectedUpload(string remarks, string id)
        {
            // Session["headerid"] = id;
            string action = "Reject";
            string headerid = Session["headerid"].ToString();
            string success = objModel.TDSRejectedUpload(remarks, headerid,action);

            return Json(success, JsonRequestBehavior.AllowGet);
        }



     

    }
}
