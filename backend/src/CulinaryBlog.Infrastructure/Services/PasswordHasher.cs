using System.Security.Cryptography;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
namespace CulinaryBlog.Infrastructure.Services;
public class PasswordHasher : IPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<ApplicationUser> identity = new(Options.Create(new PasswordHasherOptions { IterationCount = 600000 }));
    public string HashPassword(string password) => identity.HashPassword(new ApplicationUser(), password);
    public bool VerifyPassword(string password, string encoded)
    {
        if (string.IsNullOrWhiteSpace(encoded)) return false;
        try
        {
            if (!encoded.Contains('.') && !encoded.StartsWith("pbkdf2-", StringComparison.Ordinal))
                return identity.VerifyHashedPassword(new ApplicationUser(), encoded, password) != PasswordVerificationResult.Failed;
            byte[] salt, expected; int iterations;
            if (encoded.StartsWith("pbkdf2-sha256$", StringComparison.Ordinal))
            {
                var p = encoded.Split('$'); if (p.Length != 4 || !int.TryParse(p[1], out iterations) || iterations < 100000 || iterations > 2000000) return false;
                salt = Convert.FromBase64String(p[2]); expected = Convert.FromBase64String(p[3]);
            }
            else
            {
                var p = encoded.Split('.'); if (p.Length != 2) return false;
                salt = Convert.FromBase64String(p[0]); expected = Convert.FromBase64String(p[1]); iterations = 100000;
            }
            if (salt.Length != 16 || expected.Length != 32) return false;
            return CryptographicOperations.FixedTimeEquals(expected,
                Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, 32));
        }
        catch (FormatException) { return false; }
        catch (ArgumentException) { return false; }
    }
}
