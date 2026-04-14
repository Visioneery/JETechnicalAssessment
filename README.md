# JETechnicalAssessment

A Blazor application for searching movies with OMDb, showing movie details, and storing recent search history in SQLite.

## Tech stack

- .NET 10
- Blazor Server
- Entity Framework Core 10
- SQLite
- OMDb API
- Nunit, Moq

## Features

- Search movies through OMDb
- View movie details
- Show recent searches from local SQLite storage
- Keep the last 5 unique search queries
- Persist search history between application restarts

## Configuration

## OMDb API

Get your API key here:

http://www.omdbapi.com/apikey.aspx

### `appsettings.Development.json`

OMDb API URL and API key:

```json
{
  "Omdb": {
    "BaseUrl": "https://www.omdbapi.com/",
    "ApiKey": "your-api-key"
  }
}
```

FYI you can also store key in your user secrets

## Database

This project uses SQLite with EF Core migrations.

Database will be auto-generated on the first run if it does not exist.

### Create or update the database

```powershell
dotnet ef database update
```

### Add a new migration

Migrations are stored in `JETechnicalAssessment\\Data\\Migrations`:

```powershell
dotnet ef migrations add MigrationName -o Data\\Migrations
```

### Update EF tools if needed

```powershell
dotnet tool update --global dotnet-ef --version 10.0.5
```

## Viewing the database

You can inspect the SQLite database using:

- DB Browser for SQLite: https://sqlitebrowser.org/dl/
- SQLite CLI

Example:

```powershell
sqlite3 .\\app.db
```
## Local setup

- Clone repo
- Open sln in Visual Studio
- Update user secrets or `appsettings.Development.json` with your OMDb API key
- Run application


## Useful notes

- Mentioned in the requirements "1. Movie search by title." 
  - Implemented by search because it does not require the exact name and adds more flexibility in what user types
- Details of each movie are returned using search by id