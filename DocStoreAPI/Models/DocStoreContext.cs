using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    public class DocStoreContext : DbContext
    {
        public DbSet<AccessControlEntity> AcessControlEntity { get; set; }
        public DbSet<GroupEntity> GroupEntities { get; set; }
        public DbSet<MetadataEntity> MetadataEntities { get; set; }
        public DbSet<CustomMetadataEntity> CustomMetadataEntities { get; set; }
        public DbSet<BuisnessMetadata> BuisnessMetadata { get; set; }
        public DbSet<DocumentVersionEntity> DocumentVersions { get; set; }
        public DbSet<AccessLogEntity> AccessLogEntities { get; set; }
        public DbSet<BuisnessAreaEntity> BuisnessAreas { get; set; }

        public DbSet<Audit> AuditItems { get; set; }

        public DocStoreContext(DbContextOptions<DocStoreContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MetadataEntity>()
                .HasKey(me => me.Id)
                .ForSqlServerIsClustered();
            modelBuilder.Entity<MetadataEntity>()
                .Property(me => me.Id)
                .UseSqlServerIdentityColumn();
            modelBuilder.Entity<MetadataEntity>()
                .Property(me => me.Name)
                .HasMaxLength(100)
                .IsRequired();
            modelBuilder.Entity<MetadataEntity>()
                .Property(me => me.StorName)
                .HasMaxLength(20)
                .IsRequired();
            modelBuilder.Entity<MetadataEntity>()
                .Property(me => me.Extension)
                .HasMaxLength(10)
                .IsRequired();
            modelBuilder.Entity<MetadataEntity>()
                .Property(me => me.BuisnessArea)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsRequired();

            modelBuilder.Entity<MetadataEntity>()
                .OwnsOne(p => p.Locked);
            modelBuilder.Entity<MetadataEntity>()
                .OwnsOne(p => p.Archive);
            modelBuilder.Entity<MetadataEntity>()
                .OwnsOne(p => p.Created);
            modelBuilder.Entity<MetadataEntity>()
                .OwnsOne(p => p.LastUpdate);

            modelBuilder.Entity<MetadataEntity>()
                .HasMany<BuisnessMetadata>(p => p.BuisnessMetadata)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<MetadataEntity>()
                .HasMany<DocumentVersionEntity>(p => p.Versions)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

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
                .ForSqlServerIsClustered();
            modelBuilder.Entity<AccessControlEntity>()
                .Property(ace => ace.Id)
                .UseSqlServerIdentityColumn();
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
                .ForSqlServerIsClustered();
            modelBuilder.Entity<GroupEntity>()
                .Property(ge => ge.Id)
                .UseSqlServerIdentityColumn();
            modelBuilder.Entity<GroupEntity>()
                .HasIndex(ge => ge.Name)
                .ForSqlServerIsClustered(false)
                .IsUnique();
            modelBuilder.Entity<GroupEntity>()
                .Property(ge => ge.Name)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<DocumentVersionEntity>()
                .HasKey(dve => dve.Id)
                .ForSqlServerIsClustered();
            modelBuilder.Entity<DocumentVersionEntity>()
                .Property(dve => dve.Id)
                .UseSqlServerIdentityColumn();


            modelBuilder.Entity<BuisnessAreaEntity>()
                .HasKey(me => me.Id)
                .ForSqlServerIsClustered();
            modelBuilder.Entity<BuisnessAreaEntity>()
                .Property(me => me.Id)
                .UseSqlServerIdentityColumn();
            modelBuilder.Entity<BuisnessAreaEntity>()
                .HasIndex(bae => bae.Name)
                .ForSqlServerIsClustered(false)
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