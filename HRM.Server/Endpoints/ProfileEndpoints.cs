using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using HRM.Shared.Models;
using HRM.Shared.DTOs;

namespace HRM.Server.Endpoints;

public static class ProfileEndpoints
{
    public static void MapProfileEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/profile").WithTags("Profile").RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal claims, UserManager<ApplicationUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(claims.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (user is null) return Results.NotFound();
            return Results.Ok(ApiResponse<UpdateProfileRequest>.Success(new UpdateProfileRequest
            {
                FirstName = user.FirstName, LastName = user.LastName, ShortBio = user.ShortBio,
                JobTitle = user.JobTitle, DateOfBirth = user.DateOfBirth, Phone = user.PhoneNumber,
                Address = user.Address, City = user.City, Country = user.Country, PostalCode = user.PostalCode
            }));
        });

        group.MapPut("/", async (UpdateProfileRequest req, ClaimsPrincipal claims, UserManager<ApplicationUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(claims.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (user is null) return Results.NotFound();
            user.FirstName = req.FirstName; user.LastName = req.LastName; user.ShortBio = req.ShortBio;
            user.JobTitle = req.JobTitle; user.DateOfBirth = req.DateOfBirth; user.PhoneNumber = req.Phone;
            user.Address = req.Address; user.City = req.City; user.Country = req.Country; user.PostalCode = req.PostalCode;
            await userManager.UpdateAsync(user);
            return Results.Ok(ApiResponse<string>.Success("Profile updated"));
        });

        group.MapPost("/change-password", async (ChangePasswordRequest req, ClaimsPrincipal claims, UserManager<ApplicationUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(claims.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (user is null) return Results.NotFound();
            var result = await userManager.ChangePasswordAsync(user, req.CurrentPassword, req.NewPassword);
            return result.Succeeded
                ? Results.Ok(ApiResponse<string>.Success("Password changed"))
                : Results.Ok(ApiResponse<string>.Fail(string.Join("; ", result.Errors.Select(e => e.Description))));
        });

        group.MapPost("/avatar", async (HttpRequest request, ClaimsPrincipal claims, UserManager<ApplicationUser> userManager, IWebHostEnvironment env) =>
        {
            var user = await userManager.FindByIdAsync(claims.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (user is null) return Results.NotFound();

            var form = await request.ReadFormAsync();
            var file = form.Files.FirstOrDefault();
            if (file is null || file.Length == 0) return Results.BadRequest("No file");

            var avatarDir = Path.Combine(env.WebRootPath ?? "wwwroot", "avatars");
            Directory.CreateDirectory(avatarDir);
            var fileName = $"{user.Id}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(avatarDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            user.AvatarUrl = $"/avatars/{fileName}";
            await userManager.UpdateAsync(user);
            return Results.Ok(ApiResponse<string>.Success(user.AvatarUrl));
        }).DisableAntiforgery();
    }
}
