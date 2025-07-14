/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.Domain.Entities;

public class Car
{
    public Guid             /**/ Id         /**/ { get; set; } = Guid.NewGuid();
    public string           /**/ Model      /**/ { get; set; } = string.Empty;
    public string           /**/ Type       /**/ { get; set; } = string.Empty;
    public List<Service>    /**/ Services   /**/ { get; set; } = new();

    public bool IsAvailable(DateTime start, DateTime end, List<Rental> rentals)
    {
        var hasConflict = rentals.Any(r => r.CarId == Id &&
            (start <= r.EndDate && end >= r.StartDate));
        return !hasConflict;
    }
}
