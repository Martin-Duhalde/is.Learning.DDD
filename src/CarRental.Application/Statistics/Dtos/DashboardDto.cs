/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.Application.Statistics.Dtos; 

public class DashboardDto
{
    public List<DailyStatDto> DailyStats { get; set; } = new();
}