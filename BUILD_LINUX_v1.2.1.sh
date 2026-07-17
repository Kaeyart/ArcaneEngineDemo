#!/usr/bin/env bash
set -euo pipefail

PROJECT_PATH="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
UNITY_VERSION="6000.5.2f1"
UNITY_BIN="${UNITY_BIN:-$HOME/Unity/Hub/Editor/$UNITY_VERSION/Editor/Unity}"
OUTPUT_PATH="${1:-$PROJECT_PATH/Builds/Linux}"

if [[ ! -x "$UNITY_BIN" ]]; then
  printf 'Unity executable not found: %s\nSet UNITY_BIN to the Unity 6000.5.2f1 executable and run again.\n' "$UNITY_BIN" >&2
  exit 1
fi

mkdir -p "$OUTPUT_PATH"
"$UNITY_BIN" -batchmode -nographics -quit \
  -projectPath "$PROJECT_PATH" \
  -executeMethod ArcaneEngine.Editor.ArcaneEngineBuild.BuildLinux \
  -arcaneBuildPath "$OUTPUT_PATH" \
  -logFile "$OUTPUT_PATH/build.log"

chmod +x "$OUTPUT_PATH/ArcaneEngine.x86_64" "$OUTPUT_PATH/RUN_ARCANE_ENGINE.sh"
printf 'Arcane Engine v1.2.1 Linux build complete: %s\n' "$OUTPUT_PATH"
