using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libros/{libroId:int}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(ApplicationDbContext context,
            IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name = "obtenerComentariosLibro")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId, [FromQuery] PaginacionDTO paginacionDTO)
        {
            //var comentariosDB = await context.Comentarios
            //    .Where(comentarioDB => comentarioDB.LibroId == libroId)
            //    .ToListAsync();

            var queryable = context.Comentarios
                .Where(comentarioDB => comentarioDB.LibroId == libroId)
                .AsQueryable();

            await HttpContext.InsertarParametroPaginacionEnCabecera(queryable);

            var comentarios = await queryable.OrderBy(x => x.Id).Paginar(paginacionDTO).ToListAsync();


            var comentariosMap = mapper.Map<List<ComentarioDTO>>(comentarios);

            return comentariosMap;
        }


        [HttpGet("{id:int}",Name ="ObtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetComentario(int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(comentario => comentario.Id == id);

            if (comentario == null)
            {
                return NotFound();
            }

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return comentarioDTO;
        }



        [HttpPost(Name = "crearComentario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO nuevoComentario)
        {
            //No se puede hacer usar este HttpContext Sin la Etiqueta Authorize
            var emailClaim = HttpContext.User.Claims.Where(claims => claims.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(nuevoComentario);
            comentario.LibroId = libroId;
            comentario.UsuarioId = usuarioId;

            context.Comentarios.Add(comentario);

            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ObtenerComentario", new { id = comentario.Id, libroId }, comentarioDTO);

        }


        [HttpPut("{id}", Name = "actualizarComentario")]
        public async Task<ActionResult> Put(int libroId, ComentarioCreacionDTO comentarioCreacionDTO, int id)
        {
            var existeLibro = await context.Libros.AnyAsync(libro => libro.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }


            var existe = await context.Comentarios.AnyAsync(comentario => comentario.Id == id); 

            if (!existe)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);

            comentario.Id = id;
            comentario.LibroId = libroId;

            context.Update(comentario);
            await context.SaveChangesAsync();

            return NoContent();


        }



    }
}
