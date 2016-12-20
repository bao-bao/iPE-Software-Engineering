using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace iPE.Models
{
    public partial class Collections : DbContext
    {
        public Collections()
            : base("name=Collection")
        {
        }

        public virtual DbSet<TB_Collection> TB_Match { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelbuilder)
        {
            modelbuilder.Entity<TB_Collection>()
                .Property(e => e.nba_team)
                .IsUnicode(false);

            modelbuilder.Entity<TB_Collection>()
                .Property(e => e.football_team)
                .IsUnicode(false);
        }
    }
}