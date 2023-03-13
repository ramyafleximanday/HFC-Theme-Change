

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
    public class TDSSpecialRateUploadController : Controller
    {
        //
        // GET: /ASMS/TDSSpecialRateUpload/
        private fsIreposiroty objModel;
        ErrorLog objErrorLog = new ErrorLog();
        CmnFunctions objCmnFunctions = new CmnFunctions();
        private FileServer Cmnfs = new FileServer();
        FlexispendDataModel objd = new FlexispendDataModel();

        public ActionResult Index()
        {
            return View();
        }
        public TDSSpecialRateUploadController()
            : this(new FlexispendDataModel())
        {

        }

        public TDSSpecialRateUploadController(fsIreposiroty objM)
        {
            objModel = objM;
        }


        [HttpGet]
        public ActionResult TDSSpecialRateUpload()
        {

            return View();
        }

        [HttpPost]
        public void UploadFilesnew()
        {
            try
            {
                foreach (string filenew in Request.Files)
                {
                    var fileContent = Request.Files[filenew];

                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        HttpPostedFileBase hpf = Request.Files[filenew] as HttpPostedFileBase;
                        TempData["_supplierattmtfileexcel"] = hpf;
                        Session["_supplierattmtfileexcel"] = hpf;

                    }
                }
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
        }

        //To Check Excel File Valid or not
        [HttpPost]
        public async Task<JsonResult> UploadFiles()
        {

            // Get Excel Sheet Name
            List<IEM.Areas.IFAMS.Models.SheetData> objparent = new List<IEM.Areas.IFAMS.Models.SheetData>();
            string Extensionnew = "";
            string Extension1 = "";
            string error = "";
            try
            {
                string path1 = "";
                if (TempData["_supplierattmtfileexcel"] != null)
                {
                    HttpPostedFileBase savefile = TempData["_supplierattmtfileexcel"] as HttpPostedFileBase;
                    string Extension = Path.GetFileName(savefile.FileName);
                    TempData["gstrfilename"] = Path.GetFileName(savefile.FileName);

                    string n = string.Format(objCmnFunctions.GetLoginUserGid().ToString() + "Local-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
                    Extension1 = System.IO.Path.GetExtension(savefile.FileName);
                    Extensionnew = n + Extension1;
                    path1 = string.Format("{0}{1}", HoldFileUploadUrlDSA(), Extensionnew);
                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);
                    }
                    savefile.SaveAs(path1);

                    TempData["gstrfilpath"] = path1;

                    IEM.Areas.IFAMS.Models.SheetData objModel;

                    int count = 0;
                    string sheets = "";
                    FileStream stream = new FileStream(path1, FileMode.Open, FileAccess.Read);
                    DataSet result1 = new DataSet();
                    // To check Excel File Extension
                    if (Extension1.ToLower() == ".xlsx")
                    {
                        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        excelReader.IsFirstRowAsColumnNames = true;
                        result1 = excelReader.AsDataSet();
                        excelReader.Close();
                    }
                    else if (Extension1.ToLower() == ".xls")
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
                        objModel = new IEM.Areas.IFAMS.Models.SheetData();
                        objModel.SheetId = count;
                        objModel.SheetName = error.ToString();
                        objparent.Add(objModel);
                    }


                    if (result1 != null && result1.Tables.Count > 0)
                    {
                        foreach (DataTable dt in result1.Tables)
                        {
                            sheets = dt.TableName.ToString().Trim();
                            count++;
                            objModel = new IEM.Areas.IFAMS.Models.SheetData();
                            objModel.SheetId = count;
                            objModel.SheetName = sheets.ToString();
                            objparent.Add(objModel);
                        }
                    }

                    Session["Tempdataset"] = result1;
                    // TempData.Remove("_supplierattmtfileexcel");
                }
                return Json(objparent.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("Error occurred..." + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public string HoldFileUploadUrlDSA()
        {
            string x = "";
            try
            {
                x = System.Configuration.ConfigurationManager.AppSettings["HoldFileUpload"].ToString();
            }
            catch { x = ""; }
            return (x == null || x.Trim() == "") ? "" : x;
        }


        //Excel File Header Valid Or Not
        public JsonResult Localconveyancestatuserrs(EOW_TravelClaim obj)
        {
            string mag = "";
            string strCols = "";
            DataTable duptable = new DataTable();
            string[] strColArr;
            string ExcelResult = "";
            try
            {
                DataSet result1 = new DataSet();
                result1 = (DataSet)Session["Tempdataset"];
                var dataTable = new DataTable();
                dataTable = result1.Tables[obj.Branch.ToString()];
                Session["Tempdatatablesu"] = dataTable;


                //selva 28-01-2021
                var UniqueRows = dataTable.AsEnumerable().Distinct(DataRowComparer.Default);
                duptable = UniqueRows.CopyToDataTable();
                if (duptable.Rows.Count != dataTable.Rows.Count)
                {
                    mag = "Duplicate Record Found";

                }

                else
                {

                    foreach (DataColumn dtColumn in dataTable.Columns)
                    {
                        strCols = strCols + dtColumn.ColumnName.Trim() + ":";
                    }

                    strCols = strCols.Substring(0, strCols.Length - 1);
                    strColArr = strCols.Split(':');

                    if ((strColArr[0].ToString() == "Supplier Code"
                    && strColArr[1].ToString() == "PAN No"
                    && strColArr[2].ToString() == "Tax"
                    && strColArr[3].ToString() == "Tax Sub-Type"
                    && strColArr[4].ToString() == "Rate"
                    && strColArr.Count().ToString() == "5"
                    ))
                    {

                        mag = "Success";
                    }
                    else
                    {
                        //To Check Excel Column Count And ExcelColumn Name Valid or Not
                        if ((strColArr.Count().ToString() == "5") && (dataTable.Columns[0].ToString().Trim().ToUpper() == "Supplier Code"
                            && dataTable.Columns[1].ToString().Trim().ToUpper() == "PAN No"
                            && dataTable.Columns[2].ToString().Trim().ToUpper() == "Tax"
                            && dataTable.Columns[3].ToString().Trim().ToUpper() == "Tax Sub-Type"
                            && dataTable.Columns[4].ToString().Trim().ToUpper() == "Rate"

                            ))
                        {
                            if (Session["Tempdatatablesu"] != null)
                            {
                                DataTable errdataval = new DataTable();
                                errdataval = (DataTable)Session["Tempdatatablesu"];
                                //ExcelResult = datasupload(errdataval);
                                // ExcelResult = datasupload(errdataval);
                                errdataval = (DataTable)Session["Errdatatablesu"];
                                mag = "Success";
                            }
                        }

                        else
                        {
                            mag = "Faild";
                        }
                    }

                }



                return Json(mag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("Error occurred..." + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public PartialViewResult _Supplierexcelstatus(string ddlSelectsheet)
        {
            try
            {
                if (Session["Tempdatatablesu"] != null)
                {
                    DataTable errdataval = new DataTable();
                    errdataval = (DataTable)Session["Tempdatatablesu"];
                    datasupload(errdataval);
                    errdataval = (DataTable)Session["Errdatatablesu"];
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
            return PartialView();
        }


        // To Check Excel data Empty and Valid Or Not 
        private string datasupload(DataTable dataTable)
        {
            int sno = 0;
            int totalrecord = 0;
            int invalid = 0;
            int valid = 0;
            string Result = "";
            DataTable maindatatable = new DataTable();
            DataTable duptable = new DataTable();
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

                    string status = "";
                    string errs = "";
                    int RowNo = 0;
                    int j = 1;

                    //selva Duplicate Record Checking in excel sheet

                    var UniqueRows = dataTable.AsEnumerable().Distinct(DataRowComparer.Default);
                    duptable = UniqueRows.CopyToDataTable();
                    if (duptable.Rows.Count != dataTable.Rows.Count)
                    {
                        Result = "Duplicate Record Found";
                        return Result;
                    }

                    else
                    {

                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {

                            errs = "";
                            RowNo = j + i;
                            //To check Taxable value Empty or not
                            if (dataTable.Rows[i][0].ToString() == "")
                            {
                                if (errs == "")
                                {
                                    errs = "Supplier Code Should not empty";
                                }
                                else
                                {
                                    errs = errs + "," + "Supplier Code Should not empty";
                                }
                            }

                            if (dataTable.Rows[i][1].ToString() == "")
                            {
                                if (errs == "")
                                {
                                    errs = "PAN No Should not empty";
                                }
                                else
                                {
                                    errs = errs + "," + "PAN No Should not empty";
                                }
                            }

                            else
                            {
                                exceltext = dataTable.Rows[i][0].ToString();
                                string value = dataTable.Rows[i][1].ToString();
                                status = objModel.GetTDSUploadStatusexcel(exceltext, "", "", "SupplierCode","");

                                if (status != value)
                                {
                                    if (errs == "")
                                    {
                                        errs = "Panno Not Match for this Supplier";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Panno Not Match for this Supplier";
                                    }
                                }
                            }


                            if (dataTable.Rows[i][2].ToString() == "")
                            {
                                if (errs == "")
                                {
                                    errs = "Tax Should not empty";
                                }
                                else
                                {
                                    errs = errs + "," + "Tax Should not empty";
                                }
                            }

                            else
                            {
                                exceltext = dataTable.Rows[i][2].ToString();

                                status = objModel.GetTDSUploadStatusexcel(exceltext, "", "", "Tax","");

                                if (status == "notexists")
                                {
                                    if (errs == "")
                                    {
                                        errs = "Tax Not in Master Data";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Tax Not in Master Data";
                                    }
                                }
                            }

                            // Tax Checking
                       /*     if (dataTable.Rows[i][2].ToString() != "")
                            {

                                string supcode = dataTable.Rows[i][0].ToString();
                                status = objModel.GetTDSUploadStatusexcel(supcode, "", "", "TDSTaxtype","");

                                string value = dataTable.Rows[i][2].ToString();

                                if (status != value)
                                {

                                    if (errs == "")
                                    {
                                        errs = "TaxCode Not Match Given Supplier";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "TaxCode Not Match Given Supplier";
                                    }
                                }

                            }*/

                            //Tax Subtype Checking with tdsdetails table

                           /* if (dataTable.Rows[i][3].ToString() != "")
                            {
                                string taxsubtypecode = dataTable.Rows[i][3].ToString();
                                status = objModel.GetTDSUploadStatusexcel(taxsubtypecode, "", "", "TDSTaxsubtype","");

                                if (status == "notexists")
                                {
                                    if (errs == "")
                                    {
                                        errs = "TaxSubtype Not Match";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "TaxSubtype Not Match";
                                    }
                                }
                            }*/



                            if (dataTable.Rows[i][3].ToString() == "")
                            {
                                if (errs == "")
                                {
                                    errs = "Tax Subtype Should not empty";
                                }
                                else
                                {
                                    errs = errs + "," + "Tax Subtype Should not empty";
                                }
                            }
                            /*else if (dataTable.Rows[i][3].ToString() != "")
                            {
                                string supcode = dataTable.Rows[i][0].ToString();
                                string taxsubtypecode = dataTable.Rows[i][3].ToString();
                                status = objModel.GetTDSUploadStatusexcel(taxsubtypecode, "", "", "TmpSubType", supcode);
                                if (status == "notexists")
                                {
                                    if (errs == "")
                                    {
                                        errs = "TDS Not Maintain For this Subtype in ASMS.";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "TDS Not Maintain For this Subtype in ASMS.";
                                    }
                                }
                                string supcode1 = dataTable.Rows[i][0].ToString();
                                string taxsubtypecode1 = dataTable.Rows[i][3].ToString();
                                status = objModel.GetTDSUploadStatusexcel(taxsubtypecode1, "", "", "TmpSubType", supcode1);
                                if (status == "notexists")
                                {
                                    if (errs == "")
                                    {
                                        errs = "TDS Not Maintain For this Subtype in ASMS Transaction.";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "TDS Not Maintain For this Subtype in ASMS Transaction.";
                                    }
                                }
                            }*/
                            else
                            {
                                exceltext = dataTable.Rows[i][3].ToString();

                                status = objModel.GetTDSUploadStatusexcel(exceltext, "", "", "TaxSubType","");

                                if (status == "notexists")
                                {
                                    if (errs == "")
                                    {
                                        errs = "Tax SubType Not in Master Data";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Tax SubType Not in Master Data";
                                    }
                                }
                            }

                            if (dataTable.Rows[i][4].ToString() == "")
                            {
                                if (errs == "")
                                {
                                    errs = "Rate Should not empty";
                                }
                                else
                                {
                                    errs = errs + "," + "Rate Should not empty";
                                }
                            }

                            if (dataTable.Rows[i][4].ToString() != "")
                            {
                                try
                                {
                                    decimal rate = Convert.ToDecimal(dataTable.Rows[i][4].ToString().ToString());

                                    if (rate < 0)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "Rate should be Greater than Zero";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Rate should be Greater than Zero";
                                        }
                                    }

                                    else if (rate > 100)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "Rate should not be greater than 100%";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Rate should not be greater than 100%!";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (errs == "")
                                    {
                                        errs = "Rate Amount is Invalid";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Rate Amount is Invalid";
                                    }
                                }
                            }




                            if (errs != "")
                            {
                                sno++;
                                Errdatatable.Rows.Add(sno, RowNo, errs);
                            }
                        }

                    }

                    //Selva Duplicate Record Database checking

                    if (Errdatatable.Rows.Count <= 0)
                    {
                        FS_GSTRModel tdsobjmodel = new FS_GSTRModel();
                        Errdatatable = objModel.ExceValidationTDSUpload(dataTable, tdsobjmodel);
                    }

                }

                else
                {
                    Errdatatable.Rows.Add(1, "Please  Select Valid Sheet");
                }
                invalid = Errdatatable.Rows.Count;
                valid = totalrecord - invalid;
                ViewBag.vbvalid = "Total No of Vaild Record :" + valid;
                ViewBag.vbinvalid = "Total No of InVaild Record :" + invalid;
                ViewBag.vbtotalrecord = "Total No of Record Excel File :" + totalrecord;

                Session["Errdatatablesu"] = Errdatatable;
                Session["maindatatablesu"] = maindatatable;
            }
            catch (Exception ex)
            {
                sno++;
                Errdatatable.Rows.Add(sno, Errdatatable.Rows.Count + 1, ex.Message.ToString() + " Please ckeck Excel File Error While Reading Data" + ex.ToString());

                invalid = Errdatatable.Rows.Count;
                valid = totalrecord - invalid;
                ViewBag.vbvalid = "Total No of Vaild Record :" + valid;
                ViewBag.vbinvalid = "Total No of InVaild Record :" + invalid;
                ViewBag.vbtotalrecord = "Total No of Record Excel File :" + totalrecord;

                Session["Errdatatablesu"] = Errdatatable;
                Session["maindatatablesu"] = maindatatable;
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
            return Result;
        }


        public JsonResult localdetailssu()
        {
            DataTable dt1 = new DataTable();
            string success = "";
            string filepath = "";
            string Extension1 = "";
            string Extensionnew = "";
            try
            {

                FS_GSTRModel gstrobjmodel = new FS_GSTRModel();
                //Get FileName With Content
                HttpPostedFileBase savefile = Session["_supplierattmtfileexcel"] as HttpPostedFileBase;

                //fileName
                string Extension = Path.GetFileName(savefile.FileName);
                //Get Login id Date and Time
                string n = string.Format(objCmnFunctions.GetLoginUserGid().ToString() + "Local-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
                //File Extension
                Extension1 = System.IO.Path.GetExtension(savefile.FileName);
                Extensionnew = n + Extension1;
                //FilePath
                filepath = string.Format("{0}{1}", GSTRFileUpload(), Extensionnew);
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
                //Save File in Path
                savefile.SaveAs(filepath);
                //get file name and file path
                gstrobjmodel.filename = Path.GetFileName(savefile.FileName);
                gstrobjmodel.filepath = filepath;
                DataTable dt = new DataTable();
                dt = (DataTable)Session["maindatatablesu"];
                success = objModel.TDSExcelGSTRUpload(dt, gstrobjmodel);
                var str = success.Split(';');
                var Header = str[1];

                Session["headerid"] = Header;


                return Json(success, JsonRequestBehavior.AllowGet);

            }


            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                // return Json("Error occurred..." + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }

            return Json(success, JsonRequestBehavior.AllowGet);

        }
        //GSTR File Save Path
        public string GSTRFileUpload()
        {
            string x = "";
            try
            {
                x = System.Configuration.ConfigurationManager.AppSettings["GSTRFileUpload"].ToString();
            }
            catch { x = ""; }
            return (x == null || x.Trim() == "") ? "" : x;
        }



        [HttpGet]
        public PartialViewResult _GetTDSFileList(string straction)
        {
            try
            {

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



        [HttpGet]
        public PartialViewResult _GetTDSUploadDetails(string straction, int id)
        {
            try
            {

                ViewBag.headervalue = id;

                List<FS_GSTRModel> lstGetEmployee = new List<FS_GSTRModel>();
                TempData["action"] = straction.ToString();
                Session["headerid"] = id;
                return PartialView(lstGetEmployee);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return PartialView();
            }
        }


        //Download Excel
        public ActionResult downloadsexcel()
        {
            DataTable dtnew = (DataTable)Session["Errdatatablesu"];
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "TDSSpecialrate.xls"));
            Response.ContentType = "application/vnd.ms-excel";
            DataTable dt = dtnew;
            string str = string.Empty;
            foreach (DataColumn dtcol in dt.Columns)
            {
                Response.Write(str + dtcol.ColumnName);
                str = "\t";
            }
            Response.Write("\n");
            foreach (DataRow dr in dt.Rows)
            {
                str = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    Response.Write(str + Convert.ToString(dr[j]));
                    str = "\t";
                }
                Response.Write("\n");
            }
            Response.End();
            return View();
        }



        [HttpGet]
        public PartialViewResult TDSFileCheckerSummary(string straction)
        {
            try
            {

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
                //  var path = Path.Combine(@"C:\Temp\", filename);
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

    }
}
