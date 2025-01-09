using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.U9.Cust.LI.AppPlugIn
{
    class JHBassApiData
    {
        public static string GetApiTokenAndID()
        {
            string appId = "FSAID_131eb1b";

            string appSecret = "5b7a65fbc6d1449b9259c8c28762a107";

            string permanentCode = "6195FF51F55E298447A831072A423CF4";

            StringBuilder formData = new StringBuilder();
            formData.Append("{");
            formData.Append("\"appId\":\"" + appId + "\",");
            formData.Append("\"appSecret\":\"" + appSecret + "\",");
            formData.Append("\"permanentCode\":\"" + permanentCode + "\"");
            formData.Append("}");

            //发送格式
            StringBuilder formSendData = new StringBuilder();

            formSendData.Append(formData.ToString());

            //OA服务器地址
            string strURL = "https://open.fxiaoke.com/cgi/corpAccessToken/get/V2";

            string responseText = HttpRequestClient.HttpPostJson(strURL, formSendData.ToString(), "", "");

            return responseText;
        }

        /// <summary>
        /// 查询接口
        /// </summary>
        /// <param name="corpAccessToken">token</param>
        /// <param name="corpId">id</param>
        /// <param name="fieldValue">单号</param>
        /// <param name="dataObjectApiName">实体</param>
        /// <param name="fieldName">实体字段</param>
        /// <returns></returns>
        public static string GetDatas(string strurl, string corpAccessToken, string corpId, string fieldValue, string dataObjectApiName, string fieldName)
        {
            // 定义变量
            //string corpAccessToken = "71A87B4460C7F8349F3159F43CF668C6";
            //string corpId = "FSCID_34171732673C3A790ECC1A3DA6FDB978";
            string currentOpenUserId = "FSUID_87B9AB19BE3FC24C753DC22C88635BD8";
            int offset = 0;
            int limit = 10000;
            //string fieldName = "field_liqY4__c";
            //string fieldValue = "10SO2501020125";
            string operatorValue = "EQ";
            //string dataObjectApiName = "SalesOrderObj";

            StringBuilder stringBuilder = new StringBuilder();
            // 开始构建 JSON 字符串
            stringBuilder.Append("{");
            // 添加 corpAccessToken 字段
            stringBuilder.Append("\"corpAccessToken\":\"").Append(corpAccessToken).Append("\",");
            // 添加 corpId 字段
            stringBuilder.Append("\"corpId\":\"").Append(corpId).Append("\",");
            // 添加 currentOpenUserId 字段
            stringBuilder.Append("\"currentOpenUserId\":\"").Append(currentOpenUserId).Append("\",");
            // 开始添加 data 部分
            stringBuilder.Append("\"data\":{");
            // 开始添加 search_query_info 部分
            stringBuilder.Append("\"search_query_info\":{");
            // 添加 offset 字段
            stringBuilder.Append("\"offset\":").Append(offset).Append(",");
            // 添加 limit 字段
            stringBuilder.Append("\"limit\":").Append(limit).Append(",");
            // 开始添加 filters 部分
            stringBuilder.Append("\"filters\":[");
            // 开始添加 filters 中的对象
            stringBuilder.Append("{");
            // 添加 field_name 字段
            stringBuilder.Append("\"field_name\":\"").Append(fieldName).Append("\",");
            // 添加 field_values 字段
            stringBuilder.Append("\"field_values\":[\"").Append(fieldValue).Append("\"],");
            // 添加 operator 字段
            stringBuilder.Append("\"operator\":\"").Append(operatorValue).Append("\"");
            // 结束 filters 中的对象
            stringBuilder.Append("}");
            // 结束 filters 部分
            stringBuilder.Append("]},");
            // 添加 dataObjectApiName 字段
            stringBuilder.Append("\"dataObjectApiName\":\"").Append(dataObjectApiName).Append("\"");
            // 结束 search_query_info 部分
            // 结束 data 部分
            stringBuilder.Append("}");
            // 结束 JSON 字符串
            stringBuilder.Append("}");

            // 输出最终结果
            string result = stringBuilder.ToString();

            //发送格式
            StringBuilder formSendData = new StringBuilder();

            formSendData.Append(result.ToString());

            string responseText = HttpRequestClient.HttpPostJson(strurl, formSendData.ToString(), "", "");

            return responseText;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="strurl">地址</param>
        /// <param name="corpAccessToken">token</param>
        /// <param name="corpId">id</param>
        /// <param name="fieldValue">查到的单子的id</param>
        /// <param name="dataObjectApiName">实体</param>
        /// <returns></returns>
        public static string InvaDate(string strurl, string corpAccessToken, string corpId, string fieldValue, string dataObjectApiName)
        {
            var jsonObj = new
            {
                corpAccessToken = corpAccessToken, // 从1.接口获取
                corpId = corpId, // 从1.接口获取
                currentOpenUserId = "FSUID_961427B900C0F4B37E4E83362683C8E7",  // 默认值
                data = new
                {
                    object_data_id = fieldValue,
                    dataObjectApiName = dataObjectApiName
                }
            };

            // 将对象转换为字符串
            string jsonString = JsonConvert.SerializeObject(jsonObj);

            string responseText = HttpRequestClient.HttpPostJson(strurl, jsonString.ToString(), "", "");

            return responseText;
        }

    }
}
