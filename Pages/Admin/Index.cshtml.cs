using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace GoldContest.Pages.Admin;

public class AdminIndexModel : PageModel
{
    private readonly AppDbContext _context;

    public AdminIndexModel(AppDbContext context)
    {
        _context = context;
    }

    // =============================
    // Entries shown in admin grid
    // =============================
    public List<ContestEntry> Entries { get; set; } = new();

    // =============================
    // Actual gold price (FORM BINDING)
    // =============================
    [BindProperty]
    public decimal ActualGoldPrice { get; set; }

    // =============================
    // INITIAL LOAD
    // =============================
    public async Task<IActionResult> OnGetAsync()
    {
        ProtectAdmin();

        Entries = await _context.ContestEntries
            .Where(e => !e.IsWinner)
            .OrderBy(e => e.Id)
            .ToListAsync();

        return Page();
    }

    // =============================
    // SEARCH WINNER (PREDICTION LOGIC)
    // =============================
    public async Task<IActionResult> OnPostSearchWinnerAsync()
    {
        ProtectAdmin();

        Console.WriteLine($"[ADMIN] SearchWinner called. ActualGoldPrice = {ActualGoldPrice}");

        // Safety guard
        if (ActualGoldPrice <= 0)
        {
            Console.WriteLine("[ADMIN] Invalid gold price entered");
            Entries = new();
            return Page();
        }

        var allEntries = await _context.ContestEntries
            .Where(e => !e.IsWinner)
            .ToListAsync();

        if (!allEntries.Any())
        {
            Console.WriteLine("[ADMIN] No entries available");
            Entries = new();
            return Page();
        }

        // 🔮 Prediction distance algorithm
        var evaluated = allEntries.Select(e => new
        {
            Entry = e,
            Distance =
                ActualGoldPrice < e.LowerRate
                    ? e.LowerRate - ActualGoldPrice
                    : ActualGoldPrice > e.UpperRate
                        ? ActualGoldPrice - e.UpperRate
                        : 0   // Perfect prediction
        }).ToList();

        var minDistance = evaluated.Min(x => x.Distance);

        Entries = evaluated
            .Where(x => x.Distance == minDistance)
            .Select(x => x.Entry)
            .ToList();

        Console.WriteLine($"[ADMIN] Matching entries found = {Entries.Count}");

        return Page();
    }

    // =============================
    // SET WINNER (DB GUARANTEED)
    // =============================
    public async Task<IActionResult> OnPostSetWinnerAsync(int id)
    {
        ProtectAdmin();

        Console.WriteLine($"[ADMIN] SetWinner called for ID = {id}");

        var entry = await _context.ContestEntries.FindAsync(id);
        if (entry != null)
        {
            entry.IsWinner = true;
            await _context.SaveChangesAsync();
            Console.WriteLine($"[ADMIN] Winner set: {entry.AccountName}");
        }

        Entries = await _context.ContestEntries
            .Where(e => !e.IsWinner)
            .OrderBy(e => e.Id)
            .ToListAsync();

        return Page();
    }

    // =============================
    // ADMIN ACCESS PROTECTION
    // =============================
    private void ProtectAdmin()
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            Response.Redirect("/Admin/Login");
        }
    }
}