using Intercompany;
using Inter_Hechiz.Data;
using Inter_Hechiz.tools;
using Newtonsoft.Json.Linq;
using SAPbouiCOM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Linq;

using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
//using static Inter_Hechiz.Data.Conexion;
using static System.Net.Mime.MediaTypeNames;

namespace Inter_Hechiz.Tools
{
    public class Diapiattachment
    {
        static RegistroLog objRegistraLog = new RegistroLog();
       
        static string RR = "";
        public static string RUTAFINAL;
        public static string rutaoo;
        public static string bdpublica;
        private static CookieCollection cookieData = new CookieCollection();
        static Marketing ObjSap = new Marketing();
        Respuesta R = new Respuesta();
        private static Marketing marketing;
        public static void CrearAttach(  string bddestino,string tablafinal,String RUTTTA, string rutaorigen,string nomarchi,string ext,int docentry)
        //int entrydoc, string docuorigen, string bdorigen, string bddestino)
        {
            Respuesta R = new Respuesta();
            int resul = 0;
            
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    //string query = $"SELECT \"DocEntry\" FROM \"{bddestino}\".\"OATC\" WHERE \"DocEntry\" = '{entrydoc}'";
                    //OdbcCommand cmd = new OdbcCommand(query, conn);
                    //conn.Open();
                    RUTAFINAL = RUTTTA + "\\" + nomarchi + "." + ext;
                    rutaoo = rutaorigen + "\\" + nomarchi + "." + ext;
                    bdpublica = bddestino;
                    gestorarchivos.MoverArchivo(nomarchi, rutaorigen, RUTTTA, ext);
                    //if (gestorarchivos.paso == 1)
                    //{
                    //    bdpublica = bddestino;
                    //    ConexionDiapi SAP = new ConexionDiapi();
                    //    SAP.Open();                 
                    //    {
                    //        if (SAP.myCompany.Connected)
                    //        {                                

                    //            SAPbobsCOM.Attachments2 oATT = (SAPbobsCOM.Attachments2)SAP.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2);
                             
                    //            string ruta = RUTAFINAL.ToString();

                    //            oATT.Lines.Add();
                    //            oATT.Lines.FileName = Path.GetFileNameWithoutExtension(ruta);
                    //            oATT.Lines.FileExtension = Path.GetExtension(ruta).TrimStart('.');
                    //            oATT.Lines.SourcePath = Path.GetDirectoryName(ruta);
                    //            oATT.Lines.Override = SAPbobsCOM.BoYesNoEnum.tYES;

                    //            // Agregar el attachment
                    //            if (oATT.Add() == 0)
                    //            {
                    //                string newObjectKey;
                    //                SAP.myCompany.GetNewObjectCode(out newObjectKey);
                    //                int iAttEntry = int.Parse(newObjectKey);
                    //                if (conecta_service_layer(bddestino, "fcscr-cloud\\consult07", "H73M03!!@", "https://hana-mcholding:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                    //                {
                    //                    R.Codigo = -2;
                    //                    R.Mensaje = RR;
                    //                    objRegistraLog.Graba("Sin conectar a Service Layer /Attach: " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                    //                                                                                                                                                        //return R;
                    //                }
                    //                Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                    //                JsonObject.Add(new JProperty("AttachmentEntry", iAttEntry));

                    //                var myjs = JsonObject.ToString();
                    //                if (tablafinal == "OPOR")
                    //                {
                    //                    resul = EnviaSap("PATCH", JsonObject, "https://hana-mcholding:50000/b1s/v1/PurchaseOrders(" + docentry + ")"); //
                    //                }
                    //                else if (tablafinal == "OPQT")
                    //                {
                    //                    resul = EnviaSap("PATCH", JsonObject, "https://hana-mcholding:50000/b1s/v1/PurchaseQuotations(" + docentry + ")"); //
                    //                }
                    //                else if (tablafinal == "OPRQ")
                    //                {
                    //                    resul = EnviaSap("PATCH", JsonObject, "https://hana-mcholding:50000/b1s/v1/PurchaseRequests(" + docentry + ")"); //
                    //                }
                    //                else if (tablafinal == "ODRF")
                    //                {
                    //                    resul = EnviaSap("PATCH", JsonObject, "https://hana-mcholding:50000/b1s/v1/Drafts(" + docentry + ")"); //
                    //                }
                    //                if (resul == 0) // Caso que inserto 
                    //                {

                    //                }
                    //                else
                    //                {
                    //                    objRegistraLog.Graba("Error CrearAttach: " + RR + " " + nomarchi + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                    //                    Inserterror(bddestino.Substring(6, 6), "Attach", Convert.ToString(nomarchi));
                    //                }
                    //                // Asignar a documento SAP en tabla que viene en parametro
                    //                //tablafinal
                    //                //oOrdenComp.AttachmentEntry = iAttEntry;
                    //                SAP.myCompany.Disconnect();

                    //            }
                    //            else
                    //            {
                    //                string msgErr = SAP.myCompany.GetLastErrorDescription();

                    //                //objRegistraLog.Graba("Error CrearOC: " + RR + " " + DocEntry + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                    //                //    Inserterror(bddestino.Substring(6, 6), "OPOR", Convert.ToString(DocEntry));
                    //            }
                    //            SAP.myCompany.Disconnect();


                    //        }
                    //        else
                    //        {
                    //            objRegistraLog.Graba("No conecto DIAPI: " + " / Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                    //        }

                    //    }
                    //}
                    
                   
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al intentar insertar artículo: " + ex.Message + " / Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
            }
        }
        public class Respuesta
        {
            public int Codigo { get; set; }
            public string Mensaje { get; set; }
        }
        private static void Inserterror(string pais, string tip, string nomcod)
        {

            try
            {
                using (OdbcConnection conn9 = new OdbcConnection(Conexion.strCon))

                {
                    conn9.Open();
                    string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.BD_Hechizo}\".\"@SINCROERROR\""; //Query Encabezado
                    OdbcCommand cmand = new OdbcCommand(query, conn9);
                    OdbcDataReader dr1 = cmand.ExecuteReader();
                    int numero = 0;
                    while (dr1.Read())
                    {

                        numero = Convert.ToInt32(dr1["num"]) + 1;

                    }
                    dr1.Close();
                    string queryAct = $"INSERT INTO \"{Conexion.BD_Hechizo}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
                        "values(" + numero + "," + numero + ",'" + nomcod + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
                    OdbcCommand cmc1 = new OdbcCommand(queryAct, conn9);
                    cmc1.ExecuteNonQuery();
                    conn9.Close();
                }

                LiberaMemoria.FlushMemory();
            }
            catch (Exception ex)
            {

                objRegistraLog.Graba("Error Inserterror : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

            }


        }
        public static int conecta_service_layer(string CompanyDB, string UserName, string Password, string URL)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                JsonObject.Add(new JProperty("CompanyDB", CompanyDB));
                JsonObject.Add(new JProperty("UserName", UserName));
                JsonObject.Add(new JProperty("Password", Password));
                var request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;


