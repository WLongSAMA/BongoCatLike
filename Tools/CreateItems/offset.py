import json

destination_path = "..\\..\\Assets\\"

with open("inventory.json", "r") as file:
    json_data = json.load(file)

newdata = []
for item in json_data:
    if item.get("type") == "item" and item.get("tags") != "quality:special":
        newdata.append({
            "name": item.get("name"),
            "position": {"x": 30, "y": -70},
        })

with open(destination_path + "offset.json", "w") as json_file:
    json.dump(newdata, json_file)
