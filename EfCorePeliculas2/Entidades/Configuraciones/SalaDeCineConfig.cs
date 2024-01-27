using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace EfCorePeliculas2.Entidades.Configuraciones
{
    public class SalaDeCineConfig : IEntityTypeConfiguration<SalaDeCine>
    {
        public void Configure(EntityTypeBuilder<SalaDeCine> builder)
        {
            builder.Property(c => c.Precio).HasPrecision(precision: 9, scale: 2);
            builder.Property(c => c.TipoSalaDeCine).HasDefaultValue(TipoSalaDeCine.DosDimensiones);
        }
    }
}
