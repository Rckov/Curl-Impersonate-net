# CurlImpersonate.NET

[![.NET Standard Version](https://img.shields.io/badge/.NET%20Standard-2.0-blue.svg)]()
[![Build and Release](https://github.com/Rckov/Remote-Desktop/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/Rckov/CurlImpersonate.NET/actions/workflows/build.yml)
[![GitHub Release](https://img.shields.io/github/v/release/Rckov/CurlImpersonate.NET)](https://github.com/Rckov/CurlImpersonate.NET/releases/latest)

A lightweight .NET wrapper for `curl-impersonate`.

It allows sending HTTP requests that mimic browser TLS fingerprints (JA3, JA4, JA4+), by using a native `libcurl.dll` built with impersonation support.

## Installation

1. You can download the precompiled files from the [Depler repo](https://github.com/depler/curl-impersonate-win) or extract them from the `curl-impersonate` folder.
2. Make sure the following files are located next to your `.exe` file:
   - `curl.exe`
   - `libcurl.dll`
   - `curl-ca-bundle.crt`

## Example Usage

```csharp
const string Proxy = "http://192.160.0.1/handler";
const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36";

using var session = new CurlSession(new CurlSessionConfig
{
   TimeoutSeconds = 10,
   EnableHttp2Options = false
});

// These settings are applied to the entire session (all requests)
session.SetProxy(Proxy);
session.SetUserAgent(UserAgent);

// These headers are added only to this specific request
session.AddHeader("Accept: application/json");
session.AddHeader("Accept-Language: en-US,en;q=0.9");

var response = session.ExecuteRequest(HttpMethod.Get, "https://tls.browserleaks.com/json");
```

## Features

- Send HTTP(S) requests that look like they come from real browsers (e.g., Chrome, Firefox).
- Reuse sessions and cookies.
- Easily customize headers, POST data, and proxy settings.

Tested with https://tls.browserleaks.com/json, which shows accurate browser fingerprinting like:
```json
{
  "user_agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 YaBrowser/25.6.0.0 Safari/537.36",
  "ja3_hash": "0a093df2c2a3407b73f9eea5eb244390",
  "ja3_text": "771,4865-4866-4867-49195-49199-49196-49200-52393-52392,...",
  "ja3n_hash": "8e19337e7524d2573be54efb2b0784c9",
  "ja3n_text": "771,4865-4866-4867-49195-49199-49196-49200-52393-52392,...",
  "ja4": "...",
  "akamai_text": "1:65536;2:0;4:6291456;6:262144|15663105|0|m,a,s,p"
}
```
We successfully mimicked a browser.

## License
Licensed under [MIT](LICENSE).

