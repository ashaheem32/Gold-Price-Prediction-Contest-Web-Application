using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GoldContest.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string AccountNumber { get; set; } = "";

    [BindProperty]
    public string AccountName { get; set; } = "";

    [BindProperty]
    public decimal LowerRate { get; set; }

    [BindProperty]
    public decimal UpperRate { get; set; }

    // Single current winner
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

    public async Task<IActionResult> OnPostAsync()
    {
        bool exists = await _context.ContestEntries
            .AnyAsync(x => x.AccountNumber == AccountNumber && !x.IsWinner);

        if (exists)
        {
            return new JsonResult(new
            {
                success = false,
                messageEn = "This account number has already entered the contest",
                messageAr = "رقم الحساب هذا مشارك بالفعل في المسابقة"
            });
        }

        var entry = new ContestEntry
        {
            AccountNumber = AccountNumber,
            AccountName = AccountName,
            LowerRate = LowerRate,
            UpperRate = UpperRate,
            IsWinner = false,
            CreatedAt = DateTime.Now
        };

        _context.ContestEntries.Add(entry);
        await _context.SaveChangesAsync();

        return new JsonResult(new
        {
            success = true,
            messageEn = "Your entry for the weekly contest has been submitted",
            messageAr = "تم إرسال مشاركتك في المسابقة الأسبوعية"
        });
    }
}
