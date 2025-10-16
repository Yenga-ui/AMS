using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class DtProject
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Project { get; set; }

    public DateTime? CreatedDate { get; set; }

    public byte? CreatedBy { get; set; }

    public byte? RecordStatusId { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? LastUpdated { get; set; }
}
