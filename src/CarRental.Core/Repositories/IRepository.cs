/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.Core.Repositories;

/// <summary>
/// Generic repository contract for an aggregate root of type T.
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?>                /**/ GetActiveByIdAsync     /**/ (Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>>  /**/ ListAllActivesAsync    /**/ (CancellationToken ct = default);
    Task                    /**/ AddAsync               /**/ (T entity, CancellationToken ct = default);
    Task                    /**/ UpdateAsync            /**/ (T entity, CancellationToken ct = default);
    Task                    /**/ DeleteAsync            /**/ (T entity, CancellationToken ct = default);
}