/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.Domain.Entities;

public class Customer : IEntity
{
    public Guid     /**/ Id         /**/ { get; set; } = Guid.NewGuid();
    public string   /**/ FullName   /**/ { get; set; } = string.Empty;
    public string   /**/ Address    /**/ { get; set; } = string.Empty;
    public string   /**/ UserId     /**/ { get; set; } = string.Empty!; /// IdentityUser Relationship
    public bool     /**/ IsActive   /**/ { get; set; } = true;          /// IEntity (logical delete)
    public int      /**/ Version    /**/ { get; set; } = 1;             /// IEntity: Control de concurrencia con versión incremental
}