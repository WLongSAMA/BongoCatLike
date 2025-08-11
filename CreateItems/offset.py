import shutil
import json

destination_path = "..\\Assets\\"

with open("inventory.json", "r") as file:
    json_data = json.load(file)

newdata = {}
for item in json_data:
    if item.get("type") == "item" and item.get("tags") != "quality:special":
        newdata[item.get("icon_url").split("/")[-1][:-8]] = {
            "x": 30,
            "y": -70,
        }

with open("offset.json", "w") as json_file:
    json.dump(newdata, json_file, indent=2)

shutil.copy("offset.json", destination_path + "offset.json")
