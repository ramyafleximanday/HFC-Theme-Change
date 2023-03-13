using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IEM.Areas.FLEXIBUY.Models;
using IEM.Models;

namespace IEM.Areas.FLEXIBUY.Controllers
{
    public class GrnSummaryForCWIPController : Controller
    {
        //
        // GET: /FLEXIBUY/GrnSummaryForCWIP/
        private Irepositorypr objModel;
        public GrnSummaryForCWIPController()
            : this(new fb_DataModelpr())
        {

        }
        public GrnSummaryForCWIPController(Irepositorypr objM)
        {
            objModel = objM;
        }

        [HttpGet]
        public ActionResult GrnSummaryForCWIP()
        {
            GRNInward objInward = new GRNInward();
            objInward.objInwardList = objModel.GetApprovedPo();
            return View(objInward);
        }
    }
}
