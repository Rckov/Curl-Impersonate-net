using CurlImpClient.Enums;

using System;
using System.Collections.Generic;

namespace CurlImpClient.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a libcurl operation fails.
/// </summary>
public class CurlException : Exception
{
   /// <summary>
   /// Gets the CURL error code that caused the exception.
   /// </summary>
   public CURLcode ErrorCode { get; }

   /// <summary>
   /// Gets the human-readable description of the CURL error code.
   /// </summary>
   public string Description => GetDescription(ErrorCode);

   /// <summary>
   /// Initializes a new instance of the <see cref="CurlException"/> class with the specified CURL error code.
   /// The exception message is automatically generated based on the error code.
   /// </summary>
   /// <param name="code">The CURL error code that caused the exception.</param>
   public CurlException(CURLcode code) : base($"libcurl error ({code}): {GetDescription(code)}")
   {
      ErrorCode = code;
   }

   /// <summary>
   /// Initializes a new instance of the <see cref="CurlException"/> class with the specified CURL error code and a custom message.
   /// </summary>
   /// <param name="code">The CURL error code that caused the exception.</param>
   /// <param name="message">A custom message describing the exception.</param>
   public CurlException(CURLcode code, string message) : base(message)
   {
      ErrorCode = code;
   }

   /// <summary>
   /// Gets a human-readable description of the specified CURL error code.
   /// </summary>
   /// <param name="code">The CURL error code to get the description for.</param>
   /// <returns>A string describing the error, or "Unknown libcurl error." if the code is not recognized.</returns>
   public static string GetDescription(CURLcode code)
   {
      return _descriptions.TryGetValue(code, out var desc) ? desc : "Unknown libcurl error.";
   }

