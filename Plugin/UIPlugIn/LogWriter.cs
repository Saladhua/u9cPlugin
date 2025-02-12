using System;
using System.IO;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// ��־
    /// </summary>
    public class LogWriter
    {
        /// <summary>
        /// д��־
        /// </summary>
        /// <param name="log"></param>
        public static void WriteLog(string log)
        {
            WriteLog(log, "log" + DateTime.Now.ToString("yyyyMMdd"));
        }

        /// <summary>
        /// д��־
        /// </summary>
        /// <param name="log"></param>
        /// <param name="fileName"></param>
        public static void WriteLog(string log, string fileName)
        {
            try
            {
                //��־ģʽ
                string logMode = "1";   // Common.IsWriteLog();
                if (logMode == "0")  //��д��־
                {
                    return;
                }
            }
            catch (Exception ex) //д����־
            { //Ĭ��д����־ 
                string mes = ex.Message;
            }
            try
            {
                String extName = fileName + ".txt";
                //string path = System.Environment.CurrentDirectory;
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\interfaceLog\";
                if (Directory.Exists(path) == false) //���û��Ŀ¼
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
                long len = fi.Length;   //�õ��ļ���
                if (len > 1024 * 1024 * 5) //����ļ�����5M
                {
                    fi.CopyTo(path + fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + "BAK.txt", true);
                    FileStream theStream = new FileStream(fileName, FileMode.Create);
                    theStream.Close();
                    fi.Delete();
                    theStream = new FileStream(extName, FileMode.Create); //���´������ļ�
                }
                StreamWriter sw = new StreamWriter(extName, true);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + log);
                sw.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("д����־�ļ�����:"+ex.Message , "ϵͳ��ʾ");
                Console.Write("Write log error," + ex.Message, "System Tips");

            }
        }

        /// <summary>
        /// ������־�ļ�
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