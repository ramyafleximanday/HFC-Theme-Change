using ClosedXML.Excel;
using IEM.Areas.FLEXISPEND.Models;
using IEM.Areas.MASTERS.Models;
using IEM.Common;
using IEM.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEM.Areas.FLEXISPEND.Controllers
{
    public class GSTHoldQController : Controller
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
        // GET: /FLEXISPEND/GSTRUpload/



        public GSTHoldQController()
            : this(new FlexispendDataModel())
        {

        }

        public GSTHoldQController(fsIreposiroty objM)
        {
            objModel = objM;
        }

        // GET: /FLEXISPEND/GSTHoldQ/

        //public ActionResult Index()
        //{
        //    // dropdown binding
        //    List<GSTHoldQ> LstObjModelholdq = new List<GSTHoldQ>();
        //    GSTHoldQ typemodel = new GSTHoldQ();
        //    typemodel.SunkCost = new SelectList(objModel.Get_HoldQDropdown().ToList(), "SunkCostGid", "SunkCostName");
        //    return View(typemodel);
        //}
        public ActionResult Sunkcost()
        {
            return View();
        }

        public ActionResult GSTForfieture()
        {
            return View();
        }


        public ActionResult Index()
        {
            // dropdown binding
            List<GSTHoldQ> LstObjModelholdq = new List<GSTHoldQ>();
            GSTHoldQ typemodel = new GSTHoldQ();
            typemodel.SunkCost = new SelectList(objModel.Get_HoldQDropdown().ToList(), "SunkCostGid", "SunkCostName");
            return View(typemodel);
        }

        //Get Hold Q Summary
        [HttpGet]
        public JsonResult ToGetHoldQ()
        {

            List<GSTHoldQ> lstctmaker = new List<GSTHoldQ>();
            lstctmaker = objModel.GetHoldQ().ToList();
            // return Json(lstctmaker, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(new { lstctmaker }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;



        }

        //Export Hold Q Data
        // [HttpPost]
        public ActionResult ExportHoldQ()
        {

            try
            {
                DataTable dt = Session["GSTHoldQ"] as DataTable;
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
                        Response.AddHeader("content-disposition", "attachment;filename= GstHoldQ.xlsx");

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


        //Export Sunkcost Q Data
        // [HttpPost]
        public ActionResult ExportSunkcostQ()
        {

            try
            {
                DataTable dt = Session["SunkcostQ"] as DataTable;
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
                        Response.AddHeader("content-disposition", "attachment;filename= GstSunkcostQ.xlsx");

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

        //Export ForeFieture Q Data
        // [HttpPost]
        public ActionResult ExportForfietureQ()
        {

            try
            {
                DataTable dt = Session["ForeFietureQ"] as DataTable;
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
                        Response.AddHeader("content-disposition", "attachment;filename= GstForefietureQ.xlsx");
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


        //Save Hold Q
        [HttpPost]
        public JsonResult Save_HoldQList(GSTHoldQ model)
        {
            string Result = string.Empty;
            try
            {
                Result = objModel.Save_HoldQList(model);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
            return Json(Result, JsonRequestBehavior.AllowGet);
        }


        //Save SunckCost Approval
        [HttpPost]
        public JsonResult Save_Sunckcost(GSTHoldQ model)
        {
            string Result = string.Empty;
            try
            {
                Result = objModel.Save_Sunckcost(model);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
            return Json(Result, JsonRequestBehavior.AllowGet);
        }


        //Save forfeiture
        [HttpPost]
        public JsonResult Save_forfeiture(GSTHoldQ model)
        {
            string Result = string.Empty;
            try
            {
                Result = objModel.Save_forfeiture(model);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
    }
}
