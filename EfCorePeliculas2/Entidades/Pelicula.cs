namespace EfCorePeliculas2.Entidades
{
    public class Pelicula
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public bool EnCartelera { get; set; }
        public DateTime FechaEstreno { get; set; }
        //[Unicode(false)]
        public string PosterURL { get; set; }

        // navigation properties
        public List<Genero> Generos { get; set; }

        public HashSet<SalaDeCine> SalasDeCine { get; set; }

        public HashSet<PeliculaActor> PeliculasActores { get; set; }
    }
}
