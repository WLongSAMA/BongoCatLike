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
        with open(filename, "r", encoding="utf-8") as file:
            data = json.load(file)
            return extract_tags_from_json(data)
    except FileNotFoundError:
        print(f"文件 {filename} 不存在")
        return []
    except json.JSONDecodeError:
        print(f"文件 {filename} 不是有效的JSON格式")
        return []

def main():
    with open("lasttags.json", "r", encoding="utf-8") as file:
        lasttags = json.load(file)
    newtags = process_tags_from_file("inventory.json")
    if lasttags != newtags:
        print(f"检测到 tags 已经变动")
        with open("lasttags.bak.json", "w", encoding="utf-8") as bakfile:
            json.dump(lasttags, bakfile, indent=4)
        with open("lasttags.json", "w", encoding="utf-8") as outfile:
            json.dump(newtags, outfile, indent=4)

if __name__ == "__main__":
    main()
