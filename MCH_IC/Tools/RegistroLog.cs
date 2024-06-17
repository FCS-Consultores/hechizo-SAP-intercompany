using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intercompany.tools
{
   public class RegistroLog
    {
        public int Graba(string strLog)
        {
            string strDestinoLog = Directory.GetCurrentDirectory() + @"\Log\"; //System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"\Log\"; 


            if (!Directory.Exists(strDestinoLog))
            {
                Directory.CreateDirectory(strDestinoLog);

            }

            string strNombreArchivoLog = "Log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            try
            {
                StreamWriter sw = new StreamWriter(strDestinoLog + strNombreArchivoLog, true, System.Text.Encoding.UTF8);
                sw.WriteLine(strLog);
                sw.Close();
            }
            catch (Exception)
            {
                //MessageBox.Show("Exception: " + ex1.Message);
            }
            return 0;
        }

        public int GrabaProc(string strLog)
        {
            string strDestinoLog = Path.GetDirectoryName(Directory.GetCurrentDirectory() + @"\LogProcesos"); //Directory.GetCurrentDirectory() + @"\Log\"; //System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"\Log\"; 

            if (!Directory.Exists(strDestinoLog))
            {
                Directory.CreateDirectory(strDestinoLog);

            }

            string strNombreArchivoLog = "LogProc" + ".txt";
            try
            {
                StreamWriter sw = new StreamWriter(strDestinoLog + strNombreArchivoLog, true, System.Text.Encoding.UTF8);
                sw.WriteLine(strLog);
                sw.Close();
            }
            catch (Exception)
            {
                //MessageBox.Show("Exception: " + ex1.Message);
            }
            return 0;
        }
    }
}
