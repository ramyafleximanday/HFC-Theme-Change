using IEM.Areas.EOW.Models;
using IEM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using IEM.Areas.Dashboard.Models;
using Rotativa.Options;
using IEM.Areas.MASTERS.Controllers;
using IEM.Helper;
using System.Data;
using Newtonsoft.Json;

namespace IEM.Areas.EOW.Controllers
{
    public class NonTravelClaimNewGSTController : Controller
    {
        //
        // GET: /EOW/NonTravelClaimNewGST/

        ErrorLog objErrorLog = new ErrorLog();
        private EOW_IRepository objModel;


        CmnFunctions objCmnFunctions = new CmnFunctions();
        private FileServer Cmnfs = new FileServer();
        LocalConveyanceNewController locals = new LocalConveyanceNewController();
        CentralModel cen = new CentralModel();
        CommonIUD objCommonIUD = new CommonIUD();
        proLib plib = new proLib();
        dbLib db = new dbLib();
        DataTable dt;
        EOW_EmployeeeExpense obj = new EOW_EmployeeeExpense();
        //EOW_EmployeeeExpense eowemp = new EOW_EmployeeeExpense();
        List<EOW_EmployeeeExpense> objmaindetail = new List<EOW_EmployeeeExpense>();
        string SupName = string.Empty, Panno = string.Empty, InvoiceNo = string.Empty, GSNTNNo = string.Empty, Fromdate = string.Empty, Todate = string.Empty;

        public NonTravelClaimNewGSTController()
            : this(new EOW_DataModel())
        {

        }
        public NonTravelClaimNewGSTController(EOW_IRepository objM)
        {
            objModel = objM;
        }

