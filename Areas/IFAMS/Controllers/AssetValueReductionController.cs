using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Mvc;
using IEM.Areas.IFAMS.Models;
using IEM.Common;
using IEM.App_Start;
using System.Net;
using System.IO;
using Excel;
using System.Threading.Tasks;
using ClosedXML.Excel;
namespace IEM.Areas.IFAMS.Controllers
{
    public class assetvaluereductionController : Controller
    {
        private IfamsAssetVRRepository_M ivr;
        private IRepository Tfr;
        private CmnFunctions objcmnAssetVR = new CmnFunctions();
        private CmnFunctions objCmnFunctions = new CmnFunctions();
        private FileServer Cmnfs = new FileServer();
        ErrorLog objErrorLog = new ErrorLog();
        public assetvaluereductionController() : this(new IfamsAssetVRDataModel_M(), new DataModel()) { }
        public assetvaluereductionController(IfamsAssetVRRepository_M objAssetVRModel, IRepository obTOA)
        {
            ivr = objAssetVRModel;
            Tfr = obTOA;
        }
        //
        // GET: /IFAMS/assetvaluereduction/

        public ActionResult ValueReductionSummary()
        {
            try
            {
                Session["values"] = null;
                AssetVRModel VRrecord = new AssetVRModel();
                VRrecord.VRModel = ivr.GetMakerReduction(Convert.ToString(objcmnAssetVR.GetLoginUserGid())).ToList();
                if (VRrecord.VRModel.Count == 0) { ViewBag.Message = "No records found"; ViewBag.status = "DRAFT"; }
                else
                {
                    ViewBag.status = "DRAFT";
                }
                Session["AVRExceldownload"] = VRrecord.VRModel;
                return View(VRrecord);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return View();
            }
            finally
            {

            }
        }

        [HttpPost]
        public ActionResult ValueReductionSummary(string vrfilterid, string status)
        {
            AssetVRModel vrdetails = new AssetVRModel();
            List<AssetVRModel> vrdetail = new List<AssetVRModel>();
            try
            {
                vrdetails.VRModel = ivr.GetVRDetailsearch(vrfilterid).ToList();
                if (vrfilterid != null)
                {
                    ViewBag.vrfilterid = vrfilterid;
                    vrdetails.VRModel = vrdetails.VRModel.Where(x => vrfilterid.ToUpper() == null || (x.assetdetDetid.Contains(vrfilterid.ToUpper()))).ToList();
                }                
                if (status != "--Select Status--")
                {
                    ViewBag.status = status;
                    vrdetails.VRModel = vrdetails.VRModel.Where(x => status.ToUpper() == null || (x.StatusName.Equals(status.ToUpper()))).ToList();
                }                
                if (vrdetails.VRModel.Count == 0)
                {
                    vrdetails.VRModel = Enumerable.Empty<AssetVRModel>().ToList<AssetVRModel>();
                    ViewBag.Message = "No Records Found";
                }
                Session["AVRExceldownload"] = vrdetails.VRModel;
                return View(vrdetails);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return View(vrdetails);
            }
            finally
            {

            }

        }

        public ActionResult ValueReduction()
        {
            try
            {
                AssetVRModel VRrecord = new AssetVRModel();
                if (Session["values"] != null)
                {
                    VRrecord.VRModel = (List<AssetVRModel>)Session["values"];
                }
                else
                {
                    VRrecord.VRModel = Enumerable.Empty<AssetVRModel>().ToList<AssetVRModel>();
                    ViewBag.Message = "No records found";
                }
                return View(VRrecord);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return View();
            }
            finally
            {

            }
        }

