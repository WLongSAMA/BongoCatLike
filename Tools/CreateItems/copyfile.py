import shutil
import json
import os

images_path = "..\\..\\Decompile\\20251008\\ExportedProject\\Assets\\Texture2D\\"
destination_path = "..\\..\\Assets\\"

skin_path = destination_path + "skin\\"
hat_path = destination_path + "hat\\"
emote_path = destination_path + "emote\\"
if not os.path.exists(skin_path):
    os.makedirs(skin_path)
if not os.path.exists(hat_path):
    os.makedirs(hat_path)
if not os.path.exists(emote_path):
    os.makedirs(emote_path)

with open(destination_path + "items.json", "r") as file:
    json_data = json.load(file)

for item in json_data["skin"]:
    shutil.copy(images_path + json_data["skin"][item].get("icon"), skin_path + json_data["skin"][item].get("icon"))
    for file in json_data["skin"][item].get("image"):
        shutil.copy(images_path + file, skin_path + file)

for item in json_data["hat"]:
    shutil.copy(images_path + json_data["hat"][item].get("icon"), hat_path + json_data["hat"][item].get("icon"))
    shutil.copy(images_path + json_data["hat"][item].get("image"), hat_path + json_data["hat"][item].get("image"))

for item in json_data["emote"]:
    shutil.copy(images_path + json_data["emote"][item].get("icon"), emote_path + json_data["emote"][item].get("icon"))
    shutil.copy(images_path + json_data["emote"][item].get("image"), emote_path + json_data["emote"][item].get("image"))

