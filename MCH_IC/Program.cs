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
            RegistroLog objRegistraLog = new RegistroLog();
            Intercompany.Data.Intercompany.SINCROERROR();
            //#region fabricanteGEN
            Intercompany.Data.Intercompany.Fabricante(Conexion.baseamd);
            Intercompany.Data.Intercompany.Fabricante(Conexion.baseboquete);
            Intercompany.Data.Intercompany.Fabricante(Conexion.basecalypso);
            Intercompany.Data.Intercompany.Fabricante(Conexion.basecanaima);
            Intercompany.Data.Intercompany.Fabricante(Conexion.baseconstru);
            Intercompany.Data.Intercompany.Fabricante(Conexion.baseeco);
            Intercompany.Data.Intercompany.Fabricante(Conexion.basefarmar);
            Intercompany.Data.Intercompany.Fabricante(Conexion.baseideo);
            Intercompany.Data.Intercompany.Fabricante(Conexion.baseiverco);
            Intercompany.Data.Intercompany.Fabricante(Conexion.basepanco);
            Intercompany.Data.Intercompany.Fabricante(Conexion.basepgjp);
            Intercompany.Data.Intercompany.Fabricante(Conexion.basetorino);
            Intercompany.Data.Intercompany.Fabricante(Conexion.baseworld);
            Intercompany.Data.Intercompany.Fabricante(Conexion.basezhej);
            //#endregion


            //#region GRUPOGEN
            Intercompany.Data.Intercompany.Grupo(Conexion.baseamd);
            Intercompany.Data.Intercompany.Grupo(Conexion.baseboquete);
            Intercompany.Data.Intercompany.Grupo(Conexion.basecalypso);
            Intercompany.Data.Intercompany.Grupo(Conexion.basecanaima);
            Intercompany.Data.Intercompany.Grupo(Conexion.baseconstru);
            Intercompany.Data.Intercompany.Grupo(Conexion.baseeco);
            Intercompany.Data.Intercompany.Grupo(Conexion.basefarmar);
            Intercompany.Data.Intercompany.Grupo(Conexion.baseideo);
            Intercompany.Data.Intercompany.Grupo(Conexion.baseiverco);
            Intercompany.Data.Intercompany.Grupo(Conexion.basepanco);
            Intercompany.Data.Intercompany.Grupo(Conexion.basepgjp);
            Intercompany.Data.Intercompany.Grupo(Conexion.basetorino);
            Intercompany.Data.Intercompany.Grupo(Conexion.baseworld);
            Intercompany.Data.Intercompany.Grupo(Conexion.basezhej);
            //#endregion

            //#region GRUPOSNGEN
            Intercompany.Data.Intercompany.GrupoSN(Conexion.baseamd);
            Intercompany.Data.Intercompany.GrupoSN(Conexion.baseboquete);
            Intercompany.Data.Intercompany.GrupoSN(Conexion.basecalypso);
            Intercompany.Data.Intercompany.GrupoSN(Conexion.basecanaima);
            Intercompany.Data.Intercompany.GrupoSN(Conexion.baseconstru);
            Intercompany.Data.Intercompany.GrupoSN(Conexion.baseeco);
            Intercompany.Data.Intercompany.GrupoSN(Conexion.basefarmar);
            Intercompany.Data.Intercompany.GrupoSN(Conexion.baseideo);
            Intercompany.Data.Intercompany.GrupoSN(Conexion.baseiverco);
            Intercompany.Data.Intercompany.GrupoSN(Conexion.basepanco);
            Intercompany.Data.Intercompany.GrupoSN(Conexion.basetorino);

            Intercompany.Data.Intercompany.GrupoSN(Conexion.basepgjp);
            Intercompany.Data.Intercompany.GrupoSN(Conexion.baseworld);
            Intercompany.Data.Intercompany.GrupoSN(Conexion.basezhej);
            //#endregion





            //#region CondicionesGEN
            Intercompany.Data.Intercompany.condicionespago(Conexion.baseamd);
            Intercompany.Data.Intercompany.condicionespago(Conexion.baseboquete);
            Intercompany.Data.Intercompany.condicionespago(Conexion.basecalypso);
            Intercompany.Data.Intercompany.condicionespago(Conexion.basecanaima);
            Intercompany.Data.Intercompany.condicionespago(Conexion.baseconstru);
            Intercompany.Data.Intercompany.condicionespago(Conexion.baseeco);
            Intercompany.Data.Intercompany.condicionespago(Conexion.basefarmar);
            Intercompany.Data.Intercompany.condicionespago(Conexion.baseideo);
            Intercompany.Data.Intercompany.condicionespago(Conexion.baseiverco);
            Intercompany.Data.Intercompany.condicionespago(Conexion.basepanco);
            Intercompany.Data.Intercompany.condicionespago(Conexion.basepgjp);
            Intercompany.Data.Intercompany.condicionespago(Conexion.basetorino);
            Intercompany.Data.Intercompany.condicionespago(Conexion.baseworld);
            Intercompany.Data.Intercompany.condicionespago(Conexion.basezhej);
            //#endregion

            //////////////////////#region ADUANAGEN
            ////////////////////Intercompany.Data.Intercompany.ADUANA(Conexion.baseamd);
            ////////////////////Intercompany.Data.Intercompany.ADUANA(Conexion.baseboquete);
            ////////////////////Intercompany.Data.Intercompany.ADUANA(Conexion.basecalypso);
            ////////////////////Intercompany.Data.Intercompany.ADUANA(Conexion.basecanaima);
            ////////////////////Intercompany.Data.Intercompany.ADUANA(Conexion.baseconstru);
            ////////////////////Intercompany.Data.Intercompany.ADUANA(Conexion.baseeco);
            ////////////////////Intercompany.Data.Intercompany.ADUANA(Conexion.basefarmar);
            ////////////////////Intercompany.Data.Intercompany.ADUANA(Conexion.baseideo);
            ////////////////////Intercompany.Data.Intercompany.ADUANA(Conexion.baseiverco);
            ////////////////////Intercompany.Data.Intercompany.ADUANA(Conexion.basepanco);
            ////////////////////Intercompany.Data.Intercompany.ADUANA(Conexion.basetorino);
            ////////////////////Intercompany.Data.Intercompany.ADUANA(Conexion.basepgjp);
            ////////////////////Intercompany.Data.Intercompany.ADUANA(Conexion.baseworld);
            ////////////////////Intercompany.Data.Intercompany.ADUANA(Conexion.basezhej);
            //////////////////////#endregion

            //#region familia_subfamilia
            Intercompany.Data.Intercompany.CrearFAMI_SUBFAMI(Conexion.ORIGEN);

            //#endregion

            //#region articuloGEN
            Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.baseamd);
            Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.baseboquete);
            Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.basecalypso);
            Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.basecanaima);
            Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.baseconstru);
            Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.baseeco);
            Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.basefarmar);
            Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.baseideo);
            Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.baseiverco);
            Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.basepanco);
            Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.basepgjp);
            Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.basetorino);
            Intercompany.Data.Intercompany.CreararticuloTODAS(Conexion.basezhej);
            //#endregion

            Intercompany.Data.Intercompany.CreararticuloWORLD(Conexion.baseworld);


            //#region articuloactualiza
            Intercompany.Data.Intercompany.ActualizararticuloTODAS(Conexion.baseamd);
            Intercompany.Data.Intercompany.ActualizararticuloTODAS(Conexion.baseboquete);
            Intercompany.Data.Intercompany.ActualizararticuloTODAS(Conexion.basecalypso);
            Intercompany.Data.Intercompany.ActualizararticuloTODAS(Conexion.basecanaima);
            Intercompany.Data.Intercompany.ActualizararticuloTODAS(Conexion.baseconstru);
            Intercompany.Data.Intercompany.ActualizararticuloTODAS(Conexion.baseeco);
            Intercompany.Data.Intercompany.ActualizararticuloTODAS(Conexion.basefarmar);
            Intercompany.Data.Intercompany.ActualizararticuloTODAS(Conexion.baseideo);
            Intercompany.Data.Intercompany.ActualizararticuloTODAS(Conexion.baseiverco);
            Intercompany.Data.Intercompany.ActualizararticuloTODAS(Conexion.basepanco);
            Intercompany.Data.Intercompany.ActualizararticuloTODAS(Conexion.basetorino);
            Intercompany.Data.Intercompany.ActualizararticuloTODAS(Conexion.basepgjp);
            Intercompany.Data.Intercompany.ActualizararticuloTODAS(Conexion.basezhej);
            //#endregion


            //#region SOCIO_GEN
            Intercompany.Data.Intercompany.CrearSOCIOTODAS(Conexion.baseamd);
            Intercompany.Data.Intercompany.CrearSOCIOTODAS(Conexion.baseboquete);
            Intercompany.Data.Intercompany.CrearSOCIOTODAS(Conexion.basecalypso);
            Intercompany.Data.Intercompany.CrearSOCIOTODAS(Conexion.basecanaima);
            Intercompany.Data.Intercompany.CrearSOCIOTODAS(Conexion.baseconstru);
            Intercompany.Data.Intercompany.CrearSOCIOTODAS(Conexion.baseeco);
            Intercompany.Data.Intercompany.CrearSOCIOTODAS(Conexion.basefarmar);
            Intercompany.Data.Intercompany.CrearSOCIOTODAS(Conexion.baseideo);
            Intercompany.Data.Intercompany.CrearSOCIOTODAS(Conexion.baseiverco);
            Intercompany.Data.Intercompany.CrearSOCIOTODAS(Conexion.basepanco);
            Intercompany.Data.Intercompany.CrearSOCIOTODAS(Conexion.basepgjp);
            Intercompany.Data.Intercompany.CrearSOCIOTODAS(Conexion.basetorino);
            Intercompany.Data.Intercompany.CrearSOCIOTODAS(Conexion.basezhej);
            Intercompany.Data.Intercompany.CrearSOCIOTODAS(Conexion.baseworld);
