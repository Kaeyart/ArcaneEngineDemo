#!/usr/bin/env bash
set -euo pipefail

UNITY_BIN="${1:-}"
PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
RESULTS="$PROJECT_PATH/V21_TEST_RESULTS"

if [[ -z "$UNITY_BIN" || ! -x "$UNITY_BIN" ]]; then
  echo "Usage: Tools/run-tests-linux.sh /absolute/path/to/Unity"
  exit 2
fi

mkdir -p "$RESULTS"
"$UNITY_BIN" -batchmode -nographics -projectPath "$PROJECT_PATH" -runTests -testPlatform EditMode -testResults "$RESULTS/editmode.xml" -logFile "$RESULTS/editmode.log"
"$UNITY_BIN" -batchmode -nographics -projectPath "$PROJECT_PATH" -runTests -testPlatform PlayMode -testResults "$RESULTS/playmode.xml" -logFile "$RESULTS/playmode.log"

echo "Results written to $RESULTS"

