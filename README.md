# RecordDB.WebApp

<!-- Stack Badges -->
[![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C# 14](https://img.shields.io/badge/C%23-14-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC_%26_Web_API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet)
[![Dapper](https://img.shields.io/badge/ORM-Dapper-blueviolet?style=for-the-badge&logo=nuget&logoColor=white)](https://github.com/DapperLib/Dapper)
[![SQL Server](https://img.shields.io/badge/Database-Microsoft_SQL_Server-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Bootstrap 5](https://img.shields.io/badge/UI-Bootstrap_5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![OpenAPI / Swagger](https://img.shields.io/badge/Docs-Swagger_%7C_ReDoc_%7C_Scalar-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://swagger.io/)

**RecordDB** is a music catalogue management solution built with modern **.NET 10**, **C#**, and **ASP.NET Core**. It provides full-featured catalogue tracking for artists, vinyl records, CDs, boxed sets, discs, and individual track listings, paired with financial cost summaries, rating insights, and responsive interfaces optimized for PC, tablet, and mobile browsers.

---

## Table of Contents

- [Solution Projects](#solution-projects)
- [Technology Stack](#technology-stack)
- [Key Features](#key-features)
- [Solution Architecture](#solution-architecture)
- [Presentation Layer](#presentation-layer)
- [Data Access Layer](#data-access-layer)
- [Data Model & Relationships](#data-model--relationships)
- [Getting Started](#getting-started)
- [Building the Solution](#building-the-solution)
- [API Documentation](#api-documentation)

---

## Solution Projects

The solution is divided into three clean, decoupled projects:

```
RecordDB.WebApp/
│
├── RecordDB.API/             # RESTful Web API service & Dapper Data Access Layer
├── RecordDB.MVC/             # Web Application Frontend (ASP.NET Core MVC & Razor)
└── RecordDB.Shared/          # Shared DTOs and contracts used across API & MVC
```

| Project | Type | Description |
|---|---|---|
| **`RecordDB.API`** | ASP.NET Core Web API | Exposes high-performance RESTful JSON endpoints. Manages database transactions, executes SQL Server stored procedures via Dapper, and serves API documentation through Swagger, ReDoc, and Scalar. |
| **`RecordDB.MVC`** | ASP.NET Core MVC | Modern web client application. Consumes `RecordDB.API` through typed `HttpClient` services, renders responsive Razor views with custom CSS variables, and handles multi-device navigation. |
| **`RecordDB.Shared`** | .NET Class Library | Common shared library containing Data Transfer Objects (DTOs) for requests, responses, entity projections, and pagination contracts. |

---

## Technology Stack

- **Platform & Runtime:** .NET 10.0 (C# 14)
- **Web Frontend:** ASP.NET Core MVC (Razor Views, ViewModels, Tag Helpers)
- **Styling & Assets:** Bootstrap 5, Bootstrap Icons, Google Fonts (Inter), custom mobile-first CSS design system
- **Backend API:** ASP.NET Core Web API, RESTful endpoints, Typed `HttpClient` with dependency injection
- **Data Access & ORM:** [Dapper](https://github.com/DapperLib/Dapper) 2.1, [Dapper.Contrib](https://github.com/DapperLib/Dapper.Contrib) 2.0, `Microsoft.Data.SqlClient` 7.1
- **Database:** Microsoft SQL Server with Stored Procedures and Table-Valued Parameters (TVPs)
- **API Documentation & Exploration:**
  - Swagger / OpenAPI (`Swashbuckle.AspNetCore` 10.2)
  - ReDoc (`Swashbuckle.AspNetCore.ReDoc` 10.2)
  - Scalar (`Scalar.AspNetCore` 2.17)

---

## Key Features

### 1. Artist Management & Search
- Full CRUD workflows (Create, Read, Edit, Delete) for artist profiles.
- **Dedicated Artist Search:** Search by first name, last name, or combined query.
- Displays artist biographies, photo initials, and directly browses all albums by the artist.

### 2. Record (Album) Catalogue & Showcase
- Complete album metadata: Format/Media (CD, Vinyl, Blu-ray), Release/Recorded Year, Record Label, Pressing Country, Ratings, Review notes, Disc counts, and Purchase Costs.
- **Filtered Record Search:** Real-time and submitted partial search for album names (`up_RecordSelectAll`).
- **Interactive RecordView Showcase:**
  - Responsive showcase page displaying detailed fields in balanced paired layouts.
  - Three integrated toggle buttons side-by-side: **Artist Biography**, **Record Review**, and **Track Listing**.
  - Dynamic results area at the bottom of the Record Information section (no disruptive dialog boxes or popups).
  - Automatically groups tracks by Disc number with total track count and playtime calculations.

### 3. Track Library & Fast Search
- Full management for tracks tied to specific discs and records.
- **Dedicated Track Search:** Search songs by partial or full title using stored procedure `up_SelectPartialRecordTracks`.
- Track index page displays active search banners with instant clear, direct navigation, and duration formatting (`mm:ss`).

### 4. Financial & Collection Analytics
- **Artist Totals (`sp_getTotalsForEachArtist`):** Calculates disc counts, overall expenditure, and average cost per disc per artist with 20-item block pagination and summary header cards.
- **Collection Statistics:** High-level summary of total albums, total discs, costs, and reviews.

### 5. Multi-Device Responsive UI
- **PC & Laptop:** Dense, information-rich tables with quick action toolbars, avatars, and format badges.
- **Tablet & Mobile:** Touch-friendly card layouts with accessible touch targets, clear typography, and adaptive pagination.
- **Block-Based Pagination:** 15-button sliding window with `« Prev 15` / `Next 15 »` block jumps and compact mobile controls.

---

## Solution Architecture

The solution implements a Clean Architecture pattern, decoupling the presentation layer from the database layer via a REST API:

```
┌─────────────────────────────────────────────────────────────┐
│                 Client Browsers (PC, Tablet, Phone)         │
└──────────────────────────────┬──────────────────────────────┘
                               │ HTTPS
┌──────────────────────────────▼──────────────────────────────┐
│                       RecordDB.MVC                          │
│  - Controllers: Artist, Record, Track, Total, Statistics    │
│  - ViewModels: PaginatedViewModel, Search & Showcase VMs    │
│  - Responsive Views (Desktop Tables + Mobile Touch Cards)   │
│  - Typed HttpClients: IArtistService, IRecordService, etc.  │
└──────────────────────────────┬──────────────────────────────┘
                               │ HTTPS / JSON REST API
┌──────────────────────────────▼──────────────────────────────┐
│                       RecordDB.API                          │
│  - Controllers: Attribute routing, Content Negotiation      │
│  - OpenAPI / Swagger / ReDoc / Scalar UI Documentation      │
│  - Repositories: IArtistRepository, IRecordRepository, etc. │
│  - DataAccess: IDataAccess (Dapper + Microsoft.Data.SqlClient)│
└──────────────────────────────┬──────────────────────────────┘
                               │ T-SQL / Stored Procedures
┌──────────────────────────────▼──────────────────────────────┐
│                    Microsoft SQL Server                     │
│  - Stored Procedures: up_RecordSelectByIdCore, etc.         │
│  - Table-Valued Parameters (TVPs) for bulk operations       │
│  - Tables: Artists, Records, Discs, Tracks                  │
└─────────────────────────────────────────────────────────────┘
```

---

## Presentation Layer

The frontend application (`RecordDB.MVC`) is designed with a modern design system:

- **Typography & Aesthetics:** Inter font family, dark slate navbar (`#0f172a`), neon vinyl accent gradients, and subtle card shadows.
- **Typed API Clients:** Registered in `Program.cs` as typed `HttpClient` instances:
  - `IArtistService` &rarr; `ArtistService`
  - `IRecordService` &rarr; `RecordService`
  - `IDiscService` &rarr; `DiscService`
  - `ITrackService` &rarr; `TrackService`
  - `ITotalService` &rarr; `TotalService`
  - `IStatisticService` &rarr; `StatisticService`
- **Responsive Dual-Rendering Pattern:** Views employ Bootstrap breakpoint utilities (`d-none d-lg-block` vs `d-lg-none`) to deliver full desktop data tables on large screens and touch cards on mobile devices without duplicating controller logic.
- **Interactive Inline Panels:** The showcase page (`RecordView.cshtml`) toggles details dynamically below the information grid using lightweight DOM scripting without modal popups.

---

## Data Access Layer

Data operations are centralized in `RecordDB.API/Data/DataAccess.cs` using **Dapper**:

- **IDataAccess Interface:** Exposes generic helper methods for stored procedure execution:
  - `GetData<T, U>(string storedProcedure, U parameters)`
  - `GetFirstOrDefault<T, U>(string storedProcedure, U parameters)`
  - `SaveData<T>(string storedProcedure, T parameters)`
  - `SaveDataReturnId(string storedProcedure, DynamicParameters parameters)`
  - `GetScalar<T, U>(string storedProcedure, U parameters)`
- **Stored Procedures:** All business logic and queries execute via dedicated SQL Server stored procedures:
  - `up_RecordSelectAll`: Complete record catalogue retrieval.
  - `up_RecordSelectByIdCore`: Single album details with artist join.
  - `up_GetArtistRecordTracks`: Full tracklist for a given album.
  - `sp_getTotalsForEachArtist`: Aggregates disc counts and cost per artist.
  - `up_SelectPartialRecordTracks`: Fast partial track title searching.
  - `adm_RecordInsert` & `adm_UpdateRecord`: Transactional record modifications.

---

## Data Model & Relationships

```
┌─────────────────┐       1..*       ┌─────────────────┐
│     Artist      ├──────────────────┤     Record      │
│─────────────────│                  │─────────────────│
│ ArtistId (PK)   │                  │ RecordId (PK)   │
│ FirstName       │                  │ ArtistId (FK)   │
│ LastName        │                  │ Name            │
│ Name            │                  │ Field (Genre)   │
│ Biography       │                  │ Recorded (Year) │
└─────────────────┘                  │ Label           │
                                     │ Pressing        │
                                     │ Rating          │
                                     │ Discs           │
                                     │ Media           │
                                     │ Bought          │
                                     │ Cost            │
                                     │ Review          │
                                     └────────┬────────┘
                                              │ 1..*
                                     ┌────────▼────────┐
                                     │      Disc       │
                                     │─────────────────│
                                     │ DiscId (PK)     │
                                     │ RecordId (FK)   │
                                     │ DiscNo          │
                                     │ Length          │
                                     └────────┬────────┘
                                              │ 1..*
                                     ┌────────▼────────┐
                                     │      Track      │
                                     │─────────────────│
                                     │ TrackId (PK)    │
                                     │ DiscId (FK)     │
                                     │ TrackNo         │
                                     │ Name            │
                                     │ TrackLength     │
                                     │ Extended        │
                                     └─────────────────┘
```

- **Artist &rarr; Record (1 to Many):** An artist produces multiple albums/records.
- **Record &rarr; Disc (1 to Many):** A record has one or multiple physical/digital discs (e.g., 2-LP vinyl or multi-disc CD sets).
- **Disc &rarr; Track (1 to Many):** Each disc contains an ordered sequence of tracks with duration and title.

---

## Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [Microsoft SQL Server](https://www.microsoft.com/sql-server) (LocalDB, SQL Express, or Standard/Enterprise)
- [Visual Studio 2026 / 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# Dev Kit

### 1. Database Configuration

Ensure your database connection string in [`RecordDB.API/appsettings.json`](file:///d:/Projects/RecordDB.WebApp/RecordDB.API/appsettings.json) points to your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "RecordDb": "Server=YOUR_SERVER;Initial Catalog=RecordDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"
  }
}
```

### 2. MVC API Endpoint Configuration

Ensure the API base address in [`RecordDB.MVC/appsettings.json`](file:///d:/Projects/RecordDB.WebApp/RecordDB.MVC/appsettings.json) matches the API launch URL:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7096/"
  }
}
```

---

## Building the Solution

You can build the entire solution using the .NET CLI or Visual Studio.

### Using .NET CLI

1. **Restore dependencies:**
   ```powershell
   dotnet restore
   ```

2. **Build the solution:**
   ```powershell
   dotnet build -v minimal
   ```

3. **Run the API (Terminal 1):**
   ```powershell
   dotnet run --project RecordDB.API
   ```
   *The API will start listening at `https://localhost:7096` and `http://localhost:5183`.*

4. **Run the MVC Web App (Terminal 2):**
   ```powershell
   dotnet run --project RecordDB.MVC
   ```
   *The MVC application will start listening at `https://localhost:7101` and `http://localhost:5013`.*

### Using Visual Studio

1. Open `RecordDB.WebApp.slnx`.
2. Right-click the Solution and select **Set Startup Projects...**.
3. Choose **Multiple startup projects**:
   - `RecordDB.API` &rarr; **Start**
   - `RecordDB.MVC` &rarr; **Start**
4. Press <kbd>F5</kbd> or <kbd>Ctrl+F5</kbd> to build and run both projects.

---

## API Documentation

When running `RecordDB.API` in Development mode, multiple interactive API documentation interfaces are available:

- **Scalar API Reference (Default):** `https://localhost:7096/` or `https://localhost:7096/scalar/v1`
- **Swagger UI:** `https://localhost:7096/swagger`
- **ReDoc:** `https://localhost:7096/redoc`
- **OpenAPI v1 JSON Document:** `https://localhost:7096/openapi/v1.json`

---

## License

This project is licensed under the MIT License — feel free to adapt and expand for your personal music archive or organizational use.
