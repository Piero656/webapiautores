using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAutores.DTOs
{
    public class DatoHATEOAS
    {
        public string Enlace { get; set; }
        public string Descripcion { get; set; }
        public string Metodo { get; set; }

        public DatoHATEOAS(string enlace, string descripcion, string metodo)
        {
            Enlace = enlace;
            Descripcion = descripcion;
            Metodo = metodo;
        }
    }
}
