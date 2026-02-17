# OpenTelemetry OTLP Export

OTLP export is enabled automatically when `OTEL_EXPORTER_OTLP_ENDPOINT` is set — no code change needed.

```bash
# Enable OTLP export (e.g. to Aspire dashboard or collector)
OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
```

- Do not hardcode the OTLP endpoint in code or `appsettings.json`
- In local development, the Aspire dashboard sets this automatically
- In production, set via environment/container config