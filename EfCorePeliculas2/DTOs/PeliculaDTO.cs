namespace EfCorePeliculas2.DTOs
{
    public class PeliculaDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public List<GeneroDTO> Generos { get; set; }
        public ICollection<CineDTO> Cines { get; set; }
        public ICollection<ActorDTO> Actores  { get; set; }
    }
}
