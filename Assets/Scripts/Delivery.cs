using UnityEngine;
using System.Collections;
public class Delivery : MonoBehaviour
{ 
    public IniciarPc ScriptMenu;
    public GameObject Deliver;
    public GameObject UI;

    public void Salir()
    {
        
        var canvas = UI.GetComponent<Canvas>();
        canvas.enabled = true;
        ScriptMenu.MenuAbierto=false;
        Deliver.SetActive(false);
    }

}
