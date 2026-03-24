using Titan.Library.Application.Auth;
using Titan.Library.Common.Auth;
using Titan.Library.Common.EndPoints;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.UnitTests.Auth;

public class RegisterCommandHandlerTestCases
{
    #region Fakes

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly User? _user;

        public FakeUserRepository(User? user = null) => _user = user;

        public Task<User?> FindByEmail(string email) => Task.FromResult(_user);

        public Task<int> Add(User entity) => Task.FromResult(1);

        public Task Update(User entity) => Task.CompletedTask;

        public Task Delete(User entity) => Task.CompletedTask;

        public Task<IEnumerable<User>> ToList() => Task.FromResult(Enumerable.Empty<User>());

        public Task<User?> FindById(int id) => Task.FromResult<User?>(null);
    }

    private sealed class FakeCustomerRepository : ICustomerRepository
    {
        public Task<int> Add(Customer entity) => Task.FromResult(42);

        public Task Update(Customer entity) => Task.CompletedTask;

        public Task Delete(Customer entity) => Task.CompletedTask;

        public Task<IEnumerable<Customer>> ToList() =>
            Task.FromResult(Enumerable.Empty<Customer>());

        public Task<Customer?> FindById(int id) => Task.FromResult<Customer?>(null);

        public Task<Customer?> FindByEmail(string email) => Task.FromResult<Customer?>(null);
    }

    private sealed class FakeAuthorRepository : IAuthorRepository
    {
        public Task<int> Add(Author entity) => Task.FromResult(99);

        public Task Update(Author entity) => Task.CompletedTask;

        public Task Delete(Author entity) => Task.CompletedTask;

        public Task<IEnumerable<Author>> ToList() => Task.FromResult(Enumerable.Empty<Author>());

        public Task<Author?> FindById(int id) => Task.FromResult<Author?>(null);

        public Task<Author?> FindByEmail(string email) => Task.FromResult<Author?>(null);
    }

    private sealed class FakeJwtGenerator : IJwtGenerator
    {
        public string Generate(int userId, string userType) =>
            $"fake-token-{userId}-{userType}";
    }

    #endregion

    private RegisterCommandHandler CreateHandler(User? existingUser = null) =>
        new(
            new FakeCustomerRepository(),
            new FakeAuthorRepository(),
            new FakeJwtGenerator(),
            new FakeUserRepository(existingUser)
        );

    // ── Validator ──────────────────────────────────────────────────────────

    [Fact]
    public void Validate_EmptyName_FailsWithNameRequired()
    {
        var validator = new RegisterCommandValidator();

        var result = validator.Validate(
            new RegisterCommand
            {
                Email = "user@example.com",
                Password = "secret",
                UserType = UserTypeValues.Customer,
            }
        );

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationMessageKeys.AUTH_NAME_REQUIRED, result.MessageCode);
    }

    [Fact]
    public void Validate_EmptyEmail_FailsWithEmailRequired()
    {
        var validator = new RegisterCommandValidator();

        var result = validator.Validate(
            new RegisterCommand
            {
                Name = "Alice",
                Password = "secret",
                UserType = UserTypeValues.Customer,
            }
        );

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationMessageKeys.AUTH_EMAIL_REQUIRED, result.MessageCode);
    }

    [Fact]
    public void Validate_EmptyPassword_FailsWithPasswordRequired()
    {
        var validator = new RegisterCommandValidator();

        var result = validator.Validate(
            new RegisterCommand
            {
                Name = "Alice",
                Email = "user@example.com",
                UserType = UserTypeValues.Customer,
            }
        );

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationMessageKeys.AUTH_PASSWORD_REQUIRED, result.MessageCode);
    }

    [Fact]
    public void Validate_EmptyUserType_FailsWithUserTypeRequired()
    {
        var validator = new RegisterCommandValidator();

        var result = validator.Validate(
            new RegisterCommand
            {
                Name = "Alice",
                Email = "user@example.com",
                Password = "secret",
            }
        );

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationMessageKeys.AUTH_USER_TYPE_REQUIRED, result.MessageCode);
    }

    [Fact]
    public void Validate_AdminUserType_FailsWithAdminNotAllowed()
    {
        var validator = new RegisterCommandValidator();

        var result = validator.Validate(
            new RegisterCommand
            {
                Name = "Alice",
                Email = "user@example.com",
                Password = "secret",
                UserType = UserTypeValues.Admin,
            }
        );

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ApplicationMessageKeys.AUTH_ADMIN_REGISTRATION_NOT_ALLOWED,
            result.MessageCode
        );
    }

    [Fact]
    public void Validate_UnknownUserType_FailsWithInvalidUserType()
    {
        var validator = new RegisterCommandValidator();

        var result = validator.Validate(
            new RegisterCommand
            {
                Name = "Alice",
                Email = "user@example.com",
                Password = "secret",
                UserType = "moderator",
            }
        );

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationMessageKeys.AUTH_INVALID_USER_TYPE, result.MessageCode);
    }

    [Fact]
    public void Validate_ValidCustomerCommand_Succeeds()
    {
        var validator = new RegisterCommandValidator();

        var result = validator.Validate(
            new RegisterCommand
            {
                Name = "Alice",
                Email = "alice@example.com",
                Password = "secret",
                UserType = UserTypeValues.Customer,
            }
        );

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Validate_ValidAuthorCommand_Succeeds()
    {
        var validator = new RegisterCommandValidator();

        var result = validator.Validate(
            new RegisterCommand
            {
                Name = "Bob",
                Email = "bob@example.com",
                Password = "secret",
                UserType = UserTypeValues.Author,
            }
        );

        Assert.True(result.IsSuccess);
    }

    // ── Handler ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_UserAlreadyRegistered_FailsWithAlreadyRegistered()
    {
        var existingUser = Customer.Create("Alice", "alice@example.com", "secret");
        var handler = CreateHandler(existingUser);

        var result = await handler.Handle(
            new RegisterCommand
            {
                Name = "Alice",
                Email = "alice@example.com",
                Password = "secret",
                UserType = UserTypeValues.Customer,
            },
            CancellationToken.None
        );

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationMessageKeys.USER_ALREADY_REGISTERED, result.MessageCode);
    }

    [Fact]
    public async Task Handle_NewCustomerRegistration_SucceedsWithToken()
    {
        var handler = CreateHandler(existingUser: null);

        var result = await handler.Handle(
            new RegisterCommand
            {
                Name = "Alice",
                Email = "alice@example.com",
                Password = "secret",
                UserType = UserTypeValues.Customer,
            },
            CancellationToken.None
        );

        Assert.True(result.IsSuccess);
        Assert.Equal(ApplicationMessageKeys.AUTH_REGISTER_SUCCESS, result.MessageCode);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data.Token);
        Assert.Equal("Customer", result.Data.UserType);
    }

    [Fact]
    public async Task Handle_NewAuthorRegistration_SucceedsWithToken()
    {
        var handler = CreateHandler(existingUser: null);

        var result = await handler.Handle(
            new RegisterCommand
            {
                Name = "Bob",
                Email = "bob@example.com",
                Password = "secret",
                UserType = UserTypeValues.Author,
            },
            CancellationToken.None
        );

        Assert.True(result.IsSuccess);
        Assert.Equal(ApplicationMessageKeys.AUTH_REGISTER_SUCCESS, result.MessageCode);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data.Token);
        Assert.Equal("Author", result.Data.UserType);
    }
}
