using AutoMapper;
using AutoMapper.QueryableExtensions;
using EfCorePeliculas2.DTOs;
using EfCorePeliculas2.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCorePeliculas2.Controllers
{
    [Route("api/peliculas")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public PeliculasController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PeliculaDTO>> Get(int id)
        {
            var pelicula = await context.Peliculas
                .Include(p => p.Generos.OrderByDescending(g => g.Nombre))
                .Include(p=> p.SalasDeCine)
                    .ThenInclude(sc => sc.Cine)
                .Include(p => p.PeliculasActores.Where(pa => pa.Actor.FechaNacimiento.Value.Year >= 1980))
                    .ThenInclude(pa => pa.Actor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
            {
                return NotFound();
            }

            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);

            peliculaDTO.Cines = peliculaDTO.Cines.DistinctBy(c => c.Id).ToList();

            return peliculaDTO;
        }

        [HttpGet("conprojectto/{id:int}")]
        public async Task<ActionResult<PeliculaDTO>> GetProjectTo(int id)
        {
            var pelicula = await context.Peliculas
                .ProjectTo<PeliculaDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
            {
                return NotFound();
            }

            pelicula.Cines = pelicula.Cines.DistinctBy(c => c.Id).ToList();

            return pelicula;
        }

        [HttpGet("cargadoselectivo/{id:int}")]
        public async Task<ActionResult> GetSelectivo(int id)
        {
            var pelicula = await context.Peliculas.Select(p =>
            new
            {
                Id = p.Id,
                Titulo = p.Titulo,
                Generos = p.Generos.OrderByDescending(g => g.Nombre).Select(g => g.Nombre).ToList(),
                CantidadAutores = p.PeliculasActores.Count(),
                CantidadCines = p.SalasDeCine.Select(s => s.CineId).Distinct().Count()
            }).FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
            {
                return NotFound();
            }

            return Ok(pelicula);
        }

        [HttpGet("cargadoexplicito/{id:int}")]
        public async Task<ActionResult<PeliculaDTO>> GetExplicito(int id)
        {
            var pelicula = await context.Peliculas.AsTracking().FirstOrDefaultAsync(p => p.Id == id);
            
            // Carga explícita, se realizan dos consultas a la base de datos.
            await context.Entry(pelicula)
                .Collection(p => p.Generos)
                .LoadAsync();

            // Contar la cantidad de géneros de la película, no es muy eficiente porque se realizan más consultas a la base de datos.
            var cantidadGeneros = await context.Entry(pelicula)
                .Collection(p => p.Generos)
                .Query()
                .CountAsync();

            if (pelicula is null)
            {
                return NotFound();
            }
            
            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return Ok(peliculaDTO);
        }

        [HttpGet("lazyloading/{id:int}")]
        public async Task<ActionResult<List<PeliculaDTO>>> GetLazyLoading()
        {
            var peliculas = await context.Peliculas.AsTracking().ToListAsync();

            foreach (var pelicula in peliculas)
            {
                // cargar los generos de la pelicula

                // problema n + 1
                pelicula.Generos.ToList();
            }

            var peliculasDTO = mapper.Map<List<PeliculaDTO>>(peliculas);
 
            return Ok(peliculasDTO);
        }

        [HttpGet("agrupadasPorEstreno")]
        public async Task<ActionResult<List<PeliculaDTO>>> GetAgrupadasPorCartelera()
        {
            var peliculasAgrupadas = await context.Peliculas
                .GroupBy(p => p.EnCartelera)
                .Select(g => new
                {
                    EnCartelera = g.Key,
                    Conteo = g.Count(),
                    Peliculas = g.ToList()
                }).ToListAsync();

            return Ok(peliculasAgrupadas);
        }

        [HttpGet("agrupadasPorCantidadDeGeneros")]
        public async Task<ActionResult<List<PeliculaDTO>>> GetAgrupadasPorCantidadDeGeneros()
        {
            var peliculasAgrupadas = await context.Peliculas
                .GroupBy(p => p.Generos.Count())
                .Select(g => new
                {
                    Conteo = g.Key,
                    Titulos = g.Select(p => p.Titulo),
                    Generos = g.Select(p => p.Generos)
                            .SelectMany(gen => gen)
                            .Select(gen => gen.Nombre)
                            .Distinct()
                            .ToList()
                }).ToListAsync();

            return Ok(peliculasAgrupadas);
        }

        [HttpGet("filtrar")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] PeliculasFiltroDTO peliculasFiltroDTO)
        {
           var peliculasQueryable = context.Peliculas.AsQueryable();

            if (!string.IsNullOrEmpty(peliculasFiltroDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.Titulo.Contains(peliculasFiltroDTO.Titulo));
            }

            if (peliculasFiltroDTO.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.EnCartelera);
            }

            if (peliculasFiltroDTO.ProximosEstrenos)
            {
                var hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(p => p.FechaEstreno > hoy);
            }

            if (peliculasFiltroDTO.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable
                    .Where(p => p.Generos.Select(g => g.Identificador)
                                       .Contains(peliculasFiltroDTO.GeneroId));
            }

            var peliculas = await peliculasQueryable.Include(p => p.Generos).ToListAsync();

            return mapper.Map<List<PeliculaDTO>>(peliculas);
        }       
    }
}
