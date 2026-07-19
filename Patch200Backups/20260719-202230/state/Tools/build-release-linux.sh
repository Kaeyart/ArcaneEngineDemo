#!/usr/bin/env bash
set -euo pipefail

UNITY_BIN="${1:-}"
PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"

if [[ -z "$UNITY_BIN" || ! -x "$UNITY_BIN" ]]; then
  echo "Usage: Tools/build-release-linux.sh /absolute/path/to/Unity"
  exit 2
fi

"$PROJECT_PATH/Tools/run-tests-linux.sh" "$UNITY_BIN"
"$UNITY_BIN" -batchmode -quit -nographics -projectPath "$PROJECT_PATH" -executeMethod ArcaneEngine.Editor.ArcaneEngineBuild.BuildLinux -logFile "$PROJECT_PATH/V21_TEST_RESULTS/build-linux.log"

echo "Build command completed. Inspect V21_TEST_RESULTS/build-linux.log and the configured build output."

