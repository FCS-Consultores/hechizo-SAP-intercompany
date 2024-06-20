using Intercompany.Data;
using Intercompany.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intercompany
{
    class Program
    {
        static void Main(string[] args)
        {
            Intercompany.tools.OcultarConsola.DisappearConsole();
            //RegistroLog objRegistraLog = new RegistroLog();
            //#region fabricanteGEN
            //Intercompany.Data.Intercompany.Fabricante(Conexion.baseamd);
            //Intercompany.Data.Intercompany.Fabricante(Conexion.baseboquete);
            //Intercompany.Data.Intercompany.Fabricante(Conexion.basecalypso);
            //Intercompany.Data.Intercompany.Fabricante(Conexion.basecanaima);
            //Intercompany.Data.Intercompany.Fabricante(Conexion.baseconstru);
            //Intercompany.Data.Intercompany.Fabricante(Conexion.baseeco);
            //Intercompany.Data.Intercompany.Fabricante(Conexion.basefarmar);
            //Intercompany.Data.Intercompany.Fabricante(Conexion.baseideo);
            //Intercompany.Data.Intercompany.Fabricante(Conexion.baseiverco);
            //Intercompany.Data.Intercompany.Fabricante(Conexion.basepanco);
            //Intercompany.Data.Intercompany.Fabricante(Conexion.basepgjp);
            //Intercompany.Data.Intercompany.Fabricante(Conexion.baseworld);
            //Intercompany.Data.Intercompany.Fabricante(Conexion.basezhej);
            //#endregion


            //#region GRUPOGEN
            //Intercompany.Data.Intercompany.Grupo(Conexion.baseamd);
            //Intercompany.Data.Intercompany.Grupo(Conexion.baseboquete);
            //Intercompany.Data.Intercompany.Grupo(Conexion.basecalypso);
            //Intercompany.Data.Intercompany.Grupo(Conexion.basecanaima);
            //Intercompany.Data.Intercompany.Grupo(Conexion.baseconstru);
            //Intercompany.Data.Intercompany.Grupo(Conexion.baseeco);
            //Intercompany.Data.Intercompany.Grupo(Conexion.basefarmar);
            //Intercompany.Data.Intercompany.Grupo(Conexion.baseideo);
            //Intercompany.Data.Intercompany.Grupo(Conexion.baseiverco);
            //Intercompany.Data.Intercompany.Grupo(Conexion.basepanco);
            //Intercompany.Data.Intercompany.Grupo(Conexion.basepgjp);
            //Intercompany.Data.Intercompany.Grupo(Conexion.baseworld);
            //Intercompany.Data.Intercompany.Grupo(Conexion.basezhej);
            //#endregion

            //#region ADUANAGEN
            //Intercompany.Data.Intercompany.ADUANA(Conexion.baseamd);
            //Intercompany.Data.Intercompany.ADUANA(Conexion.baseboquete);
            //Intercompany.Data.Intercompany.ADUANA(Conexion.basecalypso);
            //Intercompany.Data.Intercompany.ADUANA(Conexion.basecanaima);
            //Intercompany.Data.Intercompany.ADUANA(Conexion.baseconstru);
            //Intercompany.Data.Intercompany.ADUANA(Conexion.baseeco);
            //Intercompany.Data.Intercompany.ADUANA(Conexion.basefarmar);
            //Intercompany.Data.Intercompany.ADUANA(Conexion.baseideo);
            //Intercompany.Data.Intercompany.ADUANA(Conexion.baseiverco);
            //Intercompany.Data.Intercompany.ADUANA(Conexion.basepanco);
            //Intercompany.Data.Intercompany.ADUANA(Conexion.basepgjp);
            //Intercompany.Data.Intercompany.ADUANA(Conexion.baseworld);
            //Intercompany.Data.Intercompany.ADUANA(Conexion.basezhej);
            //#endregion

            //#region articuloGEN
            //Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.baseamd);
            //Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.baseboquete);
            //Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.basecalypso);
            //Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.basecanaima);
            //Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.baseconstru);
            //Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.baseeco);
            //Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.basefarmar);
            //Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.baseideo);
            //Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.baseiverco);
            //Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.basepanco);
            //Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.basepgjp);
            //Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.baseworld);
            //Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.basezhej);
            //#endregion

            #region SCSucursalGEN
            //Intercompany.Data.Intercompany.SCSucursal(Conexion.baseamd);
            //Intercompany.Data.Intercompany.SCSucursal(Conexion.baseboquete);
            //Intercompany.Data.Intercompany.SCSucursal(Conexion.basecalypso);
            //Intercompany.Data.Intercompany.SCSucursal(Conexion.basecanaima);
            //Intercompany.Data.Intercompany.SCSucursal(Conexion.baseconstru);
            //Intercompany.Data.Intercompany.SCSucursal(Conexion.baseeco);
            //Intercompany.Data.Intercompany.SCSucursal(Conexion.basefarmar);
            //Intercompany.Data.Intercompany.SCSucursal(Conexion.baseideo);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.baseiverco);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.basepanco);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.basepgjp);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.baseworld);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.basezhej);
            #endregion

            //inter.ActualizarArticulosDT();


            //InsServi_Ord_Vnt.convierteOVTA_OC();

            //se cambia el metodo de arriba por el metodo de abajo segun solicitud de minuta el 09/04
            //InsServi_Ord_Vnt.NuevaOporautDC();

            //InsServi_Ord_Vnt.NuevaOporautCD();
            //inter.NuevaOrdenvtaTD();
            //inter.NuevaOrdenvtaDT();


            //objRegistraLog.Graba("Prueba log No miercoles");

        }
    }
}