   /// <summary>
   /// A dictionary mapping CURL error codes to their human-readable descriptions.
   /// </summary>
   private static readonly Dictionary<CURLcode, string> _descriptions = new()
   {
      [CURLcode.CURLE_OK] = "No error, operation successful.",
      [CURLcode.CURLE_UNSUPPORTED_PROTOCOL] = "The URL you passed to libcurl used a protocol that this libcurl does not support.",
      [CURLcode.CURLE_FAILED_INIT] = "Very early initialization code failed.",
      [CURLcode.CURLE_URL_MALFORMAT] = "The URL was not properly formatted.",
      [CURLcode.CURLE_NOT_BUILT_IN] = "A requested feature, protocol or option was not built-in.",
      [CURLcode.CURLE_COULDNT_RESOLVE_PROXY] = "Couldn't resolve proxy name.",
      [CURLcode.CURLE_COULDNT_RESOLVE_HOST] = "Couldn't resolve host name.",
      [CURLcode.CURLE_COULDNT_CONNECT] = "Failed to connect to host or proxy.",
      [CURLcode.CURLE_WEIRD_SERVER_REPLY] = "Received a strange response from the server.",
      [CURLcode.CURLE_REMOTE_ACCESS_DENIED] = "Access denied to remote resource.",
      [CURLcode.CURLE_QUOTE_ERROR] = "Quote command returned error.",
      [CURLcode.CURLE_HTTP_RETURNED_ERROR] = "HTTP server returned error.",
      [CURLcode.CURLE_WRITE_ERROR] = "Error occurred when writing received data.",
      [CURLcode.CURLE_UPLOAD_FAILED] = "Failed to start upload.",
      [CURLcode.CURLE_READ_ERROR] = "Error while reading local file.",
      [CURLcode.CURLE_OUT_OF_MEMORY] = "Out of memory.",
      [CURLcode.CURLE_OPERATION_TIMEDOUT] = "Operation timeout.",
      [CURLcode.CURLE_RANGE_ERROR] = "Requested range was not delivered.",
      [CURLcode.CURLE_HTTP_POST_ERROR] = "Internal post-request generation error.",
      [CURLcode.CURLE_SSL_CONNECT_ERROR] = "SSL connection failed.",
      [CURLcode.CURLE_BAD_DOWNLOAD_RESUME] = "Couldn't resume download.",
      [CURLcode.CURLE_FILE_COULDNT_READ_FILE] = "Couldn't read local file.",
      [CURLcode.CURLE_LDAP_CANNOT_BIND] = "LDAP cannot bind.",
      [CURLcode.CURLE_LDAP_SEARCH_FAILED] = "LDAP search failed.",
      [CURLcode.CURLE_FUNCTION_NOT_FOUND] = "Function not found.",
      [CURLcode.CURLE_ABORTED_BY_CALLBACK] = "Operation aborted by callback.",
      [CURLcode.CURLE_BAD_FUNCTION_ARGUMENT] = "Function argument is incorrect.",
      [CURLcode.CURLE_INTERFACE_FAILED] = "Failed to bind interface.",
      [CURLcode.CURLE_TOO_MANY_REDIRECTS] = "Too many redirects.",
      [CURLcode.CURLE_UNKNOWN_OPTION] = "An unknown option was passed.",
      [CURLcode.CURLE_PEER_FAILED_VERIFICATION] = "SSL peer's certificate or fingerprint was not verified.",
      [CURLcode.CURLE_GOT_NOTHING] = "No data received from server.",
      [CURLcode.CURLE_SSL_ENGINE_NOTFOUND] = "SSL engine not found.",
      [CURLcode.CURLE_SSL_ENGINE_SETFAILED] = "SSL engine could not be set.",
      [CURLcode.CURLE_SEND_ERROR] = "Failed to send network data.",
      [CURLcode.CURLE_RECV_ERROR] = "Failed to receive network data.",
      [CURLcode.CURLE_SSL_CERTPROBLEM] = "Problem with the local SSL certificate.",
      [CURLcode.CURLE_SSL_CIPHER] = "Couldn't use specified cipher.",
      [CURLcode.CURLE_SSL_CACERT] = "SSL CA certificate not OK.",
      [CURLcode.CURLE_BAD_CONTENT_ENCODING] = "Unrecognized transfer encoding.",
      [CURLcode.CURLE_FILESIZE_EXCEEDED] = "Maximum file size exceeded.",
      [CURLcode.CURLE_USE_SSL_FAILED] = "Requested SSL failed.",
      [CURLcode.CURLE_SEND_FAIL_REWIND] = "Send failed during rewind.",
      [CURLcode.CURLE_SSL_ENGINE_INITFAILED] = "SSL engine initialization failed.",
      [CURLcode.CURLE_LOGIN_DENIED] = "Login denied.",
      [CURLcode.CURLE_REMOTE_FILE_NOT_FOUND] = "Remote file not found.",
      [CURLcode.CURLE_SSH] = "SSH session error.",
      [CURLcode.CURLE_SSL_SHUTDOWN_FAILED] = "SSL shutdown failed.",
      [CURLcode.CURLE_AGAIN] = "Call again later.",
      [CURLcode.CURLE_SSL_CACERT_BADFILE] = "Problem reading the SSL CA cert.",
      [CURLcode.CURLE_SSL_ISSUER_ERROR] = "Issuer check failed.",
      [CURLcode.CURLE_CHUNK_FAILED] = "Chunk callback reported error.",
      [CURLcode.CURLE_NO_CONNECTION_AVAILABLE] = "No connection available.",
      [CURLcode.CURLE_HTTP2] = "Error in the HTTP/2 framing layer.",
      [CURLcode.CURLE_HTTP2_STREAM] = "Stream error in HTTP/2 layer.",
      [CURLcode.CURLE_HTTP3] = "Error in the HTTP/3 layer.",
      [CURLcode.CURLE_SSL_PINNEDPUBKEYNOTMATCH] = "SSL public key does not match pinned key.",
      [CURLcode.CURLE_SSL_INVALIDCERTSTATUS] = "Invalid certificate status.",
      [CURLcode.CURLE_AUTH_ERROR] = "Authentication error.",
      [CURLcode.CURLE_PROXY] = "Proxy error.",
      [CURLcode.CURLE_SSL_CLIENTCERT] = "Problem with SSL client certificate.",
      [CURLcode.CURLE_UNRECOVERABLE_POLL] = "Unrecoverable poll/select error."
   };
}