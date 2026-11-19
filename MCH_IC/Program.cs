using Inter_Hechiz.Data;
using Inter_Hechiz.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inter_Hechiz
{

    class Program
    {

        static void Main(string[] args)
        {

            OcultarConsola.DisappearConsole();

            RegistroLog objRegistraLog = new RegistroLog();
            Data.Intercompany.SINCROERROR_V2();
            
            
            #region fabricanteGEN
            
            Data.Intercompany.Fabricante(Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.Fabricante(Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.Fabricante(Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.Fabricante(Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.Fabricante(Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.Fabricante(Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.Fabricante(Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.Fabricante(Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.Fabricante(Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.Fabricante(Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.Fabricante(Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region GRUPO_ART

            Data.Intercompany.GrupoArt(Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.GrupoArt(Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.GrupoArt(Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.GrupoArt(Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.GrupoArt(Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.GrupoArt(Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.GrupoArt(Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.GrupoArt(Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.GrupoArt(Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.GrupoArt(Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.GrupoArt(Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region GRUPO_SN

            Data.Intercompany.GrupoSN(Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.GrupoSN(Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.GrupoSN(Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.GrupoSN(Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.GrupoSN(Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.GrupoSN(Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.GrupoSN(Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.GrupoSN(Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.GrupoSN(Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.GrupoSN(Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.GrupoSN(Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region CondicionesGEN

            Data.Intercompany.condicionespago(Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.condicionespago(Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.condicionespago(Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.condicionespago(Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.condicionespago(Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.condicionespago(Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.condicionespago(Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.condicionespago(Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.condicionespago(Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.condicionespago(Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.condicionespago(Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region articuloGEN

            Data.Intercompany.CreararticuloTODASV2("hogar");
            Data.Intercompany.CreararticuloTODASV2("sur");
            Data.Intercompany.CreararticuloTODASV2("omdo");
            Data.Intercompany.CreararticuloTODASV2("nedn");
            Data.Intercompany.CreararticuloTODASV2("flores");
            Data.Intercompany.CreararticuloTODASV2("vital");
            Data.Intercompany.CreararticuloTODASV2("emde");
            Data.Intercompany.CreararticuloTODASV2("central");
            Data.Intercompany.CreararticuloTODASV2("desampa");
            Data.Intercompany.CreararticuloTODASV2("jjmax");
            Data.Intercompany.CreararticuloTODASV2("metropoli");

            #endregion

            #region articuloactualiza 

            Data.Intercompany.ActualizararticuloTODASV2("hogar");
            Data.Intercompany.ActualizararticuloTODASV2("sur");
            Data.Intercompany.ActualizararticuloTODASV2("omdo");
            Data.Intercompany.ActualizararticuloTODASV2("nedn");
            Data.Intercompany.ActualizararticuloTODASV2("flores");
            Data.Intercompany.ActualizararticuloTODASV2("vital");
            Data.Intercompany.ActualizararticuloTODASV2("emde");
            Data.Intercompany.ActualizararticuloTODASV2("central");
            Data.Intercompany.ActualizararticuloTODASV2("desampa");
            Data.Intercompany.ActualizararticuloTODASV2("jjmax");
            Data.Intercompany.ActualizararticuloTODASV2("metropoli");

            #endregion


            #region SOCIO_Crear

            #region enviar socios desde hogar

            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region enviar desde socios a sur

            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_sur, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_sur, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_sur, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_sur, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_sur, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_sur, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_sur, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_sur, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_sur, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_sur, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region enviar socios desde Omdo

            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region enviar socios desde nedn

            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region enviar socios desde flores

            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_flores, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_flores, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_flores, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_flores, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_flores, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_flores, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_flores, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_flores, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_flores, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_flores, Conexion.BD_metropoli, Conexion.token_metropoli);
            #endregion

            #region enviar socios desde vital

            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_vital, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_vital, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_vital, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_vital, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_vital, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_vital, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_vital, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_vital, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_vital, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_vital, Conexion.BD_metropoli, Conexion.token_metropoli);


            #endregion

            #region enviar socios a emde

            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_emde, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_emde, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_emde, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_emde, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_emde, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_emde, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_emde, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_emde, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_emde, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_emde, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region enviar socios a central

            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_central, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_central, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_central, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_central, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_central, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_central, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_central, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_central, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_central, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_central, Conexion.BD_metropoli, Conexion.token_metropoli);
            #endregion

            #region enviar socios a desampa (expresso)

            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion
            
            #region enviar socios a jjmax

            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region enviar socios a metropoli

            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.CrearSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_jjmax, Conexion.token_jjmax);

            #endregion

            #endregion


            #region SOCIOactualiza

            #region actualizar socios desde lo de Hogar

            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_hogar, Conexion.BD_metropoli, Conexion.token_metropoli);


            #endregion

            #region actualizar socios desde sur

            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_sur, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_sur, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_sur, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_sur, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_sur, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_sur, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_sur, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_sur, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_sur, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_sur, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion
             
            #region actualizar socios desde Omdo

            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_omdo, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region actualizar socios desde nedn

            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_nedn, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region actualizar socios desde flores

            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_flores, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_flores, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_flores, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_flores, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_flores, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_flores, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_flores, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_flores, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_flores, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_flores, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region actualizar socios desde vital

            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_vital, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_vital, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_vital, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_vital, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_vital, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_vital, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_vital, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_vital, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_vital, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_vital, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region actualizar socios a emde

            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_emde, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_emde, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_emde, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_emde, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_emde, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_emde, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_emde, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_emde, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_emde, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_emde, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region actualizar socios desde central

            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_central, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_central, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_central, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_central, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_central, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_central, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_central, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_central, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_central, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_central, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion

            #region actualizar socios desde sesampa- expresso

            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_desampa, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion


            #region actualizar socios desde jjmax

            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_jjmax, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion


            #region actualizar socios desde metropoli

            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ActualizaSOCIOTODAS(Conexion.BD_metropoli, Conexion.BD_jjmax, Conexion.token_jjmax);

            #endregion


            #endregion

            #region Descuentos

            Data.Intercompany.CrearGrupoDescuentoV2(Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.CrearGrupoDescuentoV2(Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.CrearGrupoDescuentoV2(Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.CrearGrupoDescuentoV2(Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.CrearGrupoDescuentoV2(Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.CrearGrupoDescuentoV2(Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.CrearGrupoDescuentoV2(Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.CrearGrupoDescuentoV2(Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.CrearGrupoDescuentoV2(Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.CrearGrupoDescuentoV2(Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.CrearGrupoDescuentoV2(Conexion.BD_metropoli, Conexion.token_metropoli);

            Data.Intercompany.ActualizarGrupoDescuentoV2("sur", Conexion.token_sur);
            Data.Intercompany.ActualizarGrupoDescuentoV2("omdo", Conexion.token_omdo);
            Data.Intercompany.ActualizarGrupoDescuentoV2("nedn", Conexion.token_nedn);
            Data.Intercompany.ActualizarGrupoDescuentoV2("hogar", Conexion.token_hogarn);
            Data.Intercompany.ActualizarGrupoDescuentoV2("flores", Conexion.token_flores);
            Data.Intercompany.ActualizarGrupoDescuentoV2("vital", Conexion.token_vital);
            Data.Intercompany.ActualizarGrupoDescuentoV2("emde", Conexion.token_emde);
            Data.Intercompany.ActualizarGrupoDescuentoV2("central", Conexion.token_central);
            Data.Intercompany.ActualizarGrupoDescuentoV2("desampa", Conexion.token_desampa);
            Data.Intercompany.ActualizarGrupoDescuentoV2("jjmax", Conexion.token_jjmax);
            Data.Intercompany.ActualizarGrupoDescuentoV2("metropoli", Conexion.token_metropoli);

            #endregion

            
            #region CrearVendedores

            Data.Intercompany.Crearvendedores("hogar");
            Data.Intercompany.Crearvendedores("sur");
            Data.Intercompany.Crearvendedores("omdo");
            Data.Intercompany.Crearvendedores("nedn");
            Data.Intercompany.Crearvendedores("flores");
            Data.Intercompany.Crearvendedores("vital");
            Data.Intercompany.Crearvendedores("emde");
            Data.Intercompany.Crearvendedores("central");
            Data.Intercompany.Crearvendedores("desampa");
            Data.Intercompany.Crearvendedores("jjmax");
            Data.Intercompany.Crearvendedores("metropoli");

            #endregion

            #region ActualizarVendedores

            Data.Intercompany.Actualizarvendedores("hogar");
            Data.Intercompany.Actualizarvendedores("sur");
            Data.Intercompany.Actualizarvendedores("omdo");
            Data.Intercompany.Actualizarvendedores("nedn");
            Data.Intercompany.Actualizarvendedores("flores");
            Data.Intercompany.Actualizarvendedores("vital");
            Data.Intercompany.Actualizarvendedores("emde");
            Data.Intercompany.Actualizarvendedores("central");
            Data.Intercompany.Actualizarvendedores("desampa");
            Data.Intercompany.Actualizarvendedores("jjmax");
            Data.Intercompany.Actualizarvendedores("metropoli");

            #endregion

           

            #region CrearEmpleados

            Data.Intercompany.CrearEmpleados("hogar");
            Data.Intercompany.CrearEmpleados("sur");
            Data.Intercompany.CrearEmpleados("omdo");
            Data.Intercompany.CrearEmpleados("nedn");
            Data.Intercompany.CrearEmpleados("flores");
            Data.Intercompany.CrearEmpleados("vital");
            Data.Intercompany.CrearEmpleados("emde");
            Data.Intercompany.CrearEmpleados("central");
            Data.Intercompany.CrearEmpleados("desampa");
            Data.Intercompany.CrearEmpleados("jjmax");
            Data.Intercompany.CrearEmpleados("metropoli");

            #endregion

            #region ActualizarEmpleados

            Data.Intercompany.ActualizarEmpleados("hogar");
            Data.Intercompany.ActualizarEmpleados("sur");
            Data.Intercompany.ActualizarEmpleados("omdo");
            Data.Intercompany.ActualizarEmpleados("nedn");
            Data.Intercompany.ActualizarEmpleados("flores");
            Data.Intercompany.ActualizarEmpleados("vital");
            Data.Intercompany.ActualizarEmpleados("emde");
            Data.Intercompany.ActualizarEmpleados("central");
            Data.Intercompany.ActualizarEmpleados("desampa");
            Data.Intercompany.ActualizarEmpleados("jjmax");
            Data.Intercompany.ActualizarEmpleados("metropoli");

            #endregion

            #region Crear SolicitudCompra de Sucursal a SolicitudCompra hechizo

            Data.Intercompany.ProcesarSolictComp("hogar", Conexion.token_hechizo);
            Data.Intercompany.ProcesarSolictComp("sur", Conexion.token_hechizo);
            Data.Intercompany.ProcesarSolictComp("omdo", Conexion.token_hechizo);
            Data.Intercompany.ProcesarSolictComp("nedn", Conexion.token_hechizo);
            Data.Intercompany.ProcesarSolictComp("flores", Conexion.token_hechizo);
            Data.Intercompany.ProcesarSolictComp("vital", Conexion.token_hechizo);
            Data.Intercompany.ProcesarSolictComp("emde", Conexion.token_hechizo);
            Data.Intercompany.ProcesarSolictComp("central", Conexion.token_hechizo);
            Data.Intercompany.ProcesarSolictComp("desampa", Conexion.token_hechizo);
            Data.Intercompany.ProcesarSolictComp("jjmax", Conexion.token_hechizo);
            Data.Intercompany.ProcesarSolictComp("metropoli", Conexion.token_hechizo);

            #endregion
         

            #region Convertir todas las solicitudes de compra a OrdVenta  (Proceso interno Hechizo, no es entre demás empresas)

            Data.Intercompany.ProcesarEnOrdVentaHechizo(Conexion.token_hechizo);

            #endregion
 

            #region Procesa las ord venta de Hechizo a Ord Compra en las sucursales (tomando como documento base las solicitudes de la sucursal de destino)

            Data.Intercompany.ProcesarEnOrdCompra("hogar", Conexion.token_hogarn);
            Data.Intercompany.ProcesarEnOrdCompra("sur", Conexion.token_sur);
            Data.Intercompany.ProcesarEnOrdCompra("omdo", Conexion.token_omdo);
            Data.Intercompany.ProcesarEnOrdCompra("nedn", Conexion.token_nedn);
            Data.Intercompany.ProcesarEnOrdCompra("flores", Conexion.token_flores);
            Data.Intercompany.ProcesarEnOrdCompra("vital", Conexion.token_vital);
            Data.Intercompany.ProcesarEnOrdCompra("emde", Conexion.token_emde);
            Data.Intercompany.ProcesarEnOrdCompra("central", Conexion.token_central);
            Data.Intercompany.ProcesarEnOrdCompra("desampa", Conexion.token_desampa);
            Data.Intercompany.ProcesarEnOrdCompra("jjmax", Conexion.token_jjmax);
            Data.Intercompany.ProcesarEnOrdCompra("metropoli", Conexion.token_metropoli);

            //Data.Intercompany.ProcesarEnOrdCompraV2("desampa", Conexion.token_desampa); 

            #endregion

            #region Replica la actualización de las ordVenta de Hechizo en las OrdCompra en las sucursales

            Data.Intercompany.ActualizarEnOrdCompra("hogar", Conexion.token_hogarn);
            Data.Intercompany.ActualizarEnOrdCompra("sur", Conexion.token_sur);
            Data.Intercompany.ActualizarEnOrdCompra("omdo", Conexion.token_omdo);
            Data.Intercompany.ActualizarEnOrdCompra("nedn", Conexion.token_nedn);
            Data.Intercompany.ActualizarEnOrdCompra("flores", Conexion.token_flores);
            Data.Intercompany.ActualizarEnOrdCompra("vital", Conexion.token_vital);
            Data.Intercompany.ActualizarEnOrdCompra("emde", Conexion.token_emde);
            Data.Intercompany.ActualizarEnOrdCompra("central", Conexion.token_central);
            Data.Intercompany.ActualizarEnOrdCompra("desampa", Conexion.token_desampa);
            Data.Intercompany.ActualizarEnOrdCompra("jjmax", Conexion.token_jjmax);
            Data.Intercompany.ActualizarEnOrdCompra("metropoli", Conexion.token_metropoli);

            // Data.Intercompany.ActualizarEnOrdCompraV2("desampa", Conexion.token_desampa); TEST


            #endregion

            #region Procesa las Facturas de deudor de Hechizo como factura de prove en las sucursales

            Data.Intercompany.ProcesarFacturaProveEnSucursales("hogar", Conexion.token_hogarn);
            Data.Intercompany.ProcesarFacturaProveEnSucursales("sur", Conexion.token_sur);
            Data.Intercompany.ProcesarFacturaProveEnSucursales("omdo", Conexion.token_omdo);
            Data.Intercompany.ProcesarFacturaProveEnSucursales("nedn", Conexion.token_nedn);
            Data.Intercompany.ProcesarFacturaProveEnSucursales("flores", Conexion.token_flores);
            Data.Intercompany.ProcesarFacturaProveEnSucursales("vital", Conexion.token_vital);
            Data.Intercompany.ProcesarFacturaProveEnSucursales("emde", Conexion.token_emde);
            Data.Intercompany.ProcesarFacturaProveEnSucursales("central", Conexion.token_central);
            Data.Intercompany.ProcesarFacturaProveEnSucursales("desampa", Conexion.token_desampa);
            Data.Intercompany.ProcesarFacturaProveEnSucursales("jjmax", Conexion.token_jjmax);
            Data.Intercompany.ProcesarFacturaProveEnSucursales("metropoli", Conexion.token_metropoli);

            #endregion

            #region  Procesa las NC de deudor de Hechizo como factura de prove en las sucursales

            Data.Intercompany.ProcesarNotasCredProveEnSucursales("hogar", Conexion.token_hogarn);
            Data.Intercompany.ProcesarNotasCredProveEnSucursales("sur", Conexion.token_sur);
            Data.Intercompany.ProcesarNotasCredProveEnSucursalesV1("omdo", Conexion.token_omdo);
            Data.Intercompany.ProcesarNotasCredProveEnSucursales("nedn", Conexion.token_nedn);
            Data.Intercompany.ProcesarNotasCredProveEnSucursales("flores", Conexion.token_flores);
            Data.Intercompany.ProcesarNotasCredProveEnSucursales("vital", Conexion.token_vital);
            Data.Intercompany.ProcesarNotasCredProveEnSucursales("emde", Conexion.token_emde);
            Data.Intercompany.ProcesarNotasCredProveEnSucursales("central", Conexion.token_central);
            Data.Intercompany.ProcesarNotasCredProveEnSucursales("desampa", Conexion.token_desampa);
            Data.Intercompany.ProcesarNotasCredProveEnSucursales("jjmax", Conexion.token_jjmax);
            Data.Intercompany.ProcesarNotasCredProveEnSucursales("metropoli", Conexion.token_metropoli);

            #endregion


            #region Replica la actualizacion de las Factura Vendedor de Hechizo en las FactProve en las sucursales

            Data.Intercompany.ActualizarEnFacturaProvee("hogar", Conexion.token_hogarn);
            Data.Intercompany.ActualizarEnFacturaProvee("sur", Conexion.token_sur);
            Data.Intercompany.ActualizarEnFacturaProvee("omdo", Conexion.token_omdo);
            Data.Intercompany.ActualizarEnFacturaProvee("nedn", Conexion.token_nedn);
            Data.Intercompany.ActualizarEnFacturaProvee("flores", Conexion.token_flores);
            Data.Intercompany.ActualizarEnFacturaProvee("vital", Conexion.token_vital);
            Data.Intercompany.ActualizarEnFacturaProvee("emde", Conexion.token_emde);
            Data.Intercompany.ActualizarEnFacturaProvee("central", Conexion.token_central);
            Data.Intercompany.ActualizarEnFacturaProvee("desampa", Conexion.token_desampa);
            Data.Intercompany.ActualizarEnFacturaProvee("jjmax", Conexion.token_jjmax);
            Data.Intercompany.ActualizarEnFacturaProvee("metropoli", Conexion.token_metropoli);


            #endregion

            #region Replica la actualizacion de las NCdeudor de Hechizo en las NCProveedor en las sucursales

            Data.Intercompany.ActualizarEnNotaCredProvee("hogar", Conexion.token_hechizo);
            Data.Intercompany.ActualizarEnNotaCredProvee("sur", Conexion.token_hechizo);
            Data.Intercompany.ActualizarEnNotaCredProvee("omdo", Conexion.token_hechizo);
            Data.Intercompany.ActualizarEnNotaCredProvee("nedn", Conexion.token_hechizo);
            Data.Intercompany.ActualizarEnNotaCredProvee("flores", Conexion.token_hechizo);
            Data.Intercompany.ActualizarEnNotaCredProvee("vital", Conexion.token_hechizo);
            Data.Intercompany.ActualizarEnNotaCredProvee("emde", Conexion.token_hechizo);
            Data.Intercompany.ActualizarEnNotaCredProvee("central", Conexion.token_hechizo);
            Data.Intercompany.ActualizarEnNotaCredProvee("desampa", Conexion.token_desampa);
            Data.Intercompany.ActualizarEnNotaCredProvee("jjmax", Conexion.token_jjmax);
            Data.Intercompany.ActualizarEnNotaCredProvee("metropoli", Conexion.token_metropoli);

            #endregion

            #region Cuentas 

            #region Crear Cuentas
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("hogar", Conexion.token_hogarn, Data.Intercompany.ProcesoCuenta.Crear);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("sur", Conexion.token_sur, Data.Intercompany.ProcesoCuenta.Crear);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("omdo", Conexion.token_omdo, Data.Intercompany.ProcesoCuenta.Crear);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("nedn", Conexion.token_nedn, Data.Intercompany.ProcesoCuenta.Crear);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("flores", Conexion.token_flores, Data.Intercompany.ProcesoCuenta.Crear);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("vital", Conexion.token_vital, Data.Intercompany.ProcesoCuenta.Crear);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("emde", Conexion.token_emde, Data.Intercompany.ProcesoCuenta.Crear);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("central", Conexion.token_central, Data.Intercompany.ProcesoCuenta.Crear);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("desampa", Conexion.token_desampa, Data.Intercompany.ProcesoCuenta.Crear);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("jjmax", Conexion.token_jjmax, Data.Intercompany.ProcesoCuenta.Crear);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("metropoli", Conexion.token_metropoli, Data.Intercompany.ProcesoCuenta.Crear);

            #endregion

            #region Actualizar cuentas
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("hogar", Conexion.token_hogarn, Data.Intercompany.ProcesoCuenta.Actualizar);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("sur", Conexion.token_sur, Data.Intercompany.ProcesoCuenta.Actualizar);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("omdo", Conexion.token_omdo, Data.Intercompany.ProcesoCuenta.Actualizar);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("nedn", Conexion.token_nedn, Data.Intercompany.ProcesoCuenta.Actualizar);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("flores", Conexion.token_flores, Data.Intercompany.ProcesoCuenta.Actualizar);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("vital", Conexion.token_vital, Data.Intercompany.ProcesoCuenta.Actualizar);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("emde", Conexion.token_emde, Data.Intercompany.ProcesoCuenta.Actualizar);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("central", Conexion.token_central, Data.Intercompany.ProcesoCuenta.Actualizar);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("desampa", Conexion.token_desampa, Data.Intercompany.ProcesoCuenta.Actualizar);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("jjmax", Conexion.token_jjmax, Data.Intercompany.ProcesoCuenta.Actualizar);
            Data.Intercompany.ProcesarCuentasdesdeHechizoV2("metropoli", Conexion.token_metropoli, Data.Intercompany.ProcesoCuenta.Actualizar);
            #endregion

            #endregion

            #region Pagos

            #region enviar pagos recibidos a Hechizo

            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_hogar, Conexion.BD_Hechizo, Conexion.token_hechizo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_sur, Conexion.BD_Hechizo, Conexion.token_hechizo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_omdo, Conexion.BD_Hechizo, Conexion.token_hechizo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_nedn, Conexion.BD_Hechizo, Conexion.token_hechizo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_flores, Conexion.BD_Hechizo, Conexion.token_hechizo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_vital, Conexion.BD_Hechizo, Conexion.token_hechizo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_emde, Conexion.BD_Hechizo, Conexion.token_hechizo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_central, Conexion.BD_Hechizo, Conexion.token_hechizo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_desampa, Conexion.BD_Hechizo, Conexion.token_hechizo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_jjmax, Conexion.BD_Hechizo, Conexion.token_hechizo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_metropoli, Conexion.BD_Hechizo, Conexion.token_hechizo);


            #endregion

            #region enviar pagos recibidos a hogar

            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_Hechizo, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_sur, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_omdo, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_nedn, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_flores, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_vital, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_emde, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_central, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_desampa, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_jjmax, Conexion.BD_hogar, Conexion.token_hogarn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_metropoli, Conexion.BD_hogar, Conexion.token_hogarn);

            #endregion

            #region enviar pagos recibidos a sur

            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_Hechizo, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_hogar, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_omdo, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_nedn, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_flores, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_vital, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_emde, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_central, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_desampa, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_jjmax, Conexion.BD_sur, Conexion.token_sur);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_metropoli, Conexion.BD_sur, Conexion.token_sur);

            #endregion

            #region enviar pagos recibidos a omdo

            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_Hechizo, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_hogar, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_sur, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_nedn, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_flores, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_vital, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_emde, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_central, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_desampa, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_jjmax, Conexion.BD_omdo, Conexion.token_omdo);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_metropoli, Conexion.BD_omdo, Conexion.token_omdo);

            #endregion

            #region enviar pagos recibidos a nedn

            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_Hechizo, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_hogar, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_sur, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_omdo, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_flores, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_vital, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_emde, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_central, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_desampa, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_jjmax, Conexion.BD_nedn, Conexion.token_nedn);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_metropoli, Conexion.BD_nedn, Conexion.token_nedn);

            #endregion

            #region enviar pagos recibidos a flores

            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_Hechizo, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_hogar, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_sur, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_omdo, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_nedn, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_vital, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_emde, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_central, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_desampa, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_jjmax, Conexion.BD_flores, Conexion.token_flores);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_metropoli, Conexion.BD_flores, Conexion.token_flores);

            #endregion

            #region enviar pagos recibidos a vital

            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_Hechizo, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_hogar, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_sur, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_omdo, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_nedn, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_flores, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_emde, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_central, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_desampa, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_jjmax, Conexion.BD_vital, Conexion.token_vital);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_metropoli, Conexion.BD_vital, Conexion.token_vital);

            #endregion

            #region enviar pagos recibidos a emde

            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_Hechizo, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_hogar, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_sur, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_omdo, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_nedn, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_flores, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_vital, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_central, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_desampa, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_jjmax, Conexion.BD_emde, Conexion.token_emde);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_metropoli, Conexion.BD_emde, Conexion.token_emde);

            #endregion

            #region enviar pagos recibidos a central

            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_Hechizo, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_hogar, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_sur, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_omdo, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_nedn, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_flores, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_vital, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_emde, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_desampa, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_jjmax, Conexion.BD_central, Conexion.token_central);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_metropoli, Conexion.BD_central, Conexion.token_central);

            #endregion

            #region enviar pagos recibidos a Desampa(expresso)

            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_Hechizo, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_hogar, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_sur, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_omdo, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_nedn, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_flores, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_vital, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_emde, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_central, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_jjmax, Conexion.BD_desampa, Conexion.token_desampa);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_metropoli, Conexion.BD_desampa, Conexion.token_desampa);

            #endregion


            #region enviar pagos recibidos a jjMax

            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_Hechizo, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_hogar, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_sur, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_omdo, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_nedn, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_flores, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_vital, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_emde, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_central, Conexion.BD_jjmax, Conexion.token_jjmax);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_metropoli, Conexion.BD_jjmax, Conexion.token_jjmax);

            #endregion


            #region enviar pagos recibidos a Metropoli

            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_Hechizo, Conexion.BD_metropoli, Conexion.token_metropoli);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_hogar, Conexion.BD_metropoli, Conexion.token_metropoli);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_sur, Conexion.BD_metropoli, Conexion.token_metropoli);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_omdo, Conexion.BD_metropoli, Conexion.token_metropoli);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_nedn, Conexion.BD_metropoli, Conexion.token_metropoli);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_flores, Conexion.BD_metropoli, Conexion.token_metropoli);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_vital, Conexion.BD_metropoli, Conexion.token_metropoli);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_emde, Conexion.BD_metropoli, Conexion.token_metropoli);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_central, Conexion.BD_metropoli, Conexion.token_metropoli);
            Data.Intercompany.ProcesarEnPagoRecibido(Conexion.BD_jjmax, Conexion.BD_metropoli, Conexion.token_metropoli);

            #endregion


            #endregion


        }
    }
}
