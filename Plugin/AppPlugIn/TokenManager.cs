
using Api.Core.Caching;
using Newtonsoft.Json.Linq;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenManager
    {
        #region Private fields
        private static volatile TokenManager _instance = null;
        private static readonly object lockObject = new object();

        public const string appid = "e015ef3a23a842419a6a36373f9db9b8";
        public const string appsecret = "405a03d085a1406dbfb74ee941de2c6e";
        #endregion

        #region Instance
        /// <summary>
        /// 获取单例对象
        /// </summary>
        /// <returns></returns>
        public static TokenManager Instance()
        {
            if (_instance == null)
            {
                lock (lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new TokenManager();
                    }
                }
            }
            return _instance;
        }
        private TokenManager() { }
        #endregion

        #region Methods
        /// <summary>
        /// 获取access_token。
        /// 先从缓存获取，没有则从微信端获取。
        /// </summary>
        /// <param name="corpId"></param>
        /// <param name="corpSecret"></param>
        /// <returns></returns>
        public static string GetAccessToken(string corpId, string corpSecret, string transid)
        {
            string errMessage;
            return GetAccessToken(corpId, corpSecret, transid, out errMessage);
        }
        /// <summary>
        /// 获取access_token。
        /// 先从缓存获取，没有则从微信端获取。
        /// </summary>
        /// <param name="corpid"></param>
        /// <param name="corpsecret"></param>
        /// <param name="errmessage"></param>
        /// <returns></returns>
        public static string GetAccessToken(string corpid, string corpsecret, string transid, out string errmessage)
        {
            errmessage = string.Empty;
            string cacheKey = string.Format("ACCESSTOKEN:{0}:{1}", corpid, corpsecret);
            string access_token = CacheManager.Instance().Get<string>(cacheKey);

            if (!string.IsNullOrEmpty(access_token))
                return access_token;

            JObject result = ApiHelper.Instance().GetAccessToken(corpid, corpsecret, transid);

            string code = result["code"].ToString();
            string msg = result["msg"].ToString();
            string token = result["data"]["token"].ToString();

            if (!code.Equals("200"))
            {
                errmessage = string.Format("GetSuiteToken: errcode:{0},errmsg:{1}", code, msg);
                return access_token;
            }

            access_token = token;
            //缓存 access_token
            CacheManager.Instance().Set(cacheKey, access_token, 24);
            return access_token;
        }
        #endregion
    }
}
