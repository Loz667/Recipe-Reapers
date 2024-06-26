using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShowHideUI : MonoBehaviour
{
    [SerializeField] GameObject pauseContainer;
    [SerializeField] GameObject inventoryContainer;

    private PlayerControls playerControls;

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

    private void Start()
    {
        playerControls.Player.Pause.performed += _ => Paused();
        playerControls.Player.Inventory.performed += _ => OpenInventory();
        pauseContainer.SetActive(false);
        inventoryContainer.SetActive(false);
    }

    private void Paused()
    {
        pauseContainer.SetActive(!pauseContainer.activeSelf);
    }

    private void OpenInventory()
    {
        inventoryContainer.SetActive(!inventoryContainer.activeSelf);
    }
}
