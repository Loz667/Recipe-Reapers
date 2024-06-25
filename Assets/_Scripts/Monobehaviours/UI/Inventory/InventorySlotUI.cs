using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] InventoryItemIcon icon = null;

    int index;
    Inventory inventory;

    public void SetupInventory(Inventory _inventory, int _index)
    {
        this.inventory = _inventory;
        this.index = _index;
        icon.SetItem(_inventory.GetItemInSlot(_index));
    }

    public int MaxAcceptable(InventoryItem item)
    {
        if (inventory.HasSpaceFor(item))
        {
            return int.MaxValue;
        }
        return 0;
    }

    public void AddItems(InventoryItem item, int number)
    {
        inventory.AddItemToSlot(index, item);
    }

    public InventoryItem GetItem()
    {
        return inventory.GetItemInSlot(index);
    }

    public int GetNumber()
    {
        return 1; 
    }

    public void RemoveItems(int number)
    {
        inventory.RemoveFromSlot(index);
    }
}
