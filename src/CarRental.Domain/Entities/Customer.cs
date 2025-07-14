/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.Domain.Entities;

public class Customer
{
    public Guid     /**/ Id         /**/ { get; set; } = Guid.NewGuid();
    public string   /**/ FullName   /**/ { get; set; } = string.Empty;
    public string   /**/ Address    /**/ { get; set; } = string.Empty;

    /// User (feature added to original specification)
    public string UserId           /**/ { get; set; } = string.Empty!; /// IdentityUser Relationship

}