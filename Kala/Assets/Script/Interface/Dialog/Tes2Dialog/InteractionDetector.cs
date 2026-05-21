using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange= null;
    public GameObject interactionIcon;

    private Vector3 initLocalScale;

    void Start()
    {
        interactionIcon.SetActive(false);
        initLocalScale = transform.localScale;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactableInRange?.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteractable interactable) && interactable.Caninteract())
        {
            interactableInRange = interactable;

            if (interactable.CanDirectInteract())
                interactableInRange.Interact();
            else
                interactionIcon.SetActive(true);
            

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }


    private void LateUpdate()
    {
        // Counteract parent X flip — health bar sentiasa hadap depan
        float worldScaleX = transform.parent.lossyScale.x;
        
        worldScaleX = (GetComponent<PlayerHealth>() != null) ? transform.lossyScale.x: worldScaleX; 

        transform.localScale = new Vector3(
            worldScaleX < 0 ? -initLocalScale.x : initLocalScale.x,
            initLocalScale.y,
            initLocalScale.z
        );
    }
}
