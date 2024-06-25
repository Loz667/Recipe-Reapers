using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] int inventorySize = 16;

    InventoryItem[] inventorySlots;

    public event Action inventoryUpdated;

    public static Inventory GetPlayerInventory()
    {
        var player = GameObject.FindWithTag("Player");
        return player.GetComponent<Inventory>();
    }

    public bool HasSpaceFor(InventoryItem item)
    {
        return FindSlot(item) >= 0;
    }

    public int GetSize()
    {
        return inventorySlots.Length;
    }

    public bool AddToFirstEmptySlot(InventoryItem item)
    {
        int i = FindSlot(item);

        if (i < 0)
        {
            return false;
        }

        inventorySlots[i] = item;
        if (inventoryUpdated != null)
        {
            inventoryUpdated();
        }
        return true;
    }

    public bool HasItem(InventoryItem item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (object.ReferenceEquals(inventorySlots[i], item))
            {
                return true;
            }
        }
        return false;
    }

    public InventoryItem GetItemInSlot(int slot)
    {
        return inventorySlots[slot];
    }

    public void RemoveFromSlot(int slot)
    {
        inventorySlots[slot] = null;
        if (inventoryUpdated != null)
        {
            inventoryUpdated();
        }
    }

    public bool AddItemToSlot(int slot, InventoryItem item)
    {
        if (inventorySlots[slot] != null)
        {
            return AddToFirstEmptySlot(item);
        }

        inventorySlots[slot] = item;
        if (inventoryUpdated != null)
        {
            inventoryUpdated();
        }
        return true;
    }

    private void Awake()
    {
        inventorySlots = new InventoryItem[inventorySize];
    }

    private int FindSlot(InventoryItem item)
    {
        return FindEmptySlot();
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null)
            {
                return i;
            }
        }
        return -1;
    }
}
