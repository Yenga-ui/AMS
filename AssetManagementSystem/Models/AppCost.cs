using System;
using System.Collections.Generic;

namespace AssetManagementSystem.Models;

public partial class AppCost
{
    public int Id { get; set; }

    public decimal? InitialCostUsd { get; set; }

    public decimal? InitialCostZmw { get; set; }

    public decimal? SalvageValueUsd { get; set; }

    public decimal? SalvageValueZmw { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }
}
