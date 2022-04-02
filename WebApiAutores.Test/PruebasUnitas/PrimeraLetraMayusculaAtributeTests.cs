using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Test.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaAtributeTests
    {
        [TestMethod]
        public void PrimeraLetraMinusculaDevuelveError()
        {
            //Preparacion

            var primeraLetraMayuscula = new PrimeraLetraMayusculaAtribute();
            var valor = "felipe";

            var valContext = new ValidationContext(new { Nombre = valor });


            //Ejecucion

            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);


            //Verificacion

            Assert.AreEqual("La Primera Letra debe ser mayuscula", resultado.ErrorMessage);

        }

        [TestMethod]
        public void ValorNulo_NoDevuelveError()
        {
            //Preparacion

            var primeraLetraMayuscula = new PrimeraLetraMayusculaAtribute();
            string valor = null;

            var valContext = new ValidationContext(new { Nombre = valor });


            //Ejecucion

            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);


            //Verificacion

            Assert.IsNull(resultado);

        }


        [TestMethod]
        public void ValorConPrimeraLetraMayuscula_NoDevuelveError()
        {
            //Preparacion

            var primeraLetraMayuscula = new PrimeraLetraMayusculaAtribute();
            string valor = "Piero";

            var valContext = new ValidationContext(new { Nombre = valor });


            //Ejecucion

            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);


            //Verificacion

            Assert.IsNull(resultado);

        }
    }
}
