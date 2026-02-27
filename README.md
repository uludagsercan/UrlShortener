# 🔗 URL Shortener

A modern, high-performance URL shortening service built with .NET 8, React, PostgreSQL, Redis, Docker, and Nginx.

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![React](https://img.shields.io/badge/React-18-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)
![Redis](https://img.shields.io/badge/Redis-7-red)
![Docker](https://img.shields.io/badge/Docker-Compose-blue)
![Nginx](https://img.shields.io/badge/Nginx-Reverse%20Proxy-green)

---

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Prerequisites](#-prerequisites)
- [Installation](#-installation)
- [Usage](#-usage)
- [API Reference](#-api-reference)
- [Project Structure](#-project-structure)
- [Docker Commands](#-docker-commands)
- [Database Commands](#-database-commands)

---

## ✨ Features

- 🔗 Shorten long URLs into compact short codes
- 👤 User registration and authentication with JWT
- 🔒 Link management for authenticated users
- 🌐 Anonymous URL shortening (no account required)
- 📊 Click count tracking and analytics
- ⚡ High-performance caching with Redis
- 🎯 Custom alias support
- ⏰ Link expiry support
- 🐳 Fully containerized with Docker
- 🔀 Nginx reverse proxy

---

## 🏗️ Architecture

```
Browser
   ↓
Nginx (Port 80) ← Reverse Proxy
   ↓         ↓
Frontend    .NET 8 API
(React)   (Clean Architecture)
               ↓         ↓
            Redis      PostgreSQL
           (Cache)      (Main DB)
```

### Clean Architecture Layers

```
UrlShortener.API            → HTTP Endpoints, Middleware, DI
UrlShortener.Application    → Business Logic, CQRS, Use Cases
UrlShortener.Domain         → Entities, Interfaces, Value Objects
UrlShortener.Infrastructure → DB, Redis, Repository Implementations
```

### Request Flow

```
POST /api/urls
      ↓
  API Endpoint
      ↓
  MediatR → CreateShortUrlHandler
      ↓
  Generate unique short code
      ↓
  Save to PostgreSQL
      ↓
  Return Short URL

GET /{shortCode}
      ↓
  Check Redis (< 1ms)
      ↓ (cache miss)
  Query PostgreSQL
      ↓
  Cache in Redis (1hr TTL)
      ↓
  Record Click
      ↓
  HTTP 302 Redirect
```

---

## 🛠️ Tech Stack

### Backend
| Technology | Version | Purpose |
|-----------|---------|---------|
| .NET | 8.0 | API Framework |
| Entity Framework Core | 8.0 | ORM |
| MediatR | 12.2 | CQRS Pattern |
| ASP.NET Core Identity | 8.0 | User Management |
| JWT Bearer | 8.0 | Authentication |
| PostgreSQL | 16 | Primary Database |
| Redis | 7 | Caching Layer |
| Npgsql | 8.0 | PostgreSQL Driver |

### Frontend
| Technology | Version | Purpose |
|-----------|---------|---------|
| React | 18 | UI Framework |
| TypeScript | 5 | Type Safety |
| Vite | 5 | Build Tool |
| Tailwind CSS | 4 | Styling |
| Axios | 1.x | HTTP Client |
| Zustand | 4.x | State Management |
| React Router | 6 | Client-side Routing |

### DevOps
| Technology | Purpose |
|-----------|---------|
| Docker | Containerization |
| Docker Compose | Multi-container Orchestration |
| Nginx | Reverse Proxy & Static File Serving |

---

## 📦 Prerequisites

Make sure the following tools are installed on your machine:

| Tool | Minimum Version | Check Command |
|------|----------------|---------------|
| Docker | 24.0+ | `docker --version` |
| Docker Compose | 2.0+ | `docker compose version` |
| .NET SDK | 8.0+ | `dotnet --version` |
| Node.js | 18.0+ | `node --version` |
| npm | 9.0+ | `npm --version` |

---

## 🚀 Installation

### Option 1 — Docker (Recommended)

This is the easiest way to get everything running with a single command.

**1. Clone the repository**

```bash
git clone https://github.com/your-username/url-shortener.git
cd url-shortener
```

**2. Configure environment**

Update the JWT secret key in `UrlShortener.API/appsettings.json`:

```json
{
  "App": {
    "BaseUrl": "http://localhost"
  },
  "Jwt": {
    "SecretKey": "your-super-secret-key-at-least-32-characters-long!",
    "Issuer": "UrlShortener",
    "Audience": "UrlShortener"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgres;Port=5432;Database=urlshortener;Username=postgres;Password=postgres"
  },
  "Redis": {
    "ConnectionString": "redis:6379"
  }
}
```

**3. Start all services**

```bash
docker compose up --build
```

The following services will start automatically:

| Container | Role | Exposed |
|-----------|------|---------|
| urlshortener-nginx | Reverse Proxy | Port 80 |
| urlshortener-api | .NET Backend | Internal |
| urlshortener-frontend | React App | Internal |
| urlshortener-postgres | Database | Port 5432 |
| urlshortener-redis | Cache | Internal |

**4. Access the application**

| Service | URL |
|---------|-----|
| 🌐 Web UI | http://localhost |
| 📡 API | http://localhost/api |
| 📖 Swagger UI | http://localhost/swagger |

> Database migrations run automatically on API startup.

---

### Option 2 — Manual Setup (Development)

Use this if you want to run services individually for development.

**1. Start infrastructure**

```bash
docker compose up postgres redis -d
```

**2. Run database migrations**

```bash
dotnet ef migrations add InitialCreate \
  --project UrlShortener.Infrastructure \
  --startup-project UrlShortener.API

dotnet ef database update \
  --project UrlShortener.Infrastructure \
  --startup-project UrlShortener.API
```

**3. Start the API**

```bash
dotnet run --project UrlShortener.API
```

API → http://localhost:5000
Swagger → http://localhost:5000/swagger

**4. Start the frontend**

```bash
cd frontend
npm install
npm run dev
```

Frontend → http://localhost:5173

---

## 💻 Usage

### Anonymous User

1. Go to http://localhost
2. Paste a long URL into the input field
3. Optionally set a custom alias
4. Click **Shorten**
5. Copy and share your short link

> Anonymous links are functional but not saved to any account. They cannot be managed after creation.

### Registered User

1. Click **Sign Up** in the top right corner
2. Create an account with email and password
3. Shorten URLs — they will be saved to your account
4. Go to **Dashboard** to:
   - View all your shortened links
   - See click count per link
   - Copy links to clipboard
   - Delete links

---

## 📡 API Reference

Full interactive documentation: **http://localhost/swagger**

### Authentication

All protected endpoints require a Bearer token:

```
Authorization: Bearer <your-jwt-token>
```

---

### Auth Endpoints

#### Register
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Response `200 OK`:**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "email": "user@example.com",
  "error": null
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Response `200 OK`:**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "email": "user@example.com",
  "error": null
}
```

---

### URL Endpoints

#### Create Short URL (Authenticated)
```http
POST /api/urls
Authorization: Bearer <token>
Content-Type: application/json

{
  "originalUrl": "https://www.example.com/very/long/url",
  "customAlias": "my-link",
  "expiresAt": null
}
```

**Response `201 Created`:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "shortCode": "my-link",
  "shortUrl": "http://localhost/my-link",
  "originalUrl": "https://www.example.com/very/long/url",
  "createdAt": "2024-03-01T10:00:00Z"
}
```

#### Create Short URL (Anonymous)
```http
POST /api/urls/anonymous
Content-Type: application/json

{
  "originalUrl": "https://www.example.com/very/long/url",
  "customAlias": null,
  "expiresAt": null
}
```

#### Get My URLs
```http
GET /api/urls
Authorization: Bearer <token>
```

**Response `200 OK`:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "shortCode": "aX3kP9",
    "shortUrl": "http://localhost/aX3kP9",
    "originalUrl": "https://www.example.com/very/long/url",
    "clickCount": 42,
    "createdAt": "2024-03-01T10:00:00Z",
    "expiresAt": null
  }
]
```

#### Delete URL
```http
DELETE /api/urls/{id}
Authorization: Bearer <token>
```

**Response `204 No Content`**

#### Redirect
```http
GET /{shortCode}
```

**Response `302 Found`** → Redirects to the original URL

---

### Password Policy

| Rule | Requirement |
|------|-------------|
| Minimum length | 6 characters |
| Digit required | ✅ Yes |
| Uppercase required | ❌ No |
| Special character required | ❌ No |

---



## 🐳 Docker Commands

```bash
# Start all services
docker compose up --build

# Start in background
docker compose up --build -d

# Stop all services
docker compose down

# Stop and remove volumes (wipes database)
docker compose down -v

# View logs
docker logs urlshortener-api -f
docker logs urlshortener-nginx -f
docker logs urlshortener-frontend -f

# Connect to PostgreSQL
docker exec -it urlshortener-postgres psql -U postgres -d urlshortener

# Connect to Redis CLI
docker exec -it urlshortener-redis redis-cli

# Clear build cache
docker builder prune -f

# Rebuild a single service
docker compose up --build api
```

---

## 🗄️ Database Commands

```bash
# Create a new migration
dotnet ef migrations add MigrationName \
  --project UrlShortener.Infrastructure \
  --startup-project UrlShortener.API

# Apply migrations
dotnet ef database update \
  --project UrlShortener.Infrastructure \
  --startup-project UrlShortener.API

# Revert last migration
dotnet ef migrations remove \
  --project UrlShortener.Infrastructure \
  --startup-project UrlShortener.API

# Revert to a specific migration
dotnet ef database update MigrationName \
  --project UrlShortener.Infrastructure \
  --startup-project UrlShortener.API

# List all migrations
dotnet ef migrations list \
  --project UrlShortener.Infrastructure \
  --startup-project UrlShortener.API
```

---

## 🔒 Security Notes

- JWT tokens expire after **7 days**
- Passwords are hashed using ASP.NET Core Identity (PBKDF2)
- Short codes are randomly generated using Base62 encoding (6 characters = 56 billion combinations)
- Add rate limiting before deploying to production
- Use environment variables or a secrets manager for sensitive config in production
- Consider adding HTTPS via Let's Encrypt in production

---

## 📄 License

MIT License — feel free to use this project however you like.