using netApi.Common;
using System;
using System.Web.Http;

namespace NetEasyPay.Controllers
{
    public class UtilityController : ApiController
    {
        /// <summary>
        /// This method allows a call from the UX/UI to log messages of any of the pre-existing levels
        /// using the same mechanism used by this API itself.
        /// </summary>
        /// <param name="level">(string) "INFO", "WARNING", "ERROR", or "FATAL"</param>
        /// <param name="pageName">(string) Name of page that requested the message be logged</param>
        /// <param name="message">(string) Text to be sent to the logger</param>
        // TODO: [Authorize]
        [HttpPut]
        [Route("api/v{version:apiVersion}/Utility/LogMessage")]
        public void LogMessage(string level, string pageName, string message)
        {
            switch (level.ToUpper())
            {
                case "INFO":
                    {
                        FOPSLog.LogInfo("FOPS UX", pageName, message);
                        break;
                    }
                case "WARNING":
                    {
                        FOPSLog.LogWarning("FOPS UX", pageName, message, null);
                        break;
                    }
                case "ERROR":
                    {
                        FOPSLog.LogError("FOPS UX", pageName, message, null);
                        break;
                    }
                case "FATAL":
                    {
                        FOPSLog.LogFatal("FOPS UX", pageName, new Exception(message));
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
