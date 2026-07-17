#!/usr/bin/env bash
set -euo pipefail

PROJECT_PATH="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
UNITY_BIN="${UNITY_BIN:-/opt/unity/editors/6000.5.2f1/Editor/Unity}"
OUTPUT_PATH="${1:-$PROJECT_PATH/Builds/Linux}"

if [[ ! -x "$UNITY_BIN" ]]; then
  printf '%s\n' "Unity executable not found at: $UNITY_BIN" "Set UNITY_BIN to the full Unity 6000.5.2f1 Editor executable path and run again."
  exit 1
fi

"$UNITY_BIN" -batchmode -nographics -quit -projectPath "$PROJECT_PATH" -executeMethod ArcaneEngine.Editor.ArcaneEngineBuild.BuildLinux -arcaneBuildPath "$OUTPUT_PATH" -logFile "$OUTPUT_PATH/build.log"
printf 'Arcane Engine v1.4.0 Linux build complete: %s\n' "$OUTPUT_PATH"
