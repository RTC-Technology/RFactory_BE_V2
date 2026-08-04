namespace RFactory.Shared.Security;

/// <summary>
/// Abstraction over password hashing so the Application layer does not depend on a
/// specific hashing library.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
