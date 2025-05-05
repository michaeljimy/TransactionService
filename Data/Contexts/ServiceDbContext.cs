using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Transaction_Service.Data.Entities;
using Transaction_Service.Utils;

namespace Transaction_Service.Data.Contexts
{
    public class ServiceDbContext : DbContext
    {

        public DbSet<TransactionTypeEntity> TransactionTypes { get; set; }

        public DbSet<TransactionEntity> Transactions { get; set; }

        public DbSet<TransactionEventEntity> TransactionEvents { get; set; }

        public DbSet<TransactionLogEntity> TransactionLogs { get; set; }

        public DbSet<RefundTransactionEntity> RefundTransactions { get; set; }

        public ServiceDbContext(DbContextOptions<ServiceDbContext> dbContextOptions) : base(dbContextOptions)
        {
            try
            {
                var databaseCreator = Database.GetService<IRelationalDatabaseCreator>() as RelationalDatabaseCreator;
                if (databaseCreator != null)
                {
                    if (!databaseCreator.CanConnect()) databaseCreator.Create();
                    if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure TransactionEntity.TransactionTypeId as an alternate key.
            modelBuilder.Entity<TransactionEntity>()
                .HasOne(te => te.TransactionType)
                .WithMany()  // Or with an appropriate collection if defined in TransactionEntity.
                .HasForeignKey(te => te.TransactionTypeId)
                .HasPrincipalKey(t => t.TransactionTypeId);

            // Configure the relationship where TransactionEventEntity.TransactionId 
            // is a foreign key referencing TransactionEntity.TransactionTypeId.
            modelBuilder.Entity<TransactionEventEntity>()
                .HasOne(te => te.Transaction)
                .WithMany()  // Or with an appropriate collection if defined in TransactionEntity.
                .HasForeignKey(te => te.TransactionId)
                .HasPrincipalKey(t => t.TransactionId);

            modelBuilder.Entity<TransactionLogEntity>()
                .HasOne(te => te.Transaction)
                .WithMany()  // Or with an appropriate collection if defined in TransactionEntity.
                .HasForeignKey(te => te.TransactionId)
                .HasPrincipalKey(t => t.TransactionId);

            modelBuilder.Entity<RefundTransactionEntity>()
                .HasOne(te => te.OriginalTransaction)
                .WithMany()  // Or with an appropriate collection if defined in TransactionEntity.
                .HasForeignKey(te => te.OriginalTransactionId)
                .HasPrincipalKey(t => t.TransactionId);


        }

    }
}