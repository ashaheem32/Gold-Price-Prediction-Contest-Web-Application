# Gold Price Prediction Contest Web Application 🏆

A robust ASP.NET Core 8 Razor Pages application designed to manage a gold price prediction contest. This system allows users to participate by predicting gold prices, and enables administrators to manage entries and automatically select winners based on the closest accurate predictions using intelligent range-based matching logic.

![Gold Contest Banner](Must_be_uploaded_to_an_image_hosting_service_or_kept_local_if_not_available)

## 🚀 Features

- **User Participation**: Simple interface for users to submit their gold price predictions.
- **Admin Dashboard**: Secure administrative area to oversee the contest.
- **Automated Winner Selection**: Logic to automatically calculate and identify winners based on the actual gold price.
- **Range-Based Matching**: Advanced algorithm to find the closest predictions.
- **Multilingual Support**: (If applicable) Built-in support for multiple languages.
- **Secure Authentication**: Admin login protection.

## 🛠️ Technology Stack

- **Framework**: .NET 8 (ASP.NET Core Razor Pages)
- **Database**: MySQL (Entity Framework Core)
- **Frontend**: Razor Pages, CSS, Bootstrap (presumed)
- **Language**: C#

## 📋 Prerequisites

Before you begin, ensure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/)

## ⚙️ Installation & Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/ashaheem32/Gold-Price-Prediction-Contest-Web-Application.git
   cd Gold-Price-Prediction-Contest-Web-Application
   ```

2. **Configure Database**
   Update the `appsettings.json` file with your MySQL connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=goldcontestdb;User=root;Password=YourPassword;"
   }
   ```

3. **Run the Application**
   ```bash
   dotnet run
   ```
   The application will start, and the database will be created automatically if it doesn't exist (ensure your MySQL server is running).

4. **Access the App**
   Open your browser and navigate to `http://localhost:5000` (or the port shown in your terminal).

## 📂 Project Structure

- `Pages/`: Contains the Razor Pages for the UI.
- `Models/`: Database entities and data models.
- `Data/`: DB Context and data access layer.
- `wwwroot/`: Static assets (CSS, JS, Images).

## 🤝 Contributing

Contributions are welcome! Please fork the repository and create a pull request for any enhancements or bug fixes.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.

## 📞 Contact

Shaheem - [Profile](https://github.com/ashaheem32)

---
