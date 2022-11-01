using Api.Core;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 
    /// </summary>
    public class GetAccessTokenResult : JsonResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public int expires_in { get; set; }
    }
}