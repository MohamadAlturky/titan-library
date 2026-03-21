namespace Titan.Library.Infrastructure.Configurations;

public static class UserTableConfiguration
{
    public const string Table = "users";

    public static class Columns
    {
        public const string Id = "id";
        public const string Name = "name";
        public const string Email = "email";
        public const string PasswordHash = "password_hash";
        public const string PasswordSalt = "password_salt";
        public const string CreatedAt = "created_at";
        public const string IsDeleted = "is_deleted";
        public const string IsActive = "is_active";
    }

    public static class AuthorTable
    {
        public const string Table = "authors";
        public const string UserId = "user_id";
    }

    public static class CustomerTable
    {
        public const string Table = "customers";
        public const string UserId = "user_id";
    }

    public static class AdminTable
    {
        public const string Table = "admins";
        public const string UserId = "user_id";
    }
}
