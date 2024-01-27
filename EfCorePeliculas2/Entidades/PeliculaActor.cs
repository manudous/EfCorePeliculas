namespace EfCorePeliculas2.Entidades
{
    public class PeliculaActor
    {
        public int PeliculaId { get; set; }
        public int ActorId { get; set; }
        public string Personaje { get; set; }
        public int Orden { get; set; }

        // navigation properties
        public Pelicula Pelicula { get; set; }
        public Actor Actor { get; set; }
    }
}
