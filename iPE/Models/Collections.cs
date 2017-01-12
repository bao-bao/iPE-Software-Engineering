namespace iPE.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Collections : DbContext
    {
        public Collections()
            : base("name=Collection")
        {
        }

        public virtual DbSet<TB_Collection> TB_Collection { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TB_Collection>()
                .Property(e => e.teamname)
                .IsUnicode(false);
        }
    }
}
