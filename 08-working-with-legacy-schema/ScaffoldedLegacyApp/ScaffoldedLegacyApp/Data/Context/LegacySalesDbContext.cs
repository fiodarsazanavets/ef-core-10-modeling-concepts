using System;
using System.Collections.Generic;
using ContosoLegacySales.Data.Scaffolded.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContosoLegacySales.Data.Context;

public partial class LegacySalesDbContext : DbContext
{
    public LegacySalesDbContext(DbContextOptions<LegacySalesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer_Address> Customer_Addresses { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<OrderDtl> OrderDtls { get; set; }

    public virtual DbSet<OrderHdr> OrderHdrs { get; set; }

    public virtual DbSet<ProductCatalog> ProductCatalogs { get; set; }

    public virtual DbSet<ProductSupplier> ProductSuppliers { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<lkp_OrderStatus> lkp_OrderStatuses { get; set; }

    public virtual DbSet<tblCustomer> tblCustomers { get; set; }

    public virtual DbSet<vw_OrderSummary> vw_OrderSummaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer_Address>(entity =>
        {
            entity.HasKey(e => e.AddrId);

            entity.ToTable("Customer_Address", "crm");

            entity.Property(e => e.AddrLine1)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.AddrLine2)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.AddrTypeCd)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CityNm)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CountryNm)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsPrimaryFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("N", "DF_Customer_Address_IsPrimaryFlag");
            entity.Property(e => e.PostalCd)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StateProv)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Cust).WithMany(p => p.Customer_Addresses)
                .HasForeignKey(d => d.CustId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Customer_Address_tblCustomer");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmpID);

            entity.ToTable("Employee", "crm");

            entity.HasIndex(e => e.EmpNo, "UQ_Employee_EmpNo").IsUnique();

            entity.Property(e => e.EmailAddr)
                .HasMaxLength(320)
                .IsUnicode(false);
            entity.Property(e => e.EmpNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FullNm)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.HireDt).HasColumnType("datetime");
            entity.Property(e => e.IsSalesRepFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("N", "DF_Employee_IsSalesRepFlag");
            entity.Property(e => e.TerminatedDt).HasColumnType("datetime");

            entity.HasOne(d => d.ManagerEmp).WithMany(p => p.InverseManagerEmp)
                .HasForeignKey(d => d.ManagerEmpID)
                .HasConstraintName("FK_Employee_Manager");
        });

        modelBuilder.Entity<OrderDtl>(entity =>
        {
            entity.HasKey(e => new { e.OrderID, e.LineNum });

            entity.ToTable("OrderDtl", "sales");

            entity.HasIndex(e => e.ProductID, "IX_OrderDtl_ProductID");

            entity.Property(e => e.LineDiscountAmt)
                .HasDefaultValue(0m, "DF_OrderDtl_LineDiscountAmt")
                .HasColumnType("money");
            entity.Property(e => e.UnitPriceAmt).HasColumnType("money");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDtls)
                .HasForeignKey(d => d.OrderID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDtl_OrderHdr");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDtls)
                .HasForeignKey(d => d.ProductID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDtl_ProductCatalog");
        });

        modelBuilder.Entity<OrderHdr>(entity =>
        {
            entity.HasKey(e => e.OrderID);

            entity.ToTable("OrderHdr", "sales");

            entity.HasIndex(e => e.CustId, "IX_OrderHdr_CustId");

            entity.HasIndex(e => e.SalesRepEmpID, "IX_OrderHdr_SalesRepEmpID");

            entity.HasIndex(e => e.StatusCode, "IX_OrderHdr_StatusCode");

            entity.HasIndex(e => e.OrderNo, "UQ_OrderHdr_OrderNo").IsUnique();

            entity.Property(e => e.Comments).HasColumnType("text");
            entity.Property(e => e.ExternalRefNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HeaderDiscountAmt)
                .HasDefaultValue(0m, "DF_OrderHdr_HeaderDiscountAmt")
                .HasColumnType("money");
            entity.Property(e => e.LegacySourceSystem)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.OrderNo)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.OrderedOn)
                .HasDefaultValueSql("(getdate())", "DF_OrderHdr_OrderedOn")
                .HasColumnType("datetime");
            entity.Property(e => e.RequiredByDt).HasColumnType("datetime");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.BillToAddr).WithMany(p => p.OrderHdrBillToAddrs)
                .HasForeignKey(d => d.BillToAddrId)
                .HasConstraintName("FK_OrderHdr_BillToAddr");

            entity.HasOne(d => d.Cust).WithMany(p => p.OrderHdrs)
                .HasForeignKey(d => d.CustId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderHdr_tblCustomer");

            entity.HasOne(d => d.SalesRepEmp).WithMany(p => p.OrderHdrs)
                .HasForeignKey(d => d.SalesRepEmpID)
                .HasConstraintName("FK_OrderHdr_Employee");

            entity.HasOne(d => d.ShipToAddr).WithMany(p => p.OrderHdrShipToAddrs)
                .HasForeignKey(d => d.ShipToAddrId)
                .HasConstraintName("FK_OrderHdr_ShipToAddr");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.OrderHdrs)
                .HasForeignKey(d => d.StatusCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderHdr_Status");
        });

        modelBuilder.Entity<ProductCatalog>(entity =>
        {
            entity.HasKey(e => e.ProductID);

            entity.ToTable("ProductCatalog", "sales");

            entity.HasIndex(e => e.SKU, "UQ_ProductCatalog_SKU").IsUnique();

            entity.Property(e => e.DownloadUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsDiscontinuedFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("N", "DF_ProductCatalog_IsDiscontinuedFlag");
            entity.Property(e => e.NotesTxt).HasColumnType("text");
            entity.Property(e => e.ProductNm)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ProductTypeCd)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.SKU)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.UnitPriceAmt).HasColumnType("money");
            entity.Property(e => e.WeightKg).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<ProductSupplier>(entity =>
        {
            entity.HasKey(e => new { e.ProductID, e.SupplierCode });

            entity.ToTable("ProductSupplier", "sales");

            entity.Property(e => e.SupplierCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PreferredFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("N", "DF_ProductSupplier_PreferredFlag");
            entity.Property(e => e.SupplierNm)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductSuppliers)
                .HasForeignKey(d => d.ProductID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductSupplier_ProductCatalog");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionCd);

            entity.ToTable("Region", "ref");

            entity.Property(e => e.RegionCd)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.RegionName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<lkp_OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusCode);

            entity.ToTable("lkp_OrderStatus", "ref");

            entity.Property(e => e.StatusCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IsActiveFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("Y", "DF_lkp_OrderStatus_IsActiveFlag");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tblCustomer>(entity =>
        {
            entity.HasKey(e => e.CustId);

            entity.ToTable("tblCustomer", "crm");

            entity.HasIndex(e => e.RegionCd, "IX_tblCustomer_RegionCd");

            entity.HasIndex(e => e.CustCode, "UQ_tblCustomer_CustCode").IsUnique();

            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())", "DF_tblCustomer_CreatedOn")
                .HasColumnType("datetime");
            entity.Property(e => e.CreditLimitAmt).HasColumnType("money");
            entity.Property(e => e.CustCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Cust_Nm)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IsPreferred)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("N", "DF_tblCustomer_IsPreferred");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.PrimaryEmailAddr)
                .HasMaxLength(320)
                .IsUnicode(false);
            entity.Property(e => e.RegionCd)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.rv)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.RegionCdNavigation).WithMany(p => p.tblCustomers)
                .HasForeignKey(d => d.RegionCd)
                .HasConstraintName("FK_tblCustomer_Region");
        });

        modelBuilder.Entity<vw_OrderSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_OrderSummary", "sales");

            entity.Property(e => e.Cust_Nm)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.NetAmount).HasColumnType("money");
            entity.Property(e => e.OrderNo)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.OrderedOn).HasColumnType("datetime");
            entity.Property(e => e.SalesRepName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.StatusCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
