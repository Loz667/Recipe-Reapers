using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStall : MonoBehaviour
{
    [SerializeField] InventoryItem[] stockedIngredients;
    [SerializeField] List<InventoryItem> availableIngredients;

    [SerializeField] private float timeToProcess;
    private float timer;

    [SerializeField] private GameObject interactPrompt;

    private Inventory inventory;

    private void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
    }

    private void Start()
    {
        timer = timeToProcess;
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                CreateAvailableItems();
            }
        }
    }

    private void CreateAvailableItems()
    {
        availableIngredients = new List<InventoryItem>(stockedIngredients);
    }

    public void CollectFood()
    {
        foreach(InventoryItem item in availableIngredients)
        {
            inventory.AddToFirstEmptySlot(item);
        }
        availableIngredients.Clear();
    }

    public bool FoodAvailable()
    {
        return availableIngredients.Count != 0;
    }

    public void ShowInteractPrompt(bool showPrompt)
    {
        if (showPrompt)
        {
            interactPrompt.SetActive(true);
        }
        else
        {
            interactPrompt.SetActive(false);
        }
    }
}
