using System;
using System.IO;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 日志
    /// </summary>
    public class LogWriter
    {
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log"></param>
        public static void WriteLog(string log)
        {
            WriteLog(log, "log" + DateTime.Now.ToString("yyyyMMdd"));
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log"></param>
        /// <param name="fileName"></param>
        public static void WriteLog(string log, string fileName)
        {
            try
            {
                //日志模式
                string logMode = "1";   // Common.IsWriteLog();
                if (logMode == "0")  //不写日志
                {
                    return;
                }
            }
            catch (Exception ex) //写入日志
            { //默认写入日志 
                string mes = ex.Message;
            }
            try
            {
                String extName = fileName + ".txt";
                //string path = System.Environment.CurrentDirectory;
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\interfaceLog\";
                if (Directory.Exists(path) == false) //如果没有目录
                {
                    Directory.CreateDirectory(path);
                }
                extName = path + extName;
                if (File.Exists(extName) == false)
                {
                    FileStream theStream = new FileStream(extName, FileMode.Create);
                    theStream.Close();
                }
                FileInfo fi = new FileInfo(extName);
                long len = fi.Length;   //得到文件大
                if (len > 1024 * 1024 * 5) //如查文件大于5M
                {
                    fi.CopyTo(path + fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + "BAK.txt", true);
                    FileStream theStream = new FileStream(fileName, FileMode.Create);
                    theStream.Close();
                    fi.Delete();
                    theStream = new FileStream(extName, FileMode.Create); //重新创建新文件
                }
                StreamWriter sw = new StreamWriter(extName, true);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + log);
                sw.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("写入日志文件错误:"+ex.Message , "系统提示");
                Console.Write("Write log error," + ex.Message, "System Tips");

            }
        }

        /// <summary>
        /// 设置日志文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void ResetLogFile(string fileName)
        {
            try
            {
                String extName = fileName + ".txt";
                //string path = System.Environment.CurrentDirectory;
                string path = "C:\\interfaceLog\\";
                extName = path + extName;
                if (File.Exists(extName) == true)
                {
                    FileInfo fi = new FileInfo(extName);
                    fi.CopyTo(path + fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + "BAK.txt", true);
                    fi.Delete();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}