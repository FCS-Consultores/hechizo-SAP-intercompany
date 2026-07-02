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
using MCH_IC.Tools;
using SAPbobsCOM;
using Inter_Hechiz.tools;
using Inter_Hechiz.Tools;
using System.Text;
using System.IO.Compression;
using Newtonsoft.Json;

namespace Inter_Hechiz.Data
{
    public class Intercompany
    {
        static RegistroLog objRegistraLog = new RegistroLog(); //Log en caso de error
        private static CookieCollection cookieData = new CookieCollection();
        static Marketing ObjSap = new Marketing();
        static string RR = "";
        public static string RUTAFINAL;
        public static string bdpublica;

        public static void SINCROERROR()
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
                        string trunca = $"TRUNCATE TABLE  \"{Conexion.BD_Hechizo}\".\"@SINCROERROR\"";
                        OdbcCommand db1 = new OdbcCommand(trunca, conn);
                        conn.Open();
                        db1.ExecuteNonQuery();
                        conn.Close();
                        objRegistraLog.Graba("ok  SINCROERROR : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error  
                    }

                    LiberaMemoria.FlushMemory();
                }
                catch (Exception)
                {
                    objRegistraLog.Graba("Error SINCROERROR : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error        
                }

            }
        }

        public static void SINCROERROR_V2()
        {
            try
            {
                using (var conn = new OdbcConnection(Conexion.strCon))
                {
                    conn.Open();
                    new OdbcCommand($"CALL {Conexion.BD_Hechizo}.SP_LIMPIA_SINCROERROR()", conn)
                        .ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba($"[{DateTime.Now.ToString("hh-mm-ss")}] Error SINCROERROR: " + ex.Message);
            }
        }

        /// <summary>
        /// crea el nombre de fabrica de Hechizo hacia las demás sucursales
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <param name="Tokendestino"></param>
        public static void Fabricante(string bdDestino, string Tokendestino)
        {

            Respuesta R = new Respuesta();

            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {

                    string query = $"SELECT \"FirmName\" FROM  \"{Conexion.BD_Hechizo}\".\"OMRC\" where \"FirmName\" NOT IN (SELECT  \"FirmName\" FROM \"{bdDestino}\".\"OMRC\")";
                    OdbcCommand CmD = new OdbcCommand(query, conn); // llama al Query Encabezado con la conexion
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {

                            while (dr.Read())
                            {

                                JObject JsonObject = new JObject();
                                JsonObject.Add(new JProperty("ManufacturerName", dr["FirmName"].ToString()));

                                CrearFabricanteTODAS(bdDestino, JsonObject, Tokendestino);
                            }
                        }

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

        /// <summary>
        /// Crea los gupos de articulos de Hechizo en las demás sucursales
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <param name="TokenDestino"></param>
        public static void GrupoArt(string bdDestino, string TokenDestino)
        {

            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {

                    string query = $"SELECT  \"ItmsGrpNam\" FROM  \"{Conexion.BD_Hechizo}\".\"OITB\" where \"ItmsGrpNam\" NOT IN (SELECT  \"ItmsGrpNam\" FROM \"{bdDestino}\".\"OITB\")";
                    OdbcCommand CmD = new OdbcCommand(query, conn);
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {

                            while (dr.Read())
                            {
                                JObject JsonObject = new JObject();
                                JsonObject.Add(new JProperty("GroupName", dr["ItmsGrpNam"].ToString()));

                                CreargrupoTODAS(bdDestino, JsonObject, TokenDestino);
                            }
                        }

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

        /// <summary>
        /// Envia a las demás sucursales los grupos de socios de negocios que tiene Hechizo
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <param name="TokenDestino"></param>
        public static void GrupoSN(string bdDestino, string TokenDestino)
        {


            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {

                    string queryEnCRSV = $"SELECT  \"GroupName\",\"GroupType\"  FROM  \"{Conexion.BD_Hechizo}\".\"OCRG\" where \"GroupName\" not in (SELECT  \"GroupName\" FROM \"{bdDestino}\".\"OCRG\")"; //Query Encabezado
                    OdbcCommand CmD = new OdbcCommand(queryEnCRSV, conn); // llama al Query Encabezado con la conexion
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {

                            while (dr.Read())
                            {

                                JObject JsonObject = new JObject();
                                JsonObject.Add(new JProperty("Name", dr["GroupName"].ToString()));
                                JsonObject.Add(new JProperty("Type", dr["GroupType"].ToString()));

                                CreargrupoSNTODAS(bdDestino, JsonObject, TokenDestino);
                            }
                        }
                        conn.Close();
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

        /// <summary>
        /// Crea las codiciones de pago desde Hechizo a las demás sucursales
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <param name="TokenDestino"></param>
        public static void condicionespago(string bdDestino, string TokenDestino)
        {

            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {

                    string queryEnCRSV = $"SELECT  \"PymntGroup\",\"PayDuMonth\" ,\"ExtraDays\" FROM  \"{Conexion.BD_Hechizo}\".\"OCTG\" where \"PymntGroup\" not in (SELECT  \"PymntGroup\" FROM \"{bdDestino}\".\"OCTG\")";
                    OdbcCommand CmD = new OdbcCommand(queryEnCRSV, conn);
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {

                            while (dr.Read())
                            {

                                JObject JsonObject = new JObject();
                                JsonObject.Add(new JProperty("PaymentTermsGroupName", dr["PymntGroup"].ToString()));
                                JsonObject.Add(new JProperty("StartFrom", dr["PayDuMonth"].ToString()));
                                JsonObject.Add(new JProperty("NumberOfAdditionalDays", Convert.ToInt32(dr["ExtraDays"])));

                                CrearcondicionTODAS(bdDestino, JsonObject, TokenDestino);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error condicionespago : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
        }

        #region Creación - actualizacion de docummentos

        /// <summary>
        /// Procesa los pagos efectuados en pagos recibido entre sociedades
        /// </summary>
        /// <param name="bdOrigen"></param>
        /// <param name="bdDestino"></param>
        /// <param name="tokenDestino"></param>
        public static void ProcesarEnPagoRecibido(string bdOrigen, string bdDestino, string tokenDestino)
        {

            string QueryEn = string.Empty;
            string QueryDet = string.Empty;
            int resul = 0;

            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                QueryEn = $@"SELECT 
                          ""DocEntry""  AS ""U_DocEntryOrigen""
                         ,""DocNum""    AS ""U_DocNumOrigen""
                         ,""TrsfrSum""  AS ""TransferSum""
 	                     ,""TrsfrRef""  AS ""TransferReference""
 	                     ,""TrsfrAcct"" AS ""TransferAccount""
                         ,""TrsfrDate"" AS ""TransferDate""
                         ,""CounterRef"" AS ""CounterReference""
                         ,""DocRate"" 
                         ,""DocType""
 	                     ,""DocDate"" 
 	                     ,""DocDueDate"" AS ""DueDate""
 	                     ,""TaxDate""
 	                     ,""DocCurr""   AS ""DocCurrency""
 	                     ,""Comments""  AS ""Remarks""
	                     ,""JrnlMemo""  AS ""JournalRemarks""
                         ,""U_OrigenIntercompany""
	                     ,""U_RefBancaria""
                         ,""U_BancoDestino""
                         FROM {bdOrigen}.OVPM WHERE ""Canceled"" = 'N'  AND ""U_Estatus"" = 'N' AND ""U_Intercompany"" = 'Y' AND ""U_DestinoIntercompany"" = '{bdDestino}' ";


                QueryDet = $@"SELECT 
                           ""AcctCode"" AS ""AccountCode""
                          ,""Descrip""  AS ""Decription""
                          ,""AppliedFC"" AS ""SumPaidFC""
                          ,""SumApplied"" AS ""SumPaid""
                          ,""GrossAmnt"" AS ""GrossAmount""
                          FROM {bdOrigen}.VPM4 WHERE ""DocNum"" = ? ";


                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    OdbcCommand CmD = new OdbcCommand(QueryEn, conn);

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            while (dr.Read())
                            {
                                JObject JsonObject = new JObject();

                                int En_Entry = Convert.ToInt32(dr["U_DocEntryOrigen"]);
                                int DocNum = Convert.ToInt32(dr["U_DocNumOrigen"]);
                                string Moneda = dr["DocCurrency"].ToString();
                                decimal TpCambio = Math.Round(Convert.ToDecimal(dr["DocRate"]), 2);
                                decimal montoTarjeta = Convert.ToDecimal(dr["TransferSum"]);

                                JsonObject = CrearObjetoJson(dr);

                                JsonObject["U_OrigenIntercompany"] = bdOrigen;
                                JsonObject["DocType"] = "rAccount";
                                JsonObject["TransferAccount"] = dr["U_BancoDestino"].ToString();
                                JsonObject.Remove("U_BancoDestino");
                                JsonObject["TransferReference"] = dr["U_RefBancaria"].ToString();
                                JsonObject.Remove("U_RefBancaria");

                                if (Moneda == "USD")
                                {
                                    JsonObject["TransferSum"] = montoTarjeta / TpCambio;
                                }

                                JArray JArrayPagoCuenta = new JArray();

                                OdbcCommand cmc = new OdbcCommand(QueryDet, conn);
                                cmc.Parameters.Add(new OdbcParameter("@num", En_Entry));

                                using (OdbcDataReader dtr = cmc.ExecuteReader())
                                {
                                    JArrayPagoCuenta = CrearObjetoJson2(dtr);
                                }

                                foreach (var item in JArrayPagoCuenta)
                                {
                                    if (Moneda == "USD" && item["SumPaidFC"] != null)
                                    {
                                        var obj = (JObject)item;
                                        obj["GrossAmount"] = obj["SumPaidFC"];
                                        obj["SumPaid"] = obj["SumPaidFC"];
                                    }
                                }

                                JsonObject.Add(new JProperty("PaymentAccounts", JArrayPagoCuenta));

                                var myjs = JsonObject.ToString();
                                resul = EnviaSapPorBasicAuth("POST", JsonObject, UrlServiceLayer + "IncomingPayments", tokenDestino);

                                if (resul == 0) // Caso que inserto
                                {
                                    string query = $"UPDATE  \"{bdOrigen}\".\"OVPM\" SET \"U_Estatus\" = 'Y', \"U_DocNumDestino\" = {ObjSap.DocNum}, \"U_DocEntryDestino\" = {ObjSap.DocEntry}  WHERE \"DocEntry\" = {En_Entry} ";
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                }
                                else
                                {
                                    string query = $"UPDATE  \"{bdOrigen}\".\"OVPM\" SET \"U_ComentarioError\" = '{RR.Replace("'", "")}',  \"U_Estatus\" = 'Y' WHERE \"DocEntry\" = {En_Entry}";
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();

                                    objRegistraLog.Graba($"Error insertando el pago docNum {DocNum}, BD destino {bdDestino}: " + RR.Replace("'", "") + " " + DocNum + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));//
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error al ProcesarPago BdOrig {bdOrigen} bdDestino {bdDestino}, {ex.Message}");
            }
        }

        /// <summary>
        /// Procesa solicitudes de compra de sucursales a Hechizo
        /// </summary>
        /// <param name="bdOrigen"></param>
        /// <param name="bdDestino"></param>
        /// <param name="tokenDestino"></param>
        public static void ProcesarSolictComp(string bdOrigen, string tokenDestino)
        {
            string BdHechizo = string.Empty;
            string BdOrig = string.Empty;

            string QueryEn = string.Empty;
            string QueryDet = string.Empty;

            int resul = 0;

            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                BdOrig = ConsultarBD(bdOrigen);

                BdHechizo = ConsultarBD("ORIGEN");

                QueryEn = $@"SELECT 
                          S.""DocEntry""  AS ""U_DocEntryOrigen""
                         ,S.""DocNum""    AS ""U_DocNumOrigen""
                         ,S.""ReqType""   AS ""ReqType""
                         ,E.""U_Code"" AS ""Requester""
                         ,S.""DocDate""
                         ,S.""ReqDate""   AS ""RequriedDate""
                         ,S.""TaxDate""
                         ,S.""Comments"" 
                         ,S.""U_OrigenIntercompany""
                         FROM {BdOrig}.OPRQ S
                         JOIN ""{BdOrig}"".OHEM E ON E.""empID"" = S.""Requester""
                         WHERE S.""CANCELED"" = 'N' AND S.""DocStatus"" = 'O' AND S.""U_Estado"" = 'N' AND S.""ReqType"" = 171 ";


                QueryDet = $@"SELECT 
                            ""ItemCode""
                           ,""U_CantOV"" 
                           ,""Quantity""
                           ,""WhsCode""  AS ""WarehouseCode""
                           ,""TaxCode""
                           ,""PQTReqDate"" AS ""RequiredDate""
                            FROM {BdOrig}.PRQ1 
                            WHERE ""DocEntry"" = ? ";

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    OdbcCommand CmD = new OdbcCommand(QueryEn, conn);

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            while (dr.Read())
                            {
                                JObject JsonObject = new JObject();

                                int En_Entry = Convert.ToInt32(dr["U_DocEntryOrigen"]);
                                int DocNum = Convert.ToInt32(dr["U_DocNumOrigen"]);

                                JsonObject = CrearObjetoJson(dr);
                                JsonObject["U_OrigenIntercompany"] = BdOrig;
                                //JsonObject["Requester"] = 3; CAMBIAR
                                JArray JArrayDetalles = new JArray();

                                OdbcCommand cmc = new OdbcCommand(QueryDet, conn);
                                cmc.Parameters.Add(new OdbcParameter("@num", En_Entry));

                                using (OdbcDataReader dtr = cmc.ExecuteReader())
                                {
                                    JArrayDetalles = CrearObjetoJson2(dtr);
                                }

                                foreach (var item in JArrayDetalles)
                                {
                                    var obj = (JObject)item;
                                    item["WarehouseCode"] = "01";

                                }

                                JsonObject.Add(new JProperty("DocumentLines", JArrayDetalles));

                                var myjs = JsonObject.ToString();
                                resul = EnviaSapPorBasicAuth("POST", JsonObject, UrlServiceLayer + "PurchaseRequests", tokenDestino);

                                if (resul == 0) // Caso que inserto
                                {

                                    string query = $"UPDATE  \"{BdOrig}\".\"OPRQ\" SET \"U_Estado\" = 'Y', \"U_DocNumDestino\" = {ObjSap.DocNum}, \"U_DocEntryDestino\" = {ObjSap.DocEntry}  WHERE \"DocEntry\" = {En_Entry} ";
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();

                                }
                                else
                                {
                                    string query = $"UPDATE  \"{BdOrig}\".\"OPRQ\" SET \"U_ComentarioError\" = '{RR.Replace("'", "")}',  \"U_Estado\" = 'Y' WHERE \"DocEntry\" = {En_Entry}";
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();

                                    objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Error creando la solicCompra en Hechizo con el docNum {DocNum} de la sucursarl de origen {BdOrig}: " + RR.Replace("'", ""));

                                }

                            }

                        }
                    }

                }

            }
            catch (Exception ex)
            {

                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Crendo la solicCompra, {ex.Message}");
            }
        }

        /// <summary>
        /// Procesa las solcitudes de Compra de Hechizo en ordenes de venta en la BD de Hechizo
        /// </summary>
        /// <param name="tokenDestino"></param>
        public static void ProcesarEnOrdVentaHechizo(string tokenDestino)
        {
            string BdHechizo = string.Empty;
            string QueryEn = string.Empty;
            string QueryDet = string.Empty;

            int resul = 0;

            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                BdHechizo = ConsultarBD("ORIGEN");


                QueryEn = $@"SELECT 
                          ""DocEntry""  AS ""U_DocEntryOrigen""
                         ,""DocNum""    AS ""U_DocNumOrigen""
                         ,""CardCode""
                         ,""Comments""
                         ,""DocDate""
                         ,""DocDueDate""
                         ,""TaxDate""
                         ,""DocCur""
                         ,""DocRate""
                         ,""GroupNum"" AS ""PaymentGroupCode""
                         ,""SlpCode"" AS ""SalesPersonCode""
                         ,""TrnspCode"" AS ""TransportationCode""
                         ,""U_OrigenIntercompany""
                         FROM {BdHechizo}.OPRQ WHERE ""CANCELED"" = 'N'  AND ""DocStatus"" = 'O' AND ""U_ConvertirOV"" = 'Y' AND ""U_OrigenIntercompany"" IS NOT NULL AND ""U_Estado""  = 'N'";

                QueryDet = $@"SELECT 
                            ""ItemCode""
                           ,""U_CantOV"" AS""Quantity""
                           ,""TaxCode""
                          FROM {BdHechizo}.PRQ1 WHERE ""DocEntry"" = ? ";

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    OdbcCommand CmD = new OdbcCommand(QueryEn, conn);

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            while (dr.Read())
                            {
                                JObject JsonObject = new JObject();

                                int En_Entry = Convert.ToInt32(dr["U_DocEntryOrigen"]);
                                int DocNum = Convert.ToInt32(dr["U_DocNumOrigen"]);
                                string bdOrigen = dr["U_OrigenIntercompany"].ToString();

                                JsonObject = CrearObjetoJson(dr);
                                JsonObject["CardCode"] = ConsultarCardCode(bdOrigen);
                                JArray JArrayPagoCuenta = new JArray();

                                OdbcCommand cmc = new OdbcCommand(QueryDet, conn);
                                cmc.Parameters.Add(new OdbcParameter("@num", En_Entry));

                                using (OdbcDataReader dtr = cmc.ExecuteReader())
                                {
                                    JArrayPagoCuenta = CrearObjetoJson2(dtr);
                                }

                                JsonObject.Add(new JProperty("DocumentLines", JArrayPagoCuenta));

                                var myjs = JsonObject.ToString();
                                resul = EnviaSapPorBasicAuth("POST", JsonObject, UrlServiceLayer + "Orders", tokenDestino);

                                if (resul == 0) // Caso que inserto
                                {
                                    string query = $"UPDATE  \"{BdHechizo}\".\"OPRQ\" SET \"U_Estado\" = 'Y', \"U_DocNumDestino\" = {ObjSap.DocNum}, \"U_DocEntryDestino\" = {ObjSap.DocEntry}, \"U_Intercompany\" = 'Y'  WHERE \"DocEntry\" = {En_Entry} ";
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();

                                    #region Cancelar documento origen(Solicitud) despúes  de su creación como orden de venta

                                    int Close = EnviaSapPorBasicAuth("POST", JsonObject, UrlServiceLayer + $"PurchaseRequests({En_Entry})/Close", tokenDestino);

                                    if (Close != 0)
                                    {
                                        string queryCanceled = $"UPDATE  \"{BdHechizo}\".\"OPRQ\" SET \"U_ComentarioError\" = 'No fue posible Cancelar el documento {RR.Replace("'", "")}', \"U_Estado\" = 'Y', \"U_Intercompany\" = 'Y' WHERE \"DocEntry\" = {En_Entry}";
                                        OdbcCommand cmandd = new OdbcCommand(queryCanceled, conn);
                                        OdbcDataReader drr1 = cmand.ExecuteReader();
                                    }

                                    #endregion

                                }
                                else
                                {
                                    string query = $"UPDATE  \"{BdHechizo}\".\"OPRQ\" SET \"U_ComentarioError\" = '{RR.Replace("'", "")}', \"U_Estado\" = 'Y', \"U_Intercompany\" = 'Y' WHERE \"DocEntry\" = {En_Entry}";
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();

                                    objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Error Crendo la OrdVenta con el docNum {DocNum}: " + RR.Replace("'", ""));

                                }

                            }

                        }
                    }

                }

            }
            catch (Exception ex)
            {

                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error al OrdVenta, {ex.Message}");
            }
        }

        /// <summary>
        /// Procesa las Ordenes de ventas de Hechizo y las convierte en Ordenes de compra a las diferentes sucursales tomando como documento base sus Solicitud de compra de la misma sucursal destino,
        /// /// no todas las ordenes de venta vienen relacionadas, por ende se usa el cardcode para saber a qué base va el destino
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <param name="tokenDestino"></param>
        public static void ProcesarEnOrdCompra(string bdDestino, string tokenDestino)
        {
            string BdHechizo = string.Empty;
            string BdDest = string.Empty;
            string QueryFactProveEn = string.Empty; ///consulta por las facturas relacionadas 
            string QueryOrdComp = string.Empty; // consulta por las ordenes de compra que se convertiran en facturas de proveedor
            string QueryDetComp = string.Empty;
            int DocEntryProcesar = 0;
            bool StatusDoc = false;
            string tablaConsulta1 = string.Empty;
            string tablaConsulta2 = string.Empty;
            string BDConsulta = string.Empty;
            int DocEntryDetalle = 0;
            string BDConsulta2 = string.Empty;

            int resul = 0;

            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                BdHechizo = ConsultarBD("ORIGEN");
                BdDest = ConsultarBD(bdDestino);
                string CardCode = ConsultarCardCode(bdDestino.ToUpper());

                QueryFactProveEn = $@"SELECT 
                                     ""DocEntry"" 
                                     ,""DocNum""   
                                     ,""U_DocEntryOrigen""
                                     ,""U_OrigenIntercompany""
                                     FROM ""{BdHechizo}"".ORDR 
                                     WHERE ""CANCELED"" = 'N'  AND ""DocStatus"" = 'O'  AND  ""U_Estado"" = 'N' AND ( ""U_OrigenIntercompany"" = '{BdDest}' OR ""CardCode""  = '{CardCode}') ";


                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    OdbcCommand CmD = new OdbcCommand(QueryFactProveEn, conn);

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            while (dr.Read())
                            {
                                try
                                {

                                    JObject JsonObject = new JObject();

                                    int En_Entry = Convert.ToInt32(dr["DocEntry"]);

                                    int DocNum = Convert.ToInt32(dr["DocNum"]);

                                    string bdOrig = dr["U_OrigenIntercompany"].ToString();

                                    int EntrySolitudHechizo = dr.IsDBNull(dr.GetOrdinal("U_DocEntryOrigen")) ? 0 : dr.GetInt32(dr.GetOrdinal("U_DocEntryOrigen"));

                                    if (!string.IsNullOrWhiteSpace(bdOrig))//vienen de un documento relacionado se debe de consultar la info de su documento base
                                    {
                                        DocEntryProcesar = ConsultarDocEntrySolicitudCompra(conn, EntrySolitudHechizo, BdHechizo, "OPRQ");
                                        StatusDoc = ConsultarStatusDoc(conn, DocEntryProcesar, BdDest, "OPRQ");

                                        tablaConsulta1 = "ORDR";//OPRQ
                                        tablaConsulta2 = "RDR1";
                                        BDConsulta = BdHechizo;//BdDest
                                        BDConsulta2 = BdHechizo;
                                        DocEntryDetalle = En_Entry;
                                    }
                                    else
                                    {
                                        DocEntryProcesar = En_Entry;
                                        StatusDoc = true;
                                        tablaConsulta1 = "ORDR";
                                        tablaConsulta2 = "RDR1";
                                        BDConsulta = BdHechizo;
                                        BDConsulta2 = BdHechizo;
                                        DocEntryDetalle = En_Entry;
                                    }

                                    QueryOrdComp = $@"SELECT 
                                                   ""DocEntry""  AS ""U_DocEntryOrigen""
                                                  ,""DocNum""    AS ""U_DocNumOrigen""
                                                  ,""CardCode""
                                                  ,""Comments""
                                                  ,""DocDate""
                                                  ,""DocDueDate""
                                                  ,""DiscPrcnt"" AS ""DiscountPercent""
                                                  ,""DocTotal""
                                                  ,""TaxDate""
                                                  ,""DocCur""
                                                  ,""DocRate""
                                                  ,""GroupNum"" AS ""PaymentGroupCode""                                                
                                                  ,""U_OrigenIntercompany""
                                                 FROM {BDConsulta}.{tablaConsulta1} 
                                                 EF WHERE ""CANCELED"" = 'N'  AND ""DocStatus"" = 'O' AND ""DocEntry"" = ? ";//""SlpCode"" AS ""SalesPersonCode""  ,""TrnspCode"" AS ""TransportationCode""



                                    QueryDetComp = $@"SELECT 
                                                     ""ItemCode""
                                                    ,""Quantity""                                                    
                                                    ,""TaxCode""
                                                    ,""LineTotal""
                                                    ,""BaseType""
                                                    ,""BaseEntry""
                                                    ,""LineNum""AS ""BaseLine""
                                                    ,""LineNum""
                                                    ,""Price"" AS ""UnitPrice""
                                                    FROM {BDConsulta2}.{tablaConsulta2} 
                                                    WHERE ""DocEntry"" = ? ";

                                    if (DocEntryProcesar != 0 && StatusDoc == true)
                                    {
                                        OdbcCommand cmcd = new OdbcCommand(QueryOrdComp, conn);
                                        cmcd.Parameters.Add(new OdbcParameter("@num", En_Entry));//DocEntryProcesar

                                        using (OdbcDataReader OrdComp = cmcd.ExecuteReader())
                                        {
                                            if (OrdComp.Read())
                                            {
                                                JsonObject = CrearObjetoJson(OrdComp);

                                                JsonObject["U_OrigenIntercompany"] = BdHechizo;
                                                JsonObject["U_DocEntryOrigen"] = En_Entry;
                                                JsonObject["U_DocNumOrigen"] = DocNum;
                                                JsonObject["CardCode"] = ConsultarCardCode(BdHechizo);
                                                JsonObject["U_OrigenIntercompany"] = BdHechizo;

                                            }
                                            else
                                            {
                                                continue;
                                            }

                                        }

                                        JArray JArrayDetalle = new JArray();

                                        OdbcCommand cmc = new OdbcCommand(QueryDetComp, conn);
                                        cmc.Parameters.Add(new OdbcParameter("@num", DocEntryDetalle));

                                        using (OdbcDataReader dtr = cmc.ExecuteReader())
                                        {
                                            JArrayDetalle = CrearObjetoJson2(dtr);
                                        }

                                        foreach (var item in JArrayDetalle)
                                        {
                                            var obj = (JObject)item;
                                            if (!string.IsNullOrWhiteSpace(bdOrig))// vienen de un documento relacionado se debe colocar su relación
                                            {
                                                item["BaseType"] = 1470000113;
                                                item["BaseEntry"] = DocEntryProcesar;
                                            }
                                            else
                                            {
                                                obj.Remove("BaseType");
                                                obj.Remove("BaseEntry");
                                                obj.Remove("BaseLine");
                                            }

                                        }

                                        JsonObject.Add(new JProperty("DocumentLines", JArrayDetalle));

                                        var myjs = JsonObject.ToString();
                                        resul = EnviaSapPorBasicAuth("POST", JsonObject, UrlServiceLayer + "PurchaseOrders", tokenDestino);

                                        if (resul == 0) // Caso que inserto
                                        {
                                            string query = $"UPDATE  \"{BdHechizo}\".\"ORDR\" SET \"U_Estado\" = 'Y', \"U_DocNumDestino\" = {ObjSap.DocNum}, \"U_DocEntryDestino\" = {ObjSap.DocEntry}  WHERE \"DocEntry\" = {En_Entry} ";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();


                                            string jsComprimido = CompactJson(myjs);
                                            objRegistraLog.GrabaJson("");
                                            objRegistraLog.GrabaJson($"[{DateTime.Now.ToString("HH:mm:ss tt")}] BDOrigen: {BdHechizo} DEntryOrg {En_Entry}, DNumOrg {DocNum} / BDDestino {BdDest}, DEntryDest {ObjSap.DocEntry} / Json: {jsComprimido} ");
                                        }
                                        else
                                        {
                                            string query = $"UPDATE  \"{BdHechizo}\".\"ORDR\" SET \"U_ComentarioError\" = '{RR.Replace("'", "")}', \"U_Estado\" = 'Y' WHERE \"DocEntry\" = {En_Entry}";
                                            //OdbcCommand cmand = new OdbcCommand(query, conn);
                                            //OdbcDataReader dr1 = cmand.ExecuteReader();

                                            //objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Error creando la Factura de proveedor en {BdDest} con el docNum factura deudor {DocNum}: " + RR.Replace("'", ""));
                                        }
                                    }
                                    else
                                    {

                                        string query = $"UPDATE  \"{BdHechizo}\".\"ORDR\" SET \"U_ComentarioError\" = 'No fue posible crear su Orden de Compra en {BdDest} ya sea porque no se encontró el docentry relacionado: {DocEntryProcesar} o el documento relacionado esta cerrado o cancelado', \"U_Estado\" = 'Y' WHERE \"DocEntry\" = {En_Entry}";
                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                        OdbcDataReader dr1 = cmand.ExecuteReader();

                                        objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] No fue posible crear su  Orden de Compra en {BdDest} ya sea porque no se encontró el docentry relacionado: {DocEntryProcesar}  o el documento relacionado esta cerrado o cancelado");
                                    }

                                }
                                catch (Exception ex)
                                {
                                    objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error al procesar las OrdCompra en bd destino {BdDest}, {ex.Message}");
                                }

                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error en Orden Compra, {ex.Message}");
            }
        }

        /// <summary>
        /// Procesa las Ordenes de ventas de Hechizo y las convierte en Ordenes de compra a las diferentes sucursales tomando como documento base sus Solicitud de compra de la misma sucursal destino,
        /// /// no todas las ordenes de venta vienen relacionadas, por ende se usa el cardcode para saber a qué base va el destino. Se agrega un log para ver si los precios que se van enviar sean los mismo que llegaron
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <param name="tokenDestino"></param>
        public static void ProcesarEnOrdCompraV2(string bdDestino, string tokenDestino)
        {
            string BdHechizo = string.Empty;
            string BdDest = string.Empty;
            string QueryFactProveEn = string.Empty; ///consulta por las facturas relacionadas 
            string QueryOrdComp = string.Empty; // consulta por las ordenes de compra que se convertiran en facturas de proveedor
            string QueryDetComp = string.Empty;
            int DocEntryProcesar = 0;
            bool StatusDoc = false;
            string tablaConsulta1 = string.Empty;
            string tablaConsulta2 = string.Empty;
            string BDConsulta = string.Empty;
            int DocEntryDetalle = 0;
            string BDConsulta2 = string.Empty;

            int resul = 0;

            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                BdHechizo = ConsultarBD("ORIGEN");
                BdDest = ConsultarBD(bdDestino);
                string CardCode = ConsultarCardCode(bdDestino.ToUpper());

                QueryFactProveEn = $@"SELECT 
                                     ""DocEntry"" 
                                     ,""DocNum""   
                                     ,""U_DocEntryOrigen""
                                     ,""U_OrigenIntercompany""
                                     FROM ""{BdHechizo}"".ORDR 
                                     WHERE ""CANCELED"" = 'N'  AND ""DocStatus"" = 'O'  AND  ""U_Estado"" = 'N' AND ( ""U_OrigenIntercompany"" = '{BdDest}' OR ""CardCode""  = '{CardCode}') ";


                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    OdbcCommand CmD = new OdbcCommand(QueryFactProveEn, conn);

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            while (dr.Read())
                            {
                                try
                                {

                                    JObject JsonObject = new JObject();

                                    int En_Entry = Convert.ToInt32(dr["DocEntry"]);

                                    int DocNum = Convert.ToInt32(dr["DocNum"]);

                                    string bdOrig = dr["U_OrigenIntercompany"].ToString();

                                    int EntrySolitudHechizo = dr.IsDBNull(dr.GetOrdinal("U_DocEntryOrigen")) ? 0 : dr.GetInt32(dr.GetOrdinal("U_DocEntryOrigen"));

                                    if (!string.IsNullOrWhiteSpace(bdOrig))//vienen de un documento relacionado se debe de consultar la info de su documento base
                                    {
                                        DocEntryProcesar = ConsultarDocEntrySolicitudCompra(conn, EntrySolitudHechizo, BdHechizo, "OPRQ");
                                        StatusDoc = ConsultarStatusDoc(conn, DocEntryProcesar, BdDest, "OPRQ");

                                        tablaConsulta1 = "ORDR";//OPRQ
                                        tablaConsulta2 = "RDR1";
                                        BDConsulta = BdHechizo;//BdDest
                                        BDConsulta2 = BdHechizo;
                                        DocEntryDetalle = En_Entry;
                                    }
                                    else
                                    {
                                        DocEntryProcesar = En_Entry;
                                        StatusDoc = true;
                                        tablaConsulta1 = "ORDR";
                                        tablaConsulta2 = "RDR1";
                                        BDConsulta = BdHechizo;
                                        BDConsulta2 = BdHechizo;
                                        DocEntryDetalle = En_Entry;
                                    }

                                    QueryOrdComp = $@"SELECT 
                                                   ""DocEntry""  AS ""U_DocEntryOrigen""
                                                  ,""DocNum""    AS ""U_DocNumOrigen""
                                                  ,""CardCode""
                                                  ,""Comments""
                                                  ,""DocDate""
                                                  ,""DocDueDate""
                                                  ,""DiscPrcnt"" AS ""DiscountPercent""
                                                  ,""DocTotal""
                                                  ,""TaxDate""
                                                  ,""DocCur""
                                                  ,""DocRate""
                                                  ,""GroupNum"" AS ""PaymentGroupCode""                                                
                                                  ,""U_OrigenIntercompany""
                                                 FROM {BDConsulta}.{tablaConsulta1} 
                                                 EF WHERE ""CANCELED"" = 'N'  AND ""DocStatus"" = 'O' AND ""DocEntry"" = ? ";//""SlpCode"" AS ""SalesPersonCode""  ,""TrnspCode"" AS ""TransportationCode""



                                    QueryDetComp = $@"SELECT 
                                                     ""ItemCode""
                                                    ,""Quantity""                                                    
                                                    ,""TaxCode""
                                                    ,""LineTotal""
                                                    ,""BaseType""
                                                    ,""BaseEntry""
                                                    ,""LineNum""AS ""BaseLine""
                                                    ,""LineNum""
                                                    ,""Price"" AS ""UnitPrice""
                                                    FROM {BDConsulta2}.{tablaConsulta2} 
                                                    WHERE ""DocEntry"" = ? ";

                                    if (DocEntryProcesar != 0 && StatusDoc == true)
                                    {
                                        OdbcCommand cmcd = new OdbcCommand(QueryOrdComp, conn);
                                        cmcd.Parameters.Add(new OdbcParameter("@num", En_Entry));//DocEntryProcesar

                                        using (OdbcDataReader OrdComp = cmcd.ExecuteReader())
                                        {
                                            if (OrdComp.Read())
                                            {
                                                JsonObject = CrearObjetoJson(OrdComp);

                                                JsonObject["U_OrigenIntercompany"] = BdHechizo;
                                                JsonObject["U_DocEntryOrigen"] = En_Entry;
                                                JsonObject["U_DocNumOrigen"] = DocNum;
                                                JsonObject["CardCode"] = ConsultarCardCode(BdHechizo);
                                                JsonObject["U_OrigenIntercompany"] = BdHechizo;

                                            }
                                            else
                                            {
                                                continue;
                                            }

                                        }

                                        JArray JArrayDetalle = new JArray();

                                        OdbcCommand cmc = new OdbcCommand(QueryDetComp, conn);
                                        cmc.Parameters.Add(new OdbcParameter("@num", DocEntryDetalle));

                                        using (OdbcDataReader dtr = cmc.ExecuteReader())
                                        {
                                            JArrayDetalle = CrearObjetoJson2(dtr);
                                        }

                                        foreach (var item in JArrayDetalle)
                                        {
                                            var obj = (JObject)item;
                                            if (!string.IsNullOrWhiteSpace(bdOrig))// vienen de un documento relacionado se debe colocar su relación
                                            {
                                                item["BaseType"] = 1470000113;
                                                item["BaseEntry"] = DocEntryProcesar;
                                            }
                                            else
                                            {
                                                obj.Remove("BaseType");
                                                obj.Remove("BaseEntry");
                                                obj.Remove("BaseLine");
                                            }

                                        }

                                        JsonObject.Add(new JProperty("DocumentLines", JArrayDetalle));

                                        var myjs = JsonObject.ToString();
                                        resul = EnviaSapPorBasicAuth("POST", JsonObject, UrlServiceLayer + "PurchaseOrders", tokenDestino);

                                        if (resul == 0) // Caso que inserto
                                        {
                                            // Comparación de precios contra BD destino
                                            ComparePricesAndLog(ObjSap.DocEntry, En_Entry, JArrayDetalle, ObjSap.DocumentLines, BdHechizo, BdDest);

                                            string query = $"UPDATE  \"{BdHechizo}\".\"ORDR\" SET \"U_Estado\" = 'Y', \"U_DocNumDestino\" = {ObjSap.DocNum}, \"U_DocEntryDestino\" = {ObjSap.DocEntry}  WHERE \"DocEntry\" = {En_Entry} ";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();
                                        }
                                        else
                                        {
                                            string query = $"UPDATE  \"{BdHechizo}\".\"ORDR\" SET \"U_ComentarioError\" = '{RR.Replace("'", "")}', \"U_Estado\" = 'Y' WHERE \"DocEntry\" = {En_Entry}";
                                        }
                                    }
                                    else
                                    {
                                        string query = $"UPDATE  \"{BdHechizo}\".\"ORDR\" SET \"U_ComentarioError\" = 'No fue posible crear su Orden de Compra en {BdDest} ya sea porque no se encontró el docentry relacionado: {DocEntryProcesar} o el documento relacionado esta cerrado o cancelado', \"U_Estado\" = 'Y' WHERE \"DocEntry\" = {En_Entry}";
                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                        OdbcDataReader dr1 = cmand.ExecuteReader();

                                        objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] No fue posible crear su  Orden de Compra en {BdDest} ya sea porque no se encontró el docentry relacionado: {DocEntryProcesar}  o el documento relacionado esta cerrado o cancelado");
                                    }

                                }
                                catch (Exception ex)
                                {
                                    objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error al procesar las OrdCompra en bd destino {BdDest}, {ex.Message}");
                                }

                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error en Orden Compra, {ex.Message}");
            }
        }

        private static string CompactJson(string json)
        {
            var jsonObject = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(jsonObject, Formatting.None);
        }

        /// <summary>
        /// Procesa las Facturas de proveedor en las sucursales que salen de Hechizo
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <param name="tokenDestino"></param>
        public static void ProcesarFacturaProveEnSucursales(string bdDestino, string tokenDestino)
        {
            string BdHechizo = string.Empty;
            string BdDest = string.Empty;
            string QueryFactProveEn = string.Empty; ///consulta por las facturas relacionadas 
            string QueryOrdComp = string.Empty; // consulta por las ordenes de compra que se convertiran en facturas de proveedor
            string QueryDetComp = string.Empty;
            string CardCode = string.Empty;

            int resul = 0;

            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                BdHechizo = ConsultarBD("ORIGEN");
                BdDest = ConsultarBD(bdDestino);
                CardCode = ConsultarCardCode(BdDest);

                QueryFactProveEn = $@"SELECT DISTINCT
                                     EF.""DocEntry"" 
                                     ,""DocNum""   
                                     ,DF.""BaseEntry""
                                     FROM {BdHechizo}.OINV EF
                                     JOIN {BdHechizo}.INV1 DF ON EF.""DocEntry"" = DF.""DocEntry"" 
                                     WHERE ""CANCELED"" = 'N'  AND ""DocStatus"" = 'O' AND  ""U_Estado"" = 'N' AND DF.""BaseType"" = 17 AND (""U_OrigenIntercompany"" = '{BdDest}' OR ""CardCode"" = ?)";

                QueryOrdComp = $@"SELECT 
                                  ""DocEntry""  AS ""U_DocEntryOrigen""
                                 ,""DocNum""    AS ""U_DocNumOrigen""
                                 ,""CardCode""
                                 ,""Comments""
                                 ,""DocDate""
                                 ,""DocDueDate""
                                 ,""TaxDate""
                                 ,""DiscPrcnt"" AS ""DiscountPercent""
                                 ,""DocTotal""
                                 ,""DocCur""
                                 ,""DocRate""
                                 ,""GroupNum"" AS ""PaymentGroupCode""
                                 ,""TrnspCode"" AS ""TransportationCode""
                                 ,""NumAtCard""
                                 ,""PayToCode""
                                 ,""Address""
                                 ,""U_OrigenIntercompany""
                                 FROM {BdDest}.OPOR 
                                 EF WHERE ""CANCELED"" = 'N'  AND ""DocStatus"" = 'O' AND ""DocEntry"" = ? ";// ,""SlpCode"" AS ""SalesPersonCode""

                QueryDetComp = $@"SELECT 
                                 ""ItemCode""
                                ,""Quantity""                               
                                ,""TaxCode""
                                ,""LineTotal""
                                ,""BaseType""
                                ,""BaseEntry""
                                ,""LineNum""AS ""BaseLine""
                                ,""PriceBefDi"" AS ""UnitPrice""
                                ,""Price"" AS ""Price""
                                FROM {BdDest}.POR1 WHERE ""DocEntry"" = ? ";

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {

                    OdbcCommand CmD = new OdbcCommand(QueryFactProveEn, conn);
                    CmD.Parameters.Add(new OdbcParameter("@num", CardCode));

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            while (dr.Read())
                            {
                                try
                                {

                                    JObject JsonObject = new JObject();

                                    int En_Entry = Convert.ToInt32(dr["DocEntry"]);
                                    int DocNum = Convert.ToInt32(dr["DocNum"]);
                                    int BaseEntry = Convert.ToInt32(dr["BaseEntry"]);

                                    int DOcEntryOrdCompra = ConsultarDocEntry(conn, BaseEntry, BdHechizo, "ORDR");

                                    bool StatusDoc = ConsultarStatusDoc(conn, DOcEntryOrdCompra, BdDest, "OPOR");

                                    if (DOcEntryOrdCompra != 0 && StatusDoc == true)
                                    {
                                        OdbcCommand cmcd = new OdbcCommand(QueryOrdComp, conn);
                                        cmcd.Parameters.Add(new OdbcParameter("@num", DOcEntryOrdCompra));

                                        using (OdbcDataReader OrdComp = cmcd.ExecuteReader())
                                        {
                                            if (OrdComp.Read())
                                            {
                                                JsonObject = CrearObjetoJson(OrdComp);

                                                JsonObject["U_OrigenIntercompany"] = BdHechizo;
                                                JsonObject["U_DocEntryOrigen"] = En_Entry;
                                                JsonObject["U_DocNumOrigen"] = DocNum;
                                                JsonObject["Series"] = ConsultarSeries(conn, "Primario", 18, BdDest, "--");
                                            }
                                            else
                                            {
                                                continue;
                                            }

                                        }

                                        JArray JArrayDetalle = new JArray();

                                        OdbcCommand cmc = new OdbcCommand(QueryDetComp, conn);
                                        cmc.Parameters.Add(new OdbcParameter("@num", DOcEntryOrdCompra));

                                        using (OdbcDataReader dtr = cmc.ExecuteReader())
                                        {

                                            JArrayDetalle = CrearObjetoJson2(dtr);
                                        }

                                        foreach (var item in JArrayDetalle)
                                        {
                                            var obj = (JObject)item;
                                            item["BaseType"] = 22;
                                            item["BaseEntry"] = DOcEntryOrdCompra;
                                        }

                                        JsonObject.Add(new JProperty("DocumentLines", JArrayDetalle));

                                        var myjs = JsonObject.ToString();
                                        resul = EnviaSapPorBasicAuth("POST", JsonObject, UrlServiceLayer + "PurchaseInvoices", tokenDestino);

                                        if (resul == 0) // Caso que inserto
                                        {
                                            string query = $"UPDATE  \"{BdHechizo}\".\"OINV\" SET \"U_Estado\" = 'Y', \"U_DocNumDestino\" = {ObjSap.DocNum}, \"U_DocEntryDestino\" = {ObjSap.DocEntry}  WHERE \"DocEntry\" = {En_Entry} ";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();
                                        }
                                        else
                                        {
                                            string query = $"UPDATE  \"{BdHechizo}\".\"OINV\" SET \"U_ComentarioError\" = '{RR.Replace("'", "")}', \"U_Estado\" = 'Y' WHERE \"DocEntry\" = {En_Entry}";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();

                                            objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Error creando la Factura de proveedor en {BdDest} con el docNum factura deudor {DocNum}: " + RR.Replace("'", ""));
                                        }
                                    }
                                    else
                                    {

                                        string query = $"UPDATE  \"{BdHechizo}\".\"OINV\" SET \"U_ComentarioError\" = 'No fue posible crear su Factura de proveedor en {BdDest} ya sea porque no se encontró el docentry relacionado: {DOcEntryOrdCompra} o el documento relacionado esta cerrado o cancelado', \"U_Estado\" = 'Y' WHERE \"DocEntry\" = {En_Entry}";
                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                        OdbcDataReader dr1 = cmand.ExecuteReader();

                                        objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] No fue posible crear su Factura de proveedor en {BdDest} ya sea porque no se encontró el docentry relacionado: {DOcEntryOrdCompra}  o el documento relacionado esta cerrado o cancelado");
                                    }

                                }
                                catch (Exception ex)
                                {

                                    objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error al procesar las facturas Provedor en bdDestino {BdDest}, {ex.Message}");
                                }

                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {
                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error al procesar las facturas Provedor en bdDestino, {ex.Message}");
            }
        }

        /// <summary>
        /// Procesa las Notas de Credito de proveedor en las sucursales que salen de Hechizo
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <param name="tokenDestino"></param>
        public static void ProcesarNotasCredProveEnSucursalesV1(string bdDestino, string tokenDestino)
        {
            string BdHechizo = string.Empty;
            string BdDest = string.Empty;
            string QueryFactProveEn = string.Empty; ///consulta por las facturas relacionadas 
            string QueryNC = string.Empty; // consulta por las ordenes de compra que se convertiran en facturas de proveedor
            string QueryDetNC = string.Empty; // En este caso consulta las lineas de el origen ya que pueden cambiar sus lineas y sus cantidades

            int resul = 0;

            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                BdHechizo = ConsultarBD("ORIGEN");
                BdDest = ConsultarBD(bdDestino);
                string CardCode = ConsultarCardCode(bdDestino.ToUpper());
                string Ccode = ConsultarCardCode("HECHIZO");
                QueryFactProveEn = $@"SELECT DISTINCT
                                     EF.""DocEntry"" 
                                     ,""DocNum""   
                                     ,DF.""BaseEntry""
                                     FROM {BdHechizo}.ORIN EF
                                     JOIN {BdHechizo}.RIN1 DF ON EF.""DocEntry"" = DF.""DocEntry"" 
                                     WHERE ""CANCELED"" = 'N' AND ""DocStatus"" = 'C' AND  ""U_Estado"" = 'N' AND DF.""BaseType"" = 13 AND ( ""U_OrigenIntercompany"" = '{BdDest}' OR ""CardCode""  = '{CardCode}') ";

                QueryNC = $@"SELECT 
                                  ""DocEntry""  AS ""U_DocEntryOrigen""
                                 ,""DocNum""    AS ""U_DocNumOrigen""
                                 ,""CardCode""
                                 ,""Comments""
                                 ,""DocDate""
                                 ,""DocDueDate""
                                 ,""TaxDate""
                                 ,""DocCur""
                                 ,""DocRate""
                                 ,""GroupNum"" AS ""PaymentGroupCode""
                                 ,""DiscPrcnt"" AS ""DiscountPercent""
                                 ,""U_OrigenIntercompany""
                                 FROM {BdHechizo}.ORIN 
                                 EF WHERE ""CANCELED"" = 'N'  AND ""DocStatus"" = 'C' AND ""DocEntry"" = ? ";//,""SlpCode"" AS ""SalesPersonCode"" // ,""TrnspCode"" AS ""TransportationCode""  ,""DocTotal""

                QueryDetNC = $@"SELECT 
                                 ""ItemCode""
                                ,""Quantity""                               
                                ,""TaxCode""
                                ,""WhsCode"" AS ""WarehouseCode""
                                ,""BaseType""
                                ,""BaseEntry""
                                ,""LineNum""AS ""BaseLine""
                                ,""PriceBefDi"" AS ""UnitPrice""
                                ,""Price"" AS ""Price""
                                FROM {BdHechizo}.RIN1 WHERE ""DocEntry"" = ? ";//,""LineTotal""

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    OdbcCommand CmD = new OdbcCommand(QueryFactProveEn, conn);

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            while (dr.Read())
                            {
                                try
                                {

                                    JObject JsonObject = new JObject();

                                    int En_Entry = Convert.ToInt32(dr["DocEntry"]);
                                    int DocNum = Convert.ToInt32(dr["DocNum"]);
                                    int BaseEntry = Convert.ToInt32(dr["BaseEntry"]);

                                    int DOcEntryFactProve = ConsultarDocEntry(conn, BaseEntry, BdHechizo, "OINV");
                                    bool StatusDoc = ConsultarStatusDoc(conn, DOcEntryFactProve, BdDest, "OPCH");

                                    if (DOcEntryFactProve != 0 && StatusDoc == true)
                                    {
                                        OdbcCommand cmcd = new OdbcCommand(QueryNC, conn);
                                        cmcd.Parameters.Add(new OdbcParameter("@num", En_Entry));//DOcEntryFactProve
                                        using (OdbcDataReader factProve = cmcd.ExecuteReader())
                                        {
                                            if (factProve.Read())
                                            {
                                                JsonObject = CrearObjetoJson(factProve);
                                                JsonObject["CardCode"] = Ccode;
                                                JsonObject["U_OrigenIntercompany"] = BdHechizo;
                                                JsonObject["U_DocEntryOrigen"] = En_Entry;
                                                JsonObject["U_DocNumOrigen"] = DocNum;
                                                JsonObject["Series"] = ConsultarSeries(conn, "Primario", 19, BdDest, "--");

                                            }
                                            else
                                            {
                                                continue;
                                            }

                                        }

                                        JArray JArrayDetalle = new JArray();

                                        OdbcCommand cmc = new OdbcCommand(QueryDetNC, conn);
                                        cmc.Parameters.Add(new OdbcParameter("@num", En_Entry));

                                        using (OdbcDataReader dtr = cmc.ExecuteReader())
                                        {

                                            JArrayDetalle = CrearObjetoJson2(dtr);
                                        }

                                        foreach (var item in JArrayDetalle)
                                        {
                                            var obj = (JObject)item;
                                            item["BaseType"] = 18;
                                            item["BaseEntry"] = DOcEntryFactProve;

                                        }

                                        JsonObject.Add(new JProperty("DocumentLines", JArrayDetalle));

                                        var myjs = JsonObject.ToString();
                                        resul = EnviaSapPorBasicAuth("POST", JsonObject, UrlServiceLayer + "PurchaseCreditNotes", tokenDestino);

                                        if (resul == 0) // Caso que inserto
                                        {
                                            string query = $"UPDATE  \"{BdHechizo}\".\"ORIN\" SET \"U_Estado\" = 'Y', \"U_DocNumDestino\" = {ObjSap.DocNum}, \"U_DocEntryDestino\" = {ObjSap.DocEntry}  WHERE \"DocEntry\" = {En_Entry} ";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();
                                        }
                                        else
                                        {
                                            string query = $"UPDATE  \"{BdHechizo}\".\"ORIN\" SET \"U_ComentarioError\" = '{RR.Replace("'", "")}', \"U_Estado\" = 'Y' WHERE \"DocEntry\" = {En_Entry}";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();

                                            objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Error creando la NC de proveedor en {BdDest} con el docNum factura deudor {DocNum}: " + RR.Replace("'", ""));
                                        }
                                    }
                                    else
                                    {

                                        string query = $"UPDATE  \"{BdHechizo}\".\"ORIN\" SET \"U_ComentarioError\" = 'No fue posible crear su NC de proveedor en {BdDest} ya sea porque no se encontró el docentry relacionado: {DOcEntryFactProve} o el documento relacionado esta cerrado o cancelado', \"U_Estado\" = 'Y' WHERE \"DocEntry\" = {En_Entry}";
                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                        OdbcDataReader dr1 = cmand.ExecuteReader();

                                        objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] No fue posible crear su NC de proveedor en {BdDest} ya sea porque no se encontró el docentry relacionado: {DOcEntryFactProve}  o el documento relacionado esta cerrado o cancelado");
                                    }

                                }
                                catch (Exception ex)
                                {

                                    objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}] Error al procesar las Notas de Credito, {ex.Message}");
                                }

                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error en NotasCredProve, {ex.Message}");
            }
        }

        public static void ProcesarNotasCredProveEnSucursales(string bdDestino, string tokenDestino)
        {
            string BdHechizo = string.Empty;
            string BdDest = string.Empty;
            string QueryFactProveEn = string.Empty; ///consulta por las facturas relacionadas 
            string QueryNC = string.Empty; // consulta por las ordenes de compra que se convertiran en facturas de proveedor
            string QueryDetNC = string.Empty; // En este caso consulta las lineas de el origen ya que pueden cambiar sus lineas y sus cantidades

            int resul = 0;

            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                BdHechizo = ConsultarBD("ORIGEN");
                BdDest = ConsultarBD(bdDestino);
                string sBD_destino;
                if (bdDestino == "desampa")
                {
                    sBD_destino = BdDest;
                }
                else
                {
                    sBD_destino = bdDestino;
                }

                string CardCode = ConsultarCardCode(sBD_destino);
                string Ccode = ConsultarCardCode("HECHIZO");
                QueryFactProveEn = $@"SELECT DISTINCT
                                     EF.""DocEntry"" 
                                     ,""DocNum""   
                                     ,DF.""BaseEntry""
                                     FROM {BdHechizo}.ORIN EF
                                     JOIN {BdHechizo}.RIN1 DF ON EF.""DocEntry"" = DF.""DocEntry"" 
                                     WHERE ""CANCELED"" = 'N' AND ""DocStatus"" = 'C' AND  ""U_Estado"" = 'N' AND DF.""BaseType"" = 13 AND ( ""U_OrigenIntercompany"" = '{BdDest}' OR ""CardCode""  = '{CardCode}') ";


                QueryNC = $@"SELECT 
                                  ""DocEntry""  AS ""U_DocEntryOrigen""
                                 ,""DocNum""    AS ""U_DocNumOrigen""
                                 ,""CardCode""
                                 ,""Comments""
                                 ,""DocDate""
                                 ,""DiscPrcnt"" AS ""DiscountPercent""
                                 ,""DocDueDate""
                                 ,""TaxDate""                                 
                                 ,""DocCur""
                                 ,""DocRate""
                                 ,""GroupNum"" AS ""PaymentGroupCode""
                                 ,""TrnspCode"" AS ""TransportationCode""
                                 ,""U_OrigenIntercompany""
                                 ,""NumAtCard"" AS ""NumAtCard""
                                 FROM {BdDest}.OPCH 
                                 EF WHERE ""CANCELED"" = 'N'  AND ""DocStatus"" = 'O' AND ""DocEntry"" = ? ";//,""SlpCode"" AS ""SalesPersonCode""

                QueryDetNC = $@"SELECT 
                                 ""ItemCode""
                                ,""Quantity""                               
                                ,""TaxCode""
                                ,""BaseType""
                                ,""BaseEntry""
                                ,""LineNum""AS ""BaseLine""
                                ,""PriceBefDi"" AS ""UnitPrice""
                                ,""Price"" AS ""Price""
                                FROM {BdHechizo}.RIN1 WHERE ""DocEntry"" = ? ";

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    OdbcCommand CmD = new OdbcCommand(QueryFactProveEn, conn);

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            while (dr.Read())
                            {
                                try
                                {

                                    JObject JsonObject = new JObject();

                                    int En_Entry = Convert.ToInt32(dr["DocEntry"]);
                                    int DocNum = Convert.ToInt32(dr["DocNum"]);
                                    int BaseEntry = Convert.ToInt32(dr["BaseEntry"]);

                                    int DOcEntryFactProve = ConsultarDocEntry(conn, BaseEntry, BdHechizo, "OINV");
                                    bool StatusDoc = ConsultarStatusDoc(conn, DOcEntryFactProve, BdDest, "OPCH");

                                    if (DOcEntryFactProve != 0 && StatusDoc == true)
                                    {
                                        OdbcCommand cmcd = new OdbcCommand(QueryNC, conn);
                                        cmcd.Parameters.Add(new OdbcParameter("@num", DOcEntryFactProve));//DOcEntryFactProve
                                        using (OdbcDataReader factProve = cmcd.ExecuteReader())
                                        {
                                            if (factProve.Read())
                                            {
                                                JsonObject = CrearObjetoJson(factProve);
                                                JsonObject["CardCode"] = Ccode;
                                                JsonObject["U_OrigenIntercompany"] = BdHechizo;
                                                JsonObject["U_DocEntryOrigen"] = En_Entry;
                                                JsonObject["U_DocNumOrigen"] = DocNum;
                                                JsonObject["Series"] = ConsultarSeries(conn, "Primario", 19, BdDest, "--");

                                            }
                                            else
                                            {
                                                continue;
                                            }

                                        }

                                        JArray JArrayDetalle = new JArray();

                                        OdbcCommand cmc = new OdbcCommand(QueryDetNC, conn);
                                        cmc.Parameters.Add(new OdbcParameter("@num", En_Entry));

                                        using (OdbcDataReader dtr = cmc.ExecuteReader())
                                        {

                                            JArrayDetalle = CrearObjetoJson2(dtr);
                                        }

                                        foreach (var item in JArrayDetalle)
                                        {
                                            var obj = (JObject)item;
                                            item["BaseType"] = 18;
                                            item["BaseEntry"] = DOcEntryFactProve;

                                        }

                                        JsonObject.Add(new JProperty("DocumentLines", JArrayDetalle));

                                        var myjs = JsonObject.ToString();
                                        resul = EnviaSapPorBasicAuth("POST", JsonObject, UrlServiceLayer + "PurchaseCreditNotes", tokenDestino);

                                        if (resul == 0) // Caso que inserto
                                        {
                                            string query = $"UPDATE  \"{BdHechizo}\".\"ORIN\" SET \"U_Estado\" = 'Y', \"U_DocNumDestino\" = {ObjSap.DocNum}, \"U_DocEntryDestino\" = {ObjSap.DocEntry}  WHERE \"DocEntry\" = {En_Entry} ";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();
                                        }
                                        else
                                        {
                                            string query = $"UPDATE  \"{BdHechizo}\".\"ORIN\" SET \"U_ComentarioError\" = '{RR.Replace("'", "")}', \"U_Estado\" = 'Y' WHERE \"DocEntry\" = {En_Entry}";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();

                                            objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Error creando la NC de proveedor en {BdDest} con el docNum factura deudor {DocNum}: " + RR.Replace("'", ""));
                                        }
                                    }
                                    else
                                    {

                                        string query = $"UPDATE  \"{BdHechizo}\".\"ORIN\" SET \"U_ComentarioError\" = 'No fue posible crear su NC de proveedor en {BdDest} ya sea porque no se encontró el docentry relacionado: {DOcEntryFactProve} o el documento relacionado esta cerrado o cancelado', \"U_Estado\" = 'Y' WHERE \"DocEntry\" = {En_Entry}";
                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                        OdbcDataReader dr1 = cmand.ExecuteReader();

                                        objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] No fue posible crear su NC de proveedor en {BdDest} ya sea porque no se encontró el docentry relacionado: {DOcEntryFactProve}  o el documento relacionado esta cerrado o cancelado");
                                    }

                                }
                                catch (Exception ex)
                                {

                                    objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}] Error al procesar las Notas de Credito, {ex.Message}");
                                }

                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error en NotasCredProve, {ex.Message}");
            }
        }

        /// <summary>
        /// Replica en las ordCompra de las sucursales las actualizaciones que se hicieron desde la OrdVenta de Hechizo
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <param name="tokenDestino"></param>
        public static void ActualizarEnOrdCompra(string bdDestino, string tokenDestino)
        {
            string BdHechizo = string.Empty;
            string BdDest = string.Empty;
            string QueryOrdComp = string.Empty;
            string QueryDetComp = string.Empty;
            int DocEntryProcesar = 0;
            bool StatusDoc = false;
            string BDConsulta2 = string.Empty;

            int resul = 0;

            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                BdHechizo = ConsultarBD("ORIGEN");
                BdDest = ConsultarBD(bdDestino);
                string CardCode = ConsultarCardCode(bdDestino.ToUpper());

                QueryOrdComp = $@"SELECT 
                                ""DocEntry"" 
                                ,""DocNum"" 
                                ,""Comments""
                                ,""DocDate""
                                ,""DocDueDate""
                                ,""TaxDate""
                                ,""DocTotal""
                                ,""DocCur""
                                ,""DocRate""
                                ,""DiscPrcnt"" AS ""DiscountPercent""
                                ,""GroupNum"" AS ""PaymentGroupCode""
                                ,""U_DocEntryDestino""
                                FROM ""{BdHechizo}"".ORDR
                                EF WHERE ""CANCELED"" = 'N' AND ""DocStatus"" = 'O'  AND ""U_Actualiza"" = 'Y' AND ""CardCode"" = '{CardCode}' ";//,""TrnspCode"" AS ""TransportationCode""

                QueryDetComp = $@"SELECT 
                                ""ItemCode""
                                ,""Quantity""                             
                                ,""TaxCode""
                                ,""LineTotal""
                                ,""LineNum""                               
                                ,""Price"" AS ""UnitPrice""
                                FROM ""{BdHechizo}"".RDR1
                                WHERE ""DocEntry"" = ? ";


                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    OdbcCommand CmD = new OdbcCommand(QueryOrdComp, conn);

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            while (dr.Read())
                            {
                                try
                                {

                                    JObject JsonObject = new JObject();

                                    int En_Entry = Convert.ToInt32(dr["DocEntry"]);

                                    int DocNum = Convert.ToInt32(dr["DocNum"]);

                                    int EntryActualizar = dr.IsDBNull(dr.GetOrdinal("U_DocEntryDestino")) ? 0 : dr.GetInt32(dr.GetOrdinal("U_DocEntryDestino"));

                                    StatusDoc = ConsultarStatusDoc(conn, EntryActualizar, BdDest, "OPOR");

                                    if (EntryActualizar != 0 && StatusDoc == true)
                                    {

                                        JsonObject = CrearObjetoJson(dr);

                                        JsonObject.Remove("DocEntry");
                                        JsonObject.Remove("DocNum");
                                        JsonObject.Remove("U_DocEntryDestino");
                                        JArray JArrayDetalle = new JArray();

                                        OdbcCommand cmc = new OdbcCommand(QueryDetComp, conn);
                                        cmc.Parameters.Add(new OdbcParameter("@num", En_Entry));

                                        using (OdbcDataReader dtr = cmc.ExecuteReader())
                                        {
                                            JArrayDetalle = CrearObjetoJson2(dtr);
                                        }

                                        JsonObject.Add(new JProperty("DocumentLines", JArrayDetalle));

                                        var myjs = JsonObject.ToString();

                                        resul = EnviaSapPorBasicAuth("PATCH", JsonObject, UrlServiceLayer + $"PurchaseOrders({EntryActualizar})", tokenDestino);

                                        if (resul == 0) // Caso que inserto
                                        {


                                            #region Consultar si hay en el campo de error mensaje de error previo

                                            string errorPrevio = string.Empty;

                                            string QuerymsgErr = $@"
                                            SELECT ""U_ComentarioError""
                                            FROM ""{BdHechizo}"".""ORDR""
                                            WHERE ""DocEntry"" = '{En_Entry}'";

                                            using (OdbcDataReader drr =
                                                new OdbcCommand(QuerymsgErr, conn).ExecuteReader())
                                            {
                                                if (drr.Read())
                                                    errorPrevio = drr["U_ComentarioError"].ToString();
                                            }

                                            #endregion

                                            string msg = "OrdComp Actualizada con éxito" + (string.IsNullOrEmpty(errorPrevio) ? "" : ", " + errorPrevio);

                                            string query = $"UPDATE  \"{BdHechizo}\".\"ORDR\" SET \"U_Actualiza\" = 'N', \"U_ComentarioError\" = '{msg}'  WHERE \"DocEntry\" = {En_Entry} ";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();

                                            string jsComprimido = CompactJson(myjs);
                                            objRegistraLog.GrabaJson("");
                                            objRegistraLog.GrabaJson($"[{DateTime.Now.ToString("HH:mm:ss tt")}] BDOrigen: {BdHechizo} DEntryOrg {En_Entry}, DNumOrg {DocNum} / BDDestino {BdDest}, DEntryDest {EntryActualizar} / Json: {jsComprimido} ");
                                        }
                                        else
                                        {
                                            string query = $"UPDATE  \"{BdHechizo}\".\"ORDR\" SET \"U_ComentarioError\" = 'Sin Actualizar: {RR.Replace("'", "")}', \"U_Actualiza\" = 'N' WHERE \"DocEntry\" = {En_Entry}";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();

                                            objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Error actualizando el laorden de compra en {BdDest} con el docNum orden de venta {DocNum}: " + RR.Replace("'", ""));
                                        }
                                    }
                                    else
                                    {

                                        string query = $"UPDATE  \"{BdHechizo}\".\"ORDR\" SET \"U_ComentarioError\" = 'No fue posible Actualizar el documento en {BdDest} ya sea porque no se encontró el docentry relacionado: {DocEntryProcesar} o el documento relacionado esta cerrado o cancelado', \"U_Actualiza\" = 'N' WHERE \"DocEntry\" = {En_Entry}";
                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                        OdbcDataReader dr1 = cmand.ExecuteReader();

                                        objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Sin Actualizar: No fue posible Actualizar el documento en {BdDest} ya sea porque no se encontró el docentry relacionado: {DocEntryProcesar}  o el documento relacionado esta cerrado o cancelado");
                                    }


                                }
                                catch (Exception ex)
                                {

                                    objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error al Actualizar las OrdCompra hacia BdDestino {BdDest}, {ex.Message}");
                                }

                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error al ActualizarEnOrdCompra hacia BdDestino {BdDest}, {ex.Message}");
            }
        }

        /// <summary>
        /// Replica en las ordCompra de las sucursales las actualizaciones que se hicieron desde la OrdVenta de Hechizo, agrega el log para validar preciso diferentes
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <param name="tokenDestino"></param>
        public static void ActualizarEnOrdCompraV2(string bdDestino, string tokenDestino)
        {
            string BdHechizo = string.Empty;
            string BdDest = string.Empty;
            string QueryOrdComp = string.Empty;
            string QueryDetComp = string.Empty;
            int DocEntryProcesar = 0;
            bool StatusDoc = false;
            string BDConsulta2 = string.Empty;

            int resul = 0;

            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                BdHechizo = ConsultarBD("ORIGEN");
                BdDest = ConsultarBD(bdDestino);
                string CardCode = ConsultarCardCode(bdDestino.ToUpper());

                QueryOrdComp = $@"SELECT 
                                ""DocEntry"" 
                                ,""DocNum"" 
                                ,""Comments""
                                ,""DocDate""
                                ,""DocDueDate""
                                ,""TaxDate""
                                ,""DocTotal""
                                ,""DocCur""
                                ,""DocRate""
                                ,""DiscPrcnt"" AS ""DiscountPercent""
                                ,""GroupNum"" AS ""PaymentGroupCode""
                                ,""U_DocEntryDestino""
                                FROM ""{BdHechizo}"".ORDR
                                EF WHERE ""CANCELED"" = 'N' AND ""DocStatus"" = 'O'  AND ""U_Actualiza"" = 'Y' AND ""CardCode"" = '{CardCode}' ";//,""TrnspCode"" AS ""TransportationCode""

                QueryDetComp = $@"SELECT 
                                ""ItemCode""
                                ,""Quantity""                             
                                ,""TaxCode""
                                ,""LineTotal""
                                ,""LineNum""                               
                                ,""Price"" AS ""UnitPrice""
                                FROM ""{BdHechizo}"".RDR1
                                WHERE ""DocEntry"" = ? ";


                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    OdbcCommand CmD = new OdbcCommand(QueryOrdComp, conn);

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            while (dr.Read())
                            {
                                try
                                {

                                    JObject JsonObject = new JObject();

                                    int En_Entry = Convert.ToInt32(dr["DocEntry"]);

                                    int DocNum = Convert.ToInt32(dr["DocNum"]);

                                    int EntryActualizar = dr.IsDBNull(dr.GetOrdinal("U_DocEntryDestino")) ? 0 : dr.GetInt32(dr.GetOrdinal("U_DocEntryDestino"));

                                    StatusDoc = ConsultarStatusDoc(conn, EntryActualizar, BdDest, "OPOR");

                                    if (EntryActualizar != 0 && StatusDoc == true)
                                    {

                                        JsonObject = CrearObjetoJson(dr);

                                        JsonObject.Remove("DocEntry");
                                        JsonObject.Remove("DocNum");
                                        JsonObject.Remove("U_DocEntryDestino");
                                        JArray JArrayDetalle = new JArray();

                                        OdbcCommand cmc = new OdbcCommand(QueryDetComp, conn);
                                        cmc.Parameters.Add(new OdbcParameter("@num", En_Entry));

                                        using (OdbcDataReader dtr = cmc.ExecuteReader())
                                        {
                                            JArrayDetalle = CrearObjetoJson2(dtr);
                                        }

                                        JsonObject.Add(new JProperty("DocumentLines", JArrayDetalle));

                                        var myjs = JsonObject.ToString();

                                        resul = EnviaSapPorBasicAuth("PATCH", JsonObject, UrlServiceLayer + $"PurchaseOrders({EntryActualizar})", tokenDestino);

                                        if (resul == 0) // Caso que inserto
                                        {
                                            // Comparación de precios contra BD destino
                                            ComparePricesAndLogActualizacion(EntryActualizar, En_Entry, JArrayDetalle, conn, BdHechizo, BdDest);

                                            #region Consultar si hay en el campo de error mensaje de error previo

                                            string errorPrevio = string.Empty;

                                            string QuerymsgErr = $@"
                                            SELECT ""U_ComentarioError""
                                            FROM ""{BdHechizo}"".""ORDR""
                                            WHERE ""DocEntry"" = '{En_Entry}'";

                                            using (OdbcDataReader drr =
                                                new OdbcCommand(QuerymsgErr, conn).ExecuteReader())
                                            {
                                                if (drr.Read())
                                                    errorPrevio = drr["U_ComentarioError"].ToString();
                                            }

                                            #endregion

                                            string msg = "OrdComp Actualizada con éxito" + (string.IsNullOrEmpty(errorPrevio) ? "" : ", " + errorPrevio);

                                            string query = $"UPDATE  \"{BdHechizo}\".\"ORDR\" SET \"U_Actualiza\" = 'N', \"U_ComentarioError\" = '{msg}'  WHERE \"DocEntry\" = {En_Entry} ";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();

                                            string jsComprimido = CompactJson(myjs);
                                            objRegistraLog.GrabaJson("");
                                            objRegistraLog.GrabaJson($"[{DateTime.Now.ToString("HH:mm:ss tt")}] BDOrigen: {BdHechizo} DEntryOrg {En_Entry}, DNumOrg {DocNum} / BDDestino {BdDest}, DEntryDest {EntryActualizar} / Json: {jsComprimido} ");
                                        }
                                        else
                                        {
                                            string query = $"UPDATE  \"{BdHechizo}\".\"ORDR\" SET \"U_ComentarioError\" = 'Sin Actualizar: {RR.Replace("'", "")}', \"U_Actualiza\" = 'N' WHERE \"DocEntry\" = {En_Entry}";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();

                                            objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Error actualizando el laorden de compra en {BdDest} con el docNum orden de venta {DocNum}: " + RR.Replace("'", ""));
                                        }
                                    }
                                    else
                                    {

                                        string query = $"UPDATE  \"{BdHechizo}\".\"ORDR\" SET \"U_ComentarioError\" = 'No fue posible Actualizar el documento en {BdDest} ya sea porque no se encontró el docentry relacionado: {DocEntryProcesar} o el documento relacionado esta cerrado o cancelado', \"U_Actualiza\" = 'N' WHERE \"DocEntry\" = {En_Entry}";
                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                        OdbcDataReader dr1 = cmand.ExecuteReader();

                                        objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Sin Actualizar: No fue posible Actualizar el documento en {BdDest} ya sea porque no se encontró el docentry relacionado: {DocEntryProcesar}  o el documento relacionado esta cerrado o cancelado");
                                    }


                                }
                                catch (Exception ex)
                                {

                                    objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error al Actualizar las OrdCompra hacia BdDestino {BdDest}, {ex.Message}");
                                }

                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error al ActualizarEnOrdCompra hacia BdDestino {BdDest}, {ex.Message}");
            }
        }

        /// <summary>
        /// Replica  en las Factura de Provee de las sucursales las actualizaciones que se hicieron desde la Facturas de venta de Hechizo
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <param name="tokenDestino"></param>
        public static void ActualizarEnFacturaProvee(string bdDestino, string tokenDestino)
        {
            string BdHechizo = string.Empty;
            string BdDest = string.Empty;
            string QueryOrdComp = string.Empty;
            string QueryDetComp = string.Empty;
            int DocEntryProcesar = 0;
            bool StatusDoc = false;
            int resul = 0;

            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                BdHechizo = ConsultarBD("ORIGEN");
                BdDest = ConsultarBD(bdDestino);
                string CardCode = ConsultarCardCode(bdDestino.ToUpper());

                QueryOrdComp = $@"SELECT 
                                ""DocEntry"" 
                                ,""DocNum"" 
                                ,""Comments""
                                ,""DocDueDate""
                                ,""TrnspCode"" AS ""TransportationCode""
                                ,""NumAtCard""
                                ,""PayToCode""
                                ,""Address""
                                ,""U_DocEntryDestino""
                                FROM {BdHechizo}.OINV
                                EF WHERE ""CANCELED"" = 'N'  AND ""DocStatus"" = 'O' AND ""U_Actualiza"" = 'Y' AND ""CardCode"" = '{CardCode}' "; //,""SlpCode"" AS ""SalesPersonCode"" ,""DiscPrcnt"" AS ""DiscountPercent""




                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    OdbcCommand CmD = new OdbcCommand(QueryOrdComp, conn);

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            while (dr.Read())
                            {
                                try
                                {

                                    JObject JsonObject = new JObject();

                                    int En_Entry = Convert.ToInt32(dr["DocEntry"]);

                                    int DocNum = Convert.ToInt32(dr["DocNum"]);

                                    int EntryActualizar = dr.IsDBNull(dr.GetOrdinal("U_DocEntryDestino")) ? 0 : dr.GetInt32(dr.GetOrdinal("U_DocEntryDestino"));

                                    StatusDoc = ConsultarStatusDoc(conn, EntryActualizar, BdDest, "OPCH");

                                    if (EntryActualizar != 0 && StatusDoc == true)
                                    {

                                        JsonObject = CrearObjetoJson(dr);

                                        JsonObject.Remove("DocEntry");
                                        JsonObject.Remove("DocNum");
                                        JsonObject.Remove("U_DocEntryDestino");

                                        var myjs = JsonObject.ToString();
                                        resul = EnviaSapPorBasicAuth("PATCH", JsonObject, UrlServiceLayer + $"PurchaseInvoices({EntryActualizar})", tokenDestino, "N");

                                        if (resul == 0) // Caso que inserto
                                        {
                                            #region Consultar si hay en el campo de error mensaje de error previo

                                            string errorPrevio = string.Empty;

                                            string QuerymsgErr = $@"
                                            SELECT ""U_ComentarioError""
                                            FROM ""{BdHechizo}"".""OINV""
                                            WHERE ""DocEntry"" = '{En_Entry}'";

                                            using (OdbcDataReader drr =
                                                new OdbcCommand(QuerymsgErr, conn).ExecuteReader())
                                            {
                                                if (drr.Read())
                                                    errorPrevio = drr["U_ComentarioError"].ToString();
                                            }

                                            #endregion

                                            string msg = "Factura Actualizada con éxito" + (string.IsNullOrEmpty(errorPrevio) ? "" : ", " + errorPrevio);


                                            string query = $"UPDATE  \"{BdHechizo}\".\"OINV\" SET \"U_Actualiza\" = 'N', \"U_ComentarioError\" = '{msg}'  WHERE \"DocEntry\" = {En_Entry} ";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();
                                        }
                                        else
                                        {
                                            string query = $"UPDATE  \"{BdHechizo}\".\"OINV\" SET \"U_ComentarioError\" = 'Sin Actualizar: {RR.Replace("'", "")}', \"U_Actualiza\" = 'N' WHERE \"DocEntry\" = {En_Entry}";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();

                                            objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Error actualizando la Factura de proveedor en {BdDest} con el docNum factura deudor {DocNum}: " + RR.Replace("'", ""));
                                        }
                                    }
                                    else
                                    {

                                        string query = $"UPDATE  \"{BdHechizo}\".\"OINV\" SET \"U_ComentarioError\" = 'No fue posible Actualizar la Factura {DocNum} en {BdDest} ya sea porque no se encontró el docentry relacionado: {DocEntryProcesar} o el documento relacionado esta cerrado o cancelado', \"U_Actualiza\" = 'N' WHERE \"DocEntry\" = {En_Entry}";
                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                        OdbcDataReader dr1 = cmand.ExecuteReader();

                                        objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Sin Actualizar: No fue posible Actualizar la Factura {DocNum} en  {BdDest} ya sea porque no se encontró el docentry relacionado: {DocEntryProcesar}  o el documento relacionado esta cerrado o cancelado");
                                    }


                                }
                                catch (Exception ex)
                                {

                                    objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error al ActualizarEnFacturaProveeo hacia BdDestino {BdDest}, {ex.Message}");
                                }

                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error  en ActualizarEnFacturaProveeo hacia BdDestino {BdDest}, {ex.Message}");
            }
        }



        #endregion

        public static Respuesta CrearFabricanteTODAS(string bdDestino, JObject JsonObj, string Token)
        {
            int resul = 0;
            Respuesta R = new Respuesta();
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                string sFirmName = JsonObj["ManufacturerName"].ToString();
                var myjs = JsonObj.ToString();

                resul = EnviaSapPorBasicAuth("POST", JsonObj, UrlServiceLayer + "Manufacturers", Token);

                if (resul == 0) // Caso que inserto 
                {
                    objRegistraLog.Graba("Insertado a Sap el fabricante : " + sFirmName + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                }
                else
                {
                    objRegistraLog.Graba($"Error insertando fabricante en la base destinno {bdDestino}: " + RR + " " + sFirmName + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));

                    Inserterror(bdDestino, "OMRC", sFirmName);
                }

                LiberaMemoria.FlushMemory();
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al intentar Insertar Fabricante : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
            }
            return R;
        }

        public static Respuesta CreargrupoTODAS(string bdDestino, JObject JsonObj, string Token)
        {
            int resul = 0;
            Respuesta R = new Respuesta();
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                string ItemGroups = JsonObj["GroupName"].ToString();

                var myjs = JsonObj.ToString();
                resul = EnviaSapPorBasicAuth("POST", JsonObj, UrlServiceLayer + "ItemGroups", Token);

                if (resul == 0) // Caso que inserto 
                {
                    objRegistraLog.Graba("Insertado a Sap el grupo : " + ItemGroups + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                }
                else
                {
                    objRegistraLog.Graba($"Error insertando grupo en la base destinno {bdDestino}: " + RR + " " + ItemGroups + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));


                    Inserterror(bdDestino, "OITB", ItemGroups);
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

        public static Respuesta CreargrupoSNTODAS(string bdDestino, JObject JsonObj, string Token)
        {
            int resul = 0;
            Respuesta R = new Respuesta();
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                string Name = JsonObj["Name"].ToString();

                var myjs = JsonObj.ToString();
                resul = EnviaSapPorBasicAuth("POST", JsonObj, UrlServiceLayer + "BusinessPartnerGroups", Token);
                if (resul == 0) // Caso que inserto 
                {
                    objRegistraLog.Graba("Insertado a Sap el grupo : " + Name + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                }
                else
                {
                    objRegistraLog.Graba($"Error CreargrupoSNTODAS en la base destino {bdDestino}: " + RR + " " + Name + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));

                    Inserterror(bdDestino, "OCRG", Name);


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

        public static Respuesta CrearcondicionTODAS(string bdDestino, JObject JsonObj, string Token)
        {
            int resul = 0;
            Respuesta R = new Respuesta();
            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                string PymntGroup = JsonObj["PaymentTermsGroupName"].ToString();

                var myjs = JsonObj.ToString();

                resul = EnviaSapPorBasicAuth("POST", JsonObj, UrlServiceLayer + "PaymentTermsTypes", Token); //
                if (resul == 0) // Caso que inserto 
                {
                    objRegistraLog.Graba("Insertado a Sap el grupo : " + PymntGroup + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                }
                else
                {
                    objRegistraLog.Graba($"Error CrearcondicionTODAS en la Bd Destino {bdDestino}: " + RR + " " + PymntGroup + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 

                    Inserterror(bdDestino, "OCTG", PymntGroup);
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

        public static Respuesta CreararticuloTODAS(string bdDestino, string Tokendestino)
        {
            int resul = 0;
            string nomcod = string.Empty;
            int cuantos = 0;
            Respuesta R = new Respuesta();
            try
            {

                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");
                string stUSUARIO = IniFil.LeerINI("ConexionHanaProd", "UserSL");
                string stPASSWORD = IniFil.LeerINI("ConexionHanaProd", "PassSL");

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                using (OdbcConnection conn2 = new OdbcConnection(Conexion.strCon))
                using (OdbcConnection conn3 = new OdbcConnection(Conexion.strCon))
                {
                    string query = $"CALL \"{Conexion.BD_Hechizo}\".IC_ADD_ITEMS2('{bdDestino}')"; //Query Encabezado
                    OdbcCommand CmD = new OdbcCommand(query, conn); // llama al Query Encabezado con la conexion
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(bdDestino, "fcscr-cloud\\" + stUSUARIO, stPASSWORD, UrlServiceLayer + "Login") != 0)
                            {

                                R.Codigo = -2;
                                R.Mensaje = RR;

                                objRegistraLog.Graba($"Sin conectar a Service Layer en el método CreararticuloTODAS en {bdDestino}: " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error 

                                return R;
                            }

                            while (dr.Read())
                            {

                                nomcod = dr["ItemCode"].ToString();
                                if (nomcod == "M07-VE-S")
                                {
                                    string hhd = "aa";
                                }
                                string queryEn = $"select count(\"ItemCode\") \"conteo\" from  \"{bdDestino}\".\"OITM\" where \"ItemCode\" = '" + nomcod + "'";
                                OdbcCommand CmD1 = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                                conn2.Open();
                                using (OdbcDataReader dr2 = CmD1.ExecuteReader())
                                {
                                    while (dr2.Read())
                                    {
                                        cuantos = Convert.ToInt32(dr2["conteo"]);

                                        if (cuantos == 0)
                                        {
                                            JObject JsonObject = new JObject();
                                            JsonObject = CrearObjetoJson(dr);
                                            JArray jArrayprecios = new JArray();
                                            string crDet = $@"
                                                                SELECT 
                                                                    ""PriceList"",
                                                                    ""Price"",
                                                                    ""Currency"",
                                                                    ""Factor"",
                                                                    ""BasePriceList"",
                                                                    ""UoMPrices""
                                                                FROM (
                                                                    SELECT  
                                                                        T1.""PriceList"",
                                                                        T1.""Price"",
                                                                        T1.""Currency"",
                                                                        T1.""Factor"",
                                                                        T1.""BasePLNum"" AS ""BasePriceList"",
                                                                        T1.""UomEntry"" AS ""UoMPrices""
                                                                    FROM ""{Conexion.BD_Hechizo}"".""ITM1"" T1
                                                                    WHERE T1.""ItemCode"" = '{nomcod}'
                                                                      AND T1.""BasePLNum"" IN (-1,-2)

                                                                    UNION ALL

                                                                    SELECT 
                                                                        T2.""PriceList"",
                                                                        T2.""Price"",
                                                                        T2.""Currency"",
                                                                        T2.""Factor"",
                                                                        T2.""BasePLNum"" AS ""BasePriceList"",
                                                                        T2.""UomEntry"" AS ""UoMPrices""
                                                                    FROM ""{Conexion.BD_Hechizo}"".""ITM1"" T2
                                                                    WHERE T2.""ItemCode"" = '{nomcod}'
                                                                      AND T2.""PriceList"" <> '1'
                                                                      AND T2.""Ovrwrite1"" = 'Y'
                                                                ) X";
                                            OdbcCommand cmc = new OdbcCommand(crDet, conn3);
                                            conn3.Open();
                                            using (OdbcDataReader dtr = cmc.ExecuteReader())
                                            {
                                                jArrayprecios = CrearObjetoJson2(dtr);
                                                JsonObject.Add(new JProperty("ItemPrices", jArrayprecios));
                                                var myjs = JsonObject.ToString();
                                                resul = EnviaSapPorSesion("POST", JsonObject, UrlServiceLayer + "Items");

                                                if (resul == 0) // Caso que inserto
                                                {

                                                    if (bdDestino == Conexion.BD_central)
                                                    {
                                                        query = $"UPDATE  \"{Conexion.BD_Hechizo}\".\"OITM\" SET \"U_Enviado\" = 1 WHERE \"ItemCode\" =  '" + nomcod + "'";
                                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                                        OdbcDataReader dr1 = cmand.ExecuteReader();
                                                    }

                                                    objRegistraLog.Graba("Insertado a Sap el artículo : " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                                }
                                                else
                                                {
                                                    objRegistraLog.Graba($"Error insertando articulo {nomcod} en la bdDestino {bdDestino}: " + RR + " " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));//

                                                    Inserterror(bdDestino, "OITM", nomcod);
                                                }
                                            }

                                            conn3.Close();
                                        }
                                        //else
                                        //{
                                        //    JObject JsonObject = new JObject();
                                        //    JsonObject = CrearObjetoJson(dr);
                                        //    JsonObject.Remove("ItemCode");
                                        //    JArray jArrayprecios = new JArray();
                                        //    string crDet = $"SELECT  \"PriceList\",\"Price\",\"Currency\",\"Factor\",\"BasePLNum\" as \"BasePriceList\" ,\"UomEntry\" as \"UoMPrices\" FROM  \"{Conexion.BD_Hechizo}\".\"ITM1\" where  \"ItemCode\" = '" + nomcod + "'";
                                        //    OdbcCommand cmc = new OdbcCommand(crDet, conn3);
                                        //    conn3.Open();
                                        //    using (OdbcDataReader dtr = cmc.ExecuteReader())
                                        //    {
                                        //        jArrayprecios = CrearObjetoJson2(dtr);
                                        //        JsonObject.Add(new JProperty("ItemPrices", jArrayprecios));
                                        //        var myjs = JsonObject.ToString();
                                        //        resul = EnviaSapPorSesion("PATCH", JsonObject, UrlServiceLayer + "Items('" + nomcod + "')");

                                        //        if (resul == 0) // Caso que inserto
                                        //        {

                                        //            if (bdDestino == Conexion.BD_central)
                                        //            {
                                        //                query = $"UPDATE  \"{Conexion.BD_Hechizo}\".\"OITM\" SET \"U_Enviado\" = 1 WHERE \"ItemCode\" =  '" + nomcod + "'";
                                        //                OdbcCommand cmand = new OdbcCommand(query, conn);
                                        //                OdbcDataReader dr1 = cmand.ExecuteReader();
                                        //            }

                                        //        }
                                        //        else
                                        //        {
                                        //            objRegistraLog.Graba($"Error actualizando en el método CreararticuloTODAS, el articulo {nomcod} en la bdDestino {bdDestino}: " + RR + " " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));//

                                        //            string bd = bdDestino;
                                        //            string empresa = bd
                                        //                .Replace("10071_", "")
                                        //                .Replace("_TEST", "");
                                        //            Inserterror(empresa, "OITM", nomcod);
                                        //        }

                                        //    }
                                        //    conn3.Close();
                                        //}

                                        LiberaMemoria.FlushMemory();
                                    }
                                }

                                conn2.Close();
                            }
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

        public static Respuesta CreararticuloTODASV2(string bdDestino)
        {
            int resul = 0;
            string nomcod = string.Empty;
            int cuantos = 0;
            Respuesta R = new Respuesta();
            try
            {

                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");
                string stUSUARIO = IniFil.LeerINI("ConexionHanaProd", "UserSL");
                string stPASSWORD = IniFil.LeerINI("ConexionHanaProd", "PassSL");
                string columnaActualizaDest = ConsultarColumna(bdDestino);
                string columnaGrpArtDest = ConsultarGrpArt(bdDestino);
                string bdDest = ConsultarBD(bdDestino);

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    string query = $@"SELECT
                                        IO.""ItemCode"" as ""ItemCode""
                                        ,IO.""ItemName"" as ""ItemName""
                                        ,IO.""FrgnName"" as ""ForeignName""
                                        ,IO.""CodeBars"" AS ""BarCode""
                                        ,GRPART.""{columnaGrpArtDest}"" AS ""ItemsGroupCode""
                                        ,IO.""PrchseItem"" as ""PurchaseItem""
                                        ,IO.""SellItem"" as ""SalesItem""
                                        ,IO.""InvntItem"" as ""InventoryItem""
                                        ,IO.""VATLiable"" AS ""VatLiable""
                                        ,IO.""SuppCatNum"" as ""SupplierCatalogNo""
                                        ,IO.""SWW"" AS ""SWW""
                                        ,IO.""InvntryUom"" AS ""InventoryUOM""
                                        ,IO.""ShipType"" as ""ShipType""
                                        ,F.""TrnspName""
                                        ,IO.""validFor"" AS ""Valid""
                                        ,IO.""frozenFor"" AS ""Frozen""
                                        ,IO.""PlaningSys"" AS ""PlanningSystem""
                                        ,IO.""PrcrmntMtd"" AS ""ProcurementMethod""
                                        ,IO.""OrdrIntrvl"" AS ""OrderIntervals""
                                        ,IO.""OrdrMulti"" AS ""OrderMultiple""
                                        ,IO.""MinOrdrQty"" AS ""MinOrderQuantity""
                                        ,IO.""LeadTime"" AS ""LeadTime""
                                        ,IO.""ToleranDay"" AS ""ToleranceDays""
                                        ,IO.""U_partida_arancelaria""
                                        ,IO.""U_Cabys""
                                        ,IO.""U_codigo_iva""
                                        ,IO.""U_CondicionImpuesto""
                                        ,IO.""U_CabysValidado""
                                        ,IO.""U_NombreCabys""
                                        ,IO.""UserText"" as ""User_Text""
                                        ,IO.""QryGroup1"" AS ""Properties1""
                                        ,IO.""QryGroup2"" AS ""Properties2""
                                        ,IO.""QryGroup3"" AS ""Properties3""
                                        ,IO.""QryGroup4"" AS ""Properties4""
                                        ,IO.""QryGroup5"" AS ""Properties5""
                                        ,IO.""QryGroup6"" AS ""Properties6""
                                        ,IO.""QryGroup7"" AS ""Properties7""
                                        ,IO.""QryGroup8"" AS ""Properties8""
                                        ,IO.""QryGroup9"" AS ""Properties9""
                                        ,IO.""QryGroup10"" AS ""Properties10""
                                        ,IO.""QryGroup11"" AS ""Properties11""
                                        ,IO.""QryGroup12"" AS ""Properties12""
                                        ,IO.""QryGroup13"" AS ""Properties13""
                                        ,IO.""QryGroup14"" AS ""Properties14""
                                        ,IO.""QryGroup15"" AS ""Properties15""
                                        ,IO.""QryGroup16"" AS ""Properties16""
                                        ,IO.""QryGroup17"" AS ""Properties17""
                                        ,IO.""QryGroup18"" AS ""Properties18""
                                        ,IO.""QryGroup20"" AS ""Properties20""
                                        ,IO.""QryGroup21"" AS ""Properties21""
                                        ,IO.""QryGroup22"" AS ""Properties22""
                                        ,IO.""QryGroup23"" AS ""Properties23""
                                        ,IO.""QryGroup24"" AS ""Properties24""
                                        ,IO.""QryGroup25"" AS ""Properties25""
                                        ,IO.""QryGroup26"" AS ""Properties26""
                                        ,IO.""QryGroup27"" AS ""Properties27""
                                        ,IO.""QryGroup28"" AS ""Properties28""
                                        ,IO.""QryGroup29"" AS ""Properties29""
                                        ,IO.""QryGroup30"" AS ""Properties30""
                                        ,IO.""QryGroup31"" AS ""Properties31""
                                        ,IO.""QryGroup32"" AS ""Properties32""
                                        ,IO.""QryGroup33"" AS ""Properties33""
                                        ,IO.""QryGroup34"" AS ""Properties34""
                                        ,IO.""QryGroup35"" AS ""Properties35""
                                        ,IO.""QryGroup36"" AS ""Properties36""
                                        ,IO.""QryGroup37"" AS ""Properties37""
                                        ,IO.""QryGroup38"" AS ""Properties38""
                                        ,IO.""QryGroup39"" AS ""Properties39""
                                        ,IO.""QryGroup40"" AS ""Properties40""
                                        ,IO.""QryGroup41"" AS ""Properties41""
                                        ,IO.""QryGroup42"" AS ""Properties42""
                                        ,IO.""QryGroup43"" AS ""Properties43""
                                        ,IO.""QryGroup44"" AS ""Properties44""
                                        ,IO.""QryGroup45"" AS ""Properties45""
                                        ,IO.""QryGroup46"" AS ""Properties46""
                                        ,IO.""QryGroup47"" AS ""Properties47""
                                        ,IO.""QryGroup48"" AS ""Properties48""
                                        ,IO.""QryGroup49"" AS ""Properties49""
                                        ,IO.""QryGroup50"" AS ""Properties50""
                                        ,IO.""QryGroup51"" AS ""Properties51""
                                        ,IO.""QryGroup52"" AS ""Properties52""
                                        ,IO.""QryGroup53"" AS ""Properties53""
                                        ,IO.""QryGroup54"" AS ""Properties54""
                                        ,IO.""QryGroup55"" AS ""Properties55""
                                        ,IO.""QryGroup56"" AS ""Properties56""
                                        ,IO.""QryGroup57"" AS ""Properties57""
                                        ,IO.""QryGroup58"" AS ""Properties58""
                                        ,IO.""QryGroup59"" AS ""Properties59""
                                        ,IO.""QryGroup60"" AS ""Properties60""
                                        ,IO.""QryGroup61"" AS ""Properties61""
                                        ,IO.""QryGroup62"" AS ""Properties62""
                                        ,IO.""QryGroup64"" AS ""Properties64""
                                        FROM ""{Conexion.BD_Hechizo}"".OITM IO
                                        LEFT  JOIN ""{bdDest}"".OITM MM on IO.""ItemCode""= MM.""ItemCode""
                                        LEFT JOIN ""_SYS_BIC"".""{Conexion.BD_Hechizo}/IC_GRP_CODE_FAMILIA"" GRPART ON GRPART.""COD_HCHZ"" = IO.""ItmsGrpCod""
                                        LEFT JOIN ""{Conexion.BD_Hechizo}"".""@IC_CTRL_ART"" CTRL  ON IO.""ItemCode"" = CTRL.""U_ItemCode""	AND CTRL.""U_BdDestino"" = '{bdDest}'
                                        LEFT JOIN ""{Conexion.BD_Hechizo}"".OSHP F ON F.""TrnspCode"" = IO.""ShipType""
                                        WHERE
                                             IO.""U_ENVIOSUC"" = 'Y'
                                            AND IO.""ItemType"" <> 'F'  
                                            AND MM.""ItemCode"" IS NULL
                                            AND(
                                                  CTRL.""U_ItemCode"" IS NULL
                                                  OR CTRL.""U_Reintentar"" = 'S'
                                              )";

                    OdbcCommand CmD = new OdbcCommand(query, conn); // llama al Query Encabezado con la conexion
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(bdDest, "fcscr-cloud\\" + stUSUARIO, stPASSWORD, UrlServiceLayer + "Login") != 0)
                            {

                                R.Codigo = -2;
                                R.Mensaje = RR;

                                objRegistraLog.Graba($"Sin conectar a Service Layer en el método CreararticuloTODAS en {bdDestino}: " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error 

                                return R;
                            }

                            while (dr.Read())
                            {
                                var articulosProcesados = new List<string>();

                                nomcod = dr["ItemCode"].ToString();

                                string queryEn = $"select count(\"ItemCode\") \"conteo\" from  \"{bdDest}\".\"OITM\" where \"ItemCode\" = '" + nomcod + "'";
                                OdbcCommand CmD1 = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                                using (OdbcDataReader dr2 = CmD1.ExecuteReader())
                                {
                                    while (dr2.Read())
                                    {
                                        cuantos = Convert.ToInt32(dr2["conteo"]);

                                        if (cuantos == 0)
                                        {
                                            JObject JsonObject = new JObject();
                                            JsonObject = CrearObjetoJson(dr);
                                            JArray jArrayprecios = new JArray();
                                            string crDet = $@"
                                                                SELECT 
                                                                    ""PriceList"",
                                                                    ""Price"",
                                                                    ""Currency"",
                                                                    ""Factor"",
                                                                    ""BasePriceList"",
                                                                    ""UoMPrices""
                                                                FROM (
                                                                    SELECT  
                                                                        T1.""PriceList"",
                                                                        T1.""Price"",
                                                                        T1.""Currency"",
                                                                        T1.""Factor"",
                                                                        T1.""BasePLNum"" AS ""BasePriceList"",
                                                                        T1.""UomEntry"" AS ""UoMPrices""
                                                                    FROM ""{Conexion.BD_Hechizo}"".""ITM1"" T1
                                                                    WHERE T1.""ItemCode"" = '{nomcod}'
                                                                      AND T1.""BasePLNum"" IN (-1,-2)

                                                                    UNION ALL

                                                                    SELECT 
                                                                        T2.""PriceList"",
                                                                        T2.""Price"",
                                                                        T2.""Currency"",
                                                                        T2.""Factor"",
                                                                        T2.""BasePLNum"" AS ""BasePriceList"",
                                                                        T2.""UomEntry"" AS ""UoMPrices""
                                                                    FROM ""{Conexion.BD_Hechizo}"".""ITM1"" T2
                                                                    WHERE T2.""ItemCode"" = '{nomcod}'
                                                                      AND T2.""PriceList"" <> '1'
                                                                      AND T2.""Ovrwrite1"" = 'Y'
                                                                ) X";
                                            OdbcCommand cmc = new OdbcCommand(crDet, conn);
                                            using (OdbcDataReader dtr = cmc.ExecuteReader())
                                            {
                                                jArrayprecios = CrearObjetoJson2(dtr);

                                                string Name = JsonObject["TrnspName"].ToString();
                                                JsonObject.Remove("TrnspName");
                                                JsonObject["ShipType"] = ConusltarFormasEnvio(Name, bdDest, conn);

                                                JsonObject.Add(new JProperty("ItemPrices", jArrayprecios));


                                                var myjs = JsonObject.ToString();
                                                resul = EnviaSapPorSesion("POST", JsonObject, UrlServiceLayer + "Items");
                                            }

                                            if (resul == 0) // Caso que inserto
                                            {
                                                // Si tenía un error previo registrado en la tabla lo limpiamos
                                                string sqlDelete = $@"DELETE FROM ""{Conexion.BD_Hechizo}"".""@IC_CTRL_ART"" 
                                                                    WHERE ""U_ItemCode"" = ? AND ""U_BdDestino"" = ?";

                                                using (OdbcCommand cmdDel = new OdbcCommand(sqlDelete, conn))
                                                {
                                                    cmdDel.Parameters.AddWithValue("@item", nomcod);
                                                    cmdDel.Parameters.AddWithValue("@bd", bdDest);
                                                    cmdDel.ExecuteNonQuery();
                                                }

                                            }
                                            else
                                            {
                                                //Intentamos actualizar primero
                                                string sqlUpdate = $@"UPDATE ""{Conexion.BD_Hechizo}"".""@IC_CTRL_ART""
                                                                      SET ""U_MensajeError"" = ?,
                                                                          ""U_FechaError""   = NOW(),
                                                                          ""U_Reintentar""   = 'N',
                                                                          ""U_Intentos"" = IFNULL(""U_Intentos"", 0) + 1
                                                                      WHERE ""U_ItemCode"" = ? AND ""U_BdDestino"" = ?";

                                                using (OdbcCommand cmdUpd = new OdbcCommand(sqlUpdate, conn))
                                                {
                                                    cmdUpd.Parameters.AddWithValue("@msg", RR.Replace("'", ""));
                                                    cmdUpd.Parameters.AddWithValue("@item", nomcod);
                                                    cmdUpd.Parameters.AddWithValue("@bd", bdDest);
                                                    int filasAfectadas = cmdUpd.ExecuteNonQuery();

                                                    // Si no existía el registro lo insertamos
                                                    if (filasAfectadas == 0)
                                                    {
                                                        string sqlInsert = $@"INSERT INTO ""{Conexion.BD_Hechizo}"".""@IC_CTRL_ART""
                                                                          (""Code"",""Name"", ""U_ItemCode"", ""U_BdDestino"", ""U_Estatus"", 
                                                                           ""U_Intentos"", ""U_MensajeError"", ""U_FechaError"", ""U_Reintentar"")
                                                                          SELECT CAST(IFNULL(MAX(CAST(""Code"" AS INT)), 0) + 1 AS NVARCHAR(50)), 
                                                                                 ?, ?, ?, 'E', 1, ?, CURRENT_TIMESTAMP, 'N'
                                                                          FROM ""{Conexion.BD_Hechizo}"".""@IC_CTRL_ART""";

                                                        using (OdbcCommand cmdIns = new OdbcCommand(sqlInsert, conn))
                                                        {
                                                            cmdIns.Parameters.AddWithValue("@Name", nomcod);
                                                            cmdIns.Parameters.AddWithValue("@item", nomcod);
                                                            cmdIns.Parameters.AddWithValue("@bd", bdDest);
                                                            cmdIns.Parameters.AddWithValue("@msg", RR.Replace("'", ""));
                                                            cmdIns.ExecuteNonQuery();
                                                        }
                                                    }
                                                }


                                                objRegistraLog.Graba($"Error insertando artículo {nomcod} en la bdDestino {bdDestino}: " + RR + " " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));

                                                Inserterror(bdDestino, "OITM", nomcod);
                                            }

                                        }

                                        LiberaMemoria.FlushMemory();
                                    }
                                }

                            }

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

        public static Respuesta ActualizararticuloTODASV2(string bdDestino)
        {
            int resul = 0;
            string bdDest = ConsultarBD(bdDestino);
            Respuesta R = new Respuesta();
            try
            {

                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");
                string stUSUARIO = IniFil.LeerINI("ConexionHanaProd", "UserSL");
                string stPASSWORD = IniFil.LeerINI("ConexionHanaProd", "PassSL");
                string columnaActualizaDest = ConsultarColumna(bdDestino);
                string columnaGrpArtDest = ConsultarGrpArt(bdDestino);

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    string queryArt = $@"SELECT
                                        IO.""ItemCode"" as ""ItemCode""
                                        ,IO.""ItemName"" as ""ItemName""
                                        ,IO.""FrgnName"" as ""ForeignName""
                                        ,IO.""CodeBars"" AS ""BarCode""
                                        ,GRPART.""{columnaGrpArtDest}"" AS ""ItemsGroupCode""
                                        ,IO.""CstGrpCode"" as ""CustomsGroupCode""
                                        ,IO.""PrchseItem"" as ""PurchaseItem""
                                        ,IO.""SellItem"" as ""SalesItem""
                                        ,IO.""InvntItem"" as ""InventoryItem""
                                        ,IO.""VATLiable"" AS ""VatLiable""
                                        ,IO.""SuppCatNum"" as ""SupplierCatalogNo""
                                        ,IO.""SWW"" AS ""SWW""
                                        ,IO.""InvntryUom"" AS ""InventoryUOM""
                                        ,IO.""ShipType"" as ""ShipType""
                                        ,F.""TrnspName""
                                        ,IO.""validFor"" AS ""Valid""
                                        ,IO.""frozenFor"" AS ""Frozen""
                                        ,IO.""PlaningSys"" AS ""PlanningSystem""
                                        ,IO.""PrcrmntMtd"" AS ""ProcurementMethod""
                                        ,IO.""OrdrIntrvl"" AS ""OrderIntervals""
                                        ,IO.""OrdrMulti"" AS ""OrderMultiple""
                                        ,IO.""MinOrdrQty"" AS ""MinOrderQuantity""
                                        ,IO.""LeadTime"" AS ""LeadTime""
                                        ,IO.""ToleranDay"" AS ""ToleranceDays""
                                        ,IO.""U_partida_arancelaria""
                                        ,IO.""U_Cabys""
                                        ,IO.""U_codigo_iva""
                                        ,IO.""U_CondicionImpuesto""
                                        ,IO.""U_CabysValidado""
                                        ,IO.""U_NombreCabys""
                                        ,IO.""UserText"" as ""User_Text""
                                        ,IO.""QryGroup1"" AS ""Properties1""
                                        ,IO.""QryGroup2"" AS ""Properties2""
                                        ,IO.""QryGroup3"" AS ""Properties3""
                                        ,IO.""QryGroup4"" AS ""Properties4""
                                        ,IO.""QryGroup5"" AS ""Properties5""
                                        ,IO.""QryGroup6"" AS ""Properties6""
                                        ,IO.""QryGroup7"" AS ""Properties7""
                                        ,IO.""QryGroup8"" AS ""Properties8""
                                        ,IO.""QryGroup9"" AS ""Properties9""
                                        ,IO.""QryGroup10"" AS ""Properties10""
                                        ,IO.""QryGroup11"" AS ""Properties11""
                                        ,IO.""QryGroup12"" AS ""Properties12""
                                        ,IO.""QryGroup13"" AS ""Properties13""
                                        ,IO.""QryGroup14"" AS ""Properties14""
                                        ,IO.""QryGroup15"" AS ""Properties15""
                                        ,IO.""QryGroup16"" AS ""Properties16""
                                        ,IO.""QryGroup17"" AS ""Properties17""
                                        ,IO.""QryGroup18"" AS ""Properties18""
                                        ,IO.""QryGroup20"" AS ""Properties20""
                                        ,IO.""QryGroup21"" AS ""Properties21""
                                        ,IO.""QryGroup22"" AS ""Properties22""
                                        ,IO.""QryGroup23"" AS ""Properties23""
                                        ,IO.""QryGroup24"" AS ""Properties24""
                                        ,IO.""QryGroup25"" AS ""Properties25""
                                        ,IO.""QryGroup26"" AS ""Properties26""
                                        ,IO.""QryGroup27"" AS ""Properties27""
                                        ,IO.""QryGroup28"" AS ""Properties28""
                                        ,IO.""QryGroup29"" AS ""Properties29""
                                        ,IO.""QryGroup30"" AS ""Properties30""
                                        ,IO.""QryGroup31"" AS ""Properties31""
                                        ,IO.""QryGroup32"" AS ""Properties32""
                                        ,IO.""QryGroup33"" AS ""Properties33""
                                        ,IO.""QryGroup34"" AS ""Properties34""
                                        ,IO.""QryGroup35"" AS ""Properties35""
                                        ,IO.""QryGroup36"" AS ""Properties36""
                                        ,IO.""QryGroup37"" AS ""Properties37""
                                        ,IO.""QryGroup38"" AS ""Properties38""
                                        ,IO.""QryGroup39"" AS ""Properties39""
                                        ,IO.""QryGroup40"" AS ""Properties40""
                                        ,IO.""QryGroup41"" AS ""Properties41""
                                        ,IO.""QryGroup42"" AS ""Properties42""
                                        ,IO.""QryGroup43"" AS ""Properties43""
                                        ,IO.""QryGroup44"" AS ""Properties44""
                                        ,IO.""QryGroup45"" AS ""Properties45""
                                        ,IO.""QryGroup46"" AS ""Properties46""
                                        ,IO.""QryGroup47"" AS ""Properties47""
                                        ,IO.""QryGroup48"" AS ""Properties48""
                                        ,IO.""QryGroup49"" AS ""Properties49""
                                        ,IO.""QryGroup50"" AS ""Properties50""
                                        ,IO.""QryGroup51"" AS ""Properties51""
                                        ,IO.""QryGroup52"" AS ""Properties52""
                                        ,IO.""QryGroup53"" AS ""Properties53""
                                        ,IO.""QryGroup54"" AS ""Properties54""
                                        ,IO.""QryGroup55"" AS ""Properties55""
                                        ,IO.""QryGroup56"" AS ""Properties56""
                                        ,IO.""QryGroup57"" AS ""Properties57""
                                        ,IO.""QryGroup58"" AS ""Properties58""
                                        ,IO.""QryGroup59"" AS ""Properties59""
                                        ,IO.""QryGroup60"" AS ""Properties60""
                                        ,IO.""QryGroup61"" AS ""Properties61""
                                        ,IO.""QryGroup62"" AS ""Properties62""
                                        ,IO.""QryGroup64"" AS ""Properties64""
                                        FROM ""{Conexion.BD_Hechizo}"".OITM IO
                                        LEFT JOIN ""{bdDest}"".OITM MM ON IO.""ItemCode"" = MM.""ItemCode""
                                        LEFT JOIN ""_SYS_BIC"".""{Conexion.BD_Hechizo}/IC_GRP_CODE_FAMILIA"" GRPART ON GRPART.""COD_HCHZ"" = IO.""ItmsGrpCod""
                                        LEFT JOIN ""{Conexion.BD_Hechizo}"".OSHP F ON F.""TrnspCode"" = IO.""ShipType""
                                        WHERE
                                             IO.""U_ENVIOSUC"" = 'Y'
                                            AND IO.""{columnaActualizaDest}"" = 'Y'
                                            AND IO.""ItemType"" <> 'F'";


                    string queryPList = $@"SELECT  ""PriceList""
                                                    ,""Price""
                                                    ,""Currency""
                                                    ,""Factor""
                                                    ,""BasePLNum"" AS ""BasePriceList""
                                                    ,""UomEntry"" AS ""UoMPrices""
                                                    FROM  ""{Conexion.BD_Hechizo}"".""ITM1"" WHERE  ""ItemCode"" = ?";
                    //haydeeeeee
                    OdbcCommand CmD = new OdbcCommand(queryArt, conn);
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(bdDest, "fcscr-cloud\\" + stUSUARIO, stPASSWORD, UrlServiceLayer + "Login") != 0)
                            {

                                R.Codigo = -2;
                                R.Mensaje = RR;

                                objRegistraLog.Graba($"[{ DateTime.Now.ToString("HH:mm:ss tt")}] Sin conectar a Service Layer en el método ActualizararticuloTODASV2 en {bdDestino}, { R.Mensaje}: ");// grabar el error 

                                return R;
                            }

                            while (dr.Read())
                            {
                                string ItemCode = dr["ItemCode"].ToString();

                                JObject JsonObject = new JObject();
                                JsonObject = CrearObjetoJson(dr);

                                string Name = JsonObject["TrnspName"].ToString();
                                JsonObject.Remove("TrnspName");
                                JsonObject["ShipType"] = ConusltarFormasEnvio(Name, bdDest, conn);

                                JsonObject.Remove("ItemCode");

                                JArray jArrayprecios = new JArray();

                                OdbcCommand cmc = new OdbcCommand(queryPList, conn);
                                cmc.Parameters.Add(new OdbcParameter("@num", ItemCode));

                                using (OdbcDataReader dtr = cmc.ExecuteReader())
                                {
                                    jArrayprecios = CrearObjetoJson2(dtr);
                                }

                                JsonObject.Add(new JProperty("ItemPrices", jArrayprecios));
                                var myjs = JsonObject.ToString();
                                resul = EnviaSapPorSesion("PATCH", JsonObject, UrlServiceLayer + $"Items('{ItemCode}')");
                                if (resul == 0)
                                {
                                    queryArt = $@"UPDATE  ""{Conexion.BD_Hechizo}"".""OITM"" SET ""{columnaActualizaDest}"" = 'N' WHERE ""ItemCode"" = '{ItemCode}'";
                                    OdbcCommand cmand = new OdbcCommand(queryArt, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                }
                                else
                                {
                                    objRegistraLog.Graba($"Error actualizando artículo {ItemCode} en {bdDest}: " + RR + " " + ItemCode + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));

                                    Inserterror(bdDest, "OITM", ItemCode);
                                }

                                LiberaMemoria.FlushMemory();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Error en el proceso de actualizar artículos en bdDestino {bdDest}, {ex.Message}");// grabar el error
            }
            return R;
        }

        /// <summary>
        /// Crear vendedores 
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <returns></returns>
        public static Respuesta Crearvendedores(string bdDestino)
        {
            int resul = 0;
            string vendedor = string.Empty;

            Respuesta R = new Respuesta();
            try
            {

                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");
                string stUSUARIO = IniFil.LeerINI("ConexionHanaProd", "UserSL");
                string stPASSWORD = IniFil.LeerINI("ConexionHanaProd", "PassSL");

                string bdDest = ConsultarBD(bdDestino);

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    string query = $@"SELECT
                                       V.""SlpName"" AS ""SalesEmployeeName"",
                                       V.""Memo""  AS ""Remarks"",
                                       V.""SlpCode"" AS ""U_Code""
                                        FROM ""{Conexion.BD_Hechizo}"".OSLP V
                                        LEFT  JOIN ""{bdDest}"".OSLP VD on VD.""U_Code""= V.""SlpCode""
                                        WHERE VD.""U_Code"" IS NULL";


                    OdbcCommand CmD = new OdbcCommand(query, conn); // llama al Query Encabezado con la conexion
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(bdDest, "fcscr-cloud\\" + stUSUARIO, stPASSWORD, UrlServiceLayer + "Login") != 0)
                            {
                                R.Codigo = -2;
                                R.Mensaje = RR;

                                objRegistraLog.Graba($"Sin conectar a Service Layer en el método CrearVendedores en {bdDestino}: " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error 

                                return R;
                            }

                            while (dr.Read())
                            {

                                JObject JsonObject = new JObject();

                                JsonObject = CrearObjetoJson(dr);
                                vendedor = JsonObject["U_Code"].ToString();

                                var myjs = JsonObject.ToString();

                                resul = EnviaSapPorSesion("POST", JsonObject, UrlServiceLayer + "SalesPersons");

                                if (resul == 0) // Caso que inserto
                                {
                                    // Si tenía un error previo registrado en la tabla lo limpiamos
                                    objRegistraLog.Graba($"Vendedor {vendedor} creado en la bdDestino {bdDestino}" + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                                }
                                else
                                {
                                    objRegistraLog.Graba($"Error creando el vendedor  {vendedor} en la bdDestino {bdDestino}: " + RR + " " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));

                                    Inserterror(bdDestino, "OSLP", vendedor);
                                }

                                LiberaMemoria.FlushMemory();

                            }

                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al intentar Insertar vendedor : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
            return R;
        }

        /// <summary>
        /// Actualizar vendedores
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <returns></returns>
        public static Respuesta Actualizarvendedores(string bdDestino)
        {
            int resul = 0;
            string vend = string.Empty;

            Respuesta R = new Respuesta();
            try
            {

                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");
                string stUSUARIO = IniFil.LeerINI("ConexionHanaProd", "UserSL");
                string stPASSWORD = IniFil.LeerINI("ConexionHanaProd", "PassSL");
                string columnaActualizaDest = ConsultarColumna2(bdDestino);

                string bdDest = ConsultarBD(bdDestino);

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    string query = $@"SELECT
                                       V.""SlpName"" AS ""SalesEmployeeName"",
                                       V.""Memo""  AS ""Remarks"",
                                       VD.""SlpCode"",
                                        V.""SlpCode""  AS ""VendedorHechizo""
                                        FROM ""{Conexion.BD_Hechizo}"".OSLP V
                                        JOIN ""{bdDest}"".OSLP VD on VD.""U_Code""= V.""SlpCode""
                                        WHERE V.""{columnaActualizaDest}"" = 'Y'";


                    OdbcCommand CmD = new OdbcCommand(query, conn); // llama al Query Encabezado con la conexion
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(bdDest, "fcscr-cloud\\" + stUSUARIO, stPASSWORD, UrlServiceLayer + "Login") != 0)
                            {
                                R.Codigo = -2;
                                R.Mensaje = RR;

                                objRegistraLog.Graba($"Sin conectar a Service Layer en el método ActualizarVendedores en {bdDestino}: " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error 

                                return R;
                            }

                            while (dr.Read())
                            {

                                JObject JsonObject = new JObject();

                                JsonObject = CrearObjetoJson(dr);

                                int vendedor = Convert.ToInt32(JsonObject["SlpCode"]);
                                JsonObject.Remove("SlpCode");

                                vend = JsonObject["VendedorHechizo"].ToString();
                                JsonObject.Remove("VendedorHechizo");

                                var myjs = JsonObject.ToString();

                                resul = EnviaSapPorSesion("PATCH", JsonObject, UrlServiceLayer + $"SalesPersons({vendedor})");

                                if (resul == 0) // Caso que proceso
                                {
                                    // Si tenía un error previo registrado en la tabla lo limpiamos
                                    //objRegistraLog.Graba($"Vendedor {vendedor} creado en la bdDestino {bdDestino}" + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                                    string queryArt = $@"UPDATE  ""{Conexion.BD_Hechizo}"".""OSLP"" SET ""{columnaActualizaDest}"" = 'N' WHERE ""SlpCode"" = {vend}";
                                    OdbcCommand cmand = new OdbcCommand(queryArt, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                }
                                else
                                {
                                    objRegistraLog.Graba($"Error actualizando el vendedor  {vendedor} en la bdDestino {bdDestino}: " + RR + " " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));

                                    Inserterror(bdDestino, "OSLP", vend);
                                }

                                LiberaMemoria.FlushMemory();

                            }

                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al intentar actualizar vendedor : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
            return R;
        }

        /// <summary>
        /// Crear empleados
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <returns></returns>
        public static Respuesta CrearEmpleados(string bdDestino)
        {
            int resul = 0;
            string empleado = string.Empty;

            Respuesta R = new Respuesta();
            try
            {

                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");
                string stUSUARIO = IniFil.LeerINI("ConexionHanaProd", "UserSL");
                string stPASSWORD = IniFil.LeerINI("ConexionHanaProd", "PassSL");

                string bdDest = ConsultarBD(bdDestino);

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    string query = $@"SELECT 
                                        E.""empID"" AS ""U_Code""
                                       ,E.""firstName"" AS ""FirstName""
                                       ,E.""lastName"" AS ""LastName""
                                       ,V.""SlpCode"" AS ""SalesPersonCode""
                                       FROM ""{Conexion.BD_Hechizo}"".OHEM E
                                       LEFT  JOIN ""{bdDest}"".OHEM ED ON ED.""U_Code"" = E.""empID""
                                       JOIN  ""{bdDest}"".OSLP V ON V.""U_Code"" = E.""salesPrson""
                                       WHERE ED.""U_Code"" IS NULL";

                    OdbcCommand CmD = new OdbcCommand(query, conn); // llama al Query Encabezado con la conexion
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(bdDest, "fcscr-cloud\\" + stUSUARIO, stPASSWORD, UrlServiceLayer + "Login") != 0)
                            {
                                R.Codigo = -2;
                                R.Mensaje = RR;

                                objRegistraLog.Graba($"Sin conectar a Service Layer en el método CrearEmpleados en {bdDestino}: " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error 

                                return R;
                            }

                            while (dr.Read())
                            {

                                JObject JsonObject = new JObject();

                                JsonObject = CrearObjetoJson(dr);
                                empleado = JsonObject["U_Code"].ToString();

                                var myjs = JsonObject.ToString();

                                resul = EnviaSapPorSesion("POST", JsonObject, UrlServiceLayer + "EmployeesInfo");

                                if (resul == 0) // Caso que inserto
                                {
                                    // Si tenía un error previo registrado en la tabla lo limpiamos
                                    objRegistraLog.Graba($"Empleado {empleado} creado en la bdDestino {bdDestino}" + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                                }
                                else
                                {
                                    objRegistraLog.Graba($"Error creando el empleado  {empleado} en la bdDestino {bdDestino}: " + RR + " " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));

                                    Inserterror(bdDestino, "OHEM", empleado);
                                }

                                LiberaMemoria.FlushMemory();
                            }

                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al intentar Insertar Empleado : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
            return R;
        }

        /// <summary>
        /// Actualizar estados
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <returns></returns>
        public static Respuesta ActualizarEmpleados(string bdDestino)
        {
            int resul = 0;
            string vend = string.Empty;

            Respuesta R = new Respuesta();
            try
            {

                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");
                string stUSUARIO = IniFil.LeerINI("ConexionHanaProd", "UserSL");
                string stPASSWORD = IniFil.LeerINI("ConexionHanaProd", "PassSL");
                string columnaActualizaDest = ConsultarColumna2(bdDestino);

                string bdDest = ConsultarBD(bdDestino);

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    string query = $@"SELECT 
                                        E.""empID"" AS ""EmplHechizo""
                                       ,ED.""empID"" AS ""U_Code""
                                       ,E.""firstName"" AS ""FirstName""
                                       ,E.""lastName"" AS ""LastName""
                                       ,V.""SlpCode"" AS ""SalesPersonCode""
                                       FROM ""{Conexion.BD_Hechizo}"".OHEM E
                                       LEFT  JOIN ""{bdDest}"".OHEM ED ON ED.""U_Code"" = E.""empID""
                                       JOIN  ""{bdDest}"".OSLP V ON V.""U_Code"" = E.""salesPrson""
                                       WHERE E.""{columnaActualizaDest}"" = 'Y'";


                    OdbcCommand CmD = new OdbcCommand(query, conn); // llama al Query Encabezado con la conexion
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(bdDest, "fcscr-cloud\\" + stUSUARIO, stPASSWORD, UrlServiceLayer + "Login") != 0)
                            {
                                R.Codigo = -2;
                                R.Mensaje = RR;

                                objRegistraLog.Graba($"Sin conectar a Service Layer en el método Actualizar Empleados en {bdDestino}: " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error 

                                return R;
                            }

                            while (dr.Read())
                            {

                                JObject JsonObject = new JObject();

                                JsonObject = CrearObjetoJson(dr);

                                string empleado = JsonObject["U_Code"].ToString();
                                JsonObject.Remove("U_Code");

                                string empleadoHech = JsonObject["EmplHechizo"].ToString();
                                JsonObject.Remove("EmplHechizo");

                                var myjs = JsonObject.ToString();

                                resul = EnviaSapPorSesion("PATCH", JsonObject, UrlServiceLayer + $"EmployeesInfo({empleado})");

                                if (resul == 0) // Caso que proceso
                                {
                                    // Si tenía un error previo registrado en la tabla lo limpiamos
                                    //objRegistraLog.Graba($"Empleado {vendedor} creado en la bdDestino {bdDestino}" + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                                    string queryArt = $@"UPDATE  ""{Conexion.BD_Hechizo}"".""OHEM"" SET ""{columnaActualizaDest}"" = 'N' WHERE ""empID"" = {empleadoHech}";
                                    OdbcCommand cmand = new OdbcCommand(queryArt, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                }
                                else
                                {
                                    objRegistraLog.Graba($"Error actualizando el vendedor  {empleado} en la bdDestino {bdDestino}: " + RR + " " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));

                                    Inserterror(bdDestino, "OHEM", empleado);
                                }

                                LiberaMemoria.FlushMemory();

                            }

                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al intentar actualizar vendedor : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
            return R;
        }

        /// <summary>
        /// Consulta el código de froma de envio recibido por Hechizo
        /// </summary>
        /// <param name="NomBRE"></param>
        /// <returns></returns>
        private static int ConusltarFormasEnvio(string Nombre, string BDdestisno, OdbcConnection conn)
        {
            int Codigo = -1; ;
            try
            {
                string queryVend = $"SELECT \"TrnspCode\" FROM \"{BDdestisno}\".\"OSHP\" WHERE \"TrnspName\" = '{Nombre.Trim()}' ";
                OdbcCommand Cm = new OdbcCommand(queryVend, conn);
                using (OdbcDataReader dtr = Cm.ExecuteReader())
                {
                    while (dtr.Read())
                    {
                        Codigo = Convert.ToInt32(dtr["TrnspCode"]);
                    }
                }

                return Codigo;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static Respuesta ActualizararticuloTODAS(string bdDestino, string tokendestino)
        {
            int resul = 0;
            string nomcod = string.Empty;
            string bdDest = ConsultarBD(bdDestino);
            string columnaActualizaDest = ConsultarColumna(bdDestino);
            int cuantos = 0;
            Respuesta R = new Respuesta();
            try
            {

                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");
                string stUSUARIO = IniFil.LeerINI("ConexionHanaProd", "UserSL");
                string stPASSWORD = IniFil.LeerINI("ConexionHanaProd", "PassSL");

                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                using (OdbcConnection conn2 = new OdbcConnection(Conexion.strCon))
                using (OdbcConnection conn3 = new OdbcConnection(Conexion.strCon))
                {
                    string query = $"CALL \"{Conexion.BD_Hechizo}\".IC_UPD_ITEMS2('{bdDest}')"; //Query Encabezado
                    OdbcCommand CmD = new OdbcCommand(query, conn); // llama al Query Encabezado con la conexion
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.RecordsAffected != 0)
                        {
                            if (conecta_service_layer(bdDest, "fcscr-cloud\\" + stUSUARIO, stPASSWORD, UrlServiceLayer + "Login") != 0)
                            {

                                R.Codigo = -2;
                                R.Mensaje = RR;

                                objRegistraLog.Graba($"Sin conectar a Service Layer en el método ActualizararticuloTODAS en {bdDest}: " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error 

                                return R;
                            }

                            while (dr.Read())
                            {

                                nomcod = dr["ItemCode"].ToString();
                                string queryEn = $"select count(\"ItemCode\") \"conteo\"  from  \"{bdDest}\".\"OITM\" where \"ItemCode\" = '" + nomcod + "'";
                                OdbcCommand CmD1 = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                                conn2.Open();
                                using (OdbcDataReader dr2 = CmD1.ExecuteReader())
                                    while (dr2.Read())
                                    {
                                        cuantos = Convert.ToInt32(dr2["conteo"]);
                                        if (cuantos == 0)
                                        {
                                            JObject JsonObject = new JObject();
                                            JsonObject = CrearObjetoJson(dr);
                                            JArray jArrayprecios = new JArray();
                                            string crDet = $@"
                                                                SELECT 
                                                                    ""PriceList"",
                                                                    ""Price"",
                                                                    ""Currency"",
                                                                    ""Factor"",
                                                                    ""BasePriceList"",
                                                                    ""UoMPrices""
                                                                FROM (
                                                                    SELECT  
                                                                        T1.""PriceList"",
                                                                        T1.""Price"",
                                                                        T1.""Currency"",
                                                                        T1.""Factor"",
                                                                        T1.""BasePLNum"" AS ""BasePriceList"",
                                                                        T1.""UomEntry"" AS ""UoMPrices""
                                                                    FROM ""{Conexion.BD_Hechizo}"".""ITM1"" T1
                                                                    WHERE T1.""ItemCode"" = '{nomcod}'
                                                                      AND T1.""BasePLNum"" IN (-1,-2)

                                                                    UNION ALL

                                                                    SELECT 
                                                                        T2.""PriceList"",
                                                                        T2.""Price"",
                                                                        T2.""Currency"",
                                                                        T2.""Factor"",
                                                                        T2.""BasePLNum"" AS ""BasePriceList"",
                                                                        T2.""UomEntry"" AS ""UoMPrices""
                                                                    FROM ""{Conexion.BD_Hechizo}"".""ITM1"" T2
                                                                    WHERE T2.""ItemCode"" = '{nomcod}'
                                                                      AND T2.""PriceList"" <> '1'
                                                                      AND T2.""Ovrwrite1"" = 'Y'
                                                                ) X";
                                            OdbcCommand cmc = new OdbcCommand(crDet, conn3);
                                            conn3.Open();
                                            using (OdbcDataReader dtr = cmc.ExecuteReader())
                                            {
                                                jArrayprecios = CrearObjetoJson2(dtr);
                                                string Name = JsonObject["TrnspName"].ToString();
                                                JsonObject.Remove("TrnspName");
                                                JsonObject["ShipType"] = ConusltarFormasEnvio(Name, bdDest, conn);
                                                JsonObject.Add(new JProperty("ItemPrices", jArrayprecios));
                                                var myjs = JsonObject.ToString();
                                                resul = EnviaSapPorSesion("POST", JsonObject, UrlServiceLayer + "Items");//
                                                if (resul == 0) // Caso que actualizo
                                                {

                                                    objRegistraLog.Graba("Actualizado a Sap el articulo : " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio
                                                                                                                                                                               // objRegistraLog.Graba("actualizado en Sap el articulo : " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio


                                                    string queryArt = $@"UPDATE  ""{Conexion.BD_Hechizo}"".""OITM"" SET ""{columnaActualizaDest}"" = 'N' WHERE ""ItemCode"" = '{nomcod}'";
                                                    OdbcCommand cmand = new OdbcCommand(queryArt, conn);
                                                    OdbcDataReader dr1 = cmand.ExecuteReader();


                                                }
                                                else
                                                {
                                                    objRegistraLog.Graba($"Error Actualizando artículo en la base destinno {bdDestino}: " + RR + " " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));//

                                                    Inserterror(bdDestino, "OITM", nomcod);
                                                }
                                            }
                                            conn3.Close();
                                        }
                                        else
                                        {

                                            JObject JsonObject = new JObject();
                                            JsonObject = CrearObjetoJson(dr);

                                            JsonObject.Remove("ItemCode");
                                            string Name = JsonObject["TrnspName"].ToString();
                                            JsonObject.Remove("TrnspName");
                                            JsonObject["ShipType"] = ConusltarFormasEnvio(Name, bdDest, conn);

                                            JArray jArrayprecios = new JArray();
                                            string crDet = $"SELECT  \"PriceList\",\"Price\",\"Currency\",\"Factor\",\"BasePLNum\" as \"BasePriceList\" ,\"UomEntry\" as \"UoMPrices\" FROM  \"{Conexion.BD_Hechizo}\".\"ITM1\" where  \"ItemCode\" = '" + nomcod + "'";
                                            OdbcCommand cmc = new OdbcCommand(crDet, conn3);
                                            conn3.Open();
                                            using (OdbcDataReader dtr = cmc.ExecuteReader())
                                            {
                                                jArrayprecios = CrearObjetoJson2(dtr);
                                                JsonObject.Add(new JProperty("ItemPrices", jArrayprecios));
                                                var myjs = JsonObject.ToString();
                                                resul = EnviaSapPorSesion("PATCH", JsonObject, UrlServiceLayer + "Items('" + nomcod + "')");//
                                                if (resul == 0) // Caso que inserto
                                                {


                                                    string queryArt = $@"UPDATE  ""{Conexion.BD_Hechizo}"".""OITM"" SET ""{columnaActualizaDest}"" = 'N' WHERE ""ItemCode"" = '{nomcod}'";
                                                    OdbcCommand cmand = new OdbcCommand(queryArt, conn);
                                                    OdbcDataReader DR1 = cmand.ExecuteReader();

                                                }
                                                else
                                                {
                                                    objRegistraLog.Graba($"Error actualizando artículo {nomcod} en {bdDest}: " + RR + " " + nomcod + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));

                                                    Inserterror(bdDest, "OITM", nomcod);
                                                }
                                            }
                                            conn3.Close();
                                        }
                                        LiberaMemoria.FlushMemory();
                                    }
                                conn2.Close();
                            }
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

        public static Respuesta CrearGrupoDescuento(string bdDestino, string TokenDetino)
        {
            int resul = 0;
            int absEntry = 0;
            Respuesta R = new Respuesta();
            IniFile IniFil;
            IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
            string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                using (OdbcConnection connDet = new OdbcConnection(Conexion.strCon))
                {
                    string query = $"CALL \"{Conexion.BD_Hechizo}\".IC_GRUPO_DSCTO";
                    OdbcCommand cmd = new OdbcCommand(query, conn);

                    conn.Open();
                    using (OdbcDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            absEntry = Convert.ToInt32(dr["AbsEntry"]);
                            string url = $"{UrlServiceLayer}EnhancedDiscountGroups({absEntry})";
                            // 1️⃣ VALIDAR SI YA EXISTE EN SERVICE LAYER
                            int existe = EnviaSapPorBasicAuth("GET", null,
                               url,
                                TokenDetino);


                            // 3️⃣ ARMAR JSON ENCABEZADO
                            JObject json = new JObject
                            {
                                { "AbsEntry", absEntry },
                                { "Type", dr["Type"].ToString() },
                                { "ObjectCode", dr["ObjectCode"].ToString() },
                                { "DiscountRelations", dr["DiscountRelations"].ToString() },
                                { "Active", dr["Active"].ToString() },
                                { "ValidFrom", dr.IsDBNull(dr.GetOrdinal("ValidFrom"))? null: Convert.ToDateTime(dr["ValidFrom"]).ToString("yyyy-MM-ddT00:00:00Z") },
                                { "ValidTo", dr.IsDBNull(dr.GetOrdinal("ValidTo"))? null: Convert.ToDateTime(dr["ValidTo"]).ToString("yyyy-MM-ddT00:00:00Z")}

                            };


                            // 4️⃣ ARMAR ARRAY DE LÍNEAS
                            JArray jArrayLineas = new JArray();

                            string qDet = $"CALL \"{Conexion.BD_Hechizo}\".IC_GRUPO_DSCTO_DET({absEntry})";
                            OdbcCommand cmdDet = new OdbcCommand(qDet, connDet);
                            connDet.Open();

                            using (OdbcDataReader drDet = cmdDet.ExecuteReader())
                            {
                                while (drDet.Read())
                                {
                                    JObject linea = new JObject
                                    {
                                        { "AbsEntry", absEntry },
                                        { "ObjectType", drDet["ObjectType"].ToString() },
                                        { "ObjectCode", drDet["ObjectCode"].ToString() },
                                        { "DiscountType", drDet["DiscountType"].ToString() },
                                        { "Discount", Convert.ToDouble(drDet["Discount"]) },
                                        { "PaidQuantity", Convert.ToDouble(drDet["PaidQuantity"]) },
                                        { "FreeQuantity", Convert.ToDouble(drDet["FreeQuantity"]) },
                                        { "MaximumFreeQuantity", Convert.ToDouble(drDet["MaximumFreeQuantity"]) }
                                    };

                                    jArrayLineas.Add(linea);
                                }
                            }
                            connDet.Close();

                            json.Add("DiscountGroupLineCollection", jArrayLineas);

                            var myjs = json.ToString();

                            if ((existe == 0))
                            {
                                // json.Remove("AbsEntry");
                                resul = EnviaSapPorBasicAuth(
                            "PATCH",
                            json,
                            $"{UrlServiceLayer}EnhancedDiscountGroups({absEntry})",
                            TokenDetino);

                            }
                            else
                            {
                                resul = EnviaSapPorBasicAuth(
                                    "POST",
                                    json,
                                    $"{UrlServiceLayer}EnhancedDiscountGroups",
                                    TokenDetino);

                            }

                            if (resul == 0)
                            {
                                objRegistraLog.Graba("Enhanced Discount Group insertado: AbsEntry " + absEntry + " Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));

                                if (bdDestino == Conexion.BD_desampa)
                                {
                                    query = $"UPDATE  \"{Conexion.BD_Hechizo}\".\"OEDG\" SET \"U_POS_SYNC\" = 'Y' WHERE \"AbsEntry\" =  " + absEntry + "";
                                    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    OdbcDataReader dr1 = cmand.ExecuteReader();
                                }

                            }
                            else
                            {
                                objRegistraLog.Graba($"Error insertando Grp Descuentos en la BD Destino {bdDestino} AbsEntry {absEntry}: " + RR + " / " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));//

                                Inserterror(bdDestino, "OEDG", absEntry.ToString());
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
                R.Mensaje = "Error: " + ex.Message;
                objRegistraLog.Graba("Error Enhanced Discount Group: " + ex.Message);
            }

            return R;
        }

        public static Respuesta CrearGrupoDescuentoV2(string bdDestino, string TokenDetino)
        {
            int resul = 0;
            int absEntry = 0;
            Respuesta R = new Respuesta();
            IniFile IniFil;
            IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
            string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");
            //string bdDest = ConsultarBD(bdDestino);
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                using (OdbcConnection connDet = new OdbcConnection(Conexion.strCon))
                {
                    string query = $@"SELECT
                                            H.""AbsEntry"",
                                            CASE H.""Type""
                                                WHEN 'S' THEN 'dgt_SpecificBP'
                                                WHEN 'C' THEN 'dgt_CustomerGroup'
                                                WHEN 'V' THEN 'dgt_VendorGroup'
                                                WHEN 'A' THEN 'dgt_AllBPs'
                                                ELSE H.""Type""
                                            END AS ""Type"",
                                            H.""ObjCode"" AS ""ObjectCode"",
                                            CASE H.""DiscRel""
                                                WHEN 'L' THEN 'dgrLowestDiscount'
                                                WHEN 'H' THEN 'dgrHighestDiscount'
                                                WHEN 'A' THEN 'dgrAverageDiscount'
                                                WHEN 'T' THEN 'dgrDiscountTotals'
                                                WHEN 'M' THEN 'dgrMultipliedDiscount'
                                                ELSE H.""DiscRel""
                                            END AS ""DiscountRelations"",
                                            CASE H.""ValidFor""
                                                WHEN 'Y' THEN 'tYES'
                                                WHEN 'N' THEN 'tNO'
                                            END AS ""Active"",
                                            H.""ValidForm"" AS ""ValidFrom"",
                                            H.""ValidTo"" AS ""ValidTo""
                                        FROM ""{Conexion.BD_Hechizo}"".""OEDG"" H
                                        LEFT JOIN ""{bdDestino}"".""OEDG"" S ON S.""U_Version"" = H.""AbsEntry""
                                        WHERE S.""U_Version"" IS NULL
                                        ORDER BY H.""AbsEntry""";



                    OdbcCommand cmd = new OdbcCommand(query, conn);

                    conn.Open();
                    using (OdbcDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            if (dr.HasRows)
                            {
                                absEntry = Convert.ToInt32(dr["AbsEntry"]);

                                // 3️⃣ ARMAR JSON ENCABEZADO
                                    JObject json = new JObject
                                {
                                    { "AbsEntry", absEntry },
                                    //{ "U_Version", absEntry },
                                    { "Type", dr["Type"].ToString() },
                                    { "ObjectCode", dr["ObjectCode"].ToString() },
                                    { "DiscountRelations", dr["DiscountRelations"].ToString() },
                                    { "Active", dr["Active"].ToString() },
                                    { "ValidFrom", dr.IsDBNull(dr.GetOrdinal("ValidFrom"))? null: Convert.ToDateTime(dr["ValidFrom"]).ToString("yyyy-MM-ddT00:00:00Z") },
                                    { "ValidTo", dr.IsDBNull(dr.GetOrdinal("ValidTo"))? null: Convert.ToDateTime(dr["ValidTo"]).ToString("yyyy-MM-ddT00:00:00Z")}

                                };

                                // 4️⃣ ARMAR ARRAY DE LÍNEAS
                                JArray jArrayLineas = new JArray();

                                string qDet = $@"SELECT
                                                        CASE L.""ObjType"" 
                                                            WHEN '4' THEN 'dgboItems'
                                                            WHEN '52' THEN 'dgboItemGroups'
                                                            WHEN '8' THEN 'dgboItemProperties'
                                                        END AS ""ObjectType"",
                                                        L.""ObjKey"" AS ""ObjectCode"",
                                                        CASE L.""DiscType""
                                                            WHEN 'D' THEN 'dgdt_Fixed'
                                                            WHEN 'V' THEN 'dgdt_Variable'
                                                            ELSE L.""DiscType""
                                                        END AS ""DiscountType"",
                                                        L.""Discount"",
                                                        L.""PayFor"" AS ""PaidQuantity"",
                                                        L.""ForFree"" AS ""FreeQuantity"",
                                                        L.""UpTo"" AS ""MaximumFreeQuantity""
                                                                FROM {Conexion.BD_Hechizo}.""EDG1"" L
                                                                WHERE L.""AbsEntry"" = ?
                                                                ORDER BY
                                                                    L.""ObjType"",
                                                                    L.""ObjKey""";

                                OdbcCommand cmdDet = new OdbcCommand(qDet, connDet);
                                cmdDet.Parameters.Add(new OdbcParameter("@code", absEntry));
                                connDet.Open();

                                using (OdbcDataReader drDet = cmdDet.ExecuteReader())
                                {
                                    while (drDet.Read())
                                    {
                                        JObject linea = new JObject
                                        {
                                            { "AbsEntry", absEntry },
                                            { "ObjectType", drDet["ObjectType"].ToString() },
                                            { "ObjectCode", drDet["ObjectCode"].ToString() },
                                            { "DiscountType", drDet["DiscountType"].ToString() },
                                            { "Discount", Convert.ToDouble(drDet["Discount"]) },
                                            { "PaidQuantity", Convert.ToDouble(drDet["PaidQuantity"]) },
                                            { "FreeQuantity", Convert.ToDouble(drDet["FreeQuantity"]) },
                                            { "MaximumFreeQuantity", Convert.ToDouble(drDet["MaximumFreeQuantity"]) }
                                        };

                                        jArrayLineas.Add(linea);
                                    }
                                }
                                connDet.Close();

                                json.Add("DiscountGroupLineCollection", jArrayLineas);

                                string myjs = json.ToString();

                                resul = EnviaSapPorBasicAuth("POST", json, $"{UrlServiceLayer}EnhancedDiscountGroups", TokenDetino);
                             

                                if (resul == 0)
                                {
                                    objRegistraLog.Graba("Enhanced Discount Group insertado: AbsEntry " + absEntry + " Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));

                                }
                                else
                                {
                                    objRegistraLog.Graba($"Error insertando Grp Descuentos en la BD Destino {bdDestino} AbsEntry {absEntry}: " + RR + " / " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));//

                                    Inserterror(bdDestino, "OEDG", absEntry.ToString());
                                }

                                LiberaMemoria.FlushMemory();
                            }


                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Error: " + ex.Message;
                objRegistraLog.Graba("Error Enhanced Discount Group: " + ex.Message);
            }

            return R;
        }

        public static Respuesta ActualizarGrupoDescuentoV2(string bdDestino, string TokenDetino)
        {
            int resul = 0;
            int absEntry = 0;
            Respuesta R = new Respuesta();
            IniFile IniFil;
            IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
            string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");
            string bdDest = ConsultarBD(bdDestino); 
            string columnaActualizaDest = ConsultarColumna(bdDestino);
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                using (OdbcConnection connDet = new OdbcConnection(Conexion.strCon))
                {
                    string query = $@"SELECT
                                            H.""AbsEntry"",
                                            CASE H.""Type""
                                                WHEN 'S' THEN 'dgt_SpecificBP'
                                                WHEN 'C' THEN 'dgt_CustomerGroup'
                                                WHEN 'V' THEN 'dgt_VendorGroup'
                                                WHEN 'A' THEN 'dgt_AllBPs'
                                                ELSE H.""Type""
                                            END AS ""Type"",
                                            H.""ObjCode"" AS ""ObjectCode"",
                                            CASE H.""DiscRel""
                                                WHEN 'L' THEN 'dgrLowestDiscount'
                                                WHEN 'H' THEN 'dgrHighestDiscount'
                                                WHEN 'A' THEN 'dgrAverageDiscount'
                                                WHEN 'T' THEN 'dgrDiscountTotals'
                                                WHEN 'M' THEN 'dgrMultipliedDiscount'
                                                ELSE H.""DiscRel""
                                            END AS ""DiscountRelations"",
                                            CASE H.""ValidFor""
                                                WHEN 'Y' THEN 'tYES'
                                                WHEN 'N' THEN 'tNO'
                                            END AS ""Active"",
                                            H.""ValidForm"" AS ""ValidFrom"",
                                            H.""ValidTo"" AS ""ValidTo""
                                        FROM ""{Conexion.BD_Hechizo}"".""OEDG"" H
                                        WHERE S.""{columnaActualizaDest}"" = 'Y'
                                        ORDER BY H.""AbsEntry""";

                    OdbcCommand cmd = new OdbcCommand(query, conn);
                    

                    conn.Open();
                    using (OdbcDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            if (dr.HasRows)
                            {
                                absEntry = Convert.ToInt32(dr["AbsEntry"]);

                                // 3️⃣ ARMAR JSON ENCABEZADO
                                JObject json = new JObject
                            {
                               // { "U_Version", absEntry },
                                { "Type", dr["Type"].ToString() },
                                { "ObjectCode", dr["ObjectCode"].ToString() },
                                { "DiscountRelations", dr["DiscountRelations"].ToString() },
                                { "Active", dr["Active"].ToString() },
                                { "ValidFrom", dr.IsDBNull(dr.GetOrdinal("ValidFrom"))? null: Convert.ToDateTime(dr["ValidFrom"]).ToString("yyyy-MM-ddT00:00:00Z") },
                                { "ValidTo", dr.IsDBNull(dr.GetOrdinal("ValidTo"))? null: Convert.ToDateTime(dr["ValidTo"]).ToString("yyyy-MM-ddT00:00:00Z")}

                            };

                                // 4️⃣ ARMAR ARRAY DE LÍNEAS
                                JArray jArrayLineas = new JArray();

                                string qDet = $@"SELECT
                                                        CASE L.""ObjType"" 
                                                            WHEN '4' THEN 'dgboItems'
                                                            WHEN '52' THEN 'dgboItemGroups'
                                                            WHEN '8' THEN 'dgboItemProperties'
                                                        END AS ""ObjectType"",
                                                        L.""ObjKey"" AS ""ObjectCode"",
                                                        CASE L.""DiscType""
                                                            WHEN 'D' THEN 'dgdt_Fixed'
                                                            WHEN 'V' THEN 'dgdt_Variable'
                                                            ELSE L.""DiscType""
                                                        END AS ""DiscountType"",
                                                        L.""Discount"",
                                                        L.""PayFor"" AS ""PaidQuantity"",
                                                        L.""ForFree"" AS ""FreeQuantity"",
                                                        L.""UpTo"" AS ""MaximumFreeQuantity""
                                                                FROM {Conexion.BD_Hechizo}EDG1 L
                                                                WHERE L.""AbsEntry"" = ?
                                                                ORDER BY
                                                                    L.""ObjType"",
                                                                    L.""ObjKey""";

                                OdbcCommand cmdDet = new OdbcCommand(qDet, connDet);
                                cmdDet.Parameters.Add(new OdbcParameter("@code", absEntry));
                                connDet.Open();

                                using (OdbcDataReader drDet = cmdDet.ExecuteReader())
                                {
                                    while (drDet.Read())
                                    {
                                        JObject linea = new JObject
                                        {
                                            //{ "AbsEntry", absEntry },
                                            { "ObjectType", drDet["ObjectType"].ToString() },
                                            { "ObjectCode", drDet["ObjectCode"].ToString() },
                                            { "DiscountType", drDet["DiscountType"].ToString() },
                                            { "Discount", Convert.ToDouble(drDet["Discount"]) },
                                            { "PaidQuantity", Convert.ToDouble(drDet["PaidQuantity"]) },
                                            { "FreeQuantity", Convert.ToDouble(drDet["FreeQuantity"]) },
                                            { "MaximumFreeQuantity", Convert.ToDouble(drDet["MaximumFreeQuantity"]) }
                                        };

                                        jArrayLineas.Add(linea);
                                    }
                                }
                                connDet.Close();

                                json.Add("DiscountGroupLineCollection", jArrayLineas);

                                var myjs = json.ToString();

                                //if ((existe == 0))
                                //{
                                // json.Remove("AbsEntry");--comentado desde el código de Hayde
                                resul = EnviaSapPorBasicAuth("PATCH", json, $"{UrlServiceLayer}EnhancedDiscountGroups({absEntry})", TokenDetino);

                                //}
                                //else
                                //{
                                //    resul = EnviaSapPorBasicAuth("POST", json, $"{UrlServiceLayer}EnhancedDiscountGroups", TokenDetino);

                                //}

                                if (resul == 0)
                                {
                                    objRegistraLog.Graba("Enhanced Discount Group insertado: AbsEntry " + absEntry + " Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));

                                    //if (bdDestino == Conexion.BD_desampa)
                                    //{
                                    //    query = $"UPDATE  \"{Conexion.BD_Hechizo}\".\"OEDG\" SET \"U_POS_SYNC\" = 'Y' WHERE \"AbsEntry\" =  " + absEntry + "";
                                    //    OdbcCommand cmand = new OdbcCommand(query, conn);
                                    //    OdbcDataReader dr1 = cmand.ExecuteReader();
                                    //}

                                }
                                else
                                {
                                    objRegistraLog.Graba($"Error insertando Grp Descuentos en la BD Destino {bdDestino} AbsEntry {absEntry}: " + RR + " / " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));//

                                    Inserterror(bdDestino, "OEDG", absEntry.ToString());
                                }

                                LiberaMemoria.FlushMemory();
                            }


                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Error: " + ex.Message;
                objRegistraLog.Graba("Error Enhanced Discount Group: " + ex.Message);
            }

            return R;
        }

        public static Respuesta CrearSOCIOTODAS(string bdOrigen, string bdDestino, string TokenDestino)
        {
            int resul = 0;
            string nomcod = string.Empty;
            string cardCode = string.Empty;
            int cuantos = 0;
            string bd2 = string.Empty;
            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                using (OdbcConnection conn2 = new OdbcConnection(Conexion.strCon))
                using (OdbcConnection conn3 = new OdbcConnection(Conexion.strCon))
                {
                    //string query = $"CALL \"{bdOrigen}\".IC_ADD_SN('{bdDestino}')"; //Query Encabezado
                    string query = $@"SELECT 
                                1 as ""Series""
                                ,IO.""CardCode""
                                ,IO.""CardName""
                                ,IO.""CardType"" AS ""CardType"" 
                                ,""{bdOrigen}"".""ICgrupoSN""('{bdDestino}',(select ""GroupCode"" from ""{bdOrigen}"".OCRG where ""GroupCode""=IO.""GroupCode""))  ""GroupCode""
                                ,IO.""Phone1"" as ""Phone1""
                                ,""{bdOrigen}"".""ICcondicionesSN""('{bdDestino}',(select ""GroupNum"" from ""{bdOrigen}"".OCTG where ""GroupNum""=IO.""GroupNum"")) ""PayTermsGrpCode""
                                ,IO.""Phone2"" as ""Phone2""
                                ,IO.""Cellular"" as ""Cellular""
                                ,IO.""CreditLine"" as ""CreditLimit""
                                ,IO.""LicTradNum"" AS ""FederalTaxID""
                                ,IO.""E_Mail"" AS ""EmailAddress""
                                ,IO.""AddID"" as ""AdditionalID""
                                ,IO.""Currency"" AS ""Currency""
                                ,IO.""validFor"" AS ""Valid""
                                ,IO.""frozenFor"" as ""Frozen""
                                ,IO.""ListNum"" as ""PriceListNum""
                                ,IO.""DebtLine"" as ""MaxCommitment""
                                ,IO.""U_Tipo_Doc_Elect""
                                ,IO.""U_tipo_identificacion""
                                ,IO.""U_tipo_documento""
                                ,IO.""U_aplica_exoneracion""
                                ,IO.""U_numero_documento""
                                ,IO.""U_nombre_institucion""
                                ,IO.""U_porcentaje_exoneracion""
                                ,IO.""U_fecha_emision""
                                ,IO.""U_fecha_vencimiento_Exonera""
                                ,IO.""U_EXO""
                                ,IO.""U_ActividadReceptor""
                                ,IO.""U_SAFE_NumId""
                                ,IO.""U_TipoDocExoAutorizacionOTRO""
                                ,IO.""U_Inciso""
                                ,IO.""U_Articulo""
                                ,IO.""U_NombreInstitucionOtros""
                                ,IO.""U_Cadenas""
                                ,IO.""U_NumeroVendedor""
                                ,IO.""U_GLN""
                                ,IO.""U_ConsumirdorFinal""
                                ,IO.""U_CondicionVenta""
                                ,IO.""U_CondicionVentaOTRO""
                                ,IO.""DpmIntAct"" as ""DownPaymentClearAct""
                                ,IO.""Free_Text"" as ""FreeText""
                                ,IO.""QryGroup1"" as ""Properties1""
                                ,IO.""QryGroup2"" as ""Properties2""
                                ,IO.""QryGroup3"" as ""Properties3""
                                ,IO.""QryGroup4"" as ""Properties4""
                                ,IO.""QryGroup5"" as ""Properties5""
                                ,IO.""QryGroup6"" as ""Properties6""
                                ,IO.""QryGroup7"" as ""Properties7""
                                ,IO.""QryGroup8"" as ""Properties8""
                                ,IO.""QryGroup9"" as ""Properties9""
                                ,IO.""QryGroup10"" as ""Properties10""
                                ,IO.""QryGroup11"" as ""Properties11""
                                ,IO.""QryGroup12"" as ""Properties12""
                                ,IO.""QryGroup13"" as ""Properties13""
                                ,IO.""QryGroup14"" as ""Properties14""
                                ,IO.""QryGroup15"" as ""Properties15""
                                ,IO.""QryGroup16"" as ""Properties16""
                                ,IO.""QryGroup17"" as ""Properties17""
                                ,IO.""QryGroup18"" as ""Properties18""
                                ,IO.""QryGroup19"" as ""Properties19""
                                ,IO.""QryGroup20"" as ""Properties20""
                                ,IO.""QryGroup21"" as ""Properties21""
                                ,IO.""QryGroup22"" as ""Properties22""
                                ,IO.""QryGroup23"" as ""Properties23""
                                ,IO.""QryGroup24"" as ""Properties24""
                                ,IO.""QryGroup25"" as ""Properties25""
                                ,IO.""QryGroup26"" as ""Properties26""
                                ,IO.""QryGroup27"" as ""Properties27""
                                ,IO.""QryGroup28"" as ""Properties28""
                                ,IO.""QryGroup29"" as ""Properties29""
                                ,IO.""QryGroup30"" as ""Properties30""
                                ,IO.""QryGroup31"" as ""Properties31""
                                ,IO.""QryGroup32"" as ""Properties32""
                                ,IO.""QryGroup33"" as ""Properties33""
                                ,IO.""QryGroup34"" as ""Properties34""
                                ,IO.""QryGroup35"" as ""Properties35""
                                ,IO.""QryGroup36"" as ""Properties36""
                                ,IO.""QryGroup37"" as ""Properties37""
                                ,IO.""QryGroup38"" as ""Properties38""
                                ,IO.""QryGroup39"" as ""Properties39""
                                ,IO.""QryGroup40"" as ""Properties40""
                                ,IO.""QryGroup41"" AS ""Properties41""
                                ,IO.""QryGroup42"" AS ""Properties42""
                                ,IO.""QryGroup43"" AS ""Properties43""
                                ,IO.""QryGroup44"" AS ""Properties44""
                                ,IO.""QryGroup45"" AS ""Properties45""
                                ,IO.""QryGroup46"" AS ""Properties46""
                                ,IO.""QryGroup47"" AS ""Properties47""
                                ,IO.""QryGroup48"" AS ""Properties48""
                                ,IO.""QryGroup49"" AS ""Properties49""
                                ,IO.""QryGroup50"" AS ""Properties50""
                                ,IO.""QryGroup51"" AS ""Properties51""
                                ,IO.""QryGroup52"" AS ""Properties52""
                                ,IO.""QryGroup53"" AS ""Properties53""
                                ,IO.""QryGroup54"" AS ""Properties54""
                                ,IO.""QryGroup55"" AS ""Properties55""
                                ,IO.""QryGroup56"" AS ""Properties56""
                                ,IO.""QryGroup57"" AS ""Properties57""
                                ,IO.""QryGroup58"" AS ""Properties58""
                                ,IO.""QryGroup59"" AS ""Properties59""
                                ,IO.""QryGroup60"" AS ""Properties60""
                                ,IO.""QryGroup61"" AS ""Properties61""
                                ,IO.""QryGroup62"" AS ""Properties62""
                                ,IO.""QryGroup63"" AS ""Properties63""
                                ,IO.""QryGroup64"" AS ""Properties64""
                                ,'1' AS ""U_Enviado"" 
                                FROM ""{bdOrigen}"".OCRD IO 
                                LEFT JOIN {bdDestino}.OCRD D ON D.""CardCode"" = IO.""CardCode""
                                WHERE IO.""U_ENVIOSUC""= 'Y'                              
                                AND IO.""CardType"" IN ('C') 
                                AND D.""CardCode"" IS NULL";//AND IO.""U_Enviado"" = 0  
                    OdbcCommand CmD = new OdbcCommand(query, conn); // llama al Query Encabezado con la conexion
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.RecordsAffected != 0)
                        {
                            IniFile IniFil;
                            IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                            string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                            while (dr.Read())
                            {
                                nomcod = dr["CardName"].ToString();
                                cardCode = dr["CardCode"].ToString();

                                string queryEn = $"select count(\"CardName\") \"conteo\"  FROM  \"{bdDestino}\".\"OCRD\" where \"CardName\" = '" + nomcod + "'";

                                string queryContact = $@"SELECT
                                                        ""Name""
                                                        ,""FirstName""
                                                        ,""MiddleName""
                                                        ,""LastName""
                                                        ,""Title""
                                                        ,""Position""
                                                        ,""Address""
                                                        ,""Tel1"" AS ""Phone1""
                                                        ,""Tel2"" AS ""Phone2""
                                                        ,""Cellolar"" AS ""MobilePhone""
                                                        ,""E_MailL"" AS ""E_Mail""
                                                        ,""Notes1"" AS ""Remarks1""
                                                        ,""Notes2"" AS ""Remarks2""
                                                        ,""BirthDate"" AS ""DateOfBirth"" 
                                                        FROM {bdOrigen}.OCPR WHERE ""CardCode"" = ? ";

                                string queryDireccion = $@"SELECT
                                                            ""Address"" AS ""AddressName""
                                                            ,""Address2"" AS ""AddressName2""
                                                            ,""Address3"" AS ""AddressName3""
                                                            ,""Street""
                                                            ,""City""
                                                            ,""County""
                                                            ,""Country""
                                                            ,""State""
                                                            ,""StreetNo""
                                                            ,""Building"" AS ""BuildingFloorRoom""
                                                            ,""GlblLocNum"" AS ""GlobalLocationNumber""
                                                            ,""AdresType"" AS ""AddressType""
                                                              FROM {bdOrigen}.CRD1 WHERE ""CardCode"" = ? ";

                                OdbcCommand CmD1 = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                                conn2.Open();
                                using (OdbcDataReader dr2 = CmD1.ExecuteReader())
                                    while (dr2.Read())
                                    {
                                        cuantos = Convert.ToInt32(dr2["conteo"]);

                                        if (cuantos == 0)
                                        {
                                            JObject JsonObject = new JObject();

                                            JsonObject = CrearObjetoJson(dr);

                                            #region Contactos

                                            JArray JArrayContacto = new JArray();
                                            OdbcCommand cmcContac = new OdbcCommand(queryContact, conn);
                                            cmcContac.Parameters.Add(new OdbcParameter("@code", cardCode));

                                            using (OdbcDataReader dtrcontact = cmcContac.ExecuteReader())
                                            {
                                                JArrayContacto = CrearObjetoJson2(dtrcontact);
                                            }

                                            #endregion

                                            #region Direcciones

                                            JArray JArrayDirecciones = new JArray();
                                            OdbcCommand cmcDirec = new OdbcCommand(queryDireccion, conn);
                                            cmcDirec.Parameters.Add(new OdbcParameter("@code", cardCode));

                                            using (OdbcDataReader dtrdireccion = cmcDirec.ExecuteReader())
                                            {
                                                JArrayDirecciones = CrearObjetoJson2(dtrdireccion);
                                            }

                                            foreach (var item in JArrayDirecciones)
                                            {
                                                var obj = (JObject)item;

                                                if (obj["AddressType"].ToString() == "B")
                                                {
                                                    obj["AddressType"] = "bo_BillTo";
                                                }
                                                else
                                                {
                                                    obj["AddressType"] = "bo_ShipTo";
                                                }

                                            }

                                            #endregion

                                            JsonObject.Add(new JProperty("BPAddresses", JArrayDirecciones));
                                            JsonObject.Add(new JProperty("ContactEmployees", JArrayContacto));

                                            var myjs = JsonObject.ToString();

                                            resul = EnviaSapPorBasicAuth("POST", JsonObject, UrlServiceLayer + "BusinessPartners", TokenDestino);

                                            if (resul == 0) // Caso que inserto 
                                            {

                                                objRegistraLog.Graba("Cliente Creado : " + cardCode + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio

                                                //string queryD = $"UPDATE  \"{bdDestino}\".\"OCRD\" SET \"U_Enviado\" = '1' WHERE \"CardCode\" =  '" + cardCode + "'";
                                                //OdbcCommand cmandd1 = new OdbcCommand(queryD, conn);
                                                //OdbcDataReader drR1 = cmandd1.ExecuteReader();

                                                //query = $"UPDATE  \"{bdOrigen}\".\"OCRD\" SET \"U_Enviado\" = '1' WHERE \"CardCode\" =  '" + cardCode + "'";
                                                //OdbcCommand cmand = new OdbcCommand(query, conn);
                                                //OdbcDataReader dr1 = cmand.ExecuteReader();


                                                /*
                                                if (bdDestino == Conexion.BD_desampa)
                                                {
                                                    string queryO = $"UPDATE  \"{bdOrigen}\".\"OCRD\" SET \"U_Enviado\" = '1' WHERE \"CardCode\" =  '" + cardCode + "'";
                                                    OdbcCommand cmand = new OdbcCommand(queryO, conn);
                                                    OdbcDataReader dr1 = cmand.ExecuteReader();

                                                }

                                                if (bdOrigen == Conexion.BD_desampa)
                                                {

                                                    string queryDD = $"UPDATE  \"{bdDestino}\".\"OCRD\" SET \"U_Enviado\" = '1' WHERE \"CardCode\" =  '" + cardCode + "'";
                                                    OdbcCommand ccmandd = new OdbcCommand(queryDD, conn);
                                                    OdbcDataReader drR01 = ccmandd.ExecuteReader();

                                                    if (bdDestino == Conexion.BD_central)
                                                    {
                                                        query = $"UPDATE  \"{bdOrigen}\".\"OCRD\" SET \"U_Enviado\" = '1' WHERE \"CardCode\" =  '" + cardCode + "'";
                                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                                        OdbcDataReader dr1 = cmand.ExecuteReader();
                                                    }

                                                }
                                                */
                                            }
                                            else
                                            {
                                                objRegistraLog.Graba($"Error CrearSOCIOTODAS en la base destinno {bdDestino}, origen{bdOrigen} : " + RR + " " + cardCode + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 

                                                Inserterror(bdDestino, "OCRD", nomcod);
                                            }
                                        }
                                    }

                                conn2.Close();
                            }

                        }
                        LiberaMemoria.FlushMemory();

                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al intentar Insertar Cliente : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
            return R;
        }

        public static Respuesta ActualizaSOCIOTODAS(string bdOrigen, string bdDestino, string TokenDestino)
        {
            int resul = 0;
            string nomcod = string.Empty;
            string codigo = string.Empty;
            string bd2 = string.Empty;
            string cardty;
            string cacode;
            Respuesta R = new Respuesta();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                using (OdbcConnection conn2 = new OdbcConnection(Conexion.strCon))
                using (OdbcConnection conn3 = new OdbcConnection(Conexion.strCon))
                {
                    // string query = $"CALL \"{bdOrigen}\".IC_UPD_SN('{bdDestino}')"; //Query Encabezado

                    string query = $@"SELECT 
                                1 as ""Series""
                                ,IO.""CardCode""
                                ,IO.""CardName""
                                ,IO.""CardType"" AS ""CardType"" 
                                ,""{bdOrigen}"".""ICgrupoSN""('{bdDestino}',(select ""GroupCode"" from ""{bdOrigen}"".OCRG where ""GroupCode""=IO.""GroupCode""))  ""GroupCode""
                                ,IO.""Phone1"" as ""Phone1""
                                ,""{bdOrigen}"".""ICcondicionesSN""('{bdDestino}',(select ""GroupNum"" from ""{bdOrigen}"".OCTG where ""GroupNum""=IO.""GroupNum"")) ""PayTermsGrpCode""
                                ,IO.""Phone2"" as ""Phone2""
                                ,IO.""Cellular"" as ""Cellular""
                                ,IO.""CreditLine"" as ""CreditLimit""
                                ,IO.""LicTradNum"" AS ""FederalTaxID""
                                ,IO.""E_Mail"" AS ""EmailAddress""
                                ,IO.""AddID"" as ""AdditionalID""
                                ,IO.""Currency"" AS ""Currency""
                                ,IO.""validFor"" AS ""Valid""
                                ,IO.""frozenFor"" as ""Frozen""
                                ,IO.""ListNum"" as ""PriceListNum""
                                ,IO.""DebtLine"" as ""MaxCommitment""
                                ,IO.""U_Tipo_Doc_Elect""
                                ,IO.""U_tipo_identificacion""
                                ,IO.""U_tipo_documento""
                                ,IO.""U_aplica_exoneracion""
                                ,IO.""U_numero_documento""
                                ,IO.""U_nombre_institucion""
                                ,IO.""U_porcentaje_exoneracion""
                                ,IO.""U_fecha_emision""
                                ,IO.""U_fecha_vencimiento_Exonera""
                                ,IO.""U_EXO""
                                ,IO.""U_ActividadReceptor""
                                ,IO.""U_SAFE_NumId""
                                ,IO.""U_TipoDocExoAutorizacionOTRO""
                                ,IO.""U_Inciso""
                                ,IO.""U_Articulo""
                                ,IO.""U_NombreInstitucionOtros""
                                ,IO.""U_Cadenas""
                                ,IO.""U_NumeroVendedor""
                                ,IO.""U_GLN""
                                ,IO.""U_ConsumirdorFinal""
                                ,IO.""U_CondicionVenta""
                                ,IO.""U_CondicionVentaOTRO""
                                ,IO.""DpmIntAct"" as ""DownPaymentClearAct""
                                ,IO.""Free_Text"" as ""FreeText""
                                ,IO.""QryGroup1"" as ""Properties1""
                                ,IO.""QryGroup2"" as ""Properties2""
                                ,IO.""QryGroup3"" as ""Properties3""
                                ,IO.""QryGroup4"" as ""Properties4""
                                ,IO.""QryGroup5"" as ""Properties5""
                                ,IO.""QryGroup6"" as ""Properties6""
                                ,IO.""QryGroup7"" as ""Properties7""
                                ,IO.""QryGroup8"" as ""Properties8""
                                ,IO.""QryGroup9"" as ""Properties9""
                                ,IO.""QryGroup10"" as ""Properties10""
                                ,IO.""QryGroup11"" as ""Properties11""
                                ,IO.""QryGroup12"" as ""Properties12""
                                ,IO.""QryGroup13"" as ""Properties13""
                                ,IO.""QryGroup14"" as ""Properties14""
                                ,IO.""QryGroup15"" as ""Properties15""
                                ,IO.""QryGroup16"" as ""Properties16""
                                ,IO.""QryGroup17"" as ""Properties17""
                                ,IO.""QryGroup18"" as ""Properties18""
                                ,IO.""QryGroup19"" as ""Properties19""
                                ,IO.""QryGroup20"" as ""Properties20""
                                ,IO.""QryGroup21"" as ""Properties21""
                                ,IO.""QryGroup22"" as ""Properties22""
                                ,IO.""QryGroup23"" as ""Properties23""
                                ,IO.""QryGroup24"" as ""Properties24""
                                ,IO.""QryGroup25"" as ""Properties25""
                                ,IO.""QryGroup26"" as ""Properties26""
                                ,IO.""QryGroup27"" as ""Properties27""
                                ,IO.""QryGroup28"" as ""Properties28""
                                ,IO.""QryGroup29"" as ""Properties29""
                                ,IO.""QryGroup30"" as ""Properties30""
                                ,IO.""QryGroup31"" as ""Properties31""
                                ,IO.""QryGroup32"" as ""Properties32""
                                ,IO.""QryGroup33"" as ""Properties33""
                                ,IO.""QryGroup34"" as ""Properties34""
                                ,IO.""QryGroup35"" as ""Properties35""
                                ,IO.""QryGroup36"" as ""Properties36""
                                ,IO.""QryGroup37"" as ""Properties37""
                                ,IO.""QryGroup38"" as ""Properties38""
                                ,IO.""QryGroup39"" as ""Properties39""
                                ,IO.""QryGroup40"" as ""Properties40""
                                ,IO.""QryGroup41"" AS ""Properties41""
                                ,IO.""QryGroup42"" AS ""Properties42""
                                ,IO.""QryGroup43"" AS ""Properties43""
                                ,IO.""QryGroup44"" AS ""Properties44""
                                ,IO.""QryGroup45"" AS ""Properties45""
                                ,IO.""QryGroup46"" AS ""Properties46""
                                ,IO.""QryGroup47"" AS ""Properties47""
                                ,IO.""QryGroup48"" AS ""Properties48""
                                ,IO.""QryGroup49"" AS ""Properties49""
                                ,IO.""QryGroup50"" AS ""Properties50""
                                ,IO.""QryGroup51"" AS ""Properties51""
                                ,IO.""QryGroup52"" AS ""Properties52""
                                ,IO.""QryGroup53"" AS ""Properties53""
                                ,IO.""QryGroup54"" AS ""Properties54""
                                ,IO.""QryGroup55"" AS ""Properties55""
                                ,IO.""QryGroup56"" AS ""Properties56""
                                ,IO.""QryGroup57"" AS ""Properties57""
                                ,IO.""QryGroup58"" AS ""Properties58""
                                ,IO.""QryGroup59"" AS ""Properties59""
                                ,IO.""QryGroup60"" AS ""Properties60""
                                ,IO.""QryGroup61"" AS ""Properties61""
                                ,IO.""QryGroup62"" AS ""Properties62""
                                ,IO.""QryGroup63"" AS ""Properties63""
                                ,IO.""QryGroup64"" AS ""Properties64""
                                FROM ""{bdOrigen}"".OCRD IO 
                                JOIN ""{bdDestino}"".OCRD D ON D.""CardCode"" = IO.""CardCode""
                                WHERE IO.""U_ENVIOSUC""= 'Y'                              
                                AND IO.""CardType"" IN ('C') 
                                AND IO.""U_Enviado"" = '0'";

                    string queryContact = $@"SELECT    
                                                        ""CardCode""
                                                        ,""CntctCode"" AS ""InternalCode""
                                                        ,""Name""
                                                        ,""FirstName""
                                                        ,""MiddleName""
                                                        ,""LastName""
                                                        ,""Title""
                                                        ,""Position""
                                                        ,""Address""
                                                        ,""Tel1"" AS ""Phone1""
                                                        ,""Tel2"" AS ""Phone2""
                                                        ,""Cellolar"" AS ""MobilePhone""
                                                        ,""E_MailL"" AS ""E_Mail""
                                                        ,""Notes1"" AS ""Remarks1""
                                                        ,""Notes2"" AS ""Remarks2""
                                                        ,""BirthDate"" AS ""DateOfBirth"" 
                                                        FROM {bdOrigen}.OCPR WHERE ""CardCode"" = ? ";

                    string queryDireccion = $@"SELECT
                                                            ""Address"" AS ""AddressName""
                                                            ,""Address2"" AS ""AddressName2""
                                                            ,""Address3"" AS ""AddressName3""
                                                            ,""Street""
                                                            ,""City""
                                                            ,""County""
                                                            ,""Country""
                                                            ,""State""
                                                            ,""StreetNo""
                                                            ,""Building"" AS ""BuildingFloorRoom""
                                                            ,""GlblLocNum"" AS ""GlobalLocationNumber""
                                                            ,""AdresType"" AS ""AddressType""
                                                              FROM {bdOrigen}.CRD1 WHERE ""CardCode"" = ? ";

                    OdbcCommand CmD = new OdbcCommand(query, conn); // llama al Query Encabezado con la conexion
                    conn.Open();
                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.RecordsAffected != 0)
                        {

                            IniFile IniFil;
                            IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                            string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                            while (dr.Read())
                            {
                                nomcod = dr["CardName"].ToString();
                                cardty = dr["CardType"].ToString();
                                cacode = dr["CardCode"].ToString();
                                string baba = bdOrigen.Substring(0, 10);

                                JObject JsonObject = new JObject();
                                JsonObject = CrearObjetoJson(dr);

                                #region Contactos

                                JArray JArrayContacto = new JArray();
                                OdbcCommand cmcContac = new OdbcCommand(queryContact, conn);
                                cmcContac.Parameters.Add(new OdbcParameter("@code", cacode));

                                using (OdbcDataReader dtrcontact = cmcContac.ExecuteReader())
                                {
                                    JArrayContacto = CrearObjetoJson2(dtrcontact);

                                    foreach (var item in JArrayContacto)
                                    {
                                        var obj = (JObject)item;

                                        obj["CardCode"] = cacode;

                                    }
                                }

                                #endregion

                                #region Direcciones

                                JArray JArrayDirecciones = new JArray();
                                OdbcCommand cmcDirec = new OdbcCommand(queryDireccion, conn);
                                cmcDirec.Parameters.Add(new OdbcParameter("@code", cacode));

                                using (OdbcDataReader dtrdireccion = cmcDirec.ExecuteReader())
                                {
                                    JArrayDirecciones = CrearObjetoJson2(dtrdireccion);
                                }

                                foreach (var item in JArrayDirecciones)
                                {
                                    var obj = (JObject)item;

                                    if (obj["AddressType"].ToString() == "B")
                                    {
                                        obj["AddressType"] = "bo_BillTo";
                                    }
                                    else
                                    {
                                        obj["AddressType"] = "bo_ShipTo";
                                    }

                                }

                                #endregion

                                JsonObject.Add(new JProperty("BPAddresses", JArrayDirecciones));
                                JsonObject.Add(new JProperty("ContactEmployees", JArrayContacto));


                                JsonObject.Remove("CardCode");
                                var myjs = JsonObject.ToString();
                                resul = EnviaSapPorBasicAuth("PATCH", JsonObject, $"{UrlServiceLayer}BusinessPartners('" + cacode + "')", TokenDestino);//
                                if (resul == 0) // Caso que inserto 
                                {
                                    objRegistraLog.Graba($"Cliente actualizado origen {bdOrigen},  Destino {bdDestino} : " + cacode + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar si fue satisfactorio

                                    //Query evita cilcos por postransaction, pone el campo U_Enviado en 1 (no llamarlo) ya que el postransaction lo pone 0 al actualizarlo (para volver a llamar al cliente)  
                                    string querydes = $"UPDATE  \"{bdDestino}\".\"OCRD\" SET \"U_Enviado\" = '1' WHERE \"CardCode\" =  '" + cacode + "'";
                                    OdbcCommand cmand1 = new OdbcCommand(querydes, conn);
                                    OdbcDataReader dR1 = cmand1.ExecuteReader();

                                    if (bdDestino == Conexion.BD_desampa) //Si no soy la la última sucursal que recorre el Main
                                    {
                                        //actualizo la última sucursal que tiene le emprea Origen
                                        string queryD = $"UPDATE  \"{bdOrigen}\".\"OCRD\" SET \"U_Enviado\" = '1' WHERE \"CardCode\" =  '" + cacode + "'";
                                        OdbcCommand cmandd1 = new OdbcCommand(queryD, conn);
                                        OdbcDataReader drR1 = cmandd1.ExecuteReader();
                                    }

                                    //Si soy la la última sucursal que recorre el Main Y Soy la última base del recorrido
                                    if (bdOrigen == Conexion.BD_desampa && bdDestino == Conexion.BD_central)
                                    {
                                        //actualizo la última sucursal que tiene le emprea Origen
                                        string queryD = $"UPDATE  \"{bdOrigen}\".\"OCRD\" SET \"U_Enviado\" = '1' WHERE \"CardCode\" =  '" + cacode + "'";
                                        OdbcCommand cmandd1 = new OdbcCommand(queryD, conn);
                                        OdbcDataReader drR1 = cmandd1.ExecuteReader();

                                    }
                                }
                                else
                                {
                                    objRegistraLog.Graba($"Error al actualizar método SOCIOTODAS en la bd Destino {bdDestino}, origen {bdOrigen}: " + RR + " " + cacode + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));


                                    Inserterror(bdDestino, "OCRD", cacode);
                                }

                                conn3.Close();
                            }

                        }

                    }
                    LiberaMemoria.FlushMemory();
                    conn.Close();
                }

            }
            catch (Exception ex)
            {
                R.Codigo = -1;
                R.Mensaje = "Alerta: " + ex.Message;
                objRegistraLog.Graba("Error al intentar Actualizar cliente : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }
            return R;
        }

        public static int EnviaSapPorBasicAuth(string Metodo, JObject JsonObject, string URL, string toctoc, string Usar = "Y")
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = Metodo;
                if (Metodo == "PATCH" && Usar == "Y")
                {
                    request.Headers.Add("B1S-ReplaceCollectionsOnPatch", "true");
                }
                request.ContentType = "application/json";
                request.ServicePoint.Expect100Continue = false;
                ServicePointManager.ServerCertificateValidationCallback += RemoteSSLTLSCertificateValidate;
                //request.CookieContainer = new CookieContainer();

                request.Headers.Add("Authorization", toctoc);

                //foreach (Cookie cookie in cookieData)
                //{
                //    request.CookieContainer.Add(new Uri(URL), new Cookie(cookie.Name, cookie.Value));
                //}

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
                        ObjSap = JsonConvert.DeserializeObject<Marketing>(msn);
                        return 0;
                    }
                    else
                    {
                        var ObError = JsonConvert.DeserializeObject<Root>(msn);
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

        private static int EnviaSapPorSesion(string Metodo, JObject JsonObject, string URL)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = Metodo;
                if (Metodo == "PATCH")
                {
                    request.Headers.Add("B1S-ReplaceCollectionsOnPatch", "true");
                }
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
                JObject JsonObject = new JObject();
                JsonObject.Add(new JProperty("CompanyDB", CompanyDB));
                JsonObject.Add(new JProperty("UserName", UserName));
                JsonObject.Add(new JProperty("Password", Password));

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

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
                throw new Exception($"Error en el método CrearObjetoJson, {ex.Message}");
            }

            return dataObj;
        }

        private static JArray CrearObjetoJson2(OdbcDataReader dataReader)
        {
            JArray arregloJson = new JArray();
            try
            {
                while (dataReader.Read())
                {
                    JObject L = new JObject();

                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        string columnName = dataReader.GetName(i);
                        object columnValue = dataReader.GetValue(i);

                        // Agregar la propiedad al objeto JSON

                        L.Add(new JProperty(columnName, columnValue));
                    }
                    arregloJson.Add(L);
                    L = null;
                }

                string json = arregloJson.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en el método CrearObjetoJson2, {ex.Message}");
            }

            return arregloJson;
        }

        private static void Inserterror(string bdDestino, string Tabla, string nomcod)
        {

            try
            {

                string empresa = bdDestino
                                   .Replace("10071_", "")
                                   .Replace("_TEST", "");

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
                          "values(" + numero + "," + numero + ",'" + nomcod + "','" + Tabla + "','" + empresa + "','" + RR.Replace("'", "") + "','" + DateTime.Now.ToString("yyyy/MM/dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";  //Query Actualiza el campo Status y DocEntry_SAP, Tabla Encabezado
                    OdbcCommand cmc1 = new OdbcCommand(queryAct, conn9);
                    cmc1.ExecuteNonQuery();
                    conn9.Close();
                }

                LiberaMemoria.FlushMemory();
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba("Error método Inserterror : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
            }


        }

        /// <summary>
        /// Consulta por el nombre de la bd
        /// </summary>
        /// <param name="BDconsulta"></param>
        /// <returns></returns>
        private static string ConsultarBD(string BDconsulta)
        {
            string sNameBd = string.Empty;
            IniFile IniFil;
            IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");

            switch (BDconsulta)
            {
                case "ORIGEN":
                    sNameBd = IniFil.LeerINI("ConexionHanaProd", "ORIGEN") ?? throw new ArgumentNullException("*ORIGEN (ConsultarBD) no configurado en el archivo config*");
                    break;

                case "sur":
                    sNameBd = IniFil.LeerINI("ConexionHanaProd", "sur") ?? throw new ArgumentNullException("*sur (ConsultarBD) no configurado en el archivo config*");
                    break;

                case "omdo":
                    sNameBd = IniFil.LeerINI("ConexionHanaProd", "omdo") ?? throw new ArgumentNullException("*omdo (ConsultarBD) no configurado en el archivo config*");
                    break;

                case "nedn":
                    sNameBd = IniFil.LeerINI("ConexionHanaProd", "nedn") ?? throw new ArgumentNullException("*nedn (ConsultarBD) no configurado en el archivo config*");
                    break;

                case "hogar":
                    sNameBd = IniFil.LeerINI("ConexionHanaProd", "hogar") ?? throw new ArgumentNullException("*hogar (ConsultarBD) no configurado en el archivo config*");
                    break;

                case "flores":
                    sNameBd = IniFil.LeerINI("ConexionHanaProd", "flores") ?? throw new ArgumentNullException("*flores (ConsultarBD) no configurado en el archivo config*");
                    break;

                case "vital":
                    sNameBd = IniFil.LeerINI("ConexionHanaProd", "vital") ?? throw new ArgumentNullException("*vital (ConsultarBD) no configurado en el archivo config*");
                    break;

                case "emde":
                    sNameBd = IniFil.LeerINI("ConexionHanaProd", "emde") ?? throw new ArgumentNullException("*emde (ConsultarBD) no configurado en el archivo config*");
                    break;

                case "central":
                    sNameBd = IniFil.LeerINI("ConexionHanaProd", "central") ?? throw new ArgumentNullException("*central (ConsultarBD) no configurado en el archivo config*");
                    break;

                case "desampa":
                    sNameBd = IniFil.LeerINI("ConexionHanaProd", "desampa") ?? throw new ArgumentNullException("*desampa (ConsultarBD) no configurado en el archivo config*");
                    break;

                default:
                    sNameBd = string.Empty;
                    break;
            }

            return sNameBd;
        }

        /// <summary>
        /// Consulta por el cardCode dependiendo de la BD de origen del la solicitud
        /// </summary>
        /// <param name="BDconsulta"></param>
        /// <returns></returns>
        private static string ConsultarCardCode(string BDconsulta)
        {
            string sNameBd = string.Empty;
            IniFile IniFil;
            IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");

            switch (BDconsulta)
            {

                case string s when s.Contains("SUR"):
                    sNameBd = IniFil.LeerINI("CodSocio", "sur") ?? throw new ArgumentNullException("*sur (ConsultarCardCode) no configurado en el archivo config*");
                    break;

                case string s when s.Contains("OMDO"):
                    sNameBd = IniFil.LeerINI("CodSocio", "omdo") ?? throw new ArgumentNullException("*omdo (ConsultarCardCode) no configurado en el archivo config*");
                    break;

                case string s when s.Contains("NEDN"):
                    sNameBd = IniFil.LeerINI("CodSocio", "nedn") ?? throw new ArgumentNullException("*nedn (ConsultarCardCode) no configurado en el archivo config*");
                    break;

                case string s when s.Contains("HOGAR"):
                    sNameBd = IniFil.LeerINI("CodSocio", "hogar") ?? throw new ArgumentNullException("*hogar (ConsultarCardCode) no configurado en el archivo config*");
                    break;

                case string s when s.Contains("FLORES"):
                    sNameBd = IniFil.LeerINI("CodSocio", "flores") ?? throw new ArgumentNullException("*flores (ConsultarCardCode) no configurado en el archivo config*");
                    break;

                case string s when s.Contains("VITAL"):
                    sNameBd = IniFil.LeerINI("CodSocio", "vital") ?? throw new ArgumentNullException("*vital (ConsultarCardCode) no configurado en el archivo config*");
                    break;

                case string s when s.Contains("EMDE"):
                    sNameBd = IniFil.LeerINI("CodSocio", "emde") ?? throw new ArgumentNullException("*emde (ConsultarCardCode) no configurado en el archivo config*");
                    break;

                case string s when s.Contains("CENTRAL"):
                    sNameBd = IniFil.LeerINI("CodSocio", "central") ?? throw new ArgumentNullException("*central (ConsultarCardCode) no configurado en el archivo config*");
                    break;

                case string s when s.Contains("HECHIZO"):
                    sNameBd = IniFil.LeerINI("CodSocio", "ORIGEN") ?? throw new ArgumentNullException("*central (ConsultarCardCode) no configurado en el archivo config*");
                    break;

                case "10071_3_102_956112":
                case "DESAMPA":
                    sNameBd = IniFil.LeerINI("CodSocio", "desampa") ?? throw new ArgumentNullException("*central (ConsultarCardCode) no configurado en el archivo config*");
                    break;

                default:
                    sNameBd = string.Empty;
                    break;
            }

            return sNameBd;
        }

        /// <summary>
        /// Consulta por los docEntry destino de donde se deben relacionar los documentos ya sea para las fatura provee (docentryOrdCompra) o NCprove (docEntryFactProvee)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="Entry"></param>
        /// <param name="dborigen"></param>
        /// <returns></returns>
        private static int ConsultarDocEntry(OdbcConnection conn, int Entry, string dborigen, string tabla)
        {
            int DocEntry = 0;

            try
            {
                string queryVend = $"SELECT \"U_DocEntryDestino\" FROM \"{dborigen}\".\"{tabla}\" WHERE \"DocEntry\" = {Entry} ";
                OdbcCommand Cm = new OdbcCommand(queryVend, conn);
                using (OdbcDataReader dtr = Cm.ExecuteReader())
                {
                    while (dtr.Read())
                    {
                        DocEntry = Convert.ToInt32(dtr["U_DocEntryDestino"]);
                    }
                }

                return DocEntry;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba($"Error al consultar el DocbaseEntry: {Entry}, en la tabla: {tabla} ,en la base: {dborigen}, en el método ConsultarDocEntry: {ex.Message}");
                return 0;
            }

        }

        /// <summary>
        /// Consultar el docentry Origen de los docuemnetos
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="BaseEntry"></param>
        /// <param name="dborigen"></param>
        /// <param name="tabla"></param>
        /// <returns></returns>
        private static int ConsultarDocEntrySolicitudCompra(OdbcConnection conn, int BaseEntry, string dborigen, string tabla)
        {

            int DocEntry = 0;

            try
            {
                string queryVend = $"SELECT \"U_DocEntryOrigen\" FROM \"{dborigen}\".\"{tabla}\" WHERE \"DocEntry\" = {BaseEntry} ";
                OdbcCommand Cm = new OdbcCommand(queryVend, conn);
                using (OdbcDataReader dtr = Cm.ExecuteReader())
                {
                    while (dtr.Read())
                    {
                        DocEntry = Convert.ToInt32(dtr["U_DocEntryOrigen"]);
                    }
                }

                return DocEntry;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba($"Error al consultar el DocbaseEntry: {BaseEntry}, en la tabla: {tabla} ,en la base: {dborigen}, en el método ConsultarDocEntry: {ex.Message}");
                return 0;
            }

        }

        /// <summary>
        /// consulta por el estado del documento en cuestion
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="Entry"></param>
        /// <param name="dbdestino"></param>
        /// <param name="tabla"></param>
        /// <returns></returns>
        private static bool ConsultarStatusDoc(OdbcConnection conn, int Entry, string dbdestino, string tabla)
        {

            string DocStatus = string.Empty;
            string Canceled = string.Empty;
            bool Procesa = false;

            try
            {

                string queryVend = $"SELECT \"DocStatus\", \"CANCELED\" FROM \"{dbdestino}\".\"{tabla}\" WHERE \"DocEntry\" = {Entry} ";
                OdbcCommand Cm = new OdbcCommand(queryVend, conn);
                using (OdbcDataReader dtr = Cm.ExecuteReader())
                {

                    while (dtr.Read())
                    {
                        DocStatus = dtr["DocStatus"].ToString();
                        Canceled = dtr["CANCELED"].ToString();
                    }
                }

                if (DocStatus == "O" && Canceled == "N")
                {
                    Procesa = true;
                }

                return Procesa;

            }
            catch (Exception ex)
            {
                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Error al consultar el DocEntry: {Entry}, en la tabla: {tabla} ,en la base: {dbdestino}, en el método ConsultarDocEntry: {ex.Message}");
                return false;
            }

        }

        /// <summary>
        /// consulta la serie en la tabla de destino
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="serieName"></param>
        /// <param name="ObjectCode"></param>
        /// <param name="dbdestino"></param>
        /// <returns></returns>
        private static int ConsultarSeries(OdbcConnection conn, string serieName, int ObjectCode, string dbdestino, string DocSubType)
        {

            int Serie = 0;

            try
            {

                string queryVend = $"SELECT \"Series\" FROM \"{dbdestino}\".\"NNM1\" WHERE \"SeriesName\" = '{serieName}'  AND \"ObjectCode\" = {ObjectCode} AND \"DocSubType\" = '{DocSubType}' ";
                OdbcCommand Cm = new OdbcCommand(queryVend, conn);
                using (OdbcDataReader dtr = Cm.ExecuteReader())
                {

                    while (dtr.Read())
                    {
                        Serie = Convert.ToInt32(dtr["Series"]);
                    }

                }

                return Serie;

            }
            catch (Exception ex)
            {
                objRegistraLog.Graba($"Error al consultar el SERIES: ObjectCode {ObjectCode} en la base: {dbdestino}: {ex.Message}");
                return 0;
            }

        }

        /// <summary>
        /// consulta la columna por la que se va a consultar con base a la BDDestino
        /// </summary>
        /// <param name="BdDestino"></param>
        /// <returns></returns>
        private static string ConsultarColumna(string BdDestino)
        {
            string ColumName = string.Empty;

            try
            {
                switch (BdDestino)
                {
                    case "hogar":
                        ColumName = "U_Envio_Hogar";
                        break;

                    case "sur":
                        ColumName = "U_Envio_Sur";
                        break;

                    case "omdo":
                        ColumName = "U_Envio_Omdo";
                        break;

                    case "nedn":
                        ColumName = "U_Envio_Nedn";
                        break;

                    case "flores":
                        ColumName = "U_Envio_Flores";
                        break;

                    case "vital":
                        ColumName = "U_Envio_Vital";
                        break;

                    case "emde":
                        ColumName = "U_Envio_Emde";
                        break;

                    case "central":
                        ColumName = "U_Envio_Central";
                        break;

                    case "desampa":
                        ColumName = "U_Envio_Expresso";
                        break;

                    default:
                        ColumName = string.Empty;
                        break;

                }

                return ColumName;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba($"[{DateTime.Now.ToString("hh-mm-ss")}] Error en el método ConsultarColumnaOACT en la base destino {BdDestino}.: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// consulta la columna por la que se va a consultar con base a la BDDestino
        /// </summary>
        /// <param name="BdDestino"></param>
        /// <returns></returns>
        private static string ConsultarColumna2(string BdDestino)
        {
            string ColumName = string.Empty;

            try
            {
                switch (BdDestino)
                {
                    case "hogar":
                        ColumName = "U_Enviado_Hogar";
                        break;

                    case "sur":
                        ColumName = "U_Enviado_Sur";
                        break;

                    case "omdo":
                        ColumName = "U_Enviado_Omdo";
                        break;

                    case "nedn":
                        ColumName = "U_Enviado_Nedn";
                        break;

                    case "flores":
                        ColumName = "U_Enviado_Flores";
                        break;

                    case "vital":
                        ColumName = "U_Enviado_Vital";
                        break;

                    case "emde":
                        ColumName = "U_Enviado_Emde";
                        break;

                    case "central":
                        ColumName = "U_Enviado_Ave";
                        break;

                    case "desampa":
                        ColumName = "U_Enviado_Expresso";
                        break;

                    default:
                        ColumName = string.Empty;
                        break;

                }

                return ColumName;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba($"[{DateTime.Now.ToString("hh-mm-ss")}] Error en el método ConsultarColumnaOACT en la base destino {BdDestino}.: {ex.Message}");
                return string.Empty;
            }
        }

        private static string ConsultarGrpArt(string BdDestino)
        {
            string ColumName = string.Empty;

            try
            {
                switch (BdDestino)
                {
                    case "hogar":
                        ColumName = "COD_HGAR";
                        break;

                    case "sur":
                        ColumName = "COD_SUR";
                        break;

                    case "omdo":
                        ColumName = "COD_OMDO";
                        break;

                    case "nedn":
                        ColumName = "COD_NEDN";
                        break;

                    case "flores":
                        ColumName = "COD_FLRES";
                        break;

                    case "vital":
                        ColumName = "COD_VTAL";
                        break;

                    case "emde":
                        ColumName = "COD_EMDE";
                        break;

                    case "central":
                        ColumName = "COD_CENT";
                        break;

                    case "desampa":
                        ColumName = "COD_DESAMPA";
                        break;

                    default:
                        ColumName = string.Empty;
                        break;

                }

                return ColumName;
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba($"[{DateTime.Now.ToString("hh-mm-ss")}] Error en el método ConsultarColumnaOACT en la base destino {BdDestino}.: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Mapea el valor que se debe enviar a SAP con base al valor que devuelve la BD
        /// </summary>
        /// <param name="actType"></param>
        /// <returns></returns>
        private static string MapActType(string actType)
        {
            switch (actType)
            {
                case "N": return "at_Other";
                case "E": return "at_Expenses";
                case "I": return "at_Revenues";
                default:
                    throw new Exception("ActType no válido: " + actType);
            }
        }

        /// <summary>
        /// valores para el proceso de cuentas cuentas
        /// </summary>
        public enum ProcesoCuenta
        {
            Crear,
            Actualizar
        }

        #region Pendiente

        /// <summary>
        /// Replica  en las NC de Provee de las sucursales las actualizaciones que se hicieron desde la Facturas de venta de Hechizo
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <param name="tokenDestino"></param>
        public static void ActualizarEnNotaCredProvee(string bdDestino, string tokenDestino)
        {
            string BdHechizo = string.Empty;
            string BdDest = string.Empty;
            string QueryOrdComp = string.Empty;
            string QueryDetComp = string.Empty;
            int DocEntryProcesar = 0;
            bool StatusDoc = false;
            int resul = 0;

            try
            {
                IniFile IniFil;
                IniFil = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini");
                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                BdHechizo = ConsultarBD("ORIGEN");
                BdDest = ConsultarBD(bdDestino);
                string CardCode = ConsultarCardCode(bdDestino.ToUpper());

                QueryOrdComp = $@"SELECT 
                                ""DocEntry"" 
                                ,""DocNum"" 
                                ,""Comments""
                                ,""DocDueDate""
                                ,""TrnspCode"" AS ""TransportationCode""
                                ,""NumAtCard""
                                ,""PayToCode""
                                ,""Address""
                                ,""U_DocEntryDestino""
                                FROM {BdHechizo}.ORIN
                                EF WHERE ""CANCELED"" = 'N'  AND ""DocStatus"" = 'O' AND ""U_Actualiza"" = 'Y' AND ""CardCode"" = '{CardCode}' "; //""DiscPrcnt"" AS ""DiscountPercent"" NO palicar para actualizar


                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    OdbcCommand CmD = new OdbcCommand(QueryOrdComp, conn);

                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {

                        if (dr.RecordsAffected != 0)
                        {
                            while (dr.Read())
                            {
                                try
                                {

                                    JObject JsonObject = new JObject();

                                    int En_Entry = Convert.ToInt32(dr["DocEntry"]);

                                    int DocNum = Convert.ToInt32(dr["DocNum"]);

                                    int EntryActualizar = dr.IsDBNull(dr.GetOrdinal("U_DocEntryDestino")) ? 0 : dr.GetInt32(dr.GetOrdinal("U_DocEntryDestino"));

                                    StatusDoc = ConsultarStatusDoc(conn, EntryActualizar, BdDest, "ORPC");

                                    if (EntryActualizar != 0 && StatusDoc == true)
                                    {

                                        JsonObject = CrearObjetoJson(dr);

                                        JsonObject.Remove("DocEntry");
                                        JsonObject.Remove("DocNum");
                                        JsonObject.Remove("U_DocEntryDestino");

                                        var myjs = JsonObject.ToString();
                                        resul = EnviaSapPorBasicAuth("PATCH", JsonObject, UrlServiceLayer + $"PurchaseCreditNotes({EntryActualizar})", tokenDestino, "N");

                                        if (resul == 0) // Caso que inserto
                                        {
                                            string query = $"UPDATE  \"{BdHechizo}\".\"ORIN\" SET \"U_Actualiza\" = 'N', \"U_ComentarioError\" = NULL  WHERE \"DocEntry\" = {En_Entry} ";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();
                                        }
                                        else
                                        {
                                            string query = $"UPDATE  \"{BdHechizo}\".\"ORIN\" SET \"U_ComentarioError\" = 'Sin Actualizar: {RR.Replace("'", "")}', \"U_Actualiza\" = 'N' WHERE \"DocEntry\" = {En_Entry}";
                                            OdbcCommand cmand = new OdbcCommand(query, conn);
                                            OdbcDataReader dr1 = cmand.ExecuteReader();

                                            objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Error actualizando la NC de proveedor en {BdDest} con el docNum NC deudor {DocNum}: " + RR.Replace("'", ""));
                                        }
                                    }
                                    else
                                    {

                                        string query = $"UPDATE  \"{BdHechizo}\".\"ORIN\" SET \"U_ComentarioError\" = 'No fue posible Actualizar la NC {DocNum} en {BdDest} ya sea porque no se encontró el docentry relacionado: {DocEntryProcesar} o el documento relacionado esta cerrado o cancelado', \"U_Actualiza\" = 'N' WHERE \"DocEntry\" = {En_Entry}";
                                        OdbcCommand cmand = new OdbcCommand(query, conn);
                                        OdbcDataReader dr1 = cmand.ExecuteReader();

                                        objRegistraLog.Graba($"[{DateTime.Now.ToString("HH:mm:ss tt")}] Sin Actualizar: No fue posible Actualizar la Nota DE Credito {DocNum} en  {BdDest} ya sea porque no se encontró el docentry relacionado: {DocEntryProcesar}  o el documento relacionado esta cerrado o cancelado");
                                    }


                                }
                                catch (Exception ex)
                                {

                                    objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error al ActualizarEnNCProveeo hacia BdDestino {BdDest}, {ex.Message}");
                                }

                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                objRegistraLog.Graba($"[{DateTime.Now.ToString("HH: mm:ss tt")}]Error  en ActualizarEnNCProveeo hacia BdDestino {BdDest}, {ex.Message}");
            }
        }

        /// <summary>
        /// Procesa las cuentas de Hechizo hacia las demás sucursales
        /// </summary>
        /// <param name="bdDestino"></param>
        /// <param name="tokenDestino"></param>
        /// <param name="proceso"></param>
        public static void ProcesarCuentasdesdeHechizoV2(string bdDestino, string tokenDestino, ProcesoCuenta proceso)
        {
            string BdHechizo = string.Empty;
            string BdDest = string.Empty;
            string QueryCuentas = string.Empty;
            string AccCode = string.Empty;

            int resul = 0;

            try
            {
                // ===============================
                // Flags de proceso (una sola vez)
                // ===============================
                bool esCrear = proceso == ProcesoCuenta.Crear;
                bool esActualizar = proceso == ProcesoCuenta.Actualizar;

                // ===============================
                // Configuración inicial
                // ===============================
                IniFile IniFil = new IniFile(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/config.ini"
                );

                string UrlServiceLayer = IniFil.LeerINI("ServiceLayer", "UrlSL");

                BdHechizo = ConsultarBD("ORIGEN");
                BdDest = ConsultarBD(bdDestino);
                string ColumnaConsultaDestino = ConsultarColumna(bdDestino);

                // ===============================
                // Query dinámico según proceso
                // ===============================
                QueryCuentas = esCrear
                                ? $@"SELECT 
                                IO.""AcctCode"" AS ""Code"",
                                IO.""AcctName"" AS ""Name"",
                                IO.""Finanse"" AS ""CashAccount"",
                                IO.""Postable"" AS ""ActiveAccount"",
                                IO.""FatherNum"" AS ""FatherAccountKey"",
                                IO.""ActType"" AS ""AccountType"",
                                IO.""ActCurr"" AS ""AcctCurrency"",
                                IO.""Protected"",
                                IO.""FrgnName"" AS ""ForeignName"",
                                IO.""LocManTran"" AS ""LockManualTransaction"",
                                IO.""FormatCode"",
                                IO.""PlngLevel"" AS ""PlanningLevel"",
                                IO.""ValidFor"",
                                IO.""FrozenFor"",
                                IO.""BlocManPos"" AS ""BlockManualPosting"",
                                IO.""Details"",
                                IO.""AccntntCod"" AS ""ExternalCode""
                            FROM ""{BdHechizo}"".OACT IO
                            LEFT JOIN ""{BdDest}"".OACT DT ON IO.""AcctCode"" = DT.""AcctCode""
                            WHERE IO.""U_Replicar"" = 'Y'
                              AND DT.""AcctCode"" IS NULL"
                                : $@"SELECT 
                                IO.""AcctCode"" AS ""Code"",
                                IO.""AcctName"" AS ""Name"",
                                IO.""Finanse"" AS ""CashAccount"",
                                IO.""Postable"" AS ""ActiveAccount"",
                                IO.""FatherNum"" AS ""FatherAccountKey"",
                                IO.""ActType"" AS ""AccountType"",
                                IO.""ActCurr"" AS ""AcctCurrency"",
                                IO.""Protected"",
                                IO.""FrgnName"" AS ""ForeignName"",
                                IO.""LocManTran"" AS ""LockManualTransaction"",
                                IO.""FormatCode"",
                                IO.""PlngLevel"" AS ""PlanningLevel"",
                                IO.""ValidFor"",
                                IO.""FrozenFor"",
                                IO.""BlocManPos"" AS ""BlockManualPosting"",
                                IO.""Details"",
                                IO.""AccntntCod"" AS ""ExternalCode""
                            FROM ""{BdHechizo}"".OACT IO
                            WHERE IO.""U_Replicar"" = 'Y'
                              AND IO.""{ColumnaConsultaDestino}"" = 'Y'";

                // ===============================
                // Ejecución
                // ===============================
                using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                {
                    OdbcCommand CmD = new OdbcCommand(QueryCuentas, conn);
                    conn.Open();

                    using (OdbcDataReader dr = CmD.ExecuteReader())
                    {
                        if (dr.RecordsAffected != 0)
                        {
                            while (dr.Read())
                            {
                                try
                                {
                                    AccCode = dr["Code"].ToString();

                                    // ===============================
                                    // Construcción JSON
                                    // ===============================
                                    JObject JsonObject = CrearObjetoJson(dr);

                                    // Map ActType → enum Service Layer
                                    if (JsonObject["AccountType"] != null)
                                        JsonObject["AccountType"] = MapActType(JsonObject["AccountType"].ToString());

                                    // En actualización no se envía Code
                                    if (esActualizar)
                                        JsonObject.Remove("Code");

                                    // ===============================
                                    // Envío a Service Layer
                                    // ===============================
                                    string metodoHttp = esCrear ? "POST" : "PATCH";
                                    string url = esCrear
                                        ? UrlServiceLayer + "ChartOfAccounts"
                                        : UrlServiceLayer + $"ChartOfAccounts('{AccCode}')";

                                    string json = JsonObject.ToString();
                                    resul = EnviaSapPorBasicAuth(metodoHttp, JsonObject, url, tokenDestino, esActualizar ? "N" : null);

                                    // ===============================
                                    // Resultado OK
                                    // ===============================
                                    if (resul == 0)
                                    {
                                        string queryOk = esCrear
                                                            ? $@"
                                                        UPDATE ""{BdHechizo}"".""OACT""
                                                        SET ""U_ComentarioError"" = NULL
                                                        WHERE ""AcctCode"" = '{AccCode}'"
                                                            : $@"
                                                        UPDATE ""{BdHechizo}"".""OACT""
                                                        SET ""{ColumnaConsultaDestino}"" = 'N',
                                                            ""U_ComentarioError"" = NULL
                                                        WHERE ""AcctCode"" = '{AccCode}'";

                                        new OdbcCommand(queryOk, conn).ExecuteReader();
                                    }
                                    // ===============================
                                    // Resultado Error
                                    // ===============================
                                    else
                                    {

                                        Inserterror(BdDest, "OACT", AccCode);

                                        #region Consultar si hay en el campo de error mensaje de error previo

                                        string errorPrevio = string.Empty;

                                        string QuerymsgErr = $@"
                                            SELECT ""U_ComentarioError""
                                            FROM ""{BdHechizo}"".""OACT""
                                            WHERE ""AcctCode"" = '{AccCode}'";

                                        using (OdbcDataReader drr =
                                            new OdbcCommand(QuerymsgErr, conn).ExecuteReader())
                                        {
                                            if (drr.Read())
                                                errorPrevio = drr["U_ComentarioError"].ToString();
                                        }

                                        #endregion

                                        string flagActualizar = esActualizar ? $@", ""{ColumnaConsultaDestino}"" = 'N'" : "";

                                        string mensajeError = $"Error al {proceso} en {BdDest} la cuenta origen {AccCode}: {RR.Replace("'", "")}" + (string.IsNullOrEmpty(errorPrevio) ? "" : ". " + errorPrevio);

                                        string queryErr = $@"UPDATE ""{BdHechizo}"".""OACT""
                                                            SET ""U_ComentarioError"" = '{mensajeError}'
                                                                {flagActualizar}
                                                            WHERE ""AcctCode"" = '{AccCode}'";

                                        new OdbcCommand(queryErr, conn).ExecuteReader();

                                        objRegistraLog.Graba($"[{DateTime.Now:HH:mm:ss tt}] Error al {proceso} la cuenta {AccCode} en {BdDest}: {RR}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    objRegistraLog.Graba(
                                        $"[{DateTime.Now:HH:mm:ss tt}] Error al {proceso} la cuenta {AccCode} en {BdDest}: {ex.Message}"
                                    );
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba(
                    $"[{DateTime.Now:HH:mm:ss tt}] Error en ProcesarCuentas al {proceso} la cuenta {AccCode} en {BdDest} desde Hechizo: {ex.Message}"
                );
            }
        }

        private static void ComparePricesAndLog(int docEntryDestino, int docEntryOrigen, JArray sentLines,  List<SapDocumentLine> responseLines, string bdOrigen, string bdDestino)
        {
            const int PRICE_DECIMALS = 6;

            try
            {
                if (responseLines == null || !responseLines.Any())
                {
                    objRegistraLog.Graba($"[{DateTime.Now:HH:mm:ss tt}] [ADVERTENCIA] DocEntryDestino {docEntryDestino}, BDDest:{bdDestino}: " +
                                         $"SAP no devolvió líneas en la respuesta.  BDOri: {bdOrigen}, DocEntryOrigen {docEntryOrigen}");
                    return;
                }

                // Diccionario de respuesta SAP indexado por LineNum
                var sapByLineNum = responseLines.ToDictionary(l => l.LineNum, l => l);

                var differences = new List<string>();

                foreach (var sentLine in sentLines)
                {
                    string itemCode = sentLine["ItemCode"]?.ToString();
                    decimal sentPrice = sentLine["UnitPrice"]?.ToObject<decimal>() ?? 0;
                    int lineNum = sentLine["LineNum"]?.ToObject<int>() ?? -1;

                    if (lineNum == -1)
                    {
                        objRegistraLog.Graba($"[{DateTime.Now:HH:mm:ss tt}] [ADVERTENCIA] DocEntryDestino {docEntryDestino},  BDDest:{bdDestino}: " +
                                             $"ItemCode {itemCode} no tiene LineNum, se omite de la comparación. BDOri: {bdOrigen},  DocEntryOrigen {docEntryOrigen}");
                        continue;
                    }

                    if (!sapByLineNum.TryGetValue(lineNum, out SapDocumentLine sapLine))
                    {
                        objRegistraLog.Graba($"[{DateTime.Now:HH:mm:ss tt}] [ADVERTENCIA] DocEntry {docEntryDestino},  BDDest:{bdDestino}: " +
                                             $"LineNum {lineNum} ItemCode {itemCode} no se encontró en la respuesta de SAP. BDOri: {bdOrigen}, DocEntryOrigen {docEntryOrigen}");
                        continue;
                    }

                    bool sonDiferentes = Math.Round(sentPrice, PRICE_DECIMALS) != Math.Round(sapLine.UnitPrice, PRICE_DECIMALS);

                    if (sonDiferentes)
                    {
                        differences.Add(
                            $"  LineNum: {lineNum,4} | ItemCode: {itemCode,-20} | " +
                            $"Precio enviado: {sentPrice,14:F6} | Precio SAP: {sapLine.UnitPrice,14:F6}"
                        );
                    }
                }

                if (differences.Any())
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"[{DateTime.Now:HH:mm:ss tt}] [DIFERENCIA DE PRECIOS] " +
                                  $"BDOrigen: {bdOrigen}, DocEntry SAP Origen: {docEntryOrigen} | BDDestino: {bdDestino}, DocEntry SAP Destino: {docEntryDestino}");

                    foreach (var diff in differences)
                        sb.AppendLine(diff);

                    sb.AppendLine(new string('-', 90));
                    objRegistraLog.Graba(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba($"[{DateTime.Now:HH:mm:ss tt}] [ERROR] ComparePricesAndLog DocEntryOrigen {docEntryOrigen}, BdOrig {bdOrigen}  | DocEntryDestino {docEntryDestino}, BdDest {bdDestino}: {ex.Message}");
            }
        }

        private static void ComparePricesAndLogActualizacion( int entryActualizar, int enEntryOrig,  JArray sentLines,  OdbcConnection conn, string bdOrigen,  string bdDestino)
        {
            const int PRICE_DECIMALS = 6;

            try
            {
                // Consultar líneas actualizadas directamente desde BD destino
                string queryLineasDestino = $@"SELECT 
                                            ""LineNum""
                                           ,""ItemCode""
                                           ,""Price"" AS ""UnitPrice""
                                           FROM ""{bdDestino}"".POR1
                                           WHERE ""DocEntry"" = ?";

                var responseLines = new List<SapDocumentLine>();

                OdbcCommand cmd = new OdbcCommand(queryLineasDestino, conn);
                cmd.Parameters.Add(new OdbcParameter("@entry", entryActualizar));

                using (OdbcDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        responseLines.Add(new SapDocumentLine
                        {
                            LineNum = rdr.GetInt32(rdr.GetOrdinal("LineNum")),
                            ItemCode = rdr["ItemCode"].ToString(),
                            UnitPrice = rdr.GetDecimal(rdr.GetOrdinal("UnitPrice"))
                        });
                    }
                }

                if (!responseLines.Any())
                {
                    objRegistraLog.Graba($"[{DateTime.Now:HH:mm:ss tt}] [ADVERTENCIA] " +
                        $"ActualizarComparacion EntryActualizar {entryActualizar}, BdDest: {bdDestino}: " +
                        $"No se encontraron líneas en BD destino. BdOrig: {bdOrigen}, DocEntryOrigen {enEntryOrig}");
                    return;
                }

                // Diccionario indexado por LineNum
                var destByLineNum = responseLines.ToDictionary(l => l.LineNum, l => l);

                var differences = new List<string>();

                foreach (var sentLine in sentLines)
                {
                    string itemCode = sentLine["ItemCode"]?.ToString();
                    decimal sentPrice = sentLine["UnitPrice"]?.ToObject<decimal>() ?? 0;
                    int lineNum = sentLine["LineNum"]?.ToObject<int>() ?? -1;

                    if (lineNum == -1)
                    {
                        objRegistraLog.Graba($"[{DateTime.Now:HH:mm:ss tt}] [ADVERTENCIA] " +
                            $"EntryActualizar {entryActualizar}, BdDest: {bdDestino}: " +
                            $"ItemCode {itemCode} no tiene LineNum, se omite. BdOrig: {bdOrigen}, DocEntryOrigen {enEntryOrig}");
                        continue;
                    }

                    if (!destByLineNum.TryGetValue(lineNum, out SapDocumentLine destLine))
                    {
                        objRegistraLog.Graba($"[{DateTime.Now:HH:mm:ss tt}] [ADVERTENCIA] " +
                            $"EntryActualizar {entryActualizar}, BdDest: {bdDestino}: " +
                            $"LineNum {lineNum} ItemCode {itemCode} no se encontró en BD destino. BdOrig: {bdOrigen}, DocEntryOrigen {enEntryOrig}");
                        continue;
                    }

                    bool sonDiferentes = Math.Round(sentPrice, PRICE_DECIMALS) != Math.Round(destLine.UnitPrice, PRICE_DECIMALS);

                    if (sonDiferentes)
                    {
                        differences.Add(
                            $"  LineNum: {lineNum,4} | ItemCode: {itemCode,-20} | " +
                            $"Precio enviado: {sentPrice,14:F6} | Precio BD Destino: {destLine.UnitPrice,14:F6}"
                        );
                    }
                }

                if (differences.Any())
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"[{DateTime.Now:HH:mm:ss tt}] [DIFERENCIA DE PRECIOS - ACTUALIZACION] " +
                                  $"BDOrigen: {bdOrigen}, DocEntryOrigen: {enEntryOrig} | " +
                                  $"BDDestino: {bdDestino}, DocEntryDestino: {entryActualizar}");

                    foreach (var diff in differences)
                        sb.AppendLine(diff);

                    sb.AppendLine(new string('-', 90));
                    objRegistraLog.Graba(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                objRegistraLog.Graba($"[{DateTime.Now:HH:mm:ss tt}] [ERROR] ComparePricesAndLogActualizacion " +
                    $"EntryActualizar {entryActualizar}, BdOrig {bdOrigen} | BdDest {bdDestino}: {ex.Message}");
            }
        }

        #endregion

        /*   
         public static void ADUANA(string bd, string toctoc)
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

                     string queryEnCRSV = $"SELECT  \"CstGrpName\" FROM  \"{Conexion.BD_Hechizo}\".\"OARG\" where \"CstGrpName\" not in (SELECT  \"CstGrpName\" FROM \"{bd}\".\"OARG\")"; //Query Encabezado
                     OdbcCommand CmD = new OdbcCommand(queryEnCRSV, conn2); // llama al Query Encabezado con la conexion
                     conn2.Open();
                     using (OdbcDataReader dr = CmD.ExecuteReader())
                     {
                         if (dr.RecordsAffected != 0)
                         {
                             while (dr.Read())
                             {

                                 CstGrpName = dr["CstGrpName"].ToString();
                                 string querY = $"SELECT  \"GroupNum\",IFNULL(\"Custom\",0) \"Custom\",\"BuyTax\" \"BuyTax\",IFNULL(\"OtherTax\",0) \"OtherTax\",IFNULL(\"TotalTax\",0) \"TotalTax\",\"Locked\",\"DataSource\",\"UserSign\",\"cstAllcAcc\",\"cstExpAcc\",\"PortAddr\", \"PortState\", \"ExciExpAcc\", \"ExciAlcAcc\" FROM  \"{Conexion.BD_Hechizo}\".\"OARG\" where \"CstGrpName\"='" + CstGrpName + "'"; //Query Encabezado
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
                                         CrearADUANATODAS(bd, CstGrpName, GroupNum, Custom, BuyTax, OtherTax, TotalTax, Locked, DataSource, cstAllcAcc, cstExpAcc, PortAddr, PortState, ExciExpAcc, ExciAlcAcc, toctoc);
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
         public static void SCSucursal(string bd, string toctoc)
         {
             int resul = 0;
             string nomcod = string.Empty;
             string ReqName;
             string U_PROYECTO;
             string Comments;
             string TABLA;
             int DocEntry;
             int idanexo = 0;
             string U_TipoCompra;
             string DocDate;
             string DocDueDate;
             string TaxDate;
             string ReqDate;
             string doctype;
             string desc;
             string adel;
             string pagopro;
             double porcen;
             //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";
             Respuesta R = new Respuesta();
             try
             {
                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn1 = new OdbcConnection(Conexion.strCon))
                 {
                     {

                         string queryEn = $"select \"DocType\",\"DocEntry\",\"ReqName\" ,\"U_PROYECTO\",\"U_TipoCompra\",\"Comments\",\"DocDate\",\"DocDueDate\",\"TaxDate\",\"ReqDate\",\"U_Adelanto\",  \"U_Porcentaje\", \"U_Pago_Procesado\",\"AtcEntry\" from \"{bd}\".\"OPRQ\" where \"U_Enviado\" ='0' and \"U_ENVIOSUC\"='Y' and \"U_TipoCompra\" in ('1','2') and \"DocStatus\"='O'";

                         // string queryEn = $"select \"DocType\",\"DocEntry\",\"ReqName\" ,\"U_PROYECTO\",\"U_TipoCompra\",\"Comments\",\"DocDate\",\"DocDueDate\",\"TaxDate\",\"ReqDate\",\"U_Adelanto\",  \"U_Porcentaje\", \"U_Pago_Procesado\" from \"{bd}\".\"OPRQ\" where \"DocNum\" in (160, 164, 165)";                        
                         OdbcCommand CmD1 = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                         conn.Open();
                         using (OdbcDataReader dr2 = CmD1.ExecuteReader())
                             while (dr2.Read())
                             {
                                 idanexo = 0;
                                 DocEntry = Convert.ToInt32(dr2["DocEntry"]);
                                 ReqName = dr2["ReqName"].ToString();
                                 U_PROYECTO = dr2["U_PROYECTO"].ToString();
                                 U_TipoCompra = dr2["U_TipoCompra"].ToString();
                                 Comments = dr2["Comments"].ToString();
                                 DocDate = dr2["DocDate"].ToString();
                                 DocDueDate = dr2["DocDueDate"].ToString();
                                 TaxDate = dr2["TaxDate"].ToString();
                                 ReqDate = dr2["ReqDate"].ToString();
                                 doctype = dr2["DocType"].ToString();
                                 //if (!DBNull.Value.Equals(dr2["AtcEntry"])) { idanexo = Convert.ToInt32(dr2["AtcEntry"]); }

                                 // idanexo = Convert.ToInt32(dr2["AtcEntry"]);
                                 adel = dr2["U_Adelanto"].ToString();

                                 porcen = Convert.ToDouble(dr2["U_Porcentaje"]);
                                 pagopro = dr2["U_Pago_Procesado"].ToString();

                                 //BUSCO CODIGO DE PROYECTO DE UNA VEZ
                                 string query = $"SELECT  \"PrjCode\"   FROM \"{Conexion.BD_Hechizo}\".\"OPRJ\" WHERE \"PrjName\"='" + U_PROYECTO + "'"; //Query Encabezado
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
                                 CrearSCTODAS(doctype, bd, DocEntry, ReqName, U_PROYECTO, U_TipoCompra, Comments, DocDate, DocDueDate, TaxDate, ReqDate, CODPROYE, adel, porcen, pagopro, idanexo, toctoc);
                                 //if (idanexo != 0)
                                 //{
                                 //    string TABLAtt = "OPRQ";//es la tabla destino en el origen donde asociara el anexo
                                 //    moveranexosorigen(bd, idanexo, TABLAtt, DocEntry);
                                 //}

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

         public static void SCImportc(string bd, string toctoc)
         {
             int resul = 0;
             string nomcod = string.Empty;
             string ReqName;
             string U_PROYECTO;
             string Comments;
             int DocEntry;
             string U_TipoCompra;
             string DocDate;
             int idanexo = 0;
             string DocDueDate;
             string TaxDate;
             string ReqDate;
             string doctype;
             string adel;
             string TABLA;
             string pagopro;
             double porcen;
             //omar en esta sc a worl brand tambien hay que mandar codigo interno_

             Respuesta R = new Respuesta();
             try
             {
                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn1 = new OdbcConnection(Conexion.strCon))
                 {
                     {

                         string queryEn = $"select \"DocType\",\"DocEntry\",\"ReqName\" ,\"U_PROYECTO\",\"U_TipoCompra\",\"Comments\",\"DocDate\",\"DocDueDate\",\"TaxDate\",\"ReqDate\",\"U_Adelanto\", \"U_Porcentaje\", \"U_Pago_Procesado\",\"AtcEntry\" from \"{bd}\".\"OPRQ\" where \"U_Enviado\" ='0' and \"U_ENVIOSUC\"='Y' and \"U_TipoCompra\"='3'  and \"DocStatus\"='O'";

                         OdbcCommand CmD1 = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                         conn.Open();
                         using (OdbcDataReader dr2 = CmD1.ExecuteReader())
                             while (dr2.Read())
                             {
                                 idanexo = 0;
                                 DocEntry = Convert.ToInt32(dr2["DocEntry"]);
                                 ReqName = dr2["ReqName"].ToString();
                                 U_PROYECTO = dr2["U_PROYECTO"].ToString();
                                 U_TipoCompra = dr2["U_TipoCompra"].ToString();
                                 Comments = dr2["Comments"].ToString();
                                 DocDate = dr2["DocDate"].ToString();
                                 DocDueDate = dr2["DocDueDate"].ToString();
                                 TaxDate = dr2["TaxDate"].ToString();
                                 ReqDate = dr2["ReqDate"].ToString();
                                 doctype = dr2["DocType"].ToString();
                                 //if (!DBNull.Value.Equals(dr2["AtcEntry"])) { idanexo = Convert.ToInt32(dr2["AtcEntry"]); }
                                 adel = dr2["U_Adelanto"].ToString();

                                 porcen = Convert.ToDouble(dr2["U_Porcentaje"]);
                                 pagopro = dr2["U_Pago_Procesado"].ToString();
                                 //BUSCO CODIGO DE PROYECTO DE UNA VEZ
                                 string query = $"SELECT  \"PrjCode\"   FROM \"{Conexion.BD_central}\".\"OPRJ\" WHERE \"PrjName\"='" + U_PROYECTO + "'"; //Query Encabezado
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
                                 CrearSCworld(doctype, bd, DocEntry, ReqName, U_PROYECTO, U_TipoCompra, Comments, DocDate, DocDueDate, TaxDate, ReqDate, CODPROYE, adel, porcen, pagopro, idanexo, toctoc);

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
         public static void OFERTAMCH_SUC(string bd, string toctoc)
         {
             int resul = 0;
             string nomcod = string.Empty;
             string CardName;
             string CardCode;
             string U_PROYECTO;
             string Comments;
             int DocEntry;
             string U_TipoCompra;
             string DocDate;
             string DocDueDate;
             string TaxDate;
             string ReqDate;
             int idanexo = 0;
             string TABLA;
             string adel;
             string pagopro;
             double porcen;
             //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";
             Respuesta R = new Respuesta();
             try
             {
                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn1 = new OdbcConnection(Conexion.strCon))
                 {
                     {
                         string queryEn = $"select \"DocEntry\",\"CardName\" ,\"U_PROYECTO\",\"U_TipoCompra\",\"Comments\",\"DocDate\",\"DocDueDate\",\"TaxDate\",\"ReqDate\",\"U_Adelanto\", \"U_Porcentaje\", \"U_Pago_Procesado\",\"AtcEntry\" from  \"{Conexion.BD_Hechizo}\".\"OPQT\" where \"U_Enviado\" ='0' and \"U_ENVIOSUC\"='Y' and \"DocStatus\"='O' AND \"U_Origen\"='" + bd + "' ";
                         OdbcCommand CmD1 = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                         conn.Open();
                         using (OdbcDataReader dr2 = CmD1.ExecuteReader())
                             while (dr2.Read())
                             {
                                 idanexo = 0;
                                 DocEntry = Convert.ToInt32(dr2["DocEntry"]);
                                 CardName = dr2["CardName"].ToString();
                                 U_PROYECTO = dr2["U_PROYECTO"].ToString();
                                 U_TipoCompra = dr2["U_TipoCompra"].ToString();
                                 Comments = dr2["Comments"].ToString();
                                 DocDate = dr2["DocDate"].ToString();
                                 DocDueDate = dr2["DocDueDate"].ToString();
                                 TaxDate = dr2["TaxDate"].ToString();
                                 ReqDate = dr2["ReqDate"].ToString();
                                 adel = dr2["U_Adelanto"].ToString();
                                 //if (!DBNull.Value.Equals(dr2["AtcEntry"])) { idanexo = Convert.ToInt32(dr2["AtcEntry"]); }

                                 porcen = Convert.ToDouble(dr2["U_Porcentaje"]);
                                 pagopro = dr2["U_Pago_Procesado"].ToString();
                                 //BUSCO CODIGO DE proveedor DE UNA VEZ
                                 string query = $"SELECT  \"CardCode\"   FROM \"{bd}\".\"OCRD\" WHERE \"CardName\"='" + CardName + "' and \"CardType\"='S'"; //Query Encabezado
                                 OdbcCommand cmand = new OdbcCommand(query, conn1);
                                 conn1.Open();
                                 OdbcDataReader dr1 = cmand.ExecuteReader();
                                 CardCode = string.Empty;
                                 while (dr1.Read())
                                 {
                                     CardCode = dr1["CardCode"].ToString();
                                 }
                                 dr1.Close();
                                 conn1.Close();
                                 //BUSCO CODIGO DE PROYECTO DE UNA VEZ
                                 query = $"SELECT  \"PrjCode\"   FROM \"{bd}\".\"OPRJ\" WHERE \"PrjName\"='" + U_PROYECTO + "'"; //Query Encabezado
                                 cmand = new OdbcCommand(query, conn1);
                                 conn1.Open();
                                 dr1 = cmand.ExecuteReader();
                                 string CODPROYE = string.Empty;
                                 while (dr1.Read())
                                 {
                                     CODPROYE = dr1["PrjCode"].ToString();
                                 }
                                 dr1.Close();
                                 conn1.Close();
                                 CrearOfertaTODAS(bd, DocEntry, CardCode, U_PROYECTO, U_TipoCompra, Comments, DocDate, DocDueDate, TaxDate, ReqDate, CODPROYE, adel, porcen, pagopro, idanexo, toctoc);

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
         public static void OC_MCH_a_SUC(string bd, string toctoc)
         {
             int resul = 0;
             string nomcod = string.Empty;
             string CardName;
             string CardCode;
             string U_PROYECTO;
             string Comments;
             int DocEntry;
             string U_TipoCompra;
             string DocDate;
             string DocDueDate;
             string TaxDate;
             string ReqDate;
             string doctype;
             string TABLA;
             int idanexo = 0;
             string adel;
             string pagopro;
             double porcen;
             //ver con omar ya que en la bf no va a estar la base como tal 
             Respuesta R = new Respuesta();
             try
             {
                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn1 = new OdbcConnection(Conexion.strCon))
                 {
                     {
                         // aca se busca  alreves a asolicitud de omar porque cuando se copia de oferta a pedido arrastra el enviado en 1 entonces 1 significa queno se ha enviado y 0 es que ya se envio solo en oc 
                         string queryEn = $"select \"DocType\",\"DocEntry\",\"CardName\" ,\"U_PROYECTO\",\"U_TipoCompra\",\"Comments\",\"DocDate\",\"DocDueDate\",\"TaxDate\",\"ReqDate\",\"U_Adelanto\", \"U_Porcentaje\", \"U_Pago_Procesado\",\"AtcEntry\" from  \"{Conexion.BD_Hechizo}\".\"OPOR\" where \"U_Enviado\" ='1' and  \"DocType\"='I' and \"U_ENVIOSUC\"='Y' and \"U_Origen\"='" + bd + "'  and \"DocStatus\"='O'";//bd.Substring(6, 6)
                         //string queryEn = $"select \"DocType\",\"DocEntry\",\"CardName\" ,\"U_PROYECTO\",\"U_TipoCompra\",\"Comments\",\"DocDate\",\"DocDueDate\",\"TaxDate\",\"ReqDate\",\"U_Adelanto\", \"U_Porcentaje\", \"U_Pago_Procesado\",\"AtcEntry\" from  \"{Conexion.ORIGEN}\".\"OPOR\" where \"DocNum\" ='3728'";//bd.Substring(6, 6)

                         OdbcCommand CmD1 = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                         conn.Open();
                         using (OdbcDataReader dr2 = CmD1.ExecuteReader())
                             while (dr2.Read())
                             {
                                 idanexo = 0;
                                 DocEntry = Convert.ToInt32(dr2["DocEntry"]);
                                 CardName = dr2["CardName"].ToString();
                                 U_PROYECTO = dr2["U_PROYECTO"].ToString();
                                 U_TipoCompra = dr2["U_TipoCompra"].ToString();
                                 Comments = dr2["Comments"].ToString();
                                 DocDate = dr2["DocDate"].ToString();
                                 DocDueDate = dr2["DocDueDate"].ToString();
                                 TaxDate = dr2["TaxDate"].ToString();
                                 ReqDate = dr2["ReqDate"].ToString();
                                 // if (!DBNull.Value.Equals(dr2["AtcEntry"])) { idanexo = Convert.ToInt32(dr2["AtcEntry"]); }
                                 adel = dr2["U_Adelanto"].ToString();
                                 doctype = dr2["DocType"].ToString();
                                 porcen = Convert.ToDouble(dr2["U_Porcentaje"]);
                                 pagopro = dr2["U_Pago_Procesado"].ToString();
                                 //en la sucursal tiene que aquedar a nombre de worl brand 
                                 //string query = $"SELECT  \"CardCode\"   FROM \"{bd}\".\"OCRD\" WHERE \"CardName\"='" + CardName + "'"; //Query Encabezado

                                 string query = $"SELECT  \"CardCode\"   FROM \"{bd}\".\"OCRD\" WHERE \"CardName\"='" + CardName + "' and \"CardType\" ='S'"; //Query Encabezado
                                 //string query = $"SELECT  \"CardCode\"   FROM \"{bd}\".\"OCRD\" WHERE \"U_Base_Datos\"='" + Conexion.ORIGEN + "' and \"CardType\" ='S'"; //Query Encabezado

                                 OdbcCommand cmand = new OdbcCommand(query, conn1);
                                 conn1.Open();
                                 OdbcDataReader dr1 = cmand.ExecuteReader();
                                 CardCode = string.Empty;
                                 while (dr1.Read())
                                 {
                                     CardCode = dr1["CardCode"].ToString();
                                 }
                                 dr1.Close();
                                 conn1.Close();

                                 //BUSCO CODIGO DE PROYECTO DE UNA VEZ
                                 query = $"SELECT  \"PrjCode\"   FROM \"{bd}\".\"OPRJ\" WHERE \"PrjName\"='" + U_PROYECTO + "'"; //Query Encabezado
                                 cmand = new OdbcCommand(query, conn1);
                                 conn1.Open();
                                 dr1 = cmand.ExecuteReader();
                                 string CODPROYE = string.Empty;
                                 while (dr1.Read())
                                 {
                                     CODPROYE = dr1["PrjCode"].ToString();
                                 }
                                 dr1.Close();
                                 conn1.Close();
                                 CrearOC_MCH(doctype, bd, DocEntry, CardCode, U_PROYECTO, U_TipoCompra, Comments, DocDate, DocDueDate, TaxDate, CODPROYE, adel, porcen, pagopro, idanexo, toctoc);
                                 //if (idanexo != 0)
                                 //{
                                 //    string TABLAtt = "OPOR";//es la tabla destino en el origen donde asociara el anexo
                                 //    moveranexosdestino(bd, idanexo, TABLAtt, DocEntry);
                                 //}
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
                 objRegistraLog.Graba("Error OC_World_a_SUC : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
             }
         }
         public static void OC_World_a_SUC(string bd, string toctoc)
         {
             int resul = 0;
             string nomcod = string.Empty;
             string CardName;
             string CardCode;
             string U_PROYECTO;
             string Comments;
             int DocEntry;
             string U_TipoCompra;
             string DocDate;
             string DocDueDate;
             string TaxDate;
             string ReqDate;
             string TABLA;
             int idanexo = 0;
             string adel;
             string pagopro;
             double porcen;
             //ver con omar ya que en la bf no va a estar la base como tal 
             Respuesta R = new Respuesta();
             try
             {
                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn1 = new OdbcConnection(Conexion.strCon))
                 {
                     {
                         // aca se busca  alreves a asolicitud de omar porque cuando se copia de oferta a pedido arrastra el enviado en 1 entonces 1 significa queno se ha enviado y 0 es que ya se envio solo en oc 
                         string queryEn = $"select \"DocEntry\",\"CardName\" ,\"U_PROYECTO\",\"U_TipoCompra\",\"Comments\",\"DocDate\",\"DocDueDate\",\"TaxDate\",\"ReqDate\",\"U_Adelanto\",  \"U_Porcentaje\", \"U_Pago_Procesado\",\"AtcEntry\"  from  \"{Conexion.BD_central}\".\"OPOR\" where \"U_Enviado\" ='1' and \"U_ENVIOSUC\"='Y' and \"U_Origen\"='" + bd + "'  and \"DocStatus\"='O'";//bd.Substring(6, 6)
                         OdbcCommand CmD1 = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                         conn.Open();
                         using (OdbcDataReader dr2 = CmD1.ExecuteReader())
                             while (dr2.Read())
                             {
                                 idanexo = 0;
                                 DocEntry = Convert.ToInt32(dr2["DocEntry"]);
                                 CardName = dr2["CardName"].ToString();
                                 U_PROYECTO = dr2["U_PROYECTO"].ToString();
                                 U_TipoCompra = dr2["U_TipoCompra"].ToString();
                                 Comments = dr2["Comments"].ToString();
                                 DocDate = dr2["DocDate"].ToString();
                                 DocDueDate = dr2["DocDueDate"].ToString();
                                 TaxDate = dr2["TaxDate"].ToString();
                                 ReqDate = dr2["ReqDate"].ToString();
                                 //if (!DBNull.Value.Equals(dr2["AtcEntry"])) { idanexo = Convert.ToInt32(dr2["AtcEntry"]); }
                                 adel = dr2["U_Adelanto"].ToString();

                                 porcen = Convert.ToDouble(dr2["U_Porcentaje"]);
                                 pagopro = dr2["U_Pago_Procesado"].ToString();
                                 //en la sucursal tiene que aquedar a nombre de worl brand 
                                 string query = $"SELECT  \"CardCode\"   FROM \"{bd}\".\"OCRD\" WHERE \"U_Base_Datos\"='" + Conexion.BD_central + "' and \"CardType\" ='S'"; //Query Encabezado

                                 OdbcCommand cmand = new OdbcCommand(query, conn1);
                                 conn1.Open();
                                 OdbcDataReader dr1 = cmand.ExecuteReader();
                                 CardCode = string.Empty;
                                 while (dr1.Read())
                                 {
                                     CardCode = dr1["CardCode"].ToString();
                                 }
                                 dr1.Close();
                                 conn1.Close();
                                 //BUSCO CODIGO DE PROYECTO DE UNA VEZ
                                 query = $"SELECT  \"PrjCode\"   FROM \"{bd}\".\"OPRJ\" WHERE \"PrjName\"='" + U_PROYECTO + "'"; //Query Encabezado
                                 cmand = new OdbcCommand(query, conn1);
                                 conn1.Open();
                                 dr1 = cmand.ExecuteReader();
                                 string CODPROYE = string.Empty;
                                 while (dr1.Read())
                                 {
                                     CODPROYE = dr1["PrjCode"].ToString();
                                 }
                                 dr1.Close();
                                 conn1.Close();
                                 //CrearOC_WORLD(bd, DocEntry, CardCode, U_PROYECTO, U_TipoCompra, Comments, DocDate, DocDueDate, TaxDate, CODPROYE, adel,  porcen, pagopro,idanexo);
                                 //if (idanexo != 0)
                                 //{
                                 //    string TABLAtt = "OPOR";//es la tabla destino en el origen donde asociara el anexo
                                 //    moveranexosdestinoWL (bd, idanexo, TABLAtt, DocEntry);
                                 //}
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
                 objRegistraLog.Graba("Error OC_World_a_SUC : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
             }
         }

         public static void OCSucursal_MCH(string bd, string toctoc)
         {
             int resul = 0;
             string nomcod = string.Empty;
             string ReqName;
             string U_PROYECTO;
             string Comments;
             int idanexo = 0;
             int DocEntry;
             string U_TipoCompra;
             string DocDate;
             string DocDueDate;
             string TaxDate;
             string ReqDate;
             string doctype;
             string desc;
             string adel;
             string TABLA;
             string pagopro;
             double porcen;
             string CardCode;
             string CardName;
             //string connectionString = "DRIVER={HDBODBC};SERVERNODE=hana-HANA-PNM2005:30015;Database=20025_COMERCIAL_RALI_TEST;Uid=SYSTEM;Pwd=Fc53rp!!!";
             Respuesta R = new Respuesta();
             try
             {
                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn1 = new OdbcConnection(Conexion.strCon))
                 {
                     {
                         string queryEn = $"select \"DocType\",\"DocEntry\",\"ReqName\" ,\"U_PROYECTO\",\"U_TipoCompra\",\"Comments\",\"DocDate\",\"DocDueDate\",\"TaxDate\",\"ReqDate\",\"U_Adelanto\",  \"U_Porcentaje\", \"U_Pago_Procesado\", \"CardName\",\"AtcEntry\"  from \"{bd}\".\"OPOR\" where  \"DocType\"= 'S' and \"U_Enviado\" ='0' and \"U_ENVIOSUC\"='Y' and \"DocStatus\"='O' and \"U_TipoCompra\" in ('1','2') ";

                         // string queryEn = $"select \"DocType\",\"DocEntry\",\"ReqName\" ,\"U_PROYECTO\",\"U_TipoCompra\",\"Comments\",\"DocDate\",\"DocDueDate\",\"TaxDate\",\"ReqDate\",\"U_Adelanto\",  \"U_Porcentaje\", \"U_Pago_Procesado\", \"CardName\",\"AtcEntry\"  from \"{bd}\".\"OPOR\" where  \"DocNum\"= '3574' ";
                         //string queryEn = $"select \"DocType\",\"DocEntry\",\"ReqName\" ,\"U_PROYECTO\",\"U_TipoCompra\",\"Comments\",\"DocDate\",\"DocDueDate\",\"TaxDate\",\"ReqDate\",\"U_Adelanto\",  \"U_Porcentaje\", \"U_Pago_Procesado\", \"CardName\" from \"{bd}\".\"OPOR\" where  \"DocType\"= 'S' and \"U_Enviado\" ='0' and \"U_ENVIOSUC\"='Y' and \"DocStatus\"='O' and \"U_TipoCompra\" in ('1','2') AND \"DocDate\" >=(SELECT ADD_DAYS(TO_DATE(CURRENT_DATE, 'YYYY-MM-DD'), -5) FROM DUMMY) ";
                         // string queryEn = $"select \"DocType\",\"DocEntry\",\"ReqName\" ,\"U_PROYECTO\",\"U_TipoCompra\",\"Comments\",\"DocDate\",\"DocDueDate\",\"TaxDate\",\"ReqDate\",\"U_Adelanto\",  \"U_Porcentaje\", \"U_Pago_Procesado\", \"CardName\" from \"{bd}\".\"OPOR\" where  \"DocType\"= 'S' and  \"DocNum\" in (1318)" ;


                         OdbcCommand CmD1 = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                         conn.Open();
                         using (OdbcDataReader dr2 = CmD1.ExecuteReader())
                             while (dr2.Read())
                             {
                                 idanexo = 0;
                                 DocEntry = Convert.ToInt32(dr2["DocEntry"]);
                                 if (DocEntry == 56655)
                                 {
                                     string hh = "pp";
                                 }
                                 ReqName = dr2["ReqName"].ToString();
                                 U_PROYECTO = dr2["U_PROYECTO"].ToString();
                                 U_TipoCompra = dr2["U_TipoCompra"].ToString();
                                 Comments = dr2["Comments"].ToString();
                                 DocDate = dr2["DocDate"].ToString();
                                 DocDueDate = dr2["DocDueDate"].ToString();
                                 TaxDate = dr2["TaxDate"].ToString();
                                 ReqDate = dr2["ReqDate"].ToString();
                                 doctype = dr2["DocType"].ToString();
                                 //if (!DBNull.Value.Equals(dr2["AtcEntry"])) { idanexo = Convert.ToInt32(dr2["AtcEntry"]); }

                                 adel = dr2["U_Adelanto"].ToString();

                                 porcen = Convert.ToDouble(dr2["U_Porcentaje"]);
                                 pagopro = dr2["U_Pago_Procesado"].ToString();

                                 CardName = dr2["CardName"].ToString();
                                 string query = $"SELECT  \"CardCode\"   FROM \"{Conexion.BD_Hechizo}\".\"OCRD\" WHERE  \"CardType\" ='S' and \"CardName\"= '" + CardName + "'"; //Query Encabezado
                                                                                                                                                                                 // string query = $"SELECT  \"CardCode\" FROM \"{Conexion.basecentral}\".\"OCRD\" WHERE \"CardName\"='" + CardName + "'"; //Query Encabezado
                                 OdbcCommand cmand = new OdbcCommand(query, conn1);
                                 conn1.Open();
                                 OdbcDataReader dr1 = cmand.ExecuteReader();
                                 CardCode = string.Empty;
                                 while (dr1.Read())
                                 {
                                     CardCode = dr1["CardCode"].ToString();
                                 }
                                 dr1.Close();
                                 conn1.Close();



                                 //BUSCO CODIGO DE PROYECTO DE UNA VEZ
                                 //    query = $"SELECT  \"PrjCode\"   FROM \"{Conexion.ORIGEN}\".\"OPRJ\" WHERE \"PrjName\"='" + U_PROYECTO + "'"; //Query Encabezado
                                 //    cmand = new OdbcCommand(query, conn1);
                                 //    conn1.Open();
                                 //dr1 = cmand.ExecuteReader();
                                 //    string CODPROYE = string.Empty;
                                 //    while (dr1.Read())
                                 //    {
                                 //        CODPROYE = dr1["PrjCode"].ToString();
                                 //    }
                                 //    dr1.Close();
                                 //    conn1.Close();

                                 CrearOCTODASSUCMCH(doctype, bd, DocEntry, ReqName, U_PROYECTO, U_TipoCompra, Comments, DocDate, DocDueDate, TaxDate, ReqDate, adel, porcen, pagopro, CardCode, idanexo, toctoc);


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
                 objRegistraLog.Graba("Error al consultar ocSUCMCH : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
             }
         }
         public static void Ovta_World_a_Wolrd(string bd, string toctoc)
         {
             int resul = 0;
             string nomcod = string.Empty;
             string CardName;
             string CardCode;
             string U_PROYECTO;
             string Comments;
             int DocEntry;
             string U_TipoCompra;
             string DocDate;

             string DocDueDate;
             string TaxDate;
             string ReqDate;
             string vacio = "";
             string adel;
             string pagopro;
             double porcen;
             //creo la orden de venta en worl brand con carcode de bd ___
             Respuesta R = new Respuesta();
             try
             {
                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn1 = new OdbcConnection(Conexion.strCon))
                 {
                     {
                         string queryEn = $"select \"DocEntry\",\"CardName\" ,\"U_PROYECTO\",\"U_TipoCompra\",\"Comments\",\"DocDate\",\"DocDueDate\",\"TaxDate\",\"ReqDate\",\"U_Adelanto\",  \"U_Porcentaje\", \"U_Pago_Procesado\" from  \"{Conexion.BD_central}\".\"OPOR\" where   \"U_ENVIOSUC\"='Y' and \"U_Origen\"='" + bd + "' and ifnull(\"U_creavta\",'')='" + vacio + "'  and \"DocStatus\"='O'";
                         OdbcCommand CmD1 = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                         conn.Open();
                         using (OdbcDataReader dr2 = CmD1.ExecuteReader())
                             while (dr2.Read())
                             {

                                 DocEntry = Convert.ToInt32(dr2["DocEntry"]);
                                 CardName = dr2["CardName"].ToString();
                                 U_PROYECTO = dr2["U_PROYECTO"].ToString();
                                 U_TipoCompra = dr2["U_TipoCompra"].ToString();
                                 Comments = dr2["Comments"].ToString();
                                 DocDate = dr2["DocDate"].ToString();
                                 DocDueDate = dr2["DocDueDate"].ToString();
                                 TaxDate = dr2["TaxDate"].ToString();
                                 adel = dr2["U_Adelanto"].ToString();

                                 porcen = Convert.ToDouble(dr2["U_Porcentaje"]);
                                 pagopro = dr2["U_Pago_Procesado"].ToString();
                                 //ReqDate = dr2["ReqDate"].ToString();
                                 //el cardcode en worlbrand de cliente es la sucursal

                                 string query = $"SELECT  \"CardCode\"   FROM \"{Conexion.BD_central}\".\"OCRD\" WHERE \"U_Base_Datos\"='" + bd + "' and \"CardType\" ='C'"; //Query Encabezado
                                                                                                                                                                             // string query = $"SELECT  \"CardCode\" FROM \"{Conexion.basecentral}\".\"OCRD\" WHERE \"CardName\"='" + CardName + "'"; //Query Encabezado
                                 OdbcCommand cmand = new OdbcCommand(query, conn1);
                                 conn1.Open();
                                 OdbcDataReader dr1 = cmand.ExecuteReader();
                                 CardCode = string.Empty;
                                 while (dr1.Read())
                                 {
                                     CardCode = dr1["CardCode"].ToString();
                                 }
                                 dr1.Close();
                                 conn1.Close();
                                 //BUSCO CODIGO DE PROYECTO DE UNA VEZ
                                 query = $"SELECT  \"PrjCode\"   FROM \"{Conexion.BD_central}\".\"OPRJ\" WHERE \"PrjName\"='" + U_PROYECTO + "'"; //Query Encabezado
                                 cmand = new OdbcCommand(query, conn1);
                                 conn1.Open();
                                 dr1 = cmand.ExecuteReader();
                                 string CODPROYE = string.Empty;
                                 while (dr1.Read())
                                 {
                                     CODPROYE = dr1["PrjCode"].ToString();
                                 }
                                 dr1.Close();
                                 conn1.Close();
                                 CrearOVTA(bd, DocEntry, CardCode, U_PROYECTO, U_TipoCompra, Comments, DocDate, DocDueDate, TaxDate, CODPROYE, adel, porcen, pagopro, toctoc);

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
                 objRegistraLog.Graba("Error Ovta_World_a_Wolrd : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
             }
         }

         private static Respuesta moveranexosorigen(string bd, int entryanex, string tablita, int entry, int entryorigen, string basebd, string toctoc)
         {
             int resul = 0;
             Respuesta R = new Respuesta();
             int entryatc;
             string query;
             // String ruta;

             string RUTAFINAL1;

             try
             {
                 using (OdbcConnection conn1 = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn11 = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn2 = new OdbcConnection(Conexion.strCon))
                 {
                     if (tablita == "ODRF")
                     {
                         query = $"SELECT  \"AtcEntry\" FROM  \"{basebd}\".\"OPOR\" WHERE \"DocEntry\"=" + entryorigen + ""; //Query Encabezado
                     }
                     else
                     {
                         query = $"SELECT  \"AtcEntry\" FROM  \"{basebd}\".\"{tablita}\" WHERE \"DocEntry\"=" + entryorigen + ""; //Query Encabezado
                     }

                     OdbcCommand cmand2 = new OdbcCommand(query, conn2);
                     conn2.Open();
                     OdbcDataReader dr2 = cmand2.ExecuteReader();
                     while (dr2.Read())
                     {
                         if (dr2.HasRows)
                         {
                             entryatc = Convert.ToInt32(dr2["AtcEntry"]);
                             {
                                 query = $"SELECT  \"FileName\" ,\"trgtPath\",\"FileExt\" FROM  \"{bd}\".\"ATC1\" WHERE \"AbsEntry\"=" + entryatc + ""; //Query Encabezado
                                 OdbcCommand cmand1 = new OdbcCommand(query, conn1);
                                 conn1.Open();
                                 OdbcDataReader dr3 = cmand1.ExecuteReader();
                                 string filename = string.Empty;
                                 string rutt = string.Empty;
                                 string ext = string.Empty;
                                 string rutanew = $@"\\ccc\ccc$\CloudControlCenter\SharedFolder\{Conexion.BD_Hechizo}\Attachments";
                                 string rutafin = $@"\\ccc\ccc$\CloudControlCenter\SharedFolder\{bd}\Attachments";
                                 var list = new ArrayList();
                                 while (dr3.Read())
                                 {
                                     filename = dr3["FileName"].ToString();
                                     rutt = dr3["trgtPath"].ToString();
                                     ext = dr3["FileExt"].ToString();
                                     RUTAFINAL = rutanew;
                                     string rutaoo = rutafin;
                                     Diapiattachment.CrearAttach(Conexion.BD_Hechizo, tablita, RUTAFINAL, rutaoo, filename, ext, entry);
                                     RUTAFINAL1 = RUTAFINAL + "\\" + filename + "." + ext;
                                     rutaoo = rutaoo + "\\" + filename + "." + ext;
                                 }
                                 if (gestorarchivos.paso == 1)
                                 {
                                     bdpublica = Conexion.BD_Hechizo;
                                     ConexionDiapi SAP = new ConexionDiapi();
                                     SAP.Open();
                                     {
                                         if (SAP.myCompany.Connected)
                                         {
                                             SAPbobsCOM.Attachments2 oATT = (SAPbobsCOM.Attachments2)SAP.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2);
                                             query = $"SELECT  \"FileName\" ,\"trgtPath\",\"FileExt\" FROM  \"{bd}\".\"ATC1\" WHERE \"AbsEntry\"=" + entryatc + ""; //Query Encabezado
                                             OdbcCommand cmand11 = new OdbcCommand(query, conn11);
                                             conn11.Open();
                                             OdbcDataReader dr55 = cmand11.ExecuteReader();
                                             while (dr55.Read())
                                             {
                                                 filename = dr55["FileName"].ToString();
                                                 rutt = dr55["trgtPath"].ToString();
                                                 ext = dr55["FileExt"].ToString();
                                                 string ruta = rutanew + "\\" + filename + "." + ext;
                                                 oATT.Lines.Add();
                                                 oATT.Lines.FileName = Path.GetFileNameWithoutExtension(ruta);
                                                 oATT.Lines.FileExtension = Path.GetExtension(ruta).TrimStart('.');
                                                 oATT.Lines.SourcePath = Path.GetDirectoryName(ruta);
                                                 oATT.Lines.Override = SAPbobsCOM.BoYesNoEnum.tYES;
                                             }
                                             // Agregar el attachment
                                             if (oATT.Add() == 0)
                                             {
                                                 string newObjectKey;
                                                 SAP.myCompany.GetNewObjectCode(out newObjectKey);
                                                 int iAttEntry = int.Parse(newObjectKey);
                                                 //if (conecta_service_layer(Conexion.ORIGEN, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                                                 //{
                                                 //    R.Codigo = -2;
                                                 //    R.Mensaje = RR;
                                                 //    objRegistraLog.Graba("Sin conectar a Service Layer /Attach: " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                                                 //                                                                                                                                        //return R;
                                                 //}
                                                 Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                                                 JsonObject.Add(new JProperty("AttachmentEntry", iAttEntry));

                                                 var myjs = JsonObject.ToString();
                                                 if (tablita == "OPOR")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseOrders(" + entry + ")", toctoc); //
                                                 }
                                                 else if (tablita == "OPQT")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseQuotations(" + entry + ")", toctoc); //
                                                 }
                                                 else if (tablita == "OPRQ")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseRequests(" + entry + ")", toctoc); //
                                                 }
                                                 else if (tablita == "ODRF")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Drafts(" + entry + ")", toctoc); //
                                                 }
                                                 if (resul == 0) // Caso que inserto 
                                                 {

                                                 }
                                                 else
                                                 {
                                                     objRegistraLog.Graba("Error CrearAttach: " + RR + " " + filename + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                                                     Inserterror(Conexion.BD_Hechizo.Substring(6, 6), "Attach", Convert.ToString(filename));
                                                 }

                                                 SAP.myCompany.Disconnect();

                                             }
                                             else
                                             {
                                                 string msgErr = SAP.myCompany.GetLastErrorDescription();

                                             }
                                             SAP.myCompany.Disconnect();
                                             dr55.Close();
                                             conn11.Close();

                                         }
                                         else
                                         {
                                             objRegistraLog.Graba("No conecto DIAPI: " + " / Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                                         }

                                     }
                                 }
                                 dr3.Close();
                                 conn1.Close();
                                 LiberaMemoria.FlushMemory();
                             }
                         }
                     }
                     dr2.Close();
                     conn2.Close();
                     LiberaMemoria.FlushMemory();
                 }

             }
             catch (Exception ex)
             {
                 R.Codigo = -1;
                 R.Mensaje = "Alerta: " + ex.Message;
                 objRegistraLog.Graba("Error al moveranexorigen: " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
             }
             return R;
         }
         public static Respuesta moveranexosdestino(string bd, int entryanex, string tablita, int entry, int entryorigen, string basebd, string toctoc)
         {
             int resul = 0;
             Respuesta R = new Respuesta();
             int entryatc;
             string RUTAFINAL1;
             try
             {
                 using (OdbcConnection conn1 = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn2 = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn11 = new OdbcConnection(Conexion.strCon))
                 {
                     string query = $"SELECT  \"AtcEntry\" FROM  \"{basebd}\".\"{tablita}\" WHERE \"DocEntry\"=" + entryorigen + ""; //Query Encabezado
                     OdbcCommand cmand2 = new OdbcCommand(query, conn2);
                     conn2.Open();
                     OdbcDataReader dr2 = cmand2.ExecuteReader();
                     while (dr2.Read())
                     {
                         if (dr2.HasRows)
                         {
                             entryatc = Convert.ToInt32(dr2["AtcEntry"]);
                             {
                                 string query1 = $"SELECT  \"FileName\" ,\"trgtPath\",\"FileExt\" FROM  \"{Conexion.BD_Hechizo}\".\"ATC1\" WHERE \"AbsEntry\"=" + entryatc + ""; //Query Encabezado
                                 OdbcCommand cmand1 = new OdbcCommand(query1, conn1);
                                 conn1.Open();
                                 OdbcDataReader dr3 = cmand1.ExecuteReader();
                                 string filename = string.Empty;
                                 string rutt = string.Empty;
                                 string ext = string.Empty;
                                 string rutanew = $@"\\ccc\ccc$\CloudControlCenter\SharedFolder\{bd}\Attachments";
                                 string rutafin = $@"\\ccc\ccc$\CloudControlCenter\SharedFolder\{Conexion.BD_Hechizo}\Attachments";
                                 var list = new ArrayList();
                                 while (dr3.Read())
                                 {
                                     filename = dr3["FileName"].ToString();
                                     rutt = dr3["trgtPath"].ToString();
                                     ext = dr3["FileExt"].ToString();
                                     String RUTAFINAL = rutanew;
                                     string rutaoo = rutafin;
                                     Diapiattachment.CrearAttach(bd, tablita, RUTAFINAL, rutaoo, filename, ext, entry);
                                     RUTAFINAL1 = RUTAFINAL + "\\" + filename + "." + ext;
                                     rutaoo = rutaoo + "\\" + filename + "." + ext;

                                 }
                                 if (gestorarchivos.paso == 1)
                                 {
                                     bdpublica = bd;
                                     ConexionDiapi SAP = new ConexionDiapi();
                                     SAP.Open();
                                     {
                                         if (SAP.myCompany.Connected)
                                         {
                                             SAPbobsCOM.Attachments2 oATT = (SAPbobsCOM.Attachments2)SAP.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2);
                                             query = $"SELECT  \"FileName\" ,\"trgtPath\",\"FileExt\" FROM  \"{Conexion.BD_Hechizo}\".\"ATC1\" WHERE \"AbsEntry\"=" + entryatc + ""; //Query Encabezado
                                             OdbcCommand cmand11 = new OdbcCommand(query, conn11);
                                             conn11.Open();
                                             OdbcDataReader dr55 = cmand11.ExecuteReader();
                                             while (dr55.Read())
                                             {
                                                 filename = dr55["FileName"].ToString();
                                                 rutt = dr55["trgtPath"].ToString();
                                                 ext = dr55["FileExt"].ToString();
                                                 //RUTAFINAL1 = rutanew + "\\" + filename + "." + ext;
                                                 string ruta = rutanew + "\\" + filename + "." + ext;
                                                 oATT.Lines.Add();
                                                 oATT.Lines.FileName = Path.GetFileNameWithoutExtension(ruta);
                                                 oATT.Lines.FileExtension = Path.GetExtension(ruta).TrimStart('.');
                                                 oATT.Lines.SourcePath = Path.GetDirectoryName(ruta);
                                                 oATT.Lines.Override = SAPbobsCOM.BoYesNoEnum.tYES;
                                             }
                                             // Agregar el attachment
                                             if (oATT.Add() == 0)
                                             {
                                                 string newObjectKey;
                                                 SAP.myCompany.GetNewObjectCode(out newObjectKey);
                                                 int iAttEntry = int.Parse(newObjectKey);
                                                 //if (conecta_service_layer(bd, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                                                 //{
                                                 //    R.Codigo = -2;
                                                 //    R.Mensaje = RR;
                                                 //    objRegistraLog.Graba("Sin conectar a Service Layer /Attach: " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                                                 //                                                                                                                                        //return R;
                                                 //}
                                                 Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                                                 JsonObject.Add(new JProperty("AttachmentEntry", iAttEntry));

                                                 var myjs = JsonObject.ToString();
                                                 if (tablita == "OPOR")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseOrders(" + entry + ")", toctoc); //
                                                 }
                                                 else if (tablita == "OPQT")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseQuotations(" + entry + ")", toctoc); //
                                                 }
                                                 else if (tablita == "OPRQ")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseRequests(" + entry + ")", toctoc); //
                                                 }
                                                 else if (tablita == "ODRF")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Drafts(" + entry + ")", toctoc); //
                                                 }
                                                 if (resul == 0) // Caso que inserto 
                                                 {

                                                 }
                                                 else
                                                 {
                                                     objRegistraLog.Graba("Error CrearAttach: " + RR + " " + filename + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                                                     Inserterror(bd.Substring(6, 6), "Attach", Convert.ToString(filename));
                                                 }

                                                 SAP.myCompany.Disconnect();

                                             }
                                             else
                                             {
                                                 string msgErr = SAP.myCompany.GetLastErrorDescription();

                                             }
                                             SAP.myCompany.Disconnect();
                                             dr55.Close();
                                             conn11.Close();

                                         }
                                         else
                                         {
                                             objRegistraLog.Graba("No conecto DIAPI: " + " / Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                                         }

                                     }
                                 }
                                 dr3.Close();
                                 conn1.Close();
                                 LiberaMemoria.FlushMemory();
                             }
                         }
                     }
                     dr2.Close();
                     conn2.Close();
                     LiberaMemoria.FlushMemory();
                 }
             }
             catch (Exception ex)
             {
                 R.Codigo = -1;
                 R.Mensaje = "Alerta: " + ex.Message;
                 objRegistraLog.Graba("Error al intentar Insertar anexo : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
             }
             return R;
         }
         public static Respuesta moveranexosdestinoWL(string bd, int entryanex, string tablita, int entry, int entryorigen, string basebd, string toctoc)
         {
             int resul = 0;
             Respuesta R = new Respuesta();
             int entryatc;
             string query;
             string query1;
             string RUTAFINAL1;
             try
             {
                 using (OdbcConnection conn1 = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn2 = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn11 = new OdbcConnection(Conexion.strCon))
                 {
                     query = $"SELECT  \"AtcEntry\" FROM  \"{basebd}\".\"{tablita}\" WHERE \"DocEntry\"=" + entryorigen + ""; //Query Encabezado
                     OdbcCommand cmand2 = new OdbcCommand(query, conn2);
                     conn2.Open();
                     OdbcDataReader dr2 = cmand2.ExecuteReader();
                     while (dr2.Read())
                     {
                         if (dr2.HasRows)
                         {
                             entryatc = Convert.ToInt32(dr2["AtcEntry"]);
                             {
                                 query = $"SELECT  \"FileName\" ,\"trgtPath\",\"FileExt\" FROM  \"{bd}\".\"ATC1\" WHERE \"AbsEntry\"=" + entryatc + ""; //Query Encabezado
                                 OdbcCommand cmand1 = new OdbcCommand(query, conn1);
                                 conn1.Open();
                                 OdbcDataReader dr3 = cmand1.ExecuteReader();
                                 string filename = string.Empty;
                                 string rutt = string.Empty;
                                 string ext = string.Empty;
                                 string rutanew = $@"\\ccc\ccc$\CloudControlCenter\SharedFolder\{Conexion.BD_central}\Attachments";
                                 string rutafin = $@"\\ccc\ccc$\CloudControlCenter\SharedFolder\{bd}\Attachments";
                                 var list = new ArrayList();
                                 while (dr3.Read())
                                 {
                                     filename = dr3["FileName"].ToString();
                                     rutt = dr3["trgtPath"].ToString();
                                     ext = dr3["FileExt"].ToString();
                                     String RUTAFINAL = rutanew;
                                     string rutaoo = rutafin;
                                     Diapiattachment.CrearAttach(Conexion.BD_central, tablita, RUTAFINAL, rutaoo, filename, ext, entry);
                                     RUTAFINAL1 = RUTAFINAL + "\\" + filename + "." + ext;
                                     rutaoo = rutaoo + "\\" + filename + "." + ext;

                                 }

                                 if (gestorarchivos.paso == 1)
                                 {
                                     bdpublica = Conexion.BD_central;
                                     ConexionDiapi SAP = new ConexionDiapi();
                                     SAP.Open();
                                     {
                                         if (SAP.myCompany.Connected)
                                         {
                                             SAPbobsCOM.Attachments2 oATT = (SAPbobsCOM.Attachments2)SAP.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2);
                                             query1 = $"SELECT  \"FileName\" ,\"trgtPath\",\"FileExt\" FROM  \"{bd}\".\"ATC1\" WHERE \"AbsEntry\"=" + entryatc + ""; //Query Encabezado
                                             OdbcCommand cmand11 = new OdbcCommand(query1, conn11);
                                             conn11.Open();
                                             OdbcDataReader dr55 = cmand11.ExecuteReader();
                                             while (dr55.Read())
                                             {
                                                 filename = dr55["FileName"].ToString();
                                                 rutt = dr55["trgtPath"].ToString();
                                                 ext = dr55["FileExt"].ToString();
                                                 string ruta = rutanew + "\\" + filename + "." + ext;
                                                 oATT.Lines.Add();
                                                 oATT.Lines.FileName = Path.GetFileNameWithoutExtension(ruta);
                                                 oATT.Lines.FileExtension = Path.GetExtension(ruta).TrimStart('.');
                                                 oATT.Lines.SourcePath = Path.GetDirectoryName(ruta);
                                                 oATT.Lines.Override = SAPbobsCOM.BoYesNoEnum.tYES;
                                             }
                                             // Agregar el attachment
                                             if (oATT.Add() == 0)
                                             {
                                                 string newObjectKey;
                                                 SAP.myCompany.GetNewObjectCode(out newObjectKey);
                                                 int iAttEntry = int.Parse(newObjectKey);
                                                 //if (conecta_service_layer(Conexion.basecentral, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                                                 //{
                                                 //    R.Codigo = -2;
                                                 //    R.Mensaje = RR;
                                                 //    objRegistraLog.Graba("Sin conectar a Service Layer /Attach: " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                                                 //                                                                                                                                        //return R;
                                                 //}
                                                 Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                                                 JsonObject.Add(new JProperty("AttachmentEntry", iAttEntry));

                                                 var myjs = JsonObject.ToString();
                                                 if (tablita == "OPOR")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseOrders(" + entry + ")", toctoc); //
                                                 }
                                                 else if (tablita == "OPQT")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseQuotations(" + entry + ")", toctoc); //
                                                 }
                                                 else if (tablita == "OPRQ")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseRequests(" + entry + ")", toctoc); //
                                                 }
                                                 else if (tablita == "ODRF")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Drafts(" + entry + ")", toctoc); //
                                                 }
                                                 if (resul == 0) // Caso que inserto 
                                                 {

                                                 }
                                                 else
                                                 {
                                                     objRegistraLog.Graba("Error CrearAttach: " + RR + " " + filename + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                                                     Inserterror(Conexion.BD_central.Substring(6, 6), "Attach", Convert.ToString(filename));
                                                 }

                                                 SAP.myCompany.Disconnect();

                                             }
                                             else
                                             {
                                                 string msgErr = SAP.myCompany.GetLastErrorDescription();

                                             }
                                             SAP.myCompany.Disconnect();
                                             dr55.Close();
                                             conn11.Close();

                                         }
                                         else
                                         {
                                             objRegistraLog.Graba("No conecto DIAPI: " + " / Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                                         }

                                     }
                                 }
                                 dr3.Close();
                                 conn1.Close();
                                 LiberaMemoria.FlushMemory();
                             }
                         }
                     }
                     dr2.Close();
                     conn2.Close();
                     LiberaMemoria.FlushMemory();
                 }
             }
             catch (Exception ex)
             {
                 R.Codigo = -1;
                 R.Mensaje = "Alerta: " + ex.Message;
                 objRegistraLog.Graba("Error al intentar moveranexdest : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
             }
             return R;
         }
         public static Respuesta moveranexosworld(string bd, int entryanex, string tablita, int entry, int entryorigen, string basebd, string toctoc)
         {
             int resul = 0;
             Respuesta R = new Respuesta();
             int entryatc;
             string RUTAFINAL1;
             string query;
             try
             {
                 using (OdbcConnection conn1 = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn2 = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn11 = new OdbcConnection(Conexion.strCon))
                 {
                     query = $"SELECT  \"AtcEntry\" FROM  \"{basebd}\".\"{tablita}\" WHERE \"DocEntry\"=" + entryorigen + ""; //Query Encabezado
                     OdbcCommand cmand2 = new OdbcCommand(query, conn2);
                     conn2.Open();
                     OdbcDataReader dr2 = cmand2.ExecuteReader();
                     while (dr2.Read())
                     {
                         if (dr2.HasRows)
                         {
                             entryatc = Convert.ToInt32(dr2["AtcEntry"]);
                             {
                                 query = $"SELECT  \"FileName\" ,\"trgtPath\",\"FileExt\" FROM  \"{Conexion.BD_central}\".\"ATC1\" WHERE \"AbsEntry\"=" + entryatc + ""; //Query Encabezado
                                 OdbcCommand cmand1 = new OdbcCommand(query, conn1);
                                 conn1.Open();
                                 OdbcDataReader dr3 = cmand1.ExecuteReader();
                                 string filename = string.Empty;
                                 string rutt = string.Empty;
                                 string ext = string.Empty;
                                 string rutanew = $@"\\ccc\ccc$\CloudControlCenter\SharedFolder\{bd}\Attachments";
                                 string rutafin = $@"\\ccc\ccc$\CloudControlCenter\SharedFolder\{Conexion.BD_central}\Attachments";
                                 var list = new ArrayList();
                                 while (dr3.Read())
                                 {
                                     filename = dr3["FileName"].ToString();
                                     rutt = dr3["trgtPath"].ToString();
                                     ext = dr3["FileExt"].ToString();
                                     String RUTAFINAL = rutanew;
                                     string rutaoo = rutafin;
                                     Diapiattachment.CrearAttach(bd, tablita, RUTAFINAL, rutaoo, filename, ext, entry);
                                     RUTAFINAL1 = RUTAFINAL + "\\" + filename + "." + ext;
                                     rutaoo = rutaoo + "\\" + filename + "." + ext;
                                 }

                                 if (gestorarchivos.paso == 1)
                                 {
                                     bdpublica = bd;
                                     string query1;
                                     ConexionDiapi SAP = new ConexionDiapi();
                                     SAP.Open();
                                     {
                                         if (SAP.myCompany.Connected)
                                         {
                                             SAPbobsCOM.Attachments2 oATT = (SAPbobsCOM.Attachments2)SAP.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2);
                                             query1 = $"SELECT  \"FileName\" ,\"trgtPath\",\"FileExt\" FROM  \"{Conexion.BD_central}\".\"ATC1\" WHERE \"AbsEntry\"=" + entryatc + ""; //Query Encabezado
                                             OdbcCommand cmand11 = new OdbcCommand(query1, conn11);
                                             conn11.Open();
                                             OdbcDataReader dr55 = cmand11.ExecuteReader();
                                             while (dr55.Read())
                                             {
                                                 filename = dr55["FileName"].ToString();
                                                 rutt = dr55["trgtPath"].ToString();
                                                 ext = dr55["FileExt"].ToString();
                                                 string ruta = rutanew + "\\" + filename + "." + ext;
                                                 oATT.Lines.Add();
                                                 oATT.Lines.FileName = Path.GetFileNameWithoutExtension(ruta);
                                                 oATT.Lines.FileExtension = Path.GetExtension(ruta).TrimStart('.');
                                                 oATT.Lines.SourcePath = Path.GetDirectoryName(ruta);
                                                 oATT.Lines.Override = SAPbobsCOM.BoYesNoEnum.tYES;
                                             }
                                             // Agregar el attachment
                                             if (oATT.Add() == 0)
                                             {
                                                 string newObjectKey;
                                                 SAP.myCompany.GetNewObjectCode(out newObjectKey);
                                                 int iAttEntry = int.Parse(newObjectKey);
                                                 //if (conecta_service_layer(bd, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                                                 //{
                                                 //    R.Codigo = -2;
                                                 //    R.Mensaje = RR;
                                                 //    objRegistraLog.Graba("Sin conectar a Service Layer /Attach: " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                                                 //                                                                                                                                        //return R;
                                                 //}
                                                 Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                                                 JsonObject.Add(new JProperty("AttachmentEntry", iAttEntry));

                                                 var myjs = JsonObject.ToString();
                                                 if (tablita == "OPOR")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseOrders(" + entry + ")", toctoc); //
                                                 }
                                                 else if (tablita == "OPQT")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseQuotations(" + entry + ")", toctoc); //
                                                 }
                                                 else if (tablita == "OPRQ")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseRequests(" + entry + ")", toctoc); //
                                                 }
                                                 else if (tablita == "ODRF")
                                                 {
                                                     resul = EnviaSapPorBasicAuth("PATCH", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Drafts(" + entry + ")", toctoc); //
                                                 }
                                                 if (resul == 0) // Caso que inserto 
                                                 {

                                                 }
                                                 else
                                                 {
                                                     objRegistraLog.Graba("Error CrearAttach: " + RR + " " + filename + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                                                     Inserterror(bd.Substring(6, 6), "Attach", Convert.ToString(filename));
                                                 }

                                                 SAP.myCompany.Disconnect();

                                             }
                                             else
                                             {
                                                 string msgErr = SAP.myCompany.GetLastErrorDescription();

                                             }
                                             SAP.myCompany.Disconnect();
                                             dr55.Close();
                                             conn11.Close();

                                         }
                                         else
                                         {
                                             objRegistraLog.Graba("No conecto DIAPI: " + " / Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));
                                         }

                                     }
                                 }
                                 dr3.Close();
                                 conn1.Close();
                                 LiberaMemoria.FlushMemory();
                             }
                         }
                     }
                     dr2.Close();
                     conn2.Close();
                     LiberaMemoria.FlushMemory();
                 }

             }
             catch (Exception ex)
             {
                 R.Codigo = -1;
                 R.Mensaje = "Alerta: " + ex.Message;
                 objRegistraLog.Graba("Error moveranexowl : " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
             }
             return R;
         }
         public static Respuesta CrearFAMI_SUBFAMI(string bd, string toctoc)
         {
             int resul = 0;
             string nomcod = string.Empty;
             Respuesta R = new Respuesta();
             try
             {
                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 {
                     string query = $"CALL \"{Conexion.BD_Hechizo}\".IC_ADD_FAMI_SUBFAMI"; //Query Encabezado
                     OdbcCommand CmD = new OdbcCommand(query, conn); // llama al Query Encabezado con la conexion
                     conn.Open();
                     using (OdbcDataReader dr = CmD.ExecuteReader())
                     {
                         LiberaMemoria.FlushMemory();
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

          public static Respuesta CrearADUANATODAS(string bd, string CstGrpName, string GroupNum, double Custom, double BuyTax, double OtherTax, double TotalTax, string Locked, string DataSource, string cstAllcAcc, string cstExpAcc, string PortAddr, string PortState, string ExciExpAcc, string ExciAlcAcc, string toctoc)
         {
             int resul = 0;
             Respuesta R = new Respuesta();
             try
             {
                 //if (conecta_service_layer(bd, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                 //{
                 //    R.Codigo = -2;
                 //    R.Mensaje = RR;
                 //    objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                 //    return R;
                 //}
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
                 resul = EnviaSapPorBasicAuth("POST", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/CustomsGroups", toctoc); //
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
         public static Respuesta CrearSCTODAS(string DocType, string bd, int DocEntry, string ReqName, string U_PROYECTO, string U_TipoCompra, string Comments, string DocDate, string DocDueDate, string TaxDate, string ReqDate, string CODPROYE, string adel, double porcen, string pagopro, int idanex, string toctoc)
         {
             int resul = 0;
             Respuesta R = new Respuesta();
             try
             {

                 //if (conecta_service_layer(Conexion.ORIGEN, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                 //{
                 //    R.Codigo = -2;
                 //    R.Mensaje = RR;
                 //    objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                 //    return R;
                 //}

                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 {
                     string queryEn = $"SELECT A.\"ItemCode\",	 A.\"Quantity\", A.\"VatGroup\",A.\"Dscription\" FROM \"{bd}\".\"PRQ1\" A left join \"{bd}\".\"OITM\" B  ON A.\"ItemCode\"= B.\"ItemCode\" where A.\"DocEntry\"='" + DocEntry + "' and  B.\"ItemType\"='I'";

                     OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                     conn.Open();
                     using (OdbcDataReader dr = CmD.ExecuteReader())
                     {
                         if (dr.HasRows)
                         {
                             List<DetallesOPORT> detallesFacturas = new List<DetallesOPORT>();
                             if (DocType == "I")
                             {
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
                             }
                             if (DocType == "S")
                             {
                                 while (dr.Read())
                                 {
                                     string ItemDescription = (string)dr["Dscription"];
                                     decimal Quantity = (decimal)dr["Quantity"];
                                     string impto = (string)dr["VatGroup"];
                                     string codprj = CODPROYE;


                                     detallesFacturas.Add(new DetallesOPORT
                                     {
                                         DocEntry = DocEntry,
                                         //ItemCode = ItemCode,
                                         Quantity = Quantity,
                                         // PriceBefDi = PriceBefDi,
                                         VatGroup = impto,
                                         PROYECTO = codprj,
                                         ItemDescription = ItemDescription
                                     });


                                 }

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

                                 JsonObject.Add(new JProperty("U_entrySC", DocEntry));
                                 JsonObject.Add(new JProperty("RequesterName", ReqName));
                                 JsonObject.Add(new JProperty("DocDate", date1));
                                 JsonObject.Add(new JProperty("DocDueDate", date2));
                                 JsonObject.Add(new JProperty("TaxDate", date3));
                                 JsonObject.Add(new JProperty("RequriedDate", date4));
                                 JsonObject.Add(new JProperty("SendNotification", "tNO"));
                                 JsonObject.Add(new JProperty("U_Origen", bd));
                                 JsonObject.Add(new JProperty("DocType", DocType));

                                 JsonObject.Add(new JProperty("U_Adelanto", adel));

                                 JsonObject.Add(new JProperty("U_Porcentaje", porcen));
                                 JsonObject.Add(new JProperty("U_Pago_Procesado", pagopro));
                                 var list = new ArrayList();
                                 if (DocType == "S")
                                 {
                                     foreach (var detalle in grupo.Detalles)
                                     {
                                         Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
                                         JsonObjectRow.Add(new JProperty("ItemDescription", detalle.ItemDescription));
                                         JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
                                         //JsonObjectRow.Add(new JProperty("UnitPrice", detalle.PriceBefDi));
                                         JsonObjectRow.Add(new JProperty("VatGroup", detalle.VatGroup));
                                         //JsonObjectRow.Add(new JProperty("ProjectCode", detalle.PROYECTO));

                                         list.Add(JsonObjectRow);
                                     }
                                 }
                                 if (DocType == "I")
                                 {
                                     foreach (var detalle in grupo.Detalles)
                                     {
                                         Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
                                         JsonObjectRow.Add(new JProperty("ItemCode", detalle.ItemCode));
                                         JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
                                         //JsonObjectRow.Add(new JProperty("UnitPrice", detalle.PriceBefDi));
                                         JsonObjectRow.Add(new JProperty("VatGroup", detalle.VatGroup));
                                         //JsonObjectRow.Add(new JProperty("ProjectCode", detalle.PROYECTO));

                                         list.Add(JsonObjectRow);
                                     }
                                 }
                                 JsonObject.Add(new JProperty("DocumentLines", list));
                                 string data = JsonObject.ToString();

                                 var myjs = JsonObject.ToString();
                                 resul = EnviaSapPorBasicAuth("POST", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseRequests", toctoc); //
                                 if (resul == 0) // Caso que inserto 
                                 {
                                     string query = $"UPDATE  \"{bd}\".\"OPRQ\" SET \"U_Enviado\" = '1' WHERE \"DocEntry\" =  '" + DocEntry + "'";



                                     // string queryAct = $"UPDATE \"{BRETASV}\".\"OITM\" SET \"U_act\" = '" + vac + "' WHERE \"ItemCode\" =  '" + En_DN + "'"; //Query Actualiza Entry_fac
                                     OdbcCommand cmand = new OdbcCommand(query, conn);
                                     OdbcDataReader dr1 = cmand.ExecuteReader();
                                     string TABLA = "OPRQ";//es la tabla destino en el origen donde asociara el anexo
                                     string basetabla = bd;
                                     // moveranexosorigen(bd, idanex, TABLA, ObjSap.DocEntry, DocEntry, basetabla);
                                 }
                                 else
                                 {
                                     objRegistraLog.Graba("Error CrearSCTODAS: " + RR + " " + DocEntry + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
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
         public static Respuesta CrearOCTODASSUCMCH(string DocType, string bd, int DocEntry, string ReqName, string U_PROYECTO, string U_TipoCompra, string Comments, string DocDate, string DocDueDate, string TaxDate, string ReqDate, string adel, double porcen, string pagopro, string CardCode, int idanex, string toctoc)
         {
             int resul = 0;
             Respuesta R = new Respuesta();
             try
             {

                 //if (conecta_service_layer(Conexion.ORIGEN, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                 //{
                 //    R.Codigo = -2;
                 //    R.Mensaje = RR;
                 //    objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                 //    return R;
                 //}

                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 {
                     string queryEn = $"SELECT \"ItemCode\",	 \"Quantity\", \"VatGroup\",\"Dscription\",\"LineTotal\" FROM \"{bd}\".\"POR1\" where \"DocEntry\"='" + DocEntry + "'";

                     OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                     conn.Open();
                     using (OdbcDataReader dr = CmD.ExecuteReader())
                     {
                         if (dr.HasRows)
                         {
                             List<DetallesOPORT> detallesFacturas = new List<DetallesOPORT>();
                             //if (DocType == "I")
                             //{
                             //    while (dr.Read())
                             //    {
                             //        string ItemCode = (string)dr["ItemCode"];
                             //        decimal Quantity = (decimal)dr["Quantity"];
                             //        string impto = (string)dr["VatGroup"];
                             //        string codprj = CODPROYE;


                             //        detallesFacturas.Add(new DetallesOPORT
                             //        {
                             //            DocEntry = DocEntry,
                             //            ItemCode = ItemCode,
                             //            Quantity = Quantity,
                             //            // PriceBefDi = PriceBefDi,
                             //            VatGroup = impto,
                             //            PROYECTO = codprj


                             //        });
                             //    }
                             //}
                             if (DocType == "S")
                             {
                                 while (dr.Read())
                                 {
                                     string ItemDescription = (string)dr["Dscription"];
                                     decimal Quantity = (decimal)dr["Quantity"];
                                     string impto = (string)dr["VatGroup"];
                                     decimal totalline = (decimal)dr["LineTotal"];


                                     detallesFacturas.Add(new DetallesOPORT
                                     {
                                         DocEntry = DocEntry,
                                         //ItemCode = ItemCode,
                                         Quantity = Quantity,
                                         LineTotal = totalline,
                                         VatGroup = impto,

                                         ItemDescription = ItemDescription
                                     });


                                 }

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
                                                                                           // DateTime Fecha4 = Convert.ToDateTime(ReqDate.ToString()); // "DocDate"
                                 string date1 = Fecha1.ToString("yyyy-MM-dd");
                                 string date2 = Fecha2.ToString("yyyy-MM-dd");

                                 string date3 = Fecha3.ToString("yyyy-MM-dd");
                                 //string date4 = Fecha4.ToString("yyyy-MM-dd");
                                 JsonObject.Add(new JProperty("U_PROYECTO", U_PROYECTO));
                                 JsonObject.Add(new JProperty("U_TipoCompra", U_TipoCompra));
                                 JsonObject.Add(new JProperty("CardCode", CardCode));
                                 JsonObject.Add(new JProperty("U_entryPA", DocEntry));
                                 // JsonObject.Add(new JProperty("RequesterName", ReqName));
                                 JsonObject.Add(new JProperty("DocDate", date1));
                                 JsonObject.Add(new JProperty("DocDueDate", date2));
                                 JsonObject.Add(new JProperty("TaxDate", date3));
                                 // JsonObject.Add(new JProperty("RequriedDate", date4));
                                 //JsonObject.Add(new JProperty("SendNotification", "tNO"));
                                 JsonObject.Add(new JProperty("U_Origen", bd));
                                 JsonObject.Add(new JProperty("DocType", DocType));
                                 JsonObject.Add(new JProperty("DocObjectCode", 22));
                                 JsonObject.Add(new JProperty("U_Adelanto", adel));
                                 JsonObject.Add(new JProperty("U_Enviado", "0"));
                                 JsonObject.Add(new JProperty("U_ENVIOSUC", "Y"));
                                 //JsonObject.Add(new JProperty("AccountCode", "6-1-06-0101"));
                                 JsonObject.Add(new JProperty("U_Porcentaje", porcen));
                                 JsonObject.Add(new JProperty("U_Pago_Procesado", pagopro));
                                 var list = new ArrayList();
                                 if (DocType == "S")
                                 {
                                     foreach (var detalle in grupo.Detalles)
                                     {
                                         Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
                                         JsonObjectRow.Add(new JProperty("ItemDescription", detalle.ItemDescription));
                                         JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
                                         JsonObjectRow.Add(new JProperty("UnitPrice", detalle.LineTotal));
                                         JsonObjectRow.Add(new JProperty("VatGroup", detalle.VatGroup));
                                         JsonObjectRow.Add(new JProperty("AccountCode", "6-1-06-0101"));
                                         //JsonObjectRow.Add(new JProperty("ProjectCode", detalle.PROYECTO));

                                         list.Add(JsonObjectRow);
                                     }
                                 }
                                 //if (DocType == "I")
                                 //{
                                 //    foreach (var detalle in grupo.Detalles)
                                 //    {
                                 //        Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
                                 //        JsonObjectRow.Add(new JProperty("ItemCode", detalle.ItemCode));
                                 //        JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
                                 //        //JsonObjectRow.Add(new JProperty("UnitPrice", detalle.PriceBefDi));
                                 //        JsonObjectRow.Add(new JProperty("VatGroup", detalle.VatGroup));
                                 //        //JsonObjectRow.Add(new JProperty("ProjectCode", detalle.PROYECTO));

                                 //        list.Add(JsonObjectRow);
                                 //    }
                                 //}
                                 JsonObject.Add(new JProperty("DocumentLines", list));
                                 string data = JsonObject.ToString();

                                 var myjs = JsonObject.ToString();
                                 resul = EnviaSapPorBasicAuth("POST", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Drafts", toctoc); //
                                 if (resul == 0) // Caso que inserto 
                                 {
                                     string query = $"UPDATE  \"{bd}\".\"OPOR\" SET \"U_Enviado\" = '1' WHERE \"DocEntry\" =  '" + DocEntry + "'";

                                     // string queryAct = $"UPDATE \"{BRETASV}\".\"OITM\" SET \"U_act\" = '" + vac + "' WHERE \"ItemCode\" =  '" + En_DN + "'"; //Query Actualiza Entry_fac
                                     OdbcCommand cmand = new OdbcCommand(query, conn);
                                     OdbcDataReader dr1 = cmand.ExecuteReader();
                                     string TABLA = "ODRF";//es la tabla destino en el origen donde asociara el anexo
                                     string basetabla = bd;
                                     // moveranexosorigen(bd, idanex, TABLA,ObjSap.DocEntry, DocEntry, basetabla);

                                 }
                                 else
                                 {
                                     objRegistraLog.Graba("Error CrearOCTODASSUCMCH: " + RR + " " + DocEntry + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                                     Inserterror(bd.Substring(6, 6), "OPOR", Convert.ToString(DocEntry));
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
                 // objRegistraLog.Graba("Error al CrearOCTODASSUCMCH: " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
                 Inserterror(bd.Substring(6, 6), "OPOR", Convert.ToString(DocEntry));
             }
             return R;
         }

         public static Respuesta CrearSCworld(string Doctype, string bd, int DocEntry, string ReqName, string U_PROYECTO, string U_TipoCompra, string Comments, string DocDate, string DocDueDate, string TaxDate, string ReqDate, string CODPROYE, string adel, double porcen, string pagopro, int idanex, string toctoc)
         {
             int resul = 0;
             Respuesta R = new Respuesta();
             try
             {

                 //if (conecta_service_layer(Conexion.basecentral, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                 //{
                 //    R.Codigo = -2;
                 //    R.Mensaje = RR;
                 //    objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                 //    return R;
                 //}

                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 {

                     string queryEn = $"SELECT B.\"U_Codigo_Interno\" \"ItemCode\",	 A.\"Quantity\", A.\"VatGroup\" FROM \"{bd}\".\"PRQ1\" A inner join \"{bd}\".\"OITM\" B on A.\"ItemCode\"=B.\"ItemCode\"  where A.\"DocEntry\"='" + DocEntry + "'";

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

                                 JsonObject.Add(new JProperty("U_entrySC", DocEntry));
                                 JsonObject.Add(new JProperty("RequesterName", ReqName));
                                 JsonObject.Add(new JProperty("DocDate", date1));
                                 JsonObject.Add(new JProperty("DocDueDate", date2));
                                 JsonObject.Add(new JProperty("TaxDate", date3));
                                 JsonObject.Add(new JProperty("RequriedDate", date4));
                                 JsonObject.Add(new JProperty("SendNotification", "tNO"));
                                 JsonObject.Add(new JProperty("U_Origen", bd));
                                 JsonObject.Add(new JProperty("DocType", Doctype));

                                 JsonObject.Add(new JProperty("U_Adelanto", adel));

                                 JsonObject.Add(new JProperty("U_Porcentaje", porcen));
                                 JsonObject.Add(new JProperty("U_Pago_Procesado", pagopro));
                                 var list = new ArrayList();
                                 foreach (var detalle in grupo.Detalles)
                                 {
                                     Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
                                     JsonObjectRow.Add(new JProperty("ItemCode", detalle.ItemCode));
                                     JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
                                     //JsonObjectRow.Add(new JProperty("UnitPrice", detalle.PriceBefDi));
                                     JsonObjectRow.Add(new JProperty("VatGroup", detalle.VatGroup));
                                     //JsonObjectRow.Add(new JProperty("ProjectCode", detalle.PROYECTO));                                
                                     list.Add(JsonObjectRow);
                                 }

                                 JsonObject.Add(new JProperty("DocumentLines", list));
                                 string data = JsonObject.ToString();

                                 var myjs = JsonObject.ToString();
                                 resul = EnviaSapPorBasicAuth("POST", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseRequests", toctoc); //
                                 if (resul == 0) // Caso que inserto 
                                 {
                                     string query = $"UPDATE  \"{bd}\".\"OPRQ\" SET \"U_Enviado\" = '1' WHERE \"DocEntry\" =  '" + DocEntry + "'";

                                     // string queryAct = $"UPDATE \"{BRETASV}\".\"OITM\" SET \"U_act\" = '" + vac + "' WHERE \"ItemCode\" =  '" + En_DN + "'"; //Query Actualiza Entry_fac
                                     OdbcCommand cmand = new OdbcCommand(query, conn);
                                     OdbcDataReader dr1 = cmand.ExecuteReader();
                                     string TABLA = "OPRQ";//es la tabla destino en el origen donde asociara el anexo
                                     string basetabla = bd;
                                     // moveranexosdestinoWL(bd, idanex, TABLA, ObjSap.DocEntry,DocEntry, basetabla);

                                 }
                                 else
                                 {
                                     objRegistraLog.Graba("Error CrearSCworld: " + RR + " " + DocEntry + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
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
         public static Respuesta CrearOfertaTODAS(string bd, int DocEntry, string CardCode, string U_PROYECTO, string U_TipoCompra, string Comments, string DocDate, string DocDueDate, string TaxDate, string ReqDate, string CODPROYE, string adel, double porcen, string pagopro, int idanex, string toctoc)
         {
             int resul = 0;
             Respuesta R = new Respuesta();
             try
             {

                 //if (conecta_service_layer(bd, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                 //{
                 //    R.Codigo = -2;
                 //    R.Mensaje = RR;
                 //    objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                 //    return R;
                 //}
                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 {
                     string queryEn = $"SELECT \"ItemCode\",	 \"Quantity\", \"VatGroup\",\"LineTotal\" FROM \"{Conexion.BD_Hechizo}\".\"PQT1\" where \"DocEntry\"='" + DocEntry + "'";

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
                                 decimal totalline = (decimal)dr["LineTotal"];

                                 detallesFacturas.Add(new DetallesOPORT
                                 {
                                     DocEntry = DocEntry,
                                     ItemCode = ItemCode,
                                     Quantity = Quantity,
                                     // PriceBefDi = PriceBefDi,
                                     VatGroup = impto,
                                     LineTotal = totalline

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

                                 JsonObject.Add(new JProperty("CardCode", CardCode));
                                 JsonObject.Add(new JProperty("DocDate", date1));
                                 JsonObject.Add(new JProperty("DocDueDate", date2));
                                 JsonObject.Add(new JProperty("TaxDate", date3));
                                 JsonObject.Add(new JProperty("RequriedDate", date4));
                                 JsonObject.Add(new JProperty("U_Origen", bd));
                                 JsonObject.Add(new JProperty("U_entryPA", DocEntry));//con este se quien es el que acptaron

                                 JsonObject.Add(new JProperty("U_Adelanto", adel));

                                 JsonObject.Add(new JProperty("U_Porcentaje", porcen));
                                 JsonObject.Add(new JProperty("U_Pago_Procesado", pagopro));

                                 var list = new ArrayList();
                                 foreach (var detalle in grupo.Detalles)
                                 {
                                     Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
                                     JsonObjectRow.Add(new JProperty("ItemCode", detalle.ItemCode));
                                     JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
                                     JsonObjectRow.Add(new JProperty("LineTotal", detalle.LineTotal));
                                     JsonObjectRow.Add(new JProperty("VatGroup", detalle.VatGroup));
                                     JsonObjectRow.Add(new JProperty("ProjectCode", CODPROYE));

                                     list.Add(JsonObjectRow);
                                 }

                                 JsonObject.Add(new JProperty("DocumentLines", list));
                                 string data = JsonObject.ToString();

                                 var myjs = JsonObject.ToString();
                                 resul = EnviaSapPorBasicAuth("POST", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseQuotations", toctoc); //
                                 if (resul == 0) // Caso que inserto 
                                 {
                                     string query = $"UPDATE  \"{Conexion.BD_Hechizo}\".\"OPQT\" SET \"U_Enviado\" = '1' WHERE \"DocEntry\" =  '" + DocEntry + "'";

                                     // string queryAct = $"UPDATE \"{BRETASV}\".\"OITM\" SET \"U_act\" = '" + vac + "' WHERE \"ItemCode\" =  '" + En_DN + "'"; //Query Actualiza Entry_fac
                                     OdbcCommand cmand = new OdbcCommand(query, conn);
                                     OdbcDataReader dr1 = cmand.ExecuteReader();
                                     string TABLA = "OPQT";//es la tabla destino en el origen donde asociara el anexo
                                     string basetabla = Conexion.BD_Hechizo;
                                     //  moveranexosdestino(bd, idanex, TABLA, ObjSap.DocEntry,DocEntry,basetabla);
                                 }
                                 else
                                 {
                                     objRegistraLog.Graba("Error CrearOfertaTODAS: " + RR + " " + DocEntry + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                                     Inserterror(bd.Substring(6, 6), "OPQTsuc", Convert.ToString(DocEntry));
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
         public static Respuesta CrearOC_MCH(string DocType, string bd, int DocEntry, string CardCode, string U_PROYECTO, string U_TipoCompra, string Comments, string DocDate, string DocDueDate, string TaxDate, string CODPROYE, string adel, double porcen, string pagopro, int idanex, string toctoc)
         {
             int resul = 0;
             Respuesta R = new Respuesta();
             try
             {

                 //if (conecta_service_layer(bd, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                 //{
                 //    R.Codigo = -2;
                 //    R.Mensaje = RR;
                 //    objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                 //    return R;
                 //}
                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 {

                     string queryEn = $"SELECT \"ItemCode\",	 A.\"Quantity\", A.\"VatGroup\",A.\"LineTotal\",A.\"Dscription\" FROM \"{Conexion.BD_Hechizo}\".\"POR1\" A  where A.\"DocEntry\"='" + DocEntry + "'";

                     OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                     conn.Open();
                     using (OdbcDataReader dr = CmD.ExecuteReader())
                     {
                         if (dr.HasRows)
                         {
                             List<DetallesOPORT> detallesFacturas = new List<DetallesOPORT>();
                             if (DocType == "I")
                             {
                                 while (dr.Read())
                                 {
                                     string ItemCode = (string)dr["ItemCode"];
                                     decimal Quantity = (decimal)dr["Quantity"];
                                     string impto = (string)dr["VatGroup"];
                                     string codprj = CODPROYE;
                                     decimal LineTotal = (decimal)dr["LineTotal"];

                                     detallesFacturas.Add(new DetallesOPORT
                                     {
                                         DocEntry = DocEntry,
                                         ItemCode = ItemCode,
                                         Quantity = Quantity,
                                         // PriceBefDi = PriceBefDi,
                                         LineTotal = LineTotal,
                                         VatGroup = impto,
                                         PROYECTO = codprj


                                     });
                                 }
                             }
                             //if (DocType == "S")
                             //{
                             //    while (dr.Read())
                             //    {
                             //        string ItemDescription = (string)dr["Dscription"];
                             //        decimal Quantity = (decimal)dr["Quantity"];
                             //        string impto = (string)dr["VatGroup"];
                             //        string codprj = CODPROYE;


                             //        detallesFacturas.Add(new DetallesOPORT
                             //        {
                             //            DocEntry = DocEntry,
                             //            //ItemCode = ItemCode,
                             //            Quantity = Quantity,
                             //            // PriceBefDi = PriceBefDi,

                             //            VatGroup = impto,
                             //            PROYECTO = codprj,
                             //            ItemDescription = ItemDescription
                             //        });


                             //    }

                             //}

                             //while (dr.Read())
                             //{
                             //    string ItemCode = (string)dr["ItemCode"];
                             //    decimal Quantity = (decimal)dr["Quantity"];
                             //    string impto = (string)dr["VatGroup"];
                             //    decimal totalline = (decimal)dr["LineTotal"];

                             //    detallesFacturas.Add(new DetallesOPORT
                             //    {
                             //        DocEntry = DocEntry,
                             //        ItemCode = ItemCode,
                             //        Quantity = Quantity,
                             //        // PriceBefDi = PriceBefDi,
                             //        VatGroup = impto,
                             //        LineTotal = totalline

                             //    });
                             //}
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
                                                                                           // DateTime Fecha4 = Convert.ToDateTime(ReqDate.ToString()); // "DocDate"
                                 string date1 = Fecha1.ToString("yyyy-MM-dd");
                                 string date2 = Fecha2.ToString("yyyy-MM-dd");

                                 string date3 = Fecha3.ToString("yyyy-MM-dd");
                                 // string date4 = Fecha4.ToString("yyyy-MM-dd");
                                 JsonObject.Add(new JProperty("U_PROYECTO", U_PROYECTO));
                                 JsonObject.Add(new JProperty("U_TipoCompra", U_TipoCompra));

                                 JsonObject.Add(new JProperty("CardCode", CardCode));
                                 JsonObject.Add(new JProperty("DocDate", DateTime.Today));
                                 JsonObject.Add(new JProperty("DocDueDate", DateTime.Today));
                                 JsonObject.Add(new JProperty("TaxDate", DateTime.Today));
                                 JsonObject.Add(new JProperty("U_Origen", bd));
                                 JsonObject.Add(new JProperty("U_Aprobado", "Y"));
                                 //JsonObject.Add(new JProperty("RequriedDate", date4));
                                 JsonObject.Add(new JProperty("U_entryPA", DocEntry));//con este se quien es el que acptaron

                                 JsonObject.Add(new JProperty("U_Adelanto", adel));
                                 //JsonObject.Add(new JProperty("U_TipoCompra", tipcom));
                                 JsonObject.Add(new JProperty("U_Porcentaje", porcen));
                                 JsonObject.Add(new JProperty("U_Pago_Procesado", pagopro));
                                 if (DocType == "S")
                                 {

                                 }

                                 var list = new ArrayList();
                                 foreach (var detalle in grupo.Detalles)
                                 {
                                     Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
                                     JsonObjectRow.Add(new JProperty("ItemCode", detalle.ItemCode));
                                     JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
                                     JsonObjectRow.Add(new JProperty("LineTotal", detalle.LineTotal));
                                     JsonObjectRow.Add(new JProperty("VatGroup", detalle.VatGroup));
                                     JsonObjectRow.Add(new JProperty("ProjectCode", CODPROYE));
                                     list.Add(JsonObjectRow);
                                 }

                                 JsonObject.Add(new JProperty("DocumentLines", list));
                                 string data = JsonObject.ToString();

                                 var myjs = JsonObject.ToString();
                                 resul = EnviaSapPorBasicAuth("POST", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseOrders", toctoc); //
                                 if (resul == 0) // Caso que inserto 
                                 {
                                     // aca se actualiza alreves a asolicitud de omar porque cuando se copia de oferta a pedido arrastra el enviado en 1 entonces 0 significa que ya se envio 
                                     string query = $"UPDATE  \"{Conexion.BD_Hechizo}\".\"OPOR\" SET \"U_Enviado\" = '1' WHERE \"DocEntry\" =  '" + DocEntry + "'";

                                     // string queryAct = $"UPDATE \"{BRETASV}\".\"OITM\" SET \"U_act\" = '" + vac + "' WHERE \"ItemCode\" =  '" + En_DN + "'"; //Query Actualiza Entry_fac
                                     OdbcCommand cmand = new OdbcCommand(query, conn);
                                     OdbcDataReader dr1 = cmand.ExecuteReader();
                                     string TABLA = "OPOR";//es la tabla destino en el origen donde asociara el anexo
                                     string basetabla = Conexion.BD_Hechizo;
                                     //moveranexosdestino(bd, idanex, TABLA, ObjSap.DocEntry,DocEntry, basetabla);
                                 }
                                 else
                                 {
                                     objRegistraLog.Graba("Error CrearOC: " + RR + " " + DocEntry + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                                     Inserterror(bd.Substring(6, 6), "OPOR", Convert.ToString(DocEntry));
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
         public static Respuesta CrearOVTA(string bd, int DocEntry, string CardCode, string U_PROYECTO, string U_TipoCompra, string Comments, string DocDate, string DocDueDate, string TaxDate, string CODPROYE, string adel, double porcen, string pagopro, string toctoc)
         {
             int resul = 0;
             Respuesta R = new Respuesta();
             try
             {

                 //if (conecta_service_layer(Conexion.basecentral, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                 //{
                 //    R.Codigo = -2;
                 //    R.Mensaje = RR;
                 //    objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                 //    return R;
                 //}
                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 {
                     string queryEn = $"SELECT  \"ItemCode\",	 A.\"Quantity\", A.\"VatGroup\",A.\"LineTotal\" FROM \"{Conexion.BD_central}\".\"POR1\" A  where A.\"DocEntry\"='" + DocEntry + "'";

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
                                 decimal totalline = (decimal)dr["LineTotal"];

                                 detallesFacturas.Add(new DetallesOPORT
                                 {
                                     DocEntry = DocEntry,
                                     ItemCode = ItemCode,
                                     Quantity = Quantity,
                                     // PriceBefDi = PriceBefDi,
                                     VatGroup = impto,
                                     LineTotal = totalline

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
                                 //DateTime Fecha4 = Convert.ToDateTime(ReqDate.ToString()); // "DocDate"
                                 string date1 = Fecha1.ToString("yyyy-MM-dd");
                                 string date2 = Fecha2.ToString("yyyy-MM-dd");

                                 string date3 = Fecha3.ToString("yyyy-MM-dd");
                                 ///string date4 = Fecha4.ToString("yyyy-MM-dd");
                                 JsonObject.Add(new JProperty("U_PROYECTO", U_PROYECTO));
                                 JsonObject.Add(new JProperty("U_TipoCompra", U_TipoCompra));

                                 // JsonObject.Add(new JProperty("CardCode", "CL00006"));
                                 JsonObject.Add(new JProperty("CardCode", CardCode));
                                 JsonObject.Add(new JProperty("DocDate", date1));
                                 JsonObject.Add(new JProperty("DocDueDate", date2));
                                 JsonObject.Add(new JProperty("TaxDate", date3));
                                 JsonObject.Add(new JProperty("U_Origen", bd));
                                 JsonObject.Add(new JProperty("U_Adelanto", adel));

                                 JsonObject.Add(new JProperty("U_Porcentaje", porcen));
                                 JsonObject.Add(new JProperty("U_Pago_Procesado", pagopro));
                                 //JsonObject.Add(new JProperty("RequriedDate", date4));
                                 //JsonObject.Add(new JProperty("U_entryPA", DocEntry));//con este se quien es el que acptaron
                                 var list = new ArrayList();
                                 foreach (var detalle in grupo.Detalles)
                                 {
                                     Newtonsoft.Json.Linq.JObject JsonObjectRow = new Newtonsoft.Json.Linq.JObject();
                                     JsonObjectRow.Add(new JProperty("ItemCode", detalle.ItemCode));
                                     JsonObjectRow.Add(new JProperty("Quantity", detalle.Quantity));
                                     JsonObjectRow.Add(new JProperty("LineTotal", detalle.LineTotal));
                                     JsonObjectRow.Add(new JProperty("VatGroup", "V1"));
                                     JsonObjectRow.Add(new JProperty("ProjectCode", CODPROYE));

                                     list.Add(JsonObjectRow);
                                 }

                                 JsonObject.Add(new JProperty("DocumentLines", list));
                                 string data = JsonObject.ToString();

                                 var myjs = JsonObject.ToString();
                                 resul = EnviaSapPorBasicAuth("POST", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Orders", toctoc); //
                                 if (resul == 0) // Caso que inserto 
                                 {
                                     string query = $"UPDATE  \"{Conexion.BD_central}\".\"OPOR\" SET \"U_Enviado\" = '1' WHERE \"DocEntry\" =  '" + DocEntry + "'";

                                     // string queryAct = $"UPDATE \"{BRETASV}\".\"OITM\" SET \"U_act\" = '" + vac + "' WHERE \"ItemCode\" =  '" + En_DN + "'"; //Query Actualiza Entry_fac
                                     OdbcCommand cmand = new OdbcCommand(query, conn);
                                     OdbcDataReader dr1 = cmand.ExecuteReader();
                                 }
                                 else
                                 {
                                     objRegistraLog.Graba("Error CrearOVTA: " + RR + " " + DocEntry + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                                     Inserterror(bd.Substring(6, 6), "ORDR", Convert.ToString(DocEntry));
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

         public static Respuesta actualizaPAMCH(string bd, int DocEntry, string Entry, string EntrySC, string toctoc)
         {
             int resul = 0;
             int entrydoc = 0;
             Respuesta R = new Respuesta();
             try
             {
                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 {

                     //  if (conecta_service_layer(Conexion.ORIGEN, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                     //{
                     //    R.Codigo = -2;
                     //    R.Mensaje = RR;
                     //    objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                     //    return R;
                     //}
                     //cerrar todos los doc diferentes al que aceptaron de ese empresa
                     Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();

                     string queryEn = $"SELECT top 1 \"DocEntry\" FROM \"{Conexion.BD_Hechizo}\".\"OPQT\" where \"U_Origen\"= '{bd}' and \"DocEntry\"!='" + Entry + "'and \"DocStatus\"='O'  and \"U_entrySC\"='" + EntrySC + "' ";

                     OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion+
                     conn.Open();
                     using (OdbcDataReader dr = CmD.ExecuteReader())
                     {
                         if (dr.HasRows)
                         {
                             while (dr.Read())
                             {
                                 entrydoc = Convert.ToInt32(dr["DocEntry"]);

                                 var myjs = JsonObject.ToString();
                                 resul = EnviaSapPorBasicAuth("POST", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseQuotations(" + entrydoc + ")/Close", toctoc);
                                 if (resul == 0) // Caso que inserto 
                                 {
                                     //string query = $"UPDATE  \"{Conexion.ORIGEN}\".\"OPQT\" SET \"U_Enviado\" = '1' WHERE \"DocEntry\" =  '" + DocEntry + "'";
                                     //conn.Open();
                                     //// string queryAct = $"UPDATE \"{BRETASV}\".\"OITM\" SET \"U_act\" = '" + vac + "' WHERE \"ItemCode\" =  '" + En_DN + "'"; //Query Actualiza Entry_fac
                                     //OdbcCommand cmand = new OdbcCommand(query, conn);
                                     //OdbcDataReader dr1 = cmand.ExecuteReader();
                                     //conn.Close();
                                 }
                                 else
                                 {
                                     objRegistraLog.Graba("Error actualizaPAMCH: " + RR + " " + DocEntry + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                                     Inserterror(bd.Substring(6, 6), "OPRQact", Convert.ToString(DocEntry));
                                 }
                             }


                         }

                         LiberaMemoria.FlushMemory();
                     }
                 }

             }
             catch (Exception ex)
             {
                 R.Codigo = -1;
                 R.Mensaje = "Alerta: " + ex.Message;
                 objRegistraLog.Graba("Error CrearOVTA: " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
             }
             return R;
         }
         public static Respuesta actualizaOC_MCH(string bd, int DocEntry, string Entry, string EntrySC, string toctoc)
         {
             int resul = 0;
             int entrydoc = 0;
             Respuesta R = new Respuesta();
             try
             {
                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 {
                     //if (conecta_service_layer(Conexion.ORIGEN, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                     //{
                     //    R.Codigo = -2;
                     //    R.Mensaje = RR;
                     //    objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                     //    return R;
                     //}
                     //cerrar todos los doc diferentes al que aceptaron de ese empresa
                     Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                     string queryEn = $"SELECT \"DocEntry\" FROM \"{Conexion.BD_Hechizo}\".\"OPOR\" where   \"U_Origen\"='{bd}' and \"DocEntry\"='" + Entry + "'  and \"DocStatus\"='O'";

                     OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                     conn.Open();
                     using (OdbcDataReader dr = CmD.ExecuteReader())
                     {
                         if (dr.HasRows)
                         {
                             while (dr.Read())
                             {
                                 entrydoc = Convert.ToInt32(dr["DocEntry"]);


                                 var myjs = JsonObject.ToString();
                                 resul = EnviaSapPorBasicAuth("POST", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseOrders(" + entrydoc + ")/Close", toctoc);
                                 if (resul == 0) // Caso que inserto 
                                 {
                                     //string query = $"UPDATE  \"{Conexion.ORIGEN}\".\"OPQT\" SET \"U_Enviado\" = '1' WHERE \"DocEntry\" =  '" + DocEntry + "'";
                                     //conn.Open();
                                     //// string queryAct = $"UPDATE \"{BRETASV}\".\"OITM\" SET \"U_act\" = '" + vac + "' WHERE \"ItemCode\" =  '" + En_DN + "'"; //Query Actualiza Entry_fac
                                     //OdbcCommand cmand = new OdbcCommand(query, conn);
                                     //OdbcDataReader dr1 = cmand.ExecuteReader();
                                     //conn.Close();
                                 }
                                 else
                                 {
                                     objRegistraLog.Graba("Error actualizaOC_MCH: " + RR + " " + DocEntry + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                                     Inserterror(bd.Substring(6, 6), "OPRQact", Convert.ToString(DocEntry));
                                 }
                             }

                         }

                         LiberaMemoria.FlushMemory();
                     }
                 }

             }
             catch (Exception ex)
             {
                 R.Codigo = -1;
                 R.Mensaje = "Alerta: " + ex.Message;
                 objRegistraLog.Graba("Error CrearOVTA: " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
             }
             return R;
         }

         public static Respuesta STATUSSucursal_MCH(string bd, string toctoc)
         {
             int resul = 0;
             int entrydoc = 0;
             int entpa;
             Respuesta R = new Respuesta();
             try
             {
                 using (OdbcConnection conn = new OdbcConnection(Conexion.strCon))
                 using (OdbcConnection conn2 = new OdbcConnection(Conexion.strCon))
                 {
                     //if (conecta_service_layer(Conexion.ORIGEN, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                     //{
                     //    R.Codigo = -2;
                     //    R.Mensaje = RR;
                     //    objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                     //    return R;
                     //}
                     //cerrar el aprobado en mch y actualizo el aprobado en la sucursal .. 
                     Newtonsoft.Json.Linq.JObject JsonObject = new Newtonsoft.Json.Linq.JObject();
                     string queryEn = $"SELECT \"DocEntry\",\"U_entryPA\" FROM \"{Conexion.BD_Hechizo}\".\"OPOR\" where   \"U_Origen\"='{bd}' and \"DocType\"='S'   and \"DocStatus\"='O' and ifnull(\"U_entryPA\",0) <> 0 and \"WddStatus\"='P' and \"U_Enviado\" = '1'";

                     OdbcCommand CmD = new OdbcCommand(queryEn, conn); // llama al Query Encabezado con la conexion
                     conn.Open();
                     using (OdbcDataReader dr = CmD.ExecuteReader())
                     {
                         if (dr.HasRows)
                         {
                             while (dr.Read())
                             {
                                 entrydoc = Convert.ToInt32(dr["DocEntry"]);
                                 entpa = Convert.ToInt32(dr["U_entryPA"]);
                                 var myjs = JsonObject.ToString();
                                 resul = EnviaSapPorBasicAuth("POST", JsonObject, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseOrders(" + entrydoc + ")/Close", toctoc);
                                 if (resul == 0) // Caso que inserto 
                                 {
                                     string query = $"UPDATE  \"{bd}\".\"OPOR\" SET \"U_Aprobado\" = 'Y',\"U_Enviado\" = '1' WHERE \"DocEntry\" =  " + entpa + "";
                                     conn2.Open();
                                     OdbcCommand cmand = new OdbcCommand(query, conn2);
                                     OdbcDataReader dr1 = cmand.ExecuteReader();
                                     conn2.Close();
                                 }
                                 else
                                 {
                                     objRegistraLog.Graba("Error STATUSSucursal_MCH: " + RR + " " + entrydoc + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                                     Inserterror(bd.Substring(6, 6), "ODRFsuc", Convert.ToString(entrydoc));
                                 }
                             }

                         }

                         LiberaMemoria.FlushMemory();
                     }
                     conn.Close();
                     //cerrar los rechazados en origen EN ODRF y en la sucursal
                     //if (conecta_service_layer(bd, "fcscr-cloud\\consult07", "11111111", "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/Login") != 0) //"10032_SOL_ARA" //"10030_STELLINA_TEST"
                     //{
                     //    R.Codigo = -2;
                     //    R.Mensaje = RR;
                     //    objRegistraLog.Graba("Sin conectar a Service Layer : " + R.Mensaje + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error                   
                     //    return R;
                     //}
                     Newtonsoft.Json.Linq.JObject JsonObject2 = new Newtonsoft.Json.Linq.JObject();
                     queryEn = $"SELECT \"DocEntry\",\"U_entryPA\" FROM \"{Conexion.BD_Hechizo}\".\"ODRF\"  where   \"U_Origen\"='{bd}' and \"DocType\"='S'   and \"ObjType\"='22' and \"WddStatus\" ='N'  AND \"U_Enviado\" = '0'";

                     OdbcCommand CmD2 = new OdbcCommand(queryEn, conn2); // llama al Query Encabezado con la conexion
                     conn2.Open();
                     using (OdbcDataReader dr2 = CmD2.ExecuteReader())
                     {
                         if (dr2.HasRows)
                         {
                             while (dr2.Read())
                             {
                                 entrydoc = Convert.ToInt32(dr2["DocEntry"]);
                                 entpa = Convert.ToInt32(dr2["U_entryPA"]);
                                 queryEn = $"UPDATE  \"{bd}\".\"OPOR\" SET \"U_Aprobado\"='N',\"U_Enviado\" = '1' where    \"DocEntry\"=" + entpa + "";
                                 conn.Open();
                                 OdbcCommand cmand = new OdbcCommand(queryEn, conn);
                                 OdbcDataReader dr1 = cmand.ExecuteReader();
                                 conn.Close();
                                 var myjs = JsonObject2.ToString();
                                 resul = EnviaSapPorBasicAuth("POST", JsonObject2, "https://DESBD-CCC.fcscr-cloud.net:50015/b1s/v1/PurchaseOrders(" + entpa + ")/Close", toctoc);
                                 if (resul == 0) // Caso que inserto 
                                 {

                                 }
                                 else
                                 {
                                     objRegistraLog.Graba("Error STATUSSucursal_MCH: " + RR + " " + entrydoc + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// 
                                     Inserterror(bd.Substring(6, 6), "OPORsuc", Convert.ToString(entrydoc));
                                 }
                             }

                         }

                         LiberaMemoria.FlushMemory();
                     }
                     conn2.Close();
                 }

             }
             catch (Exception ex)
             {
                 R.Codigo = -1;
                 R.Mensaje = "Alerta: " + ex.Message;
                 objRegistraLog.Graba("Error CrearOVTA: " + ex.Message + "/ " + "Hora: " + DateTime.Now.ToString("HH:mm:ss tt"));// grabar el error
             }
             return R;
         }

         */

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
        public List<SapDocumentLine> DocumentLines { get; set; }
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

    public class SapDocumentLine
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public decimal UnitPrice { get; set; }
    }

}
