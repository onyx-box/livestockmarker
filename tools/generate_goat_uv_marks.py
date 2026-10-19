
#!/usr/bin/env python3

import argparse
from pathlib import Path

import json5
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[1]
GAME = Path.home() / ".games/vintagestory"

SHAPE = GAME / (
    "assets/survival/shapes/entity/animal/"
    "mammal/hooved/goat/valais/adult.json"
)

TEXTURE = GAME / (
    "assets/survival/textures/entity/animal/"
    "mammal/hooved/goat/valais-adult.png"
)

OUTPUT = ROOT / (
    "livestockmarker/assets/livestockmarker/"
    "textures/entity/goat/marks"
)

# Kandydaci na zewnętrzną sierść grzbietu.
# Nie zakładamy, że każdy ma powierzchnię "up".
TARGETS = {
    "bodyHAIR1",
    "midHair1",
    "rearHAIR1",
}

COLORS = {
    "red": (215, 35, 35),
    "blue": (40, 90, 230),
    "green": (35, 185, 70),
    "yellow": (240, 210, 30),
    "white": (250, 250, 250),
    "black": (25, 25, 25),
}

DEBUG_COLORS = [
    (255, 0, 255),
    (0, 255, 255),
    (255, 170, 0),
]


def walk(elements):
    for element in elements:
        yield element
        yield from walk(element.get("children") or [])


def load_data():
    with SHAPE.open(encoding="utf-8-sig") as f:
        shape = json5.load(f)

    image = Image.open(TEXTURE).convert("RGBA")

    sx = image.width / shape["textureWidth"]
    sy = image.height / shape["textureHeight"]

    return shape, image, sx, sy


def find_regions(shape, sx, sy):
    regions = []

    for element in walk(shape.get("elements", [])):
        name = element.get("name", "")

        if name not in TARGETS:
            continue

        face = (element.get("faces") or {}).get("up")

        if not face or face.get("texture") != "#hide":
            print(f"SKIP {name}: missing up/#hide")
            continue

        uv = face.get("uv")

        if not uv or len(uv) != 4:
            print(f"SKIP {name}: missing UV")
            continue

        x1, y1, x2, y2 = uv

        rect = (
            round(min(x1, x2) * sx),
            round(min(y1, y2) * sy),
            round(max(x1, x2) * sx),
            round(max(y1, y2) * sy),
        )

        regions.append((name, rect))
        print(f"{name}: UV={uv}, pixels={rect}")

    return regions



def paint(image, rect, rgb, debug=False):
    from PIL import ImageChops

    x1, y1, x2, y2 = rect

    x1 = max(0, min(image.width, x1))
    x2 = max(0, min(image.width, x2))
    y1 = max(0, min(image.height, y1))
    y2 = max(0, min(image.height, y2))

    w = x2 - x1
    h = y2 - y1

    if w < 3 or h < 3:
        return

    overlay = Image.new("RGBA", image.size)
    draw = ImageDraw.Draw(overlay)

    if debug:
        draw.rectangle(
            (x1, y1, x2 - 1, y2 - 1),
            fill=(*rgb, 255)
        )
    else:
        # Niewielka, nieregularna plama.
        # Zostawiamy margines od granic UV.
        points = [
            (0.20, 0.30),
            (0.35, 0.12),
            (0.65, 0.18),
            (0.85, 0.38),
            (0.77, 0.68),
            (0.55, 0.85),
            (0.28, 0.72),
            (0.12, 0.50),
        ]

        pixels = [
            (
                round(x1 + px * (w - 1)),
                round(y1 + py * (h - 1))
            )
            for px, py in points
        ]

        draw.polygon(
            pixels,
            fill=(*rgb, 215)
        )

    # Zachowujemy oryginalną przezroczystość.
    source_alpha = image.getchannel("A")
    overlay_alpha = overlay.getchannel("A")

    overlay.putalpha(
        ImageChops.multiply(
            overlay_alpha,
            source_alpha
        )
    )

    image.alpha_composite(overlay)



def main():
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "mode",
        choices=["inspect", "debug", "generate"]
    )
    args = parser.parse_args()

    shape, original, sx, sy = load_data()
    regions = find_regions(shape, sx, sy)

    if not regions:
        raise SystemExit(
            "No matching UV regions. Check TARGETS."
        )

    if args.mode == "inspect":
        return

    OUTPUT.mkdir(parents=True, exist_ok=True)

    if args.mode == "debug":
        image = original.copy()

        for index, (name, rect) in enumerate(regions):
            rgb = DEBUG_COLORS[index % len(DEBUG_COLORS)]
            paint(image, rect, rgb, debug=True)
            print(f"DEBUG {name}: {rgb}")

        path = OUTPUT / "valais-adult-debug.png"
        image.save(path)
        print(f"Saved: {path}")
        return

    for color_name, rgb in COLORS.items():
        image = original.copy()

        for _, rect in regions:
            paint(image, rect, rgb)

        path = OUTPUT / f"valais-adult-{color_name}.png"
        image.save(path)
        print(f"Saved: {path}")


if __name__ == "__main__":
    main()
