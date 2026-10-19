
from pathlib import Path
from PIL import Image, ImageDraw

game = Path.home() / ".games/vintagestory"

source = (
    game
    / "assets/survival/textures/entity/animal/mammal"
    / "hooved/goat/valais-adult.png"
)

output = (
    Path(__file__).resolve().parent
    / "valais-adult-marked.png"
)

image = Image.open(source).convert("RGBA")
draw = ImageDraw.Draw(image)

# UV górnej powierzchni elementu Body
x1, y1, x2, y2 = 13, 47, 22, 56

# Nieregularna czerwona plama
points = [
    (15, 49),
    (18, 48),
    (21, 50),
    (20, 53),
    (18, 55),
    (15, 54),
    (14, 51)
]

draw.polygon(points, fill=(210, 35, 35, 255))

image.save(output)

print(f"Generated: {output}")
print(f"Image size: {image.size}")
