using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class DtStatus
{
    public int Id { get; set; }

    public string? Status { get; set; }

    public int? TypeId { get; set; }

    public string? Description { get; set; }

    public int? RecordStatusId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? LastUpdatedDate { get; set; }
}
