using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacarCamion : MonoBehaviour, interacuar
{
    private int enlatados = 0;
    private int especias = 0;
    private int carne = 0;
    private int pasta = 0;
    private int pizza = 0;
    private int fritos = 0;
    private int total = 0;


    public IniciarPc ScriptMenu;
    public GameObject UI;
    public GameObject PopUp;
    public Valores ScriptValores;
    
    public bool puedeinteract()
    {
        if(ScriptMenu.MenuAbierto==false && total==0){
        return true;
        }
        else{
            return false;
        }
    } 
    
    public void interact()
    {
        if (!puedeinteract()) return; 
        AgarrarProd(true);
    }


    private void AgarrarProd(bool abierto)
    {
        if (ScriptMenu.MenuAbierto = abierto )
        {
            PopUp.SetActive(true);
            var canvas = UI.GetComponent<Canvas>();
            canvas.enabled = false;
        }
    } 
}
