using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Reapers.Core;
using System;

public class FoodInventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject infoPopup;
    [SerializeField] private TextMeshProUGUI infoPopupMsg;

    private PlayerControls playerControls;
    private bool inFrontOfFoodStall = false;
    private FoodStall foodStall;

    private const string FOOD_STALL_TAG = "Food Stall";

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        playerControls.Player.Interact.performed += _ => Interact();
    }

    private void Interact()
    {
        if (inFrontOfFoodStall &&  foodStall.FoodAvailable())
        {
            foodStall.CollectFood();
            infoPopup.SetActive(true);
            infoPopupMsg.text = "Food Collected";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(FOOD_STALL_TAG))
        {
            inFrontOfFoodStall = true;
            foodStall = other.gameObject.GetComponent<FoodStall>();
            foodStall.ShowInteractPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(FOOD_STALL_TAG))
        {
            inFrontOfFoodStall = false;
            foodStall.ShowInteractPrompt(false);
        }
    }
}
