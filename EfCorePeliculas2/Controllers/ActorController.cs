using AutoMapper;
using AutoMapper.QueryableExtensions;
using EfCorePeliculas2.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCorePeliculas2.Controllers
{
    [Route("api/actores")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ActorController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActorDTO>>> Get()
        {
            //return await context.Actores
            //    .Select(a => new ActorDTO { Id = a.Id, Nombre = a.Nombre })
            //    .ToListAsync();
            return await context.Actores
                .ProjectTo<ActorDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
