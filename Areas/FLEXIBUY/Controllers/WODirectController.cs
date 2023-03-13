using IEM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IEM.Areas.FLEXIBUY.Models;
using System.Data;
using Newtonsoft.Json;
using IEM.Areas.MASTERS.Controllers;
using System.Collections;
using IEM.Helper;
using System.Net;
using System.IO;
using IEM.Models;
//using IEM.App_Start;

namespace IEM.Areas.FLEXIBUY.Controllers
{
    // [NoDirectAccess]
    public class WODirectController : Controller
    {
        ErrorLog objErrorLog = new ErrorLog();
        private WORepository objModel;
            proLib plib = new proLib();
        dbLib db = new dbLib();
        CmnFunctions objCmnFunctions = new CmnFunctions();
        public WODirectController()
            : this(new WODataModel())
        {

        }
        public WODirectController(WORepository objM)
        {
            objModel = objM;
        }

        public ActionResult DirectWO(string cbfdetailsgid = null, string wogid = null, string viewtype = null)
        {
            Session["objlist"] = null;
            ViewBag.isapproval = "";
            ViewBag.isapprovalMode = "0";
            ViewBag.deleteflag = "0";
            if (cbfdetailsgid != null)
                ViewBag.cbfdetailsgid = cbfdetailsgid;
            else
                ViewBag.cbfdetailsgid = "0";
            if (wogid != null)
            {
                Session["WOHeaderGid"] = Convert.ToInt32(wogid);
                ViewBag.wogid = wogid;
            }
            else
            {
                ViewBag.wogid = "0";
            }
            if (viewtype == "view")
            {
                ViewBag.viewtype = "1";
            }
            else if (viewtype == "delmat" || viewtype == "checker")
            {
                ViewBag.viewtype = "1";
                ViewBag.isapprovalMode = "1";
                ViewBag.isapproval = viewtype;
            }
            else if (viewtype == "delete")
            {
                ViewBag.viewtype = "1";
                ViewBag.deleteflag = "1";
            }
            else
            {
                ViewBag.viewtype = "0";
            }
            if (Session["WOHeaderGid"] != null)
            {
                if (ViewBag.wogid != "0")
                {
                    ViewBag.wogid = Convert.ToString((int)Session["WOHeaderGid"]);
                }
            }
            return View();
        }

