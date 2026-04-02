import shutil
import json
import os

# 加载配置
with open("config.json", "r", encoding="utf-8") as config:
    config_data = json.load(config)

base_dest = config_data.get("destination_path")
base_images = config_data.get("images_path")

# 定义类别
categories = ["skin", "hat", "emote", "consumable"]

# 清空并创建目录
for cat in categories:
    path = os.path.join(base_dest, cat)
    if os.path.exists(path):
        shutil.rmtree(path)
    os.makedirs(path)

# 加载物品数据
with open(os.path.join(base_dest, "items.json"), "r", encoding="utf-8") as file:
    json_data = json.load(file)

# 复制文件
for cat in categories:
    dest_dir = os.path.join(base_dest, cat)
    for item_name, item_info in json_data.get(cat, {}).items():
        # 复制 icon
        icon = item_info.get("icon")
        if icon:
            shutil.copy2(os.path.join(base_images, icon), os.path.join(dest_dir, icon))
        
        # 复制 image(s)
        images = item_info.get("image")
        if isinstance(images, str):
            images = [images]
        for img in images or []:
            shutil.copy2(os.path.join(base_images, img), os.path.join(dest_dir, img))
