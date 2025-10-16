using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class AppMaitenance
{
    public int Id { get; set; }

    public int? EmployeeId { get; set; }

    public int? StockId { get; set; }

    public string? Diagnosis { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public DateTime? CheckInDate { get; set; }

    public int? CreatedBy { get; set; }

    public string? Ticket { get; set; }

    public string? SerialNo { get; set; }

    public int? Status { get; set; }

    public DtStock Stock { get; set; }
    public DtEmployee Employee { get; set; }
}
