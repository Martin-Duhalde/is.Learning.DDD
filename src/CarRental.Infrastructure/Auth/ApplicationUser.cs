/// MIT License © 2025 Martín Duhalde + ChatGPT

using Microsoft.AspNetCore.Identity;

namespace CarRental.Infrastructure.Auth;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
