using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementSystem.Models;

public partial class AssetContext : DbContext
{
    public AssetContext()
    {
    }

    public AssetContext(DbContextOptions<AssetContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppAssignment> AppAssignments { get; set; }

    public virtual DbSet<AppBorrow> AppBorrows { get; set; }

    public virtual DbSet<AppCost> AppCosts { get; set; }

    public virtual DbSet<AppMaitenance> AppMaitenances { get; set; }

    public virtual DbSet<AppRequisition> AppRequisitions { get; set; }

    public virtual DbSet<AppTracker> AppTrackers { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<DtCategory> DtCategories { get; set; }

    public virtual DbSet<DtCategoryType> DtCategoryTypes { get; set; }

    public virtual DbSet<DtCondition> DtConditions { get; set; }

    public virtual DbSet<DtCurrency> DtCurrencies { get; set; }

    public virtual DbSet<DtDistrict> DtDistricts { get; set; }

    public virtual DbSet<DtEmployee> DtEmployees { get; set; }

    public virtual DbSet<DtLocation> DtLocations { get; set; }

    public virtual DbSet<DtMake> DtMakes { get; set; }

    public virtual DbSet<DtProject> DtProjects { get; set; }

    public virtual DbSet<DtProvince> DtProvinces { get; set; }

    public virtual DbSet<DtStatus> DtStatuses { get; set; }

    public virtual DbSet<DtStatusType> DtStatusTypes { get; set; }

    public virtual DbSet<DtStock> DtStocks { get; set; }

    public virtual DbSet<DtUser> DtUsers { get; set; }

    public virtual DbSet<DtUserRole> DtUserRoles { get; set; }

    public virtual DbSet<StockItemsV> StockItemsVs { get; set; }

    public virtual DbSet<UserV> UserVs { get; set; }

    public virtual DbSet<VwDttag> VwDttags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=sql.cidrz.org;Initial Catalog=ASSET;Persist Security Info=True;User ID=sa;Password=Abc123;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppAssignment>(entity =>
        {
            entity.ToTable("APP_ASSIGNMENT", tb =>
                {
                    tb.HasTrigger("trg_Insert_BORROW_Cleanup");
                    tb.HasTrigger("trg_Insert_Stock_Cleanup");
                });

            entity.Property(e => e.Comments)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateAssigned)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<AppBorrow>(entity =>
        {
            entity.ToTable("APP_BORROW");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<AppCost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_APP_DEPRECIATION");

            entity.ToTable("APP_COST");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InitialCostUsd)
                .HasColumnType("money")
                .HasColumnName("InitialCostUSD");
            entity.Property(e => e.InitialCostZmw)
                .HasColumnType("money")
                .HasColumnName("InitialCostZMW");
            entity.Property(e => e.SalvageValueUsd)
                .HasColumnType("money")
                .HasColumnName("SalvageValueUSD");
            entity.Property(e => e.SalvageValueZmw)
                .HasColumnType("money")
                .HasColumnName("SalvageValueZMW");
        });

        modelBuilder.Entity<AppMaitenance>(entity =>
        {
            entity.ToTable("APP_MAITENANCE");

            entity.Property(e => e.CheckInDate)
                .HasColumnType("datetime")
                .HasColumnName("checkInDate");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Diagnosis)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.ReturnDate)
                .HasColumnType("datetime")
                .HasColumnName("returnDate");
            entity.Property(e => e.SerialNo)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("serialNo");
            entity.Property(e => e.Status)
                .HasDefaultValue(0)
                .HasColumnName("status");
            entity.Property(e => e.Ticket)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("ticket");
        });

        modelBuilder.Entity<AppRequisition>(entity =>
        {
            entity.ToTable("APP_REQUISITION");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DispatchNo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");
            entity.Property(e => e.RecordStatusId).HasDefaultValue(7);
        });

