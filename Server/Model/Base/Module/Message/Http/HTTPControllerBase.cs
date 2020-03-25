using System.Net;
using Sining.Tools;

namespace Sining.Network
{
    public class HTTPControllerBase
    {
        protected HttpListenerContext Context { get; private set; }
        protected HttpListenerRequest Request => Context?.Request;
        protected HttpListenerResponse Response  => Context?.Response;
        private string _contentType;

        public void SetContext(HttpListenerContext context)
        {
            Context = context;
            _contentType = context.Request.ContentType;
        }

        private ActionResult CreateActionResult(int code, string response)
        {
            return ObjectPool<ActionResult>.Rent().Init(code, _contentType, response);
        }

        protected ActionResult NotFound()
        {
            return CreateActionResult(404, null);
        }

        protected ActionResult Error(string response)
        {
            return CreateActionResult(400, response);
        }

        protected ActionResult Success(string response = "")
        {
            return CreateActionResult(200, response);
        }
    }
}