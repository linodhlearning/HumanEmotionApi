# HumanEmotionApi

A CRUD REST API for managing Human Emotions built with **.NET 10 LTS**, **SQLite**, **CQRS/MediatR**, and **Repository + Unit of Work** patterns.

## ✨ Features

- **5 Basic Emotions** — Joy, Sadness, Anger, Fear, Disgust — each with a percentage (0–100%)
- **ShowColor** property — interpolates from ⬜ white (`#FFFFFF`) to 🟩 deep green (`#008000`) as more emotions are added
- **CRUD** for both Humans and their Emotions
- **CQRS** with [MediatR](https://github.com/jbogard/MediatR)
- **Repository Pattern + Unit of Work**
- **SQLite** database via EF Core 10
- **99% test coverage** with xUnit
- **GitHub Actions CI** pipeline

## 🏗️ Architecture

```
HumanEmotionApi/
├── src/
│   ├── HumanEmotionApi.Domain/          # Entities, Enums, Exceptions
│   ├── HumanEmotionApi.Application/     # CQRS Commands/Queries, DTOs, Interfaces
│   ├── HumanEmotionApi.Infrastructure/  # EF Core, Repositories, Unit of Work
│   └── HumanEmotionApi.Api/             # Controllers, Middleware, Program.cs
└── tests/
    └── HumanEmotionApi.Tests/           # xUnit tests (Domain, Application, Infrastructure, API)
```

## 🎨 ShowColor Algorithm

The `showColor` property on a Human is a hex color that reflects their total emotional state:

| Emotion Score | Color |
|---|---|
| 0% (no emotions) | `#FFFFFF` (white) |
| 20% (1 emotion @ 100%) | `#CCE6CC` (light green) |
| 50% | `#80C080` (medium green) |
| 100% (all 5 @ 100%) | `#008000` (deep green) |

> Score = sum of all emotion percentages / 500 (5 emotions × 100% max)

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Run the API
```bash
dotnet run --project src/HumanEmotionApi.Api/HumanEmotionApi.Api.csproj
```

Open **Swagger UI** at: `http://localhost:5050/swagger`

### Run Tests
```bash
dotnet test HumanEmotionApi.slnx
```

## 📡 API Endpoints

### Humans
| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/humans` | Get all humans |
| `POST` | `/api/humans` | Create a human |
| `GET` | `/api/humans/{id}` | Get human by ID |
| `PUT` | `/api/humans/{id}` | Update human name |
| `DELETE` | `/api/humans/{id}` | Delete human |

### Emotions
| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/emotions/types` | List the 5 emotion types |
| `GET` | `/api/humans/{id}/emotions` | List human's emotions |
| `POST` | `/api/humans/{id}/emotions` | Add an emotion |
| `PUT` | `/api/humans/{id}/emotions/{type}` | Update emotion % |
| `DELETE` | `/api/humans/{id}/emotions/{type}` | Remove emotion |

### Example: Create a Human and add Joy at 80%
```bash
# Create human
curl -X POST http://localhost:5050/api/humans \
  -H "Content-Type: application/json" \
  -d '{"name": "Alice"}'

# Add Joy at 80% (use the id from above)
curl -X POST http://localhost:5050/api/humans/{id}/emotions \
  -H "Content-Type: application/json" \
  -d '{"emotionType": 1, "percentage": 80}'

# Get human — showColor will be #CCE6CC (light green)
curl http://localhost:5050/api/humans/{id}
```

**Emotion Types:** `1=Joy`, `2=Sadness`, `3=Anger`, `4=Fear`, `5=Disgust`

## 🧪 Testing

- **80 tests** across Domain, Application, Infrastructure, API layers
- **99.68% line coverage**, 100% branch coverage
- Code quality via `dotnet format` + Roslyn analyzers

## ⚙️ CI/CD

GitHub Actions pipeline (`.github/workflows/ci.yml`):
- Build in Release mode
- Run all tests with coverage
- `dotnet format` verification (code style check)
- Upload test results as artifacts

## 🛠️ Tech Stack

| Technology | Version |
|---|---|
| .NET | 10.0 LTS |
| EF Core + SQLite | 10.0.0 |
| MediatR | 12.4.1 |
| xUnit | 2.9.3 |
| Moq | 4.20.70 |
| Swashbuckle (Swagger) | 6.6.2 |
| coverlet | 6.0.4 |
