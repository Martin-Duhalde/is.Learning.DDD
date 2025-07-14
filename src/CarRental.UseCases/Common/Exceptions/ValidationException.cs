//using FluentValidation.Results;

//namespace CarRental.UseCases.Common.Exceptions;

///// <summary>
///// ❌ Excepción lanzada cuando la validación de un request falla.
///// </summary>
//public class ValidationException : Exception
//{
//    /// <summary>
//    /// Diccionario con los errores agrupados por propiedad.
//    /// </summary>
//    public IDictionary<string, string[]> Errors { get; }

//    public ValidationException() : base("Se encontraron errores de validación.")
//    {
//        Errors = new Dictionary<string, string[]>();
//    }

//    public ValidationException(IEnumerable<ValidationFailure> failures)
//        : this()
//    {
//        Errors = failures
//            .GroupBy(e => e.PropertyName)
//            .ToDictionary(
//                g => g.Key,
//                g => g.Select(e => e.ErrorMessage).Distinct().ToArray()
//            );
//    }
//}