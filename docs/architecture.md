# Architecture

SuryaHUD is designed as a lightweight real-time Windows overlay.

## Target flow

Metric providers -> normalized snapshot -> overlay state -> renderer

## Engineering principles

- Keep the render/update loop lightweight.
- Isolate metric collection from presentation.
- Keep platform-specific integrations behind small interfaces.
- Make timing and sampling deterministic and testable.
- Gracefully degrade when a metric is unavailable.

Before major releases, record CPU overhead, memory usage, update frequency and frame-time impact.
