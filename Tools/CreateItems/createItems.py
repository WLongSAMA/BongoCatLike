import os
import json

with open("config.json", "r") as config:
    config_data = json.load(config)

content = ""
if os.path.exists("inventory.json"):
    with open("inventory.json", "r") as file:
        content = file.read()
else:
    import requests
    response = requests.get(config_data.get("url"))
    content = response.text.rstrip(b'\x00'.decode())
    if response.status_code == 200:
        with open("inventory.json", "w") as file:
            file.write(content)

json_data = json.loads(content)

def remove_keywords(text):
    keywords = [";itemslot:skin",
                ";itemslot:hat",
                ";itemslot:emote",
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
for item in json_data:
    if item.get("type") == "item" and item.get("tags") != "quality:special":
        data = {
            "name": item.get("name"),
            "description": item.get("description"),
            "modified": item.get("modified"),
            "created": item.get("date_created"),
            "icon": item.get("icon_url").split("/")[-1],
            "image": "",
            "tags": remove_keywords(item.get("tags")),
        }
        if item.get("item_slot") == "skin":
            name = data["icon"][:-8]
            data["image"] = [
                name + "Left.png",
                name + "LeftPunch.png",
                name + "Right.png",
                name + "RightPunch.png"
            ]
            skindata[item.get("itemdefid")] = data
        elif item.get("item_slot") == "hat":
            data["image"] = data["icon"].replace("Icon.", ".")
            hatdata[item.get("itemdefid")] = data
        elif item.get("item_slot") == "emote":
            data["image"] = data["icon"].replace("Icon.", ".")
            emotedata[item.get("itemdefid")] = data

newdata = {
    "skin": skindata,
    "hat": hatdata,
    "emote": emotedata
}

with open(config_data.get("destination_path") + "items.json", "w") as json_file:
    json.dump(newdata, json_file)

print("skin:", len(newdata["skin"]))
print("hat:", len(newdata["hat"]))
print("emote:", len(newdata["emote"]))
