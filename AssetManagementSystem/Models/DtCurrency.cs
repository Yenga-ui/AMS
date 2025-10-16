using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class DtCurrency
{
    public int Id { get; set; }

    public string? Currency { get; set; }

    public string? Code { get; set; }

    public int? RecordStatusId { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? LastUpdated { get; set; }
}
