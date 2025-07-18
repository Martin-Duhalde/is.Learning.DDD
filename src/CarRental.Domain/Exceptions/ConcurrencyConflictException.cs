namespace CarRental.Domain.Exceptions;

public class ConcurrencyConflictException : DomainException
{
    public ConcurrencyConflictException(string? message = null)
        : base(message ?? "A concurrency conflict occurred.") { }
}