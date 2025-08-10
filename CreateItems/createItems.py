import json
import requests

url = "https://api.steampowered.com/IGameInventory/GetItemDefArchive/v0001?appid=3419430&digest=CEAA2C94E932B799E7B7498D18EB450004622927"

response = requests.get(url, verify=False)
content = response.text.rstrip(b'\x00'.decode())
if response.status_code == 200:
    with open("inventory.json", "w") as file:
        file.write(content)

json_data = json.loads(content)

newdata = {}
skindata = []
hatdata = []
for item in json_data:
    if item.get("type") == "item" and item.get("tags") != "quality:special":
        data = {
            "itemdefid": item.get("itemdefid"),
            "Timestamp": item.get("Timestamp"),
            "modified": item.get("modified"),
            "date_created": item.get("date_created"),
            "name": item.get("name"),
            "description": item.get("description"),
            "icon": item.get("icon_url").split("/")[-1],
            "image": "",
            "name_color": item.get("name_color"),
            "quality": item.get("tags").replace("quality:", "").replace(";itemslot:skin", "").replace(";itemslot:hat", ""),
        }
        if item.get("item_slot") == "skin":
            images = []
            name = data["icon"][:-8]
            images.append(name + "Left.png")
            images.append(name + "LeftPunch.png")
            images.append(name + "Right.png")
            images.append(name + "RightPunch.png")
            data["image"] = images
            skindata.append(data)
        elif item.get("item_slot") == "hat":
            data["image"] = data["icon"].replace("Icon.", ".")
            hatdata.append(data)

newdata["skin"] = skindata
newdata["hat"] = hatdata

with open("items.json", "w") as json_file:
    json.dump(newdata, json_file, indent=2)

print("原始项目数量：", len(json_data))
print("新项目数量：", len(newdata["skin"]) + len(newdata["hat"]))