        [HttpPost]
        public ActionResult ValueReduction(string command, string vrlocation, string vrfilter1, FormCollection collections)
        {
            AssetVRModel vrdetails = new AssetVRModel();
            try
            {
                if (command == "SEARCH")
                {
                    vrdetails.VRModel = ivr.GetVRDetailsearch(vrfilter1, vrlocation).ToList();
                   
                    ViewBag.vrlocation = vrlocation;
                    ViewBag.vrfilter1 = vrfilter1;
                    if (vrdetails.VRModel.Count == 0)
                    {
                        vrdetails.VRModel = Enumerable.Empty<AssetVRModel>().ToList<AssetVRModel>();
                        ViewBag.Message = "No Records Found";
                    }
                }
                Session["values"] = vrdetails.VRModel;

                if (command == "CLEAR")
                {
                    Session["values"] = null;
                    return RedirectToAction("ValueReduction", "AssetValueReduction");
                }
                return View(vrdetails);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString()); return View(vrdetails);
            }
            finally
            {

            }
        }

        [HttpPost]
        public JsonResult ReductionSave(Models.AssetVRModel status)
        {
            string Result = string.Empty;
            string Maker = objcmnAssetVR.authorize("383");
            try
            {
                if (Maker == "Success")
                {
                    ivr.Updateasset(status);
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else { return Json(Maker, JsonRequestBehavior.AllowGet); }
            }

            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }
            finally
            {

            }
        }

