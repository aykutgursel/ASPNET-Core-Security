using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using WhiteBlackList.Web.MiddleWares;

namespace WhiteBlackList.Web.Filters
{
    /// <summary>
    /// Method seviyesinde Check WhiteList kontrolü
    /// </summary>
    public class CheckWhiteList : ActionFilterAttribute
    {
        private readonly IPList _ipList;
        public CheckWhiteList(IOptions<IPList> ipList)
        {
            _ipList = ipList.Value;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var requestIpAddress = context.HttpContext.Connection.RemoteIpAddress;

            var isWhiteList = _ipList.WhiteList.Any(x => IPAddress.Parse(x).Equals(requestIpAddress));

            if (!isWhiteList)
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);

                return;
            }


            base.OnActionExecuting(context);
        }
    }
}
