/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.Domain.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated in the domain.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}