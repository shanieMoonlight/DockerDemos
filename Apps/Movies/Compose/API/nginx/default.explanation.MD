# nginx proxy headers — plain English

This document explains common `proxy_*` directives used when nginx proxies requests to your API and why they matter.

## Directives

- `proxy_set_header Host $host;`
  - Sends the original `Host` header (what the client requested, e.g. `localhost` or `example.com`) to the upstream.
  - Why: the backend can use the Host for virtual-host routing, generating absolute URLs, or auth checks.

- `proxy_set_header X-Real-IP $remote_addr;`
  - Adds a header with the immediate client's TCP IP address.
  - Why: useful for logging or basic client-IP capture (note: if there is another proxy in front of nginx that IP may be the proxy).

- `proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;`
  - Appends the client IP to the `X-Forwarded-For` chain (creates or extends the list). Example value: `203.0.113.5, 10.0.0.1`.
  - Why: preserves the full client→proxy chain so the backend can determine the original client IP when multiple proxies are involved.

- `proxy_set_header X-Forwarded-Proto $scheme;`
  - Tells the backend whether the original request used `http` or `https`.
  - Why: important for generating correct redirects, absolute links, and for cookie security flags.

- `proxy_http_version 1.1;`
  - Use HTTP/1.1 for upstream connections (required for keep-alive and WebSocket proxying).
  - Why: required when the backend needs HTTP/1.1 features such as `Upgrade`/`Connection` handling.

- `proxy_buffering off;`
  - Disable nginx response buffering for proxied requests — nginx will stream upstream responses directly to the client.
  - Why: good for streaming or low-latency responses; trade-off is higher memory/connection usage on nginx.

## Practical notes

- Keep both `X-Real-IP` and `X-Forwarded-For` for compatibility; inspect the forwarded chain in your app to find the original client IP.
- If nginx terminates TLS, ensure `X-Forwarded-Proto` is present so your app can generate `https` links and set Secure cookies.
- For WebSockets include `Upgrade`/`Connection` headers and use `proxy_http_version 1.1`.
- `proxy_buffering off` is handy for debugging/streaming, but consider enabling buffering in high-throughput production to reduce memory pressure.

## Quick WebSocket headers (optional)

When proxying WebSockets also include these lines:

```
proxy_set_header Upgrade $http_upgrade;
proxy_set_header Connection $connection_upgrade; # define $connection_upgrade via a map if needed
proxy_http_version 1.1;
```

## Getting the real client IP in ASP.NET Core

Enable forwarded headers and (optionally) configure known proxies/networks:

```csharp
// in Program.cs
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Optionally set KnownProxies or KnownNetworks in production
});

app.UseForwardedHeaders();

// then access the client IP via HttpContext.Connection.RemoteIpAddress
```

---

If you want, I can also add the WebSocket headers and a small `map` to the nginx config to set `$connection_upgrade` automatically.