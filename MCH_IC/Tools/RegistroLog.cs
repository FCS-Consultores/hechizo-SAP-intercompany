using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inter_Hechiz.tools
{
   public class RegistroLog
    {
        public void Graba(string strLog)
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string logPath = Path.Combine(basePath, "Log");

                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

                string file = Path.Combine(
                    logPath,
                    $"Log_{DateTime.Now:yyyyMMdd}.txt"
                );

                File.AppendAllText(file, strLog + Environment.NewLine, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                // Mínimo deja rastro
                File.AppendAllText(
                    @"C:\Temp\log_fatal.txt",
                    DateTime.Now + " - " + ex + Environment.NewLine
                );
            }
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

        public void GrabaJson(string strLog)
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string logPath = Path.Combine(basePath, "Log");

                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

                string file = Path.Combine(
                    logPath,
                    $"LogJson_{DateTime.Now:yyyyMMdd}.txt"
                );

                File.AppendAllText(file, strLog + Environment.NewLine, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                // Mínimo deja rastro
                File.AppendAllText(
                    @"C:\Temp\log_fatal.txt",
                    DateTime.Now + " - " + ex + Environment.NewLine
                );
            }
        }

    }
}
