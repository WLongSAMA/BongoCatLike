import shutil
import json

images_path = "..\\ExportedProject\\Assets\\Texture2D\\"
destination_path = "..\\Assets\\"

with open("items.json", "r") as file:
    json_data = json.load(file)

for item in json_data["skin"]:
    shutil.move(images_path + item.get("icon"), destination_path + item.get("icon"))
    for file in item.get("image"):
        shutil.move(images_path + file, destination_path + file)

for item in json_data["hat"]:
    shutil.move(images_path + item.get("icon"), destination_path + item.get("icon"))
    shutil.move(images_path + item.get("image"), destination_path + item.get("image"))

shutil.copy("items.json", destination_path + "items.json")
