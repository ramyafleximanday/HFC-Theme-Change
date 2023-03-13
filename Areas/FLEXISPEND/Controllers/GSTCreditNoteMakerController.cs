using IEM.Areas.FLEXISPEND.Models;
using IEM.Areas.MASTERS.Models;
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
    public class GSTCreditNoteMakerController : Controller
    {

        #region Declaration
        private fsIreposiroty objModel;
        dbLib db = new dbLib();
        proLib plib = new proLib();
        DataTable dt = new DataTable();
        CmnFunctions cmnfunc = new CmnFunctions();
        #endregion





        ErrorLog objErrorLog = new ErrorLog();
        private FileServer Cmnfs = new FileServer();

        CommonIUD objCommonIUD = new CommonIUD();

        IEM_MST_EXPENSECATEGORY expcat = new IEM_MST_EXPENSECATEGORY();
        FlexispendDataModel ObjCygnet = new FlexispendDataModel();
        //
        // GET: /FLEXISPEND/GSTCreditNoteMaker/



        public GSTCreditNoteMakerController()
            : this(new FlexispendDataModel())
        {

        }

        public GSTCreditNoteMakerController(fsIreposiroty objM)
        {
            objModel = objM;
        }
        public ActionResult GSTCreditNoteMaker()
        {
            // dropdown binding
            List<GSTCreditNote_Model> LstObjModelholdq = new List<GSTCreditNote_Model>();
            GSTCreditNote_Model typemodel = new GSTCreditNote_Model();
            typemodel.HSN = new SelectList(objModel.Get_CreditnoteHsnDropdown().ToList(), "HsnId", "HsnCode");
            typemodel.ReceiverLocation = new SelectList(objModel.Get_RecevierLocationDropdown().ToList(), "ReceiverLocationid", "receiverlocationName");
            typemodel.ProviderLocation = new SelectList(objModel.Get_ProviderLocationDropdown().ToList(), "ProviderLocationid", "Cygnet_Provider_Location_Name");

            return View(typemodel);
        }

        //Checker
        public ActionResult GSTCreditNoteChecker()
        {
            //// dropdown binding
            //List<GSTCreditNote_Model> LstObjModelholdq = new List<GSTCreditNote_Model>();
            //GSTCreditNote_Model typemodel = new GSTCreditNote_Model();
            //typemodel.HSN = new SelectList(objModel.Get_CreditnoteHsnDropdown().ToList(), "HsnId", "HsnCode");
            //typemodel.ReceiverLocation = new SelectList(objModel.Get_RecevierLocationDropdown().ToList(), "ReceiverLocationid", "receiverlocationName");

            return View();
        }

        [HttpPost]
        public JsonResult GetCreditNoteMaker(string Rejected, string DateFrom, string DateTo, string SupplierId)
        {
            try
            {
                string Data1 = "", Data2 = "";
                DataSet ds = db.GetCreditNoteMaker(plib.ConvertDate(DateFrom), plib.ConvertDate(DateTo), SupplierId, Rejected, "1", plib.LoginUserId);
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
        public JsonResult LoadCreditNoteInfo()
        {
            try
            {
                string Data1 = "";
                DataSet ds = db.GetCreditNoteMaker("", "", "", "0", "0", plib.LoginUserId);
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
        public JsonResult LoadInvoiceDetails(string ECFNo, string InvNo)
        {
            try
            {
                string Data1 = "";
                DataSet ds = db.GetInvoiceAmountDetails(ECFNo, InvNo, "1", plib.LoginUserId);
                if (ds != null)
                {
                    dt = ds.Tables[0];
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
        public JsonResult SetCreditNoteMakerGST(string Creditnotegid, string supplierId, string ecfid, string invid, string creditnoteno,
                            string creditnoteamt, string remarks, string HsnId, string providerlocationid, string receiverlocationid, string isremoved, string Cygnet_gid)
        {
            try
            {
                string Data1 = "";
                string Maker = string.Empty;
                Maker = cmnfunc.authorize("215");
                if (Maker == "Success")
                {
                    DataSet ds = db.SetCreditNoteMakerGST(Creditnotegid, supplierId, ecfid, invid, creditnoteno, (creditnoteamt == "" || creditnoteamt == null) ? "0" : creditnoteamt, remarks, HsnId, providerlocationid, receiverlocationid, isremoved, plib.LoginUserId, Cygnet_gid);
                    if (ds != null)
                    {
                        dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            Session["CreditnoteGid"] = Convert.ToString(dt.Rows[0]["Creditnotegid"]);
                            Data1 = JsonConvert.SerializeObject(dt);
                        }

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


        //Remarks Update

        [HttpPost]
        public JsonResult SetCreditNoteRemarksUpdate(string Creditnotegid, string remarks, string Action)
        {
            try
            {
                string Data1 = "";
                string Maker = string.Empty;
                Maker = cmnfunc.authorize("215");
                if (Maker == "Success")
                {
                    DataSet ds = db.SetCreditNoteReMarksGST(Creditnotegid, remarks, Action);
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
        public JsonResult GetCreditNoteChecker(string DateFrom, string DateTo, string SupplierId)
        {
            try
            {
                string Data1 = "", Data2 = "";
                DataSet ds = db.GetCreditNoteCheckerGST(plib.ConvertDate(DateFrom), plib.ConvertDate(DateTo), SupplierId, plib.LoginUserId);
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
        public JsonResult SetCreditNoteChecker(string Creditnotegid, string Status, string remarks)
        {
            try
            {
                string Data1 = "";
                string checker = string.Empty;
                checker = cmnfunc.authorize("216");
                if (checker == "Success")
                {
                    DataSet ds = db.SetCreditNoteChecker(Creditnotegid, Status, remarks, plib.LoginUserId);
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

        //selva 19-11-2020
        [HttpPost]
        public JsonResult GetCygnetInvoiceDetails(string Cygnet_gid)
        {
            try
            {
                string Data1 = "";
                DataSet ds = db.GetCygnetInvoiceDetails(Cygnet_gid, plib.LoginUserId);

                dt = ds.Tables[0];
                if (dt.Rows.Count > 0) { Data1 = JsonConvert.SerializeObject(dt); }
                var jsonResult = Json(new { Data1 }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = 2147483647;
                return jsonResult;

            }
            catch
            {
                return null;
            }
        }

        public PartialViewResult CygnetGrid(string panno = "", string suppliername = "", string GSNTNNo = "", string InvoiceNo = "", string FromDate = "", string ToDate = "", string SupplierId = "0")
        {
            CygnetSearchModel objowner = new CygnetSearchModel();

            if (TempData["SearchItems"] != null)
            {
                objowner.CygModel = (List<CygnetSearchModel>)TempData["SearchItems"];
            }
            else
            {
                CygnetSearchModel invsearch = new CygnetSearchModel();
                invsearch.Cygnet_SupplierPanNo = panno;
                invsearch.Cygnet_SupplierName = suppliername;
                invsearch.Cygnet_Supplier_GSTNNo = GSNTNNo;
                invsearch.Cygnet_InvoiceNo = InvoiceNo;
                invsearch.Cygnet_InvoiceFromDate = FromDate;
                invsearch.Cygnet_InvoiceToDate = ToDate;
                invsearch.InvoiceSupplier_gid = SupplierId;
                objowner.CygModel = ObjCygnet.SelectCrdditNoteInvoiceSearch(invsearch).ToList();
                TempData["SearchItems"] = objowner.CygModel;
            }
            return PartialView(objowner);
        }

        public PartialViewResult _LoadGstDetails(string gid = "")
        {
            List<GSTCreditNote_Model> lstnew = new List<GSTCreditNote_Model>();
            if (gid != "" && gid != "0")
            {
                Session["CreditnoteGid"] = gid;
            }
            return PartialView();
        }


    }
}
