import os
import json
import requests

from steam.client import SteamClient
from steam.enums.common import EResult

def get_inventory_info(client, game_id):
    return client.send_um_and_wait("Inventory.GetItemDefMeta#1", {"appid": game_id})

def generate_inventory(client, game_id):
    inventory = get_inventory_info(client, game_id)
    if inventory.header.eresult != EResult.OK:
        return None

    url = "https://api.steampowered.com/IGameInventory/GetItemDefArchive/v0001?appid={}&digest={}".format(game_id, inventory.body.digest)
    print(url)
    if os.path.exists("config.json"):
        with open("config.json", "r+", encoding="utf-8") as config_file:
            data = json.load(config_file)
            data["url"] = url
            config_file.seek(0)
            json.dump(data, config_file, ensure_ascii=False, indent=4)
            config_file.truncate()
    else:
        with open("config.json", "w", encoding="utf-8") as config_file:
            data = {}
            data["url"] = url
            json.dump(data, config_file, indent=4, ensure_ascii=False)

    response = requests.get(url)
    if response.status_code == 200:
        content = response.content.rstrip(b'\x00')
        return json.loads(content.decode("utf-8"))
    else:
        return None

def main():
    appid = 3419430

    client = SteamClient()
    client.anonymous_login()

    inventory_data = generate_inventory(client, appid)
    if inventory_data is not None:
        with open("inventory.json", "w", encoding="utf-8") as file:
            json.dump(inventory_data, file)

if __name__ == "__main__":
    main()