                request.CookieContainer = new CookieContainer();
                request.Accept = "application/json;odata.metadata=minimal";
                request.ContentType = "application/json;odata=minimalmetadata;charset=utf-8";
                ServicePointManager.ServerCertificateValidationCallback += RemoteSSLTLSCertificateValidate;


                //if (cookieData.Count > 0)
                //{
                //    foreach (Cookie cookie in cookieData)
                //    {
                //        request.CookieContainer.Add(new Uri(URL), new Cookie(cookie.Name, cookie.Value));
                //    }
                //}
                var myjs = JsonObject.ToString();
                using (var requestStream = request.GetRequestStream())
                {
                    var writer = new StreamWriter(requestStream);
                    writer.Write(myjs);
                    writer.Close();
                }

                var result = new Dictionary<bool, string>();
                var webResponse = (HttpWebResponse)request.GetResponse();
                using (var response = new StreamReader(webResponse.GetResponseStream()))
                {
                    result.Add(true, response.ReadToEnd());
                }
                cookieData = webResponse.Cookies;

                return 0;
            }
            catch (Exception ex)
            {
                RR = ex.Message;
                return -1;
            }

        }



        public static int EnviaSap(string Metodo, JObject JsonObject, string URL) //, ref Marketing ObjSap, ref string RR
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = Metodo;
                request.ServicePoint.Expect100Continue = false;
                ServicePointManager.ServerCertificateValidationCallback += RemoteSSLTLSCertificateValidate;
                request.CookieContainer = new CookieContainer();

                foreach (Cookie cookie in cookieData)
                {
                    request.CookieContainer.Add(new Uri(URL), new Cookie(cookie.Name, cookie.Value));
                }

                if (JsonObject != null)
                {
                    var myjs = JsonObject.ToString();
                    using (var requestStream = request.GetRequestStream())
                    {
                        var writer = new StreamWriter(requestStream);
                        writer.Write(myjs);
                        writer.Close();
                    }
                }

                var result = new Dictionary<bool, string>();
                HttpWebResponse webResponse;
                string Obj = string.Empty;
                try
                {
                    webResponse = (HttpWebResponse)request.GetResponse();
                    Obj = "M";
                }
                catch (WebException mse)
                {
                    webResponse = mse.Response as HttpWebResponse;
                    Obj = "E";
                }
                using (var response = new StreamReader(webResponse.GetResponseStream()))
                {
                    result.Add(true, response.ReadToEnd());
                    string msn = result[true];
                    if (Obj == "M")
                    {
                        ObjSap = Newtonsoft.Json.JsonConvert.DeserializeObject<Marketing>(msn);
                        return 0;
                    }
                    else
                    {
                        var ObError = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(msn);
                        RR = ObError.error.message.value;
                        return -2;
                    }

                }
            }
            catch (Exception ec)
            {
                RR = ec.Message;
                return -1;
            }


        }
        //public class Respuesta1
        //{
        //    public int Codigo { get; set; }
        //    public string Mensaje { get; set; }
        //}

        private static bool RemoteSSLTLSCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors ssl)
        {
            return true;
        }

        public class Marketing
        {
            public int DocEntry { get; set; }
            public int DocNum { get; set; }
        }


        public class Root
        {
            public Error error { get; set; }
        }

        public class Error
        {
            public int code { get; set; }
            public Message message { get; set; }
        }

        public class Message
        {
            public string lang { get; set; }
            public string value { get; set; }
        }

    }
}
