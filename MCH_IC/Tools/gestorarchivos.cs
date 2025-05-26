using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCH_IC.Tools
{
    public class gestorarchivos
    {
        public static void MoverArchivo(string nombreArchivo, string rutaOrigen, string rutaDestino)
        {
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

                // Ruta completa del archivo a buscar
                string rutaArchivoOrigen = Path.Combine(rutaOrigen, nombreArchivo);

                if (!File.Exists(rutaArchivoOrigen))
                {
                    Console.WriteLine("El archivo especificado no se encontró en la ruta de origen.");
                    return;
                }

                // Ruta final en el destino
                string rutaArchivoDestino = Path.Combine(rutaDestino, nombreArchivo);

                // Mover el archivo
                File.Move(rutaArchivoOrigen, rutaArchivoDestino);

                Console.WriteLine("Archivo movido correctamente.");
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
