namespace MinimalAPI.Auth;

internal class SecureSecretVault
{
	private const string JwtDebugKey = "VeryMuchSuperLongerJwtDebugKeyWhichIsAtleast32Characters";
	private const string ConnectionString =
		"Server=localhost;" +
		"Database=WebbLabb2;" +
		"Trusted_Connection=True;" +
		"TrustServerCertificate=True";

	internal static string GetJwtSecret() => JwtDebugKey;

	internal static string GetConnectionString() => ConnectionString;
}
