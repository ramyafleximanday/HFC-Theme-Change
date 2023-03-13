using ClosedXML.Excel;
using IEM.Areas.FLEXISPEND.Models;
using IEM.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEM.Areas.FLEXISPEND.Controllers
{
    public class EmployeeMasterController : Controller
    {
         
        // GET: /FLEXISPEND/EmployeeMaster/
        string sLinkVar = "~/Areas/FLEXISPEND/Views/EmployeeMaster/", sExeVar = ".cshtml";
        private dbLib db = new dbLib();
        private proLib pl = new proLib();
        public ActionResult Index()
        {
            List<EmployeeMaster_Model> records = new List<EmployeeMaster_Model>();
            EmployeeMaster_Datamodel obj = new EmployeeMaster_Datamodel();
            records = obj.GetEmployeeList("top10");
            TempData["SearchItems"] = records;
            return View();
        }
        EmployeeMaster_Datamodel objdataModel = new EmployeeMaster_Datamodel();

        private EmployeeRepository objModel;


        public EmployeeMasterController()
            : this(new EmployeeMaster_Datamodel())
        {

        }
        public EmployeeMasterController(EmployeeRepository objM)
        {
            objModel = objM;
        }

        //public List<EmployeeMaster_Model>  Index()
        //{
        //    List<EmployeeMaster_Model> records = new List<EmployeeMaster_Model>();
        //    EmployeeMaster_Datamodel obj = new EmployeeMaster_Datamodel();
        //    records = obj.GetEmployeeList("top10");
        //    TempData["SearchItems"] = records;
        //    return records;
        //}
        //Employee Master Edit
        [HttpGet]
        public PartialViewResult EmployeeMasterEdit(int id)
        {
            List<EmployeeMaster_Model> records = new List<EmployeeMaster_Model>();
            EmployeeMaster_Model TypeModel = objModel.GetEmployeeDetailbyid(id);
            return PartialView(sLinkVar + "EmployeeMasterEdit" + sExeVar, TypeModel);
         }
        [HttpPost]
        public JsonResult EmployeeMasterFull(string action="full")
        {
            EmployeeMaster_Datamodel dm = new EmployeeMaster_Datamodel();
            List<EmployeeMaster_Model> lst = new List<EmployeeMaster_Model>();
            lst = dm.GetEmployeeList("full");
            //Session["Ecfamountpayment"] = Convert.ToInt32(Session["Ecfamountpayment"].ToString()) + Convert.ToInt32(delamt.ToString());
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        //Employee Grade update

        public string Employee_Grade_Edit(string employeegid, string employeegrade, string iswrklupdreq)
        {
            string Emp_Msg = "";
            Emp_Msg = objdataModel.Employee_Grade_Edit(employeegid, employeegrade, iswrklupdreq);
           return Emp_Msg;
        }

        //downloading Excel File
        [HttpPost]
        public JsonResult DownloadExcel(string ViewType, string FileName)
        {
            DataSet ds = db.GetCommonExcelDownload(ViewType, pl.LoginUserId);
            DataTable _downloadingData = ds.Tables[0];
            if (_downloadingData != null)
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(_downloadingData);
                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;

                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("content-disposition", "attachment;filename= " + FileName + ".xls");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
