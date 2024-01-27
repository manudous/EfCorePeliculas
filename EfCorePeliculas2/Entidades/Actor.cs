namespace EfCorePeliculas2.Entidades
{
    public class Actor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Biografia { get; set; }
        //[Column(TypeName = "date")]
        public DateTime? FechaNacimiento { get; set; }

        // navigation properties

        public HashSet<PeliculaActor> PeliculasActores { get; set; }
    }
}
