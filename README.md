# ProfileApi

# Profile API

A RESTful API that fetches demographic data (gender, age, nationality) from external APIs and stores enriched profiles in SQLite.

## 🔗 Links
- **Live API:** `https://your-app.pxxl.app`
- **GitHub Repo:** `https://github.com/yourusername/profile-api`

## 🛠️ Technologies
- ASP.NET Core 9.0
- SQLite + EF Core
- Genderize, Agify, Nationalize APIs

## ⚡ Setup

```bash
git clone https://github.com/yourusername/profile-api.git
cd profile-api
dotnet restore
dotnet run

Endpoints
POST /api/profiles - Create profile
GET /api/profiles/{id} - Get one profile
GET /api/profiles - Get all profiles
DELETE /api/profiles/{id} - Delete profile

Deployment (PXXL)
- Push to GitHub
- Connect to PXXL App
- Set env: HOME=/app/data
- Deploy