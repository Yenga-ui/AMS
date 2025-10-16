using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class AppTracker
{
    public int Id { get; set; }

    public string? ActionPerformed { get; set; }

    public int? StockId { get; set; }

    public int? EmployeeId { get; set; }

    public int? CreatedById { get; set; }

    public DateTime? CreatedDate { get; set; }
}
