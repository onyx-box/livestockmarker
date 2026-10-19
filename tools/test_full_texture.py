
from pathlib import Path
from PIL import Image

source = (
    Path.home()
    / ".games/vintagestory/assets/survival/textures/"
    / "entity/animal/mammal/hooved/goat/valais-adult.png"
)

target = (
    Path(__file__).resolve().parents[1]
    / "livestockmarker/assets/livestockmarker/"
    / "textures/entity/goat/valais-marked.png"
)

image = Image.open(source).convert("RGBA")

pixels = image.load()

for y in range(image.height):
    for x in range(image.width):
        r, g, b, a = pixels[x, y]

        if a > 0:
            pixels[x, y] = (255, 0, 220, a)

target.parent.mkdir(parents=True, exist_ok=True)
image.save(target)

print(f"Saved: {target}")
