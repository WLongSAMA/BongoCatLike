import shutil
import json

images_path = "..\\..\\Decompile\\20250811\\ExportedProject\\Assets\\Texture2D\\"
destination_path = "..\\..\\Assets\\"

with open(destination_path + "items.json", "r") as file:
    json_data = json.load(file)

for item in json_data["skin"]:
    shutil.copy(images_path + json_data["skin"][item].get("icon"), destination_path + "skin\\" + json_data["skin"][item].get("icon"))
    for file in json_data["skin"][item].get("image"):
        shutil.copy(images_path + file, destination_path + "skin\\" + file)

for item in json_data["hat"]:
    shutil.copy(images_path + json_data["hat"][item].get("icon"), destination_path + "hat\\" + json_data["hat"][item].get("icon"))
    shutil.copy(images_path + json_data["hat"][item].get("image"), destination_path + "hat\\" + json_data["hat"][item].get("image"))
