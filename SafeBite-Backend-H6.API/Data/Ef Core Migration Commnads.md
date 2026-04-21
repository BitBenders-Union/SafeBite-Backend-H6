dotnet tool install --global dotnet-ef


cd .\SafeBite-Backend-H6.API
dotnet ef migrations add {MigrationName} --context AuthDbContext --output-dir Data/Migrations/Auth
dotnet ef database update --context AuthDbContext


cd .\SafeBite-Backend-H6.API
dotnet ef migrations add {MigrationName} --context AppDbContext --output-dir Data/Migrations/App
dotnet ef database update --context AppDbContext



husk at være i den rigtige folder ellers kan den ikke finde projektet.!