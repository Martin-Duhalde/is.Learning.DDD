/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Statistics.Dtos;

/// <summary>
/// 📈 DTO representing a ranked car by brand, model and type usage.
/// </summary>
public class TopCarByBrandModelDto
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}