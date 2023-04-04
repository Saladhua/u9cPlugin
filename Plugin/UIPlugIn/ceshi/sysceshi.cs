using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace YY.U9.Cust.LI.UIPlugIn.ceshi
{
    public class sysceshi
    {

        #region 农行接口通讯方法
        private static Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(server), port);
            Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            tempSocket.Connect(ipe);
            if (tempSocket.Connected)
            {
                s = tempSocket;
            }
            return s;
        }
        /// <summary>
        /// 通讯发送报文
        /// </summary>
        /// <param name="server">服务ip 这里是连接农行客户端 所以ip默认应该是127.0.0.1</param>
        /// <param name="port">端口  这里是连接农行客户端，客户端登录后系统设置里会设置端口</param>
        /// <param name="DbAccNo">查询的账户银行账号</param>
        /// <param name="startTime">末笔时间戳 （文档有说明，通过这个查过的数据就不再过滤出来）</param>
        /// <param name="StarDate">查询的起始日期</param>
        /// <param name="EndDate">查询的截止日期</param>
        /// <returns></returns>
        private static string SocketSendReceive(string server, int port, string DbAccNo, string startTime, DateTime StarDate, DateTime EndDate)
        {
            string _head = "<ap><CCTransCode>CQRA10</CCTransCode><ProductID>ICC</ProductID><ChannelType>ERP</ChannelType><CorpNo></CorpNo><OpNo></OpNo><AuthNo></AuthNo><ReqSeqNo></ReqSeqNo><ReqDate>" + DateTime.Today.ToShortDateString() + "</ReqDate><ReqTime>" + DateTime.Now.ToShortTimeString() + "</ReqTime><Sign></Sign>";
            string request = _head + "<CCTransCode>CQRA10</CCTransCode><Corp><StartDate>" + StarDate.ToString("yyyyMMdd") + "</StartDate><EndDate>" + EndDate.ToString("yyyyMMdd") + "</EndDate></Corp><Channel><LastJrnNo></LastJrnNo></Channel><Cmp><DbAccNo>" + DbAccNo + "</DbAccNo><DbProv>38</DbProv><DbCur>01</DbCur><StartTime>" + startTime + "</StartTime></Cmp></ap>";
            Byte[] byl = Encoding.Default.GetBytes(request);
            string _len = "1" + byl.Length.ToString().PadRight(6, ' ');//根据文档说明 报文前面要有7位数字，第一位是加密否标示，后面6位是报文的长度
            Byte[] bytesSent = Encoding.Default.GetBytes(_len + request);
            Byte[] bytesReceived = new Byte[256];
            // Create a socket connection with the specified server and port.
            Socket s = ConnectSocket(server, port);
            if (s == null)
                throw new Exception("Error：通讯连接失败！请检查农行客户端是否登录！");

            // Send request to the server.
            s.Send(bytesSent, bytesSent.Length, 0);
            // Receive the server home page content.
            int bytes = 0;
            string page = "";
            // The following will block until te page is transmitted.
            do
            {
                bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                page = page + Encoding.Default.GetString(bytesReceived, 0, bytes);
            }
            while (bytes > 0);
            return page;
        }
        #endregion

    }
}
