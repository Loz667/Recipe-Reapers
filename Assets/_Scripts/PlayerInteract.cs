using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] InputActionReference interactAction;

    private void Update()
    {
        if (interactAction.action.triggered)
        {
            IInteractable interactable = GetInteractObject();
            if (interactable != null )
                interactable.Interact(transform);
        }
    }

    public IInteractable GetInteractObject()
    {
        List<IInteractable> interactableList = new List<IInteractable>();
        float interactRange = 2f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
                interactableList.Add(interactable);
        }

        IInteractable closestInteractable = null;
        foreach (IInteractable interactable in interactableList)
        {
            if (closestInteractable == null)
            {
                closestInteractable = interactable;
            }
            else
            {
                if (Vector3.Distance(transform.position, interactable.GetInteractTransform().position) <
                    Vector3.Distance(transform.position, closestInteractable.GetInteractTransform().position))
                {
                    closestInteractable = interactable;
                }            
            }
        }
        return closestInteractable;
    }
}
