using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor //: IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        // Se puede utilizar placeholders {0} o {1} para colocar el nombre del campo (0) o las restricciones (1)
        [StringLength(maximumLength: 120)]
        [MinLength(3, ErrorMessage = "El campo {0} debe ser mayor a {1} caracteres")]
        [PrimeraLetraMayusculaAtribute]
        public string Nombre { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }
        //[Range(18,120)]
        //[NotMapped]
        //public int Edad { get; set; }
        //[CreditCard]
        //[NotMapped]
        //public string TarjetaDeCredito { get; set; }
        //[Url]
        //[NotMapped]
        //public string Url { get; set; }

        //public int Menor { get; set; }
        //public int Mayor { get; set; }


        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (!string.IsNullOrEmpty(Nombre))
        //    {
        //        var primeraLetra = Nombre[0].ToString();

        //        if (primeraLetra != primeraLetra.ToUpper())
        //        {
        //            yield return new ValidationResult("La primera Letra debe ser mayuscula",
        //                new string[] { nameof(Nombre) });
        //        }
        //    }

        //    //if (Menor >= Mayor)
        //    //{
        //    //    yield return new ValidationResult("El campoo Menor no Pueder ser mayor o igual al campo Mayor",
        //    //            new string[] { nameof(Menor) });
        //    //}


        //}




    }
}
