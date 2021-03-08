using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace TheatreCMS.Controllers
{
    public class TheatreAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // if not logged, it will work as normal and redirect to login
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                //logged and without permissions to access, redirects to custom action
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "UnauthorizedAccess" }));
            }
        }
    }
}