using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class AutorCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 120)]
        [MinLength(3, ErrorMessage = "El campo {0} debe ser mayor a {1} caracteres")]
        [PrimeraLetraMayusculaAtribute]
        public string Nombre { get; set; }
    }
}
