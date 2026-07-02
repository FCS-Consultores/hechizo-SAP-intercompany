using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbobsCOM;
using System.IO;
using Inter_Hechiz.Tools;
using Inter_Hechiz.tools;

namespace MCH_IC.Tools
{
    public class ConexionDiapi
    {
        public Company myCompany = null;
        private int error;

        public bool Open()
        {
            RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error

            try
            {
                IniFile IniFile;
                IniFile = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");

                string stSERVER = IniFile.LeerINI("ConexionSap", "Server");
                //string stDB = IniFile.LeerINI("ConexionSap", "CompanyDB");
                string stUSUARIO = IniFile.LeerINI("ConexionSap", "UserName");
                string stPASSWORD = IniFile.LeerINI("ConexionSap", "Password");
                string stDBUSUARIO = IniFile.LeerINI("ConexionSap", "DbUserName");
                string stDBPASSWORD = IniFile.LeerINI("ConexionSap", "DbPassword");


                //antes de quitar el dispatchment
                string stDB = Diapiattachment.bdpublica; 
                //string stDB = Intercompany.bdpublica;
                bool respuesta = false;
                //MessageBox.Show("Intentando conectar a SAP");
                myCompany = new Company();

                myCompany.UseTrusted = false;
                myCompany.DbServerType = BoDataServerTypes.dst_HANADB;
                myCompany.Server = stSERVER; //"HANA-23.erpcloudweb.com:30013"; NDB@172.1.1.170:30015
                myCompany.CompanyDB = stDB;  // "10025_COMEDORES";
                myCompany.UserName = stUSUARIO; // "manager";
                myCompany.Password = stPASSWORD; // "B1Admin@1";
               // myCompany.DbUserName = stDBUSUARIO; // "System";
               // myCompany.DbPassword = stDBPASSWORD; // "Fcs2022!!!";
                myCompany.language = BoSuppLangs.ln_Spanish_La;
               // myCompany.SLDServer = "172.1.1.176:40000";
                //myCompany.LicenseServer = "172.1.1.164:40000";


                error = myCompany.Connect();

                if (error == 0)
                {

                    respuesta = true;
                    // MessageBox.Show("Conexion exitosa");

                }
                else
                {

                    objRegistraLog.Graba("-------------------------------------ERROR_CONEXION SAP-------------------------------------------------");
                    string msgErr = $"Conexion sin exito user {stUSUARIO}, pass {stPASSWORD}: " + myCompany.GetLastErrorDescription();
                    objRegistraLog.Graba(msgErr + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                    return false;

                }

                return respuesta;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba("-------------------------------------ERROR_METODO_CONEXION SAP-------------------------------------------------");
                objRegistraLog.Graba("Error al momento de conectar con SAP: " + " /" + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                return false;

            }
        }
    }
}
