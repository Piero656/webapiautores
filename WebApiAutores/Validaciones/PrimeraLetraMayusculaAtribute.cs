﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAutores.Validaciones
{
    public class PrimeraLetraMayusculaAtribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null ||  string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var primeraLetra = value.ToString()[0].ToString();

            if (primeraLetra != primeraLetra.ToUpper())
            {
                return new ValidationResult("La Primera Letra debe ser mayuscula");
            }

            return ValidationResult.Success;

        }
    }
}