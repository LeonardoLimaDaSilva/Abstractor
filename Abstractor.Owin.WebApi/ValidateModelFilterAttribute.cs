using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Abstractor.Owin.WebApi
{
    /// <summary>
    ///     Validates the model state.
    /// </summary>
    public class ValidateModelFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        ///     Validates the model state. Sets the response as Bad Request if not valid.
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                                                          HttpStatusCode.BadRequest,
                                                          actionContext.ModelState);
        }
    }
}