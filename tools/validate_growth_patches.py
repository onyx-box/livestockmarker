
#!/usr/bin/env python3

import json
from pathlib import Path

import json5

ROOT = Path(__file__).resolve().parents[1]
GAME = Path.home() / ".games/vintagestory"

PATCH = ROOT / (
    "livestockmarker/assets/livestockmarker/"
    "patches/animal-growth.json"
)

def main():
    patches = json.loads(PATCH.read_text())

    for patch in patches:
        asset = patch["file"].split(":", 1)[1]

        source = (
            GAME / "assets/survival" / asset
        )

        if not source.exists():
            raise RuntimeError(
                f"Missing game asset: {source}"
            )

        data = json5.loads(source.read_text())

        index = int(
            patch["path"].split("/")[3]
        )

        behaviors = data["server"]["behaviors"]

        actual = behaviors[index]["code"]

        if actual != "grow":
            raise RuntimeError(
                f"INVALID PATCH: {asset}, "
                f"index={index}, actual={actual}"
            )

        if patch.get("value") != "livestockmarkergrow":
            raise RuntimeError(
                f"Unexpected replacement: {asset}"
            )

        print(
            f"OK: {asset}, grow at index {index}"
        )

    print("All growth patches validated.")


if __name__ == "__main__":
    main()
