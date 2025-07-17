/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.Domain.Entities;

public class Car : IEntity
{
    public Guid             /**/ Id         /**/ { get; set; } = Guid.NewGuid();
    public string           /**/ Model      /**/ { get; set; } = string.Empty;
    public string           /**/ Type       /**/ { get; set; } = string.Empty;
    public List<Service>    /**/ Services   /**/ { get; set; } = new();
    public bool             /**/ IsActive   /**/ { get; set; } = true;      /// IEntity (logical delete)
    public int              /**/ Version    /**/ { get; set; } = 1;         /// IEntity: Control de concurrencia con versión incremental
}
