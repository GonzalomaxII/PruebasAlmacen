using UnityEngine;
using System.Collections;
using TMPro;
public class Tienda : MonoBehaviour
{

    public int enlatados = 0;
    public int especias = 0;
    public int carne = 0;
    public int pasta = 0;
    public int pizza = 0;
    public int fritos = 0;
    public int total = 0;

    public int cantidad = 0;
    public TextMeshProUGUI TotalTXT;
    public TextMeshProUGUI CantidadTXT;
    public TextMeshProUGUI BalanceTXT;
    public TextMeshProUGUI BalanceUI;
    public Valores ScriptValores;
    public MovimientoCamion movingObject; 

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
        
    }

    public void AnadirEspecias()
    {
        especias++;
        cantidad++;
        total+=200;
        
    }

    public void AnadirCarne()
    {
        carne++;
        cantidad++;
        total+=1000;
        
    }

    public void AnadirPasta()
    {
        pasta++;
        cantidad++;
        total+=250;
        
    }

    public void AnadirPizza()
    {
        pizza++;
        cantidad++;
        total+=500;
        
    }

    public void AnadirFritos()
    {
        fritos++;
        cantidad++;
        total+=700;
        
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
        
    }

    public void ConfirmarCompra()
    {
        if(ScriptValores.Plata-total>0){
            ScriptValores.Plata-=total;
            total=0;
            ScriptValores.EstadoDelivery=1;
            movingObject.MoveToFirstPosition();
            Carga.SetActive(true);
            
            Comprar.SetActive(false);
        }
    }



    public void Salir()
    {
        VaciarCompras();
        Comprar.SetActive(false);
        var canvas = UI.GetComponent<Canvas>();
        canvas.enabled = true;
        ScriptMenu.MenuAbierto=false;
    }
    
    public void ActualizarUI()
    {
        if (TotalTXT != null) TotalTXT.text = "Total: "+total.ToString();
        if (CantidadTXT != null) CantidadTXT.text = "Cantidad: "+cantidad.ToString();
        if (BalanceTXT != null) BalanceTXT.text = "Balance: "+ScriptValores.Plata.ToString()+"$";
        if (BalanceUI != null) BalanceUI.text = "Balance: "+ScriptValores.Plata.ToString()+"$";
    }
    
}
