using IEM.Common;
using IEM.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace IEM.Areas.FLEXISPEND.Controllers
{

    public class RecoveryController : Controller
    {
        #region Declaration
        dbLib db = new dbLib();
        proLib plib = new proLib();
        DataTable dt = new DataTable();
        CmnFunctions cmnfunc = new CmnFunctions();
        #endregion

        //
        // GET: /FLEXISPEND/Recovery/
        public ActionResult Maker()
        {
            return View();
        }

        //
        // GET: /FLEXISPEND/Recovery/
        public ActionResult Checker()
        {
            return View();
        }
        
        //GSTPhase3_1
        [HttpPost]
        public JsonResult GetRecoveryMaker(string Rejected, string DateFrom, string DateTo, string SupplierId, string Recoveryno)
        {
            try
            {
                string Data1 = "", Data2 = "";
                DataSet ds = db.GetRecoveryMaker(plib.ConvertDate(DateFrom), plib.ConvertDate(DateTo), SupplierId, Rejected, "1", plib.LoginUserId, Recoveryno);
                if (ds != null)
                {
                    dt = ds.Tables[0];
                    if (dt.Rows.Count > 0) { Data1 = JsonConvert.SerializeObject(dt); }

                    dt = ds.Tables[1];
                    if (dt.Rows.Count > 0) { Data2 = JsonConvert.SerializeObject(dt); }

                    return Json(new { Data1, Data2 }, JsonRequestBehavior.AllowGet);
                }
                else { return null; }
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult LoadRecoveryInfo()

        {
            try
            {
                string Data1 = "";
                DataSet ds = db.GetRecoveryMaker("", "", "", "0", "0", plib.LoginUserId,"");
                if (ds != null)
                {
                    dt = ds.Tables[1];
                    if (dt.Rows.Count > 0) { Data1 = JsonConvert.SerializeObject(dt); }

                    return Json(new { Data1 }, JsonRequestBehavior.AllowGet);
                }
                else { return null; }
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult LoadInvoiceDetails(string ECFNo, string InvNo,string ViewType="1",string type="N")
        {
            try
            {
                string Data1 = "",Data2="";
                DataSet ds = db.GetInvoiceAmountDetails(ECFNo, InvNo, ViewType, plib.LoginUserId, type);
                if (ds != null)
                {
                    dt = ds.Tables[0];
                    if (dt.Rows.Count > 0) { Data1 = JsonConvert.SerializeObject(dt); }

                    if(ds.Tables.Count>1)
                    {
                        dt = ds.Tables[1];
                        if (dt.Rows.Count > 0) { Data2 = JsonConvert.SerializeObject(dt); }
                    }
                    return Json(new { Data1,Data2 }, JsonRequestBehavior.AllowGet);
                }
                else { return null; }
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult SetRecoveryMaker(string Recoverygid, string supplierId, string ecfid, string invid, string Recoveryno,
                            string Recoveryamt, string remarks, string isremoved, string creditgl)
        {
            try
            {
                string Data1 = "";
                string Maker = string.Empty;
                Maker = cmnfunc.authorize("471");
                if (Maker == "Success")
                {
                    DataSet ds = db.SetRecoveryMaker(Recoverygid, supplierId, ecfid, invid, Recoveryno, (Recoveryamt == "" || Recoveryamt == null) ? "0" : Recoveryamt, remarks, isremoved, creditgl, plib.LoginUserId);
                    if (ds != null)
                    {
                        dt = ds.Tables[0];
                        if (dt.Rows.Count > 0) { Data1 = JsonConvert.SerializeObject(dt); }

                        return Json(new { Data1 }, JsonRequestBehavior.AllowGet);
                    }
                    else { return null; }
                }
                else
                {
                    Data1 = "Unauthorized User!";
                    return Json(new { Data1 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return null;
            }
        }


        //Credit Note Checker
        [HttpPost]
        public JsonResult GetRecoveryChecker(string DateFrom, string DateTo, string SupplierId, string Recoveryno)
        {
            try
            {
                string Data1 = "", Data2 = "";
                DataSet ds = db.GetRecoveryChecker(plib.ConvertDate(DateFrom), plib.ConvertDate(DateTo), SupplierId, plib.LoginUserId, Recoveryno);
                if (ds != null)
                {
                    dt = ds.Tables[0];
                    if (dt.Rows.Count > 0) { Data1 = JsonConvert.SerializeObject(dt); }

                    dt = ds.Tables[1];
                    if (dt.Rows.Count > 0) { Data2 = JsonConvert.SerializeObject(dt); }

                    return Json(new { Data1, Data2 }, JsonRequestBehavior.AllowGet);
                }
                else { return null; }
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult SetRecoveryChecker(string Recoverygid, string Status, string remarks)
        {
            try
            {
                string Data1 = "";
                 string checker = string.Empty;
                checker = cmnfunc.authorize("472");
                if (checker == "Success")
                {
                    DataSet ds = db.SetRecoveryChecker(Recoverygid, Status, remarks, plib.LoginUserId);
                    if (ds != null)
                    {
                        dt = ds.Tables[0];
                        if (dt.Rows.Count > 0) { Data1 = JsonConvert.SerializeObject(dt); }

                        return Json(new { Data1 }, JsonRequestBehavior.AllowGet);
                    }
                    else { return null; }
                }
                else
                {
                    Data1 = "Unauthorized User!";
                    return Json(new { Data1 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {

                return null;
            }
        }
    }
}
