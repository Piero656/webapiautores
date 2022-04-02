using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {
            var datosHateos = new List<DatoHATEOAS>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            datosHateos.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerRoot",new { }), descripcion: "self", metodo: "GET"));

            datosHateos.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutores", new { }), descripcion: "autores", metodo: "GET"));


            if (esAdmin.Succeeded)
            {
                datosHateos.Add(new DatoHATEOAS(enlace: Url.Link("crearAutor", new { }), descripcion: "autor_crear", metodo: "POST"));
                datosHateos.Add(new DatoHATEOAS(enlace: Url.Link("crearLibro", new { }), descripcion: "libro-crear", metodo: "POST"));
            }



            return datosHateos;
        }

    }
}
