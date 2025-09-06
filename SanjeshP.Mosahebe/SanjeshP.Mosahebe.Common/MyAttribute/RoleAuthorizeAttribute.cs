using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Common.MyAttribute
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.HttpContext.Response.Redirect("/Login/Login");
            }

            if (context.HttpContext.User.FindFirst("RoleID").Value != "ADMIN")
            {
                context.HttpContext.Response.Redirect("/Login/Login");
            }
        }
    }
}
