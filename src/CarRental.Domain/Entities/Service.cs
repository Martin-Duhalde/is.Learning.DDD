/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.Domain.Entities;

public class Service
{
    public Guid         /**/ Id     /**/ { get; set; } = Guid.NewGuid();
    public DateTime     /**/ Date   /**/ { get; set; }
    public Guid         /**/ CarId  /**/ { get; set; }
    public Car?         /**/ Car    /**/ { get; set; }
}
