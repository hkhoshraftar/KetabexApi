﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KetabexApi.DataModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class KetabexDbEntities : DbContext
    {
        public KetabexDbEntities()
            : base("name=KetabexDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Follow> Follow { get; set; }
        public virtual DbSet<Like> Like { get; set; }
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAuth> UserAuth { get; set; }
        public virtual DbSet<UserKey> UserKey { get; set; }
        public virtual DbSet<UserSession> UserSession { get; set; }
    }
}
