using Intercompany.tools;
using Intercompany.models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using Intercompany.Models;

namespace Intercompany.Data
{
    class inter
    {
        static RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
        private static CookieCollection cookieData = new CookieCollection();
        static Marketing ObjSap = new Marketing();
        static string RR = "";
        public static Respuesta SINCROERROR()
        {
            Respuesta R = new Respuesta();
            using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
            {
                try
                {
                    DateTime horaActual = DateTime.Now;
                    TimeSpan horaLimite = new TimeSpan(7, 0, 0);
                    // Comparar las horas
                    int comparacion = TimeSpan.Compare(horaActual.TimeOfDay, horaLimite);
                    if (comparacion < 0)
                    {
               
                        string trunca = $"TRUNCATE TABLE  \"{Conexion.ORIGEN}\".\"@SINCROERROR\"";
                        OdbcCommand db1 = new OdbcCommand(trunca, conn);
                        conn.Open();
                        db1.ExecuteNonQuery();
                        conn.Close();
                        objRegistraLog.Graba("ok Service Layer SINCROERROR : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error  
                    }
                    LiberaMemoria.FlushMemory();
                }
                catch (Exception)
                {

                    objRegistraLog.Graba("Error SINCROERROR : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error        
                }


            }
            return R;
        }
        public static Respuesta CrearFabricanteTODAS()
        {
            int resul = 0;
            string En_DN = "";
            string pais = "";
            string nomcod = string.Empty;
            DateTime horaActual = DateTime.Now;
            TimeSpan horaLimite = new TimeSpan(7, 0, 0);
            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    string queryEnCRSV = $"SELECT  \"FirmName\" FROM  \"{Conexion.ORIGEN}\".\"OMRC\" where \"FirmName\" not in (SELECT  \"FirmName\" FROM \"{Conexion.acero}\".\"OMRC\")"; //Query Encabezado
                    OdbcCommand CmD = new OdbcCommand(queryEnCRSV, conn); // llama al Query Encabezado con la conexion


                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        conn.Open();
                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(Conexion.acero, "fcspa-cloud\\Rali49", "Pr12!!0bn9", "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                            {
                                R.Codigo = -2;
                                R.Mensaje = RR;
                                objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                                return R;
                            }


                            while (dr.Read())
                            {

                                Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                                pais = "DISTRIBUIDORA";
                                nomcod = dr["FirmName"].ToString();
                                JsonObject.Add(new JProperty("ManufacturerName", nomcod));

                                var myjs = JsonObject.ToString();
                                resul = EnviaSap("POST", JsonObject, "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Manufacturers"); //

                                if (resul == 0) // Caso que inserto 
                                {

                                    objRegistraLog.GrabaProc("Insertado a Sap Satisfactoriamente el Documento : " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio

                                }
                                else
                                {
                                    objRegistraLog.Graba("Error insertando fabricanteTD: " + RR + " " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                                                                                                                                                         //RR = "";
                                                                                                                                                                         //BUSCO EL MAX DOCENTRY
                                    string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                    int numero = 0;
                                    while (dr1.Read())
                                    {

                                        numero = Convert.ToInt32(dr1["num"]) + 1;

                                    }
                                    dr1.Close();
                                    string tip = "OMRC";
                                    string PAIS = pais.ToString();
                                    string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
                                        "values(" + numero + "," + numero + ",'" + nomcod + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
                                    OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
                                    cmc1.ExecuteNonQuery();

                                }
                            }

                            conn.Close();
                            LiberaMemoria.FlushMemory();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;

                objRegistraLog.Graba("Error al intentar Insertar FabricanteTD : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

            }

            return R;
        }
        public static Respuesta CrearGrupoTODAS()
        {
            int resul = 0;
            string En_DN = "";
            string pais = "";
            string nomcod = string.Empty;
            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    string queryEnCRSV = $"SELECT  \"ItmsGrpNam\" FROM  \"{Conexion.ORIGEN}\".\"OITB\"where \"ItmsGrpNam\" not in (SELECT  \"ItmsGrpNam\" FROM \"{Conexion.DESTINO}\".\"OITB\")"; //Query Encabezado
                    OdbcCommand CmD = new OdbcCommand(queryEnCRSV, conn); // llama al Query Encabezado con la conexion

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(Conexion.DESTINO, "fcspa-cloud\\Rali49", "Pr12!!0bn9", "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                            {
                                R.Codigo = -2;
                                R.Mensaje = RR;
                                objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                                return R;
                            }


                            while (dr.Read())
                            {

                                Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                                pais = "DISTRIBUIDORA";
                                nomcod = dr["ItmsGrpNam"].ToString();
                                JsonObject.Add(new JProperty("GroupName", nomcod));

                                var myjs = JsonObject.ToString();
                                resul = EnviaSap("POST", JsonObject, "https://hana-002.fcspa-cloud.net:50000/b1s/v1/ItemGroups"); //

                                if (resul == 0) // Caso que inserto 
                                {

                                    objRegistraLog.GrabaProc("Insertado a Sap Satisfactoriamente el Documento: " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio

                                }
                                else
                                {
                                    objRegistraLog.Graba("Error insertando documento GrupoTD: " + RR + " " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                                                                                                                                                              //RR = "";
                                                                                                                                                                              //BUSCO EL MAX DOCENTRY
                                    string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                    int numero = 0;
                                    while (dr1.Read())
                                    {

                                        numero = Convert.ToInt32(dr1["num"]) + 1;

                                    }
                                    dr1.Close();
                                    string tip = "OITB";
                                    string PAIS = "DISTRIBUIDORA";
                                    string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
                                        "values(" + numero + "," + numero + ",'" + En_DN + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
                                    OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
                                    cmc1.ExecuteNonQuery();

                                }
                            }

                            conn.Close();
                            LiberaMemoria.FlushMemory();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;

                objRegistraLog.Graba("Error al intentar Insertar GrupoTD : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

            }

            return R;
        }

        public static Respuesta CrearArticulosTD()
        {
            int resul = 0;
            string nomgru = string.Empty;
            string nomcod = string.Empty;
            //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";
            //List<Task<int>> tasks = new List<Task<int>>();
            Respuesta R = new Respuesta();
            try
            {

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {

                    string queryEnCRSV = $"SELECT top 5  * FROM \"_SYS_BIC\".\"{Conexion.ORIGEN}.INTERCOMPANY/IC_ADD_ITEMSTD\""; //Query Encabezado
                    OdbcCommand CmD = new OdbcCommand(queryEnCRSV, conn); // llama al Query Encabezado con la conexion

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(Conexion.DESTINO, "fcspa-cloud\\Rali49", "Pr12!!0bn9", "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                            {
                                R.Codigo = -2;
                                R.Mensaje = RR;
                                objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                                return R;
                            }

                            while (dr.Read())
                            {
                                nomcod = dr["ItemCode"].ToString();
                                Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                                JsonObject = CrearObjetoJson(dr);
                                var myjs = JsonObject.ToString();
                                resul = EnviaSap("POST", JsonObject, "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Items");//
                                if (resul == 0) // Caso que inserto 
                                {

                                    objRegistraLog.GrabaProc("Insertado a Sap Satisfactoriamente el Documento: " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio

                                }
                                else
                                {
                                    objRegistraLog.Graba("Error insertando documento ArticulosTD: " + RR + " " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                                                                                                                                                                  //RR = "";
                                                                                                                                                                                  //BUSCO EL MAX DOCENTRY
                                    string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                    int numero = 0;
                                    while (dr1.Read())
                                    {

                                        numero = Convert.ToInt32(dr1["num"]) + 1;

                                    }
                                    dr1.Close();
                                    string tip = "OITM";
                                    string pais = "DISTRIBUIDORA";
                                    string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
                                        "values(" + numero + "," + numero + ",'" + nomcod + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
                                    OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
                                    cmc1.ExecuteNonQuery();


                                }
                            }



                            conn.Close();
                            LiberaMemoria.FlushMemory();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;

                objRegistraLog.Graba("Error al intentar Insertar ArticulosTD : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

            }

            return R;
        }
        public static Respuesta CrearAduanasTODAS()
        {
            int resul = 0;
            string nomgru = string.Empty;
            string nomcod = string.Empty;
            //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";
            //List<Task<int>> tasks = new List<Task<int>>();
            Respuesta R = new Respuesta();
            try
            {

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {

                    string queryEnCRSV = $"SELECT top 5  * FROM \"_SYS_BIC\".\"{Conexion.ORIGEN}.INTERCOMPANY/IC_ADD_ITEMSTD\""; //Query Encabezado
                    OdbcCommand CmD = new OdbcCommand(queryEnCRSV, conn); // llama al Query Encabezado con la conexion

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(Conexion.DESTINO, "fcspa-cloud\\Rali49", "Pr12!!0bn9", "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                            {
                                R.Codigo = -2;
                                R.Mensaje = RR;
                                objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                                return R;
                            }

                            while (dr.Read())
                            {
                                nomcod = dr["ItemCode"].ToString();
                                Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                                JsonObject = CrearObjetoJson(dr);
                                var myjs = JsonObject.ToString();
                                resul = EnviaSap("POST", JsonObject, "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Items");//
                                if (resul == 0) // Caso que inserto 
                                {

                                    objRegistraLog.GrabaProc("Insertado a Sap Satisfactoriamente el Documento: " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio

                                }
                                else
                                {
                                    objRegistraLog.Graba("Error insertando documento ArticulosTD: " + RR + " " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                                                                                                                                                                  //RR = "";
                                                                                                                                                                                  //BUSCO EL MAX DOCENTRY
                                    string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                    int numero = 0;
                                    while (dr1.Read())
                                    {

                                        numero = Convert.ToInt32(dr1["num"]) + 1;

                                    }
                                    dr1.Close();
                                    string tip = "OITM";
                                    string pais = "DISTRIBUIDORA";
                                    string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
                                        "values(" + numero + "," + numero + ",'" + nomcod + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
                                    OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
                                    cmc1.ExecuteNonQuery();


                                }
                            }



                            conn.Close();
                            LiberaMemoria.FlushMemory();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;

                objRegistraLog.Graba("Error al intentar Insertar ArticulosTD : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

            }

            return R;
        }
        public static Respuesta CrearFamiliaTODAS()
        {
            int resul = 0;
            string nomgru = string.Empty;
            string nomcod = string.Empty;
            //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";
            //List<Task<int>> tasks = new List<Task<int>>();
            Respuesta R = new Respuesta();
            try
            {

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {

                    string queryEnCRSV = $"SELECT top 5  * FROM \"_SYS_BIC\".\"{Conexion.DESTINO}.INTERCOMPANY/IC_ADD_ITEMSDT\""; //Query Encabezado
                    OdbcCommand CmD = new OdbcCommand(queryEnCRSV, conn); // llama al Query Encabezado con la conexion

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(Conexion.ORIGEN, "fcspa-cloud\\Rali49", "Pr12!!0bn9", "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                            {
                                R.Codigo = -2;
                                R.Mensaje = RR;
                                objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                                return R;
                            }

                            while (dr.Read())
                            {
                                nomcod = dr["ItemCode"].ToString();
                                Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                                JsonObject = CrearObjetoJson(dr);
                                var myjs = JsonObject.ToString();
                                resul = EnviaSap("POST", JsonObject, "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Items");//
                                if (resul == 0) // Caso que inserto 
                                {

                                    objRegistraLog.GrabaProc("Insertado a Sap Satisfactoriamente el Documento: " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio

                                }
                                else
                                {
                                    objRegistraLog.Graba("Error insertando documento ArticulosDT: " + RR + " " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                                                                                                                                                                  //RR = "";
                                                                                                                                                                                  //BUSCO EL MAX DOCENTRY
                                    string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                    int numero = 0;
                                    while (dr1.Read())
                                    {

                                        numero = Convert.ToInt32(dr1["num"]) + 1;

                                    }
                                    dr1.Close();
                                    string tip = "OITM";
                                    string pais = "COMERCIAL";
                                    string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
                                        "values(" + numero + "," + numero + ",'" + nomcod + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
                                    OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
                                    cmc1.ExecuteNonQuery();


                                }
                            }



                            conn.Close();
                            LiberaMemoria.FlushMemory();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;

                objRegistraLog.Graba("Error al intentar Insertar ArticulosDT : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

            }

            return R;
        }
        public static Respuesta convierteOVTA_OC()
        {
            int resul = 0;
            string En_DN = "";
            string pais = "";
            // string bdsucursal = "20006_MODENA_S.A.";
            // double precio = 0;
            //  int ser = 73;
            Respuesta R = new Respuesta();
            //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";
            //CREA ORDEN DE COMPRA DESDE LA ORDEN DE VENTA DE PRIZMA QUE CAE EN SAP VER VISTA IC_OVTA_A_OC_DT PARA LUEGO SER ENVIADA 
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    if (conecta_service_layer(Conexion.DESTINO, "fcspa-cloud\\Rali49", "Pr12!!0bn9", "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                    {
                        R.Codigo = -2;
                        R.Mensaje = RR;
                        objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

                        return R;
                    }

                    //using (OdbcConnection conn = new OdbcConnection(
                    //    //Conexion.strCon
                    //    connectionString
                    //    ))
                    {
                      
                        string queryEn = $"SELECT \"ItemCode\", \"DocEntry\",	 \"Quantity\",	 \"PriceBefDi\",	\"DiscPrcnt\"  FROM \"_SYS_BIC\".\"{Conexion.DESTINO}.INTERCOMPANY/IC_OVTA_A_OC_DT\"";
                      
                        OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion

                        conn.Open();

                        using (OdbcDataReader dr = CmD.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                List<DetallesOPORT> detallesFacturas = new List<DetallesOPORT>();
                                while (dr.Read())
                                {
                                    int DocEntry = (int)dr["DocEntry"];
                                    string ItemCode = (string)dr["ItemCode"];
                                    decimal Quantity = (decimal)dr["Quantity"];
                                    decimal PriceBefDi = (decimal)dr["PriceBefDi"];
                                    //decimal DiscPrcnt = (decimal)dr["DiscPrcnt"];

                                    detallesFacturas.Add(new DetallesOPORT
                                    {
                                        DocEntry = DocEntry,
                                        ItemCode = ItemCode,
                                        Quantity = Quantity,
                                        PriceBefDi = PriceBefDi,
                                        //DiscPrcnt = DiscPrcnt
                                    });
                                }
                                var detallesAgrupados = detallesFacturas
                                .GroupBy(detalle => detalle.DocEntry)
                                .Select(grupo => new { NumeroDocumento = grupo.Key, Detalles = grupo.ToList() })
                                .ToList();

                                foreach (var grupo in detallesAgrupados)
                                {
                                    string COD = "PL00030";
                                    Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                                    string DocDueDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                                    JsonObject.Add(new JProperty("CardCode",COD ));
                                   // JsonObject.Add(new JProperty("DocDueDate", DocDueDate));
                                    JsonObject.Add(new JProperty("Comments", "Orden Automatica Prizma"));
                                    JsonObject.Add(new JProperty("U_ENVIOSUC", "Y"));
                                    JsonObject.Add(new JProperty("U_sisori", "Sistema"));
                                    var list = new ArrayList();
                                    foreach (var detalle in grupo.Detalles)
                                    {
                                        Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
                                        JsonObjectRow.Add(new JProperty("ItemCode", detalle.ItemCode));
                                        JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
                                        JsonObjectRow.Add(new JProperty("UnitPrice", detalle.PriceBefDi));
                                        //JsonObjectRow.Add(new JProperty("DiscPrcnt", detalle.DiscPrcnt));
                                        list.Add(JsonObjectRow);
                                    }

                                    JsonObject.Add(new JProperty("DocumentLines", list));
                                    string data = JsonObject.ToString();
                                    resul = EnviaSap("POST", JsonObject, "https://hana-002.fcspa-cloud.net:50000/b1s/v1/PurchaseOrders");
                                    if (resul == 0) // Caso que inserto 
                                    {

                                        objRegistraLog.GrabaProc("Insertado a Sap Satisfactoriamente el Documento: " + grupo.NumeroDocumento + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                        try
                                        {
                                            //string query = $"UPDATE  \"{Conexion.DESTINO}\".OPOR('{grupo.NumeroDocumento}')"; //Query Encabezado
                                            string query = $"UPDATE  \"{Conexion.DESTINO}\".ORDR\" SET \"U_Enviado\" = '0' WHERE \"ItemCode\" =  '" + grupo.NumeroDocumento + "'";
                                            // string queryAct = $"UPDATE \"{BRETASV}\".\"OITM\" SET \"U_act\" = '" + vac + "' WHERE \"ItemCode\" =  '" + En_DN + "'"; //Query Actualiza Entry_fac
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    }
                                    else
                                    {
                                        objRegistraLog.Graba("Error convirtiendo OVTA a OC: " + RR + " " + grupo.NumeroDocumento + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                                                                                                                                                                         //RR = "";
                                                                                                                                                                                         //BUSCO EL MAX DOCENTRY
                                        string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                        OdbcDataReader dr1 = cmand.ExecuteReader();
                                        int numero = 0;
                                        string method = "ORDR PRIZMA";
                                        while (dr1.Read())
                                        {

                                            numero = Convert.ToInt32(dr1["num"]) + 1;

                                        }
                                        dr1.Close();
                                        string tip = "ORDR";
                                        // string PAIS = "Costa Rica";
                                        string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
                                                 "values(" + numero + "," + numero + ",'" + grupo.NumeroDocumento + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
                                        OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
                                        cmc1.ExecuteNonQuery();


                                    }
                                }
                            }


                            conn.Close();
                            LiberaMemoria.FlushMemory();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;

                objRegistraLog.Graba("Error al intentar Insertar orden convierte ovta_oc : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

            }

            return R;
        }
        public static Respuesta NuevaOporautDC()
        {
            int resul = 0;
            string En_DN = "";
            string pais = "Dist";
            //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";
            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    if (conecta_service_layer(Conexion.DESTINO, "fcspa-cloud\\Rali49", "Pr12!!0bn9", "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                    {
                        R.Codigo = -2;
                        R.Mensaje = RR;
                        objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                        return R;
                    }
                    
                    {
                        string queryEn = $"SELECT * FROM \"_SYS_BIC\".\"{Conexion.DESTINO}.INTERCOMPANY/IC_GET_ITEMS_REORDER\"";
                        //string crDet =$"SELECT \"LineNum\",\"ItemCode\",\"Quantity\",\"DiscountPercent\",\"ItemDescription\",\"Price\",\"TaxCode\",\"LineTotal\" FROM \"_SYS_BIC\".\"10028_COPPER_GROUP/SINCRONIZADOR_ORDR_CR_DET\"('PLACEHOLDER' = ('$$numdoc$$','{En_DN}'))";

                        OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion

                        conn.Open();

                        using (OdbcDataReader dr = CmD.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                List<DetallesOPORT> detallesFacturas = new List<DetallesOPORT>();
                                JObject JsonObject = new JObject();
                                string cod = "PL00030";

                                JsonObject.Add(new JProperty("CardCode", cod));
                                JsonObject.Add(new JProperty("Comments", "Orden Automatica "));
                                JsonObject.Add(new JProperty("U_ENVIOSUC", "Y"));//se van en automatico con el intercompany

                                var list = new ArrayList();
                                while (dr.Read())
                                {
                                    Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
                                    JsonObjectRow.Add(new JProperty("ItemCode", dr["ItemCode"]));
                                    JsonObjectRow.Add(new JProperty("Quantity", dr["Quantity"]));
                                    //JsonObjectRow.Add(new JProperty("UnitPrice", dr["PriceBefDi"]));
                                    //JsonObjectRow.Add(new JProperty("PriceSource", "Manual"));

                                    list.Add(JsonObjectRow);
                                }
                                JsonObject.Add(new JProperty("DocumentLines", list));
                                string data = JsonObject.ToString();
                                resul = EnviaSap("POST", JsonObject, "https://hana-002.fcspa-cloud.net:50000/b1s/v1/PurchaseOrders");
                                if (resul == 0) // Caso que inserto 
                                {

                                    objRegistraLog.GrabaProc("Insertado a Sap Satisfactoriamente el Documento: "  + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio

                                }
                                else
                                {
                                    objRegistraLog.Graba("Error insertando documento orden autDC: " + RR + " " + "Orden autDC" + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                                                                                                                                                                 //RR = "";
                                                                                                                                                                                 //BUSCO EL MAX DOCENTRY
                                    string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                    int numero = 0;
                                    string method = "OPOR";
                                    while (dr1.Read())
                                    {

                                        numero = Convert.ToInt32(dr1["num"]) + 1;

                                    }
                                    dr1.Close();
                                    string tip = "OPORDC";
                                    // string PAIS = "Costa Rica";
                                    string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
                                             "values(" + numero + "," + numero + ",'" + 0 + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
                                    OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
                                    cmc1.ExecuteNonQuery();


                                }
                            }

                            conn.Close();
                            LiberaMemoria.FlushMemory();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;

                objRegistraLog.Graba("Error al intentar Insertar orden  oporautDC: " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

            }

            return R;
        }
        public static Respuesta NuevaOrdenvtaDT()
        {
            int resul = 0;
            string En_DN = "";
            string pais = "";
            // string bdsucursal = "20006_MODENA_S.A.";
            // double precio = 0;
            //  int ser = 73;
            Respuesta R = new Respuesta();
            //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";



            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    if (conecta_service_layer(Conexion.ORIGEN, "fcspa-cloud\\Rali49", "Pr12!!0bn9", "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                    {
                        R.Codigo = -2;
                        R.Mensaje = RR;
                        objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

                        return R;
                    }

                    //using (OdbcConnection conn = new OdbcConnection(
                    //    //Conexion.strCon
                    //    connectionString
                    //    ))
                    {

                        string queryEn = $"SELECT * FROM \"_SYS_BIC\".\"{Conexion.DESTINO}.INTERCOMPANY/IC_GET_PURCHASE_ORDERSDT\"";
                        //string crDet =$"SELECT \"LineNum\",\"ItemCode\",\"Quantity\",\"DiscountPercent\",\"ItemDescription\",\"Price\",\"TaxCode\",\"LineTotal\" FROM \"_SYS_BIC\".\"10028_COPPER_GROUP/SINCRONIZADOR_ORDR_CR_DET\"('PLACEHOLDER' = ('$$numdoc$$','{En_DN}'))";

                        OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion

                        conn.Open();

                        using (OdbcDataReader dr = CmD.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                List<DetallesOPORT> detallesFacturas = new List<DetallesOPORT>();
                                while (dr.Read())
                                {
                                    int DocEntry = (int)dr["DocEntry"];
                                    string ItemCode = (string)dr["ItemCode"];
                                    decimal Quantity = (decimal)dr["Quantity"];
                                    decimal PriceBefDi = (decimal)dr["PriceBefDi"];
                                    decimal DiscPrcnt = (decimal)dr["DiscPrcnt"];

                                    detallesFacturas.Add(new DetallesOPORT
                                    {
                                        DocEntry = DocEntry,
                                        ItemCode = ItemCode,
                                        Quantity = Quantity,
                                        PriceBefDi = PriceBefDi,
                                        DiscPrcnt = DiscPrcnt
                                    });
                                }
                                var detallesAgrupados = detallesFacturas
                                .GroupBy(detalle => detalle.DocEntry)
                                .Select(grupo => new { NumeroDocumento = grupo.Key, Detalles = grupo.ToList() })
                                .ToList();

                                foreach (var grupo in detallesAgrupados)
                                {
                                    Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                                    string DocDueDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                                    JsonObject.Add(new JProperty("CardCode", "C15967"));
                                    JsonObject.Add(new JProperty("DocDueDate", DocDueDate));
                                    JsonObject.Add(new JProperty("Comments", "Orden Automatica"));
                                    var list = new ArrayList();
                                    foreach (var detalle in grupo.Detalles)
                                    {
                                        Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
                                        JsonObjectRow.Add(new JProperty("ItemCode", detalle.ItemCode));
                                        JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
                                        JsonObjectRow.Add(new JProperty("UnitPrice", detalle.PriceBefDi));
                                        JsonObjectRow.Add(new JProperty("DiscPrcnt", detalle.DiscPrcnt));
                                        list.Add(JsonObjectRow);
                                    }
                                    JsonObject.Add(new JProperty("DocumentLines", list));
                                    string data = JsonObject.ToString();
                                    resul = EnviaSap("POST", JsonObject, "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Orders");
                                    if (resul == 0) // Caso que inserto 
                                    {

                                        objRegistraLog.GrabaProc("Insertado a Sap Satisfactoriamente el Documento: " + grupo.NumeroDocumento + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                        try
                                        {
                                            string query = $"CALL \"{Conexion.DESTINO}\".FCS_IC_UPDATE_SEND_DIST('{grupo.NumeroDocumento}')"; //Query Encabezado
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    }
                                    else
                                    {
                                        objRegistraLog.Graba("Error insertando documento ovta DT: " + RR + " " + grupo.NumeroDocumento + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                                                                                                                                                                         //RR = "";
                                                                                                                                                                                         //BUSCO EL MAX DOCENTRY
                                        string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                        OdbcDataReader dr1 = cmand.ExecuteReader();
                                        int numero = 0;
                                        string method = "Orden de venta";
                                        while (dr1.Read())
                                        {

                                            numero = Convert.ToInt32(dr1["num"]) + 1;

                                        }
                                        dr1.Close();
                                        string tip = "ORDR";
                                        // string PAIS = "Costa Rica";
                                        string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
                                                 "values(" + numero + "," + numero + ",'" + grupo.NumeroDocumento + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
                                        OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
                                        cmc1.ExecuteNonQuery();


                                    }
                                }
                            }


                            conn.Close();
                            LiberaMemoria.FlushMemory();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;

                objRegistraLog.Graba("Error al intentar Insertar orden vta DT : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

            }

            return R;
        }
        public static Respuesta NuevaOrdenvtaTD()
        {
            int resul = 0;
            string En_DN = "";
           string pais="";
           // string bdsucursal = "20006_MODENA_S.A.";
           // double precio = 0;
          //  int ser = 73;
            Respuesta R = new Respuesta();
            //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";


           
                try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    if (conecta_service_layer(Conexion.DESTINO, "fcspa-cloud\\Rali49", "Pr12!!0bn9", "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                    {
                        R.Codigo = -2;
                        R.Mensaje = RR;
                        objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

                        return R;
                    }

                //using (OdbcConnection conn = new OdbcConnection(
                //    //Conexion.strCon
                //    connectionString
                //    ))
                {

                    string queryEn = $"SELECT * FROM \"_SYS_BIC\".\"{Conexion.ORIGEN}.INTERCOMPANY/IC_GET_PURCHASE_ORDERSTD\"";
                    //string crDet =$"SELECT \"LineNum\",\"ItemCode\",\"Quantity\",\"DiscountPercent\",\"ItemDescription\",\"Price\",\"TaxCode\",\"LineTotal\" FROM \"_SYS_BIC\".\"10028_COPPER_GROUP/SINCRONIZADOR_ORDR_CR_DET\"('PLACEHOLDER' = ('$$numdoc$$','{En_DN}'))";

                    OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion

                    conn.Open();
             
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.HasRows) {
                            List<DetallesOPORT> detallesFacturas = new List<DetallesOPORT>();
                            while (dr.Read())
                            {
                                int DocEntry = (int)dr["DocEntry"];
                                string ItemCode = (string)dr["ItemCode"];
                                decimal Quantity = (decimal)dr["Quantity"];
                                decimal PriceBefDi = (decimal)dr["PriceBefDi"];
                                decimal DiscPrcnt = (decimal)dr["DiscPrcnt"];

                                detallesFacturas.Add(new DetallesOPORT
                                {
                                    DocEntry = DocEntry,
                                    ItemCode = ItemCode,
                                    Quantity = Quantity,
                                    PriceBefDi = PriceBefDi,
                                    DiscPrcnt = DiscPrcnt
                                });
                            }
                            var detallesAgrupados = detallesFacturas
                            .GroupBy(detalle => detalle.DocEntry)
                            .Select(grupo => new { NumeroDocumento = grupo.Key, Detalles = grupo.ToList() })
                            .ToList();

                            foreach (var grupo in detallesAgrupados)
                            {
                                Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                                string DocDueDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                                JsonObject.Add(new JProperty("CardCode", "CL02034"));
                                JsonObject.Add(new JProperty("DocDueDate", DocDueDate));
                                JsonObject.Add(new JProperty("Comments", "Orden Automatica"));
                           
                                    var list = new ArrayList();
                                foreach (var detalle in grupo.Detalles)
                                {
                                    Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
                                    JsonObjectRow.Add(new JProperty("ItemCode", detalle.ItemCode));
                                    JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
                                    JsonObjectRow.Add(new JProperty("UnitPrice", detalle.PriceBefDi));
                                    JsonObjectRow.Add(new JProperty("DiscPrcnt", detalle.DiscPrcnt));
                                    list.Add(JsonObjectRow);
                                }
                                JsonObject.Add(new JProperty("DocumentLines", list));
                                string data = JsonObject.ToString();
                                resul = EnviaSap("POST", JsonObject, "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Orders");
                                if (resul == 0) // Caso que inserto 
                                {

                                    objRegistraLog.GrabaProc("Insertado a Sap Satisfactoriamente el Documento: " + grupo.NumeroDocumento + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                    try
                                    {
                                        string query = $"CALL \"{Conexion.ORIGEN}\".FCS_IC_UPDATE_SEND_DIST('{grupo.NumeroDocumento}')"; //Query Encabezado
                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                        OdbcDataReader dr1 = cmand.ExecuteReader();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                }
                                else
                                {
                                    objRegistraLog.Graba("Error insertando documento ovta TD: " + RR + " " + grupo.NumeroDocumento + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                                                                                                                                                                     //RR = "";
                                                                                                                                                                                     //BUSCO EL MAX DOCENTRY
                                    string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                    int numero = 0;
                                    string method = "Orden de venta";
                                    while (dr1.Read())
                                    {

                                        numero = Convert.ToInt32(dr1["num"]) + 1;

                                    }
                                    dr1.Close();
                                    string tip = "ORDR";
                                        // string PAIS = "Costa Rica";
                                        string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
                                                 "values(" + numero + "," + numero + ",'" + grupo.NumeroDocumento + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
                                        OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
                                        cmc1.ExecuteNonQuery();


                                    }
                                }
                        }
                        

                        conn.Close();
                        LiberaMemoria.FlushMemory();
                    }

                }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;

                objRegistraLog.Graba("Error al intentar Insertar orden venta TD : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

            }

            return R;
        }
        public static Respuesta NuevaOporautCD()
        {
            int resul = 0;
            string En_DN = "";
            string pais = "Tiend";
            //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";
            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    if (conecta_service_layer(Conexion.ORIGEN, "fcspa-cloud\\Rali49", "Pr12!!0bn9", "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                    {
                        R.Codigo = -2;
                        R.Mensaje = RR;
                        objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                        return R;
                    }
                    
                    {
                       string queryEn = $"SELECT * FROM \"_SYS_BIC\".\"{Conexion.ORIGEN}.INTERCOMPANY/IC_GET_ITEMS_REORDER\"";
                    //string crDet =$"SELECT \"LineNum\",\"ItemCode\",\"Quantity\",\"DiscountPercent\",\"ItemDescription\",\"Price\",\"TaxCode\",\"LineTotal\" FROM \"_SYS_BIC\".\"10028_COPPER_GROUP/SINCRONIZADOR_ORDR_CR_DET\"('PLACEHOLDER' = ('$$numdoc$$','{En_DN}'))";

                    OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion

                    conn.Open();
              
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.HasRows) {
                            List<DetallesOPORT> detallesFacturas = new List<DetallesOPORT>();
                            JObject JsonObject = new JObject();
                            string cod = "PL0001";
                             
                            JsonObject.Add(new JProperty("CardCode", cod));
                            JsonObject.Add(new JProperty("Comments", "Orden Automatica"));
                            JsonObject.Add(new JProperty("U_ENVIOSUC", "Y"));//se van en automatico con el intercompany

                                var list = new ArrayList();
                            while (dr.Read())
                            {
                                Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
                                JsonObjectRow.Add(new JProperty("ItemCode", dr["ItemCode"]));
                                JsonObjectRow.Add(new JProperty("Quantity", dr["Quantity"]));
                               // JsonObjectRow.Add(new JProperty("UnitPrice", dr["PriceBefDi"]));
                                //JsonObjectRow.Add(new JProperty("PriceSource", "Manual"));

                                list.Add(JsonObjectRow);
                            }
                            JsonObject.Add(new JProperty("DocumentLines", list));
                            string data = JsonObject.ToString();
                            resul = EnviaSap("POST", JsonObject, "https://hana-002.fcspa-cloud.net:50000/b1s/v1/PurchaseOrders");
                            if (resul == 0) // Caso que inserto 
                            {

                                objRegistraLog.GrabaProc("Insertado a Sap Satisfactoriamente el Documento: " + "Orden autCD" + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                               
                            }
                            else
                            {
                                objRegistraLog.Graba("Error insertando documento Orden OporautCD: " + RR + " " + "Orden de martes" + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                                                                                                                                                                 //RR = "";
                                                                                                                                                                                 //BUSCO EL MAX DOCENTRY
                                string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
                                OdbcCommand cmand = new OdbcCommand(query, conn);
                                OdbcDataReader dr1 = cmand.ExecuteReader();
                                int numero = 0;
                                string method = "OPOR";
                                while (dr1.Read())
                                {

                                    numero = Convert.ToInt32(dr1["num"]) + 1;

                                }
                                dr1.Close();
                                string tip = "OPORCD";
                                    // string PAIS = "Costa Rica";
                                    string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
                                             "values(" + numero + "," + numero + ",'" + 0 + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
                                    OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
                                    cmc1.ExecuteNonQuery();


                                }
                            }                       

                        conn.Close();
                        LiberaMemoria.FlushMemory();
                    }

                }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;

                objRegistraLog.Graba("Error al intentar Insertar orden OporautCD : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

            }

            return R;
        }
     
        public static Respuesta ActualizarArticulosTD()
        {
            int resul = 0;      
            string nomcod = string.Empty;
            //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";
            //string method = "Actualizar";
            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    string queryEnCRSV = $"SELECT * FROM \"_SYS_BIC\".\"{Conexion.ORIGEN}.INTERCOMPANY/IC_UPDATE_ITEMSTD\""; //Query Encabezado
                    

                    OdbcCommand CmD = new OdbcCommand(queryEnCRSV, conn); // llama al Query Encabezado con la conexion

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(Conexion.DESTINO, "fcspa-cloud\\Rali49", "Pr12!!0bn9", "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                            {
                                R.Codigo = -2;
                                R.Mensaje = RR;
                                objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                                return R;
                            }


                            while (dr.Read())
                            {

                              JObject JsonObject = new JObject();   
                                                             
                                nomcod = dr["ItemCode"].ToString();
                                JsonObject = CrearObjetoJson(dr);
                                JsonObject.Remove("ItemCode");
                                var myjs = JsonObject.ToString();
                               
                                resul = EnviaSap("PATCH", JsonObject, "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Items('" + nomcod + "')"); //
                         

                                if (resul == 0) // Caso que inserto 
                                {

                                    objRegistraLog.GrabaProc("Actualizado a Sap Satisfactoriamente el articulo: " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                    try {
                                        string query = $"CALL \"{Conexion.DESTINO}\".FCS_IC_CLEAR_UPDATE('{nomcod}')"; //Query Encabezado
                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                        OdbcDataReader dr1 = cmand.ExecuteReader();
                                    } catch(Exception e) {
                                        Console.WriteLine(e);
                                    }
                                }
                                else
                                {
                                    objRegistraLog.Graba("Error actualizando ArticulosTD: " + RR + " " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                                                                                                                                                      //RR = "";
                                                                                                                                                                      //BUSCO EL MAX DOCENTRY
                                    string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                    int numero = 0;
                                    while (dr1.Read())
                                    {

                                        numero = Convert.ToInt32(dr1["num"]) + 1;

                                    }
                                    dr1.Close();
                                    string tip = "OITM";
                                    string pais = "DISTRIBUIDORA";
                                    string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
                                        "values(" + numero + "," + numero + ",'" + nomcod + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
                                    OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
                                    cmc1.ExecuteNonQuery();

                                }
                            }

                            conn.Close();
                            LiberaMemoria.FlushMemory();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;

                objRegistraLog.Graba("Error al intentar Insertar ArticulosTD: " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

            }

            return R;
        }
        public static Respuesta ActualizarArticulosDT()
        {
            int resul = 0;
            string nomcod = string.Empty;
            //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";
            //string method = "Actualizar";
            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    string queryEnCRSV = $"SELECT * FROM \"_SYS_BIC\".\"{Conexion.DESTINO}.INTERCOMPANY/IC_UPDATE_ITEMSDT\""; //Query Encabezado


                    OdbcCommand CmD = new OdbcCommand(queryEnCRSV, conn); // llama al Query Encabezado con la conexion

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(Conexion.ORIGEN, "fcspa-cloud\\Rali49", "Pr12!!0bn9", "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                            {
                                R.Codigo = -2;
                                R.Mensaje = RR;
                                objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                                return R;
                            }


                            while (dr.Read())
                            {

                                JObject JsonObject = new JObject();

                                nomcod = dr["ItemCode"].ToString();
                                JsonObject = CrearObjetoJson(dr);
                                JsonObject.Remove("ItemCode");
                                var myjs = JsonObject.ToString();
                                resul = EnviaSap("PATCH", JsonObject, "https://hana-002.fcspa-cloud.net:50000/b1s/v1/Items('" + nomcod + "')"); //
                                if (resul == 0) // Caso que inserto 
                                {

                                    objRegistraLog.GrabaProc("Actualizado a Sap Satisfactoriamente el articulo: " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                    try
                                    {
                                        string query = $"CALL \"{Conexion.ORIGEN}\".FCS_IC_CLEAR_UPDATE('{nomcod}')"; //Query Encabezado
                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                        OdbcDataReader dr1 = cmand.ExecuteReader();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                }
                                else
                                {
                                    objRegistraLog.Graba("Error actualizando ArticulosDT: " + RR + " " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                                                                                                                                                    
                                    string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                    int numero = 0;
                                    while (dr1.Read())
                                    {
                                        numero = Convert.ToInt32(dr1["num"]) + 1;
                                    }
                                    dr1.Close();
                                    string tip = "OITM";
                                    string pais = "DISTRIBUIDORA";
                                    string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
                                        "values(" + numero + "," + numero + ",'" + nomcod + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
                                    OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
                                    cmc1.ExecuteNonQuery();

                                }
                            }

                            conn.Close();
                            LiberaMemoria.FlushMemory();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;

                objRegistraLog.Graba("Error al intentar Insertar ArticulosDT : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

            }

            return R;
        }
        private static int EnviaSap(string Metodo, JObject JsonObject, string URL) //, ref Marketing ObjSap, ref string RR
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
        public static int conecta_service_layer(string CompanyDB, string UserName, string Password, string URL)
        {
            try
            {
                Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                JsonObject.Add(new JProperty("CompanyDB", CompanyDB));
                JsonObject.Add(new JProperty("UserName", UserName));
                JsonObject.Add(new JProperty("Password", Password));
                var request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;
                ServicePointManager.ServerCertificateValidationCallback += RemoteSSLTLSCertificateValidate;
                request.CookieContainer = new CookieContainer();
                if (cookieData.Count > 0)
                {
                    foreach (Cookie cookie in cookieData)
                    {
                        request.CookieContainer.Add(new Uri(URL), new Cookie(cookie.Name, cookie.Value));
                    }
                }
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

        private static bool RemoteSSLTLSCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors ssl)
        {
            return true;
        }

        private static JObject CrearObjetoJson(OdbcDataReader dataReader)
        {
            JObject dataObj = new JObject();
            try
            {
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    string columnName = dataReader.GetName(i);
                    object columnValue = dataReader.GetValue(i);

                    // Agregar la propiedad al objeto JSON
                    dataObj.Add(new JProperty(columnName, columnValue));
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra durante el proceso
                Console.WriteLine("Error al crear el objeto JSON: " + ex.Message);
            }

            return dataObj;
        }
    }

    public class Respuesta
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; }
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
