using CSharpFunctionalExtensions;
using FluentValidation.Results;
using RoomAggregate = Epam.ItMarathon.ApiService.Domain.Aggregate.Room.Room;
using MediatR;
namespace Epam.ItMarathon.ApiService.Application.UseCases.User.Commands
{
  
    public record DeleteUserRequest(string UserCode, ulong? UserId)
        : IRequest<Result<RoomAggregate, ValidationResult>>;

    
}