using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedtoDtStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APP_ASSIGNMENT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockId = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    RecordStatusId = table.Column<int>(type: "int", nullable: true),
                    ConditionId = table.Column<int>(type: "int", nullable: true),
                    DateAssigned = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    UpdateById = table.Column<int>(type: "int", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_ASSIGNMENT", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_BORROW",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    DateFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    DateTo = table.Column<DateOnly>(type: "date", nullable: true),
                    StockId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_BORROW", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_COST",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InitialCostUSD = table.Column<decimal>(type: "money", nullable: true),
                    InitialCostZMW = table.Column<decimal>(type: "money", nullable: true),
                    SalvageValueUSD = table.Column<decimal>(type: "money", nullable: true),
                    SalvageValueZMW = table.Column<decimal>(type: "money", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_DEPRECIATION", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_MAITENANCE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    StockId = table.Column<int>(type: "int", nullable: true),
                    Diagnosis = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    returnDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    checkInDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ticket = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    serialNo = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    status = table.Column<int>(type: "int", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_MAITENANCE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_REQUISITION",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockItemId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    RequestingProjectId = table.Column<int>(type: "int", nullable: true),
                    DispatchNo = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    ApprovalStatusId = table.Column<int>(type: "int", nullable: true),
                    RequestedById = table.Column<int>(type: "int", nullable: true),
                    ApprovedById = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    RecordStatusId = table.Column<int>(type: "int", nullable: true, defaultValue: 7),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_REQUISITION", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_TRACKER",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionPerformed = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    StockId = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_TRACKER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_USER",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    RecordStatusId = table.Column<int>(type: "int", nullable: true, defaultValue: 7),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_USER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DT_CATEGORY",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    TypeId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    RecordStatusId = table.Column<int>(type: "int", nullable: true, defaultValue: 7),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DT_CATEGORY", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DT_CATEGORY_TYPE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DT_CATEGORY_TYPE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DT_CONDITION",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Condition = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    Description = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    RecordStatusId = table.Column<int>(type: "int", nullable: true, defaultValue: 7),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DT_CONDITION", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DT_CURRENCY",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Currency = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    RecordStatusId = table.Column<int>(type: "int", nullable: true, defaultValue: 7),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdateBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DT_CURRENCY", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DT_DISTRICT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    District = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: true),
                    RecordStatusId = table.Column<int>(type: "int", nullable: true, defaultValue: 7),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DT_DISTRICT", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DT_EMPLOYEE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Fullname = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Gender = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    NRC = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    JobTitle = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    Email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    RecordStatusId = table.Column<int>(type: "int", nullable: true, defaultValue: 7),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee2", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DT_LOCATION",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: true),
                    DistrictId = table.Column<int>(type: "int", nullable: true),
                    RecordStatusId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DT_LOCATION", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DT_MAKE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Make = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    RecordStatusId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DT_MAKE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DT_PROJECT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Project = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<byte>(type: "tinyint", nullable: true),
                    RecordStatusId = table.Column<byte>(type: "tinyint", nullable: true, defaultValue: (byte)7),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DT_PROVINCE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Province = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    RecordStatausId = table.Column<int>(type: "int", nullable: true, defaultValue: 7),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DT_PROVINCE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DT_STATUS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    RecordStatusId = table.Column<int>(type: "int", nullable: true, defaultValue: 7),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DT_STATUS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DT_STATUS_TYPE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    RecordStatusId = table.Column<int>(type: "int", nullable: true, defaultValue: 7),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DT_STATUS_TYPE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DT_USER_ROLE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    Description = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DT_USER_ROLE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DT_STOCK",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ITNumberI_Imported = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    OrderNumber = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    ITNumber = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true, computedColumnSql: "(('IT'+case when len(CONVERT([varchar],[Id]))=(1) then '000'+CONVERT([varchar],[Id]) when len(CONVERT([varchar],[Id]))=(2) then '00'+CONVERT([varchar],[Id]) else CONVERT([varchar],[Id]) end)+'S')", stored: false),
                    SerialNo = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Make = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Model = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    ProjectID = table.Column<int>(type: "int", nullable: true),
                    ConditionId = table.Column<int>(type: "int", nullable: true),
                    StockStatusId = table.Column<int>(type: "int", nullable: true, defaultValue: 6),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    RecordStatusId = table.Column<int>(type: "int", nullable: true, defaultValue: 7),
                    CurrencyId = table.Column<int>(type: "int", unicode: false, maxLength: 50, nullable: true),
                    PurchasePrice = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Comment = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    PurchaseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DateReceived = table.Column<DateOnly>(type: "date", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdateBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    SITE_CODE = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    REQ_DIMENSION1_CODE = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    REQ_DIMENSION2_CODE = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    REQ_DIMENSION3_CODE = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    REQ_DIMENSION4_CODE = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Decomissioned = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Disposed = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    CreatedByUser = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    LastUpdatedByUser = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    modified = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DT_STOCK", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DT_STOCK_DT_CATEGORY_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "DT_CATEGORY",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DT_STOCK_DT_CONDITION_ConditionId",
                        column: x => x.ConditionId,
                        principalTable: "DT_CONDITION",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DT_STOCK_DT_CURRENCY_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "DT_CURRENCY",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DT_STOCK_DT_LOCATION_LocationId",
                        column: x => x.LocationId,
                        principalTable: "DT_LOCATION",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DT_STOCK_DT_PROJECT_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "DT_PROJECT",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DT_STOCK_DT_STATUS_StockStatusId",
                        column: x => x.StockStatusId,
                        principalTable: "DT_STATUS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DT_STOCK_CategoryId",
                table: "DT_STOCK",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DT_STOCK_ConditionId",
                table: "DT_STOCK",
                column: "ConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_DT_STOCK_CurrencyId",
                table: "DT_STOCK",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_DT_STOCK_LocationId",
                table: "DT_STOCK",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_DT_STOCK_ProjectID",
                table: "DT_STOCK",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_DT_STOCK_StockStatusId",
                table: "DT_STOCK",
                column: "StockStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APP_ASSIGNMENT");

            migrationBuilder.DropTable(
                name: "APP_BORROW");

            migrationBuilder.DropTable(
                name: "APP_COST");

            migrationBuilder.DropTable(
                name: "APP_MAITENANCE");

            migrationBuilder.DropTable(
                name: "APP_REQUISITION");

            migrationBuilder.DropTable(
                name: "APP_TRACKER");

            migrationBuilder.DropTable(
                name: "APP_USER");

            migrationBuilder.DropTable(
                name: "DT_CATEGORY_TYPE");

            migrationBuilder.DropTable(
                name: "DT_DISTRICT");

            migrationBuilder.DropTable(
                name: "DT_EMPLOYEE");

            migrationBuilder.DropTable(
                name: "DT_MAKE");

            migrationBuilder.DropTable(
                name: "DT_PROVINCE");

            migrationBuilder.DropTable(
                name: "DT_STATUS_TYPE");

            migrationBuilder.DropTable(
                name: "DT_STOCK");

            migrationBuilder.DropTable(
                name: "DT_USER_ROLE");

            migrationBuilder.DropTable(
                name: "DT_CATEGORY");

            migrationBuilder.DropTable(
                name: "DT_CONDITION");

            migrationBuilder.DropTable(
                name: "DT_CURRENCY");

            migrationBuilder.DropTable(
                name: "DT_LOCATION");

            migrationBuilder.DropTable(
                name: "DT_PROJECT");

            migrationBuilder.DropTable(
                name: "DT_STATUS");
        }
    }
}
