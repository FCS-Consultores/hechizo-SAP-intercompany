using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Int_MZ.tools
{
    public class RegistroProcesos
    {
        public static void LogProcesos(string messages)
        {
            try
            {
                string patch;
                patch = Path.GetDirectoryName(Directory.GetCurrentDirectory() + @"\LogProcesos");

                 //System.String exis = Directory.Exists(patch);

                if (!Directory.Exists(patch))
                {
                    if (File.Exists(patch + @"\LogProc.txt"))
                    {
                        StreamWriter writer;
                        writer = System.IO.File.AppendText(patch + @"\LogProc.txt");
                        writer.WriteLine(messages);
                        writer.Close();
                        writer.Dispose();
                    }
                    else
                    {
                        System.IO.FileStream Archivo;
                        Archivo = System.IO.File.Create(patch + @"\LogProc.txt");
                        Archivo.Close();
                        Archivo.Dispose();


                        StreamWriter writer;
                        writer = System.IO.File.AppendText(patch + @"\LogProc.txt");
                        writer.WriteLine(messages);
                        writer.Close();
                        writer.Dispose();
                    }
                }
                else
                {
                    Directory.CreateDirectory(patch);

                    System.IO.FileStream Archivo;
                    Archivo = System.IO.File.Create(patch + @"\LogProc.txt");
                    Archivo.Close();
                    Archivo.Dispose();


                    StreamWriter writer;
                    writer = System.IO.File.AppendText(patch + @"\LogProc.txt");
                    writer.WriteLine(messages);
                    writer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception ex)
            {
            }
        }

    }
}
