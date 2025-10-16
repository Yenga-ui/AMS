using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagementSystem.Models;

public partial class DtUser
{
    public int Id { get; set; }
    public string Email { get; set; }
    public int? Role { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Add navigation property
    [ForeignKey("Role")]
    public virtual DtUserRole RoleNavigation { get; set; }
}
