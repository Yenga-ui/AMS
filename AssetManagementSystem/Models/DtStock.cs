using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class DtStock
{
    public int Id { get; set; }

    public string? ItnumberIImported { get; set; }

    public string? OrderNumber { get; set; }

    public string? Itnumber { get; set; }

    public string? SerialNo { get; set; }

    public string? Make { get; set; }

    public string? Model { get; set; }

    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public int? ProjectId { get; set; }

    public int? ConditionId { get; set; }

    public int? StockStatusId { get; set; }

    public int? LocationId { get; set; }

    public int? RecordStatusId { get; set; }

    public int? CurrencyId { get; set; }

    public string? PurchasePrice { get; set; }

    public string? Comment { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public DateOnly? DateReceived { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdateBy { get; set; }

    public string? LastUpdated { get; set; }

    public string? SiteCode { get; set; }

    public string? ReqDimension1Code { get; set; }

    public string? ReqDimension2Code { get; set; }

    public string? ReqDimension3Code { get; set; }

    public string? ReqDimension4Code { get; set; }

    public string? Decomissioned { get; set; }

    public string? Disposed { get; set; }

    public string? CreatedByUser { get; set; }

    public string? LastUpdatedByUser { get; set; }

    public DateTime? Modified { get; set; }

    public int? Assigned { get; set; }

    public int? Warehouse { get; set; }

    public DtCategory? Category { get; set; }

    public DtProject? Project { get; set; }

    public DtLocation? Location { get; set; }   

    public DtStatus? StockStatus { get; set; }

    public DtCondition? Condition { get; set; }
    public DtCurrency? Currency { get; set; }
}

