using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : MonoBehaviour, IInteractable
{
    [SerializeField] string interactText;

    public string GetInteractText()
    {
        return interactText;
    }

    public Transform GetInteractTransform()
    {
        return transform;
    }

    public void Interact(Transform interactor)
    {
        
    }
}
