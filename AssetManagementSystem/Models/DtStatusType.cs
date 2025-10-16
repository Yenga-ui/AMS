using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class DtStatusType
{
    public int Id { get; set; }

    public string? StatusType { get; set; }

    public string? Description { get; set; }

    public int? CreatedBy { get; set; }

    public int? RecordStatusId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? UpdatedById { get; set; }

    public DateOnly? LastUpdatedDate { get; set; }
}
