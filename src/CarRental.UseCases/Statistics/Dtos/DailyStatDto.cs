/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Statistics.Dtos;

public class DailyStatDto
{
    public DateTime Date { get; set; }
    public int Rentals { get; set; }
    public int Cancellations { get; set; }
    public int UnusedCars { get; set; }
}