        public ActionResult Index()
        {
            try
            {
                Session["QueueGide"] = "";
                if (Session["DashBoard"] != null)
                {
                    EOW_TravelClaim objMExpense = new EOW_TravelClaim();
                    List<EOW_TravelClaim> objobjMExpense = new List<EOW_TravelClaim>();
                    List<EOW_EmployeeeExpense> objmaindetail = new List<EOW_EmployeeeExpense>();

                    EOW_EmployeeeExpense eowemp = new EOW_EmployeeeExpense();
                    eowemp = (EOW_EmployeeeExpense)Session["DashBoard"];
                    if (eowemp.ecfremark != null)
                    {
                        ViewBag.ecfrmarks = eowemp.ecfremark;
                    }
                    Session["EcfGid"] = eowemp.ecf_GID.ToString();
                    Session["invoiceGid"] = eowemp.invoice_GID.ToString();
                    if (eowemp.Queue_GID != 0)
                    {
                        Session["QueueGide"] = eowemp.Queue_GID.ToString();
                    }
                    else
                    {
                        Session["QueueGide"] = "";
                    }
                    ViewBag.process = "Postdata";
                    ViewBag.message = "success";

                    Session["Ecfamountvalmain"] = eowemp.ECF_Amount;
                    eowemp.ECF_Amount = objCmnFunctions.GetINRAmount(eowemp.ECF_Amount);
                    ViewBag.EOW_EmployeeeExpenseheader = eowemp;
                    Session["DashBoard"] = null;

                    if (Session["loginraisemode"] != null)
                    {
                        if (eowemp.raisermodeId == "P")
                        {
                            Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginProxyUserGid().ToString();
                            Session["SelfModeecfval"] = objCmnFunctions.GetLoginProxyUserGid().ToString();
                        }
                        else
                        {
                            Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginUserGid().ToString();
                            Session["SelfModeecfval"] = objCmnFunctions.GetLoginUserGid().ToString();
                        }
                        List<EOW_EmployeeeExpense> objmaindetaild = new List<EOW_EmployeeeExpense>();
                        objmaindetaild = objModel.SelectEmployeeeheader(objCmnFunctions.GetLoginProxyUserGid().ToString()).ToList();
                        Session["raiserfcc"] = objmaindetaild[0].Exp_FC.ToString();
                        Session["raiserccc"] = objmaindetaild[0].Exp_CC.ToString();
                    }
                    else
                    {
                        Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginUserGid().ToString();
                        Session["SelfModeecfval"] = objCmnFunctions.GetLoginUserGid().ToString();
                        objmaindetail = objModel.SelectEmployeeeheader(objCmnFunctions.GetLoginUserGid().ToString()).ToList();
                        objobjMExpense = objModel.tSelectEmployeeeBasic(Session["SelfModeRaiseid"].ToString()).ToList();
                        eowemp.Grade = objmaindetail[0].Grade.ToString();
                        eowemp.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
                        eowemp.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
                        eowemp.raisermodeId = "S";//"C OR S OR P" 
                        eowemp.Raiser_Modedata = new SelectList(objModel.GetRaiserMode().ToList(), "raisermodeId", "raisermodeName", eowemp.raisermodeId);
                        Session["raiserfcc"] = objmaindetail[0].Exp_FC.ToString();
                        Session["raiserccc"] = objmaindetail[0].Exp_CC.ToString();
                        objMExpense.OUCode = objobjMExpense[0].OUCode.ToString();
                        objMExpense.ProductCode = objobjMExpense[0].ProductCode.ToString();
                        objMExpense.Exp_FCC = new SelectList(objModel.fcdata().ToList(), "raiserfcId", "raiserfcName", Session["raiserfcc"].ToString());
                        objMExpense.Exp_CCC = new SelectList(objModel.ccdata().ToList(), "raiserccId", "raiserccName", Session["raiserccc"].ToString());
                        objMExpense.Exp_FCCC = objobjMExpense[0].Exp_FCCC.ToString();
                        //Expense Dropdownlist load
                        // objMExpense.ExpNatureofExpdata = new SelectList(objModelTravel.NatureofExpensesdata().ToList(), "NatureofExpensesId", "NatureofExpensesName");
                        objMExpense.ExpNatureofExpdata = new SelectList(objModel.NatureofExpensesdataother().ToList(), "NatureofExpensesId", "NatureofExpensesName");
                        //objMExpense.TravelModedata = new SelectList(objModelTravel.tTravelModedata().ToList(), "TravelModeId", "TravelModeName");
                        objMExpense.Citydata = new SelectList(objModel.tTravelcitydata().ToList(), "TravelCityId", "TravelCityName");
                        ViewBag.EOW_EmployeeeExpenseheader = objMExpense;
                    }
                    eowemp.Raiser_Modedata = new SelectList(objModel.GetRaiserMode().ToList(), "raisermodeId", "raisermodeName", eowemp.raisermodeId);
                    if (Session["EcfGid"] != null)
                    {
                        obj = objModel.GetEcfHeader(Session["EcfGid"].ToString());
                        if (obj.ClaimMonth != null)
                        {
                            eowemp.ECF_Amount = obj.ECF_Amount;
                            eowemp.ecfdescription = obj.ecfdescription; //30-12-2020
                            eowemp.ecfremark = obj.ecfremark;
                            eowemp.noofperson = obj.noofperson;
                            eowemp.ClaimMonth = obj.ClaimMonth;
                            eowemp.ECF_Date = obj.ECF_Date;
                        }
                        else
                        {
                            Session.Remove("EcfGid");
                        }
                    }

                    ViewBag.EOW_Employeeheader = eowemp;
                } 
                else
                {
                    EOW_EmployeeeExpense eowemp = new EOW_EmployeeeExpense();
                    if (Session["loginraisemode"] != null)
                    {
                        Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginProxyUserGid().ToString();
                        Session["SelfModeecfval"] = objCmnFunctions.GetLoginProxyUserGid().ToString();
                        objmaindetail = objModel.SelectEmployeeeheader(objCmnFunctions.GetLoginProxyUserGid().ToString()).ToList();
                        eowemp.Grade = objmaindetail[0].Grade.ToString();
                        eowemp.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
                        eowemp.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
                        eowemp.raisermodeId = "P";
                        objmaindetail = objModel.SelectEmployeeeheader(objCmnFunctions.GetLoginProxyUserGid().ToString()).ToList();
                        Session["raiserfcc"] = objmaindetail[0].Exp_FC.ToString();
                        Session["raiserccc"] = objmaindetail[0].Exp_CC.ToString();
                    }
                    else
                    {
                        EOW_TravelClaim objMExpense = new EOW_TravelClaim();
                        List<EOW_TravelClaim> objobjMExpense = new List<EOW_TravelClaim>();
                        List<EOW_EmployeeeExpense> objmaindetail = new List<EOW_EmployeeeExpense>();

                        Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginUserGid().ToString();
                        Session["SelfModeecfval"] = objCmnFunctions.GetLoginUserGid().ToString();
                        objmaindetail = objModel.SelectEmployeeeheader(objCmnFunctions.GetLoginUserGid().ToString()).ToList();
                        objobjMExpense = objModel.tSelectEmployeeeBasic(Session["SelfModeRaiseid"].ToString()).ToList();
                        eowemp.Grade = objmaindetail[0].Grade.ToString();
                        eowemp.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
                        eowemp.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
                        eowemp.raisermodeId = "S";//"C OR S OR P" 
                        Session["raiserfcc"] = objmaindetail[0].Exp_FC.ToString();
                        Session["raiserccc"] = objmaindetail[0].Exp_CC.ToString();
                        objMExpense.OUCode = objobjMExpense[0].OUCode.ToString();
                        objMExpense.ProductCode = objobjMExpense[0].ProductCode.ToString();
                        objMExpense.Exp_FCC = new SelectList(objModel.fcdata().ToList(), "raiserfcId", "raiserfcName", Session["raiserfcc"].ToString());
                        objMExpense.Exp_CCC = new SelectList(objModel.ccdata().ToList(), "raiserccId", "raiserccName", Session["raiserccc"].ToString());
                        objMExpense.Exp_FCCC = objobjMExpense[0].Exp_FCCC.ToString();
                        //Expense Dropdownlist load
                        // objMExpense.ExpNatureofExpdata = new SelectList(objModelTravel.NatureofExpensesdata().ToList(), "NatureofExpensesId", "NatureofExpensesName");
                        objMExpense.ExpNatureofExpdata = new SelectList(objModel.NatureofExpensesdataother().ToList(), "NatureofExpensesId", "NatureofExpensesName");
                        //objMExpense.TravelModedata = new SelectList(objModelTravel.tTravelModedata().ToList(), "TravelModeId", "TravelModeName");
                        objMExpense.Citydata = new SelectList(objModel.tTravelcitydata().ToList(), "TravelCityId", "TravelCityName");
                        ViewBag.EOW_EmployeeeExpenseheader = objMExpense;
                    }
                    Session["EcfGid"] = null;
                    Session["invoiceGid"] = null;
                    Session["Ecfamountpaymenttt"] = null;
                    Session["Ecfamountvaloe"] = null;
                    Session["Ecfamountvaltm"] = null;
                    ViewBag.PayMethod = "Single Payment";
                    eowemp.Raiser_Modedata = new SelectList(objModel.GetRaiserMode().ToList(), "raisermodeId", "raisermodeName", eowemp.raisermodeId);
                    if (Session["EcfGid"] != null)
                    {
                        obj = objModel.GetEcfHeader(Session["EcfGid"].ToString());
                        if (obj.ClaimMonth != null)
                        {
                            eowemp.ECF_Amount = obj.ECF_Amount;
                            eowemp.ecfdescription = obj.ecfdescription; //30-12-2020
                            eowemp.ecfremark = obj.ecfremark;
                            eowemp.noofperson = obj.noofperson;
                            eowemp.ClaimMonth = obj.ClaimMonth;
                            eowemp.ECF_Date = obj.ECF_Date;
                        }
                        else
                        {
                            Session.Remove("EcfGid");
                        }
                    }

                    ViewBag.EOW_Employeeheader = eowemp;
                }
                
                

                Session["NonBranchClaimFlage"] = objModel.travelbranchflag(objCmnFunctions.GetLoginUserGid());
                Session["Ecfdeclaratonnote"] = objModel.GetDecnote("3", "S").ToString();
                return View();

            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return View();
            }
        }
        //public ActionResult Index()
        //{
        //    try
        //    {
        //        EOW_TravelClaim objMExpense = new EOW_TravelClaim();
        //        List<EOW_TravelClaim> objobjMExpense = new List<EOW_TravelClaim>();
        //        List<EOW_EmployeeeExpense> objmaindetail = new List<EOW_EmployeeeExpense>();


        //        Session["QueueGide"] = "";

        //        if (Session["loginraisemode"] != null)
        //        {
        //            Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginProxyUserGid().ToString();
        //            Session["SelfModeecfval"] = objCmnFunctions.GetLoginProxyUserGid().ToString();
        //            objmaindetail = objModel.SelectEmployeeeheader(objCmnFunctions.GetLoginProxyUserGid().ToString()).ToList();



