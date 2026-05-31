using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;

namespace OweMeApi.Modules.FriendCodes;

public class FriendCodesService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public string GenerateFriendCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        return new string([.. Enumerable.Repeat(chars, 6).Select(s => s[Random.Shared.Next(s.Length)])]);
    }

    public async Task DeleteExpiredCodes()
    {
        var expiredCodes = await _context.FriendCodes
            .Where(c => c.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        _context.FriendCodes.RemoveRange(expiredCodes);
        await _context.SaveChangesAsync();
    }
}
