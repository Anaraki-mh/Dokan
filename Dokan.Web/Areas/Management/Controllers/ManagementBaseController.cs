using Dokan.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dokan.Web.Areas.Management.Controllers
{
    //[Authorize(Roles = "admin")]
    public class ManagementBaseController : Controller
    {
        public ManagementBaseController()
        {
            LayoutHelper.PrepareLayout();
        }
    }
}