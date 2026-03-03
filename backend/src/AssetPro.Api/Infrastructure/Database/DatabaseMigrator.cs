using DbUp;
using DbUp.Engine;

namespace AssetPro.Api.Infrastructure.Database;

public static class DatabaseMigrator
{
    public static DatabaseUpgradeResult Migrate(string connectionString)
    {
        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DatabaseMigrator).Assembly,
                s => s.Contains("Infrastructure.Database.Migrations"))
            .WithTransactionPerScript()
            .LogToConsole()
            .Build();

        return upgrader.PerformUpgrade();
    }
}
