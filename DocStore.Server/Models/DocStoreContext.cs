﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocStore.Server.Shared;
using DocStore.Shared.Models;

namespace DocStore.Server.Models
{
    public class DocStoreContext : DbContext
    {
        public DbSet<AccessControlEntity> AcessControlEntity { get; set; }
        public DbSet<GroupEntity> GroupEntities { get; set; }
        public DbSet<MetadataEntity> MetadataEntities { get; set; }
        public DbSet<BuisnessMetadata> BuisnessMetadata { get; set; }
        public DbSet<DocumentVersionEntity> DocumentVersions { get; set; }
        public DbSet<AccessLogEntity> AccessLogEntities { get; set; }
        public DbSet<BuisnessAreaEntity> BuisnessAreas { get; set; }
        public DbSet<SearchLogEntity> SearchLogs { get; set; }

        public DbSet<Audit> AuditItems { get; set; }

        public DocStoreContext(DbContextOptions<DocStoreContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MetadataEntity>(me =>
            {
                me.HasKey("Id").IsClustered();
                me.Property("Id").UseIdentityColumn();

                me.Property("Name").HasMaxLength(100).IsRequired();

                me.Property("StorName").HasMaxLength(20).IsRequired();

                me.Property("Extension").HasMaxLength(10).IsRequired();

                me.Property("BuisnessArea").HasMaxLength(20).IsUnicode(false).IsRequired();

                me.OwnsOne(p => p.Locked);
                me.OwnsOne(p => p.Archive);
                me.OwnsOne(p => p.Created);
                me.OwnsOne(p => p.LastUpdate);
            });



            modelBuilder.Entity<LockState>()
                .Property(ls => ls.By)
                .IsRequired(false);
            modelBuilder.Entity<LockState>()
                .Property(ls => ls.At)
                .IsRequired(false);
            modelBuilder.Entity<LockState>()
                .Property(ls => ls.Expiration)
                .IsRequired(false);

            modelBuilder.Entity<ArchiveState>()
                .Property(ar => ar.By)
                .IsRequired(false);
            modelBuilder.Entity<ArchiveState>()
                .Property(ar => ar.At)
                .IsRequired(false);

            modelBuilder.Entity<AccessControlEntity>()
                .HasKey(ace => ace.Id)
                .IsClustered();
            modelBuilder.Entity<AccessControlEntity>()
                .Property(ace => ace.Id)
                .UseIdentityColumn();
            modelBuilder.Entity<AccessControlEntity>()
                .Property(ace => ace.Group)
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode(false);
            modelBuilder.Entity<AccessControlEntity>()
                .Property(ace => ace.BusinessArea)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsRequired();

            modelBuilder.Entity<GroupEntity>()
                .HasKey(ge => ge.Id)
                .IsClustered();
            modelBuilder.Entity<GroupEntity>()
                .Property(ge => ge.Id)
                .UseIdentityColumn();
            modelBuilder.Entity<GroupEntity>()
                .HasIndex(ge => ge.Name)
                .IsClustered(false)
                .IsUnique();
            modelBuilder.Entity<GroupEntity>()
                .Property(ge => ge.Name)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<DocumentVersionEntity>()
                .HasKey(dve => dve.Id)
                .IsClustered();
            modelBuilder.Entity<DocumentVersionEntity>()
                .Property(dve => dve.Id)
                .UseIdentityColumn();


            modelBuilder.Entity<BuisnessAreaEntity>()
                .HasKey(me => me.Id)
                .IsClustered();
            modelBuilder.Entity<BuisnessAreaEntity>()
                .Property(me => me.Id)
                .UseIdentityColumn();
            modelBuilder.Entity<BuisnessAreaEntity>()
                .HasIndex(bae => bae.Name)
                .IsClustered(false)
                .IsUnique();
            modelBuilder.Entity<BuisnessAreaEntity>()
                .Property(bae => bae.Name)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsRequired();
        }

        public void RejectChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }


        public void RejectModifications()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
        }
        }
    }