        //            eowemp.Grade = objmaindetail[0].Grade.ToString();
        //            eowemp.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
        //            eowemp.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
        //            eowemp.raisermodeId = "P";

        //            objmaindetail = objModel.SelectEmployeeeheader(objCmnFunctions.GetLoginProxyUserGid().ToString()).ToList();
        //            Session["raiserfcc"] = objmaindetail[0].Exp_FC.ToString();
        //            Session["raiserccc"] = objmaindetail[0].Exp_CC.ToString();
        //        }
        //        else
        //        {

        //            Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginUserGid().ToString();
        //            Session["SelfModeecfval"] = objCmnFunctions.GetLoginUserGid().ToString();
        //            objmaindetail = objModel.SelectEmployeeeheader(objCmnFunctions.GetLoginUserGid().ToString()).ToList();
        //            eowemp.Grade = objmaindetail[0].Grade.ToString();
        //            eowemp.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
        //            eowemp.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
        //            eowemp.raisermodeId = "S";//"C OR S OR P" 

        //            Session["raiserfcc"] = objmaindetail[0].Exp_FC.ToString();
        //            Session["raiserccc"] = objmaindetail[0].Exp_CC.ToString();
        //            objobjMExpense = objModel.tSelectEmployeeeBasic(Session["SelfModeRaiseid"].ToString()).ToList();
        //            objMExpense.OUCode = objobjMExpense[0].OUCode.ToString();
        //            objMExpense.ProductCode = objobjMExpense[0].ProductCode.ToString();


        //            objMExpense.Exp_FCC = new SelectList(objModel.fcdata().ToList(), "raiserfcId", "raiserfcName", Session["raiserfcc"].ToString());
        //            objMExpense.Exp_CCC = new SelectList(objModel.ccdata().ToList(), "raiserccId", "raiserccName", Session["raiserccc"].ToString());
        //            objMExpense.Exp_FCCC = objobjMExpense[0].Exp_FCCC.ToString();

        //            //Expense Dropdownlist load
        //            // objMExpense.ExpNatureofExpdata = new SelectList(objModelTravel.NatureofExpensesdata().ToList(), "NatureofExpensesId", "NatureofExpensesName");

        //            objMExpense.ExpNatureofExpdata = new SelectList(objModel.NatureofExpensesdataother().ToList(), "NatureofExpensesId", "NatureofExpensesName");


        //            //objMExpense.TravelModedata = new SelectList(objModelTravel.tTravelModedata().ToList(), "TravelModeId", "TravelModeName");
        //            objMExpense.Citydata = new SelectList(objModel.tTravelcitydata().ToList(), "TravelCityId", "TravelCityName");


        //            ViewBag.EOW_EmployeeeExpenseheader = objMExpense;
        //        }
        //        if (Session["EcfGid"] != null)
        //        {
        //            obj = objModel.GetEcfHeader(Session["EcfGid"].ToString());

        //            eowemp.ECF_Amount = obj.ECF_Amount;
        //            eowemp.ecfdescription = obj.ecfdescription; //selva add desc 16-02-2020
        //            eowemp.ecfremark = obj.ecfremark;
        //            eowemp.noofperson = obj.noofperson;
        //            eowemp.ClaimMonth = obj.ClaimMonth;
        //            eowemp.ECF_Date = obj.ECF_Date;
        //        }

        //        ////07.12.2020 Vadivu
        //        //if (Session["EcfGid"] != null)
        //        //{
        //        //    obj = objModel.GetEcfHeader(Session["EcfGid"].ToString());
        //        //    if (obj.ClaimMonth != null)
        //        //    {
        //        //        eowemp.ECF_Amount = obj.ECF_Amount;
        //        //        eowemp.ecfremark = obj.ecfremark;
        //        //        eowemp.noofperson = obj.noofperson;
        //        //        eowemp.ClaimMonth = obj.ClaimMonth;
        //        //        eowemp.ECF_Date = obj.ECF_Date;
        //        //    }
        //        //    else
        //        //    {
        //        //        Session.Remove("EcfGid");
        //        //    }


        //        //}


        //        eowemp.Raiser_Modedata = new SelectList(objModel.GetRaiserMode().ToList(), "raisermodeId", "raisermodeName", eowemp.raisermodeId);
        //        ViewBag.EOW_Employeeheader = eowemp;
        //        // vadivu
        //        Session["EcfGid"] = null;

        //        Session["NonBranchClaimFlage"] = objModel.travelbranchflag(objCmnFunctions.GetLoginUserGid());
        //        Session["Ecfdeclaratonnote"] = objModel.GetDecnote("3", "S").ToString();

        //        return View();
        //        // }
        //    }
        //    catch (Exception ex)
        //    {
        //        objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
        //        return View();
        //    }
        //}




