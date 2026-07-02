using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inter_Hechiz.Tools
{
    public class gestorarchivos
    {
        public static int paso = 0;

        public static void MoverArchivo(string nombreArchivo, string rutaOrigen, string rutaDestino,string ext)
        {
            paso = 0;
            try
            {
                // Asegurarse de que la ruta de origen exista
                if (!Directory.Exists(rutaOrigen))
                {
                    Console.WriteLine("La ruta de origen no existe.");
                    return;
                }

                // Asegurarse de que la ruta de destino exista (y crearla si no)
                if (!Directory.Exists(rutaDestino))
                {
                    Directory.CreateDirectory(rutaDestino);
                }
                string archivito = nombreArchivo + "." + ext;
                // Ruta completa del archivo a buscar
                string rutaArchivoOrigen = Path.Combine(rutaOrigen, archivito);

                if (!File.Exists(rutaArchivoOrigen))
                {
                    Console.WriteLine("El archivo especificado no se encontró en la ruta de origen.");
                    return;
                }

                // Ruta final en el destino
                string rutaArchivoDestino = Path.Combine(rutaDestino, archivito);

                // Mover el archivo
                if (!File.Exists(rutaArchivoDestino))
                {
                    File.Copy(rutaArchivoOrigen, rutaArchivoDestino, true);
                    paso = 1;
                }
                else
                {
                    File.Copy(rutaArchivoOrigen, rutaArchivoDestino, true);
                    Console.WriteLine("Archivo movido correctamente.");
                    paso = 1;
                }
                
    }
            catch (IOException ex)
            {
                Console.WriteLine("Error al mover el archivo: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inesperado: " + ex.Message);
            }
          }
    }
}
