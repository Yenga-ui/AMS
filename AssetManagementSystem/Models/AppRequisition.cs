using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class AppRequisition
{
    public int Id { get; set; }

    public int? StockItemId { get; set; }

    public int? Quantity { get; set; }

    public int? RequestingProjectId { get; set; }

    public string? DispatchNo { get; set; }

    public int? ApprovalStatusId { get; set; }

    public int? RequestedById { get; set; }

    public int? ApprovedById { get; set; }

    public int? CreatedById { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? RecordStatusId { get; set; }

    public int? UpdatedById { get; set; }

    public DateTime? LastUpdateDate { get; set; }
}
