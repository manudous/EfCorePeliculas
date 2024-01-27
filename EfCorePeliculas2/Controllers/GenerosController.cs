using EfCorePeliculas2.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCorePeliculas2.Controllers
{
    [Route("api/generos")]
    [ApiController]
    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public GenerosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genero>>> Get()
        {
            return await context.Generos.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Genero>> Get(int id)
        {
            var genero = await context.Generos.FirstOrDefaultAsync(g => g.Identificador == id);

            if (genero is null)
            {
                return NotFound();
            }
            return Ok(genero);
        }

        [HttpGet("primer")]
        public async Task<ActionResult<Genero>> GetFirst()
        {
            var genero = await context.Generos.FirstOrDefaultAsync(g => g.Nombre.StartsWith("C"));

            if (genero is null)
            {
                return NotFound();
            }
            return Ok(genero);
        }

        [HttpGet("filtrar")]
        public async Task<ActionResult<IEnumerable<Genero>>> Filtrar(string nombre)
        {
            return await context.Generos
                .Where(g => g.Nombre.Contains(nombre))
                .OrderBy(g => g.Nombre)
                .ToListAsync();
        }

        [HttpGet("paginacion")]
        public async Task<ActionResult<IEnumerable<Genero>>> GetPaginacion(int pagina = 1)
        {
            var cantidadRegistrosPorPagina = 2;
            var generos = await context.Generos
                .Skip((pagina - 1)*cantidadRegistrosPorPagina)
                .Take(cantidadRegistrosPorPagina)
                .ToListAsync();

            return generos;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Genero genero)
        {
            var estatus1 = context.Entry(genero).State;
            context.Add(genero);
            var estatus2 = context.Entry(genero).State;
            await context.SaveChangesAsync();
            var estatus3 = context.Entry(genero).State;
            return Ok();
        }

        [HttpPost("varios")]
        public async Task<ActionResult> Post([FromBody] Genero[] generos)
        {
            context.AddRange(generos);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
