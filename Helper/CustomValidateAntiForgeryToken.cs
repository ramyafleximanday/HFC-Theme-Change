using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using IEM.Common;

namespace IEM.Helper
{
    public class CustomValidateAntiForgeryToken : FilterAttribute, IAuthorizationFilter
    {
        ErrorLog objErrorLog = new ErrorLog();
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            string antiForgeryCookie = "";
            //try
            //{
            //if (filterContext.HttpContext.Request.HttpMethod == "POST" && (!filterContext.HttpContext.Request.Url.AbsolutePath.Equals("/iem")) &&
            //    (!filterContext.HttpContext.Request.Url.AbsolutePath.Equals("/")) && (!filterContext.HttpContext.Request.Url.AbsolutePath.Contains("/Loginpage/empLoginPage")) &&
            //    (!filterContext.HttpContext.Request.Url.AbsolutePath.Equals("/IEM/IEMCommon/Welcome")))
            //{
            Boolean Flag = true;
            string url = filterContext.HttpContext.Request.Url.AbsolutePath.ToUpper();
            if (url.Contains("/LOGINPAGE/EMPLOGINPAGE") || url.Equals("/"))
            {
                Flag = false;
            }
            if (System.Configuration.ConfigurationManager.AppSettings["CSRF"] == "Enable")
            {
                if (filterContext.HttpContext.Request.UrlReferrer != null)
                {
                    if (!url.Contains("FLEXISPEND") && !url.Contains("FLEXIBUY/REPORT") && !url.Contains("HELPER") && !url.Contains("POUCH"))
                    {
                        if (filterContext.HttpContext.Request.Form["Login"] == null)
                        {
                            if (filterContext.HttpContext.Request.HttpMethod != "GET")
                            {
                                antiForgeryCookie = filterContext.HttpContext.Request.Cookies[AntiForgeryConfig.CookieName].Name.ToString();
                                if (filterContext.HttpContext.Request.IsAjaxRequest())
                                {
                                    antiForgeryCookie = filterContext.HttpContext.Request.Cookies[AntiForgeryConfig.CookieName].Name.ToString();
                                    objErrorLog.WriteErrorLog(antiForgeryCookie, antiForgeryCookie);
                                    AntiForgery.Validate(filterContext.HttpContext.Request.Cookies[antiForgeryCookie].Value, filterContext.HttpContext.Request.Headers["__RequestVerificationToken"]);
                                }
                                else
                                {
                                    AntiForgery.Validate();
                                }

                            }
                        }
                    }
                }
                else if (Flag == true)
                {
                    filterContext.Result = new RedirectResult("~/Loginpage/empLoginPage");
                }
            }

            //catch (Exception ex)
            //{
            //    //System.IO.File.AppendAllText(System.Configuration.ConfigurationManager.AppSettings["Antiforgerylog"], "\n Catch:" + ex.Message + "\n Cookie:" + filterContext.HttpContext.Request.Cookies[antiForgeryCookie].Value + "\n Form:" + filterContext.HttpContext.Request.Headers["__RequestVerificationToken"] + "\n DATE&TIME" + DateTime.Now.ToString());
            //    objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            //    //throw new HttpAntiForgeryException("Anti forgery token cookie not found" + ex.Message);                  
            //    //throw new ArgumentNullException(ex.Message);
            //}
        }
    }

}