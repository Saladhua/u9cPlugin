using Api.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ApiHelper
    {
        #region Private fields
        private static volatile ApiHelper _instance = null;
        private static readonly object lockObject = new object();
        /// <summary>
        /// 基础URL
        /// </summary>
        //const string baseUrl = "http://10.8.100.51:8888/v2/platform/get/";
        const string baseUrl = "http://10.8.100.204:8888/v2/platform/get/";


        #endregion

        #region Instance
        /// <summary>
        /// 获取单例对象
        /// </summary>
        /// <returns></returns>
        public static ApiHelper Instance()
        {
            if (_instance == null)
            {
                lock (lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new ApiHelper();
                    }
                }
            }
            return _instance;
        }
        private ApiHelper() { }
        #endregion

        #region 获取AccessToken
        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="corpid"></param>
        /// <param name="corpsecret"></param>
        /// <returns></returns>
        public JObject GetAccessToken(string corpid, string corpsecret, string transid)
        {
            string url = string.Format("{0}token?appid={1}&appsecret={2}&transid={3}", baseUrl, corpid, corpsecret, transid);
            var access_token = string.Empty;
            string result = HttpHelper.HttpGet.GetJsonResult<GetAccessTokenResult>(url);
            //解析josn  
            JObject jo = (JObject)JsonConvert.DeserializeObject(result);
            return jo;
        }

        #endregion
    }
}
