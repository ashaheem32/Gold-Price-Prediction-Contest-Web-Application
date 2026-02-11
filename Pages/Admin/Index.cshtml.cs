using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GoldContest.Pages.Admin;

public class AdminIndexModel : PageModel
{
    private readonly AppDbContext _context;

    public AdminIndexModel(AppDbContext context)
    {
        _context = context;
    }

    // Entries shown in admin grid
    public List<ContestEntry> Entries { get; set; } = new();

    // Current winner (only one at a time)
    public ContestEntry? CurrentWinner { get; set; }

    // Actual gold price (form binding)
    [BindProperty]
    public decimal ActualGoldPrice { get; set; }

    // INITIAL LOAD
    public async Task<IActionResult> OnGetAsync()
    {
        if (!IsAdmin()) return RedirectToPage("/Admin/Login");

        await ExpireOldWinners();
        await LoadData();
        return Page();
    }

    // SEARCH WINNER (prediction logic)
    public async Task<IActionResult> OnPostSearchWinnerAsync()
    {
        if (!IsAdmin()) return RedirectToPage("/Admin/Login");

        await ExpireOldWinners();

        if (ActualGoldPrice <= 0)
        {
            Entries = new();
            await LoadCurrentWinner();
            return Page();
        }

        var allEntries = await _context.ContestEntries
            .Where(e => !e.IsWinner)
            .ToListAsync();

        if (!allEntries.Any())
        {
            Entries = new();
            await LoadCurrentWinner();
            return Page();
        }

        // Prediction distance algorithm
        var evaluated = allEntries.Select(e => new
        {
            Entry = e,
            Distance =
                ActualGoldPrice < e.LowerRate
                    ? e.LowerRate - ActualGoldPrice
                    : ActualGoldPrice > e.UpperRate
                        ? ActualGoldPrice - e.UpperRate
                        : 0
        }).ToList();

        var minDistance = evaluated.Min(x => x.Distance);

        Entries = evaluated
            .Where(x => x.Distance == minDistance)
            .Select(x => x.Entry)
            .ToList();

        await LoadCurrentWinner();
        return Page();
    }

    // SET WINNER -- removes previous winner first, only one at a time
    public async Task<IActionResult> OnPostSetWinnerAsync(int id)
    {
        if (!IsAdmin()) return RedirectToPage("/Admin/Login");

        // Remove any existing winners first
        var existingWinners = await _context.ContestEntries
            .Where(e => e.IsWinner)
            .ToListAsync();

        foreach (var w in existingWinners)
        {
            w.IsWinner = false;
            w.WonAt = null;
        }

        // Set the new winner
        var entry = await _context.ContestEntries.FindAsync(id);
        if (entry != null)
        {
            entry.IsWinner = true;
            entry.WonAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        await LoadData();
        return Page();
    }

    // DELETE / REMOVE WINNER
    public async Task<IActionResult> OnPostRemoveWinnerAsync(int id)
    {
        if (!IsAdmin()) return RedirectToPage("/Admin/Login");

        var entry = await _context.ContestEntries.FindAsync(id);
        if (entry != null)
        {
            entry.IsWinner = false;
            entry.WonAt = null;
            await _context.SaveChangesAsync();
        }

        await LoadData();
        return Page();
    }

    // Helper: check admin session
    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("IsAdmin") == "true";
    }

    // Helper: load all page data
    private async Task LoadData()
    {
        Entries = await _context.ContestEntries
            .Where(e => !e.IsWinner)
            .OrderBy(e => e.Id)
            .ToListAsync();

        await LoadCurrentWinner();
    }

    // Helper: load current winner
    private async Task LoadCurrentWinner()
    {
        CurrentWinner = await _context.ContestEntries
            .Where(e => e.IsWinner)
            .FirstOrDefaultAsync();
    }

    // Helper: auto-expire winners older than 7 days
    private async Task ExpireOldWinners()
    {
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
    }
}
