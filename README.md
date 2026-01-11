# AI-VerticalSlice1
using AI to create a complete both the software development plan and C# code implementations (using the Vertical Slice design pattern)  to develop a solution that imports data from a simple csv file, updates database entites using the csv values, and produces a report of the updating.

Milestone 1 (Baseline)

Targets:
- Visual Studio 2017
- .NET Framework 4.6.1
- SQL Server Express instance: .\SQLEXPRESS
- EF6 (Code First) + SimpleInjector

## Quick start
1. Open `CsvUpdateDemo.sln` in Visual Studio 2017
2. Restore NuGet packages (right-click solution -> Restore NuGet Packages)
3. Ensure SQL Server Express is installed and instance name is `.\SQLEXPRESS`
4. Run the app

On first run, EF will create:
- Database: `CsvUpdateDemo`
- Table: `Employees`

The console prints readiness diagnostics and a confirmation message.
