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

    response = requests.get(url)
    if response.status_code == 200:
        return response.text.rstrip(b'\x00'.decode())
    else:
        return None


def main():
    appid = 3419430

    client = SteamClient()
    client.anonymous_login()

    inventory_data = generate_inventory(client, appid)
    if inventory_data is not None:
        with open("inventory.json", "w") as file:
            file.write(inventory_data)


if __name__ == "__main__":
    main()
