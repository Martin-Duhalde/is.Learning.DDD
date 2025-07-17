/// MIT License © 2025 Martín Duhalde + ChatGPT
using CarRental.Core.Repositories;
using CarRental.Domain.Exceptions;

using MediatR;

namespace CarRental.UseCases.Customers.Delete
{
    public record DeleteCustomerCommand(Guid Id) : IRequest<Unit>;

    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Unit>
    {
        private readonly ICustomerRepository _customerRepository;

        public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetActiveByIdAsync(request.Id, cancellationToken) 
                ?? throw new DomainException("Customer not found");

            await _customerRepository.DeleteAsync(customer);

            return Unit.Value;
        }
    }
}
