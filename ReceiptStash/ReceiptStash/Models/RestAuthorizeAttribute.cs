using System;
using System.Web.Mvc;
using System.Web.WebPages;

namespace ReceiptStash
{
    public class RESTAuthorizeAttribute : AuthorizeAttribute
    {
        private const string _securityToken = "token"; // Name of the url parameter.

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Authorize(filterContext))
            {
                return;
            }
            HandleUnauthorizedRequest(filterContext);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }

        private bool Authorize (AuthorizationContext authorizationContext)
        {
            return true;
        }

        /* How to use RequestContext?
        private bool Authorize(AuthorizationContext actionContext)
        {
            try
            {
                HttpRequestBase request = actionContext.RequestContext.HttpContext.Request;
                string token = request.Params[_securityToken];
                return SecurityManager.IsTokenValid(token, CommonManager.GetIP(request), request.UserAgent);
            }
            catch (Exception)
            {
                return false;
            }
        }
        */
    }
}
