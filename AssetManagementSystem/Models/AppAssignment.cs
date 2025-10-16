using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class AppAssignment
{
    public int Id { get; set; }

    public int? StockId { get; set; }

    public int? EmployeeId { get; set; }

    public int? ProjectId { get; set; }

    public int? LocationId { get; set; }

    public string? Comments { get; set; }

    public int? RecordStatusId { get; set; }

    public int? ConditionId { get; set; }

    public DateTime? DateAssigned { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedById { get; set; }

    public int? UpdateById { get; set; }

    public DateTime? LastUpdateDate { get; set; }

    public int? Status { get; set; }

    public DtStock Stock { get; set; }
    public DtEmployee Employee { get; set; }
    public DtLocation Location { get; set; }
    public DtProject Project { get; set; }
}
