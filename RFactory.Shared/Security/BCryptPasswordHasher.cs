using BC = BCrypt.Net.BCrypt;

namespace RFactory.Shared.Security;

/// <summary>
/// BCrypt-based <see cref="IPasswordHasher"/>. Work factor 12 balances brute-force
/// resistance with acceptable login latency.
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string password) => BC.HashPassword(password, WorkFactor);

    public bool Verify(string password, string hash) => BC.Verify(password, hash);
}
