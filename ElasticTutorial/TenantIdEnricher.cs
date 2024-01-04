using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

public class TenantIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantIdEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (_httpContextAccessor.HttpContext?.Request?.Headers.TryGetValue("TenantId", out var tenantId) == true)
        {
            logEvent.AddPropertyIfAbsent(new LogEventProperty("TenantId", new ScalarValue(tenantId)));
        }
    }
}