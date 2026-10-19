from pathlib import Path
from PIL import Image, ImageDraw

PROJECT_ROOT = Path(__file__).resolve().parents[1]

SOURCE = (
    Path.home()
    / ".games/vintagestory/assets/survival/textures/entity/animal/mammal/hooved/goat/valais-adult.png"
)

OUTPUT_DIR = (
    PROJECT_ROOT
    / "livestockmarker/assets/livestockmarker/textures/entity/goat/marks"
)

OUTPUT_DIR.mkdir(parents=True, exist_ok=True)

COLORS = {
    "red":    (210, 40, 40, 235),
    "blue":   (45, 95, 220, 235),
    "green":  (50, 165, 75, 235),
    "yellow": (235, 205, 40, 235),
    "white":  (245, 245, 245, 235),
    "black":  (25, 25, 25, 235),
}

# Kilka kandydatów na widoczne miejsca na sierści.
# To nie jest jeszcze finalne "idealne UV", tylko praktyczna wersja 1.
# Plama jest rysowana w kilku miejscach, żeby mieć wysoką szansę widoczności na modelu.
SPOTS = [
    [(18, 56), (25, 53), (34, 56), (36, 63), (31, 69), (22, 68), (17, 62)],
    [(30, 78), (38, 74), (47, 78), (49, 86), (44, 93), (35, 92), (29, 85)],
    [(14, 96), (22, 92), (30, 96), (32, 104), (27, 111), (18, 111), (12, 104)],
]

def darker(color, amount=50):
    r, g, b, a = color
    return (max(r - amount, 0), max(g - amount, 0), max(b - amount, 0), a)

def lighter(color, amount=30):
    r, g, b, a = color
    return (min(r + amount, 255), min(g + amount, 255), min(b + amount, 255), a)

def draw_mark(draw, points, fill_color):
    outline = darker(fill_color, 55)
    highlight = lighter(fill_color, 25)

    draw.polygon(points, fill=fill_color, outline=outline)

    # małe dodatkowe "kleksy", żeby nie wyglądało zbyt geometrycznie
    min_x = min(p[0] for p in points)
    min_y = min(p[1] for p in points)
    max_x = max(p[0] for p in points)
    max_y = max(p[1] for p in points)

    draw.ellipse(
        (min_x + 2, min_y + 2, min_x + 6, min_y + 6),
        fill=highlight
    )
    draw.ellipse(
        (max_x - 6, max_y - 5, max_x - 2, max_y - 1),
        fill=outline
    )

def generate_variant(color_name, rgba):
    image = Image.open(SOURCE).convert("RGBA")
    draw = ImageDraw.Draw(image)

    for spot in SPOTS:
        draw_mark(draw, spot, rgba)

    output = OUTPUT_DIR / f"valais-adult-{color_name}.png"
    image.save(output)
    print(f"Generated: {output}")

def main():
    for color_name, rgba in COLORS.items():
        generate_variant(color_name, rgba)

if __name__ == "__main__":
    main()