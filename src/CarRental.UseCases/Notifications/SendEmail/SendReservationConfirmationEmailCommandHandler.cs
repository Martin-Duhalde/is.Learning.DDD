/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Interfaces;
using CarRental.Core.Repositories;
using CarRental.Domain.Exceptions;
using CarRental.Infrastructure.Auth;

using MediatR;

using Microsoft.AspNetCore.Identity;

namespace CarRental.UseCases.Notifications.SendEmail;

/// <summary>
/// 📬 Command to send confirmation email after rental reservation.
/// </summary>
public record SendReservationConfirmationEmailCommand(Guid RentalId) : IRequest<Unit>;

public class SendReservationConfirmationEmailCommandHandler : IRequestHandler<SendReservationConfirmationEmailCommand, Unit>
{
    private readonly IRentalRepository              /**/ _rentalRepository;
    private readonly IEmailService                  /**/ _emailService;
    private readonly UserManager<ApplicationUser>   /**/ _userManager;

    public SendReservationConfirmationEmailCommandHandler(IRentalRepository rentalRepo, IEmailService emailService, UserManager<ApplicationUser> userManager)
    {
        _rentalRepository   /**/ = rentalRepo;
        _emailService       /**/ = emailService;
        _userManager        /**/ = userManager;
    }

    public async Task<Unit> Handle(SendReservationConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdWithDetailsAsync(request.RentalId, cancellationToken)
            ?? throw new DomainException("Rental not found.");

        if (rental.Customer == null)                                            /**/ throw new DomainException("Rental Customer data is missing.");
        if (string.IsNullOrEmpty(rental.Customer.UserId))                       /**/ throw new DomainException("Customer UserId is missing.");

        var user = await _userManager.FindByIdAsync(rental.Customer.UserId) ??  /**/ throw new DomainException("User not found.");

        if (string.IsNullOrEmpty(user.Email))                                   /**/ throw new DomainException("User email is missing.");
        if (rental.Car == null)                                                 /**/ throw new DomainException("Rental Car data is missing.");


        var carModel  /**/ = string.IsNullOrEmpty(rental.Car.Model)  /**/ ? "Unknown Model" /**/ : rental.Car.Model;
        var carType   /**/ = string.IsNullOrEmpty(rental.Car.Type)   /**/ ? "Unknown Type"  /**/ : rental.Car.Type;

        var body = $"Dear {rental.Customer.FullName},\n\n" +
                   $"Your reservation has been confirmed.\n\n" +
                   $"Car: {carModel} ({carType})\n" +
                   $"Rental period: {rental.StartDate:yyyy-MM-dd} to {rental.EndDate:yyyy-MM-dd}\n\n" +
                   "Thank you for choosing our service.";

        await _emailService.SendEmailAsync(user.Email, "noreply@carrental.com", "Car Rental Confirmation", body);

        return Unit.Value;
    }
}