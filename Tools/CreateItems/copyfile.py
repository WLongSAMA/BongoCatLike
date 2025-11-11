import shutil
import json
import os

with open("config.json", "r") as config:
    config_data = json.load(config)

skin_path = config_data.get("destination_path") + "skin\\"
hat_path = config_data.get("destination_path") + "hat\\"
emote_path = config_data.get("destination_path") + "emote\\"
if not os.path.exists(skin_path):
    os.makedirs(skin_path)
if not os.path.exists(hat_path):
    os.makedirs(hat_path)
if not os.path.exists(emote_path):
    os.makedirs(emote_path)

with open(config_data.get("destination_path") + "items.json", "r") as file:
    json_data = json.load(file)

for item in json_data["skin"]:
    shutil.copy(config_data.get("images_path") + json_data["skin"][item].get("icon"), skin_path + json_data["skin"][item].get("icon"))
    for file in json_data["skin"][item].get("image"):
        shutil.copy(config_data.get("images_path") + file, skin_path + file)

for item in json_data["hat"]:
    shutil.copy(config_data.get("images_path") + json_data["hat"][item].get("icon"), hat_path + json_data["hat"][item].get("icon"))
    shutil.copy(config_data.get("images_path") + json_data["hat"][item].get("image"), hat_path + json_data["hat"][item].get("image"))

for item in json_data["emote"]:
    shutil.copy(config_data.get("images_path") + json_data["emote"][item].get("icon"), emote_path + json_data["emote"][item].get("icon"))
    shutil.copy(config_data.get("images_path") + json_data["emote"][item].get("image"), emote_path + json_data["emote"][item].get("image"))
