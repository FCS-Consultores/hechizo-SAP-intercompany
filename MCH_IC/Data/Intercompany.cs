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
using Microsoft.VisualBasic;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using Intercompany.Models;

namespace Intercompany.Data
{
    public class Intercompany
    {
        static RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
        private static CookieCollection cookieData = new CookieCollection();
        static Marketing ObjSap = new Marketing();
        static string RR = "";
        private static Marketing marketing;

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
      



        public static void Fabricante(string bd)
        {
            int resul = 0;
            string nomcod = string.Empty;
            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn2 = new OdbcConnection(Conexion.strCon))
                {
                    
                    string queryEnCRSV = $"SELECT  \"FirmName\" FROM  \"{Conexion.ORIGEN}\".\"OMRC\" where \"FirmName\" not in (SELECT  \"FirmName\" FROM \"{bd}\".\"OMRC\")"; //Query Encabezado
                    OdbcCommand CmD = new OdbcCommand(queryEnCRSV, conn2); // llama al Query Encabezado con la conexion
                    conn2.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                    
                        if (dr.RecordsAffected != 0)
                        {

                            while (dr.Read())
                            {
                                nomcod = dr["FirmName"].ToString();
                                CrearFabricanteTODAS(bd, nomcod);
                            }
                        }
                        conn2.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al consultar Fabricante : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }          
        }
        public static void Grupo(string bd)
        {
            int resul = 0;
            string nomcod = string.Empty;
            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn2 = new OdbcConnection(Conexion.strCon))
                {

                    string queryEnCRSV = $"SELECT  \"ItmsGrpNam\" FROM  \"{Conexion.ORIGEN}\".\"OITB\" where \"ItmsGrpNam\" not in (SELECT  \"ItmsGrpNam\" FROM \"{bd}\".\"OITB\")"; //Query Encabezado
                    OdbcCommand CmD = new OdbcCommand(queryEnCRSV, conn2); // llama al Query Encabezado con la conexion
                    conn2.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {

                            while (dr.Read())
                            {
                                nomcod = dr["ItmsGrpNam"].ToString();
                                CreargrupoTODAS(bd, nomcod);
                            }
                        }
                        conn2.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al consultar grupo : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
        }
        public static void ADUANA(string bd)
        {
            int resul = 0;
            string nomcod = string.Empty;
            double Custom;
            double BuyTax;
            double OtherTax;
            double TotalTax;
            string CstGrpName;
            string GroupNum;
            string Locked;
            string DataSource;
            int UserSign;
            string cstAllcAcc;
            string cstExpAcc;
            string PortAddr;
            string PortState;
            string ExciExpAcc;
            string ExciAlcAcc;
            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn2 = new OdbcConnection(Conexion.strCon))
                using (OdbcConnection conn3 = new OdbcConnection(Conexion.strCon))
                {

                    string queryEnCRSV = $"SELECT  \"CstGrpName\" FROM  \"{Conexion.ORIGEN}\".\"OARG\" where \"CstGrpName\" not in (SELECT  \"CstGrpName\" FROM \"{bd}\".\"OARG\")"; //Query Encabezado
                    OdbcCommand CmD = new OdbcCommand(queryEnCRSV, conn2); // llama al Query Encabezado con la conexion
                    conn2.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.RecordsAffected != 0)
                        {
                            while (dr.Read())
                            {

                                CstGrpName = dr["CstGrpName"].ToString();
                                string querY = $"SELECT  \"GroupNum\",IFNULL(\"Custom\",0) \"Custom\",IFNULL(\"BuyTax\",0) \"BuyTax\",IFNULL(\"OtherTax\",0) \"OtherTax\",IFNULL(\"TotalTax\",0) \"TotalTax\",\"Locked\",\"DataSource\",\"UserSign\",\"cstAllcAcc\",\"cstExpAcc\",\"PortAddr\", \"PortState\", \"ExciExpAcc\", \"ExciAlcAcc\" FROM  \"{Conexion.ORIGEN}\".\"OARG\" where \"CstGrpName\"='" +  CstGrpName +"'"; //Query Encabezado
                                OdbcCommand CmD1 = new OdbcCommand(querY, conn3); // llama al Query Encabezado con la conexion
                                conn3.Open();
                                using (OdbcDataReader dr2 = CmD1.ExecuteReader())

                                    while (dr2.Read())
                                    {
                                        GroupNum = dr2["GroupNum"].ToString();
                                        Custom = Convert.ToDouble(dr2["Custom"]);
                                        BuyTax = Convert.ToDouble(dr2["BuyTax"]);
                                        OtherTax = Convert.ToDouble(dr2["OtherTax"]);
                                        TotalTax = Convert.ToDouble(dr2["TotalTax"]);
                                        Locked = dr2["Locked"].ToString();
                                        DataSource = dr2["DataSource"].ToString();
                                       // UserSign = Convert.ToInt32(dr2["UserSign"]);
                                        cstAllcAcc = dr2["cstAllcAcc"].ToString();
                                        cstExpAcc = dr2["cstExpAcc"].ToString();
                                        PortAddr = dr2["PortAddr"].ToString();
                                        PortState = dr2["PortState"].ToString();
                                        ExciExpAcc = dr2["ExciExpAcc"].ToString();
                                        ExciAlcAcc = dr2["ExciAlcAcc"].ToString();
                                        CrearADUANATODAS(bd, CstGrpName, GroupNum, Custom, BuyTax, OtherTax, TotalTax, Locked, DataSource,  cstAllcAcc, cstExpAcc, PortAddr, PortState, ExciExpAcc, ExciAlcAcc);
                                    }
                                conn3.Close();
                            }
                        }
                        conn2.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al consultar grupo : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
        }



        public static void SCSucursal(string bd)
        {
            int resul = 0;
            string nomcod = string.Empty;
            string ReqName;
            string U_PROYECTO;
            string Comments;
             int DocEntry;
            string  U_TipoCompra;
            string DocDate;
            string DocDueDate;
            string TaxDate;
            string ReqDate;
      
            //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";
            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                using (OdbcConnection conn1 = new OdbcConnection(Conexion.strCon))
                {
                    {
                        string queryEn = $"select \"DocEntry\",\"ReqName\" ,\"U_PROYECTO\",\"U_TipoCompra\",\"Comments\",\"DocDate\",\"DocDueDate\",\"TaxDate\",\"ReqDate\" from \"{bd}\".\"OPRQ\" where \"U_Enviado\" ='0' and \"U_ENVIOSUC\"='Y'";
                        OdbcCommand CmD1 = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                        conn.Open();
                        using (OdbcDataReader dr2 = CmD1.ExecuteReader())
                            while (dr2.Read())
                            {
                                DocEntry = Convert.ToInt32(dr2["DocEntry"]);
                                ReqName = dr2["ReqName"].ToString();
                                U_PROYECTO = dr2["U_PROYECTO"].ToString();
                                U_TipoCompra = dr2["U_TipoCompra"].ToString();
                                Comments = dr2["Comments"].ToString();
                                DocDate = dr2["DocDate"].ToString();
                                DocDueDate = dr2["DocDueDate"].ToString();
                                TaxDate = dr2["TaxDate"].ToString();
                                ReqDate = dr2["ReqDate"].ToString();

                                //BUSCO CODIGO DE PROYECTO DE UNA VEZ
                                string query = $"SELECT  \"PrjCode\"   FROM \"{Conexion.ORIGEN}\".\"OPRJ\" WHERE \"PrjName\"='"+ U_PROYECTO +"'"; //Query Encabezado
                                OdbcCommand cmand = new OdbcCommand(query, conn1);
                                conn1.Open();
                                OdbcDataReader dr1 = cmand.ExecuteReader();
                                string CODPROYE = string.Empty;
                                while (dr1.Read())
                                {
                                     CODPROYE = dr1["PrjCode"].ToString();
                                }
                                dr1.Close();
                                conn1.Close();
                                CrearSCTODAS(bd, DocEntry, ReqName, U_PROYECTO, U_TipoCompra, Comments, DocDate, DocDueDate, TaxDate, ReqDate, CODPROYE);
                            }
                    }

                    conn.Close();
                    LiberaMemoria.FlushMemory();
                }                    
                }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al consultar SC : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
        }

        //public static Respuesta convierteOVTA_OC()
        ////{
        ////    int resul = 0;
        ////    string En_DN = "";
        ////    string pais = "";
        ////    // string bdsucursal = "20006_MODENA_S.A.";
        ////    // double precio = 0;
        ////    //  int ser = 73;
        ////    Respuesta R = new Respuesta();
        ////    //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";
        ////    //CREA ORDEN DE COMPRA DESDE LA ORDEN DE VENTA DE PRIZMA QUE CAE EN SAP VER VISTA IC_OVTA_A_OC_DT PARA LUEGO SER ENVIADA 
        ////    try
        ////    {
        ////        using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
        ////        {
        ////            if (conecta_service_layer(Conexion.DESTINO, "fcspa-cloud\\mch05", "QUtg!335Gb", "https://hana-mcholding:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
        ////            {
        ////                R.Codigo = -2;
        ////                R.Mensaje = RR;
        ////                objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

        ////                return R;
        ////            }

        ////            //using (OdbcConnection conn = new OdbcConnection(
        ////            //    //Conexion.strCon
        ////            //    connectionString
        ////            //    ))
        ////            {

        ////                string queryEn = $"SELECT \"ItemCode\", \"DocEntry\",	 \"Quantity\",	 \"PriceBefDi\",	\"DiscPrcnt\"  FROM \"_SYS_BIC\".\"{Conexion.DESTINO}.INTERCOMPANY/IC_OVTA_A_OC_DT\"";

        ////                OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion

        ////                conn.Open();

        ////                using (OdbcDataReader dr = CmD.ExecuteReader())
        ////                {
        ////                    if (dr.HasRows)
        ////                    {
        ////                        List<DetallesOPORT> detallesFacturas = new List<DetallesOPORT>();
        ////                        while (dr.Read())
        ////                        {
        ////                            int DocEntry = (int)dr["DocEntry"];
        ////                            string ItemCode = (string)dr["ItemCode"];
        ////                            decimal Quantity = (decimal)dr["Quantity"];
        ////                            decimal PriceBefDi = (decimal)dr["PriceBefDi"];
        ////                            //decimal DiscPrcnt = (decimal)dr["DiscPrcnt"];

        ////                            detallesFacturas.Add(new DetallesOPORT
        ////                            {
        ////                                DocEntry = DocEntry,
        ////                                ItemCode = ItemCode,
        ////                                Quantity = Quantity,
        ////                                PriceBefDi = PriceBefDi,
        ////                                //DiscPrcnt = DiscPrcnt
        ////                            });
        ////                        }
        ////                        var detallesAgrupados = detallesFacturas
        ////                        .GroupBy(detalle => detalle.DocEntry)
        ////                        .Select(grupo => new { NumeroDocumento = grupo.Key, Detalles = grupo.ToList() })
        ////                        .ToList();

        ////                        foreach (var grupo in detallesAgrupados)
        ////                        {
        ////                            string COD = "PL00030";
        ////                            Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
        ////                            string DocDueDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        ////                            JsonObject.Add(new JProperty("CardCode",COD ));
        ////                           // JsonObject.Add(new JProperty("DocDueDate", DocDueDate));
        ////                            JsonObject.Add(new JProperty("Comments", "Orden Automatica Prizma"));
        ////                            JsonObject.Add(new JProperty("U_ENVIOSUC", "Y"));
        ////                            JsonObject.Add(new JProperty("U_sisori", "Sistema"));
        ////                            var list = new ArrayList();
        ////                            foreach (var detalle in grupo.Detalles)
        ////                            {
        ////                                Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
        ////                                JsonObjectRow.Add(new JProperty("ItemCode", detalle.ItemCode));
        ////                                JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
        ////                                JsonObjectRow.Add(new JProperty("UnitPrice", detalle.PriceBefDi));
        ////                                //JsonObjectRow.Add(new JProperty("DiscPrcnt", detalle.DiscPrcnt));
        ////                                list.Add(JsonObjectRow);
        ////                            }

        ////                            JsonObject.Add(new JProperty("DocumentLines", list));
        ////                            string data = JsonObject.ToString();
        ////                            resul = EnviaSap("POST", JsonObject, "https://hana-mcholding:50000/b1s/v1/PurchaseOrders");
        ////                            if (resul == 0) // Caso que inserto 
        ////                            {

        ////                                objRegistraLog.GrabaProc("Insertado a Sap Satisfactoriamente el Documento: " + grupo.NumeroDocumento + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
        ////                                try
        ////                                {
        ////                                    //string query = $"UPDATE  \"{Conexion.DESTINO}\".OPOR('{grupo.NumeroDocumento}')"; //Query Encabezado
        ////                                    string query = $"UPDATE  \"{Conexion.DESTINO}\".ORDR\" SET \"U_Enviado\" = '0' WHERE \"ItemCode\" =  '" + grupo.NumeroDocumento + "'";
        ////                                    // string queryAct = $"UPDATE \"{BRETASV}\".\"OITM\" SET \"U_act\" = '" + vac + "' WHERE \"ItemCode\" =  '" + En_DN + "'"; //Query Actualiza Entry_fac
        ////                                    OdbcCommand cmand = new OdbcCommand(query, conn);
        ////                                    OdbcDataReader dr1 = cmand.ExecuteReader();
        ////                                }
        ////                                catch (Exception e)
        ////                                {
        ////                                    Console.WriteLine(e);
        ////                                }
        ////                            }
        ////                            else
        ////                            {
        ////                                objRegistraLog.Graba("Error convirtiendo OVTA a OC: " + RR + " " + grupo.NumeroDocumento + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
        ////                                                                                                                                                                                 //RR = "";
        ////                                                                                                                                                                                 //BUSCO EL MAX DOCENTRY
        ////                                string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
        ////                                OdbcCommand cmand = new OdbcCommand(query, conn);
        ////                                OdbcDataReader dr1 = cmand.ExecuteReader();
        ////                                int numero = 0;
        ////                                string method = "ORDR PRIZMA";
        ////                                while (dr1.Read())
        ////                                {

        ////                                    numero = Convert.ToInt32(dr1["num"]) + 1;

        ////                                }
        ////                                dr1.Close();
        ////                                string tip = "ORDR";
        ////                                // string PAIS = "Costa Rica";
        ////                                string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
        ////                                         "values(" + numero + "," + numero + ",'" + grupo.NumeroDocumento + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
        ////                                OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
        ////                                cmc1.ExecuteNonQuery();


        ////                            }
        ////                        }
        ////                    }


        ////                    conn.Close();
        ////                    LiberaMemoria.FlushMemory();
        ////                }

        ////            }
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        R.Codigo = -1;
        ////        R.Mensaje = "Alerta: " + ex.Message;

        ////        objRegistraLog.Graba("Error al intentar Insertar orden convierte ovta_oc : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

        ////    }

        ////    return R;
        //}

        //public static Respuesta NuevaOrdenvtaDT()
        //{
        //    int resul = 0;
        //    string En_DN = "";
        //    string pais = "";
        //    // string bdsucursal = "20006_MODENA_S.A.";
        //    // double precio = 0;
        //    //  int ser = 73;
        //    Respuesta R = new Respuesta();
        //    //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";



        //    try
        //    {
        //        using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
        //        {
        //            if (conecta_service_layer(Conexion.ORIGEN, "fcspa-cloud\\mch05", "QUtg!335Gb", "https://hana-mcholding:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
        //            {
        //                R.Codigo = -2;
        //                R.Mensaje = RR;
        //                objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

        //                return R;
        //            }

        //            //using (OdbcConnection conn = new OdbcConnection(
        //            //    //Conexion.strCon
        //            //    connectionString
        //            //    ))
        //            {

        //                string queryEn = $"SELECT * FROM \"_SYS_BIC\".\"{Conexion.DESTINO}.INTERCOMPANY/IC_GET_PURCHASE_ORDERSDT\"";
        //                //string crDet =$"SELECT \"LineNum\",\"ItemCode\",\"Quantity\",\"DiscountPercent\",\"ItemDescription\",\"Price\",\"TaxCode\",\"LineTotal\" FROM \"_SYS_BIC\".\"10028_COPPER_GROUP/SINCRONIZADOR_ORDR_CR_DET\"('PLACEHOLDER' = ('$$numdoc$$','{En_DN}'))";

        //                OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion

        //                conn.Open();

        //                using (OdbcDataReader dr = CmD.ExecuteReader())
        //                {
        //                    if (dr.HasRows)
        //                    {
        //                        List<DetallesOPORT> detallesFacturas = new List<DetallesOPORT>();
        //                        while (dr.Read())
        //                        {
        //                            int DocEntry = (int)dr["DocEntry"];
        //                            string ItemCode = (string)dr["ItemCode"];
        //                            decimal Quantity = (decimal)dr["Quantity"];
        //                            decimal PriceBefDi = (decimal)dr["PriceBefDi"];
        //                            decimal DiscPrcnt = (decimal)dr["DiscPrcnt"];

        //                            detallesFacturas.Add(new DetallesOPORT
        //                            {
        //                                DocEntry = DocEntry,
        //                                ItemCode = ItemCode,
        //                                Quantity = Quantity,
        //                                PriceBefDi = PriceBefDi,
        //                                DiscPrcnt = DiscPrcnt
        //                            });
        //                        }
        //                        var detallesAgrupados = detallesFacturas
        //                        .GroupBy(detalle => detalle.DocEntry)
        //                        .Select(grupo => new { NumeroDocumento = grupo.Key, Detalles = grupo.ToList() })
        //                        .ToList();

        //                        foreach (var grupo in detallesAgrupados)
        //                        {
        //                            Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
        //                            string DocDueDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        //                            JsonObject.Add(new JProperty("CardCode", "C15967"));
        //                            JsonObject.Add(new JProperty("DocDueDate", DocDueDate));
        //                            JsonObject.Add(new JProperty("Comments", "Orden Automatica"));
        //                            var list = new ArrayList();
        //                            foreach (var detalle in grupo.Detalles)
        //                            {
        //                                Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
        //                                JsonObjectRow.Add(new JProperty("ItemCode", detalle.ItemCode));
        //                                JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
        //                                JsonObjectRow.Add(new JProperty("UnitPrice", detalle.PriceBefDi));
        //                                JsonObjectRow.Add(new JProperty("DiscPrcnt", detalle.DiscPrcnt));
        //                                list.Add(JsonObjectRow);
        //                            }
        //                            JsonObject.Add(new JProperty("DocumentLines", list));
        //                            string data = JsonObject.ToString();
        //                            resul = EnviaSap("POST", JsonObject, "https://hana-mcholding:50000/b1s/v1/Orders");
        //                            if (resul == 0) // Caso que inserto 
        //                            {

        //                                objRegistraLog.GrabaProc("Insertado a Sap Satisfactoriamente el Documento: " + grupo.NumeroDocumento + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
        //                                try
        //                                {
        //                                    string query = $"CALL \"{Conexion.DESTINO}\".FCS_IC_UPDATE_SEND_DIST('{grupo.NumeroDocumento}')"; //Query Encabezado
        //                                    OdbcCommand cmand = new OdbcCommand(query, conn);
        //                                    OdbcDataReader dr1 = cmand.ExecuteReader();
        //                                }
        //                                catch (Exception e)
        //                                {
        //                                    Console.WriteLine(e);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                objRegistraLog.Graba("Error insertando documento ovta DT: " + RR + " " + grupo.NumeroDocumento + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
        //                                                                                                                                                                                 //RR = "";
        //                                                                                                                                                                                 //BUSCO EL MAX DOCENTRY
        //                                string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
        //                                OdbcCommand cmand = new OdbcCommand(query, conn);
        //                                OdbcDataReader dr1 = cmand.ExecuteReader();
        //                                int numero = 0;
        //                                string method = "Orden de venta";
        //                                while (dr1.Read())
        //                                {

        //                                    numero = Convert.ToInt32(dr1["num"]) + 1;

        //                                }
        //                                dr1.Close();
        //                                string tip = "ORDR";
        //                                // string PAIS = "Costa Rica";
        //                                string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
        //                                         "values(" + numero + "," + numero + ",'" + grupo.NumeroDocumento + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
        //                                OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
        //                                cmc1.ExecuteNonQuery();


        //                            }
        //                        }
        //                    }


        //                    conn.Close();
        //                    LiberaMemoria.FlushMemory();
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        R.Codigo = -1;
        //        R.Mensaje = "Alerta: " + ex.Message;

        //        objRegistraLog.Graba("Error al intentar Insertar orden vta DT : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

        //    }

        //    return R;
        //}
        //public static Respuesta NuevaOrdenvtaTD()
        //{
        //    int resul = 0;
        //    string En_DN = "";
        //   string pais="";
        //   // string bdsucursal = "20006_MODENA_S.A.";
        //   // double precio = 0;
        //  //  int ser = 73;
        //    Respuesta R = new Respuesta();
        //    //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";



        //        try
        //    {
        //        using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
        //        {
        //            if (conecta_service_layer(Conexion.DESTINO, "fcspa-cloud\\mch05", "QUtg!335Gb", "https://hana-mcholding:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
        //            {
        //                R.Codigo = -2;
        //                R.Mensaje = RR;
        //                objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

        //                return R;
        //            }

        //        //using (OdbcConnection conn = new OdbcConnection(
        //        //    //Conexion.strCon
        //        //    connectionString
        //        //    ))
        //        {

        //            string queryEn = $"SELECT * FROM \"_SYS_BIC\".\"{Conexion.ORIGEN}.INTERCOMPANY/IC_GET_PURCHASE_ORDERSTD\"";
        //            //string crDet =$"SELECT \"LineNum\",\"ItemCode\",\"Quantity\",\"DiscountPercent\",\"ItemDescription\",\"Price\",\"TaxCode\",\"LineTotal\" FROM \"_SYS_BIC\".\"10028_COPPER_GROUP/SINCRONIZADOR_ORDR_CR_DET\"('PLACEHOLDER' = ('$$numdoc$$','{En_DN}'))";

        //            OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion

        //            conn.Open();

        //            using (OdbcDataReader dr = CmD.ExecuteReader())
        //            {
        //                if (dr.HasRows) {
        //                    List<DetallesOPORT> detallesFacturas = new List<DetallesOPORT>();
        //                    while (dr.Read())
        //                    {
        //                        int DocEntry = (int)dr["DocEntry"];
        //                        string ItemCode = (string)dr["ItemCode"];
        //                        decimal Quantity = (decimal)dr["Quantity"];
        //                        decimal PriceBefDi = (decimal)dr["PriceBefDi"];
        //                        decimal DiscPrcnt = (decimal)dr["DiscPrcnt"];

        //                        detallesFacturas.Add(new DetallesOPORT
        //                        {
        //                            DocEntry = DocEntry,
        //                            ItemCode = ItemCode,
        //                            Quantity = Quantity,
        //                            PriceBefDi = PriceBefDi,
        //                            DiscPrcnt = DiscPrcnt
        //                        });
        //                    }
        //                    var detallesAgrupados = detallesFacturas
        //                    .GroupBy(detalle => detalle.DocEntry)
        //                    .Select(grupo => new { NumeroDocumento = grupo.Key, Detalles = grupo.ToList() })
        //                    .ToList();

        //                    foreach (var grupo in detallesAgrupados)
        //                    {
        //                        Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
        //                        string DocDueDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        //                        JsonObject.Add(new JProperty("CardCode", "CL02034"));
        //                        JsonObject.Add(new JProperty("DocDueDate", DocDueDate));
        //                        JsonObject.Add(new JProperty("Comments", "Orden Automatica"));

        //                            var list = new ArrayList();
        //                        foreach (var detalle in grupo.Detalles)
        //                        {
        //                            Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
        //                            JsonObjectRow.Add(new JProperty("ItemCode", detalle.ItemCode));
        //                            JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
        //                            JsonObjectRow.Add(new JProperty("UnitPrice", detalle.PriceBefDi));
        //                            JsonObjectRow.Add(new JProperty("DiscPrcnt", detalle.DiscPrcnt));
        //                            list.Add(JsonObjectRow);
        //                        }
        //                        JsonObject.Add(new JProperty("DocumentLines", list));
        //                        string data = JsonObject.ToString();
        //                        resul = EnviaSap("POST", JsonObject, "https://hana-mcholding:50000/b1s/v1/Orders");
        //                        if (resul == 0) // Caso que inserto 
        //                        {

        //                            objRegistraLog.GrabaProc("Insertado a Sap Satisfactoriamente el Documento: " + grupo.NumeroDocumento + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
        //                            try
        //                            {
        //                                string query = $"CALL \"{Conexion.ORIGEN}\".FCS_IC_UPDATE_SEND_DIST('{grupo.NumeroDocumento}')"; //Query Encabezado
        //                                OdbcCommand cmand = new OdbcCommand(query, conn);
        //                                OdbcDataReader dr1 = cmand.ExecuteReader();
        //                            }
        //                            catch (Exception e)
        //                            {
        //                                Console.WriteLine(e);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            objRegistraLog.Graba("Error insertando documento ovta TD: " + RR + " " + grupo.NumeroDocumento + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
        //                                                                                                                                                                             //RR = "";
        //                                                                                                                                                                             //BUSCO EL MAX DOCENTRY
        //                            string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
        //                            OdbcCommand cmand = new OdbcCommand(query, conn);
        //                            OdbcDataReader dr1 = cmand.ExecuteReader();
        //                            int numero = 0;
        //                            string method = "Orden de venta";
        //                            while (dr1.Read())
        //                            {

        //                                numero = Convert.ToInt32(dr1["num"]) + 1;

        //                            }
        //                            dr1.Close();
        //                            string tip = "ORDR";
        //                                // string PAIS = "Costa Rica";
        //                                string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
        //                                         "values(" + numero + "," + numero + ",'" + grupo.NumeroDocumento + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
        //                                OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
        //                                cmc1.ExecuteNonQuery();


        //                            }
        //                        }
        //                }


        //                conn.Close();
        //                LiberaMemoria.FlushMemory();
        //            }

        //        }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        R.Codigo = -1;
        //        R.Mensaje = "Alerta: " + ex.Message;

        //        objRegistraLog.Graba("Error al intentar Insertar orden venta TD : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

        //    }

        //    return R;
        //}


        //    return R;
        //}
        //public static Respuesta ActualizarArticulosDT()
        //{
        //    int resul = 0;
        //    string nomcod = string.Empty;
        //    //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";
        //    //string method = "Actualizar";
        //    Respuesta R = new Respuesta();
        //    try
        //    {
        //        using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
        //        {
        //            string queryEnCRSV = $"SELECT * FROM \"_SYS_BIC\".\"{Conexion.DESTINO}.INTERCOMPANY/IC_UPDATE_ITEMSDT\""; //Query Encabezado


        //            OdbcCommand CmD = new OdbcCommand(queryEnCRSV, conn); // llama al Query Encabezado con la conexion

        //            conn.Open();

        //            using (OdbcDataReader dr = CmD.ExecuteReader())
        //            {

        //                if (dr.RecordsAffected != 0)
        //                {
        //                    if (conecta_service_layer(Conexion.ORIGEN, "fcspa-cloud\\mch05", "QUtg!335Gb", "https://hana-mcholding:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
        //                    {
        //                        R.Codigo = -2;
        //                        R.Mensaje = RR;
        //                        objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
        //                        return R;
        //                    }


        //                    while (dr.Read())
        //                    {

        //                        JObject JsonObject = new JObject();

        //                        nomcod = dr["ItemCode"].ToString();
        //                        JsonObject = CrearObjetoJson(dr);
        //                        JsonObject.Remove("ItemCode");
        //                        var myjs = JsonObject.ToString();
        //                        resul = EnviaSap("PATCH", JsonObject, "https://hana-mcholding:50000/b1s/v1/Items('" + nomcod + "')"); //
        //                        if (resul == 0) // Caso que inserto 
        //                        {

        //                            objRegistraLog.GrabaProc("Actualizado a Sap Satisfactoriamente el articulo: " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
        //                            try
        //                            {
        //                                string query = $"CALL \"{Conexion.ORIGEN}\".FCS_IC_CLEAR_UPDATE('{nomcod}')"; //Query Encabezado
        //                                OdbcCommand cmand = new OdbcCommand(query, conn);
        //                                OdbcDataReader dr1 = cmand.ExecuteReader();
        //                            }
        //                            catch (Exception e)
        //                            {
        //                                Console.WriteLine(e);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            objRegistraLog.Graba("Error actualizando ArticulosDT: " + RR + " " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio

        //                            string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
        //                            OdbcCommand cmand = new OdbcCommand(query, conn);
        //                            OdbcDataReader dr1 = cmand.ExecuteReader();
        //                            int numero = 0;
        //                            while (dr1.Read())
        //                            {
        //                                numero = Convert.ToInt32(dr1["num"]) + 1;
        //                            }
        //                            dr1.Close();
        //                            string tip = "OITM";
        //                            string pais = "DISTRIBUIDORA";
        //                            string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
        //                                "values(" + numero + "," + numero + ",'" + nomcod + "','" + tip + "','" + pais.ToString() + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
        //                            OdbcCommand cmc1 = new OdbcCommand(queryAct, conn);
        //                            cmc1.ExecuteNonQuery();

        //                        }
        //                    }

        //                    conn.Close();
        //                    LiberaMemoria.FlushMemory();
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        R.Codigo = -1;
        //        R.Mensaje = "Alerta: " + ex.Message;

        //        objRegistraLog.Graba("Error al intentar Insertar ArticulosDT : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error

        //    }

        //    return R;
        //}
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

        private static bool RemoteSSLTLSCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors ssl)
        {
            return true;
        }

        private static void Inserterror(string pais,string tip, string nomcod) //, ref Marketing ObjSap, ref string RR
        {
          
            try
            {
                using (OdbcConnection conn9 = new OdbcConnection(Conexion.strCon))
                   
                {
                    conn9.Open();
                    string query = $"SELECT  ifnull(max(\"DocEntry\"),0)  as num  FROM \"{Conexion.ORIGEN}\".\"@SINCROERROR\""; //Query Encabezado
                    OdbcCommand cmand = new OdbcCommand(query, conn9);
                    OdbcDataReader dr1 = cmand.ExecuteReader();
                    int numero = 0;
                    while (dr1.Read())
                    {

                        numero = Convert.ToInt32(dr1["num"]) + 1;

                    }
                    dr1.Close();
                    string queryAct = $"INSERT INTO \"{Conexion.ORIGEN}\".\"@SINCROERROR\" (\"DocEntry\",\"DocNum\",\"U_docunum\",\"U_tipo\",\"U_pais\",\"U_mensaje\",\"U_fecha\",\"U_hora\")" +
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

        public static Respuesta CrearFabricanteTODAS(string bd, string codigo)
        {
            int resul = 0;       
            Respuesta R = new Respuesta();
            try
            {               
                    if (conecta_service_layer(bd, "fcspa-cloud\\mch05", "QUtg!335Gb", "https://hana-mcholding:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                    {
                        R.Codigo = -2;
                        R.Mensaje = RR;
                        objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                        return R;
                    }                      
                        Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();                              
                        JsonObject.Add(new JProperty("ManufacturerName", codigo));
                 
                        var myjs = JsonObject.ToString();
                        resul = EnviaSap("POST", JsonObject, "https://hana-mcholding:50000/b1s/v1/Manufacturers"); //
                        if (resul == 0) // Caso que inserto 
                        {
                            objRegistraLog.Graba("Insertado a Sap el fabricante : " + codigo + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                        }
                        else
                        {
                            objRegistraLog.Graba("Error insertando fabricante: " + RR + " " + codigo + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                            Inserterror(bd.Substring(6, 6), "OMRC", codigo);
                        }                

                        LiberaMemoria.FlushMemory();            
                     }            
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al intentar Insertar Fabricante : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
            return R;
        }
        public static Respuesta CreargrupoTODAS(string bd, string codigo)
        {
            int resul = 0;
            Respuesta R = new Respuesta();
            try
            {
                if (conecta_service_layer(bd, "fcspa-cloud\\mch05", "QUtg!335Gb", "https://hana-mcholding:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                {
                    R.Codigo = -2;
                    R.Mensaje = RR;
                    objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                    return R;
                }
                Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                JsonObject.Add(new JProperty("GroupName", codigo));
                var myjs = JsonObject.ToString();
                resul = EnviaSap("POST", JsonObject, "https://hana-mcholding:50000/b1s/v1/ItemGroups"); //
                if (resul == 0) // Caso que inserto 
                {
                    objRegistraLog.Graba("Insertado a Sap el grupo : " + codigo + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                }
                else
                {
                    objRegistraLog.Graba("Error insertando grupo: " + RR + " " + codigo + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                    Inserterror(bd.Substring(6,6), "OITB", codigo);
                }

                LiberaMemoria.FlushMemory();
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al intentar Insertar grupo : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
            return R;
        }
        public static Respuesta CreararticuloTODAS(string bd)
        {
            int resul = 0;
            string nomcod = string.Empty;
            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    string query = $"CALL \"{Conexion.ORIGEN}\".IC_ADD_ITEMS2('{bd}')"; //Query Encabezado
                    OdbcCommand CmD = new OdbcCommand(query, conn); // llama al Query Encabezado con la conexion
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(bd, "fcspa-cloud\\mch05", "QUtg!335Gb", "https://hana-mcholding:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
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
                                resul = EnviaSap("POST", JsonObject, "https://hana-mcholding:50000/b1s/v1/Items");//

                                if (resul == 0) // Caso que inserto 
                                {
                                    objRegistraLog.Graba("Insertado a Sap el articulo : " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                }
                                else
                                {
                                    objRegistraLog.Graba("Error insertando articulo: " + RR + " " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                                    Inserterror(bd.Substring(6, 6), "OITM", nomcod);
                                }
                            }
                            LiberaMemoria.FlushMemory();
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al intentar Insertar articulo : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
            return R;
        }
        public static Respuesta CrearADUANATODAS(string bd, string CstGrpName, string GroupNum, double Custom, double BuyTax, double OtherTax, double TotalTax, string Locked, string DataSource,  string cstAllcAcc, string cstExpAcc, string  PortAddr,string  PortState,string  ExciExpAcc, string ExciAlcAcc)
        {
            int resul = 0;
            Respuesta R = new Respuesta();
            try
            {
                if (conecta_service_layer(bd, "fcspa-cloud\\mch05", "QUtg!335Gb", "https://hana-mcholding:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                {
                    R.Codigo = -2;
                    R.Mensaje = RR;
                    objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                    return R;
                }
                Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                JsonObject.Add(new JProperty("CstGrpName", CstGrpName));
                JsonObject.Add(new JProperty("GroupNum", GroupNum));
                JsonObject.Add(new JProperty("Customs", Custom));
                JsonObject.Add(new JProperty("BuyTax", BuyTax));
                JsonObject.Add(new JProperty("OtherTax", OtherTax));
                JsonObject.Add(new JProperty("TotalTax", TotalTax));
                JsonObject.Add(new JProperty("Locked", Locked));
                JsonObject.Add(new JProperty("DataSource", DataSource));
                JsonObject.Add(new JProperty("UserSign", 1));
                JsonObject.Add(new JProperty("cstAllcAcc", cstAllcAcc));
                JsonObject.Add(new JProperty("cstExpAcc", cstExpAcc));
                JsonObject.Add(new JProperty("PortAddr", PortAddr));
                JsonObject.Add(new JProperty("PortState", PortState));
                JsonObject.Add(new JProperty("ExciExpAcc", ExciExpAcc));
                JsonObject.Add(new JProperty("ExciAlcAcc", ExciAlcAcc));

                var myjs = JsonObject.ToString();
                resul = EnviaSap("POST", JsonObject, "https://hana-mcholding:50000/b1s/v1/CustomsGroups"); //
                if (resul == 0) // Caso que inserto 
                {
                    objRegistraLog.Graba("Insertado a Sap ADUANA : " + CstGrpName + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                }
                else
                {
                    objRegistraLog.Graba("Error insertando AFUANA: " + RR + " " + CstGrpName + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                    Inserterror(bd.Substring(6, 6), "OARC", CstGrpName);
                }

                LiberaMemoria.FlushMemory();
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al intentar Insertar ADUANA: " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
            return R;
        }
        public static Respuesta CrearSCTODAS(string bd, int DocEntry, string ReqName, string U_PROYECTO, string U_TipoCompra, string Comments, string DocDate, string DocDueDate, string TaxDate, string  ReqDate,string CODPROYE)
        {
            int resul = 0;
            Respuesta R = new Respuesta();
            try
            {

                if (conecta_service_layer(Conexion.ORIGEN, "fcspa-cloud\\mch05", "QUtg!335Gb", "https://hana-mcholding:50000/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                {
                    R.Codigo = -2;
                    R.Mensaje = RR;
                    objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                    return R;
                }
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    string queryEn = $"SELECT \"ItemCode\",	 \"Quantity\", \"VatGroup\" FROM \"{bd}\".\"PRQ1\" where \"DocEntry\"='" + DocEntry + "'";

                    OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            List<DetallesOPORT> detallesFacturas = new List<DetallesOPORT>();
                            while (dr.Read())
                            {
                                string ItemCode = (string)dr["ItemCode"];
                                decimal Quantity = (decimal)dr["Quantity"];
                                string impto = (string)dr["VatGroup"];
                                string codprj = CODPROYE;             

                                detallesFacturas.Add(new DetallesOPORT
                                {
                                    DocEntry = DocEntry,
                                    ItemCode = ItemCode,
                                    Quantity = Quantity,
                                    // PriceBefDi = PriceBefDi,
                                    VatGroup = impto,
                                    PROYECTO = codprj

                                });
                            }
                            var detallesAgrupados = detallesFacturas
                            .GroupBy(detalle => detalle.DocEntry)
                            .Select(grupo => new { NumeroDocumento = grupo.Key, Detalles = grupo.ToList() })
                            .ToList();

                            foreach (var grupo in detallesAgrupados)
                            {
                          
                                Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                             
                                JsonObject.Add(new JProperty("Comments", Comments));                  
                                DateTime Fecha1 = Convert.ToDateTime(DocDate.ToString()); // "DocDate"
                                DateTime Fecha2 = Convert.ToDateTime(DocDueDate.ToString()); // "DocDate"
                                DateTime Fecha3 = Convert.ToDateTime(TaxDate.ToString()); // "DocDate"
                                DateTime Fecha4 = Convert.ToDateTime(ReqDate.ToString()); // "DocDate"
                                string date1 = Fecha1.ToString("yyyy-MM-dd");
                                string date2 = Fecha2.ToString("yyyy-MM-dd");

                                string date3 = Fecha3.ToString("yyyy-MM-dd");
                                string date4 = Fecha4.ToString("yyyy-MM-dd");
                                JsonObject.Add(new JProperty("U_PROYECTO", U_PROYECTO));
                                JsonObject.Add(new JProperty("U_TipoCompra", U_TipoCompra));
                                JsonObject.Add(new JProperty("DocDate", date1));
                                JsonObject.Add(new JProperty("DocDueDate", date2));
                                JsonObject.Add(new JProperty("TaxDate", date3));
                                JsonObject.Add(new JProperty("RequriedDate", date4));
                                JsonObject.Add(new JProperty("SendNotification", "tNO"));
                                var list = new ArrayList();
                                foreach (var detalle in grupo.Detalles)
                                {
                                    Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
                                    JsonObjectRow.Add(new JProperty("ItemCode", detalle.ItemCode));
                                    JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
                                    //JsonObjectRow.Add(new JProperty("UnitPrice", detalle.PriceBefDi));
                                    JsonObjectRow.Add(new JProperty("VatGroup", detalle.VatGroup));
                                    JsonObjectRow.Add(new JProperty("ProjectCode", detalle.PROYECTO));
                                    
                                    list.Add(JsonObjectRow);
                                }

                                JsonObject.Add(new JProperty("DocumentLines", list));
                                string data = JsonObject.ToString();

                                var myjs = JsonObject.ToString();
                                resul = EnviaSap("POST", JsonObject, "https://hana-mcholding:50000/b1s/v1/PurchaseRequests"); //
                                if (resul == 0) // Caso que inserto 
                                {
                                    string query = $"UPDATE  \"{bd}\".\"OPRQ\" SET \"U_Enviado\" = '1' WHERE \"DocEntry\" =  '" + DocEntry + "'";
                       
                                    // string queryAct = $"UPDATE \"{BRETASV}\".\"OITM\" SET \"U_act\" = '" + vac + "' WHERE \"ItemCode\" =  '" + En_DN + "'"; //Query Actualiza Entry_fac
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                }
                                else
                                {
                                    objRegistraLog.Graba("Error insertando SC: " + RR + " " + DocEntry + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                                    Inserterror(bd.Substring(6, 6), "OPRQ", Convert.ToString(DocEntry));
                                }

                                
                            }
                        }
                    }
                    conn.Close();
                    LiberaMemoria.FlushMemory();
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al intentar Insertar SC: " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
            return R;
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
