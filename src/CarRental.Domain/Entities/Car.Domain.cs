/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Exceptions;

namespace CarRental.Domain.Entities;

public class Car
{
    private readonly List<Service> _services = new();

    private Car()
    {
        // Required by EF Core
    }

    private Car(Guid id, string model, string type, bool isActive, int version, IEnumerable<Service>? services = null)
    {
        Id = id;
        SetModel(model);
        SetType(type);
        IsActive = isActive;
        Version = version;

        if (services != null)
        {
            _services = new List<Service>(services);
        }
    }

    public Guid Id { get; private set; }
    public string Model { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public IReadOnlyCollection<Service> Services => _services.AsReadOnly();
    public bool IsActive { get; private set; } = true;
    public int Version { get; private set; } = 1; // Concurrency handled by repositories

    public static Car Create(string model, string type)
    {
        return new Car(Guid.NewGuid(), model, type, isActive: true, version: 1);
    }

    /// <summary>
    /// Helper to recreate the aggregate from persistence/tests.
    /// </summary>
    public static Car Restore(Guid id, string model, string type, bool isActive, int version, IEnumerable<Service>? services = null)
    {
        return new Car(id, model, type, isActive, version, services);
    }

    public static Car ForTesting(Guid? id = null, string? model = null, string? type = null, bool isActive = true, int version = 1)
    {
        return new Car(id ?? Guid.NewGuid(), model ?? "Model", type ?? "Type", isActive, version);
    }

    public void UpdateDetails(string model, string type)
    {
        SetModel(model);
        SetType(type);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Reactivate()
    {
        IsActive = true;
    }

    public void IncrementVersion() => Version++;

    private void SetModel(string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new DomainException("Model is required.");

        Model = model.Trim();
    }

    private void SetType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new DomainException("Type is required.");

        Type = type.Trim();
    }
}
