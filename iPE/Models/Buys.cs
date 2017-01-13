namespace iPE.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Buys : DbContext
    {
        public Buys()
            : base("name=Buy")
        {
        }

        public virtual DbSet<TB_Buy> TB_Buy { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
