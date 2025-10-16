using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class DtCategoryType
{
    public int Id { get; set; }

    public string? Type { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }
}
