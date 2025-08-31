import os
import json
import requests

destination_path = "..\\..\\Assets\\"

# url = "https://api.steampowered.com/IGameInventory/GetItemDefArchive/v0001?appid=3419430&digest=CEAA2C94E932B799E7B7498D18EB450004622927"
url = "https://api.steampowered.com/IGameInventory/GetItemDefArchive/v0001?appid=3419430&digest=D93DA1D3EDE56235556DA16C5C2A5DC9303B0EC8"

content = ""
if os.path.exists("inventory.json"):
    with open("inventory.json", "r") as file:
        content = file.read()
else:
    response = requests.get(url, verify=False)
    content = response.text.rstrip(b'\x00'.decode())
    if response.status_code == 200:
        with open("inventory.json", "w") as file:
            file.write(content)

json_data = json.loads(content)

skindata = {}
hatdata = {}
for item in json_data:
    if item.get("type") == "item" and item.get("tags") != "quality:special":
        data = {
            "name": item.get("name"),
            "description": item.get("description"),
            "modified": item.get("modified"),
            "created": item.get("date_created"),
            "icon": item.get("icon_url").split("/")[-1],
            "image": "",
            "tags": item.get("tags").replace(";itemslot:skin", "").replace(";itemslot:hat", ""),
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

newdata = {
    "skin": skindata,
    "hat": hatdata
}

with open(destination_path + "items.json", "w") as json_file:
    json.dump(newdata, json_file)
