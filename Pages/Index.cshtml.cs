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

    // =====================
    // Form fields
    // =====================
    [BindProperty]
    public string AccountNumber { get; set; } = "";

    [BindProperty]
    public string AccountName { get; set; } = "";

    [BindProperty]
    public decimal LowerRate { get; set; }

    [BindProperty]
    public decimal UpperRate { get; set; }

    // =====================
    // Winners for side table
    // =====================
    public List<ContestEntry> Winners { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Load latest winners
        Winners = await _context.ContestEntries
            .Where(e => e.IsWinner)
            .OrderByDescending(e => e.Id)
            .Take(5)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Prevent duplicate account number until winner is chosen
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