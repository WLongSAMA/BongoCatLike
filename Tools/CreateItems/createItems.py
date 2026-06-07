import os
import json

with open("config.json", "r", encoding="utf-8") as config:
    config_data = json.load(config)

content = ""
if os.path.exists("inventory.json"):
    with open("inventory.json", "r", encoding="utf-8") as file:
        content = file.read()
else:
    import requests
    response = requests.get(config_data.get("url"))
    content = response.text.rstrip(b'\x00'.decode())
    if response.status_code == 200:
        with open("inventory.json", "w", encoding="utf-8") as file:
            file.write(content)

json_data = json.loads(content)

def remove_keywords(text):
    keywords = [";itemslot:skin",
                ";itemslot:hat",
                ";itemslot:emote",
                ";itemslot:consumable",
                ";cosmetics_quality:common",
                ";cosmetics_quality:uncommon",
                ";cosmetics_quality:rare",
                ";cosmetics_quality:epic",
                ";cosmetics_quality:legendary",
                ";emote_quality:common",
                ";emote_quality:uncommon",
                ";emote_quality:rare",
                ";emote_quality:epic",
                ";emote_quality:legendary"
                ]
    for keyword in keywords:
        text = text.replace(keyword, '')
    return text

skindata = {}
hatdata = {}
emotedata = {}
consumabledata = {}
for item in json_data:
    if item.get("type") == "item" and item.get("tags") != "quality:special" and item.get("tags", "") != "":
        data = {
            "name": item.get("name"),
            "description": item.get("description"),
            "modified": item.get("modified"),
            "created": item.get("date_created"),
            "icon": item.get("icon_url").split("/")[-1],
            "image": "",
            "tags": remove_keywords(item.get("tags")),
        }
        if item.get("tags").find("itemslot:skin;") != -1:
            name = data["icon"][:-8]
            data["image"] = [
                name + "Left.png",
                name + "LeftPunch.png",
                name + "Right.png",
                name + "RightPunch.png"
            ]
            skindata[item.get("itemdefid")] = data
        elif item.get("tags").find("itemslot:hat;") != -1:
            data["image"] = data["icon"].replace("Icon.", ".")
            hatdata[item.get("itemdefid")] = data
        elif item.get("tags").find("itemslot:emote;") != -1:
            data["image"] = data["icon"].replace("Icon.", ".")
            emotedata[item.get("itemdefid")] = data
        elif item.get("tags").find("itemslot:consumable;") != -1:
            data["image"] = data["icon"].replace("Icon.", ".")
            consumabledata[item.get("itemdefid")] = data

newdata = {
    "skin": skindata,
    "hat": hatdata,
    "emote": emotedata,
    "consumable": consumabledata
}

with open(config_data.get("destination_path") + "items.json", "w", encoding="utf-8") as json_file:
    json.dump(newdata, json_file)

print("skin:", len(newdata["skin"]))
print("hat:", len(newdata["hat"]))
print("emote:", len(newdata["emote"]))
print("consumable:", len(newdata["consumable"]))
