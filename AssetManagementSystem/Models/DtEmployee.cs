using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class DtEmployee
{
    public int Id { get; set; }

    public string? EmployeeCode { get; set; }

    public string? Fullname { get; set; }

    public string? Gender { get; set; }

    public string? Nrc { get; set; }

    public string? JobTitle { get; set; }

    public string? Email { get; set; }

    public int? RecordStatusId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? LastUpdate { get; set; }
}
