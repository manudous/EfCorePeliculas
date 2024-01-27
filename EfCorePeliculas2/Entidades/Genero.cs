using System.ComponentModel.DataAnnotations.Schema;

namespace EfCorePeliculas2.Entidades
{
    //[Table("TablaGeneros", Schema = "peliculas")]
    public class Genero
    {
        //[Key]
        public int Identificador { get; set; }
        //[StringLength(150)]
        //[MaxLength(40)]
        //[Required]
        //[Column("NombreGenero")]
        public string Nombre { get; set; }

        public HashSet<Pelicula> Peliculas { get; set; }
    }
}

