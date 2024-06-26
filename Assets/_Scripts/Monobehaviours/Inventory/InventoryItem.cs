using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] string itemID = null;
    [SerializeField] string itemName = null;
    [SerializeField, TextArea] string itemDescription = null;
    [SerializeField] Sprite itemIcon = null;
    [SerializeField] bool isStackable = false;

    static Dictionary<string, InventoryItem> inventoryItems;

    public static InventoryItem GetFromID(string _itemID)
    {
        if (inventoryItems == null)
        {
            inventoryItems = new Dictionary<string, InventoryItem>();
            var itemList = Resources.LoadAll<InventoryItem>("");
            foreach (var item in itemList)
            {
                if (inventoryItems.ContainsKey(item.itemID))
                {
                    Debug.LogError(string.Format("There appears to be a duplicate item ID for objects {0} and {1}", 
                        inventoryItems[item.itemID], item));

                    continue;
                }
                inventoryItems[item.itemID] = item;
            }
        }

        if (_itemID == null || !inventoryItems.ContainsKey(_itemID)) return null;
        return inventoryItems[_itemID];
    }

    public Sprite GetIcon()
    {
        return itemIcon;
    }

    public string GetItemID()
    {
        return itemID;
    }

    public bool IsStackable()
    {
        return isStackable;
    }

    public string GetItemName()
    {
        return itemName;
    }

    public string GetItemDescription()
    {
        return itemDescription;
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        if (string.IsNullOrWhiteSpace(itemID))
        {
            itemID = System.Guid.NewGuid().ToString();
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {

    }
}