        [HttpPost]
        public JsonResult GetWOHeaderDetails(string cbfdetid = "0")
        {
            //string cbfdetid = "19";
            string data0 = string.Empty;
            string data1 = string.Empty;
            string data2 = string.Empty;
            string data3 = string.Empty;
            string data4 = string.Empty;
            string ErrorMsg = string.Empty;
            int wogid = 0;
            try
            {
                string[] cbfdetarray = cbfdetid.Split(',');

                // Added by k.Bharathy
                if (Session["WOHeaderGid"] == null || Session["WOHeaderGid"] == "0")
                {
                    // wogid = (int)Session["WOHeaderGid"];
                    DataSet ds = new DataSet();
                    DataTable dt;
                    ds = objModel.GetWOHeader(wogid);
                    dt = ds.Tables[0];
                    if (dt.Rows.Count > 0) { data0 = JsonConvert.SerializeObject(dt); }
                    dt = ds.Tables[1];
                    if (dt.Rows.Count > 0) { data1 = JsonConvert.SerializeObject(dt); }
                    dt = ds.Tables[2];
                    if (dt.Rows.Count > 0) { data2 = JsonConvert.SerializeObject(dt); }
                    dt = ds.Tables[3];
                    if (dt.Rows.Count > 0) { data3 = JsonConvert.SerializeObject(dt); }
                    dt = ds.Tables[4];
                    DataRow row = dt.NewRow();
                    dt.Rows.InsertAt(row, 0);
                    dt.Rows[0][0] = 0;
                    dt.Rows[0][1] = "-- Select One --";
                    if (dt.Rows.Count > 0) { data4 = JsonConvert.SerializeObject(dt); }
                } 
                else if (Session["WOHeaderGid"] != null && Session["WOHeaderGid"] != "0")
                {
                    wogid = (int)Session["WOHeaderGid"];
                    DataSet ds = new DataSet();
                    DataTable dt;
                    ds = objModel.GetWODetailsParent(wogid);
                    dt = ds.Tables[0];
                    if (dt.Rows.Count > 0) { data0 = JsonConvert.SerializeObject(dt); }
                    dt = ds.Tables[1];
                    if (dt.Rows.Count > 0) { data1 = JsonConvert.SerializeObject(dt); }
                    dt = ds.Tables[2];
                    if (dt.Rows.Count > 0) { data2 = JsonConvert.SerializeObject(dt); }
                    dt = ds.Tables[3];
                    if (dt.Rows.Count > 0) { data3 = JsonConvert.SerializeObject(dt); }
                    dt = ds.Tables[4];
                    if (dt.Rows.Count > 0) { data4 = JsonConvert.SerializeObject(dt); }

                }
                 


                return Json(new { data0, data1, data2, data3, data4, ErrorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorMsg = "2";
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json(new { data0, data1, data2, data3, data4, ErrorMsg }, JsonRequestBehavior.AllowGet);
            }

            //bharathy
        }

        [HttpPost]
        public JsonResult AddWOParentDetails(WOEntity WoParentDetailsInsert)
        {
            objErrorLog.WriteErrorLog("wo direct controller - ", " 159");
            string data0 = string.Empty;
            string data1 = string.Empty;
            string data2 = string.Empty;
            string ErrorMsg = string.Empty;
            WoParentDetailsInsert.WOType = "O"; // For With_Out PRPAR based raised Wo
            try
            {
                ErrorMsg = "1";
                int result = objModel.AddParentWODetails(WoParentDetailsInsert);
                if (result == 0)
                {
                    ErrorMsg = "1";
                }
                else
                {
                    ErrorMsg = "0";
                    DataSet ds = new DataSet();
                    DataTable dt;
                    ds = objModel.GetDirectWODetails(result);
                    dt = ds.Tables[0];
                    if (dt.Rows.Count > 0) { data0 = JsonConvert.SerializeObject(dt); }
                    dt = ds.Tables[1];
                    if (dt.Rows.Count > 0) { data1 = JsonConvert.SerializeObject(dt); }
                    dt = ds.Tables[2];
                    if (dt.Rows.Count > 0) { data2 = JsonConvert.SerializeObject(dt); }
                }

                return Json(new { data0, data1, ErrorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog("wo direct controller - " + ex.Message.ToString(), ex.ToString() + " 190");
                return Json(new { data0, data1, ErrorMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddWOParentDetails1(WOEntity WoParentDetailsInsert)
        {
            string data0 = string.Empty;
            string data1 = string.Empty;
            string data2 = string.Empty;
            string ErrorMsg = string.Empty;
            try
            {
                ErrorMsg = "1";
                int result = objModel.AddParentWODetails1(WoParentDetailsInsert);
                if (result == 0)
                {
                    ErrorMsg = "1";
                }
                else
                {
                    ErrorMsg = "0";
                    DataSet ds = new DataSet();
                    DataTable dt;
                    ds = objModel.GetDirectWODetails(result);
                    dt = ds.Tables[0];
                    if (dt.Rows.Count > 0) { data0 = JsonConvert.SerializeObject(dt); }
                    dt = ds.Tables[1];
                    if (dt.Rows.Count > 0) { data1 = JsonConvert.SerializeObject(dt); }
                    dt = ds.Tables[2];
                    if (dt.Rows.Count > 0) { data2 = JsonConvert.SerializeObject(dt); }
                }

                return Json(new { data0, data1, data2, ErrorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json(new { data0, data1, data2, ErrorMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateWOParentDetails(WOEntity WoParentDetailsInsert)
        {
            string data0 = string.Empty;
            string data1 = string.Empty;
            string ErrorMsg = string.Empty;
            try
            {
                ErrorMsg = "1";
                int result = objModel.UpdateParentWODetails(WoParentDetailsInsert);
                if (result == 0)
                {
                    ErrorMsg = "1";
                }
                else
                {
                    ErrorMsg = "0";
                    DataSet ds = new DataSet();
                    DataTable dt;
                    ds = objModel.GetDirectWODetails(result);
                    dt = ds.Tables[0];
                    if (dt.Rows.Count > 0) { data0 = JsonConvert.SerializeObject(dt); }
                    dt = ds.Tables[1];
                    if (dt.Rows.Count > 0) { data1 = JsonConvert.SerializeObject(dt); }
                }

                return Json(new { data0, data1, ErrorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json(new { data0, data1, ErrorMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Get PO GstTax Details
        public JsonResult GetTaxdetails(string PODetailGId, string ProvLocId, string Baseamt, string ReciverLocId, string HSN_ID)
        {
            DataTable dt = new DataTable();
            try
            {
                string Data1 = "", Data2 = "";
                DataSet ds = db.GetGst_DirectWo(PODetailGId, ProvLocId, Baseamt, ReciverLocId, HSN_ID);
                dt = ds.Tables[0];
                if (dt.Rows.Count > 0) { Data1 = JsonConvert.SerializeObject(dt); }
                if (ds.Tables.Count > 1)
                {
                    dt = ds.Tables[1];
                    if (dt.Rows.Count > 0) { Data2 = JsonConvert.SerializeObject(dt); }
                }

                return Json(new { Data1, Data2 }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        //ramya added on 02 oct 22

        [HttpPost]
        public JsonResult SubmitApprover(WOEntity ObjWOEntity)
        {
            string ErrorMsg = string.Empty;
            string result = string.Empty;
            string Data2 = "0";
            //vadivu add
            int WoGid = ObjWOEntity.WOGid;
            try
            {
                ErrorMsg = "0";
                DataTable dt = new DataTable();
                DataSet ds1 = db.CheckAuthorize(Convert.ToString(ObjWOEntity.WOGid), Convert.ToString(plib.LoginUserId), "WO");

                dt = ds1.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["Result"]) == "Y")
                    {
                        result = Convert.ToString(dt.Rows[0]["Result"]);
                    }
                    else
                    {

                        result = JsonConvert.SerializeObject(dt);
                    }
                }
                else
                {

                    result = JsonConvert.SerializeObject(dt);
                }

                if (result == "Y")
                {

                    ErrorMsg = objModel.GetDelmatemployeeForWoDirect(ObjWOEntity);// ramua modified on 02 oct 22
                    if (ErrorMsg == "success" || ErrorMsg == "Success")
                    {
                        CbfSumModel objforMail = new CbfSumModel();
                        // int refgid = ObjWOEntity.WOGid;
                        string reqstatus = objforMail.GetRequestStatus(Convert.ToInt32(ObjWOEntity.WOGid), "WO");
                        int queuegid = objforMail.GetQueueGidForMail(Convert.ToInt32(ObjWOEntity.WOGid), "WO");
                        string curapprovalstage = "0";
                        if (reqstatus == "S")
                        {
                            curapprovalstage = "0";
                        }
                        else
                        {
                            curapprovalstage = "1";
                        }
                        ForMailController mailsender = new ForMailController();
                        //old
                        // mailsender.sendusermail("FB", "WO", Convert.ToString(queuegid), reqstatus, curapprovalstage);
                        //vadivu add
                        string EcfGid = "EcfGid";
                        int PrGid = 0;
                        string pogid = "pogid";
                        string cbfgid = "cbfgid";
                        mailsender.sendusermailEOW("FB", "WO", Convert.ToString(queuegid), reqstatus, curapprovalstage, EcfGid, PrGid, pogid, cbfgid, WoGid);


                        //end

                        ErrorMsg = "1";
                        DataSet ds2 = new DataSet();
                        DataTable dt2 = new DataTable();

                        prsummodel prmodel = new prsummodel();
                        ds2 = prmodel.Getapprovaldetails("WO");
                        dt2 = ds2.Tables[0];


                        if (ds2.Tables[0].Rows.Count > 0)
                        {
                            Data2 = dt2.Rows[0]["queue_ref_gid"].ToString() + ":" + dt2.Rows[0]["doc_status_gid"].ToString();
                        }
                        ErrorMsg = ErrorMsg + "-" + Data2;
                        return Json(new { ErrorMsg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ErrorMsg = ErrorMsg + "-" + Data2;
                        return Json(new { ErrorMsg }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ErrorMsg = "2";
                    ErrorMsg = ErrorMsg + "-" + Data2;
                    return Json(new { ErrorMsg }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                ErrorMsg = ex + "-" + Data2;
                return Json(new { ErrorMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Employee AutoComplete
        //Employee Auto Complete
        [HttpPost]
        public JsonResult GetAutoCompleteEmployeeAdd(string txt)
        {
            try { return Json(GetAutoCompleteEmployeeAdd(txt, plib.LoginUserId), JsonRequestBehavior.AllowGet); }
            catch { return null; }
        }

        //Employee Auto Complete
        public List<string> GetAutoCompleteEmployeeAdd(string txt, string _UsrId)
        {
            try
            {
                List<string> result = new List<string>();
                DataSet ds = db.GetAutoComplete(txt, "13", _UsrId);
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        result.Add(string.Format("{0}~{1}", dr[0], dr[1]));
                    }
                }
                ds.Dispose();
                return result;
            }
            catch { return null; }
        }
        #endregion

    }
}
