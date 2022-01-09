using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace WhiteBlackList.Web.MiddleWares
{
    /// <summary>
    /// App seviyesinde Check WhiteList kontrolü
    /// </summary>
    public class IPSafeListMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly IPList _ipList;

        public IPSafeListMiddleWare(RequestDelegate next, IOptions<IPList> ipList)
        {
            _next = next;
            _ipList = ipList.Value;
        }

        //WhiteList appsettings.json da tanımlıdır
        //Gelen request WhiteList içerisindemi kontrolü
        public async Task Invoke(HttpContext context)
        {
            var requestIpAddress = context.Connection.RemoteIpAddress;

            var isWhiteList = _ipList.WhiteList.Any(x => IPAddress.Parse(x).Equals(requestIpAddress));

            if (!isWhiteList)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            await _next(context);
        }
    }
}
