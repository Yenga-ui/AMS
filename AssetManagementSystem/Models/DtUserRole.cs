using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class DtUserRole
{
    public int Id { get; set; }

    public string? Role { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }
}
