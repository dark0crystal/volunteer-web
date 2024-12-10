using System;
using System.Collections.Generic;

namespace volunteer_web.Server.Data;

public partial class Complaint
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public string ComplaintType { get; set; } = null!;

    public string ComplaintText { get; set; } = null!;

    public virtual VolunteeringPost Post { get; set; } = null!;
}