        //Debitline save
        [HttpPost]
        public JsonResult TravelModeCreate(EOW_TravelClaim TravelClaimModel)
        {
            string message = "";
            try
            {

                if (Session["EcfGid"] == null || Convert.ToInt64(Session["EcfGid"].ToString()) == -1)
                {
                    string ecfnewamt = "0";
                    obj.ECF_Amount = "0";
                    string clmmonth = "";
                    //string message = "";
                    string insrtinvoice = "";
                    List<EOW_EmployeeeExpense> objmaindetail = new List<EOW_EmployeeeExpense>();
                    objmaindetail = objModel.SelectEmployeeeheader(objCmnFunctions.GetLoginUserGid().ToString()).ToList();
                    if (obj.ecfremark != null)
                    {
                        ViewBag.ecfrmarks = obj.ecfremark;
                    }

                    EOW_EmployeeeExpense eowemp = new EOW_EmployeeeExpense();
                    if (Session["loginraisemode"] != null)
                    {
                        obj.raisermodeId = "P";
                        objmaindetail = objModel.SelectEmployeeeheader(objCmnFunctions.GetLoginProxyUserGid().ToString()).ToList();
                        obj.Grade = objmaindetail[0].Grade.ToString();
                        obj.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
                        obj.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
                    }
                    else
                    {
                        obj.raisermodeId = "S";//"C OR S OR P" 
                        objmaindetail = objModel.SelectEmployeeeheader(objCmnFunctions.GetLoginUserGid().ToString()).ToList();
                        obj.Grade = objmaindetail[0].Grade.ToString();
                        obj.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
                        obj.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
                    }

                    Session["Ecfamountvalmain"] = obj.ECF_Amount;
                    obj.Raiser_Modedata = new SelectList(objModel.GetRaiserMode().ToList(), "raisermodeId", "raisermodeName", obj.raisermodeId);
                    clmmonth = TravelClaimModel.ClaimPeriodFrom;
                    obj.ClaimMonth = "";    //locals.getconverttomonthtodate(TravelClaimModel.ClaimPeriodFrom);  selva commend 22-12-2020
                    obj.ECF_Date = DateTime.Now.ToString("dd-MM-yyyy");
                    obj.noofperson = "1";
                    message = objModel.InsertEmployeeeBasic(obj, objCmnFunctions.GetLoginUserGid().ToString(), "R");
                    Session["EcfGid"] = objModel.selectecfgidBasic(obj, objCmnFunctions.GetLoginUserGid().ToString());

                    if (Session["EcfGid"] != "" && Session["EcfGid"] != null)
                    {
                        EOW_Employeelst addemp = new EOW_Employeelst();
                        addemp.employeeGid = Convert.ToInt32(Session["SelfModeRaiseid"].ToString());
                        addemp.empbranch = objModel.Getemployeebrach(Session["SelfModeRaiseid"].ToString());
                        addemp.empfc = Session["raiserfcc"].ToString();
                        addemp.empcc = Session["raiserccc"].ToString();
                        message = objModel.insertempperson(addemp, Session["EcfGid"].ToString());
                    }

                }

                if (Convert.ToInt64(Session["EcfGid"].ToString()) > 0)
                    message = objModel.InsertTravelModeCreateGst(TravelClaimModel, Session["EcfGid"].ToString(), "0", Session["SelfModeRaiseid"].ToString());
                ViewBag.SearchItems = null;

                ViewBag.process = "Postdata";
                ViewBag.message = message;
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("Error occurred..." + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpGet]
        public PartialViewResult ECFDetails()
        {
            List<EOW_EmployeeeExpense> lstnew = new List<EOW_EmployeeeExpense>();
            EOW_EmployeeeExpense eowemp = new EOW_EmployeeeExpense();
            if (Session["EcfGid"] != null)
            {
                eowemp = objModel.GetEcfHeader(Session["EcfGid"].ToString());
            }
            ViewBag.EOW_Employeeheader = eowemp;

            return PartialView();
        }

        [HttpPost]
        public ActionResult ECFDetails(EOW_EmployeeeExpense obj)
        {
            try
            {
                string ecfnewamt = obj.ECF_Amount;
                obj.ECF_Amount = objCmnFunctions.GetRemovecommas(obj.ECF_Amount);
                string clmmonth = "";
                string message = "";
                string insrtinvoice = "";
                string IFSCCode = "";
                string IFSCCodeMessage = "";
                List<EOW_EmployeeeExpense> objmaindetail = new List<EOW_EmployeeeExpense>();
                EOW_EmployeeeExpense eowemp = new EOW_EmployeeeExpense();
                if (obj.ecfremark != null)
                {
                    ViewBag.ecfrmarks = obj.ecfremark;
                }
                //if (Session["loginraisemode"] != null)
                //{
                //    obj.raisermodeId = "P";
                //    objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginProxyUserGid().ToString()).ToList();
                //    obj.Grade = objmaindetail[0].Grade.ToString();
                //    obj.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
                //    obj.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
                //}
                //else
                //{
                //    obj.raisermodeId = "S";//"C OR S OR P" 
                //    objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginUserGid().ToString()).ToList();
                //    obj.Grade = objmaindetail[0].Grade.ToString();
                //    obj.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
                //    obj.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
                //}


                Session["Ecfamountvalmain"] = obj.ECF_Amount;
                obj.Raiser_Modedata = new SelectList(objModel.GetRaiserMode().ToList(), "raisermodeId", "raisermodeName", obj.raisermodeId);
                clmmonth = obj.ClaimMonth;
                int nodays = 0;
                string result = objModel.GetStatusexcel("", Session["SelfModeecfval"].ToString(), "", "Ecfraiser");
                nodays = Convert.ToInt32(result);
                if (nodays < 15)
                {
                    ViewBag.process = "Postdataerr";
                    message = "Employee Can't Raise ECF";
                }
                else
                {
                    obj.ClaimMonth = locals.getconverttomonthtodate(obj.ClaimMonth);
                    //if (Session["EcfGid"] != null && Session["invoiceGid"] != null)
                    if (Session["EcfGid"] != null)
                    {
                        message = objModel.InsertEmployeeeBasicupdate(obj, objCmnFunctions.GetLoginUserGid().ToString(), Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString());
                        if (message != "")
                        {
                            ViewBag.process = "Postdata";
                            if (obj.raisermodeId == "P")
                            {
                                if (Session["loginraisemode"] != null)
                                {
                                    Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginProxyUserGid().ToString();
                                }
                                else
                                {
                                    Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginUserGid().ToString();
                                }
                            }
                            else
                            {
                                Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginUserGid().ToString();
                            }

                            //ViewBag.IFSCCodemsg = "";
                            // ViewBag.IFSCodemessage = "";
                            IFSCCode = objModel.InsertEmployeeePaymentbasicupdate(Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString(), Session["Ecfamountvalmain"].ToString());
                            //message = "success";
                            if (IFSCCode.ToString().Trim().Equals(""))
                            {
                                // ViewBag.IFSCodemessage = "IFSC Code is not update!!! Your Claim may get REJECT by EPU Team!";
                                IFSCCodeMessage = "IFSC Code is not updated!!! Your Claim may get REJECT by EPU Team!";

                            }
                            else
                            {
                                //  message = "ECF details Not Updated Properly Please check ECF Header Details";
                            }


                            //insrtinvoice = objModelTravel.InsertEmployeeeBasicinvoiceupdate(obj, Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString());

                            //if (insrtinvoice != "")
                            //{
                            //    Session["invoiceGid"] = insrtinvoice.ToString();

                            //    string IFSCCode = "";
                            //    ViewBag.IFSCCodemsg = "";
                            //    IFSCCode = objModelTravel.InsertEmployeeePaymentbasicupdate(Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString(), Session["Ecfamountvalmain"].ToString());

                            //    if (IFSCCode.ToString().Trim().Equals(""))
                            //    {
                            //        ViewBag.IFSCCodemsg = "IFSC Code is not updated!!! Your Claim may get REJECT by EPU Team!";
                            //    }
                            //    message = "success";
                            //}
                            //else
                            //{
                            //    message = "ECF details Not Updated Properly Please check ECF Header Details";
                            //}
                            message = "success";
                        }
                    }
                    else
                    {
                        message = objModel.InsertEmployeeeBasic(obj, objCmnFunctions.GetLoginUserGid().ToString(), "R");
                        if (message != "")
                        {
                            ViewBag.process = "Postdata";
                            if (obj.raisermodeId == "P")
                            {
                                if (Session["loginraisemode"] != null)
                                {
                                    Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginProxyUserGid().ToString();
                                }
                                else
                                {
                                    Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginUserGid().ToString();
                                }
                            }
                            else
                            {
                                Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginUserGid().ToString();
                            }


                            Session["EcfGid"] = objModel.selectecfgidBasic(obj, objCmnFunctions.GetLoginUserGid().ToString());
                            insrtinvoice = objModel.InsertEmployeeeBasicinvoice(obj, Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString(), "E");
                            if (insrtinvoice != "")
                            {
                                Session["invoiceGid"] = objModel.selectinvoiceidBasic(obj, Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString(), "E");
                                //objModel.InsertEmployeeePaymentbasic(Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString(), Session["invoiceGid"].ToString(), Session["Ecfamountvalmain"].ToString());

                                // string IFSCCode = "";
                                ViewBag.IFSCCodemsg = "";
                                IFSCCode = objModel.InsertEmployeeePaymentbasic(Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString(), Session["invoiceGid"].ToString(), Session["Ecfamountvalmain"].ToString());
                                if (IFSCCode.ToString().Trim().Equals(""))
                                {
                                    ViewBag.IFSCCodemsg = "IFSC Code is not updated!!! Your Claim may get REJECT by EPU Team!";
                                }
                                message = "success";
                            }


                            else
                            {
                                message = "ECF details Not Inserted Properly Please check ECF Header Details";
                            }
                        }
                    }
                }
                ViewBag.message = message;
                obj.ClaimMonth = clmmonth;
                obj.ECF_Amount = ecfnewamt;
                ViewBag.EOW_EmployeeeExpenseheader = obj;
                return View();
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return View();
            }
        }

        [HttpGet]
        public PartialViewResult NonTravelModeDetail()
        {
            //List<EOW_EmployeeeExpense> lst = new List<EOW_EmployeeeExpense>();
            //return PartialView();

            List<EOW_EmployeeeExpense> lst = new List<EOW_EmployeeeExpense>();
            return PartialView();
        }

        //public PartialViewResult CygnetSearch_Supplier()
        //{
        //    List<CygnetSearchModel> obj = new List<CygnetSearchModel>();
        //    CygnetSearchModel invsearch = new CygnetSearchModel();
        //    invsearch.Cygnet_SupplierPanNo = "";
        //    obj = objModel.SelectInvoiceSearch(invsearch).ToList();
        //    return PartialView();
        //}


        [HttpPost]
        public JsonResult GetCygnetSearchInvDetails(CygnetSearchModel CustomerInvDetail)
        {
            string Data1 = "";
            DataTable dt = objModel.GetCygnetSearchInvDetails(CustomerInvDetail);
            if (dt.Rows.Count > 0)
            {
                Data1 = JsonConvert.SerializeObject(dt);
            }

            return Json(new { Data1 }, JsonRequestBehavior.AllowGet);
        }


        //SELVA Create
        public PartialViewResult CygnetSearch_Supplier()
        {
            List<CygnetSearchModel> obj = new List<CygnetSearchModel>();
            CygnetSearchModel invsearch = new CygnetSearchModel();
            invsearch.Cygnet_SupplierPanNo = "";
            obj = objModel.SelectInvoiceSearch(invsearch).ToList();
            return PartialView();
        }

        //public PartialViewResult CygnetGrid(string panno = "", string suppliegid = "", string GSNTNNo = "", string InvoiceNo = "", string FromDate = "", string ToDate = "")
        //{
        //    List<CygnetSearchModel> objowner = new List<CygnetSearchModel>();

        //    if (Session["SearchItems"] != null)
        //    {
        //        objowner = (List<CygnetSearchModel>)TempData["SearchItems"];
        //    }
        //    else
        //    {
        //        CygnetSearchModel invsearch = new CygnetSearchModel();
        //        invsearch.Cygnet_SupplierPanNo = panno;
        //        invsearch.InvoiceSupplier_gid = suppliegid;
        //        invsearch.Cygnet_SupplierName = suppliegid;
        //        invsearch.Cygnet_Supplier_GSTNNo = GSNTNNo;
        //        invsearch.Cygnet_InvoiceNo = InvoiceNo;
        //        invsearch.Cygnet_InvoiceFromDate = FromDate;
        //        invsearch.Cygnet_InvoiceToDate = ToDate;
        //        objowner = objModel.SelectInvoiceSearch(invsearch).ToList();
        //        TempData["SearchItems"] = objowner;
        //    }
        //    return PartialView(objowner.ToList());
        //}

        //selva 22-Mar-2020 Modified 
        public PartialViewResult CygnetGrid()
        {
            List<CygnetSearchModel> objowner = new List<CygnetSearchModel>();

            SupName = Session["SupplierName"].ToString();
            Panno = Session["Panno"].ToString();
            GSNTNNo = Session["GSNTNNo"].ToString();
            InvoiceNo = Session["InvoiceNo"].ToString();
            Fromdate = Session["FromDate"].ToString();
            Todate = Session["ToDate"].ToString();


            if (Session["SearchItems"] != null)
            {
                objowner = (List<CygnetSearchModel>)TempData["SearchItems"];
            }
            else
            {
                CygnetSearchModel invsearch = new CygnetSearchModel();
                invsearch.Cygnet_SupplierPanNo = Panno;
               // invsearch.InvoiceSupplier_gid = suppliegid;
                invsearch.Cygnet_SupplierName = SupName;
                invsearch.Cygnet_Supplier_GSTNNo = GSNTNNo;
                invsearch.Cygnet_InvoiceNo = InvoiceNo;
                invsearch.Cygnet_InvoiceFromDate = Fromdate;
                invsearch.Cygnet_InvoiceToDate = Todate;
                objowner = objModel.SelectInvoiceSearch(invsearch).ToList();
                TempData["SearchItems"] = objowner;

                Session["SupplierName"] = "";
                Session["Panno"] = "";
                Session["GSNTNNo"] = "";
                Session["InvoiceNo"] = "";
                Session["FromDate"] = "";
                Session["ToDate"] = "";
            }
            return PartialView(objowner.ToList());
        }


        //28-12-2020 selva create
        [HttpPost]
        public JsonResult GetCygnetSearchCountInvDetails(string panno = "", string suppliername = "", string GSNTNNo = "", string InvoiceNo = "", string FromDate = "", string ToDate = "")
        {
            string Data1 = "";
            CygnetSearchModel CustomerInvDetail = new CygnetSearchModel();
            CustomerInvDetail.Cygnet_SupplierPanNo = panno;
            CustomerInvDetail.Cygnet_SupplierName = suppliername;
            CustomerInvDetail.Cygnet_Supplier_GSTNNo = GSNTNNo;
            CustomerInvDetail.Cygnet_InvoiceNo = InvoiceNo;

            CustomerInvDetail.Cygnet_InvoiceFromDate = FromDate;

            CustomerInvDetail.Cygnet_InvoiceToDate = ToDate;

            DataTable dt = objModel.GetCygnetSearchInvDetailsCount(CustomerInvDetail);

            Session["Panno"] = panno;
            Session["SupplierName"] = suppliername;
            Session["GSNTNNo"] = GSNTNNo;
            Session["InvoiceNo"] = InvoiceNo;
            Session["FromDate"] = FromDate;
            Session["ToDate"] = ToDate;
           


            if (dt.Rows.Count > 0)
            {
                Data1 = JsonConvert.SerializeObject(dt);
            }
            return Json(new { Data1 }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult SetInvoiceNonTravelDetailsGST(string EcfId, string InvId, string ProviderLocation, string GstinVendor, string Suppliergid, string InvNo,
            string InvDate, string Amount, string Desc, string WOTaxAmount, string IsVerify, string IsRemoved, string GstCharged, string ReceiverLocation, string GstinFiccl, string ServiceMonth)
        {
            try
            {

                //SetInvoiceNonTravelDetails


                DataTable dt = new DataTable();
                string Data1 = "", Data2 = "", Data3 = "";
                DataSet ds = db.SetInvoiceNonTravelDetails(Session["EcfGid"].ToString(), InvId, ProviderLocation, GstinVendor, Suppliergid, InvNo,
             InvDate, Amount, Desc, WOTaxAmount, IsVerify, IsRemoved, GstCharged, ReceiverLocation, GstinFiccl, ServiceMonth, objCmnFunctions.GetLoginUserGid().ToString());
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];

                        if (dt.Rows.Count > 0)
                        {
                            Session["invoiceGid"] = Convert.ToString(dt.Rows[0]["InvId"]);
                            Data1 = JsonConvert.SerializeObject(dt);
                        }
                    }
                    if (ds.Tables.Count > 1)
                    {
                        dt = ds.Tables[1];
                        if (dt.Rows.Count > 0)
                        {
                            Data2 = JsonConvert.SerializeObject(dt);
                        }
                    }
                    if (ds.Tables.Count > 2)
                    {
                        dt = ds.Tables[2];
                        if (dt.Rows.Count > 0) { Data3 = JsonConvert.SerializeObject(dt); }
                    }
                }
                //Session["invoicegid"] = inv.InvId;
                return Json(new { Data1, Data2, Data3 }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }

        //non travles gst

        [HttpPost]
        public JsonResult SetInvoicenonTravelDetailGST(string EcfId, string InvId, string ProviderLocation, string GstinVendor, string Suppliergid, string InvNo,
            string InvDate, string Amount, string Desc, string WOTaxAmount, string IsVerify, string IsRemoved, string GstCharged,
            string ReceiverLocation, string GstinFiccl, string ServiceMonth, string ExpTravelID, string SunCost, string AdjustAmount, string Cygnet_Gid)
        {
            try
            {
                DataTable dt = new DataTable();
                string Data1 = "", Data2 = "", Data3 = "";
                DataSet ds = db.SetInvoiceNonTravelDetailsForGST(Session["EcfGid"].ToString(), InvId, ProviderLocation, GstinVendor, Suppliergid, InvNo,
             InvDate, Amount, Desc, WOTaxAmount, IsVerify, IsRemoved, GstCharged, ReceiverLocation, GstinFiccl, ServiceMonth, objCmnFunctions.GetLoginUserGid().ToString(),
             ExpTravelID.ToString(), SunCost, AdjustAmount, Cygnet_Gid);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            Session["invoiceGid"] = Convert.ToString(dt.Rows[0]["InvId"]);
                            Data1 = JsonConvert.SerializeObject(dt);
                        }
                    }
                    if (ds.Tables.Count > 1)
                    {
                        dt = ds.Tables[1];
                        if (dt.Rows.Count > 0) { Data2 = JsonConvert.SerializeObject(dt); }
                    }
                    if (ds.Tables.Count > 2)
                    {
                        dt = ds.Tables[2];
                        if (dt.Rows.Count > 0) { Data3 = JsonConvert.SerializeObject(dt); }
                    }
                }
                //Session["invoicegid"] = inv.InvId;
                return Json(new { Data1, Data2, Data3 }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult DestroySession()
        {
            Session["invoiceGid"] = null;
            string Data1 = "Success";
            Data1 = JsonConvert.SerializeObject(Data1);
            return Json(new { Data1 }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public PartialViewResult _LoadGstDetails()
        {
            List<EOW_EmployeeeExpense> lstnew = new List<EOW_EmployeeeExpense>();
            return PartialView();
        }

        [HttpPost]
        public JsonResult _EmpECFCreate(EOW_EmployeeeExpense EmployeeeExpenseModel)
        {
            ForMailController mailsender = new ForMailController();
            string Err = "";
            try
            {
                if (ModelState.IsValid)
                {
                    //decimal pay = Convert.ToDecimal(Session["Ecfamountpayment1"].ToString());
                    //decimal exp = Convert.ToDecimal(Session["Ecfamountval"].ToString());
                    //decimal ecf = Convert.ToInt32(Session["Ecfamountvalmain"].ToString());
                    decimal pay = 0;
                    decimal exp = 0;
                    decimal ecfamount = Convert.ToDecimal(Session["Ecfamountvalmain"].ToString());
                    try
                    {
                        List<EOW_Payment> lstPayment = new List<EOW_Payment>();
                        lstPayment = objModel.GetEmployeeePayment(Session["EcfGid"].ToString(), "0").ToList();
                        for (int tr = 0; lstPayment.Count > tr; tr++)
                        {
                            pay = pay + Convert.ToDecimal(lstPayment[tr].PaymentAmount);
                        }

                        List<EOW_EmployeeeExpense> lst = new List<EOW_EmployeeeExpense>();
                        lst = objModel.GetEmployeeeExpensenew(Session["EcfGid"].ToString(), "0").ToList();
                        //Ramya added acfamount
                        ecfamount = Convert.ToDecimal(lst[0].ECF_Amount.ToString());
                        for (int tr = 0; lst.Count > tr; tr++)
                        {
                            exp = exp + Convert.ToDecimal(lst[tr].Exp_Amount.ToString()); ;
                        }
                        exp = exp - ecfamount;
                    }
                    catch (Exception ex)
                    {
                        objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                    }


                    var result = objModel.GetEmployeePayModeEraAcc(Convert.ToInt32(Session["SelfModeRaiseid"]));
                    if (exp != 0)
                    {
                        Err = "Expense";
                    }
                    else if (pay < 0)
                    {
                        Err = "Payment";
                    }
                    else if (result == "No")
                    {
                        Err = "BankAcc";
                    }
                    else
                    {
                        if (Session["loginraisemode"] != null)
                        {
                            Err = objModel.Insertecfdraftproxcy(EmployeeeExpenseModel, Session["EcfGid"].ToString());
                            Err = "Proxy";
                        }
                        else
                        {
                            //string demlat = objModel.Getraiserdelmat(Session["EcfGid"].ToString(), Session["SelfModeRaiseid"].ToString());
                            //if (demlat == "Yes")
                            //{
                            Err = objModel.Insertecf(EmployeeeExpenseModel, Session["EcfGid"].ToString(), "0", objCmnFunctions.GetLoginUserGid().ToString(), Session["SelfModeRaiseid"].ToString(), "E", Session["QueueGide"].ToString());
                            if (Err == "")
                            {
                                Err = "Error";
                            }
                            else
                            {
                                Err = Err.ToString();
                                Session["QueueGide"] = null;
                                Session["EcfGid"] = null;
                                Session["invoiceGid"] = null;
                                Session["Ecfamountpayment1"] = null;
                                Session["Ecfamountval"] = null;
                                //string results = mailsender.sendusermail("EOW", "Regular", "658", "S", "0");
                            }
                            //}
                            //else
                            //{
                            //    Err = "Delmat";
                            //}
                        }
                    }
                }
                ViewBag.SearchItems = null;
                return Json(Err, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("Error occurred..." + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public PartialViewResult NonTravelModeEdit(int id, string viewfor)
        {
            try
            {
                EOW_EmployeeeExpense objMExpenseEdit = new EOW_EmployeeeExpense();
                List<EOW_EmployeeeExpense> objobjMExpense = new List<EOW_EmployeeeExpense>();
                string invoicegid;
                if (viewfor == "edit")
                {
                    ViewBag.viewfor = "edit";
                }
                else if (viewfor == "view")
                {
                    ViewBag.viewfor = "view";
                }
                id = Convert.ToInt32(Session["EmpExpenseactiverow"].ToString());
                ViewBag.SearchItems = null;
                if (Session["invoiceGid"] != null)
                {
                    invoicegid = Session["invoiceGid"].ToString();

                }
                else
                {
                    invoicegid = "0";
                }

                //objobjMExpense = objModel.GetEditEmployeeeExpensenew(Session["EcfGid"].ToString(), invoicegid, id).ToList();
                objobjMExpense = objModel.SelectEmployeeeExpensebyidnew(Session["EcfGid"].ToString(), invoicegid, id).ToList();


                //objMExpenseEdit.NatureofExpensesName = objobjMExpense[0].NatureofExpensesName.ToString(); 

                objMExpenseEdit.Exp_OUCode = objobjMExpense[0].Exp_OUCode.ToString();
                objMExpenseEdit.Exp_ProductCode = objobjMExpense[0].Exp_ProductCode.ToString();
                objMExpenseEdit.Exp_ClaimPeriodFrom = objobjMExpense[0].Exp_ClaimPeriodFrom.ToString();
                objMExpenseEdit.Exp_ClaimPeriodTo = objobjMExpense[0].Exp_ClaimPeriodTo.ToString();
                objMExpenseEdit.Exp_FC = objobjMExpense[0].Exp_FC.ToString();
                objMExpenseEdit.Exp_CC = objobjMExpense[0].Exp_CC.ToString();
                objMExpenseEdit.Exp_FCC = new SelectList(objModel.fcdata().ToList(), "raiserfcId", "raiserfcName", objobjMExpense[0].Exp_FC.ToString());
                objMExpenseEdit.Exp_CCC = new SelectList(objModel.ccdata().ToList(), "raiserccId", "raiserccName", objobjMExpense[0].Exp_CC.ToString());
                objMExpenseEdit.Exp_FCCC = objModel.selectfccc(objobjMExpense[0].Exp_FC.ToString(), objobjMExpense[0].Exp_CC.ToString());
                objMExpenseEdit.Exp_Amount = objCmnFunctions.GetINRAmount(objobjMExpense[0].Exp_Amount.ToString());
                objMExpenseEdit.NatureofExpensesId = objobjMExpense[0].NatureofExpensesId;
                objMExpenseEdit.ExpenseCategoryId = objobjMExpense[0].ExpenseCategoryId;
                objMExpenseEdit.SubCategoryId = objobjMExpense[0].SubCategoryId;
                objMExpenseEdit.ExpNatureofExpdata = new SelectList(objModel.NatureofExpensesdataother().ToList(), "NatureofExpensesId", "NatureofExpensesName", objobjMExpense[0].NatureofExpensesId.ToString());
                objMExpenseEdit.ExpCatdata = new SelectList(objModel.ExpenseCategorydata(objobjMExpense[0].NatureofExpensesId).ToList(), "ExpenseCategoryId", "ExpenseCategoryName", objobjMExpense[0].ExpenseCategoryId.ToString());
                objMExpenseEdit.ExpSubCatdata = new SelectList(objModel.SubCategorydata(objobjMExpense[0].ExpenseCategoryId).ToList(), "SubCategoryId", "SubCategoryName", objobjMExpense[0].SubCategoryId.ToString());
                objMExpenseEdit.Citydata = new SelectList(objModel.tTravelcitydata().ToList(), "TravelCityName", "TravelCityName", objobjMExpense[0].PlaceFrom.ToString());
                objMExpenseEdit.Citydatato = new SelectList(objModel.tTravelcitydata().ToList(), "TravelCityName", "TravelCityName", objobjMExpense[0].PlaceTo.ToString());

                objMExpenseEdit.HsncodeList = new SelectList(objModel.HsnCodeList(objobjMExpense[0].SubCategoryId).ToList(), "hsnid", "hsncode", Convert.ToString(objobjMExpense[0].Hsnid));
                objMExpenseEdit.PlaceFrom = objobjMExpense[0].PlaceFrom;
                objMExpenseEdit.PlaceTo = objobjMExpense[0].PlaceTo;
                objMExpenseEdit.HsnDescription = objobjMExpense[0].HsnDescription;
                objMExpenseEdit.RCMFlag = objobjMExpense[0].RCMFlag;
                ViewBag.TravelotherClaimheader = objMExpenseEdit;
                // Session["Ecfamountvalforedit"] = Convert.ToDecimal(Session["Ecfamountval"].ToString()) + Convert.ToDecimal(objobjMExpense[0].Exp_Amount.ToString());
                return PartialView(objMExpenseEdit);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return PartialView();
            }
        }

        [HttpPost]
        public JsonResult _Emplistaddp(EOW_Employeelst Employeelst)
        {
            try
            {
                Session["EmpExpenseactiverow"] = Employeelst.empName.ToString();
                string message = "";
                message = "Success";
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("Error occurred..." + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LoadInvoiceHeaderDetails(string InvId)
        {
            try
            {
                DataTable dt = new DataTable();
                string Data1 = "", Data2 = "", Data3 = "";
                DataSet ds = db.LoadInvoiceHeaderDetails(Convert.ToString(Session["EcfGid"]), InvId);
                Session["invoiceGid"] = !string.IsNullOrEmpty(InvId) ? InvId : null;
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        if (dt.Rows.Count > 0) { Data1 = JsonConvert.SerializeObject(dt); }
                    }
                    if (ds.Tables.Count > 1)
                    {
                        dt = ds.Tables[1];
                        if (dt.Rows.Count > 0) { Data2 = JsonConvert.SerializeObject(dt); }
                    }
                    if (ds.Tables.Count > 2)
                    {
                        dt = ds.Tables[2];
                        if (dt.Rows.Count > 0) { Data3 = JsonConvert.SerializeObject(dt); }
                    }
                }
                return Json(new { Data1, Data2, Data3 }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult NontravelExpenseEdit(EOW_TravelClaim objMExpenseEdit)
        {
            try
            {
                string msg = "";
                msg = objModel.UpdateNonEmployeeeExpensenew(objMExpenseEdit, Session["EcfGid"].ToString(), Session["EmpExpenseactiverow"].ToString(), Session["SelfModeRaiseid"].ToString());
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("Error occurred..." + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteEmpExpense(EOW_EmployeeeExpense EmployeeeExpensemodel)
        {
            try
            {
                int TravelModeGID = (int)EmployeeeExpensemodel.Exp_GID;
                string delamt = objModel.DeletePettyCashExpense(TravelModeGID, Session["EcfGid"].ToString());
                return Json(EmployeeeExpensemodel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("Error occurred...", JsonRequestBehavior.AllowGet);
            }
        }

        //Vadivu  Invoice Delete 19.12.2020
        [HttpPost]
        public JsonResult SetInvoiceTravelDetails(string EcfId, string InvId, string ProviderLocation, string GstinVendor, string Suppliergid, string InvNo,
            string InvDate, string Amount, string Desc, string WOTaxAmount, string IsVerify, string IsRemoved, string GstCharged, string ReceiverLocation, string GstinFiccl, string ServiceMonth)
        {
            try
            {
                DataTable dt = new DataTable();
                string Data1 = "", Data2 = "", Data3 = "", Data4 = "";
                DataSet ds = db.SetInvoiceTravelDetails(Session["EcfGid"].ToString(), InvId, ProviderLocation, GstinVendor, Suppliergid, InvNo,
             InvDate, Amount, Desc, WOTaxAmount, IsVerify, IsRemoved, GstCharged, ReceiverLocation, GstinFiccl, ServiceMonth, objCmnFunctions.GetLoginUserGid().ToString());
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            Session["invoiceGid"] = Convert.ToString(dt.Rows[0]["InvId"]);
                            Data1 = JsonConvert.SerializeObject(dt);
                        }
                    }
                    if (ds.Tables.Count > 1)
                    {
                        dt = ds.Tables[1];
                        if (dt.Rows.Count > 0) { Data2 = JsonConvert.SerializeObject(dt); }
                    }
                    if (ds.Tables.Count > 2)
                    {
                        dt = ds.Tables[2];
                        if (dt.Rows.Count > 0) { Data3 = JsonConvert.SerializeObject(dt); }
                    }

                    //split payment checking

                    if (ds.Tables.Count > 3)
                    {
                        dt = ds.Tables[3];
                        if (dt.Rows.Count > 0)
                        {
                            string PayMethod = dt.Rows[0]["PayMethod"].ToString();

                            Data4 = JsonConvert.SerializeObject(PayMethod);
                        }


                    }


                }
                //Session["invoicegid"] = inv.InvId;
                return Json(new { Data1, Data2, Data3, Data4 }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult _EmpECFDraft(EOW_EmployeeeExpense EmployeeeExpenseModel)
        {
            string Err = "";
            try
            {
                Err = objModel.Insertecfdraft(EmployeeeExpenseModel, Session["EcfGid"].ToString());
                if (Err == "")
                {
                    Err = "Error";
                }
                else
                {
                    Err = "Success";
                    Session["EcfGid"] = null;
                    Session["invoiceGid"] = null;
                    Session["Ecfamountpaymenttt"] = null;
                    Session["Ecfamountvaloe"] = null;
                    Session["Ecfamountvaltm"] = null;
                    Session["QueueGide"] = null;
                }
                ViewBag.SearchItems = null;
                return Json(Err, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("Error occurred..." + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        //12-03-2021 Selva Created for Supplier Name Auto Complete in Cygnet Search
        [HttpPost]
        public JsonResult AutoCompletecodes(string term)
        {
            string Err = "";
            List<CentraldataModel> objparenttax = new List<CentraldataModel>();
            objparenttax = objModel.SelectSupplierSearchcode(term).ToList();
            return Json(objparenttax, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AutoCompletenames(string term)
        {
            string Err = "";
            List<CentraldataModel> objparenttax = new List<CentraldataModel>();
            objparenttax = objModel.SelectSupplierSearchname(term).ToList();
            return Json(objparenttax, JsonRequestBehavior.AllowGet);
        }

    }


}
