using Microsoft.AspNetCore.Identity;

namespace RealmAuthApi.Security;

public sealed class AspNetPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password)
        => _hasher.HashPassword(new object(), password);

    public bool Verify(string hashed, string password)
        => _hasher.VerifyHashedPassword(new object(), hashed, password)
            is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
}