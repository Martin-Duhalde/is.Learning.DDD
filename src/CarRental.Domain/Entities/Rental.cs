/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.Domain.Entities;

public enum RentalStatus { Active, Cancelled }

public class Rental
{
    public Guid         /**/ Id             /**/ { get; set; } = Guid.NewGuid();
    public Guid         /**/ CustomerId     /**/ { get; set; } 
    public Customer?    /**/ Customer       /**/ { get; set; }
    public Guid         /**/ CarId          /**/ { get; set; }
    public Car?         /**/ Car            /**/ { get; set; }
    public DateTime     /**/ StartDate      /**/ { get; set; }
    public DateTime     /**/ EndDate        /**/ { get; set; }
    public RentalStatus /**/ Status         /**/ { get; set; } = RentalStatus.Active;
    public DateTime?    /**/ CancelledAt    /**/ { get; set; }
}
