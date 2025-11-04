using CSharpFunctionalExtensions;
using Epam.ItMarathon.ApiService.Application.UseCases.User.Commands;
using Epam.ItMarathon.ApiService.Application.UseCases.User.Queries;
using Epam.ItMarathon.ApiService.Domain.Abstract;
using Epam.ItMarathon.ApiService.Domain.Shared.ValidationErrors;
using FluentValidation.Results;
using MediatR;
using RoomAggregate = Epam.ItMarathon.ApiService.Domain.Aggregate.Room.Room;

namespace Epam.ItMarathon.ApiService.Application.UseCases.User.Handlers
{

    public class DeleteUserHandler(IRoomRepository roomRepository)
        : IRequestHandler<DeleteUserRequest, Result<RoomAggregate, ValidationResult>>
    {
        public async Task<Result<RoomAggregate, ValidationResult>> Handle(DeleteUserRequest request,
            CancellationToken cancellationToken)
        {
            //  1. Get room by admin userCode
            var roomResult = await roomRepository.GetByUserCodeAsync(request.UserCode, cancellationToken);

            if (roomResult.IsFailure)
            {
                return Result.Failure<RoomAggregate, ValidationResult>(
                    new NotFoundError([
                        new ValidationFailure("userCode", "Користувача з таким userCode не знайдено.")
                    ]));
            }

            var room = roomResult.Value;

            // 2. Find acting user by auth code
            var actingUser = room.Users.FirstOrDefault(u => u.AuthCode == request.UserCode);

            if (actingUser is null)
            {
                return Result.Failure<RoomAggregate, ValidationResult>(
                    new NotFoundError([
                        new ValidationFailure("userCode", "Користувача з userCode не знайдено в цій кімнаті.")
                    ]));
            }

            //  3. Check admin
            if (!actingUser.IsAdmin)
            {
                return Result.Failure<RoomAggregate, ValidationResult>(
                    new BadRequestError([
                        new ValidationFailure("userCode", "Користувач з userCode не є адміністратором.")
                    ]));
            }

            // 4. Find user to delete
            var userToDelete = room.Users.FirstOrDefault(u => u.Id == request.UserId);

            if (userToDelete is null)
            {
                return Result.Failure<RoomAggregate, ValidationResult>(
                    new NotFoundError([
                        new ValidationFailure("userId", "Користувача з id не знайдено.")
                    ]));
            }

            //  5. Check "cannot delete yourself"
            if (actingUser.Id == userToDelete.Id)
            {
                return Result.Failure<RoomAggregate, ValidationResult>(
                    new BadRequestError([
                        new ValidationFailure("userId", "Адміністратор не може видалити сам себе.")
                    ]));
            }

            // 6. Check room is not closed
            if (room.ClosedOn is not null)
            {
                return Result.Failure<RoomAggregate, ValidationResult>(
                    new BadRequestError([
                        new ValidationFailure("room.ClosedOn", "Кімната вже закрита.")
                    ]));
            }

            //  7. Execute deletion using domain logic
            var deleteResult = room.DeleteUser(request.UserId);

            if (deleteResult.IsFailure)
            {
                return deleteResult;
            }

            //  8. Update room
            var updateResult = await roomRepository.UpdateAsync(room, cancellationToken);

            if (updateResult.IsFailure)
            {
                return Result.Failure<RoomAggregate, ValidationResult>(
                    new BadRequestError([
                        new ValidationFailure(string.Empty, updateResult.Error)
                    ]));
            }

            //  9. Return updated room
            var updatedRoomResult = await roomRepository.GetByUserCodeAsync(request.UserCode, cancellationToken);
            return updatedRoomResult;
        }
    }
}

public interface IRequestHandler<T1, T2, T3>
    {
    }
