using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    PartyManager partyManager;

    private void Awake()
    {
        partyManager = FindFirstObjectByType<PartyManager>();
    }

    private void Update()
    {
        GetComponent<TextMeshProUGUI>().text = String.Format("{0}", partyManager.GetCurrentHealth());
    }
}