        [HttpPost]
        public JsonResult locautosearch(string term)
        {
            try
            {
                List<AssetVRModel> autoloc = new List<AssetVRModel>();
                autoloc = ivr.VRAutoBranch(term).ToList();
                return Json(autoloc, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
            finally
            {
            }

        }

        [HttpPost]
        public JsonResult locautoassetid(string term)
        {
            try
            {
                List<AssetVRModel> autoloc = new List<AssetVRModel>();
                autoloc = ivr.VRAutoAsset(term).ToList();
                return Json(autoloc, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
            finally
            {
            }

        }



        // [HttpGet]
        public PartialViewResult ValueReductionViewdetails(string VRassetGid, string viewfor, FormCollection collections, SaleMakerModel status)
        {
            try
            {

                AssetVRModel vrlist = new AssetVRModel();
                vrlist.VRModel = ivr.GetVRDetail(VRassetGid).ToList();

                if (viewfor == "checkerView")
                {
                    ViewBag.viewfor = "checkerView";
                }
                if (viewfor == "abort")
                {
                    ViewBag.viewfor = "abort";
                }
                if (viewfor == "view")
                {
                    ViewBag.viewfor = "view";
                }
                if (vrlist.VRModel.Count == 0)
                {
                    ViewBag.Message = "No Records Found";
                }
                return PartialView(vrlist);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return PartialView();
            }
            finally
            {
            }
        }


        [HttpPost]
        public JsonResult ValueReductionView(string command, string id, string remarks)
        {
            string result = "";
            try
            {

                if (command == "abort")
                {
                    result = ivr.AbortVR(id);
                    ViewBag.viewfor = "abort";
                    if (result == "success")
                    {
                        result = "success";
                    }
                    else
                    {
                        result = "Fail";
                    }

                }
                
                if (command == "Submit")
                {
                    if (id != "")
                    {
                        result = ivr.VRChkApprovalStatus(id, "Submit", remarks);
                        if (result == "success")
                            if (result == "success")
                            {
                                result = "success";
                            }
                            else
                            {
                                result = "Fail";
                            }
                    }
                    else
                    {
                        result = "Fail";
                    }
                }
                if (command == "APPROVE")
                {
                    if (id != "")
                    {
                        // Session[""] = id;
                        result = ivr.VRChkApprovalStatus(id, "APPROVED", remarks);
                        if (result == "success")
                            if (result == "success")
                            {
                                result = "success";
                            }
                            else
                            {
                                result = "Fail";
                            }

                    }
                }
                if (command == "REJECT")
                {
                    if (id != "")
                    {
                        // Session["Saleno"] = id;
                        result = ivr.VRChkApprovalStatus(id, "REJECTED", remarks);
                        if (result == "success")
                        {
                            result = "success";
                        }
                        else
                        {
                            result = "Fail";
                        }

                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }
            finally
            {
            }
        }

        //-------------------------CHECKER----------------------

        public ActionResult ValueReductionChkSummary()
        {
            try
            {
                AssetVRModel VRrecord = new AssetVRModel();
                VRrecord.VRModel = ivr.GetCheckerReduction(Convert.ToString(objcmnAssetVR.GetLoginUserGid())).ToList();
                if (VRrecord.VRModel.Count == 0) { ViewBag.Mesage = "No records found"; }

                return View(VRrecord);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return View();
            }
            finally
            {

            }
        }

        [HttpPost]
        public ActionResult ValueReductionChkSummary(string vrchkfilterid)
        {
            AssetVRModel vrdetails = new AssetVRModel();
            List<AssetVRModel> vrdetail = new List<AssetVRModel>();
            try
            {

                vrdetails.VRModel = ivr.GetVRchkDetailsearch(vrchkfilterid.Trim()).ToList();
                
                if (vrdetails.VRModel.Count == 0)
                {
                    vrdetails.VRModel = Enumerable.Empty<AssetVRModel>().ToList<AssetVRModel>();
                    ViewBag.Message = "No Records Found";
                }
                return View(vrdetails);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return View(vrdetails);

            }
            finally
            {

            }

        }


        public JsonResult ReductionStatus()
        {

            IfamsAssetVRDataModel_M Model = new IfamsAssetVRDataModel_M();

            try
            {
                return Json(Model.GetVRStatus(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
            finally
            {
            }
        }
        public ActionResult AVRExceldownload()
        {

            List<AssetVRModel> cList;
            cList = (List<AssetVRModel>)Session["AVRExceldownload"];

            DataTable dt = new DataTable();
            dt.Columns.Add("S.No.");
            dt.Columns.Add("Reduction Number");
            dt.Columns.Add("Asset Id");
            dt.Columns.Add("Reduced Amount");
            dt.Columns.Add("Old Asset Value");
            dt.Columns.Add("New Asset Value");
            for (int i = 0; i < cList.Count; i++)
            {
                dt.Rows.Add(
                i + 1
                , cList[i].assetrefno.ToString()
                , cList[i].assetdetDetid.ToString()
                , cList[i].assetreducedamt.ToString()
                , cList[i].assetreduval.ToString()
                , cList[i].assetnewval.ToString());
            }


            //export to excel from gridview
            GridView gv = new GridView();
            gv.DataSource = dt;
            gv.DataBind();
            // Session["exceldownload"] = gv;
            if (gv.Rows.Count != 0)
            {
                //return new DownloadFileActionResult((GridView)Session["exceldownload"], "DocumentGroup.xls");
                return new DownloadFileActionResult(gv, "Asset Value Reduction Summary.xls");
            }
            else
            {
                ViewBag.Message = "No records found";
            }

            return RedirectToAction("Index");
        }
        #region /*Reduction Upload*/
        [HttpGet]
        public PartialViewResult VRBulk()
        {
            try
            {
                Session["TransferNo"] = null;
                Session["Status"] = null;
                Session["Errdatatable"] = null;
                Session["maindatatable"] = null;
                Session["RAISERattmtfileexcel"] = null;
                Session["orginal"] = null;
                Session["err"] = null;
                Session["Tempdataset"] = null;
                Session["filename"] = null;
                Session["Tempdatatable"] = null;
                Session["maindatatable"] = null;
                Session["gid"] = null;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
            finally
            {
            }
            return PartialView();
        }
        public void UploadFilesnew()
        {
            try
            {
                string filename = "";
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                        Session["RAISERattmtfileexcel"] = hpf;
                        Session["orginal"] = hpf.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
            finally
            {
            }
        }
        [HttpPost]
        public async Task<JsonResult> UploadFiles()
        {
            string Extension1 = "";
            string error = "";
            try
            {
                string path1 = "";
                if (Session["RAISERattmtfileexcel"] != null)
                {
                    HttpPostedFileBase savefile = (HttpPostedFileBase)Session["RAISERattmtfileexcel"];
                    string Extension = Path.GetFileName(savefile.FileName);
                    string n = string.Format("AVR{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
                    Extension1 = System.IO.Path.GetExtension(savefile.FileName);
                    Session["filename"] = n + " _ " + Extension;
                    var CmnFilePath = objcmnAssetVR.IEMAttachmentPath();
                    path1 = CmnFilePath + n + " _ " + Extension;
                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);
                    }
                    savefile.SaveAs(path1);
                }
                List<SheetData> objparent = new List<SheetData>();
                SheetData objModel;
                int count = 0;
                string sheets = "";
                FileStream stream = new FileStream(path1, FileMode.Open, FileAccess.Read);
                DataSet result1 = new DataSet();
                if (Extension1 == ".xlsx")
                {
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    excelReader.IsFirstRowAsColumnNames = true;
                    result1 = excelReader.AsDataSet();
                    excelReader.Close();
                }
                else if (Extension1 == ".xls")
                {
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    excelReader.IsFirstRowAsColumnNames = true;
                    result1 = excelReader.AsDataSet();
                    excelReader.Close();
                }
                else
                {
                    error = "Error";
                    count++;
                    objModel = new SheetData();
                    objModel.SheetId = count;
                    objModel.SheetName = error.ToString();
                    objparent.Add(objModel);
                }
                foreach (DataTable dt in result1.Tables)
                {
                    sheets = dt.TableName.ToString().Trim();
                    count++;
                    objModel = new SheetData();
                    objModel.SheetId = count;
                    objModel.SheetName = sheets.ToString();
                    objparent.Add(objModel);
                }
                Session["Tempdataset"] = result1;
                return Json(objparent.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }
        }
        public JsonResult Makerupdate(TransferMakerModel obj)
        {
            string mag = "";
            string strCols = "";
            string[] strColArr;
            try
            {
                DataSet result1 = new DataSet();
                result1 = (DataSet)Session["Tempdataset"];
                var dataTable = new DataTable();
                dataTable = result1.Tables[obj.SheetName.ToString()];
                foreach (DataColumn dtColumn in dataTable.Columns)
                {
                    strCols = strCols + dtColumn.ColumnName.Trim() + ":";
                }
                strCols = strCols.Substring(0, strCols.Length - 1);
                strColArr = strCols.Split(':');
                if (strColArr[0].ToString() == "SNo"
                    && strColArr[1].ToString() == "Asset ID"
                    && strColArr[2].ToString() == "Type (Addition / Reduction)"
                    && strColArr[3].ToString() == "Effective date"
                    && strColArr[4].ToString() == "Amount"
                    && strColArr[5].ToString() == "Remarks"
                    && strColArr.Count().ToString() == "6")
                {
                    mag = "Success";
                }
                else
                {
                    mag = "Faild";
                }
                if (mag == "Success")
                {
                    int count = dataTable.Rows.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (dataTable.Rows[i]["SNo"].ToString() == "" && dataTable.Rows[i]["Asset ID"].ToString() == ""
                            && dataTable.Rows[i]["Asset ID"].ToString() == ""
                            && dataTable.Rows[i]["Type (Addition / Reduction)"].ToString() == ""
                            && dataTable.Rows[i]["Effective date"].ToString() == ""
                            && dataTable.Rows[i]["Amount"].ToString() == ""
                            && dataTable.Rows[i]["Remarks"].ToString() == "")
                        {
                            dataTable.Rows[i].Delete();
                            dataTable.AcceptChanges();
                            count--;
                            i--;
                        }
                    }
                    Session["Tempdatatable"] = dataTable;
                }
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
            return Json(mag, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult Uploadstatus(string ddlSelectsheet)
        {
            try
            {
                if (Session["Tempdatatable"] != null)
                {
                    DataTable errdataval = new DataTable();
                    errdataval = (DataTable)Session["Tempdatatable"];
                    datasupload(errdataval);
                    errdataval = (DataTable)Session["Errdatatable"];
                    if (errdataval.Rows.Count == 0)
                    {
                        ViewBag.visble = "Yes";
                    }
                }
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
            return PartialView(string.Empty);
        }
        private void datasupload(DataTable dataTable)
        {
            int sno = 0;
            int totalrecord = 0;
            int invalid = 0;
            int valid = 0;
            string duplicateasset = "";
            string notduplicatenewbranch = "";
            string grpId1 = "";
            string grpId2 = "";
            DataTable maindatatable = new DataTable();
            maindatatable = dataTable;
            totalrecord = maindatatable.Rows.Count;
            DataTable Errdatatable = new DataTable();
            Errdatatable.Columns.Add("SNo");
            Errdatatable.Columns.Add("Line");
            Errdatatable.Columns.Add("Error Description");
            try
            {
                if (dataTable.Rows.Count > 0)
                {
                    string exceltext = "";
                    string assetdata = "";
                    string assetbranch = "";
                    string status = "";
                    string errs = "";
                    int RowNo = 0;
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        errs = "";
                        RowNo = i + 1;
                        assetdata = dataTable.Rows[i][1].ToString();
                        if (duplicateasset.Contains(assetdata) == true)
                        {
                            errs = "Duplicate Asset id Found";
                        }
                        else
                        {
                            if (dataTable.Rows[i][1].ToString() == "")
                            {
                                errs = "Asset Id must not be empty ";
                            }
                            else
                            {
                                exceltext = dataTable.Rows[i][1].ToString();
                                status = Tfr.GetStatusexcel(assetdata, exceltext, "AssetIdCheck");
                                if (status == "Free")
                                {
                                    status = Tfr.GetStatusexcel(assetdata, exceltext, "AssetIdProcessCheck");
                                    if (status == "Free")
                                    {
                                    }
                                    else
                                    {
                                        if (errs == "")
                                        {
                                            errs = status;
                                        }
                                        else
                                        {
                                            errs = errs + "," + status;
                                        }
                                    }
                                }
                                else
                                {
                                    if (errs == "")
                                    {
                                        errs = status;
                                    }
                                    else
                                    {
                                        errs = errs + "," + status;
                                    }
                                }
                            }
                        }
                        if (dataTable.Rows[i][2].ToString() == "")
                        {
                            if (errs == "")
                            {
                                errs = "Type (Addition / Reduction) must not be empty.";
                            }
                            else
                            {
                                errs = errs + "," + "Type (Addition / Reduction) must not be empty.";
                            }
                        }
                        if (dataTable.Rows[i][3].ToString() == "")
                        {
                            if (errs == "")
                            {
                                errs = "Effective date must not be empty.";
                            }
                            else
                            {
                                errs = errs + "," + "Effective date must not be empty.";
                            }
                        }
                        if (dataTable.Rows[i][3].ToString() == "")
                        {
                            if (errs == "")
                            {
                                errs = "Amount must not be empty/Amont must not be Zero.";
                            }
                            else
                            {
                                errs = errs + "," + "Amount must not be empty/Amont must not be Zero.";
                            }
                        }
                        if (dataTable.Rows[i][4].ToString() == "")
                        {
                            if (errs == "")
                            {
                                errs = "Remarks must not be empty.";
                            }
                            else
                            {
                                errs = errs + "," + "Remarks must not be empty.";
                            }
                        }
                        if (errs != "")
                        {
                            sno++;
                            Errdatatable.Rows.Add(sno, RowNo, errs);
                        }
                    }
                }
                else
                {
                    Errdatatable.Rows.Add(1, "Please select a valid sheet");
                }
                duplicateasset = "";
                invalid = Errdatatable.Rows.Count;
                valid = totalrecord - invalid;
                ViewBag.vbvalid = "Total No of Vaild Record(s) :" + valid;
                ViewBag.vbinvalid = "Total No of InVaild Record(s) :" + invalid;
                ViewBag.vbtotalrecord = "Total No of Record(s) in Excel File :" + totalrecord;
                Session["Errdatatable"] = Errdatatable;
                Session["maindatatable"] = maindatatable;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
            finally
            {
            }
        }
        [HttpGet]
        public PartialViewResult localdetails(string ddlSelectsheet)
        {
            try { Session["err"] = "flag"; }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
            return PartialView();
        }
        public JsonResult localdetails() // For File Upload 
        {
            try
            {
                Session["err"] = "flag";
                DataTable uploadedData = new DataTable();
                uploadedData = (DataTable)Session["maindatatable"];
                string check;
                string assettype = "AVR";
                string filename = Session["filename"].ToString();
                check = ivr.ImportAssetDetails(uploadedData, filename, assettype).ToString();
                return Json(check, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #region Attachments
        public void UploadFilesnewAttach()
        {
            try
            {
                string filename = "";
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                        Session["Attachfile"] = hpf;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
            finally
            {
            }
        }
        public string UploadAttachments(AssetVRModel _data)
        {
            string Result = string.Empty; Int64 AttachmentId = 0;
            DataTable dt = new DataTable();
            try
            {
                string filename = "";
                HttpPostedFileBase hpf = (HttpPostedFileBase)Session["Attachfile"];
                filename = Path.GetFileName(hpf.FileName);
                _data.AttachName = filename;
                var CmnFilePath = System.Configuration.ConfigurationManager.AppSettings["FAFilePath"].ToString();
                dt = ivr.UploadAttachments(_data);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Clear"] == "1")
                    {
                        AttachmentId = Convert.ToInt64(dt.Rows[0]["AttachmentId"]);
                        CmnFilePath = CmnFilePath + AttachmentId + " _ " + filename;
                        hpf.SaveAs(CmnFilePath);
                    }
                    else
                    {
                        Result = Convert.ToString(dt.Rows[0]["Message"]);
                        return Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                Result = ex.Message.ToString();
                return Result;
            }
            finally
            {
            }
            return Result;
        }
        public ActionResult downloadsexcel()
        {
            try
            {
                Session["err"] = "";
                DataTable dtnew = (DataTable)Session["Errdatatable"];
                DataTable dtmainnew = (DataTable)Session["maindatatable"];
                dtmainnew.Columns.Add("Error Description");
                for (int i = 1; i <= dtmainnew.Rows.Count; i++)
                    for (int j = 0; j < dtnew.Rows.Count; j++)
                    {
                        int line = Convert.ToInt32(dtnew.Rows[j]["Line"]);
                        if (i == line)
                            dtmainnew.Rows[line - 1]["Error Description"] = dtnew.Rows[j]["Error Description"];
                    }
                DataTable dt = dtmainnew;
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "AVR_Template");
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=AVR_ErrorLog.xlsx");
                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public JsonResult CheckFileExists(string id)
        {
            string result = "0";
            try
            {
                var CmnFilePath = objCmnFunctions.IEMAttachmentPath();
                var localpath = Path.Combine(CmnFilePath, id);
                var apiresult = Cmnfs.DownloadFile(id).Result;
                if (apiresult != "Fail")
                {
                    result = "1";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public FileResult DownloadDocument(string id)
        {
            try
            {
                string filename = id;
                var CmnFilePath = objCmnFunctions.IEMAttachmentPath();
                var path = Path.Combine(CmnFilePath, filename);
                var apiresult = Cmnfs.DownloadFile(id).Result;
                if (apiresult == "Fail")
                {
                    return File("", "");
                }
                else
                {
                    byte[] filebyte = Convert.FromBase64String(apiresult);
                    return File(filebyte, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
                }
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return File("", "");
            }
        }
        public string AbortAttachments(string _Action, Int64 AttachIds)
        {
            string Result = string.Empty;
            AssetVRModel _data = new AssetVRModel();
            _data.AttachId = AttachIds;
            _data.Type = _Action;
            try
            {
                Result = ivr.AbortAttachments(_data).ToString();
                return Result;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Result;
            }
        }
        public ActionResult VRAttachment()
        {
            AssetVRModel _data = new AssetVRModel();
            _data.attachLst = Enumerable.Empty<AssetVRModel>().ToList<AssetVRModel>();
            return PartialView(_data);
        }
        public ActionResult samdownloadsexcel() //Download Template Click
        {
            using (DataTable dtnew = new DataTable())
            {
                dtnew.Columns.Add("SNo");
                dtnew.Columns.Add("Asset ID");
                dtnew.Columns.Add("Type (Addition / Reduction)");
                dtnew.Columns.Add("Effective date");
                dtnew.Columns.Add("Amount");
                dtnew.Columns.Add("Remarks");
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtnew, "Transfer_Template");
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=ValueReduction_Template.xlsx");
                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            return View();
        }
        #endregion
        #endregion
    }
}
