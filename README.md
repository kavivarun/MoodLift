# MoodLift

A modern mental wellness tracking application that combines mood tracking, data analytics, AI-powered personalized recommendations, and music therapy to help users monitor and improve their mental well-being.

## Project Overview

MoodLift is a comprehensive mental health companion built with **Blazor Server** that enables users to:
- Track detailed mood and lifestyle patterns through a progressive 9-step form
- Visualize trends and insights with interactive charts and analytics
- Receive AI-generated personalized wellness tips using **Azure OpenAI **
- Discover mood-appropriate music through **Spotify Web API** integration
- Maintain secure user sessions with **Google OAuth 2.0** authentication

## Tech Stack

### Frontend
- **Blazor Server Components** with Interactive Server rendering mode
- **ApexCharts for Blazor** - Interactive data visualizations
- **Bootstrap** with custom CSS for responsive design

### Backend
- **ASP.NET Core 9.0** - Web application framework
- **Entity Framework Core 9.0** - ORM with SQLite database
- **Clean Architecture** - Multi-project solution structure

### Authentication & Security
- **Google OAuth 2.0** - Secure user authentication
- **Custom Cookie Authentication Scheme** - Extended 12-hour sessions
- **User Data Isolation** - All data scoped by Google User ID

### AI & External Integrations
- **Azure OpenAI (GPT-4)** - Personalized wellness tips and music recommendations
- **Spotify Web API** - Music search and embedded playback
- **Microsoft.Extensions.AI** - Structured AI responses with JSON schema

### Testing & Development
- **NUnit** - Unit testing framework
- **Moq** - Mocking framework for unit tests
- **SQLite** - Development database (configurable for SQL Server)

## Getting Started

### Prerequisites
- **.NET 9 SDK** or later
- **Google Cloud Console** account (for OAuth setup)
- **Azure OpenAI** service (for AI features)
- **Spotify Developer** account (for music integration)

### Required Configuration

1. **Configure App Settings**
   
   Create/Fill `appsettings.json` in the MoodLift project with:
   ```json
   {
     "ConnectionStrings": {
       "MoodLiftDb": "Data Source=moodlift.db"
     },
     "Google": {
       "ClientId": "your-google-client-id",
       "ClientSecret": "your-google-client-secret"
     },
     "AzureOpenAI": {
       "Endpoint": "https://your-resource.openai.azure.com/",
       "ApiKey": "your-azure-openai-key",
       "Deployment": "gpt-4"
     },
     "Spotify": {
       "ClientId": "your-spotify-client-id",
       "ClientSecret": "your-spotify-client-secret"
     }
   }
   ```

2. **Set up External Services**
   - **Google OAuth**: Create credentials in [Google Cloud Console](https://console.cloud.google.com/)
   - **Azure OpenAI**: Deploy GPT-4 model in [Azure Portal](https://portal.azure.com/)
   - **Spotify API**: Register app at [Spotify Developer Dashboard](https://developer.spotify.com/)

3. **Run Database Migrations**
   ```bash
   dotnet ef database update --project MoodLift.Infrastructure --startup-project MoodLift
   ```

4. **Start the Application**
   ```bash
   dotnet run --project MoodLift
   ```

## Key Programming Concepts & Examples

### Polymorphism (Method Overloading)

**Location**: `MoodLift.Core/Interfaces/IMoodEntryService.cs` (Lines 12-14)

**Purpose**: Provides flexible ways to retrieve recent mood entries - either by count or by time range.

### Interface Examples

**1. Generic Repository Pattern**: `MoodLift.Core/Interfaces/IRepository.cs`

**2. Current User Service**: `MoodLift.Core/Interfaces/ICurrentUserService.cs`

### NUnit Test Example

**Location**: `MoodLift.Tests/SpotifyServiceTest.cs`


### Anonymous Method with LINQ Lambda Expression

**Location**: `MoodLift/Components/Pages/Analytics.razor` (Lines 235-237)


**Purpose**: Transforms enum values into chart data using lambda expressions for filtering and projection.

### 🗂️ Generics/Generic Collections

**Location**: `MoodLift.Infrastructure/Repositories/Repository.cs`


### Blazor Server UI

**Interactive Components**: All pages use Blazor Server with `@rendermode InteractiveServer`
- **Progressive Forms**: Multi-step mood entry with client-side validation
- **Real-time Charts**: Interactive ApexCharts with live data updates
- **Component-based Architecture**: Reusable components like `LoadingOverlay`

### Entity Framework with LINQ

**Location**: `MoodLift.Infrastructure/Repositories/MoodLiftDbContext.cs`


### External APIs & Machine Learning

**1. Azure OpenAI Integration** (`MoodLift/Components/Pages/Tips.razor`):

**2. Spotify Web API** (`MoodLift.Infrastructure/Services/SpotifyService.cs`):


## Running Tests

```bash
dotnet test MoodLift.Tests
```

---

**Built using .NET 9, Blazor Server, and modern web technologies By Kavi and Saloni**