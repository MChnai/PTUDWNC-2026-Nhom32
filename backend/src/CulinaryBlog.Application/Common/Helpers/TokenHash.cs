using System.Security.Cryptography;
using System.Text;

namespace CulinaryBlog.Application.Common.Helpers;

public static class TokenHash
{
    // Keep the existing database columns; store only SHA-256 digests in them.
    public static string Compute(string token) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
