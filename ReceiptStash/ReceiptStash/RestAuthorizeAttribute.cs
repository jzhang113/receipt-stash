using System;
using System.Web.Mvc;

namespace ReceiptStash
{
    public class RESTAuthorizeAttribute : AuthorizeAttribute
    {
        private const string _securityToken = "token"; // Name of the url parameter.

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (true)
            {
                return;
            }
            HandleUnauthorizedRequest(filterContext);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}
