namespace iPE.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Matches : DbContext
    {
        public Matches()
            : base("name=Match")
        {
        }

        public virtual DbSet<TB_Match> TB_Match { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TB_Match>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_Match>()
                .Property(e => e.sponsor)
                .IsUnicode(false);

            modelBuilder.Entity<TB_Match>()
                .Property(e => e.location)
                .IsUnicode(false);

            modelBuilder.Entity<TB_Match>()
                .Property(e => e.description)
                .IsUnicode(false);
        }
    }
}
