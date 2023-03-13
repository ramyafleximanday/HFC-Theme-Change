
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


namespace IEM.Areas.FLEXISPEND.Controllers
{
    public class GSTRUploadController : Controller
    {

        private fsIreposiroty objModel;
        ErrorLog objErrorLog = new ErrorLog();
        private FileServer Cmnfs = new FileServer();
        CmnFunctions objCmnFunctions = new CmnFunctions();
        CommonIUD objCommonIUD = new CommonIUD();
        dbLib db = new dbLib();
        proLib plib = new proLib();
        DataTable dt = new DataTable();
        IEM_MST_EXPENSECATEGORY expcat = new IEM_MST_EXPENSECATEGORY();
        // CmnFunctions objCmnFunctions = new CmnFunctions();

        // GET: /FLEXISPEND/GSTRUpload/
        public GSTRUploadController()
            : this(new FlexispendDataModel())
        {

        }

        public GSTRUploadController(fsIreposiroty objM)
        {
            objModel = objM;
        }
        public ActionResult GSTRUpload()
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
                    mag = "Duplicate Record Found in Excel!!";
                }
                else
                {
                    foreach (DataColumn dtColumn in dataTable.Columns)
                    {
                        strCols = strCols + dtColumn.ColumnName.Trim() + ":";
                    }


                    strCols = strCols.Substring(0, strCols.Length - 1);
                    strColArr = strCols.Split(':');
                    //To check Type is Regular or Credit Note
                    if (strColArr[3].ToString() == "Invoice type")
                    {

                        //to check excel column  
                        if ((strColArr[0].ToString() == "GSTIN of supplier"
                              && strColArr[1].ToString() == "Trade/Legal name"
                              && strColArr[2].ToString() == "Invoice number"
                              && strColArr[3].ToString() == "Invoice type"
                              && strColArr[4].ToString() == "Invoice Date"
                              && strColArr[5].ToString() == "Invoice Value(₹)"
                              && strColArr[6].ToString() == "Place of supply"
                              && strColArr[7].ToString() == "Supply Attract Reverse Charge"
                              && strColArr[8].ToString() == "Rate(%)"
                              && strColArr[9].ToString() == "Taxable Value (₹)"
                              && strColArr[10].ToString() == "Integrated Tax(₹)"
                      && strColArr[11].ToString() == "Central Tax(₹)"
                      && strColArr[12].ToString() == "State/UT Tax(₹)"
                  && strColArr[13].ToString() == "Cess(₹)"
                  && strColArr[14].ToString() == "GSTR-1/5 Period"
                  && strColArr[15].ToString() == "GSTR-1/5 Filing Date"
                  && strColArr[16].ToString() == "ITC Availability"
                  && strColArr[17].ToString() == "Reason"
                  && strColArr[18].ToString() == "Applicable"
                   && strColArr[19].ToString() == "HSN Code"
                     && strColArr.Count().ToString() == "20"))
                        {

                            mag = "Success";



                        }
                        else
                        {
                            //To Check Excel Column Count And ExcelColumn Name Valid or Not
                            if ((strColArr.Count().ToString() == "20") && (
                        dataTable.Columns[0].ToString().Trim().ToUpper() == "GSTIN of supplier" && dataTable.Columns[1].ToString().Trim().ToUpper() == "Trade/Legal name"
                            && dataTable.Columns[2].ToString().Trim().ToUpper() == "Invoice number" && dataTable.Columns[3].ToString().Trim().ToUpper() == "Invoice type"
                            && dataTable.Columns[4].ToString().Trim().ToUpper() == "Invoice Date" && dataTable.Columns[5].ToString().Trim().ToUpper() == "Invoice Value(₹)"
                            && dataTable.Columns[6].ToString().Trim().ToUpper() == "Place of supply" && dataTable.Columns[7].ToString().Trim().ToUpper() == "Supply Attract Reverse Charge"
                            && dataTable.Columns[8].ToString().Trim().ToUpper() == "Rate(%)" && dataTable.Columns[9].ToString().Trim().ToUpper() == "Taxable Value (₹)"
                            && dataTable.Columns[10].ToString().Trim().ToUpper() == "Integrated Tax(₹)" && dataTable.Columns[11].ToString().Trim().ToUpper() == "Central Tax(₹)"
                            && dataTable.Columns[12].ToString().Trim().ToUpper() == "State/UT Tax(₹)" && dataTable.Columns[13].ToString().Trim().ToUpper() == "Cess(₹)"
                            && dataTable.Columns[14].ToString().Trim().ToUpper() == "GSTR-1/5 Period" && dataTable.Columns[15].ToString().Trim().ToUpper() == "GSTR-1/5 Filing Date"
                            && dataTable.Columns[16].ToString().Trim().ToUpper() == "ITC Availability" && dataTable.Columns[17].ToString().Trim().ToUpper() == "Reason"
                            && dataTable.Columns[18].ToString().Trim().ToUpper() == "Applicable" && dataTable.Columns[19].ToString().Trim().ToUpper() == "HSN Code"))
                            {
                                if (Session["Tempdatatablesu"] != null)
                                {
                                    DataTable errdataval = new DataTable();
                                    errdataval = (DataTable)Session["Tempdatatablesu"];
                                    //ExcelResult = datasupload(errdataval);
                                    ExcelResult = datasupload(errdataval);
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




                    //if type is Credit note ,to check column count 
                    else
                    {


                        if ((strColArr[0].ToString() == "GSTIN of supplier"
                                                      && strColArr[1].ToString() == "Trade/Legal name"
                                                      && strColArr[2].ToString() == "Note number"
                                                      && strColArr[3].ToString() == "Note type"
                                                      && strColArr[4].ToString() == "Note Supply type"
                                                      && strColArr[5].ToString() == "Note date"
                                                      && strColArr[6].ToString() == "Note Value (₹)"
                                                      && strColArr[7].ToString() == "Place of supply"
                                                      && strColArr[8].ToString() == "Supply Attract Reverse Charge"
                                                      && strColArr[9].ToString() == "Rate(%)"
                                                      && strColArr[10].ToString() == "Taxable Value (₹)"
                                              && strColArr[11].ToString() == "Integrated Tax(₹)"
                                              && strColArr[12].ToString() == "Central Tax(₹)"
                                          && strColArr[13].ToString() == "State/UT Tax(₹)"
                                          && strColArr[14].ToString() == "Cess(₹)"
                                          && strColArr[15].ToString() == "GSTR-1/5 Period"
                                          && strColArr[16].ToString() == "GSTR-1/5 Filing Date"
                                          && strColArr[17].ToString() == "ITC Availability"
                                          && strColArr[18].ToString() == "Reason"
                                           && strColArr[19].ToString() == "Applicable"
                                              && strColArr[20].ToString() == "HSN Code"
                                             && strColArr.Count().ToString() == "21"))
                        {

                            mag = "Success";



                        }
                        else
                        {
                            //To Check Excel Column Count And ExcelColumn Name Valid or Not
                            if ((strColArr.Count().ToString() == "21") && (dataTable.Columns[0].ToString().Trim().ToUpper() == "GSTIN of supplier" && dataTable.Columns[1].ToString().Trim().ToUpper() == "Trade/Legal name"
                                && dataTable.Columns[2].ToString().Trim().ToUpper() == "Note number" && dataTable.Columns[3].ToString().Trim().ToUpper() == "Note type"
                                && dataTable.Columns[4].ToString().Trim().ToUpper() == "Note Supply type" && dataTable.Columns[5].ToString().Trim().ToUpper() == "Note date"
                                && dataTable.Columns[6].ToString().Trim().ToUpper() == "Note Value (₹)" && dataTable.Columns[7].ToString().Trim().ToUpper() == "Place of supply"
                                && dataTable.Columns[8].ToString().Trim().ToUpper() == "Supply Attract Reverse Charge" && dataTable.Columns[9].ToString().Trim().ToUpper() == "Rate(%)"
                                && dataTable.Columns[10].ToString().Trim().ToUpper() == "Taxable Value (₹)" && dataTable.Columns[11].ToString().Trim().ToUpper() == "Integrated Tax(₹)"
                                && dataTable.Columns[12].ToString().Trim().ToUpper() == "Central Tax(₹)" && dataTable.Columns[13].ToString().Trim().ToUpper() == "State/UT Tax(₹)"
                                && dataTable.Columns[14].ToString().Trim().ToUpper() == "Cess(₹)" && dataTable.Columns[15].ToString().Trim().ToUpper() == "GSTR-1/5 Period"
                                && dataTable.Columns[16].ToString().Trim().ToUpper() == "GSTR-1/5 Filing Date" && dataTable.Columns[17].ToString().Trim().ToUpper() == "ITC Availability"
                                && dataTable.Columns[18].ToString().Trim().ToUpper() == "Reason" && dataTable.Columns[19].ToString().Trim().ToUpper() == "Applicable"
                                && dataTable.Columns[20].ToString().Trim().ToUpper() == "HSN Code"))
                            {
                                if (Session["Tempdatatablesu"] != null)
                                {
                                    DataTable errdataval = new DataTable();
                                    errdataval = (DataTable)Session["Tempdatatablesu"];
                                    //ExcelResult = datasupload(errdataval);
                                    ExcelResult = datasupload(errdataval);
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
                            //ramya 16 Feb 21
                            #region Regular
                            if (dataTable.Rows[i][3].ToString() == "Regular")
                            {
                                errs = "";
                                RowNo = j + i;
                                //To check Taxable value Empty or not
                                //if (dataTable.Rows[i][9].ToString() == "")
                                //{
                                //    if (errs == "")
                                //    {
                                //        errs = "Taxable Value Invalid";
                                //    }
                                //    else
                                //    {
                                //        errs = errs + "," + "Taxable Value Invalid";
                                //    }
                                //}
                                //else
                                //{

                                try
                                {
                                    decimal TaxAmount = Convert.ToDecimal(dataTable.Rows[i][9].ToString());
                                    //if (TaxAmount <= 0)
                                    if (TaxAmount < 0) // removed = for 100% GST invoice upload
                                    {
                                        if (errs == "")
                                        {
                                            errs = "Taxable Value is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Taxable Value is Invalid";
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    if (errs == "")
                                    {
                                        errs = "Taxable Value is Invalid";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Taxable Value is Invalid";
                                    }

                                }
                                //}
                                //  //To check Invoice Amount Empty or not
                                //if (dataTable.Rows[i][5].ToString() == "")
                                //{
                                //    if (errs == "")
                                //    {
                                //        errs = "Invoice Value Should Not be Empty";
                                //    }
                                //    else
                                //    {
                                //        errs = errs + "," + "Invoice Value Should Not be Empty";
                                //    }
                                //}
                                //else
                                //{
                                try
                                {
                                    decimal invoiceAmount = Convert.ToDecimal(dataTable.Rows[i][5].ToString());
                                    if (invoiceAmount <= 0)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "Invoice value is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Invoice value is Invalid";
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    if (errs == "")
                                    {
                                        errs = "Invoice Value is Invalid";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Invoice Value is Invalid";
                                    }

                                }
                                //}

                                //CGST Amount
                                if (dataTable.Rows[i][11].ToString() != "")
                                {
                                    try
                                    {
                                        decimal cgstvalue = Convert.ToDecimal(dataTable.Rows[i][11].ToString().ToString());

                                        if (cgstvalue < 0)
                                        {
                                            if (errs == "")
                                            {
                                                errs = "CGST Amount is Invalid";
                                            }
                                            else
                                            {
                                                errs = errs + "," + "CGST Amount is Invalid";
                                            }
                                        }


                                    }
                                    catch (Exception)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "CGST Amount is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "CGST Amount is Invalid";
                                        }
                                    }
                                }



                                //SGST Amount
                                if (dataTable.Rows[i][12].ToString() != "")
                                {
                                    try
                                    {
                                        decimal SGStvalue = Convert.ToDecimal(dataTable.Rows[i][12].ToString().ToString());
                                        if (SGStvalue < 0)
                                        {
                                            if (errs == "")
                                            {
                                                errs = "SGST Amount is Invalid";
                                            }
                                            else
                                            {
                                                errs = errs + "," + "SCGST Amount is Invalid";
                                            }
                                        }

                                    }
                                    catch (Exception)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "SGST Amount is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "SGST Amount is Invalid";
                                        }
                                    }
                                }
                                //IGST Amount
                                if (dataTable.Rows[i][10].ToString() != "")
                                {
                                    try
                                    {
                                        decimal IGSTvalue = Convert.ToDecimal(dataTable.Rows[i][10].ToString());
                                        if (IGSTvalue < 0)
                                        {
                                            if (errs == "")
                                            {
                                                errs = "IGST Amount is Invalid";
                                            }
                                            else
                                            {
                                                errs = errs + "," + "IGST Amount is Invalid";
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "IGST Amount is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "IGST Amount is Invalid";
                                        }
                                    }
                                }
                                //To check Place of Supply Empty or Not
                                if (dataTable.Rows[i][6].ToString() == "")
                                {
                                    if (errs == "")
                                    {
                                        errs = "Place of Supply Should Not be Empty";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Place of Supply Should Not be Empty";
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        // To check Place of Supply Valid Or Not
                                        exceltext = dataTable.Rows[i][6].ToString();

                                        status = objModel.GetGSTRUploadStatusexcel("", exceltext, "", "ReceiverLocation");
                                        if (status == "notexists")
                                        {
                                            if (errs == "")
                                            {
                                                errs = "Place of Supply Not in Master Data ";
                                            }
                                            else
                                            {
                                                errs = errs + "," + "Place of Supply Not in Master Data ";
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "Place of Supply Not in Master Data ";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Place of Supply Not in Master Data ";
                                        }
                                    }
                                }



                                //To check GSTIN of Supplier Empty or Not
                                if (dataTable.Rows[i][0].ToString() == "")
                                {
                                    if (errs == "")
                                    {
                                        errs = "GSTIN of Supplier Should Not be Empty ";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "GSTIN of Supplier Should Not be Empty ";
                                    }
                                }
                                else
                                {
                                    // To check GSTIN of Supplier Valid Or Not
                                    try
                                    {
                                        exceltext = dataTable.Rows[i][0].ToString();
                                        var Supplier = dataTable.Rows[i][1].ToString();
                                        status = objModel.GetGSTRUploadStatusexcel(exceltext, Supplier, "", "GSTNCode");
                                    }
                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "GSTIN of Supplier is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "GSTIN of Supplier is Invalid";
                                        }
                                    }
                                }
                                //GSTN ADOCH End


                                //Get Receiver Location 

                                exceltext = dataTable.Rows[i][0].ToString();

                                string value = dataTable.Rows[i][6].ToString();

                                status = objModel.GetGSTRUploadStatusexcel(exceltext, " ", " ", "ProviderLocation");
                                /*17-07-2021 GSTN Validation Start*/
                                if (status == "notexists")
                                {
                                    if (errs == "")
                                    {
                                        errs = "GSTIN - " +exceltext+" not maintain for this Vendor OR check Vendor Status!";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "GSTIN - " + exceltext + " not maintain for this Vendor OR check Vendor Status!";
                                    }
                                }
                                else
                                {
                                    //To check Provider Location and Receiver Location same or different
                                    if (value == status)
                                    {
                                        // To check Provider Location and Receiver Location   same
                                        //To check CGST ,SGSt Amount EMpty or not
                                        if ((dataTable.Rows[i][11].ToString() == "") && (dataTable.Rows[i][12].ToString() == ""))
                                        {
                                            if (errs == "")
                                            {
                                                errs = "Central and State Tax(₹) Should Not be Empty";
                                            }
                                            else
                                            {
                                                errs = errs + "," + "Central and State Tax(₹) Should Not be Empty";
                                            }
                                        }

                                        //invoice amount should be equal to taxable value and cgst sgst , Provider location Receiver Location is Same
                                        else
                                        {

                                            try
                                            {
                                                decimal CGSTAmount = Convert.ToDecimal(dataTable.Rows[i][11].ToString());
                                                decimal SGSTAmount = Convert.ToDecimal(dataTable.Rows[i][12].ToString());


                                                if (CGSTAmount > 0 && SGSTAmount > 0)
                                                {
                                                    decimal Invoiceamt = Convert.ToDecimal(dataTable.Rows[i][5].ToString());
                                                    decimal taxamt = Convert.ToDecimal(dataTable.Rows[i][9].ToString());
                                                    decimal Cgst = Convert.ToDecimal(dataTable.Rows[i][11].ToString());
                                                    decimal Sgst = Convert.ToDecimal(dataTable.Rows[i][12].ToString());
                                                    decimal totalamt = ((Convert.ToDecimal(taxamt)) + (Convert.ToDecimal(Cgst)) + (Convert.ToDecimal(Sgst)));
                                                    if (Invoiceamt != totalamt)
                                                    {

                                                        if (errs == " ")
                                                        {
                                                            errs = "Invoice Amount is not equal to sum of Taxable Amount + CGST and SGST Amount ";
                                                        }
                                                        else
                                                        {
                                                            errs = errs + "," + "Invoice Amount is not equal to sum of Taxable Amount + CGST and SGST Amount  ";
                                                        }
                                                    }

                                                    if (taxamt > Invoiceamt)
                                                    {
                                                        if (errs == "")
                                                        {
                                                            errs = "Taxable amount should not be greater than Inovice Amount";
                                                        }
                                                        else
                                                        {
                                                            errs = errs + "," + "Taxable amount should not be greater than Inovice Amount!";
                                                        }
                                                    }
                                                }

                                                else if (CGSTAmount < 0 && SGSTAmount < 0)
                                                {
                                                    if (errs == "")
                                                    {
                                                        errs = "CGST,SGST Amount should be greater than Zero";
                                                    }
                                                    else
                                                    {
                                                        errs = errs + "," + "CGST,SGST Amount should be greater than Zero";
                                                    }
                                                }

                                                //else 
                                                //{
                                                //    if (errs == "")
                                                //    {
                                                //        errs = "CGST,SGST Amount should be Greater than Zero";
                                                //    }
                                                //    else
                                                //    {
                                                //        errs = errs + "," + "CGST,SGST Amount should be Greater than Zero";
                                                //    }
                                                //}
                                            }

                                            catch (Exception)
                                            {
                                                if (errs == "")
                                                {
                                                    errs = " CGST,SGST Amount is Invalid ";
                                                }
                                                else
                                                {
                                                    errs = errs + "," + "CGST,SGST Amount is Invalid ";
                                                }
                                            }
                                        }
                                    }

                                    //To check Provider Location and Receiver Location   different
                                    else
                                    {
                                        // IGST amount empty or not
                                        if (dataTable.Rows[i][10].ToString().Trim() == "")
                                        {
                                            if (errs == "")
                                            {
                                                errs = "Integrated Tax should not be empty";

                                            }
                                            else
                                            {
                                                errs = errs + "," + "Integrated Tax should not be empty";
                                            }
                                        }
                                        else
                                        {
                                            //invoice amount should be equal to taxable value and Igst Amount Provider location Receiver Location is different
                                            try
                                            {
                                                decimal IGSTAmount = Convert.ToDecimal(dataTable.Rows[i][10].ToString());

                                                if (IGSTAmount > 0)
                                                {
                                                    // try
                                                    // {
                                                    decimal Invoiceamount = Convert.ToDecimal(dataTable.Rows[i][5].ToString());
                                                    decimal taxaount = Convert.ToDecimal(dataTable.Rows[i][9].ToString());
                                                    decimal IGST = Convert.ToDecimal(dataTable.Rows[i][10].ToString());
                                                    decimal totalamtIgst = ((Convert.ToDecimal(taxaount)) + (Convert.ToDecimal(IGST)));

                                                    if (Invoiceamount != totalamtIgst)
                                                    {

                                                        if (errs == "")
                                                        {
                                                            errs = "Invoice Amount should be equal to sum of Taxable amount and IGST Amount ";
                                                        }
                                                        else
                                                        {
                                                            errs = errs + "," + "Invoice Amount should be equal to sum of Taxable amount and IGST Amount  ";
                                                        }
                                                    }
                                                    if (taxaount > Invoiceamount)
                                                    {
                                                        if (errs == "")
                                                        {
                                                            errs = "Taxable amount should not be greater than Inovice Amount";
                                                        }
                                                        else
                                                        {
                                                            errs = errs + "," + "Taxable amount should not be greater than Inovice Amount!";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (errs == "")
                                                    {
                                                        errs = "IGST Amount should be greater than Zero";
                                                    }
                                                    else
                                                    {
                                                        errs = errs + "," + "IGST Amount should be greater than Zero";
                                                    }
                                                }
                                            }

                                            catch (Exception)
                                            {
                                                if (errs == "")
                                                {
                                                    errs = "IGST Amount is Invalid";
                                                }
                                                else
                                                {
                                                    errs = errs + "," + "IGST Amount is Invalid";
                                                }
                                            }

                                        }
                                    }
                                } /*17-07-2021 GSTN Validation End*/

                                //To check Invoice number empty or not   || Convert.ToDecimal(dataTable.Rows[i][2].ToString().ToString()) <= 0
                                if (dataTable.Rows[i][2].ToString() == "" || dataTable.Rows[i][2].ToString() == "0")
                                {
                                    if (errs == "")
                                    {
                                        errs = "Invoice number should not be Empty/Zero";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Invoice number should not be Empty/Zero";
                                    }
                                }

                                else
                                {
                                    try
                                    {
                                        string invoiceno = dataTable.Rows[i][2].ToString();

                                        //to validate Duplicate Invoice
                                        string gstin = dataTable.Rows[i][0].ToString();

                                        status = objModel.GetGSTRUploadStatusexcel(gstin, invoiceno, "", "DuplicateInvoice");
                                        if (status == "Exists")
                                        {
                                            if (errs == "")
                                            {
                                                errs = "Duplicate Invoice Number - " + invoiceno + " : Invoice No already Exists!"; //ramya added 16 Feb 21
                                            }
                                            else
                                            {
                                                errs = errs + "," + "Duplicate Invoice Number - " + invoiceno + " : Invoice No already Exists!";
                                            }
                                        }

                                    }
                                    catch (Exception)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "Invoice number is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Invoice number is Invalid";
                                        }
                                    }

                                }
                                ////To Check Invoice Type Emty or not
                                //if (dataTable.Rows[i][3].ToString() == "")
                                //{

                                //    if (errs == "")
                                //    {
                                //        errs = "Invoice type should not be Empty";
                                //    }
                                //    else
                                //    {
                                //        errs = errs + "," + "Invoice type should not be Empty";
                                //    }
                                //}
                                //To Check Invoice Date Empty or not
                                //if (dataTable.Rows[i][4].ToString() == "")
                                //{
                                //    if (errs == "")
                                //    {
                                //        errs = "Invoice Date should not be Empty";
                                //    }
                                //    else
                                //    {
                                //        errs = errs + "," + "Invoice Date should not be Empty";
                                //    }
                                //}
                                ////Check Invoice Date Valid 
                                //if (dataTable.Rows[i][4].ToString() != "")
                                //{
                                try
                                {
                                    DateTime invdate; //ramy 16 Feb 21
                                    DateTime curdate = System.DateTime.Today;
                                    invdate = objCmnFunctions.convertoDateTime(dataTable.Rows[i][4].ToString());
                                    if (invdate > System.DateTime.Today)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "Invoice Date Should Not be greater than Current Date!";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Invoice Date Should Not be greater than Current Date!";
                                        }
                                    }
                                }
                                catch (Exception)
                                {

                                    if (errs == "")
                                    {
                                        errs = "Invoice Date is Invalid";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Invoice Date is Invalid";
                                    }
                                }
                                // }
                                // to check Rate Empty or not
                                //if (dataTable.Rows[i][8].ToString() == " ")
                                //{

                                //    if (errs == "")
                                //    {
                                //        errs = "Rate should not be Empty";
                                //    }
                                //    else
                                //    {
                                //        errs = errs + "," + "Rate should not be Empty";
                                //    }
                                //}
                                //else
                                //{
                                try
                                {
                                    decimal Rate = Convert.ToDecimal(dataTable.Rows[i][8].ToString());
                                    if (Rate > 0 && Rate <= 100)
                                    {
                                        decimal Ratevalue = Convert.ToDecimal(dataTable.Rows[i][8].ToString());
                                    }
                                    else
                                    {
                                        if (errs == "")
                                        {
                                            errs = "Rate should be greater than Zero or less than or equal to 100";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Rate should be greater than Zero or less than or equal to 100";
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    if (errs == "")
                                    {
                                        errs = "Invalid Rate value! ";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Invalid Rate value!";
                                    }
                                }
                                //}

                                // Invoice Amt greater than the tax Rate

                                /* if (dataTable.Rows[i][8].ToString() != "")
                                 {
                                     try
                                     {
                                         //decimal Invoicevalue = Convert.ToDecimal(dataTable.Rows[i][5].ToString());
                                         decimal Taxrate = Convert.ToDecimal(dataTable.Rows[i][8].ToString());
                                         if (Taxrate > 100)
                                         {
                                             if (errs == "")
                                             {
                                                 errs = "Tax Rate should not be greater than 100%";
                                             }
                                             else
                                             {
                                                 errs = errs + "," + "Tax Rate should not be greater than 100%!";
                                             }
                                         }

                                     }
                                     catch (Exception ex)
                                     {
                                         if (errs == "")
                                         {
                                             errs = "Invalid Tax Rate value! ";
                                         }
                                         else
                                         {
                                             errs = errs + "," + "Invalid Tax Rate value!";
                                         }
                                     }
                                 }
                                 if (dataTable.Rows[i][9].ToString() != " " && dataTable.Rows[i][5].ToString() != " ")
                                 {
                                     try
                                     {

                                         decimal Invoicevalue = Convert.ToDecimal(dataTable.Rows[i][5].ToString());
                                         decimal Taxableamut = Convert.ToDecimal(dataTable.Rows[i][9].ToString());
                                         if (Taxableamut > Invoicevalue)
                                         {
                                             if (errs == "")
                                             {
                                                 errs = "Taxable amount should not be greater than Inovice Amount";
                                             }
                                             else
                                             {
                                                 errs = errs + "," + "Taxable amount should not be greater than Inovice Amount!";
                                             }
                                         }

                                     }
                                     catch (Exception ex)
                                     {
                                         if (errs == "")
                                         {
                                             errs = "Invalid Taxable amount! ";
                                         }
                                         else
                                         {
                                             errs = errs + "," + "Invalid Taxable amount!";
                                         }
                                     }
                                 }*/
                                /*  //To check HSN Code Empty or not
                                  if (dataTable.Rows[i][19].ToString() == " ")
                                  {
                                      if (errs == "")
                                      {
                                          errs = "HSN Code Should Not be Empty ";
                                      }
                                      else
                                      {
                                          errs = errs + "," + "HSN Code Should Not be Empty ";
                                      }
                                  }
                                  else
                                  {
                                      //to check  HSN code Valid or not
                                      string gstin = dataTable.Rows[i][0].ToString();

                                      exceltext = dataTable.Rows[i][19].ToString();

                                      status = objModel.GetGSTRUploadStatusexcel(gstin, " ", exceltext, "HSNCode");
                                      if (status == "notexists")
                                      {
                                          if (errs == "")
                                          {
                                              errs = "HSN Code Not in Master Data  ";
                                          }
                                          else
                                          {
                                              errs = errs + "," + "HSN Code Not in Master Data  ";
                                          }
                                      }
                                  }

                                  // To Check Gstr Filling Date Valid

                                      if (dataTable.Rows[i][15].ToString() == " ")
                                      {

                                          try
                                          {
                                              objCmnFunctions.convertoDateTime(dataTable.Rows[i][15].ToString()).ToString();
                                          }
                                          catch (Exception ex)
                                          {

                                              if (errs == "")
                                              {
                                                  errs = "GSTR Filling Date is Invalid";
                                              }
                                              else
                                              {
                                                  errs = errs + "," + "GSTR Filling Date is Invalid";
                                              }
                                          }

                                      }*/
                                ////
                                //if (dataTable.Rows[i][15].ToString() != "")
                                //{

                                //    try
                                //    {
                                //        objCmnFunctions.convertoDateTime(dataTable.Rows[i][15].ToString()).ToString();
                                //    }
                                //    catch (Exception ex)
                                //    {

                                //        if (errs == "")
                                //        {
                                //            errs = "GSTR Filling Date is Invalid";
                                //        }
                                //        else
                                //        {
                                //            errs = errs + "," + "GSTR Filling Date is Invalid";
                                //        }
                                //    }

                                //}
                                //// Check Gstr Filling Period Valid
                                /*if (dataTable.Rows[i][14].ToString() == " ")
                                {

                                    try
                                    {
                                        string month = dt.Rows[i][14].ToString().Replace("'", " ");
                                        // string fillingmonth = dt.Rows[i][14].ToString().Replace("'", " ");
                                        string filling = month.Replace("'", " ");
                                        objCmnFunctions.getconverttomonthtodatewith3chars(filling);
                                    }
                                    catch (Exception ex)
                                    {

                                        if (errs == "")
                                        {
                                            errs = "GSTR Filling period is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "GSTR Filling Period is Invalid";
                                        }
                                    }

                                }

                                if (dataTable.Rows[i][14].ToString() != " ")
                                {

                                    try
                                    {
                                        //string month = dataTable.Rows[i][14].ToString();
                                        string fillingmonth = dataTable.Rows[i][14].ToString().Replace("'", " ");
                                        /// string filling = month.Replace("'", "");
                                        objCmnFunctions.getconverttomonthtodatewith3chars(fillingmonth);
                                    }
                                    catch (Exception ex)
                                    {

                                        if (errs == "")
                                        {
                                            errs = "GSTR Filling period is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "GSTR Filling Period is Invalid";
                                        }
                                    }

                                }*/

                                //Cess Amount
                                /*if (dataTable.Rows[i][13].ToString() == "")
                                {
                                    if (errs == "")
                                    {
                                        errs = "Cess Amount is Invalid";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Cess Amount is Invalid";
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        decimal cess = Convert.ToDecimal(dataTable.Rows[i][13].ToString());
                                        //if (cess > 0) //ramya commentted 10 Feb 21
                                        //{
                                        //    decimal cessvalue = Convert.ToDecimal(dataTable.Rows[i][13].ToString());
                                        //}
                                        //else
                                        //{
                                        //    if (errs == "")
                                        //    {
                                        //        errs = "Cess Amount is Not greater than Zero";
                                        //    }
                                        //    else
                                        //    {
                                        //        errs = errs + "," + "Cess Amount is Not greater than Zero";
                                        //    }
                                        //}

                                    }
                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "Cess Amount is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Cess Amount is Invalid";
                                        }
                                    }
                                }*/

                                //Applicable
                                //decimal Applicable = Convert.ToDecimal(dataTable.Rows[i][18].ToString().ToString());
                                /* if (dataTable.Rows[i][18].ToString() == " ")
                                 {
                                     if (errs == "")
                                     {
                                         errs = "Applicable Amount is Invalid";
                                     }
                                     else
                                     {
                                         errs = errs + "," + "Applicable Amount is Invalid";
                                     }
                                 }
                                 else
                                 {
                                     try
                                     {
                                         Decimal applicable = Convert.ToDecimal(dataTable.Rows[i][18].ToString());
                                         //if (applicable > 0)
                                         //{
                                         //    Decimal applicablevalue = Convert.ToDecimal(dataTable.Rows[i][18].ToString());
                                         //}
                                         //else
                                         //{
                                         //    if (errs == "")
                                         //    {
                                         //        errs = "Applicable Amount is not Greater Than Zero";
                                         //    }
                                         //    else
                                         //    {
                                         //        errs = errs + "," + "Applicable Amount is not Greater Than Zero";
                                         //    }
                                         //}

                                     }
                                     catch (Exception ex)
                                     {
                                         if (errs == "")
                                         {
                                             errs = "Applicable Amount is Invalid";
                                         }
                                         else
                                         {
                                             errs = errs + "," + "Applicable Amount is Invalid";
                                         }
                                     }
                                 }*/


                                //ITC Availability
                                if (dataTable.Rows[i][16].ToString() != "Yes" && dataTable.Rows[i][16].ToString() != "No")
                                {
                                    if (errs == "")
                                    {
                                        errs = "ITC Availability is Invalid";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "ITC Availability is Invalid";
                                    }
                                }

                                //Reverse Charge
                                if (dataTable.Rows[i][7].ToString() != "Yes" && dataTable.Rows[i][7].ToString() != "No")
                                {
                                    if (errs == "")
                                    {
                                        errs = "Supply Attract Reverse Charge is Invalid";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Supply Attract Reverse Charge is Invalid";
                                    }
                                }
                            }
                            //}

                            #endregion

                            //Credit note
                            else if (dataTable.Rows[i][3].ToString() == "Credit Note")
                            {
                                errs = "";
                                RowNo = j + i;
                                //To check GSTIN of Supplier Empty or Not
                                if (dataTable.Rows[i][0].ToString() == " ")
                                {
                                    if (errs == "")
                                    {
                                        errs = "GSTIN of Supplier Should Not be Empty ";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "GSTIN of Supplier Should Not be Empty ";
                                    }
                                }
                                else
                                {
                                    // To check GSTIN of Supplier Valid Or Not
                                    exceltext = dataTable.Rows[i][0].ToString();
                                    status = objModel.GetGSTRUploadStatusexcel(exceltext, "", "", "GSTNCode");
                                    if (status == "notexists")
                                    {
                                        if (errs == "")
                                        {
                                            //Credit note
                                            errs = "GSTIN of Supplier Not in Master Data - " + exceltext;
                                        }
                                        else
                                        {
                                            errs = errs + "," + "GSTIN of Supplier Not in Master Data - " + exceltext;
                                        }
                                    }
                                }
                                //To check Invoice number empty or not
                                if (dataTable.Rows[i][2].ToString() == "" || dataTable.Rows[i][2].ToString() == "0")
                                {
                                    if (errs == "")
                                    {
                                        //Credit note
                                        errs = "Note number Should Not be Empty/Zero";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Note number Should Not be Empty/Zero";
                                    }
                                }
                                //To Check Invoice Type Emty or not //ramya commentted on 16 Feb 21
                                //if (dataTable.Rows[i][3].ToString() == "")
                                //{

                                //    if (errs == "")
                                //    {
                                //        //Credit note
                                //        errs = "Note type Should Not be Empty";
                                //    }
                                //    else
                                //    {
                                //        errs = errs + "," + "Note type Should Not be Empty";
                                //    }
                                //}
                                //else if (dataTable.Rows[i][3].ToString() != "Credit Note")
                                //{

                                //    if (errs == "")
                                //    {
                                //        //Credit note
                                //        errs = "Note type Should Not be Empty";
                                //    }
                                //    else
                                //    {
                                //        errs = errs + "," + "Note type Should Not be Empty";
                                //    }
                                //}
                                //To Check Invoice Date Empty or not
                                if (dataTable.Rows[i][4].ToString() == "") //ramya 16 Feb 21 index given 5
                                {

                                    if (errs == "")
                                    {
                                        //Credit note
                                        errs = "Note Date Should Not be Empty";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Note Date Should Not be Empty";
                                    }
                                }
                                //Check Invoice Date Valid 
                                if (dataTable.Rows[i][4].ToString() != "")
                                {
                                    try
                                    {
                                        //Credit note
                                        objCmnFunctions.convertoDateTime(dataTable.Rows[i][4].ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            //Credit note
                                            errs = "Note Date is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Note Date is Invalid";
                                        }
                                    }
                                }
                                //To check Invoice Amount Empty or not
                                if (dataTable.Rows[i][5].ToString() == "")
                                {

                                    if (errs == "")
                                    {
                                        //Credit note
                                        errs = "Note Value Should Not be Empty";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Note Value Should Not be Empty";
                                    }
                                }

                                else
                                {
                                    try
                                    {
                                        decimal note = Convert.ToDecimal(dataTable.Rows[i][5].ToString());
                                        if (note <= 0)
                                        {
                                            //decimal noteamt = Convert.ToDecimal(dataTable.Rows[i][5].ToString());
                                            if (errs == "")
                                            {
                                                //Credit note
                                                errs = "Note Value should be Greater than Zero";
                                            }
                                            else
                                            {
                                                errs = errs + "," + "Note Value should be Greater than Zero";
                                            }
                                        }
                                        //else
                                        //{
                                        //    if (errs == "")
                                        //    {
                                        //        //Credit note
                                        //        errs = "Note Value should be Greater than Zero";
                                        //    }
                                        //    else
                                        //    {
                                        //        errs = errs + "," + "Note Value should be Greater than Zero";
                                        //    }
                                        //}
                                    }
                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            //Credit note
                                            errs = "Note Value is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Note Value is Invalid";
                                        }
                                    }
                                }

                                // to check Rate Empty or not
                                if (dataTable.Rows[i][8].ToString() == "")
                                {
                                    if (errs == "")
                                    {
                                        //Credit note
                                        errs = "Rate Should Not be Empty!";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Rate Should Not be Empty!";
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        decimal rate = Convert.ToDecimal(dataTable.Rows[i][8].ToString());
                                        if (rate <= 0)
                                        {
                                            if (errs == "")
                                            {
                                                //Credit note
                                                errs = "Rate should be Greater than Zero";
                                            }
                                            else
                                            {
                                                errs = errs + "," + "Rate should be Greater than Zero";
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "Rate is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Rate is Invalid";
                                        }
                                    }
                                }
                                //To check Taxable value Empty or not
                                if (dataTable.Rows[i][9].ToString() == " ")
                                {

                                    if (errs == "")
                                    {
                                        errs = "Taxable Value Should Not be Empty";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Taxable Value  Should Not be Empty";
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        decimal taxable = Convert.ToDecimal(dataTable.Rows[i][9].ToString());
                                        if (taxable <= 0)
                                        {
                                            if (errs == "")
                                            {
                                                errs = "Taxable Value should be Greater Than Zero";
                                            }
                                            else
                                            {
                                                errs = errs + "," + "Taxable Value should be Greater Than Zero";
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "Taxable Value  is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Taxable Value  is Invalid";
                                        }
                                    }
                                }
                                //Invoice Amt greater than taxable amt validation
                                if ((dataTable.Rows[i][5].ToString() != "") && (dataTable.Rows[i][9].ToString() != ""))
                                {
                                    try
                                    {
                                        Decimal Invoicevalue = Convert.ToDecimal(dataTable.Rows[i][5].ToString());
                                        Decimal Taxrate = Convert.ToDecimal(dataTable.Rows[i][9].ToString());
                                        if (Taxrate > Invoicevalue)
                                        {
                                            if (errs == "")
                                            {
                                                //Credit note 
                                                errs = "Taxable amount should not be greater than Invoice Amount!";
                                            }
                                            else
                                            {
                                                errs = errs + "," + "Taxable amount should not be greater than Invoice Amount!";
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "Taxable amount is Invalid ";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Taxable amount is Invalid";
                                        }
                                    }
                                }
                                //To check HSN Code Empty or not
                                if (dataTable.Rows[i][19].ToString() == "")
                                {
                                    if (errs == "")
                                    {
                                        errs = "HSN Code Should Not be Empty ";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "HSN Code Should Not be Empty ";
                                    }
                                }
                                else
                                {
                                    //to check  HSN code Valid or not
                                    exceltext = dataTable.Rows[i][19].ToString();
                                    string gstin = dataTable.Rows[i][0].ToString();
                                    status = objModel.GetGSTRUploadStatusexcel(gstin, "", exceltext, "HSNCode");
                                    if (status == "notexists")
                                    {
                                        if (errs == "")
                                        {
                                            errs = "HSN Code Not in Master Data ";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "HSN Code Not in Master Data ";
                                        }
                                    }
                                }
                                //Gstr Filling Period
                                if (dataTable.Rows[i][15].ToString() == "")
                                {
                                    try
                                    {
                                        string fillingmonth = dataTable.Rows[i][15].ToString().Replace("'", "");

                                        objCmnFunctions.getconverttomonthtodatewith3chars(fillingmonth);
                                    }


                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "GSTR Filling Period Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "GSTR Filling Period Invalid";
                                        }
                                    }
                                }
                                //if (dataTable.Rows[i][15].ToString() != " ")
                                //{
                                //    try
                                //    {
                                //        string fillingmonth = dataTable.Rows[i][15].ToString().Replace("'", " ");

                                //        objCmnFunctions.getconverttomonthtodatewith3chars(fillingmonth);
                                //    }


                                //    catch (Exception ex)
                                //    {
                                //        if (errs == "")
                                //        {
                                //            errs = "Gstr Filling Period  Invalid";
                                //        }
                                //        else
                                //        {
                                //            errs = errs + "," + "Gstr Filling Period  Invalid";
                                //        }
                                //    }
                                //}
                                // Gstr Filling Date
                                if (dataTable.Rows[i][15].ToString() == " ")
                                {
                                    try
                                    {


                                        objCmnFunctions.convertoDateTime(dataTable.Rows[i][15].ToString());
                                    }


                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            //Credit Note
                                            errs = "GSTR Filling Date Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "GSTR Filling Date  Invalid";
                                        }
                                    }
                                }




                                if (dataTable.Rows[i][15].ToString() != "")
                                {
                                    try
                                    {
                                        objCmnFunctions.convertoDateTime(dataTable.Rows[i][15].ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "GSTR Filling Date Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "GSTR Filling Date Invalid";
                                        }
                                    }
                                }
                                //Cess Amount
                                if (dataTable.Rows[i][13].ToString() == "")
                                {
                                    if (errs == "")
                                    {
                                        //Credit Note
                                        errs = "Cess Amount is Invalid";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Cess Amount is Invalid";
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        decimal cess = Convert.ToDecimal(dataTable.Rows[i][13].ToString());
                                        //if (cess > 0)
                                        //{
                                        //    decimal cessamt = Convert.ToDecimal(dataTable.Rows[i][14].ToString());

                                        //}
                                        //else
                                        //{
                                        //    if (errs == "")
                                        //    {
                                        //        errs = "Cess Amountshould be Greater than Zero";
                                        //    }
                                        //    else
                                        //    {
                                        //        errs = errs + "," + "Cess Amount should be Greater than Zero";
                                        //    }
                                        //}

                                    }
                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "Cess Amount is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Cess Amount is Invalid";
                                        }
                                    }
                                }

                                //Applicable

                                if (dataTable.Rows[i][18].ToString() == " ")
                                {
                                    if (errs == "")
                                    {
                                        errs = "Applicable Amount is Invalid";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Applicable Amount is Invalid";
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        decimal applicable = Convert.ToDecimal(dataTable.Rows[i][18].ToString());
                                        //if (applicable > 0)
                                        //{
                                        //    decimal applicablevalue = Convert.ToDecimal(dataTable.Rows[i][19].ToString());

                                        //}
                                        //else
                                        //{
                                        //    if (errs == "")
                                        //    {
                                        //        errs = "Applicable Amount should be Greater than Zero";
                                        //    }
                                        //    else
                                        //    {
                                        //        errs = errs + "," + "Applicable Amount should be Greater than Zero";
                                        //    }
                                        //}

                                    }
                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "Applicable Amount is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "Applicable Amount is Invalid";
                                        }
                                    }
                                }


                                //ITC Availability
                                if (dataTable.Rows[i][16].ToString() != "Yes" && dataTable.Rows[i][16].ToString() != "No")
                                {
                                    if (errs == "")
                                    {
                                        errs = "ITC Availability is Invalid";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "ITC Availability is Invalid";
                                    }
                                }

                                //Reverse Charge
                                if (dataTable.Rows[i][7].ToString() != "Yes" && dataTable.Rows[i][7].ToString() != "No")
                                {
                                    if (errs == "")
                                    {
                                        errs = "Supply Attract Reverse Charge is Invalid";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "Supply Attract Reverse Charge is Invalid";
                                    }
                                }
                                //IGST
                                if (dataTable.Rows[i][10].ToString() == "")
                                {
                                    if (errs == "")
                                    {
                                        errs = "IGST Amount is Invalid";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "IGST Amount is Invalid";
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        decimal cgst = Convert.ToDecimal(dataTable.Rows[i][10].ToString());
                                        //if (cgst >= 0)
                                        //{
                                        //    decimal cgstval = Convert.ToDecimal(dataTable.Rows[i][10].ToString());
                                        //}
                                        //else
                                        //{
                                        //    if (errs == "")
                                        //    {
                                        //        errs = "IGST Amount should be Greater than Zero!";
                                        //    }
                                        //    else
                                        //    {
                                        //        errs = errs + "," + "IGST Amount should be Greater than Zero!";
                                        //    }
                                        //}

                                    }
                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "IGST Amount is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "IGST Amount is Invalid";
                                        }
                                    }
                                }
                                //Cgst
                                if (dataTable.Rows[i][11].ToString() == "")
                                {
                                    if (errs == "")
                                    {
                                        errs = "CGST Amount is Invalid";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "CGST Amount is Invalid";
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        decimal cgst = Convert.ToDecimal(dataTable.Rows[i][11].ToString());
                                        //if (cgst >= 0)
                                        //{
                                        //    decimal cgstval = Convert.ToDecimal(dataTable.Rows[i][11].ToString());
                                        //}
                                        //else
                                        //{
                                        //    if (errs == "")
                                        //    {
                                        //        errs = "CGST Amount should be Greater than Zero";
                                        //    }
                                        //    else
                                        //    {
                                        //        errs = errs + "," + "CGST Amount should be Greater than Zero";
                                        //    }
                                        //}
                                    }
                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "CGST Amount is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "CGST Amount is Invalid";
                                        }
                                    }
                                }

                                //SGST

                                if (dataTable.Rows[i][12].ToString() == "")
                                {
                                    if (errs == "")
                                    {
                                        errs = "SGST Amount is Invalid";
                                    }
                                    else
                                    {
                                        errs = errs + "," + "SGST Amount is Invalid";
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        decimal cgst = Convert.ToDecimal(dataTable.Rows[i][12].ToString());
                                        //if (cgst >= 0) //ramya 16 Feb 21
                                        //{
                                        //    decimal cgstval = Convert.ToDecimal(dataTable.Rows[i][12].ToString());

                                        //}
                                        //else
                                        //{
                                        //    if (errs == "")
                                        //    {
                                        //        errs = "SGST Amount should be Greater than Zero";
                                        //    }
                                        //    else
                                        //    {
                                        //        errs = errs + "," + "SGST Amount should be Greater than Zero";
                                        //    }
                                        //}

                                    }
                                    catch (Exception ex)
                                    {
                                        if (errs == "")
                                        {
                                            errs = "SGST Amount is Invalid";
                                        }
                                        else
                                        {
                                            errs = errs + "," + "SGST Amount is Invalid";
                                        }
                                    }
                                }

                            }
                            //Credit Note End

                            else
                            {

                                if (errs == "")
                                {
                                    errs = "Invalid Invoice type ";
                                }
                                else
                                {
                                    errs = errs + "," + "Invalid Invoice type";
                                }

                            }


                            if (errs != "")
                            {
                                sno++;
                                Errdatatable.Rows.Add(sno, RowNo + 1, errs); // ramya added +1 on 16 may 22
                            }


                        }

                    }

                    //selva Added Duplicate Invoice Found in checking database 20-01-2020


                    if (Errdatatable.Rows.Count <= 0)
                    {
                        FS_GSTRModel gstrobjmodel = new FS_GSTRModel();


                        Errdatatable = objModel.ExceValidationGSTRUpload(dataTable, gstrobjmodel);
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

        //Download Excel
        public ActionResult downloadsexcel()
        {
            DataTable dtnew = (DataTable)Session["Errdatatablesu"];
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "GSTRUploadStatus.xls"));
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
                success = objModel.ExcelGSTRUpload(dt, gstrobjmodel);
                var str = success.Split(';');
                var Header = str[1];
                // Header = Session["Header"].ToString();
                Session["headerid"] = Header;
                // Session["Header"].ToString() = str[1];

                //  Session["GetHere[]"] = a;


                //var Header= str[1];
                //  Session["Header"].ToString();


                return Json(success, JsonRequestBehavior.AllowGet);
                // return Json(dt, JsonRequestBehavior.AllowGet);
            }


            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                // return Json("Error occurred..." + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }

            return Json(success, JsonRequestBehavior.AllowGet);

        }



        //Get Summary GSTR Upload
        [HttpGet]
        public PartialViewResult _GetGSTRUploadDetails(string straction)
        {
            try
            {

                List<FS_GSTRModel> lstGetEmployee = new List<FS_GSTRModel>();
                TempData["action"] = straction.ToString();
                // TempData["headerid"] = header_gid.ToString();
                return PartialView(lstGetEmployee);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return PartialView();
            }
        }

        //Export GSTR Upload Data
        // [HttpPost]
        public ActionResult ExportGSTRUpload()
        {

            try
            {
                DataTable dt = objModel.ExportGSTRlist();
                if (dt != null)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var workSheet = wb.Worksheets.Add(dt, "Report");
                        wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        wb.Style.Font.Bold = false;
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename= GSTRUpload.xlsx");

                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());

            }
            return View();

        }


    }
}
