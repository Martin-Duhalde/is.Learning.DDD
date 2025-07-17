/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.Domain.Entities;

public class Service : IEntity
{
    public Guid         /**/ Id         /**/ { get; set; } = Guid.NewGuid();
    public DateTime     /**/ Date       /**/ { get; set; }
    public Guid         /**/ CarId      /**/ { get; set; }
    public Car?         /**/ Car        /**/ { get; set; }
    public bool         /**/ IsActive   /**/ { get; set; } = true;          /// IEntity (logical delete)
    public int          /**/ Version    /**/ { get; set; } = 1;             /// IEntity: Control de concurrencia con versión incremental

}
