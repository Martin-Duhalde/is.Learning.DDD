/// MIT License © 2025 Martín Duhalde + ChatGPT
/// Code From: Clean.Architecture.Infrastructure (Ardalis)

namespace CarRental.Infrastructure.Email;

public class MailserverConfiguration()
{
    public string Hostname { get; set; } = "localhost";
    public int Port { get; set; } = 25;
}
