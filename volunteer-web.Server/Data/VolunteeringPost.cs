using System;
using System.Collections.Generic;

namespace volunteer_web.Server.Data;

public partial class VolunteeringPost
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int NumOfDays { get; set; } = 0;

    public string Location { get; set; } = null!;

    public string OrgName { get; set; } = null!;

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string PostAdminEmail { get; set; } = null!;

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
}
