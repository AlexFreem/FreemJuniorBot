namespace FreemJuniorBot.Models;

/// <summary>
/// Database settings bound from configuration/environment variables.
/// </summary>
public sealed class DatabaseSettings
{
    // Direct connection string candidates (bind via Database:DbConnectionString, Database:DatabaseUrl, Database:Connection)
    public string? DbConnectionString { get; set; }
    public string? DatabaseUrl { get; set; }
    public string? Connection { get; set; }

    // Postgres piecewise variables (bind via Database:PostgresHost, etc.)
    public string? PostgresHost { get; set; }
    public string? PostgresPort { get; set; }
    public string? PostgresDb { get; set; }
    public string? PostgresUser { get; set; }
    public string? PostgresPassword { get; set; }

    /// <summary>
    /// Primary connection string resolved from available configuration values.
    /// </summary>
    public string? ConnectionString =>
        DbConnectionString
        ?? DatabaseUrl
        ?? Connection
        ?? BuildPostgresFromParts();

    private string? BuildPostgresFromParts()
    {
        var host = PostgresHost;
        var db = PostgresDb;
        var user = PostgresUser;
        var password = PostgresPassword;
        var port = string.IsNullOrWhiteSpace(PostgresPort) ? "5432" : PostgresPort;

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(db) || string.IsNullOrWhiteSpace(user))
            return null;

        return $"Host={host};Port={port};Database={db};Username={user};Password={password}";
    }
}