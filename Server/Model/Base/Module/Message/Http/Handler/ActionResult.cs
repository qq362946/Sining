namespace Sining.Network
{
    public class ActionResult
    {
        public int StatusCode;
        public string Response;
        public string ContentType;

        public ActionResult Init(int statusCode, string contentType, string response)
        {
            StatusCode = statusCode;
            Response = response;
            ContentType = contentType;
            return this;
        }
    }
}