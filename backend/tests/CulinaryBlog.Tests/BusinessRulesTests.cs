using System.Security.Claims;
using System.Security.Cryptography;
using CulinaryBlog.Application.Common.Authorization;
using CulinaryBlog.Application.Common.Helpers;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Xunit;
namespace CulinaryBlog.Tests;
public class BusinessRulesTests
{
    [Fact] public void EmptyRecipeCannotBePublished() { var r = new Recipe(); Assert.Throws<InvalidOperationException>(() => r.Publish()); Assert.Equal(RecipeStatus.Draft, r.Status); }
    [Fact] public void PublishingTwiceKeepsOriginalPublicationDate() { var r = new Recipe { Steps = [new RecipeStep()] }; r.Publish(); var date = r.PublishedAt; r.Publish(); Assert.Equal(date, r.PublishedAt); }
    [Theory] [InlineData(false, "Author", false)] [InlineData(true, "Author", true)] [InlineData(false, "Admin", true)]
    public void OwnershipIsRequiredUnlessAdmin(bool owner, string role, bool allowed) { var author = Guid.NewGuid(); var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, (owner ? author : Guid.NewGuid()).ToString()),new Claim(ClaimTypes.Role,role)],"test")); Assert.Equal(allowed,RecipePermission.CanEdit(user,author)); }
    [Fact] public void AnonymousWithForgedClaimsCannotEdit() { var id=Guid.NewGuid(); var user=new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier,id.ToString())]));Assert.False(RecipePermission.CanEdit(user,id)); }
    [Fact] public void PasswordHashesAreSaltedAndVerifiable() { var h=new PasswordHasher();var a=h.HashPassword("Kitchen_Test_2026!");var b=h.HashPassword("Kitchen_Test_2026!");Assert.NotEqual(a,b);Assert.True(h.VerifyPassword("Kitchen_Test_2026!",a));Assert.False(h.VerifyPassword("wrong",a)); }
    [Theory] [InlineData("not.base64!")] [InlineData("pbkdf2-sha256$999999999$bad$bad")] [InlineData("")]
    public void CorruptPasswordHashNeverThrows(string value) => Assert.False(new PasswordHasher().VerifyPassword("test",value));
    [Fact] public void ExistingLegacyAccountsStillAuthenticate() { var salt=new byte[16];var hash=Rfc2898DeriveBytes.Pbkdf2("legacy",salt,100000,HashAlgorithmName.SHA256,32);Assert.True(new PasswordHasher().VerifyPassword("legacy",Convert.ToBase64String(salt)+"."+Convert.ToBase64String(hash))); }
    [Fact] public async Task RiffAudioIsNotAWebPImage() { using var data=new MemoryStream("RIFF1234WAVEpayload"u8.ToArray());var file=new FormFile(data,0,data.Length,"file","bad.webp"){Headers=new HeaderDictionary(),ContentType="image/webp"};Assert.False((await FileValidationHelper.ValidateImageAsync(file)).IsValid); }
    [Fact] public void TokenDigestDoesNotExposeTheRawToken() { var raw=Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));var digest=TokenHash.Compute(raw);Assert.Equal(64,digest.Length);Assert.NotEqual(raw,digest);Assert.Equal(digest,TokenHash.Compute(raw)); }
}