        modelBuilder.Entity<AppTracker>(entity =>
        {
            entity.ToTable("APP_TRACKER");

            entity.Property(e => e.ActionPerformed)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("APP_USER");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.RecordStatusId).HasDefaultValue(7);
        });

        modelBuilder.Entity<DtCategory>(entity =>
        {
            entity.ToTable("DT_CATEGORY");

            entity.Property(e => e.Category)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.RecordStatusId).HasDefaultValue(7);
            entity.Property(e => e.TypeId)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DtCategoryType>(entity =>
        {
            entity.ToTable("DT_CATEGORY_TYPE");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DtCondition>(entity =>
        {
            entity.ToTable("DT_CONDITION");

            entity.Property(e => e.Condition)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.RecordStatusId).HasDefaultValue(7);
        });

        modelBuilder.Entity<DtCurrency>(entity =>
        {
            entity.ToTable("DT_CURRENCY");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Currency)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.RecordStatusId).HasDefaultValue(7);
        });

        modelBuilder.Entity<DtDistrict>(entity =>
        {
            entity.ToTable("DT_DISTRICT");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.District)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.RecordStatusId).HasDefaultValue(7);
        });

        modelBuilder.Entity<DtEmployee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Employee2");

            entity.ToTable("DT_EMPLOYEE");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.JobTitle)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdate).HasColumnType("datetime");
            entity.Property(e => e.Nrc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NRC");
            entity.Property(e => e.RecordStatusId).HasDefaultValue(7);
        });

        modelBuilder.Entity<DtLocation>(entity =>
        {
            entity.ToTable("DT_LOCATION");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.Location)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DtMake>(entity =>
        {
            entity.ToTable("DT_MAKE");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.Make)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DtProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_projects");

            entity.ToTable("DT_PROJECT");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.Project)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.RecordStatusId).HasDefaultValue((byte)7);
        });

        modelBuilder.Entity<DtProvince>(entity =>
        {
            entity.ToTable("DT_PROVINCE");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.Province)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.RecordStatausId).HasDefaultValue(7);
        });

        modelBuilder.Entity<DtStatus>(entity =>
        {
            entity.ToTable("DT_STATUS");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.RecordStatusId).HasDefaultValue(7);
            entity.Property(e => e.Status)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DtStatusType>(entity =>
        {
            entity.ToTable("DT_STATUS_TYPE");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.RecordStatusId).HasDefaultValue(7);
            entity.Property(e => e.StatusType)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DtStock>(entity =>
        {
            entity.ToTable("DT_STOCK");

            entity.Property(e => e.Assigned)
                .HasDefaultValue(0)
                .HasColumnName("assigned");
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedByUser).IsUnicode(false);
            entity.Property(e => e.Decomissioned).IsUnicode(false);
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Disposed).IsUnicode(false);
            entity.Property(e => e.Itnumber)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasComputedColumnSql("(('IT'+case when len(CONVERT([varchar],[Id]))=(1) then '000'+CONVERT([varchar],[Id]) when len(CONVERT([varchar],[Id]))=(2) then '00'+CONVERT([varchar],[Id]) else CONVERT([varchar],[Id]) end)+'S')", false)
                .HasColumnName("ITNumber");
            entity.Property(e => e.ItnumberIImported)
                .IsUnicode(false)
                .HasColumnName("ITNumberI_Imported");
            entity.Property(e => e.LastUpdated)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdatedByUser).IsUnicode(false);
            entity.Property(e => e.Make).IsUnicode(false);
            entity.Property(e => e.Model).IsUnicode(false);
            entity.Property(e => e.Modified)
                .HasColumnType("datetime")
                .HasColumnName("modified");
            entity.Property(e => e.OrderNumber).IsUnicode(false);
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.PurchasePrice).IsUnicode(false);
            entity.Property(e => e.RecordStatusId).HasDefaultValue(7);
            entity.Property(e => e.ReqDimension1Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REQ_DIMENSION1_CODE");
            entity.Property(e => e.ReqDimension2Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REQ_DIMENSION2_CODE");
            entity.Property(e => e.ReqDimension3Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REQ_DIMENSION3_CODE");
            entity.Property(e => e.ReqDimension4Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REQ_DIMENSION4_CODE");
            entity.Property(e => e.SerialNo).IsUnicode(false);
            entity.Property(e => e.SiteCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SITE_CODE");
            entity.Property(e => e.StockStatusId).HasDefaultValue(6);
            entity.Property(e => e.Warehouse)
                .HasDefaultValue(1)
                .HasColumnName("warehouse");
        });

        modelBuilder.Entity<DtUser>(entity =>
        {
            entity.ToTable("DT_USER");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("createdBy");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("modifiedDate");
            entity.Property(e => e.Role).HasColumnName("role");

            // Add relationship
            entity.HasOne(d => d.RoleNavigation)
                .WithMany()
                .HasForeignKey(d => d.Role)
                .HasConstraintName("FK_DtUser_Role");
        });
        modelBuilder.Entity<DtUserRole>(entity =>
        {
            entity.ToTable("DT_USER_ROLE");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<StockItemsV>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("StockItems_v");

            entity.Property(e => e.Category)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Condition)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasMaxLength(4000);
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.District)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Facility)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Itnumber)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("ITNumber");
            entity.Property(e => e.Itnumber1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ITNumber1");
            entity.Property(e => e.Make)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Model)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Project)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ProjectCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Province)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.PurchasePriceUsd)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PurchasePriceUSD");
            entity.Property(e => e.PurchasePriceZmk)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PurchasePriceZMK");
            entity.Property(e => e.RecordStatus)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.SerialNo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.StockStatus)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserV>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("User_v");

            entity.Property(e => e.AccessLevel)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasMaxLength(4000);
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.JobTitle)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RecordStatus)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwDttag>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_DTtags");

            entity.Property(e => e.Itnumber)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("ITNumber");
            entity.Property(e => e.Location)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Make).IsUnicode(false);
            entity.Property(e => e.Model).IsUnicode(false);
            entity.Property(e => e.Project)
                .HasMaxLength(150)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ReqDimension2Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REQ_DIMENSION2_CODE");
            entity.Property(e => e.SerialNo).IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
