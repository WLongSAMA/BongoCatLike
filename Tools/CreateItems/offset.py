import json

with open("config.json", "r") as config:
    config_data = json.load(config)

with open("inventory.json", "r") as file:
    json_data = json.load(file)

newdata = []
for item in json_data:
    if item.get("type") == "item" and item.get("tags").find("quality:special") == -1 and item.get("item_slot") == "hat":
        newdata.append({
            "name": item.get("name"),
            "position": {"x": 30, "y": -70},
        })

with open(config_data.get("destination_path") + "offset.json", "w") as json_file:
    json.dump(newdata, json_file)
