using UnityEngine;
using System.Collections;
using TMPro;
public class Tienda : MonoBehaviour
{

    private int enlatados = 0;
    private int especias = 0;
    private int carne = 0;
    private int pasta = 0;
    private int pizza = 0;
    private int fritos = 0;
    private int total = 0;

    private int balance = 10000;
    private int cantidad = 0;
    public TextMeshProUGUI TotalTXT;
    public TextMeshProUGUI CantidadTXT;
    public TextMeshProUGUI BalanceTXT;
    public TextMeshProUGUI BalanceUI;
    public Valores ScriptValores;

    public IniciarPc ScriptMenu;

    public GameObject Comprar;
    public GameObject UI;

    public GameObject Carga;
    public Delivery Deliver;

    public void AnadirEnlatados()
    {
        enlatados++;
        cantidad++;
        total+=500;
        ActualizarUI();
    }

    public void AnadirEspecias()
    {
        especias++;
        cantidad++;
        total+=200;
        ActualizarUI();
    }

    public void AnadirCarne()
    {
        carne++;
        cantidad++;
        total+=1000;
        ActualizarUI();
    }

    public void AnadirPasta()
    {
        pasta++;
        cantidad++;
        total+=250;
        ActualizarUI();
    }

    public void AnadirPizza()
    {
        pizza++;
        cantidad++;
        total+=500;
        ActualizarUI();
    }

    public void AnadirFritos()
    {
        fritos++;
        cantidad++;
        total+=700;
        ActualizarUI();
    }
    public void VaciarCompras()
    {
        enlatados=0;
        especias=0;
        carne=0;
        pasta=0;
        pizza=0;
        fritos=0;
        cantidad=0;
        total=0;
        ActualizarUI();
    }

    public void ConfirmarCompra()
    {
        if(ScriptValores.Plata-total>0){
            enlatados=0;
            especias=0;
            carne=0;
            pasta=0;
            pizza=0;
            fritos=0;
            cantidad=0;
            ScriptValores.Plata-=total;
            total=0;
            balance=ScriptValores.Plata;
            ScriptValores.EstadoDelivery=1;
            Carga.SetActive(true);
            Deliver.AlDelivery();
            ActualizarUI();
            Comprar.SetActive(false);
        }
    }



    public void Salir()
    {
        Comprar.SetActive(false);
        var canvas = UI.GetComponent<Canvas>();
        canvas.enabled = true;
        ScriptMenu.MenuAbierto=false;
    }
    
    private void ActualizarUI()
    {
        if (TotalTXT != null) TotalTXT.text = "Total: "+total.ToString();
        if (CantidadTXT != null) CantidadTXT.text = "Cantidad: "+cantidad.ToString();
        if (BalanceTXT != null) BalanceTXT.text = "Balance: "+balance.ToString()+"$";
        if (BalanceUI != null) BalanceUI.text = "Balance: "+balance.ToString()+"$";
    }
    
}
