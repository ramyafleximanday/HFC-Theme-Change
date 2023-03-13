using IEM.Areas.FLEXIBUY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IEM.Models;
namespace IEM.Areas.FLEXIBUY.Controllers
{
    public class CBFClosureChkController : Controller
    {
         private IrepositoryAn objRep;
        public CBFClosureChkController()
            : this(new CbfSumModel())
        {

        }
        public CBFClosureChkController(IrepositoryAn objModel)
        {
            objRep = objModel;
        }
        public ActionResult CbfClosureChkIndex()
        {
            CbfSumEntity objSumEntity;
            objSumEntity = objRep.GetCbfClosureChkSummary();
            Session["lscbfsmychecker"] = objSumEntity;
            return View(objSumEntity);
        }

    }
}
