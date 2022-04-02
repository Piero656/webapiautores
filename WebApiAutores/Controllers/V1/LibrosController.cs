using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores;
using WebApiAutores.Entidades;
using WebApiAutores.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

namespace WebApiAutores.Controllers.V1
{
    [Route("api/v1/libros")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        // GET: api/Libros
        [HttpGet(Name = "obtenerLibros")]
        public async Task<ActionResult<IEnumerable<LibroDTO>>> GetLibros()
        {
            var libros = await _context.Libros
                .Include(librosBD => librosBD.Comentarios)
                .Include(libroBD => libroBD.AutoresLibros)
                    .ThenInclude(autorlibro => autorlibro.Autor)
                .ToListAsync();

            return mapper.Map<List<LibroDTO>>(libros);
        }

        // GET: api/Libros/5
        [HttpGet("{id:int}",Name ="obtenerLibro")]
        public async Task<ActionResult<LibroDTO>> GetLibro(int id)
        {
            var libro = await _context.Libros
                .Include(libroDB => libroDB.Comentarios)
                .Include(libroDB => libroDB.AutoresLibros)
                    .ThenInclude(autorlibro => autorlibro.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();



            return mapper.Map<LibroDTO>(libro);
        }

        // PUT: api/Libros/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}", Name = "actualizarLibro")]
        public async Task<ActionResult> PutLibro(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDB = await _context.Libros
                 .Include(x => x.AutoresLibros)
                 .FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null)
            {
                return NotFound();
            }

            libroDB = mapper.Map(libroCreacionDTO, libroDB);

            AsignarOrdenAutores(libroDB);

            await _context.SaveChangesAsync();
            return NoContent();


        }

        [HttpPatch("{id:int}", Name = "patchLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var libroDB = await _context.Libros.FirstOrDefaultAsync(libro => libro.Id == id);

            var libroDTO = mapper.Map<LibroPatchDTO>(libroDB);

            patchDocument.ApplyTo(libroDTO, ModelState);

            var esValido = TryValidateModel(libroDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDTO, libroDB);

            await _context.SaveChangesAsync();
            return NoContent();


        }




        // POST: api/Libros
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost(Name = "crearLibro")]
        public async Task<ActionResult<LibroCreacionDTO>> PostLibro(LibroCreacionDTO libro)
        {
            //var existeAutor = await _context.Autores.AnyAsync(x => x.Id == libro.AutorId);

            //if (!existeAutor)
            //{
            //    return BadRequest($"No exite el autor de id: {libro.AutorId}");
            //}


            if (libro.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }

            var autoresIds = await _context.Autores.Where(autorBD => libro.AutoresIds.Contains(autorBD.Id))
                .Select(x => x.Id).ToListAsync();

            if (libro.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No Existe uno de los autores enviados");
            }

            

            var nuevoLibro = mapper.Map<Libro>(libro);

            AsignarOrdenAutores(nuevoLibro);


            _context.Add(nuevoLibro);
            await _context.SaveChangesAsync();

            var libroCreado = await _context.Libros
                .Include(libroDB => libroDB.Comentarios)
                .Include(libro => libro.AutoresLibros)
                    .ThenInclude(autorlibro => autorlibro.Autor)
                .FirstOrDefaultAsync(libro => libro.Id == nuevoLibro.Id);

            var libroDTO = mapper.Map<LibroDTO>(libroCreado);


            return CreatedAtRoute("obtenerLibro", new { id = nuevoLibro.Id }, libroDTO);
        }

        // DELETE: api/Libros/5
        [HttpDelete("{id}", Name = "eliminarLibro")]
        public async Task<IActionResult> DeleteLibro(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }

            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }

        }
    }
}
