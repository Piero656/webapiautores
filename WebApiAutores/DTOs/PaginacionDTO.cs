using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAutores.DTOs
{
    public class PaginacionDTO
    {

        public int Pagina { get; set; }

        private int recodsPorPagina = 10;

        private readonly int cantidadMaximaPorPagina = 50;

        public int RecordsPorPagina 
        {
            get
            {
                return recodsPorPagina;
            }
            set
            {
                recodsPorPagina = (value > cantidadMaximaPorPagina) ? cantidadMaximaPorPagina : value;
            }
        
        
        }
    }
}
