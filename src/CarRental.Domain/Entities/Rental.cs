/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.Domain.Entities;

public enum RentalStatus { Active, Cancelled }

public class Rental : IEntity
{
    public Guid         /**/ Id             /**/ { get; set; } = Guid.NewGuid();
    public Guid         /**/ CustomerId     /**/ { get; set; } 
    public Customer?    /**/ Customer       /**/ { get; set; }
    public Guid         /**/ CarId          /**/ { get; set; }
    public Car?         /**/ Car            /**/ { get; set; }
    public DateTime     /**/ StartDate      /**/ { get; set; }
    public DateTime     /**/ EndDate        /**/ { get; set; }
    public RentalStatus /**/ RentalStatus   /**/ { get; set; } = RentalStatus.Active;
    public DateTime?    /**/ CancelledAt    /**/ { get; set; }
    public bool         /**/ IsActive       /**/ { get; set; } = true;      /// IEntity (logical delete)
    public int          /**/ Version        /**/ { get; set; } = 1;         /// IEntity: Control de concurrencia con versión incremental
}
