using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class WinnersModel : PageModel
{
    private readonly AppDbContext _context;

    public WinnersModel(AppDbContext context)
    {
        _context = context;
    }

    public List<ContestEntry> Winners { get; set; } = new();

    public async Task OnGetAsync()
    {
        Winners = await _context.ContestEntries
            .Where(e => e.IsWinner)
            .OrderBy(e => e.AccountNumber)
            .ToListAsync();
    }
}