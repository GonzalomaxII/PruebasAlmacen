using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IniciarPc : MonoBehaviour, interacuar
{

    public bool MenuAbierto= false;
    public GameObject Tienda;
    public GameObject UI;
    public GameObject Delivery;
    public Valores ScriptValores;
    
    public bool puedeinteract()
    {
        return !MenuAbierto;
    } 
    
    public void interact()
    {
        if (!puedeinteract()) return; 
        iniciapc(true);
    }


    private void iniciapc(bool abierto)
    {
        if (MenuAbierto = abierto )
        {
            if(ScriptValores.EstadoDelivery == 0)
            {
                Tienda.SetActive(true);
                var canvas = UI.GetComponent<Canvas>();
                canvas.enabled = false;
            }
            else if (ScriptValores.EstadoDelivery == 1)
            {
                Delivery.SetActive(true);
                var canvas = UI.GetComponent<Canvas>();
                canvas.enabled = false;
            }
        }
    } 
    
    
}
