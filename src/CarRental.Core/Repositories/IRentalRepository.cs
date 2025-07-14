/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;

namespace CarRental.Core.Repositories;

/// <summary>
/// Repository for Rental aggregate.
/// </summary>
public interface IRentalRepository : IRepository<Rental>
{

    Task<bool>          /**/ ExistsAsync                    /**/ (Guid carId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<Rental?>       /**/ GetByIdWithDetailsAsync        /**/ (Guid rentalId, CancellationToken cancellationToken = default);
    Task<List<Rental>>  /**/ ListActivesBetweenDatesAsync   /**/ (DateTime from, DateTime to, CancellationToken cancellationToken);
    Task<List<Rental>>  /**/ GetRentalsByCustomerIdAsync    /**/ (Guid customerId, CancellationToken cancellationToken = default);
    Task<List<Rental>>  /**/ ListLast7DaysAsync             /**/ (CancellationToken cancellationToken = default);
    Task<List<Rental>>  /**/ ListCancelledAsync             /**/ (DateTime from, DateTime to, CancellationToken ct = default);
    Task<int>           /**/ CountCancelledAsync            /**/ (DateTime from, DateTime to, CancellationToken ct = default);
    Task                /**/ CancelAsync                    /**/ (Guid rentalId, CancellationToken ct = default);
}