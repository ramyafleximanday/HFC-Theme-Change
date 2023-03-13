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
    public class TravelClaimNewGSTController : Controller
    {
        ErrorLog objErrorLog = new ErrorLog();
        private EOW_IRepository objModelTravel;
        CmnFunctions objCmnFunctions = new CmnFunctions();
        private FileServer Cmnfs = new FileServer();
        LocalConveyanceNewController locals = new LocalConveyanceNewController();
        CentralModel cen = new CentralModel();
        CommonIUD objCommonIUD = new CommonIUD();
        proLib plib = new proLib();
        dbLib db = new dbLib();
        DataTable dt;
        EOW_EmployeeeExpense obj = new EOW_EmployeeeExpense();
        EOW_TravelClaim objtravel = new EOW_TravelClaim();
        EOW_EmployeeeExpense eowemp = new EOW_EmployeeeExpense();
        CygnetDataModel ObjCygnet = new CygnetDataModel();
        List<EOW_EmployeeeExpense> objmaindetail = new List<EOW_EmployeeeExpense>();
        string SupName = string.Empty, Panno = string.Empty, InvoiceNo = string.Empty, GSNTNNo = string.Empty, Fromdate = string.Empty, Todate = string.Empty;
        
       

        // public  string suppliername="";

        public TravelClaimNewGSTController()
            : this(new EOW_DataModel())
        {

        }
        public TravelClaimNewGSTController(EOW_IRepository objM)
        {
            objModelTravel = objM;
        }

        // Dash board index new

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
                    //Session["invoiceGid"] = eowemp.invoice_GID.ToString();
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
                            Session["SelfModeecfval"] = objCmnFunctions.GetLoginUserGid().ToString();
                        }
                        else
                        {
                            Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginUserGid().ToString();
                            Session["SelfModeecfval"] = objCmnFunctions.GetLoginUserGid().ToString();
                        }
                        List<EOW_EmployeeeExpense> objmaindetaild = new List<EOW_EmployeeeExpense>();
                        objmaindetaild = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginProxyUserGid().ToString()).ToList();
                        Session["raiserfcc"] = objmaindetaild[0].Exp_FC.ToString();
                        Session["raiserccc"] = objmaindetaild[0].Exp_CC.ToString();
                    }

                    else
                    {


                        Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginUserGid().ToString();
                        Session["SelfModeecfval"] = objCmnFunctions.GetLoginUserGid().ToString();
                        objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginUserGid().ToString()).ToList();
                        eowemp.Grade = objmaindetail[0].Grade.ToString();
                        eowemp.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
                        eowemp.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
                        eowemp.raisermodeId = "S";//"C OR S OR P" 
                        Session["raiserfcc"] = objmaindetail[0].Exp_FC.ToString();
                        Session["raiserccc"] = objmaindetail[0].Exp_CC.ToString();
                        objobjMExpense = objModelTravel.tSelectEmployeeeBasic(Session["SelfModeRaiseid"].ToString()).ToList();
                        objMExpense.OUCode = objobjMExpense[0].OUCode.ToString();
                        objMExpense.ProductCode = objobjMExpense[0].ProductCode.ToString();
                        objMExpense.Exp_FCC = new SelectList(objModelTravel.fcdata().ToList(), "raiserfcId", "raiserfcName", Session["raiserfcc"].ToString());
                        objMExpense.Exp_CCC = new SelectList(objModelTravel.ccdata().ToList(), "raiserccId", "raiserccName", Session["raiserccc"].ToString());
                        objMExpense.Exp_FCCC = objobjMExpense[0].Exp_FCCC.ToString();
                        objMExpense.ExpNatureofExpdata = new SelectList(objModelTravel.NatureofExpensesdata().ToList(), "NatureofExpensesId", "NatureofExpensesName");
                        objMExpense.TravelModedata = new SelectList(objModelTravel.tTravelModedata().ToList(), "TravelModeId", "TravelModeName");
                        objMExpense.Citydata = new SelectList(objModelTravel.tTravelcitydata().ToList(), "TravelCityId", "TravelCityName");
                        ViewBag.EOW_EmployeeeExpenseheader = objMExpense;
                    }
                }

                else
                {

                    EOW_TravelClaim objMExpense = new EOW_TravelClaim();
                    List<EOW_TravelClaim> objobjMExpense = new List<EOW_TravelClaim>();
                    List<EOW_EmployeeeExpense> objmaindetail = new List<EOW_EmployeeeExpense>();

                    if (Session["loginraisemode"] != null)
                    {
                        Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginProxyUserGid().ToString();
                        Session["SelfModeecfval"] = objCmnFunctions.GetLoginProxyUserGid().ToString();
                        objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginProxyUserGid().ToString()).ToList();
                        eowemp.Grade = objmaindetail[0].Grade.ToString();
                        eowemp.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
                        eowemp.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
                        eowemp.raisermodeId = "P";
                        objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginProxyUserGid().ToString()).ToList();
                        Session["raiserfcc"] = objmaindetail[0].Exp_FC.ToString();
                        Session["raiserccc"] = objmaindetail[0].Exp_CC.ToString();
                    }
                    else
                    {

                        Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginUserGid().ToString();
                        Session["SelfModeecfval"] = objCmnFunctions.GetLoginUserGid().ToString();
                        objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginUserGid().ToString()).ToList();
                        eowemp.Grade = objmaindetail[0].Grade.ToString();
                        eowemp.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
                        eowemp.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
                        eowemp.raisermodeId = "S";//"C OR S OR P" 
                        Session["raiserfcc"] = objmaindetail[0].Exp_FC.ToString();
                        Session["raiserccc"] = objmaindetail[0].Exp_CC.ToString();
                        objobjMExpense = objModelTravel.tSelectEmployeeeBasic(Session["SelfModeRaiseid"].ToString()).ToList();
                        objMExpense.OUCode = objobjMExpense[0].OUCode.ToString();
                        objMExpense.ProductCode = objobjMExpense[0].ProductCode.ToString();
                        objMExpense.Exp_FCC = new SelectList(objModelTravel.fcdata().ToList(), "raiserfcId", "raiserfcName", Session["raiserfcc"].ToString());
                        objMExpense.Exp_CCC = new SelectList(objModelTravel.ccdata().ToList(), "raiserccId", "raiserccName", Session["raiserccc"].ToString());
                        objMExpense.Exp_FCCC = objobjMExpense[0].Exp_FCCC.ToString();
                        objMExpense.ExpNatureofExpdata = new SelectList(objModelTravel.NatureofExpensesdata().ToList(), "NatureofExpensesId", "NatureofExpensesName");
                        objMExpense.TravelModedata = new SelectList(objModelTravel.tTravelModedata().ToList(), "TravelModeId", "TravelModeName");
                        objMExpense.Citydata = new SelectList(objModelTravel.tTravelcitydata().ToList(), "TravelCityId", "TravelCityName");
                        ViewBag.EOW_EmployeeeExpenseheader = objMExpense;
                    }



                    ViewBag.process = "";
                    Session["EcfGid"] = null;
                    Session["invoiceGid"] = null;
                    Session["Ecfamountpaymenttt"] = null;
                    Session["Ecfamountvaloe"] = null;
                    Session["Ecfamountvaltm"] = null;
                    ViewBag.PayMethod = "Single Payment";
                }

                //selva 15-12-2020
                if (Session["EcfGid"] != null)
                {
                    obj = objModelTravel.GetEcfHeader(Session["EcfGid"].ToString());
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

                eowemp.Raiser_Modedata = new SelectList(objModelTravel.GetRaiserMode().ToList(), "raisermodeId", "raisermodeName", eowemp.raisermodeId);
                ViewBag.EOW_Employeeheader = eowemp;
                //  Session["EcfGid"] = null;
                Session["NonBranchClaimFlage"] = objModelTravel.travelbranchflag(objCmnFunctions.GetLoginUserGid());
                Session["Ecfdeclaratonnote"] = objModelTravel.GetDecnote("3", "S").ToString();
                return View();
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return View();
            }
        }

        //

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
        //            objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginProxyUserGid().ToString()).ToList();
        //            eowemp.Grade = objmaindetail[0].Grade.ToString();
        //            eowemp.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
        //            eowemp.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
        //            eowemp.raisermodeId = "P";
        //            objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginProxyUserGid().ToString()).ToList();
        //            Session["raiserfcc"] = objmaindetail[0].Exp_FC.ToString();
        //            Session["raiserccc"] = objmaindetail[0].Exp_CC.ToString();
        //        }
        //        else
        //        {

        //            Session["SelfModeRaiseid"] = objCmnFunctions.GetLoginUserGid().ToString();
        //            Session["SelfModeecfval"] = objCmnFunctions.GetLoginUserGid().ToString();
        //            objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginUserGid().ToString()).ToList();
        //            eowemp.Grade = objmaindetail[0].Grade.ToString();
        //            eowemp.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
        //            eowemp.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
        //            eowemp.raisermodeId = "S";//"C OR S OR P" 
        //            Session["raiserfcc"] = objmaindetail[0].Exp_FC.ToString();
        //            Session["raiserccc"] = objmaindetail[0].Exp_CC.ToString();
        //            objobjMExpense = objModelTravel.tSelectEmployeeeBasic(Session["SelfModeRaiseid"].ToString()).ToList();
        //            objMExpense.OUCode = objobjMExpense[0].OUCode.ToString();
        //            objMExpense.ProductCode = objobjMExpense[0].ProductCode.ToString();
        //            objMExpense.Exp_FCC = new SelectList(objModelTravel.fcdata().ToList(), "raiserfcId", "raiserfcName", Session["raiserfcc"].ToString());
        //            objMExpense.Exp_CCC = new SelectList(objModelTravel.ccdata().ToList(), "raiserccId", "raiserccName", Session["raiserccc"].ToString());
        //            objMExpense.Exp_FCCC = objobjMExpense[0].Exp_FCCC.ToString();
        //            objMExpense.ExpNatureofExpdata = new SelectList(objModelTravel.NatureofExpensesdata().ToList(), "NatureofExpensesId", "NatureofExpensesName");
        //            objMExpense.TravelModedata = new SelectList(objModelTravel.tTravelModedata().ToList(), "TravelModeId", "TravelModeName");
        //            objMExpense.Citydata = new SelectList(objModelTravel.tTravelcitydata().ToList(), "TravelCityId", "TravelCityName");
        //            ViewBag.EOW_EmployeeeExpenseheader = objMExpense;
        //        }

        //        //07.12.2020 Vadivu

        //        if (Session["EcfGid"] != null)
        //        {
        //            obj = objModelTravel.GetEcfHeader(Session["EcfGid"].ToString());
        //            if (obj.ClaimMonth != null)
        //            {
        //                eowemp.ECF_Amount = obj.ECF_Amount;
        //                eowemp.ecfremark = obj.ecfremark;
        //                eowemp.noofperson = obj.noofperson;
        //                eowemp.ClaimMonth = obj.ClaimMonth;
        //                eowemp.ECF_Date = obj.ECF_Date;
        //            }
        //            else
        //            {
        //                Session.Remove("EcfGid");
        //            }
        //        }
        //        eowemp.Raiser_Modedata = new SelectList(objModelTravel.GetRaiserMode().ToList(), "raisermodeId", "raisermodeName", eowemp.raisermodeId);
        //        ViewBag.EOW_Employeeheader = eowemp;
        //        Session["NonBranchClaimFlage"] = objModelTravel.travelbranchflag(objCmnFunctions.GetLoginUserGid());
        //        Session["Ecfdeclaratonnote"] = objModelTravel.GetDecnote("3", "S").ToString();
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
        //        return View();
        //    }
        //}


        [HttpPost]
        public JsonResult ECFDetails(EOW_EmployeeeExpense obj)
        {
            try
            {
                string ecfnewamt = obj.ECF_Amount;
                obj.ECF_Amount = objCmnFunctions.GetRemovecommas(obj.ECF_Amount);
                string clmmonth = "";
                string message = "";
                string insrtinvoice = "";
                string IFSCCode = "";
                string IFSCCodeMess = "";
                List<EOW_EmployeeeExpense> objmaindetail = new List<EOW_EmployeeeExpense>();
                objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginUserGid().ToString()).ToList();
                if (obj.ecfremark != null)
                {
                    ViewBag.ecfrmarks = obj.ecfremark;
                }

                EOW_EmployeeeExpense eowemp = new EOW_EmployeeeExpense();
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
                RaiserDetail();
                Session["Ecfamountvalmain"] = obj.ECF_Amount;
                obj.Raiser_Modedata = new SelectList(objModelTravel.GetRaiserMode().ToList(), "raisermodeId", "raisermodeName", obj.raisermodeId);
                clmmonth = obj.ClaimMonth;
                int nodays = 0;
                string result = objModelTravel.GetStatusexcel("", Session["SelfModeecfval"].ToString(), "", "Ecfraiser");
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
                        message = objModelTravel.InsertEmployeeeBasicupdate(obj, objCmnFunctions.GetLoginUserGid().ToString(), Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString());
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

                            IFSCCode = objModelTravel.InsertEmployeeePaymentbasicupdate(Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString(), Session["Ecfamountvalmain"].ToString());

                            if (IFSCCode.ToString().Trim().Equals(""))
                            {

                                IFSCCodeMess = "IFSC Code is not updated!!! Your Claim may get REJECT by EPU Team!";
                            }


                            //insrtinvoice = objModelTravel.InsertEmployeeeBasicinvoiceupdate(obj, Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString());
                            //if (insrtinvoice != "")
                            //{
                            //    Session["invoiceGid"] = insrtinvoice.ToString();
                            //    string IFSCCode = "";
                            //    ViewBag.IFSCCodemsg = "";
                            //    IFSCCode = objModelTravel.InsertEmployeeePaymentbasicupdate(Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString(), Session["Ecfamountvalmain"].ToString());
                            //    message = "success";
                            //    if (IFSCCode.ToString().Trim().Equals(""))
                            //    {
                            //        ViewBag.IFSCCodemsg = "IFSC Code is not update!!! Your Claim may get REJECT by EPU Team!";
                            //    }
                            //    //message = objModelTravel.insertempperson(Employeelst, Session["EcfGid"].ToString());
                            //}
                            //else
                            //{
                            //    message = "ECF details Not Updated Properly Please check ECF Header Details";
                            //}
                            message = "Success";
                        }
                    }
                    else
                    {
                        message = objModelTravel.InsertEmployeeeBasic(obj, objCmnFunctions.GetLoginUserGid().ToString(), "T");
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
                            Session["EcfGid"] = objModelTravel.selectecfgidBasic(obj, objCmnFunctions.GetLoginUserGid().ToString());
                            insrtinvoice = objModelTravel.InsertEmployeeeBasicinvoice(obj, Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString(), "T");
                            if (insrtinvoice != "")
                            {
                                //  string IFSCCode = "";
                                ViewBag.IFSCCodemsg = "";
                                if (Session["invoiceGid"] != null && Session["invoiceGid"] != "")
                                {
                                    // leave blank // ramya added on 19 sep 22 since or condition allowing to read the below method
                                }
                                else
                                {
                                    Session["invoiceGid"] = objModelTravel.selectinvoiceidBasic(obj, Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString(), "E");// ramya commentted on 19 sep 22 on overall app to avoid invoice gid mismatch

                                }
                                IFSCCode = objModelTravel.InsertEmployeeePaymentbasic(Session["SelfModeRaiseid"].ToString(), Session["EcfGid"].ToString(), Session["invoiceGid"].ToString(), Session["Ecfamountvalmain"].ToString());
                                if (IFSCCode.ToString().Trim().Equals(""))
                                {
                                    ViewBag.IFSCCodemsg = "IFSC Code is not updated!!! Your Claim may get REJECT by EPU Team!";
                                }
                                EOW_Employeelst addemp = new EOW_Employeelst();
                                addemp.employeeGid = Convert.ToInt32(Session["SelfModeRaiseid"].ToString());
                                addemp.empbranch = objModelTravel.Getemployeebrach(Session["SelfModeRaiseid"].ToString());
                                addemp.empfc = Session["raiserfcc"].ToString();
                                addemp.empcc = Session["raiserccc"].ToString();
                                message = objModelTravel.insertempperson(addemp, Session["EcfGid"].ToString());
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
                ViewBag.EOW_Employeeheader = obj;
                Session["NonBranchClaimFlage"] = objModelTravel.travelbranchflag(objCmnFunctions.GetLoginUserGid());


                var res = new { message, IFSCCodeMess };
                // return Json(message, IFSCCodeMess, JsonRequestBehavior.AllowGet);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("Error occurred..." + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }


        public PartialViewResult TravelModeEdit(int id, string viewfor)
        {
            try
            {
                EOW_TravelClaim objMExpenseEdit = new EOW_TravelClaim();
                List<EOW_TravelClaim> objobjMExpense = new List<EOW_TravelClaim>();
                Session["invoiceGid"] = "0";
                if (viewfor == "edit")
                {
                    ViewBag.viewfor = "edit";
                }
                else if (viewfor == "view")
                {
                    ViewBag.viewfor = "view";
                }
                id = Convert.ToInt32(Session["TravelModeactiverow"].ToString());
                ViewBag.SearchItems = null;
                objobjMExpense = objModelTravel.GetTravelModedatasingle(Session["EcfGid"].ToString(), Session["invoiceGid"].ToString(), "T", id).ToList();

                objMExpenseEdit.NatureofExpensesId = Convert.ToInt32(objobjMExpense[0].NatureofExpensesId.ToString());
                objMExpenseEdit.ExpenseCategoryId = Convert.ToInt32(objobjMExpense[0].ExpenseCategoryId.ToString());
                objMExpenseEdit.SubCategoryId = Convert.ToInt32(objobjMExpense[0].SubCategoryId.ToString());
                objMExpenseEdit.OUCode = objobjMExpense[0].OUCode.ToString();
                objMExpenseEdit.ProductCode = objobjMExpense[0].ProductCode.ToString();
                objMExpenseEdit.ClaimPeriodFrom = objobjMExpense[0].ClaimPeriodFrom.ToString();
                objMExpenseEdit.ClaimPeriodTo = objobjMExpense[0].ClaimPeriodTo.ToString();
                objMExpenseEdit.FC = objobjMExpense[0].FC.ToString();
                objMExpenseEdit.CC = objobjMExpense[0].CC.ToString();

                objMExpenseEdit.Exp_FCC = new SelectList(objModelTravel.fcdata().ToList(), "raiserfcId", "raiserfcName", objobjMExpense[0].FC.ToString());
                objMExpenseEdit.Exp_CCC = new SelectList(objModelTravel.ccdata().ToList(), "raiserccId", "raiserccName", objobjMExpense[0].CC.ToString());
                objMExpenseEdit.Exp_FCCC = objobjMExpense[0].Exp_FCCC.ToString();

                objMExpenseEdit.travelDescription = objobjMExpense[0].travelDescription.ToString();
                objMExpenseEdit.Amount = objCmnFunctions.GetINRAmount(objobjMExpense[0].Amount.ToString());
                objMExpenseEdit.Distance = objobjMExpense[0].Distance.ToString();
                objMExpenseEdit.Rate = objobjMExpense[0].Rate.ToString();

                objMExpenseEdit.Traveltypes = objModelTravel.ExpenseCategorydataflag(objobjMExpense[0].NatureofExpensesId);
                objMExpenseEdit.Travelclass = objModelTravel.Travelclassdataflag(objobjMExpense[0].TravelModeId);

                objMExpenseEdit.ExpNatureofExpdata = new SelectList(objModelTravel.NatureofExpensesdata().ToList(), "NatureofExpensesId", "NatureofExpensesName", objobjMExpense[0].NatureofExpensesId.ToString());
                objMExpenseEdit.ExpCatdata = new SelectList(objModelTravel.ExpenseCategorydata(objobjMExpense[0].NatureofExpensesId).ToList(), "ExpenseCategoryId", "ExpenseCategoryName", objobjMExpense[0].ExpenseCategoryId.ToString());
                objMExpenseEdit.ExpSubCatdata = new SelectList(objModelTravel.SubCategorydata(objobjMExpense[0].ExpenseCategoryId).ToList(), "SubCategoryId", "SubCategoryName", objobjMExpense[0].SubCategoryId.ToString());
                objMExpenseEdit.Citydata = new SelectList(objModelTravel.tTravelcitydata().ToList(), "TravelCityName", "TravelCityName", objobjMExpense[0].PlaceFrom.ToString());
                objMExpenseEdit.Citydatato = new SelectList(objModelTravel.tTravelcitydata().ToList(), "TravelCityName", "TravelCityName", objobjMExpense[0].PlaceTo.ToString());
                objMExpenseEdit.TravelModedata = new SelectList(objModelTravel.tTravelModedata().ToList(), "TravelModeId", "TravelModeName", objobjMExpense[0].TravelModeId.ToString());
                //objMExpenseEdit.HsncodeList = new SelectList(objModelTravel.GetHsnList().ToList(), "TravelHsnid", "TravelHsnCode", objobjMExpense[0].TravelHsnid.ToString());
                objMExpenseEdit.HsncodeList = new SelectList(objModelTravel.HsnCodeList(objobjMExpense[0].SubCategoryId).ToList(), "hsnid", "hsncode", Convert.ToString(objobjMExpense[0].TravelHsnid));

                objMExpenseEdit.TravelHsnDesc = objobjMExpense[0].TravelHsnDesc.ToString();
                //Ramya Added
                objMExpenseEdit.RCMFlag = objobjMExpense[0].RCMFlag.ToString();
                objMExpenseEdit.TravelClassdata = new SelectList(objModelTravel.tTravelClassdatadata(objobjMExpense[0].TravelModeId).ToList(), "TravelClassId", "TravelClassName", objobjMExpense[0].TravelClassId.ToString());
                Session["Ecfamountvaltmeditt"] = Convert.ToDecimal(Session["Ecfamountvaltm"].ToString()) + Convert.ToDecimal(objobjMExpense[0].Amount.ToString());
                RaiserDetail();
                ViewBag.TravelotherClaimheader = objMExpenseEdit;
                ViewBag.EOW_Employeeheader = obj;

                return PartialView(objMExpenseEdit);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return PartialView();
            }
        }

        public PartialViewResult TravelClaimAddExp(int id, string viewfor)
        {
            try
            {
                EOW_TravelClaim objMExpenseEdit = new EOW_TravelClaim();
                List<EOW_TravelClaim> objobjMExpense = new List<EOW_TravelClaim>();
                if (Session["invoiceGid"] == null)
                    Session["invoiceGid"] = "0";

                if (viewfor == "edit")
                {
                    ViewBag.viewfor = "edit";
                }
                else if (viewfor == "view")
                {
                    ViewBag.viewfor = "view";
                }
                id = Convert.ToInt32(Session["TravelModeactiverow"].ToString());
                ViewBag.SearchItems = null;
                objobjMExpense = objModelTravel.GetTravelModedatasingle(Session["EcfGid"].ToString(), Session["invoiceGid"].ToString(), "T", id).ToList();

                objMExpenseEdit.NatureofExpensesId = Convert.ToInt32(objobjMExpense[0].NatureofExpensesId.ToString());
                objMExpenseEdit.ExpenseCategoryId = Convert.ToInt32(objobjMExpense[0].ExpenseCategoryId.ToString());
                objMExpenseEdit.SubCategoryId = Convert.ToInt32(objobjMExpense[0].SubCategoryId.ToString());
                objMExpenseEdit.OUCode = objobjMExpense[0].OUCode.ToString();
                objMExpenseEdit.ProductCode = objobjMExpense[0].ProductCode.ToString();
                objMExpenseEdit.ClaimPeriodFrom = objobjMExpense[0].ClaimPeriodFrom.ToString();
                objMExpenseEdit.ClaimPeriodTo = objobjMExpense[0].ClaimPeriodTo.ToString();
                objMExpenseEdit.FC = objobjMExpense[0].FC.ToString();
                objMExpenseEdit.CC = objobjMExpense[0].CC.ToString();

                objMExpenseEdit.Exp_FCC = new SelectList(objModelTravel.fcdata().ToList(), "raiserfcId", "raiserfcName", objobjMExpense[0].FC.ToString());
                objMExpenseEdit.Exp_CCC = new SelectList(objModelTravel.ccdata().ToList(), "raiserccId", "raiserccName", objobjMExpense[0].CC.ToString());
                objMExpenseEdit.Exp_FCCC = objobjMExpense[0].Exp_FCCC.ToString();

                objMExpenseEdit.travelDescription = objobjMExpense[0].travelDescription.ToString();
                objMExpenseEdit.Amount = objCmnFunctions.GetINRAmount(objobjMExpense[0].Amount.ToString());
                objMExpenseEdit.Distance = objobjMExpense[0].Distance.ToString();
                objMExpenseEdit.Rate = objobjMExpense[0].Rate.ToString();

                objMExpenseEdit.Traveltypes = objModelTravel.ExpenseCategorydataflag(objobjMExpense[0].NatureofExpensesId);
                objMExpenseEdit.Travelclass = objModelTravel.Travelclassdataflag(objobjMExpense[0].TravelModeId);

                objMExpenseEdit.ExpNatureofExpdata = new SelectList(objModelTravel.NatureofExpensesdata().ToList(), "NatureofExpensesId", "NatureofExpensesName", objobjMExpense[0].NatureofExpensesId.ToString());
                objMExpenseEdit.ExpCatdata = new SelectList(objModelTravel.ExpenseCategorydata(objobjMExpense[0].NatureofExpensesId).ToList(), "ExpenseCategoryId", "ExpenseCategoryName", objobjMExpense[0].ExpenseCategoryId.ToString());
                objMExpenseEdit.ExpSubCatdata = new SelectList(objModelTravel.SubCategorydata(objobjMExpense[0].ExpenseCategoryId).ToList(), "SubCategoryId", "SubCategoryName", objobjMExpense[0].SubCategoryId.ToString());
                objMExpenseEdit.Citydata = new SelectList(objModelTravel.tTravelcitydata().ToList(), "TravelCityName", "TravelCityName", objobjMExpense[0].PlaceFrom.ToString());
                objMExpenseEdit.Citydatato = new SelectList(objModelTravel.tTravelcitydata().ToList(), "TravelCityName", "TravelCityName", objobjMExpense[0].PlaceTo.ToString());
                objMExpenseEdit.TravelModedata = new SelectList(objModelTravel.tTravelModedata().ToList(), "TravelModeId", "TravelModeName", objobjMExpense[0].TravelModeId.ToString());
                //objMExpenseEdit.HsncodeList = new SelectList(objModelTravel.GetHsnList().ToList(), "TravelHsnid", "TravelHsnCode", objobjMExpense[0].TravelHsnid.ToString());
                objMExpenseEdit.HsncodeList = new SelectList(objModelTravel.HsnCodeList(objobjMExpense[0].SubCategoryId).ToList(), "hsnid", "hsncode", Convert.ToString(objobjMExpense[0].TravelHsnid));

                objMExpenseEdit.TravelHsnDesc = objobjMExpense[0].TravelHsnDesc.ToString();
                //Ramya Added
                objMExpenseEdit.RCMFlag = objobjMExpense[0].RCMFlag.ToString();
                objMExpenseEdit.TravelClassdata = new SelectList(objModelTravel.tTravelClassdatadata(objobjMExpense[0].TravelModeId).ToList(), "TravelClassId", "TravelClassName", objobjMExpense[0].TravelClassId.ToString());
                Session["Ecfamountvaltmeditt"] = Convert.ToDecimal(Session["Ecfamountvaltm"].ToString()) + Convert.ToDecimal(objobjMExpense[0].Amount.ToString());
                RaiserDetail();
                ViewBag.EOW_EmployeeeExpenseheader = objMExpenseEdit;
                ViewBag.EOW_Employeeheader = obj;

                return PartialView(objMExpenseEdit);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return PartialView();
            }
        }


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
                    objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginUserGid().ToString()).ToList();
                    if (obj.ecfremark != null)
                    {
                        ViewBag.ecfrmarks = obj.ecfremark;
                    }

                    EOW_EmployeeeExpense eowemp = new EOW_EmployeeeExpense();
                    if (Session["loginraisemode"] != null)
                    {
                        obj.raisermodeId = "P";
                        objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginProxyUserGid().ToString()).ToList();
                        obj.Grade = objmaindetail[0].Grade.ToString();
                        obj.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
                        obj.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
                    }
                    else
                    {
                        obj.raisermodeId = "S";//"C OR S OR P" 
                        objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginUserGid().ToString()).ToList();
                        obj.Grade = objmaindetail[0].Grade.ToString();
                        obj.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
                        obj.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
                    }

                    Session["Ecfamountvalmain"] = obj.ECF_Amount;
                    obj.Raiser_Modedata = new SelectList(objModelTravel.GetRaiserMode().ToList(), "raisermodeId", "raisermodeName", obj.raisermodeId);
                    clmmonth = TravelClaimModel.ClaimPeriodFrom;
                    obj.ClaimMonth = "";      //locals.getconverttomonthtodate(TravelClaimModel.ClaimPeriodFrom);  selva commend 22-12-2020
                    obj.ECF_Date = DateTime.Now.ToString("dd-MM-yyyy");
                    obj.noofperson = "1";
                    message = objModelTravel.InsertEmployeeeBasic(obj, objCmnFunctions.GetLoginUserGid().ToString(), "T");
                    Session["EcfGid"] = objModelTravel.selectecfgidBasic(obj, objCmnFunctions.GetLoginUserGid().ToString());

                    if (Session["EcfGid"] != "" && Session["EcfGid"] != null)
                    {
                        EOW_Employeelst addemp = new EOW_Employeelst();
                        addemp.employeeGid = Convert.ToInt32(Session["SelfModeRaiseid"].ToString());
                        addemp.empbranch = objModelTravel.Getemployeebrach(Session["SelfModeRaiseid"].ToString());
                        addemp.empfc = Session["raiserfcc"].ToString();
                        addemp.empcc = Session["raiserccc"].ToString();
                        message = objModelTravel.insertempperson(addemp, Session["EcfGid"].ToString());
                    }

                }

                if (Convert.ToInt64(Session["EcfGid"].ToString()) > 0)
                    message = objModelTravel.InsertTravelModeCreateGst(TravelClaimModel, Session["EcfGid"].ToString(), "0", Session["SelfModeRaiseid"].ToString());
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
        [HttpGet]
        public PartialViewResult ECFDetails()
        {
            List<EOW_TravelClaim> lstnew = new List<EOW_TravelClaim>();

            if (Session["EcfGid"] != null)
            {
                eowemp = objModelTravel.GetEcfHeader(Session["EcfGid"].ToString());

                //eowemp.ECF_Amount = "1000";
                //eowemp.ecfremark = "testc";
                //eowemp.noofperson = "1";
            }
            ViewBag.EOW_Employeeheader = eowemp;
            //lstnew = objModelTravel.GetTravelModedata(Session["EcfGid"].ToString(), Session["invoiceGid"].ToString(), "T").ToList();
            return PartialView();
        }

        [HttpGet]
        public PartialViewResult TravelModeDetail()
        {
            List<EOW_TravelClaim> lstnew = new List<EOW_TravelClaim>();
            //lstnew = objModelTravel.GetTravelModedata(Session["EcfGid"].ToString(), Session["invoiceGid"].ToString(), "T").ToList();
            return PartialView();
        }

        [HttpPost]
        public JsonResult SetInvoiceTravelDetailGST(string EcfId, string InvId, string ProviderLocation, string GstinVendor, string Suppliergid, string InvNo,
            string InvDate, string Amount, string Desc, string WOTaxAmount, string IsVerify, string IsRemoved, string GstCharged,
            string ReceiverLocation, string GstinFiccl, string ServiceMonth, string ExpTravelID, string SunCost, string AdjustAmount, string Cygnet_gid)
        {
            try
            {
                DataTable dt = new DataTable();
                string Data1 = "", Data2 = "", Data3 = "";
                //   DataSet ds = db.SetInvoiceTravelDetailsForGST(Session["EcfGid"].ToString(), InvId, ProviderLocation, GstinVendor, Suppliergid, InvNo,
                //InvDate, Amount, Desc, WOTaxAmount, IsVerify, IsRemoved, GstCharged, ReceiverLocation, GstinFiccl, ServiceMonth, objCmnFunctions.GetLoginUserGid().ToString(),
                //ExpTravelID.ToString(), SunCost, AdjustAmount);
                DataSet ds = db.SetInvoiceTravelDetailsForGST(Session["EcfGid"].ToString(), InvId, ProviderLocation, GstinVendor, Suppliergid, InvNo,
       InvDate, Amount, Desc, WOTaxAmount, IsVerify, IsRemoved, GstCharged, ReceiverLocation, GstinFiccl, ServiceMonth, objCmnFunctions.GetLoginUserGid().ToString(),
       ExpTravelID.ToString(), SunCost, AdjustAmount, Cygnet_gid);


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

        [HttpGet]
        public PartialViewResult EmployeeList()
        {
            //List<EOW_TravelClaim> lstnew = new List<EOW_TravelClaim>();
            //lstnew = objModelTravel.GetTravelModedata(Session["EcfGid"].ToString(), Session["invoiceGid"].ToString(), "T").ToList();
            return PartialView();
        }

        public PartialViewResult CygnetSearch_Supplier()
        {
            List<CygnetSearchModel> obj = new List<CygnetSearchModel>();
            CygnetSearchModel invsearch = new CygnetSearchModel();
            invsearch.Cygnet_SupplierPanNo = "";
            obj = ObjCygnet.SelectInvoiceSearch(invsearch).ToList();
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
        //        invsearch.Suppliergid = suppliegid;
        //        invsearch.Cygnet_SupplierName = suppliegid;
        //        invsearch.Cygnet_Supplier_GSTNNo = GSNTNNo;
        //        invsearch.Cygnet_InvoiceNo = InvoiceNo;
        //        invsearch.Cygnet_InvoiceFromDate = FromDate;
        //        invsearch.Cygnet_InvoiceToDate = ToDate;
        //        objowner = ObjCygnet.SelectInvoiceSearch(invsearch).ToList();
        //        TempData["SearchItems"] = objowner;
        //    }
        //    return PartialView(objowner.ToList());
        //}

        //selva 22-Mar-2021 Modified
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
                // invsearch.Suppliergid = SupName;
                invsearch.Cygnet_SupplierName = SupName;
                invsearch.Cygnet_Supplier_GSTNNo = GSNTNNo;
                invsearch.Cygnet_InvoiceNo = InvoiceNo;
                invsearch.Cygnet_InvoiceFromDate = Fromdate;
                invsearch.Cygnet_InvoiceToDate = Todate;
                objowner = ObjCygnet.SelectInvoiceSearch(invsearch).ToList();
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

            Session["Panno"] = panno;
            Session["SupplierName"] = suppliername;
            Session["GSNTNNo"] = GSNTNNo;
            Session["InvoiceNo"] = InvoiceNo;
            Session["FromDate"] = FromDate;
            Session["ToDate"] = ToDate;

            DataTable dt = objModelTravel.GetCygnetSearchInvDetailsCount(CustomerInvDetail);
            if (dt.Rows.Count > 0)
            {
                Data1 = JsonConvert.SerializeObject(dt);

            }
            return Json(new { Data1 }, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public JsonResult GetCygnetSearchInvDetails(CygnetSearchModel CustomerInvDetail)
        {
            string Data1 = "";
            DataTable dt = objModelTravel.GetCygnetSearchInvDetails(CustomerInvDetail);
            if (dt.Rows.Count > 0)
            {
                Data1 = JsonConvert.SerializeObject(dt);
            }

            return Json(new { Data1 }, JsonRequestBehavior.AllowGet);
        }


        public void RaiserDetail()
        {
            if (Session["loginraisemode"] != null)
            {
                obj.raisermodeId = "P";
                objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginProxyUserGid().ToString()).ToList();
                obj.Grade = objmaindetail[0].Grade.ToString();
                obj.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
                obj.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
            }
            else
            {
                obj.raisermodeId = "S";//"C OR S OR P" 
                objmaindetail = objModelTravel.SelectEmployeeeheader(objCmnFunctions.GetLoginUserGid().ToString()).ToList();
                obj.Grade = objmaindetail[0].Grade.ToString();
                obj.Raiser_Code = objmaindetail[0].Raiser_Code.ToString();
                obj.Raiser_Name = objmaindetail[0].Raiser_Name.ToString();
            }

            obj.Raiser_Modedata = new SelectList(objModelTravel.GetRaiserMode().ToList(), "raisermodeId", "raisermodeName", obj.raisermodeId);
        }
        [HttpPost]
        public JsonResult TravelModeEdit(EOW_TravelClaim TravelClaimModel)
        {
            string message = "";
            try
            {
                if (Session["invoiceGid"] == null)
                    Session["invoiceGid"] = "0";

                message = objModelTravel.UpdateTravelModeCreateGST(TravelClaimModel, Session["EcfGid"].ToString(), Session["invoiceGid"].ToString(), Session["SelfModeRaiseid"].ToString(), Session["TravelModeactiverow"].ToString());
                ViewBag.SearchItems = null;
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("Error occurred..." + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        //Petyy cashe

        [HttpPost]
        public JsonResult SetPettyCashinvoiceGST(string EcfId, string InvId, string ProviderLocation, string GstinVendor, string Suppliergid, string InvNo,
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

        //12-03-2021 Selva Created for Supplier Name Auto Complete in Cygnet Search
        [HttpPost]
        public JsonResult AutoCompletecodes(string term)
        {
            string Err = "";
            List<CentraldataModel> objparenttax = new List<CentraldataModel>();
            objparenttax = objModelTravel.SelectSupplierSearchcode(term).ToList();
            return Json(objparenttax, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AutoCompletenames(string term)
        {
            string Err = "";
            List<CentraldataModel> objparenttax = new List<CentraldataModel>();
            objparenttax = objModelTravel.SelectSupplierSearchname(term).ToList();
            return Json(objparenttax, JsonRequestBehavior.AllowGet);
        }
    }
}
