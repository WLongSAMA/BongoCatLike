import json

def extract_tags_from_json(json_data):
    all_tags = set()
    for item in json_data:
        if 'tags' in item and item['tags']:
            tags = item['tags'].split(';')
            for tag in tags:
                tag = tag.strip()
                if tag:
                    all_tags.add(tag)

    return sorted(list(all_tags))

def process_tags_from_file(filename):
    try:
        with open(filename, 'r') as file:
            data = json.load(file)
            tags = extract_tags_from_json(data)
            return tags
    except FileNotFoundError:
        print(f"文件 {filename} 不存在")
        return []
    except json.JSONDecodeError:
        print(f"文件 {filename} 不是有效的JSON格式")
        return []

tags = process_tags_from_file('inventory.json')
for i, tag in enumerate(tags, 1):
    print(f"{i}. {tag}")
