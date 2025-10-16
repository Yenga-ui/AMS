using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class AppBorrow
{
    public int Id { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public int? StockId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? Status { get; set; }

    public DtStock Stock { get; set; }
    public DtEmployee Employee { get; set; }
}
