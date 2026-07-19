#!/usr/bin/env bash
set -euo pipefail
ROOT="${1:-$(pwd)}"
cd "$ROOT"

printf 'Arcane Engine static audit\n'
printf 'Project: %s\n\n' "$PWD"

fail=0
for path in \
  Assets/ArcaneEngine/Scripts/Core/GameWorld.cs \
  Assets/ArcaneEngine/Scripts/Core/PlayerController.cs \
  Assets/ArcaneEngine/Scripts/UI/V21ProductUI.cs \
  Assets/ArcaneEngine/Scripts/Spells/HexBoard.cs \
  ProjectSettings/ProjectVersion.txt; do
  if [[ ! -f "$path" ]]; then
    printf 'MISSING: %s\n' "$path"
    fail=1
  fi
done

if grep -RIl --include='*.asset' 'm_Script: {fileID: 0}' Assets/ArcaneEngine/Resources/V21Content 2>/dev/null | grep -q .; then
  printf '\nBroken generated assets:\n'
  grep -RIl --include='*.asset' 'm_Script: {fileID: 0}' Assets/ArcaneEngine/Resources/V21Content || true
  fail=1
fi

printf '\nLargest C# files:\n'
find Assets/ArcaneEngine -type f -name '*.cs' -print0 \
  | xargs -0 wc -l \
  | sort -nr \
  | head -n 16

printf '\nConflict markers:\n'
if grep -RInE '^(<<<<<<<|=======|>>>>>>>)' Assets/ArcaneEngine --include='*.cs'; then
  fail=1
else
  printf 'none\n'
fi

exit "$fail"
