using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class StockItemsV
{
    public int Id { get; set; }

    public string? Itnumber { get; set; }

    public string? Itnumber1 { get; set; }

    public string? SerialNo { get; set; }

    public string? Make { get; set; }

    public string? Model { get; set; }

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? ProjectCode { get; set; }

    public string? Project { get; set; }

    public string? Condition { get; set; }

    public string? StockStatus { get; set; }

    public string? Province { get; set; }

    public string? District { get; set; }

    public string? Facility { get; set; }

    public string? RecordStatus { get; set; }

    public string? PurchasePriceZmk { get; set; }

    public string? PurchasePriceUsd { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public DateOnly? DateReceived { get; set; }

    public string? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }
}
