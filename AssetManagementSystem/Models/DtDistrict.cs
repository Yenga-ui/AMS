using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class DtDistrict
{
    public int Id { get; set; }

    public string? District { get; set; }

    public int? ProvinceId { get; set; }

    public int? RecordStatusId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? LastUpdated { get; set; }
}
