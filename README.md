# Razor Pages Web App — Quick Start

This project is an ASP.NET Core Razor Pages application. Follow these steps to run it locally and configure your own connection string.

***

## ✅ 1. Clone the repository

```bash
git clone <YOUR_REPO_URL>
cd WEB_APP
```

***

## ✅ 2. Place your connection string

Open **`appsettings.json`** and find:

```json
"ConnectionStrings": {
  "DefaultConnection": "<YOUR CONNECTION STRING HERE>"
}
```

Replace the empty string with **your own connection string**.

***

## ✅ 3. Restore dependencies

```bash
dotnet restore
```

***

## ✅ 4. Run the app with live reload

```bash
dotnet watch
```

This will start the app and automatically reload on code changes. The console will show the local URL (e.g., `https://localhost:5001`). Open it in your browser.

***

### ✅ Notes

*   If you want to use `.env` instead of editing `appsettings.json`, create a `.env` file with:
        ConnectionStrings__DefaultConnection=your-connection-string-here
    and load it in `Program.cs` using `DotNetEnv` as shown earlier.
*   Do **not** commit secrets or `.env` to Git. Add `.env` to `.gitignore`.

***