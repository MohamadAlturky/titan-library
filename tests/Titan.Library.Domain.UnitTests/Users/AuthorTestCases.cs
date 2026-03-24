using Titan.Library.Domain.Users;

namespace Titan.Library.Domain.UnitTests.Users;

public class AuthorTestCases
{
    // ── Create ─────────────────────────────────────────────────────────────

    [Fact]
    public void Create_SetsNameEmailAndUserType()
    {
        var author = Author.Create("Bob", "bob@example.com", "secret");

        Assert.Equal("Bob", author.Name);
        Assert.Equal("bob@example.com", author.Email);
        Assert.Equal(UserType.Author, author.UserType);
    }

    [Fact]
    public void Create_HashesPassword()
    {
        var author = Author.Create("Bob", "bob@example.com", "secret");

        Assert.NotEmpty(author.PasswordHash);
        Assert.NotEmpty(author.PasswordSalt);
    }

    [Fact]
    public void Create_PasswordVerifiesCorrectly()
    {
        var author = Author.Create("Bob", "bob@example.com", "secret");

        Assert.True(author.VerifyPassword("secret"));
    }

    [Fact]
    public void Create_WrongPasswordDoesNotVerify()
    {
        var author = Author.Create("Bob", "bob@example.com", "secret");

        Assert.False(author.VerifyPassword("wrong"));
    }

    [Fact]
    public void Create_SetsCreatedAt()
    {
        var before = DateTime.UtcNow;
        var author = Author.Create("Bob", "bob@example.com", "secret");
        var after = DateTime.UtcNow;

        Assert.InRange(author.CreatedAt, before, after);
    }

    [Fact]
    public void Create_StartsActive()
    {
        var author = Author.Create("Bob", "bob@example.com", "secret");

        Assert.True(author.IsActive);
        Assert.False(author.IsDeleted);
    }

    [Fact]
    public void Create_StartsWithNoBooks()
    {
        var author = Author.Create("Bob", "bob@example.com", "secret");

        Assert.Empty(author.Books);
    }

    // ── SetPassword ────────────────────────────────────────────────────────

    [Fact]
    public void SetPassword_UpdatesHashAndSalt()
    {
        var author = Author.Create("Bob", "bob@example.com", "original");
        var oldHash = author.PasswordHash;
        var oldSalt = author.PasswordSalt;

        author.SetPassword("newpassword");

        Assert.NotEqual(oldHash, author.PasswordHash);
        Assert.NotEqual(oldSalt, author.PasswordSalt);
    }

    [Fact]
    public void SetPassword_NewPasswordVerifiesCorrectly()
    {
        var author = Author.Create("Bob", "bob@example.com", "original");

        author.SetPassword("newpassword");

        Assert.True(author.VerifyPassword("newpassword"));
        Assert.False(author.VerifyPassword("original"));
    }

    // ── Soft delete / activate ─────────────────────────────────────────────

    [Fact]
    public void Delete_SetsIsDeleted()
    {
        var author = Author.Create("Bob", "bob@example.com", "secret");

        author.Delete();

        Assert.True(author.IsDeleted);
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        var author = Author.Create("Bob", "bob@example.com", "secret");

        author.Deactivate();

        Assert.False(author.IsActive);
    }

    [Fact]
    public void Activate_SetsIsActiveToTrue()
    {
        var author = Author.Create("Bob", "bob@example.com", "secret");
        author.Deactivate();

        author.Activate();

        Assert.True(author.IsActive);
    }

    // ── RepresentUserTypeString ────────────────────────────────────────────

    [Fact]
    public void RepresentUserTypeString_ReturnsAuthor()
    {
        var author = Author.Create("Bob", "bob@example.com", "secret");

        Assert.Equal("Author", author.RepresentUserTypeString());
    }

    // ── TakeSnapshot ──────────────────────────────────────────────────────

    [Fact]
    public void TakeSnapshot_CapturesAllFields()
    {
        var author = Author.Create("Bob", "bob@example.com", "secret");
        author.Id = 7;

        var snapshot = author.TakeSnapshot();

        Assert.Equal(7, snapshot.Id);
        Assert.Equal("Bob", snapshot.Name);
        Assert.Equal("bob@example.com", snapshot.Email);
        Assert.Equal(author.PasswordHash, snapshot.PasswordHash);
        Assert.Equal(author.PasswordSalt, snapshot.PasswordSalt);
        Assert.Equal(author.CreatedAt, snapshot.CreatedAt);
        Assert.Equal(author.IsDeleted, snapshot.IsDeleted);
        Assert.Equal(author.IsActive, snapshot.IsActive);
        Assert.Equal(UserType.Author, snapshot.UserType);
    }

    // ── Reconstitute ──────────────────────────────────────────────────────

    [Fact]
    public void Reconstitute_RestoresAllFields()
    {
        var original = Author.Create("Bob", "bob@example.com", "secret");
        original.Id = 7;
        original.Delete();
        var snapshot = original.TakeSnapshot();

        var restored = Author.Reconstitute(snapshot);

        Assert.Equal(7, restored.Id);
        Assert.Equal("Bob", restored.Name);
        Assert.Equal("bob@example.com", restored.Email);
        Assert.Equal(original.PasswordHash, restored.PasswordHash);
        Assert.Equal(original.PasswordSalt, restored.PasswordSalt);
        Assert.Equal(original.CreatedAt, restored.CreatedAt);
        Assert.True(restored.IsDeleted);
        Assert.Equal(UserType.Author, restored.UserType);
    }

    [Fact]
    public void Reconstitute_PasswordVerifiesAfterRestore()
    {
        var original = Author.Create("Bob", "bob@example.com", "secret");
        var snapshot = original.TakeSnapshot();

        var restored = Author.Reconstitute(snapshot);

        Assert.True(restored.VerifyPassword("secret"));
    }
}
