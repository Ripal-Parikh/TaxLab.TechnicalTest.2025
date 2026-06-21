# TaxLab.TechnicalTest.2025

This test is designed to evaluate your technical ability and problem solving skills.

Please fork this repository and complete the User Story outlined below.

## User Story

As a user creating a New Zealand personal tax return in TaxLab, I want to be able to enter a salary and get the amount of tax to pay.

Please consult this spreadsheet for the marginal tax rate calculation and test data:
https://docs.google.com/spreadsheets/d/1PFvSHqPPEtzjhdjynkonfSCqnIv1PkWytw2dQgX8XMQ/edit?usp=sharing

Please note

- If the salary is a loss (ie less than zero) then there is no tax to pay

User Inputs:
Annual Salary

Outputs:
Tax to pay

---

## Implementation

### Technology

- **C# / .NET 8**
- **ASP.NET Core Web API** with Swagger
- **Entity Framework Core 8** with SQLite
- **xUnit** for unit testing

### Project Structure

| Project | Type | Purpose |
|---|---|---|
| `TaxLab` | Web API | Exposes the `POST /tax-calculations` endpoint |
| `TaxLab.Data` | Class Library | EF Core DbContext, entities, and migrations |
| `TaxLab.Worker` | Console App | Imports tax rate data from CSV into the database |
| `TaxLab.Tests` | xUnit | Unit tests for the tax calculation logic |

---

## Getting Started

### Step 1 — Run the Worker

The worker reads tax band data from the CSV file and populates the SQLite database. Run it once before starting the API.

```bash
dotnet run --project TaxLab.Worker
```

This will:
- Create `taxlab.db` at the solution root
- Apply the database schema (EF Core)
- Import the NZ tax bands from `TaxLab.TechnicalTest.2025 - Calculation.csv`

The worker is **idempotent** — running it again drops and recreates the database with fresh data.

### Step 2 — Run the API

```bash
dotnet run --project TaxLab
```
Or press **F5** in Visual Studio with `TaxLab` set as the startup project.

### Step 3 — Test via Swagger

Navigate to:
```
https://localhost:7109/swagger
```
---
## Running Tests

```bash
dotnet test TaxLab.Tests
```
Or in Visual Studio: **Test → Run All Tests**

Note: 80% code is written by AI

