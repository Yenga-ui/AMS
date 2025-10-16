using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class DtProvince
{
    public int Id { get; set; }

    public string? Province { get; set; }

    public int? RecordStatausId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? LastUpdated { get; set; }
}
