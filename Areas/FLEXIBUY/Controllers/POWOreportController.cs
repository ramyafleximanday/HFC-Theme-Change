using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IEM.Areas.FLEXIBUY.Models;
using ClosedXML.Excel;
using System.IO;
using IEM.Helper;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using IEM.Common;
using System.Configuration;
using Newtonsoft.Json;

namespace IEM.Areas.FLEXIBUY.Controllers
{
    public class POWOreportController : Controller
    {
        //
        // GET: /FLEXIBUY/POWOreport/
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        ErrorLog objErrorLog = new ErrorLog();

        proLib pl = new proLib();
        dbLib db = new dbLib();
        DataTable dt;

        private void Getcon()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["connectionstring"].ToString();
                con.Open();
            }
        }
        //  private IPOWOreport Res;
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult FetchReportPOWOData(string datefrom, string dateto, string rbpoworpt)
        {

            DataTable dt = new DataTable();
            Int32 data = 0;
            try
            {
                string fdate = "";
                string todate = "";
                string Data1 = "";
                if (datefrom != "" && dateto != "")
                {
                    DateTime dt1 = new DateTime();
                    dt1 = Convert.ToDateTime(datefrom);
                    fdate = dt1.ToString("yyyy-MM-dd");
                    dt1 = Convert.ToDateTime(dateto);
                    todate = dt1.ToString("yyyy-MM-dd");

                    Getcon();
                    cmd = new SqlCommand("Pr_fb_trn_POWOQuery", con);
                    cmd.Parameters.AddWithValue("@FromDate", fdate.Trim());
                    cmd.Parameters.AddWithValue("@ToDate", todate.Trim());
                    cmd.Parameters.AddWithValue("@action", rbpoworpt);
                    cmd.CommandType = CommandType.StoredProcedure;
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    data = dt.Rows.Count;
                    if (dt.Rows.Count > 0)
                    {

                        Data1 = JsonConvert.SerializeObject(dt);
                        Session["powoquery"] = dt;
                    }
                }
                return Json(new { Data1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog("GetPOWOQueryReport() Method" + ex.Message.ToString(), ex.ToString());
                throw ex;
            }
        }

     

        //public JsonResult GetPOWOQueryReport(string datefrom, string dateto, string rbpoworpt)
        //{
        //    string resultstatus = "";
        //    DataTable dt = new DataTable();
        //    Int32 data = 0;
        //    try
        //    {
        //        string fdate = "";
        //        string todate = "";
        //        if (datefrom != "" && dateto != "")
        //        {
        //            DateTime dt1 = new DateTime();
        //            dt1 = Convert.ToDateTime(datefrom);
        //            fdate = dt1.ToString("yyyy-MM-dd");
        //            dt1 = Convert.ToDateTime(dateto);
        //            todate = dt1.ToString("yyyy-MM-dd");

        //            Getcon();
        //            cmd = new SqlCommand("Pr_fb_trn_POWOQuery", con);
        //            cmd.Parameters.AddWithValue("@FromDate", fdate.Trim());
        //            cmd.Parameters.AddWithValue("@ToDate", todate.Trim());
        //            cmd.Parameters.AddWithValue("@action", rbpoworpt);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            da = new SqlDataAdapter(cmd);
        //            da.Fill(dt);
        //            data = dt.Rows.Count;
        //            if (dt.Rows.Count > 0)
        //            {
        //                resultstatus = "success";
        //                Session["powoquery"] = dt;
        //            }
        //            else
        //            {
        //                data = 0;
        //                resultstatus = "No Record";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objErrorLog.WriteErrorLog("GetPOWOQueryReport() Method" + ex.Message.ToString(), ex.ToString());
        //        throw ex;
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }

        //    return Json(data, resultstatus, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetPOWOQueryReportexport(string rbpoworpt)
        {
            DataTable dt1 = new DataTable();
            dt1 = (DataTable)Session["powoquery"];
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt1, "Flexi " + rbpoworpt.ToUpper() + " Query Report");
                // wb.Worksheet(1).Cell(i, 1).InsertTable(dt2);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=POWO-" + rbpoworpt.ToUpper() + " Report.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                return View();
                // return Json("", JsonRequestBehavior.AllowGet);
            }
        }


    }
}
