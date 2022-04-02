using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;
using WebApiAutores.Servicios;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    //[Route("api/v1/autores")]
    [Route("api/autores")]
    [CabeceraEstaPresente("x-version","1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Policy = "EsAdmin")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    //[Authorize]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicio servicio;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioSinglenton servicioSinglenton;
        private readonly ILogger<AutoresController> logger;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IServicio servicio,
            ServicioScoped servicioScoped, ServicioTransient servicioTransient,
            ServicioSinglenton servicioSinglenton, ILogger<AutoresController> logger,
            IMapper mapper, IConfiguration configuration,
            IAuthorizationService authorizationService)
        {
            this.context = context;
            this.servicio = servicio;
            this.servicioScoped = servicioScoped;
            this.servicioTransient = servicioTransient;
            this.servicioSinglenton = servicioSinglenton;
            this.logger = logger;
            this.mapper = mapper;
            this.configuration = configuration;
            this.authorizationService = authorizationService;
        }


        //Se puede tener más de una ruta para un mismo EndPoint
        //[HttpGet]
        //[HttpGet("listado")]
        //[HttpGet("/listado")]
        ////[ResponseCache(Duration = 10)]
        //[ServiceFilter(typeof(MiFiltroDeAccion))]
        //public async Task<ActionResult<List<Autor>>> Get()
        //{
        //    throw new NotImplementedException();
        //    servicio.RealizarTarea();
        //    logger.LogInformation("Estamos obteniendo los autores");
        //    return await context.Autores.Include(x=>x.Libros).ToListAsync();
        //}


        [HttpGet(Name = "obtenerAutores")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOSAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Autores.AsQueryable();

            await HttpContext.InsertarParametroPaginacionEnCabecera(queryable);

            //var autores = await context.Autores.ToListAsync();

            var autores = await queryable.OrderBy(autor => autor.Nombre).Paginar(paginacionDTO).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);


            //if (incluirHATEOAS)
            //{
            //    var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");


            //    autoresDTO.ForEach(x => GenerarEnlaces(x, esAdmin.Succeeded));

            //    var resultado = new ColeccionDeRecursos<AutorDTO>()
            //    {
            //        Valores = autoresDTO
            //    };

            //    resultado.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutores", new { }), descripcion: "self",
            //        metodo: "GET"));

            //    if (esAdmin.Succeeded)
            //    {
            //        resultado.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("crearAutor", new { }), descripcion: "crear-autor",
            //            metodo: "POST"));
            //    }

            //    return Ok(resultado);
            //}





 
        }


        [HttpGet("primero")]
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] string miValor, [FromQuery] string query)
        {
            return await context.Autores.FirstOrDefaultAsync();
        }


        // Task se usa para poder devolver funciones de forma asincrona
        // sin el Task no se puede devolver un ActionResult de una funcion asincrona 


        [HttpGet("{id:int}", Name = "obtenerAutor")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOSAutorFilterAttribute))]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(200)]
        public async Task<ActionResult<AutorDTO>> Get(int id)
        {
            var autor = await context.Autores
                .Include(autor => autor.AutoresLibros)
                    .ThenInclude(autorlibro => autorlibro.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<AutorDTO>(autor);

            return dto;
        }


      

        // Tambien se puede usar IActionResult pero en este caso no se especifica el tipo de dato que se devulel
        // por lo que se puede devolver cualquier tipo de dato dentro de un ActionResult.

        //[HttpGet("{id:int}/{param2=persona}")]
        //public async Task<IActionResult> Get(int id, string param2)
        //{
        //    var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

        //    if (autor == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(autor);
        //}

        [HttpGet("{nombre}", Name = "obtenerAutorPorNombre")]
        public async Task<ActionResult<List<AutorDTO>>> GetPorNombre([FromRoute] string nombre)
        {
            var autores = await context.Autores
                .Where(x => x.Nombre.Contains(nombre))
                .ToListAsync();

            if (autores == null)
            {
                return NotFound();
            }

            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("GUID")]
        //[ResponseCache(Duration = 10)]
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public ActionResult ObtenerGuids()
        {
            return Ok(new
            {
                AutoresControllerScope = servicioScoped.Guid,
                ServicioA_Scope = servicio.ObtenerScope(),
                AutoresControllerSinglenton = servicioSinglenton.Guid,
                ServicioA_Singlenton = servicio.ObtenerSinglenton(),
                AutoresControllerTransient = servicioTransient.Guid,
                ServicioA_Transient = servicio.ObtenerTransient(),
            }); ;
        }

        [HttpPost(Name = "crearAutor")]
        public async Task<IActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {

            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarAutor")]
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {

            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();

        }


        ///<summary>
        /// Borra un autor
        /// </summary>
        /// <param name="id"> Id del autor a Borrar </param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "borrarAutor")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Autores.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }



       [HttpGet("configuraciones")]
        public ActionResult<string> ObtenerConfiguracion()
        {
            return configuration["apellido"];
        }

    }

}
