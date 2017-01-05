using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace iPE.Models
{
    public partial class Users : DbContext
    {
        public Users()
            : base("name=User")
        {
        }

        static public int NORMAL_USER = 1;
        static public int MANAGER_USER = 2;

        public virtual DbSet<TB_User> TB_User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelbuilder)
        {
            modelbuilder.Entity<TB_User>()
                .Property(e => e.username)
                .IsUnicode(false);

            modelbuilder.Entity<TB_User>()
                .Property(e => e.pwd)
                .IsUnicode(false);

            modelbuilder.Entity<TB_User>()
                .Property(e => e.phone)
                .IsUnicode(false);

            modelbuilder.Entity<TB_User>()
                .Property(e => e.organization)
                .IsFixedLength()
                .IsUnicode(false);

            modelbuilder.Entity<TB_User>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelbuilder.Entity<TB_User>()
                .Property(e => e.card_id)
                .IsFixedLength()
                .IsUnicode(false);
        }

        public System.Data.Entity.DbSet<iPE.Models.UserLoginModel> UserLoginModels { get; set; }
    }
}