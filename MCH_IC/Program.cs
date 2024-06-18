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
            #region fabricanteGEN
            Intercompany.Data.Intercompany.Fabricante(Conexion.baseamd);
            //Fabricante(Conexion.baseamd);
            //Fabricante(Conexion.baseboquete);
            //Fabricante(Conexion.baseamd);
            //Fabricante(Conexion.baseamd);
            //Fabricante(Conexion.baseamd);
            //Fabricante(Conexion.baseamd);

            #endregion
            //inter.CrearFabricanteTODAS();      
            //inter.CrearGrupoTODAS();
            //inter.CrearFamiliaTODAS();
            //inter.CrearAduanasTODAS();
            //inter.CrearArticulosTD();
            //inter.CrearAduanasTODAS();
            //inter.ActualizarArticulosTD();

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
