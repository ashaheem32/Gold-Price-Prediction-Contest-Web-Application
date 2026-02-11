using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class WinnersModel : PageModel
{
    private readonly AppDbContext _context;

    public WinnersModel(AppDbContext context)
    {
        _context = context;
    }

    public ContestEntry? CurrentWinner { get; set; }

    public async Task OnGetAsync()
    {
        // Auto-expire winners older than 7 days
        var expired = await _context.ContestEntries
            .Where(e => e.IsWinner && e.WonAt != null && e.WonAt < DateTime.Now.AddDays(-7))
            .ToListAsync();

        if (expired.Any())
        {
            foreach (var w in expired)
            {
                w.IsWinner = false;
                w.WonAt = null;
            }
            await _context.SaveChangesAsync();
        }

        CurrentWinner = await _context.ContestEntries
            .Where(e => e.IsWinner)
            .FirstOrDefaultAsync();
    }
}
