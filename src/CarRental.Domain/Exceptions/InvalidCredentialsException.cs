/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.Domain.Exceptions;

public class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException() : base("Invalid credentials.") { }
}
