using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Smart_Mart
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string? Name { get; set; }
        public string? City { get; set; }
        public ICollection<SalesRecord> SalesRecords { get; set; } = new List<SalesRecord>();
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Category { get; set; }
        public ICollection<SalesRecord> SalesRecords { get; set; } = new List<SalesRecord>();
    }

    public class SalesRecord
    {
        public int SaleId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public DateTime? SaleDate { get; set; }

        public Customer? Customer { get; set; }
        public Product? Product { get; set; }
    }

    public class SmartMartContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SalesRecord> SalesRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SmartMartDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SalesRecord>()
                .HasKey(s => s.SaleId);

            modelBuilder.Entity<SalesRecord>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.SalesRecords)
                .HasForeignKey(s => s.CustomerId);

            modelBuilder.Entity<SalesRecord>()
                .HasOne(s => s.Product)
                .WithMany(p => p.SalesRecords)
                .HasForeignKey(s => s.ProductId);
        }
    }
}