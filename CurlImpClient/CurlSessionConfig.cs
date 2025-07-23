namespace CurlImpClient;

/// <summary>
/// Configuration settings for the <see cref="CurlSession"/> class.
/// </summary>
public class CurlSessionConfig
{
   /// <summary>
   /// The timeout in seconds. Default is 30.
   /// </summary>
   public int TimeoutSeconds { get; set; } = 30;

   /// <summary>
   /// The browser to impersonate. Default is "chrome116".
   /// </summary>
   public string Browser { get; set; } = "chrome116";

   /// <summary>
   /// The cookie file path. Default is ":memory:".
   /// </summary>
   public string CookieFile { get; set; } = ":memory:";

   /// <summary>
   /// Whether to enable HTTP/2 options. Default is true.
   /// </summary>
   public bool EnableHttp2Options { get; set; } = true;

   /// <summary>
   /// The path to the CA file. Default is "curl-ca-bundle.crt".
   /// </summary>
   public string CaFilePath { get; set; } = "curl-ca-bundle.crt";
}