using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Epam.ItMarathon.ApiService.Application.UseCases.User.Commands;
using Epam.ItMarathon.ApiService.Application.UseCases.User.Handlers;
using Epam.ItMarathon.ApiService.Domain.Abstract;
using Epam.ItMarathon.ApiService.Domain.Aggregate.Room;
using Epam.ItMarathon.ApiService.Domain.Entities.User;
using FluentAssertions;
using FluentValidation.Results;
using NSubstitute;
using Xunit;

public class DeleteUserHandlerTests
{
    private readonly IRoomRepository _roomRepository = Substitute.For<IRoomRepository>();
    private readonly CancellationToken _ct = CancellationToken.None;

    private static User CreateUser(ulong id, string authCode, bool isAdmin = false)
    {
        var wish = Epam.ItMarathon.ApiService.Domain.ValueObjects.Wish.Wish
            .Create("Test Wish", "https://example.com");

        return User.Create(
            id: id,
            createdOn: DateTime.UtcNow,
            modifiedOn: DateTime.UtcNow,
            roomId: 1,
            authCode: authCode,
            firstName: "Test",
            lastName: "User",
            phone: "+380000000000",
            email: "test@test.com",
            deliveryInfo: "Test address",
            giftRecipientUserId: null,
            wantSurprise: false,
            interests: "",
            isAdmin: isAdmin,
            wishes: new List<Epam.ItMarathon.ApiService.Domain.ValueObjects.Wish.Wish> { wish }
        );
    }

    private static Room CreateRoom(User admin, params User[] others)
    {
        var users = new List<User> { admin };
        users.AddRange(others);

        var roomResult = Room.InitialCreate(
            closedOn: null,
            invitationCode: "ABC",
            name: "Test Room",
            description: "Desc",
            invitationNote: "",
            giftExchangeDate: DateTime.UtcNow,
            giftMaximumBudget: 1000,
            users: users,
            minUsersLimit: 1,
            maxUsersLimit: 10,
            maxWishesLimit: 10
        );

        return roomResult.Value;
    }


    [Fact]
    public async Task Should_Return_NotFound_When_Room_Not_Found()
    {
        // Arrange
        _roomRepository.GetByUserCodeAsync("admin123", _ct)
            .Returns(Task.FromResult(Result.Failure<Room, ValidationResult>(
                new ValidationResult()
            )));

        var handler = new DeleteUserHandler(_roomRepository);

        // Act
        var result = await handler.Handle(new DeleteUserRequest("admin123", 5), _ct);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Return_NotFound_When_ActingUser_Not_In_Room()
    {
        // Arrange
        var admin = CreateUser(1, "admin123", isAdmin: true);
        var other = CreateUser(2, "user999");

        var room = CreateRoom(admin, other);

        _roomRepository.GetByUserCodeAsync("wrongCode", _ct)
            .Returns(Result.Success<Room, ValidationResult>(room));

        var handler = new DeleteUserHandler(_roomRepository);

        // Act
        var result = await handler.Handle(new DeleteUserRequest("wrongCode", 2), _ct);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Return_BadRequest_When_ActingUser_Not_Admin()
    {
        // Arrange
        var notAdmin = CreateUser(1, "code123", isAdmin: false);
        var user = CreateUser(2, "code999");

        var room = CreateRoom(notAdmin, user);

        _roomRepository.GetByUserCodeAsync("code123", _ct)
            .Returns(Result.Success<Room, ValidationResult>(room));

        var handler = new DeleteUserHandler(_roomRepository);

        // Act
        var result = await handler.Handle(new DeleteUserRequest("code123", 2), _ct);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Return_NotFound_When_UserToDelete_Not_Found()
    {
        // Arrange
        var admin = CreateUser(1, "adminABC", isAdmin: true);
        var user = CreateUser(2, "u222");

        var room = CreateRoom(admin, user);

        _roomRepository.GetByUserCodeAsync("adminABC", _ct)
            .Returns(Result.Success<Room, ValidationResult>(room));

        var handler = new DeleteUserHandler(_roomRepository);

        // Act
        var result = await handler.Handle(new DeleteUserRequest("adminABC", 999), _ct);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Return_BadRequest_When_Admin_Tries_Delete_Self()
    {
        // Arrange
        var admin = CreateUser(1, "adminABC", isAdmin: true);
        var user = CreateUser(2, "userX");

        var room = CreateRoom(admin, user);

        _roomRepository.GetByUserCodeAsync("adminABC", _ct)
            .Returns(Result.Success<Room, ValidationResult>(room));

        var handler = new DeleteUserHandler(_roomRepository);

        // Act
        var result = await handler.Handle(new DeleteUserRequest("adminABC", 1), _ct);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Succeed_When_Deleting_Normal_User()
    {
        // Arrange
        var admin = CreateUser(1, "adminABC", isAdmin: true);
        var user = CreateUser(2, "userABC");

        var room = CreateRoom(admin, user);

        _roomRepository.GetByUserCodeAsync("adminABC", _ct)
            .Returns(Result.Success<Room, ValidationResult>(room));

        _roomRepository.UpdateAsync(room, _ct)
            .Returns(Result.Success());

        _roomRepository.GetByUserCodeAsync("adminABC", _ct)
            .Returns(Result.Success<Room, ValidationResult>(room));

        var handler = new DeleteUserHandler(_roomRepository);

        // Act
        var result = await handler.Handle(new DeleteUserRequest("adminABC", 2), _ct);

        // Assert
        result.IsSuccess.Should().BeTrue();
        room.Users.Count.Should().Be(1);
    }
}
