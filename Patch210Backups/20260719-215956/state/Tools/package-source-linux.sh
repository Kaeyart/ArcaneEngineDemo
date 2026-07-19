#!/usr/bin/env bash
set -euo pipefail

PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
PARENT="$(dirname "$PROJECT_PATH")"
OUT="${1:-$PARENT/ArcaneEngineDemo_v2.1.0_source.zip}"

cd "$PARENT"
zip -qr "$OUT" "$(basename "$PROJECT_PATH")" \
  -x '*/Library/*' '*/Temp/*' '*/Logs/*' '*/UserSettings/*' '*/Build/*' '*/Builds/*' '*/V21_TEST_RESULTS/*' '*/.git/*'
sha256sum "$OUT" > "$OUT.sha256"
echo "Created $OUT"

