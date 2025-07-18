/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.Domain.Exceptions;

/// <summary>
/// 🚫 Excepción lanzada cuando un recurso de dominio no es encontrado (por ID, inactivo o eliminado lógicamente).
/// </summary>
public class DomainNotFoundException : DomainException
{
    /// <param name="entityName">Nombre de la entidad (por ejemplo: "Car").</param>
    /// <param name="id">Identificador del recurso buscado.</param>
    public DomainNotFoundException(string entityName, object id)
        : base($"{entityName} with ID '{id}' was not found or is inactive.")
    {
    }
}