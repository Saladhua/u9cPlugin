using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.U9.Cust.LI.UIPlugIn
{
    public class Common
    {
        /// <summary>
        /// 对象转decimal，非法返回0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal toDecimal(object value)
        {
            try
            {
                if (value == null || value.ToString().Trim() == "")
                    return 0;
                return Convert.ToDecimal(value);
            }
            catch (Exception ex)
            {
                string meses = ex.Message;
                return 0;
            }
        }
        /// <summary>
        /// 对象转decimal，非法返回0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long toLong(object value)
        {
            try
            {
                if (value == null || value.ToString().Trim() == "")
                    return 0;
                return Convert.ToInt64(value);
            }
            catch (Exception ex)
            {
                string mes = ex.Message;
                return 0;
            }
        }
    }
}
