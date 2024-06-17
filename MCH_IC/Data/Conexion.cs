using Intercompany.tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Intercompany.Data
{
    public class Conexion
    {
        public static string ORIGEN=ori();
        public static string acero=aceros();
        public static string amd= amd();
        public static string boquete = boquete();
        public static string calypso = calypso();
        public static string canaima = canaima();
        public static string constru = constru();
        public static string eco = eco();
        public static string farmar = farmar();
        public static string ideo = ideo();
        public static string iverco = iverco();
        public static string panco = panco();
        public static string pgjp = pgjp();
        public static string world = world();
        public static string zhej = zhej();
      

        public static string conexionhana()
        {
            string connectionString = "";
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error

            try
            {

                IniFile IniFil;
                // IniFil = new IniFile(Application.StartupPath + "/config.ini"); //"Application.StartupPath" (Este se usan con el using Forms) //el archivo config.ini queda alojado en el la carpeta bin del publicado
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");

                string stDNS = IniFil.LeerINI("ConexionHanaProd", "DSN");
                string stSERVER = IniFil.LeerINI("ConexionHanaProd", "SERVERNODE");
                string stUSUARIO = IniFil.LeerINI("ConexionHanaProd", "UID");
                string stPASSWORD = IniFil.LeerINI("ConexionHanaProd", "PWD");
                string stDB = IniFil.LeerINI("ConexionHanaProd", "DATABASENAME");

                return connectionString = "DSN=" + stDNS + "; SERVERNODE=" + stSERVER + ";  UID=" + stUSUARIO + "; PWD=" + stPASSWORD + "; DATABASENAME=" + stDB + "";
                //return connectionString =  "SERVERNODE=" + stSERVER + ";  UID=" + stUSUARIO + "; PWD = " + stPASSWORD ; 
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba("error inicio coneccion " + ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return connectionString = "";
            }



        }
        public static string ori()
        {
             RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error

            try
            {

                IniFile IniFil;
                // IniFil = new IniFile(Application.StartupPath + "/config.ini"); //"Application.StartupPath" (Este se usan con el using Forms) //el archivo config.ini queda alojado en el la carpeta bin del publicado
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string ORIGEN = IniFil.LeerINI("ConexionHanaProd", "ORIGEN");               
                return ORIGEN;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }



        }
        public static string aceros()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string aceros = IniFil.LeerINI("ConexionHanaProd", "aceros");
                return aceros;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        public static string amd()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string amd = IniFil.LeerINI("ConexionHanaProd", "amd");
                return amd;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        public static string boquete()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string boquete = IniFil.LeerINI("ConexionHanaProd", "boquete");
                return boquete;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        public static string calypso()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string calypso = IniFil.LeerINI("ConexionHanaProd", "calypso");
                return calypso;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        public static string canaima()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string canaima = IniFil.LeerINI("ConexionHanaProd", "canaima");
                return canaima;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        public static string constru()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string constru = IniFil.LeerINI("ConexionHanaProd", "constru");
                return constru;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        public static string eco()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string eco = IniFil.LeerINI("ConexionHanaProd", "eco");
                return eco;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        public static string farmar()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string farmar = IniFil.LeerINI("ConexionHanaProd", "farmar");
                return farmar;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        public static string ideo()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string ideo = IniFil.LeerINI("ConexionHanaProd", "ideo");
                return ideo;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        public static string iverco()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string iverco = IniFil.LeerINI("ConexionHanaProd", "iverco");
                return iverco;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        public static string panco()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string panco = IniFil.LeerINI("ConexionHanaProd", "panco");
                return panco;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        public static string pgjp()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string pgjp = IniFil.LeerINI("ConexionHanaProd", "pgjp");
                return pgjp;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        public static string world()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string world = IniFil.LeerINI("ConexionHanaProd", "world");
                return world;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        public static string zhej()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string zhej = IniFil.LeerINI("ConexionHanaProd", "zhej");
                return zhej;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        public static string strCon = conexionhana();
     }
}
