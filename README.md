# Gold Price Prediction Contest Web Application

A robust ASP.NET Core Razor Pages application for managing a weekly gold price prediction contest. Users submit their gold price range predictions, and administrators select winners based on the closest match to the actual gold price using an intelligent distance-based algorithm.

<img width="1456" height="831" alt="Screenshot 2026-02-12 at 1 11 54 AM" src="https://github.com/user-attachments/assets/fddd120e-5b52-47ee-aa77-1ca724f19a80" />


## Features

### Contest Entry
- Simple, elegant form for users to submit gold price range predictions (lower and upper rate)
- Duplicate account number detection to prevent multiple entries per round
- Real-time toast notifications for submission feedback

### Winner Management
- **Single winner at a time** -- selecting a new winner automatically removes the previous one
- **7-day auto-expiry** -- winners are automatically cleared after one week via a background service, allowing a new round to begin
- Admin can manually remove a winner at any time
- Winner spotlight page with animated display

### Admin Dashboard
- Secure session-based admin login
- Search for the best prediction by entering the actual gold price
- Distance-based algorithm finds the closest prediction(s)
- Current winner displayed in a sidebar card with expiry countdown
- Quick stats overview (entry count, active winner)
- One-click "Set Winner" and "Remove Winner" actions

### Multilanguage (English / Arabic)
- Language toggle (EN / Arabic) on all public pages
- Full RTL layout support when Arabic is selected
- Arabic font rendering via Google Noto Sans Arabic
- Language preference persists across pages via `localStorage`
- Server responses include both English and Arabic messages

### UI
- Premium dark theme with gold accents throughout
- Playfair Display serif headings with gold gradient text
- Responsive design for mobile and desktop
- Animated winner banner on the home page
- Confetti and glowing trophy effects on the winners page

## Technology Stack

- **Framework**: ASP.NET Core 8 (Razor Pages)
- **Database**: MySQL with Entity Framework Core
- **ORM Package**: MySql.EntityFrameworkCore
- **Frontend**: Razor Pages, custom CSS, Google Fonts
- **Language**: C#

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or later)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/) (running locally)

## Installation & Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/ashaheem32/Gold-Price-Prediction-Contest-Web-Application.git
   cd Gold-Price-Prediction-Contest-Web-Application
   ```

2. **Configure the database connection**

   Edit `appsettings.json` with your MySQL credentials:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=goldcontestdb;User=root;Password=YourPassword;"
     }
   }
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```
   The database and tables are created automatically on first run via `EnsureCreated()`.

4. **Open in your browser**
   ```
   http://localhost:5100
   ```

## Project Structure

```
GoldContest/
├── Data/
│   └── AppDbContext.cs            # EF Core database context
├── Models/
│   └── ContestEntry.cs            # Contest entry entity (with WonAt for expiry)
├── Pages/
│   ├── Index.cshtml / .cs         # Home page -- contest entry form
│   ├── Winners.cshtml / .cs       # Public winner spotlight page
│   ├── Error.cshtml / .cs         # Error page
│   ├── _ViewImports.cshtml        # Tag helper imports
│   └── Admin/
│       ├── Index.cshtml / .cs     # Admin dashboard (entries, search, winner sidebar)
│       ├── Login.cshtml / .cs     # Admin login
│       └── Logout.cshtml / .cs    # Admin logout
├── Properties/
│   └── launchSettings.json        # Launch profile (port 5100)
├── wwwroot/
│   └── Dollar.png                 # Background image
├── Program.cs                     # App startup + WinnerExpiryService
├── appsettings.json               # Connection string + admin password
└── GoldContest.csproj             # Project file
```

## Default Admin Credentials

The admin password is configured in `appsettings.json` under `AdminSettings:Password`. The default value is:

```
admin@123
```

Access the admin panel at `/Admin/Login`.

## How Winner Selection Works

1. Admin enters the **actual gold price** in the dashboard
2. The algorithm calculates the distance from each entry's predicted range:
   - If the actual price falls within the range: distance = 0 (perfect prediction)
   - If below the range: distance = lower rate - actual price
   - If above the range: distance = actual price - upper rate
3. Entries with the minimum distance are shown as candidates
4. Admin clicks "Set Winner" -- the previous winner (if any) is automatically removed
5. After 7 days, the winner expires automatically and the cycle resets

## Contributing

Contributions are welcome. Fork the repository and create a pull request for any enhancements or bug fixes.

1. Fork the project
2. Create your feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add your feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Open a pull request

## License

Distributed under the MIT License. See `LICENSE` for more information.

## Contact

Shaheem - [GitHub Profile](https://github.com/ashaheem32)
