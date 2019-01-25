using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace product_qc_web.Models
{
    public partial class HexsaveContext : DbContext
    {
        public HexsaveContext()
        {
        }

        public HexsaveContext(DbContextOptions<HexsaveContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TDelivery> TDelivery { get; set; }
        public virtual DbSet<TManufacture> TManufacture { get; set; }
        public virtual DbSet<TProduct> TProduct { get; set; }
        public virtual DbSet<TQualityCheck> TQualityCheck { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=192.168.15.201\\SQLEXPRESS;Initial Catalog=Hexsave;Persist Security Info=True;User ID=webuse;Password=Hex54232885");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<TDelivery>(entity =>
            {
                entity.HasKey(e => new { e.WorkOrderNum, e.MachineNum })
                    .HasName("PK_Delivery");

                entity.ToTable("T_Delivery");

                entity.Property(e => e.WorkOrderNum)
                    .HasColumnName("Work_Order_Num")
                    .HasColumnType("numeric(12, 0)");

                entity.Property(e => e.MachineNum)
                    .HasColumnName("Machine_Num")
                    .HasColumnType("numeric(3, 0)");

                entity.Property(e => e.DeliveryDestination)
                    .HasColumnName("Delivery_Destination")
                    .HasMaxLength(50);

                entity.Property(e => e.ExchangeReturnMalfunctionNote)
                    .HasColumnName("Exchange_Return_Malfunction_Note")
                    .HasMaxLength(255);

                entity.Property(e => e.LastModifiedTime)
                    .HasColumnName("Last_Modified_Time")
                    .HasColumnType("date");

                entity.HasOne(d => d.TManufacture)
                    .WithOne(p => p.TDelivery)
                    .HasForeignKey<TDelivery>(d => new { d.WorkOrderNum, d.MachineNum })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_T_Delivery_T_Manufacture");
            });

            modelBuilder.Entity<TManufacture>(entity =>
            {
                entity.HasKey(e => new { e.WorkOrderNum, e.MachineNum });

                entity.ToTable("T_Manufacture");

                entity.Property(e => e.WorkOrderNum)
                    .HasColumnName("Work_Order_Num")
                    .HasColumnType("numeric(12, 0)");

                entity.Property(e => e.MachineNum)
                    .HasColumnName("Machine_Num")
                    .HasColumnType("numeric(3, 0)");

                entity.Property(e => e.ProductCode)
                    .HasColumnName("Product_Code")
                    .HasColumnType("numeric(3, 0)");

                entity.HasOne(d => d.ProductCodeNavigation)
                    .WithMany(p => p.TManufacture)
                    .HasForeignKey(d => d.ProductCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_T_Manufacture_T_Product");
            });

            modelBuilder.Entity<TProduct>(entity =>
            {
                entity.HasKey(e => e.ProductCode);

                entity.ToTable("T_Product");

                entity.Property(e => e.ProductCode)
                    .HasColumnName("Product_Code")
                    .HasColumnType("numeric(3, 0)");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasColumnName("Product_Name")
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<TQualityCheck>(entity =>
            {
                entity.HasKey(e => new { e.WorkOrderNum, e.MachineNum })
                    .HasName("PK_QualityCheck");

                entity.ToTable("T_QualityCheck");

                entity.Property(e => e.WorkOrderNum)
                    .HasColumnName("Work_Order_Num")
                    .HasColumnType("numeric(12, 0)");

                entity.Property(e => e.MachineNum)
                    .HasColumnName("Machine_Num")
                    .HasColumnType("numeric(3, 0)");

                entity.Property(e => e.QcFinishedTime)
                    .HasColumnName("QC_Finished_Time")
                    .HasColumnType("date");

                entity.HasOne(d => d.TManufacture)
                    .WithOne(p => p.TQualityCheck)
                    .HasForeignKey<TQualityCheck>(d => new { d.WorkOrderNum, d.MachineNum })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_T_QualityCheck_T_Manufacture");
            });
        }
    }
}
