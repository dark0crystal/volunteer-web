using System;
using System.Collections.Generic;

namespace volunteer_web.Server.Data;

public partial class Register
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string FamilyName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}
