using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using SAPbobsCOM;
using Newtonsoft.Json;
using Inter_Hechiz.tools;

namespace Inter_Hechiz.Data
{
    public class Conexion
    {
        public static string token_hechizo = CrearToken("BD_Hechizo");
        public static string token_sur = CrearToken("BD_sur");
        public static string token_omdo =CrearToken("BD_omdo");
        public static string token_nedn = CrearToken("BD_nedn");
        public static string token_hogarn = CrearToken("BD_hogar");
        public static string token_flores = CrearToken("BD_flores");
        public static string token_vital = CrearToken("BD_vital");
        public static string token_emde = CrearToken("BD_emde");
        public static string token_central = CrearToken("BD_central");
        public static string token_desampa = CrearToken("BD_desampa");
        public static string token_jjmax = CrearToken("BD_jjmax");
        public static string token_metropoli = CrearToken("BD_metropoli");

        public static string strCon = conexionhana();
        public static string BD_Hechizo = Consultar_Hechizo();
        public static string BD_sur = Consultar_sur();
        public static string BD_omdo = Consultar_omdo();
        public static string BD_nedn = Consultar_nedn();
        public static string BD_hogar = Consultar_hogar();
        public static string BD_flores = Consultar_flores();
        public static string BD_vital = Consultar_vital();
        public static string BD_emde = Consultar_emde();
        public static string BD_central = Consultar_central();
        public static string BD_desampa = Consultar_desampa();
        public static string BD_jjmax = Consultar_jjMax();
        public static string BD_metropoli = Consultar_metropoli();

        public static string conexionhana()
        {
            string connectionString = string.Empty;
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error

            try
            {

                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");

                string stDNS = IniFil.LeerINI("ConexionHanaProd", "DSN");
                string stSERVER = IniFil.LeerINI("ConexionHanaProd", "SERVERNODE");
                string stUSUARIO = IniFil.LeerINI("ConexionHanaProd", "UID");
                string stPASSWORD = IniFil.LeerINI("ConexionHanaProd", "PWD");
                //string stDB = IniFil.LeerINI("ConexionHanaProd", "DATABASENAME");

                return connectionString = "DRIVER=" + stDNS + "; SERVERNODE=" + stSERVER + ";  UID=" + stUSUARIO + "; PWD=" + stPASSWORD;// + "; DATABASENAME=" + stDB + "";
               
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba("error inicio coneccion " + ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return connectionString = "";
            }

        }

        private static string Consultar_Hechizo()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error

            try
            {

                IniFile IniFil;
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
        private static string Consultar_sur()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string amd = IniFil.LeerINI("ConexionHanaProd", "sur");
                return amd;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        private static string Consultar_omdo()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string boquete = IniFil.LeerINI("ConexionHanaProd", "omdo");
                return boquete;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        private static string Consultar_nedn()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string calypso = IniFil.LeerINI("ConexionHanaProd", "nedn");
                return calypso;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        private static string Consultar_hogar()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string canaima = IniFil.LeerINI("ConexionHanaProd", "hogar");
                return canaima;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        private static string Consultar_flores()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string constru = IniFil.LeerINI("ConexionHanaProd", "flores");
                return constru;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }

        private static string Consultar_vital()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string eco = IniFil.LeerINI("ConexionHanaProd", "vital");
                return eco;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        private static string Consultar_emde()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string torino = IniFil.LeerINI("ConexionHanaProd", "emde");
                return torino;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        private static string Consultar_central()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string farmar = IniFil.LeerINI("ConexionHanaProd", "central");
                return farmar;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }
        private static string Consultar_desampa()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string farmar = IniFil.LeerINI("ConexionHanaProd", "desampa");
                return farmar;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }

        private static string Consultar_jjMax()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string farmar = IniFil.LeerINI("ConexionHanaProd", "jjmax");
                return farmar;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }

        private static string Consultar_metropoli()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string farmar = IniFil.LeerINI("ConexionHanaProd", "metropoli");
                return farmar;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(ex + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return "";
            }
        }

        private static string CrearToken(string _DB)
        {

            string _user = string.Empty;
            string _pass = string.Empty;
            string _sToken = string.Empty;
            RegistroLog objRegistraLog = new RegistroLog();

            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");

                _user = IniFil.LeerINI("ConexionHanaProd", "UserSL") ?? throw new ArgumentNullException("*UserSL no configurado en el archivo config*");
                _pass = IniFil.LeerINI("ConexionHanaProd", "PassSL") ?? throw new ArgumentNullException("*PassSL no configurado en el archivo config*");

                switch (_DB)
                {
                    case "BD_Hechizo":
                        _DB = Consultar_Hechizo();
                        break;

                    case "BD_sur":
                        _DB = Consultar_sur();
                        break;

                    case "BD_omdo":
                        _DB = Consultar_omdo();
                        break;

                    case "BD_nedn":
                        _DB = Consultar_nedn();
                        break;

                    case "BD_hogar":
                        _DB = Consultar_hogar();
                        break;

                    case "BD_flores":
                        _DB = Consultar_flores();
                        break;

                    case "BD_vital":
                        _DB = Consultar_vital();
                        break;

                    case "BD_emde":
                        _DB = Consultar_emde();
                        break;

                    case "BD_central":
                        _DB = Consultar_central();
                        break;

                    case "BD_desampa":
                        _DB = Consultar_desampa();
                        break;

                    case "BD_jjmax":
                        _DB = Consultar_jjMax();
                        break;

                    case "BD_metropoli":
                        _DB = Consultar_metropoli();
                        break;

                    default:
                        _DB = string.Empty;
                        break;

                }

                var credenciales = new
                {
                    UserName = _user,
                    CompanyDB = _DB
                };

                string Creential = JsonConvert.SerializeObject(credenciales);

                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Creential}:{_pass}"));

                return _sToken = $"Basic {authToken}";

            }
            catch (Exception ex)
            {
                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Error al conseguir el token de la bd {_DB}, {ex.Message}");
                return string.Empty;
            }
        }

    }

    public static class ConexionSAP
    {
        public static Company Compania = new Company();
    }

}
