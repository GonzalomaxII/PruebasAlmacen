using UnityEngine;
using UnityEngine.InputSystem;

public class DetectorInteract : MonoBehaviour
{
    private interacuar enrango = null;
    

    public void AlInteractuar(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            enrango?.interact();
        }
    }
//
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collison)
    {
        if(collison.TryGetComponent(out interacuar interactable) && interactable.puedeinteract())
        {
            enrango=interactable;
            
        }
    }

    private void OnTriggerExit2D(Collider2D collison)
    {
        if(collison.TryGetComponent(out interacuar interactable) && interactable == enrango)
        {
            enrango=null;
            
        }
    }
}
