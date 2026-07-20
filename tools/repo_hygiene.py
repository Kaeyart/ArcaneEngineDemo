#!/usr/bin/env python3
from __future__ import annotations

import argparse
import fnmatch
import json
import subprocess
import sys
from pathlib import Path

EXPECTED_UNITY = "6000.5.2f1"
MAX_TRACKED_BYTES = 95 * 1024 * 1024

REQUIRED = (
    "Assets",
    "Packages/manifest.json",
    "ProjectSettings/ProjectVersion.txt",
    "README.md",
    "CHANGELOG.md",
    "CONTRIBUTING.md",
    "SECURITY.md",
    ".gitignore",
    ".gitattributes",
)

GENERATED_ROOTS = {
    "Library",
    "Temp",
    "Obj",
    "Build",
    "Builds",
    "Logs",
    "UserSettings",
    "MemoryCaptures",
    "Recordings",
    "artifacts",
    "builds",
    ".repo-cleanup-backups",
}

def run_git(project: Path, *args: str) -> list[str]:
    result = subprocess.run(
        ["git", "-C", str(project), *args],
        check=True,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
    )
    return [line for line in result.stdout.splitlines() if line]

def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--project", default=".")
    parser.add_argument(
        "--report-only",
        action="store_true",
        help="Print errors but return success. Used immediately after installing repository tooling.",
    )
    args = parser.parse_args()

    project = Path(args.project).resolve()
    errors: list[str] = []
    warnings: list[str] = []

    if not (project / ".git").exists():
        errors.append(f"Not a Git working copy: {project}")

    for relative in REQUIRED:
        if not (project / relative).exists():
            errors.append(f"Missing required path: {relative}")

    version_file = project / "ProjectSettings/ProjectVersion.txt"
    if version_file.is_file():
        version_text = version_file.read_text(encoding="utf-8", errors="replace")
        expected_line = f"m_EditorVersion: {EXPECTED_UNITY}"
        if expected_line not in version_text:
            errors.append(
                f"Unity version mismatch: expected {EXPECTED_UNITY} in ProjectVersion.txt"
            )

    manifest = project / "Packages/manifest.json"
    if manifest.is_file():
        try:
            json.loads(manifest.read_text(encoding="utf-8"))
        except Exception as exc:
            errors.append(f"Packages/manifest.json is invalid JSON: {exc}")

    tracked: list[str] = []
    if (project / ".git").exists():
        try:
            tracked = run_git(project, "ls-files")
        except Exception as exc:
            errors.append(f"Unable to inspect tracked files: {exc}")

    for relative in tracked:
        first = relative.split("/", 1)[0]
        if first in GENERATED_ROOTS:
            errors.append(f"Generated path is tracked: {relative}")
        if first.endswith("Backups"):
            errors.append(f"Installer backup is tracked: {relative}")
        if fnmatch.fnmatch(relative, ".arcane-*-last-backup"):
            errors.append(f"Local patch-state pointer is tracked: {relative}")

        path = project / relative
        try:
            if path.is_file() and path.stat().st_size > MAX_TRACKED_BYTES:
                errors.append(
                    f"Tracked file exceeds 95 MiB: {relative} ({path.stat().st_size} bytes)"
                )
        except OSError as exc:
            warnings.append(f"Could not stat tracked file {relative}: {exc}")

    attributes = project / ".gitattributes"
    if attributes.is_file():
        attributes_text = attributes.read_text(encoding="utf-8", errors="replace")
        for extension in ("*.png", "*.wav", "*.fbx", "*.psd"):
            if extension not in attributes_text or "filter=lfs" not in attributes_text:
                errors.append(f"Git LFS rule is missing or incomplete for {extension}")

    marker_300 = project / "Assets/ArcaneEngine/PATCH_3_0_0_ALPHA_1.txt"
    if marker_300.is_file():
        for relative in (
            "docs/PATCH_NOTES_3.0.0-alpha.1.md",
            "docs/IMPLEMENTATION_MANIFEST_3.0.0-alpha.1.md",
            "docs/VALIDATION_CHECKLIST_3.0.0-alpha.1.md",
        ):
            if not (project / relative).is_file():
                errors.append(f"3.0 marker exists but release document is missing: {relative}")

    if not (project / "LICENSE").exists() and not (project / "LICENSE.md").exists():
        warnings.append(
            "No license file is present. Public visibility does not grant reuse rights."
        )

    print(f"ArcaneEngineDemo repository audit: {project}")
    print(f"Tracked files inspected: {len(tracked)}")
    for warning in warnings:
        print(f"WARN  {warning}")
    for error in errors:
        print(f"ERROR {error}")

    if errors:
        print(f"Result: FAIL ({len(errors)} error(s), {len(warnings)} warning(s))")
        return 0 if args.report_only else 1

    print(f"Result: PASS ({len(warnings)} warning(s))")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
