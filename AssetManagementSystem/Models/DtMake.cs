using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class DtMake
{
    public int Id { get; set; }

    public string? Make { get; set; }

    public int? CreatedBy { get; set; }

    public int? RecordStatusId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? LastUpdated { get; set; }
}
