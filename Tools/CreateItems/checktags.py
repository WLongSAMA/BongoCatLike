import json

def extract_tags_from_json(json_data):
    all_tags = set()
    for item in json_data:
        if "tags" in item and item["tags"]:
            tags = item["tags"].split(";")
            for tag in tags:
                tag = tag.strip()
                if tag:
                    all_tags.add(tag)
    return sorted(all_tags)

def process_tags_from_file(filename):
    try:
        with open(filename, "r") as file:
            data = json.load(file)
            return extract_tags_from_json(data)
    except FileNotFoundError:
        print(f"文件 {filename} 不存在")
        return []
    except json.JSONDecodeError:
        print(f"文件 {filename} 不是有效的JSON格式")
        return []

with open("lasttags.json", "r") as openfile:
    lasttags = json.load(openfile)
    newtags = process_tags_from_file("inventory.json")
    if lasttags != newtags:
        print(f"检测到 tags 已经变动")
        with open("newtags.json", "w") as savefile:
            json.dump(newtags, savefile)
