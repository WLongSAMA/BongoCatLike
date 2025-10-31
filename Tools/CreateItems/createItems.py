import os
import json

destination_path = "..\\..\\Assets\\"

# url = "https://api.steampowered.com/IGameInventory/GetItemDefArchive/v0001?appid=3419430&digest=CEAA2C94E932B799E7B7498D18EB450004622927"
# url = "https://api.steampowered.com/IGameInventory/GetItemDefArchive/v0001?appid=3419430&digest=D93DA1D3EDE56235556DA16C5C2A5DC9303B0EC8"
#url = "https://api.steampowered.com/IGameInventory/GetItemDefArchive/v0001?appid=3419430&digest=988C208479DF444DC90D541CBBC907A2ACE9699D"
#url = "https://api.steampowered.com/IGameInventory/GetItemDefArchive/v0001?appid=3419430&digest=4BD478CB8836FAC7F881F83A3D2889B3002E8B1B"
url = "https://api.steampowered.com/IGameInventory/GetItemDefArchive/v0001?appid=3419430&digest=3A90AB764093B4624786D0121CB875EB0D31A37F"

content = ""
if os.path.exists("inventory.json"):
    with open("inventory.json", "r") as file:
        content = file.read()
else:
    import requests
    response = requests.get(url)
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

with open(destination_path + "items.json", "w") as json_file:
    json.dump(newdata, json_file)

print("skin:", len(newdata["skin"]))
print("hat:", len(newdata["hat"]))
print("emote:", len(newdata["emote"]))
