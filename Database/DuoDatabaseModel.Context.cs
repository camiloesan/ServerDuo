﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Database
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DuoContext : DbContext
    {
        public DuoContext()
            : base("name=DuoContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
<<<<<<<< HEAD:Database/DuoDatabaseModel.Context.cs
        public virtual DbSet<FriendRequest> FriendRequests { get; set; }
        public virtual DbSet<Friendship> Friendships { get; set; }
        public virtual DbSet<UserBlock> UserBlocks { get; set; }
        public virtual DbSet<User> Users { get; set; }
========
        public virtual DbSet<FriendRequests> FriendRequests { get; set; }
        public virtual DbSet<Friendships> Friendships { get; set; }
        public virtual DbSet<UserBlocks> UserBlocks { get; set; }
        public virtual DbSet<Users> Users { get; set; }
>>>>>>>> FinalBranchXavi(Don't-Touch):Database/DuoContext.Context.cs
    }
}
