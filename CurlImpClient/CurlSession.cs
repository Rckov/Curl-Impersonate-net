using CurlImpClient.Enums;
using CurlImpClient.Exceptions;
using CurlImpClient.Natives;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CurlImpClient;

/// <summary>
/// Manages HTTP requests using libcurl.
/// </summary>
public class CurlSession : IDisposable
{
   private bool _disposed;

   private IntPtr _handle;
   private IntPtr _headers = IntPtr.Zero;

   private readonly WriteCallback _writeCallback;
   private readonly StringBuilder _builder = new();
   private readonly CurlSessionConfig _sessionConfig = new();

   private delegate UIntPtr WriteCallback(IntPtr ptr, UIntPtr size, UIntPtr nmemb, IntPtr userdata);

   /// <summary>
   /// Initializes a new instance of the <see cref="CurlSession"/> class with the specified configuration.
   /// </summary>
   /// <param name="config">The configuration to use. If null, the default configuration is used.</param>
   public CurlSession(CurlSessionConfig config = null)
   {
      _handle = NativeMethods.curl_easy_init(); 
      _sessionConfig = config ?? new CurlSessionConfig();

      if (_handle == IntPtr.Zero)
      {
         throw new CurlException(CURLcode.CURLE_FAILED_INIT, "Failed to initialize CURL.");
      }

      _writeCallback = WriteMemoryCallback;
      InitializeCurlOptions();
   }

   private void InitializeCurlOptions()
   {
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_impersonate(_handle, _sessionConfig.Browser, 1));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_FOLLOWLOCATION, 1));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_COOKIEFILE, _sessionConfig.CookieFile));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_COOKIEJAR, _sessionConfig.CookieFile));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_TIMEOUT, _sessionConfig.TimeoutSeconds));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_CAINFO, _sessionConfig.CaFilePath));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_SSL_VERIFYPEER, 1));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_SSL_VERIFYHOST, 1));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_WRITEFUNCTION, _writeCallback));
   }

   /// <summary>
   /// Adds a header to the request.
   /// </summary>
   /// <param name="header">The header to add.</param>
   public void AddHeader(string header)
   {
      if (string.IsNullOrWhiteSpace(header))
      {
         throw new ArgumentException("Header cannot be null or empty.", nameof(header));
      }

      _headers = NativeMethods.curl_slist_append(_headers, header);
   }

   /// <summary>
   /// Adds multiple headers to the request.
   /// </summary>
   /// <param name="headers">A dictionary of headers to add.</param>
   public void AddHeaders(IDictionary<string, string> headers)
   {
      if (headers == null)
      {
         throw new ArgumentNullException(nameof(headers));
      }

      foreach (var kv in headers)
      {
         if (string.IsNullOrWhiteSpace(kv.Key) || string.IsNullOrWhiteSpace(kv.Value))
         {
            continue;
         }

         AddHeader($"{kv.Key}: {kv.Value}");
      }
   }

   /// <summary>
   /// Sets the proxy for the request.
   /// </summary>
   /// <param name="proxy">The proxy to use. (http://{host:port} | http://{login:password@host:port})</param>
   public void SetProxy(string proxy)
   {
      if (string.IsNullOrWhiteSpace(proxy))
      {
         throw new ArgumentException("Proxy cannot be null or empty.", nameof(proxy));
      }

      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_PROXY, proxy));
   }

   /// <summary>
   /// Sets the user agent for the request.
   /// </summary>
   /// <param name="userAgent">The user agent to use.</param>
   public void SetUserAgent(string userAgent)
   {
      if (string.IsNullOrWhiteSpace(userAgent))
      {
         throw new ArgumentException("UserAgent cannot be null or empty.", nameof(userAgent));
      }

      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_USERAGENT, userAgent));
   }

   /// <summary>
   /// Sets the POST data for the request.
   /// </summary>
   /// <param name="data">The POST data to send.</param>
   public void SetPostData(string data)
   {
      if (data == null)
      {
         throw new ArgumentNullException(nameof(data));
      }

      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_POSTFIELDS, data));
   }

   /// <summary>
   /// Sets the CA file path for SSL/TLS verification.
   /// </summary>
   /// <param name="path">The path to the CA file.</param>
   public void SetCaFile(string path)
   {
      if (string.IsNullOrWhiteSpace(path))
      {
         throw new ArgumentException("CA file path cannot be null or empty.", nameof(path));
      }

      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_CAINFO, path));
   }

   /// <summary>
   /// Executes an HTTP request and returns the response.
   /// </summary>
   /// <param name="method">The HTTP method to use.</param>
   /// <param name="url">The URL to request.</param>
   /// <returns>The response from the server.</returns>
   public string ExecuteRequest(HttpMethod method, string url)
   {
      if (string.IsNullOrWhiteSpace(url))
      {
         throw new ArgumentException("URL cannot be null or empty.", nameof(url));
      }

      _builder.Clear();

      if (_sessionConfig.EnableHttp2Options)
      {
         ConfigureHttp2Options();
      }

      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_CUSTOMREQUEST, method.ToString().ToUpperInvariant()));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_URL, url));

      if (_headers != IntPtr.Zero)
      {
         NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_HTTPHEADER, _headers));
      }

      NativeMethods.ThrowIfError(NativeMethods.curl_easy_perform(_handle));

      if (_headers != IntPtr.Zero)
      {
         NativeMethods.curl_slist_free_all(_headers);
         _headers = IntPtr.Zero;
      }

      return _builder.ToString();
   }

   private void ConfigureHttp2Options()
   {
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_HTTP2_PSEUDO_HEADERS_ORDER, "masp"));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_HTTP2_SETTINGS, "1:65536;3:1000;4:6291456;6:262144"));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_HTTP2_WINDOW_UPDATE, 15663105));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(_handle, CURLoption.CURLOPT_SSL_PERMUTE_EXTENSIONS, 1));
   }

   private UIntPtr WriteMemoryCallback(IntPtr ptr, UIntPtr size, UIntPtr nmemb, IntPtr userdata)
   {
      var realSize = (int)(size.ToUInt64() * nmemb.ToUInt64());
      var buffer = new byte[realSize];

      Marshal.Copy(ptr, buffer, 0, realSize);

      _builder.Append(Encoding.UTF8.GetString(buffer));

      return (UIntPtr)(ulong)realSize;
   }

   /// <summary>
   /// Releases the resources used by the <see cref="CurlSession"/> instance.
   /// </summary>
   public void Dispose()
   {
      if (_disposed)
      {
         return;
      }

      if (_headers != IntPtr.Zero)
      {
         NativeMethods.curl_slist_free_all(_headers);
      }

      if (_handle != IntPtr.Zero)
      {
         NativeMethods.curl_easy_cleanup(_handle);
      }

      _handle = IntPtr.Zero;
      _headers = IntPtr.Zero;

      _disposed = true;
      GC.SuppressFinalize(this);
   }

   ~CurlSession()
   {
      Dispose();
   }
}