//#endregion


            //#region SOCIOactualiza
            Intercompany.Data.Intercompany.ActualizaSOCIOTODAS(Conexion.baseamd);
            Intercompany.Data.Intercompany.ActualizaSOCIOTODAS(Conexion.baseboquete);
            Intercompany.Data.Intercompany.ActualizaSOCIOTODAS(Conexion.basecalypso);
            Intercompany.Data.Intercompany.ActualizaSOCIOTODAS(Conexion.basecanaima);
            Intercompany.Data.Intercompany.ActualizaSOCIOTODAS(Conexion.baseconstru);
            Intercompany.Data.Intercompany.ActualizaSOCIOTODAS(Conexion.baseeco);
            Intercompany.Data.Intercompany.ActualizaSOCIOTODAS(Conexion.basefarmar);
            Intercompany.Data.Intercompany.ActualizaSOCIOTODAS(Conexion.baseideo);
            Intercompany.Data.Intercompany.ActualizaSOCIOTODAS(Conexion.baseiverco);
            Intercompany.Data.Intercompany.ActualizaSOCIOTODAS(Conexion.basepanco);
            Intercompany.Data.Intercompany.ActualizaSOCIOTODAS(Conexion.basepgjp);
            Intercompany.Data.Intercompany.ActualizaSOCIOTODAS(Conexion.basetorino);
            Intercompany.Data.Intercompany.ActualizaSOCIOTODAS(Conexion.basezhej);
            Intercompany.Data.Intercompany.ActualizaSOCIOTODAS(Conexion.baseworld);
            //#endregion

            //#region SCSucursalGEN
            Intercompany.Data.Intercompany.SCSucursal(Conexion.baseamd);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.baseboquete);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.basecalypso);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.basecanaima);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.baseconstru);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.baseeco);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.basefarmar);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.baseideo);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.baseiverco);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.basepanco);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.basepgjp);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.basetorino);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.baseworld);
            Intercompany.Data.Intercompany.SCSucursal(Conexion.basezhej);
            //#endregion

            //NEW METODO
            #region OCSucursal_MCH
            Intercompany.Data.Intercompany.OCSucursal_MCH(Conexion.baseamd);
            Intercompany.Data.Intercompany.OCSucursal_MCH(Conexion.baseboquete);
            Intercompany.Data.Intercompany.OCSucursal_MCH(Conexion.basecalypso);
            Intercompany.Data.Intercompany.OCSucursal_MCH(Conexion.basecanaima);
            Intercompany.Data.Intercompany.OCSucursal_MCH(Conexion.baseconstru);
            Intercompany.Data.Intercompany.OCSucursal_MCH(Conexion.baseeco);
            Intercompany.Data.Intercompany.OCSucursal_MCH(Conexion.basefarmar);
            Intercompany.Data.Intercompany.OCSucursal_MCH(Conexion.baseideo);
            Intercompany.Data.Intercompany.OCSucursal_MCH(Conexion.baseiverco);
            Intercompany.Data.Intercompany.OCSucursal_MCH(Conexion.basepanco);
            Intercompany.Data.Intercompany.OCSucursal_MCH(Conexion.basepgjp);
            Intercompany.Data.Intercompany.OCSucursal_MCH(Conexion.basetorino);
            Intercompany.Data.Intercompany.OCSucursal_MCH(Conexion.baseworld);
            Intercompany.Data.Intercompany.OCSucursal_MCH(Conexion.basezhej);
            #endregion
            // FIN NEW METODO
            // NEW METODO actualiza oc sucursal a mch
            #region OCSucursal_MCH
            Intercompany.Data.Intercompany.STATUSSucursal_MCH(Conexion.baseamd);
            Intercompany.Data.Intercompany.STATUSSucursal_MCH(Conexion.baseboquete);
            Intercompany.Data.Intercompany.STATUSSucursal_MCH(Conexion.basecalypso);
            Intercompany.Data.Intercompany.STATUSSucursal_MCH(Conexion.basecanaima);
            Intercompany.Data.Intercompany.STATUSSucursal_MCH(Conexion.baseconstru);
            Intercompany.Data.Intercompany.STATUSSucursal_MCH(Conexion.baseeco);
            Intercompany.Data.Intercompany.STATUSSucursal_MCH(Conexion.basefarmar);
            Intercompany.Data.Intercompany.STATUSSucursal_MCH(Conexion.baseideo);
            Intercompany.Data.Intercompany.STATUSSucursal_MCH(Conexion.baseiverco);
            Intercompany.Data.Intercompany.STATUSSucursal_MCH(Conexion.basepanco);
            Intercompany.Data.Intercompany.STATUSSucursal_MCH(Conexion.basepgjp);
            Intercompany.Data.Intercompany.STATUSSucursal_MCH(Conexion.basetorino);
            Intercompany.Data.Intercompany.STATUSSucursal_MCH(Conexion.baseworld);
            Intercompany.Data.Intercompany.STATUSSucursal_MCH(Conexion.basezhej);
            #endregion
            //FIN NEW METODO

            #region SCImportcGEN
            Intercompany.Data.Intercompany.SCImportc(Conexion.baseamd);
            Intercompany.Data.Intercompany.SCImportc(Conexion.baseboquete);
            Intercompany.Data.Intercompany.SCImportc(Conexion.basecalypso);
            Intercompany.Data.Intercompany.SCImportc(Conexion.basecanaima);
            Intercompany.Data.Intercompany.SCImportc(Conexion.baseconstru);
            Intercompany.Data.Intercompany.SCImportc(Conexion.baseeco);
            Intercompany.Data.Intercompany.SCImportc(Conexion.basefarmar);
            Intercompany.Data.Intercompany.SCImportc(Conexion.baseideo);
            Intercompany.Data.Intercompany.SCImportc(Conexion.baseiverco);
            Intercompany.Data.Intercompany.SCImportc(Conexion.basepanco);
            Intercompany.Data.Intercompany.SCImportc(Conexion.basepgjp);
            Intercompany.Data.Intercompany.SCImportc(Conexion.basetorino);
            Intercompany.Data.Intercompany.SCImportc(Conexion.baseworld);//preguntar a omar si esta la quito ya que sino quedaria wolrbrand a world brande y no tiene sentuido
            Intercompany.Data.Intercompany.SCImportc(Conexion.basezhej);
            #endregion



            #region OFERTAMCH_SUCGEN
            Intercompany.Data.Intercompany.OFERTAMCH_SUC(Conexion.baseamd);
            Intercompany.Data.Intercompany.OFERTAMCH_SUC(Conexion.baseboquete);
            Intercompany.Data.Intercompany.OFERTAMCH_SUC(Conexion.basecalypso);
            Intercompany.Data.Intercompany.OFERTAMCH_SUC(Conexion.basecanaima);
            Intercompany.Data.Intercompany.OFERTAMCH_SUC(Conexion.baseconstru);
            Intercompany.Data.Intercompany.OFERTAMCH_SUC(Conexion.baseeco);
            Intercompany.Data.Intercompany.OFERTAMCH_SUC(Conexion.basefarmar);
            Intercompany.Data.Intercompany.OFERTAMCH_SUC(Conexion.baseideo);
            Intercompany.Data.Intercompany.OFERTAMCH_SUC(Conexion.baseiverco);
            Intercompany.Data.Intercompany.OFERTAMCH_SUC(Conexion.basepanco);
            Intercompany.Data.Intercompany.OFERTAMCH_SUC(Conexion.basepgjp);
            Intercompany.Data.Intercompany.OFERTAMCH_SUC(Conexion.basetorino);
            Intercompany.Data.Intercompany.OFERTAMCH_SUC(Conexion.baseworld);
            Intercompany.Data.Intercompany.OFERTAMCH_SUC(Conexion.basezhej);
            #endregion



            #region actualizaMCH_OFERTA
            Intercompany.Data.Intercompany.BuscaACT_OFERTASUC(Conexion.baseamd);
            Intercompany.Data.Intercompany.BuscaACT_OFERTASUC(Conexion.baseboquete);
            Intercompany.Data.Intercompany.BuscaACT_OFERTASUC(Conexion.basecalypso);
            Intercompany.Data.Intercompany.BuscaACT_OFERTASUC(Conexion.basecanaima);
            Intercompany.Data.Intercompany.BuscaACT_OFERTASUC(Conexion.baseconstru);
            Intercompany.Data.Intercompany.BuscaACT_OFERTASUC(Conexion.baseeco);
            Intercompany.Data.Intercompany.BuscaACT_OFERTASUC(Conexion.basefarmar);
            Intercompany.Data.Intercompany.BuscaACT_OFERTASUC(Conexion.baseideo);
            Intercompany.Data.Intercompany.BuscaACT_OFERTASUC(Conexion.baseiverco);
            Intercompany.Data.Intercompany.BuscaACT_OFERTASUC(Conexion.basepanco);
            Intercompany.Data.Intercompany.BuscaACT_OFERTASUC(Conexion.basepgjp);
            Intercompany.Data.Intercompany.BuscaACT_OFERTASUC(Conexion.basetorino);
            Intercompany.Data.Intercompany.BuscaACT_OFERTASUC(Conexion.baseworld);
            Intercompany.Data.Intercompany.BuscaACT_OFERTASUC(Conexion.basezhej);
            #endregion

            #region orden_compra_sucursal

            Intercompany.Data.Intercompany.OC_MCH_a_SUC(Conexion.baseamd);
            Intercompany.Data.Intercompany.OC_MCH_a_SUC(Conexion.baseboquete);
            Intercompany.Data.Intercompany.OC_MCH_a_SUC(Conexion.basecalypso);
            Intercompany.Data.Intercompany.OC_MCH_a_SUC(Conexion.basecanaima);
            Intercompany.Data.Intercompany.OC_MCH_a_SUC(Conexion.baseconstru);
            Intercompany.Data.Intercompany.OC_MCH_a_SUC(Conexion.baseeco);
            Intercompany.Data.Intercompany.OC_MCH_a_SUC(Conexion.basefarmar);
            Intercompany.Data.Intercompany.OC_MCH_a_SUC(Conexion.baseideo);
            Intercompany.Data.Intercompany.OC_MCH_a_SUC(Conexion.baseiverco);
            Intercompany.Data.Intercompany.OC_MCH_a_SUC(Conexion.basepanco);
            Intercompany.Data.Intercompany.OC_MCH_a_SUC(Conexion.basetorino);
            Intercompany.Data.Intercompany.OC_MCH_a_SUC(Conexion.basepgjp);
            Intercompany.Data.Intercompany.OC_MCH_a_SUC(Conexion.baseworld);
            Intercompany.Data.Intercompany.OC_MCH_a_SUC(Conexion.basezhej);
            #endregion


            #region actualizaMCH_OCOMPRA
            Intercompany.Data.Intercompany.BuscaACT_OCMCH(Conexion.baseamd);
            Intercompany.Data.Intercompany.BuscaACT_OCMCH(Conexion.baseboquete);
            Intercompany.Data.Intercompany.BuscaACT_OCMCH(Conexion.basecalypso);
            Intercompany.Data.Intercompany.BuscaACT_OCMCH(Conexion.basecanaima);
            Intercompany.Data.Intercompany.BuscaACT_OCMCH(Conexion.baseconstru);
            Intercompany.Data.Intercompany.BuscaACT_OCMCH(Conexion.baseeco);
            Intercompany.Data.Intercompany.BuscaACT_OCMCH(Conexion.basefarmar);
            Intercompany.Data.Intercompany.BuscaACT_OCMCH(Conexion.baseideo);
            Intercompany.Data.Intercompany.BuscaACT_OCMCH(Conexion.baseiverco);
            Intercompany.Data.Intercompany.BuscaACT_OCMCH(Conexion.basepanco);
            Intercompany.Data.Intercompany.BuscaACT_OCMCH(Conexion.basepgjp);
            Intercompany.Data.Intercompany.BuscaACT_OCMCH(Conexion.basetorino);
            Intercompany.Data.Intercompany.BuscaACT_OCMCH(Conexion.baseworld);
            Intercompany.Data.Intercompany.BuscaACT_OCMCH(Conexion.basezhej);
            #endregion

            #region orden_compra_WORLD_sucursal

            Intercompany.Data.Intercompany.OC_World_a_SUC(Conexion.baseamd);
            Intercompany.Data.Intercompany.OC_World_a_SUC(Conexion.baseboquete);
            Intercompany.Data.Intercompany.OC_World_a_SUC(Conexion.basecalypso);
            Intercompany.Data.Intercompany.OC_World_a_SUC(Conexion.basecanaima);
            Intercompany.Data.Intercompany.OC_World_a_SUC(Conexion.baseconstru);
            Intercompany.Data.Intercompany.OC_World_a_SUC(Conexion.baseeco);
            Intercompany.Data.Intercompany.OC_World_a_SUC(Conexion.basefarmar);
            Intercompany.Data.Intercompany.OC_World_a_SUC(Conexion.baseideo);
            Intercompany.Data.Intercompany.OC_World_a_SUC(Conexion.baseiverco);
            Intercompany.Data.Intercompany.OC_World_a_SUC(Conexion.basepanco);
            Intercompany.Data.Intercompany.OC_World_a_SUC(Conexion.basepgjp);
            Intercompany.Data.Intercompany.OC_World_a_SUC(Conexion.basetorino);
            Intercompany.Data.Intercompany.OC_World_a_SUC(Conexion.baseworld);
            Intercompany.Data.Intercompany.OC_World_a_SUC(Conexion.basezhej);
            #endregion




            #region ORDEN_VENTA
            Intercompany.Data.Intercompany.Ovta_World_a_Wolrd(Conexion.baseworld);
            #endregion

        }
    }
}
