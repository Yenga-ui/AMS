using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class UserV
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? JobTitle { get; set; }

    public string? Email { get; set; }

    public string? AccessLevel { get; set; }

    public string? RecordStatus { get; set; }

    public string? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public int? CreatedById { get; set; }
}
