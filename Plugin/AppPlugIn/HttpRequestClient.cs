using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace YY.U9.Cust.LI.AppPlugIn
{
    class HttpRequestClient
    {
        /// <summary>
        /// http的Post请求
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="data"></param>
        public static string HttpPost(string requestUrl, string data, string token, string transid)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "POST";
            request.ContentType = "application/xml";
            //request.Headers.Add("token", token);
            //request.Headers.Add("transid", transid);
            request.UseDefaultCredentials = true;
            request.ServicePoint.Expect100Continue = false;
            request.Timeout = 1000 * 60 * 2;
            request.KeepAlive = true;
            byte[] param = Encoding.UTF8.GetBytes(data.Trim());
            request.ContentLength = param.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(param, 0, param.Length);
            writer.Close();
            HttpWebResponse response;
            string responseText = string.Empty;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }
            StreamReader myreader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            responseText = myreader.ReadToEnd();
            myreader.Close();

            return responseText;
            //using (HttpClient client = new HttpClient())
            //{
            //    client.DefaultRequestHeaders.Add("token", token);
            //    client.DefaultRequestHeaders.Add("transid", transid);
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    //var content = ;
            //    HttpResponseMessage response = await client.PostAsync(requestUrl, new StringContent(data, Encoding.UTF8, "application/json"));
            //    string rdata = response.Content.ReadAsStringAsync().Result;
            //    return rdata;
            //}
        }

        /// <summary>
        /// http的Get请求
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="data"></param>
        public static string HttpGet(string requestUrl, string data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "GET";
            request.ContentType = "application/json; charset=utf-8";
            byte[] param = Encoding.UTF8.GetBytes(data.Trim());
            request.ContentLength = param.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(param, 0, param.Length);
            writer.Close();
            HttpWebResponse response;
            string responseText = string.Empty;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }
            StreamReader myreader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            responseText = myreader.ReadToEnd();
            myreader.Close();

            return responseText;
        }

        /// <summary>
        /// HTTP的Get请求
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string HttpGet(string requestUrl, string data, string method)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
            request.Method = "GET";
            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = requestBytes.Length;
            request.Timeout = 6000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string ret = reader.ReadToEnd();
                reader.Close();
                return ret;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

 

    }
}
