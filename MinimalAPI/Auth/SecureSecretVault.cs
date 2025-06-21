namespace MinimalAPI.Auth;

internal static class SecureSecretVault
{
	private const string jwtDebugKey = "VeryMuchSuperLongerJwtDebugKeyWhichIsAtleast32Characters";
	private const string connectionString =
		"Server=localhost;" +
#if DEBUG
		"Database=DebugDb;" +
#else
		"Database=WebbLabb2;" +
#endif
		"Trusted_Connection=True;" +
		"TrustServerCertificate=True";
	private const string connectionTestString =
		"Server=localhost;" +
		"Trusted_Connection=True;" +
		"TrustServerCertificate=True;" +
		"Timeout=5";
	private const string debugUserEmail = "debug@here";
	private const string debugUserPassword = "DebugPassword123!";

	internal static string GetJwtSecret() => jwtDebugKey;
	internal static string GetConnectionString() => connectionString;
	internal static string ConnectionTestString => connectionTestString;
	internal static string DebugUserEmail => debugUserEmail;
	internal static string DebugUserPassword => debugUserPassword;
}
