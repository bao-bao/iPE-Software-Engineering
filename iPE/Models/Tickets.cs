namespace iPE.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Tickets : DbContext
    {
        public Tickets()
            : base("name=Tickets")
        {
        }

        public virtual DbSet<TB_Ticket> TB_Tickets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TB_Ticket>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_Ticket>()
                .Property(e => e.description)
                .IsUnicode(false);
        }
    }
}
