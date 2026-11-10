using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SacarCamion : MonoBehaviour, interacuar
{
    public IniciarPc ScriptMenu;
    public GameObject UI;
    public GameObject PopUp;
    public TextMeshProUGUI Agarrado;

    public Valores Jugador;
    public Tienda Totales;
    public MovimientoCamion movingObject; 

    [Header("Unidades por Caja")]
    public int unidadesEnlatados = 20;
    public int unidadesEspecias = 15;
    public int unidadesCarne = 20;
    public int unidadesPasta = 20;
    public int unidadesPizza = 10;
    public int unidadesFritos = 10;
    
    public bool puedeinteract()
    {
        // Se puede interactuar si el menú está cerrado y el camión aún tiene mercadería
        return !ScriptMenu.MenuAbierto && !CamionEstaVacio();
    } 

    public void interact()
    {
        if (!puedeinteract()) return; 
        AgarrarProd();
    }

    private void AgarrarProd()
    {
        ScriptMenu.MenuAbierto = true;
        PopUp.SetActive(true);

        // Regla: Solo 1 caja a la vez
        if (Jugador.tieneCaja || Jugador.cantidad > 0)
        {
            Agarrado.text = $"¡Ya llevás una caja de {Jugador.tipoCaja} encima!\nGuardala en el depósito antes de sacar otra.";
            return;
        }

        // Descargamos 1 caja del primer producto que tenga stock disponible
        if (Totales.enlatados > 0)
        {
            Totales.enlatados--;
            AsignarCajaAlJugador("Enlatados", unidadesEnlatados);
            Agarrado.text = $"Recogiste:\n1 caja de {unidadesEnlatados} productos enlatados";
        }
        else if (Totales.especias > 0)
        {
            Totales.especias--;
            AsignarCajaAlJugador("Especias", unidadesEspecias);
            Agarrado.text = $"Recogiste:\n1 caja de {unidadesEspecias} especias";
        }
        else if (Totales.carne > 0)
        {
            Totales.carne--;
            AsignarCajaAlJugador("Carne", unidadesCarne);
            Agarrado.text = $"Recogiste:\n1 caja de {unidadesCarne} carne";
        }
        else if (Totales.pasta > 0)
        {
            Totales.pasta--;
            AsignarCajaAlJugador("Pasta", unidadesPasta);
            Agarrado.text = $"Recogiste:\n1 caja de {unidadesPasta} pasta";
        }
        else if (Totales.pizza > 0)
        {
            Totales.pizza--;
            AsignarCajaAlJugador("Pizza", unidadesPizza);
            Agarrado.text = $"Recogiste:\n1 caja de {unidadesPizza} pizzas";
        }
        else if (Totales.fritos > 0)
        {
            Totales.fritos--;
            AsignarCajaAlJugador("Fritos", unidadesFritos);
            Agarrado.text = $"Recogiste:\n1 caja de {unidadesFritos} fritos";
        }

        // Restamos 1 a la cantidad de bultos general
        Totales.cantidad = Mathf.Max(0, Totales.cantidad - 1);
    }

    private void AsignarCajaAlJugador(string tipo, int unidades)
    {
        Jugador.tieneCaja = true;
        Jugador.tipoCaja = tipo;
        Jugador.unidadesCaja = unidades;
        Jugador.cantidad = unidades;

        // Mantener compatibilidad con variables numéricas individuales
        Jugador.enlatados = (tipo == "Enlatados") ? unidades : 0;
        Jugador.especias = (tipo == "Especias") ? unidades : 0;
        Jugador.carne = (tipo == "Carne") ? unidades : 0;
        Jugador.pasta = (tipo == "Pasta") ? unidades : 0;
        Jugador.pizza = (tipo == "Pizza") ? unidades : 0;
        Jugador.fritos = (tipo == "Fritos") ? unidades : 0;
    }

    public void SalirRecog()
    {
        ScriptMenu.MenuAbierto = false;
        PopUp.SetActive(false);

        if (UI != null)
        {
            var canvas = UI.GetComponent<Canvas>();
            if (canvas != null) canvas.enabled = true;
        }
        // Si el camión quedó sin ningún tipo de producto, se retira
        if (CamionEstaVacio())
        {
            Totales.cantidad = 0;
            Totales.ActualizarUI();
            movingObject.MoveToSecondPosition();
            Debug.Log("<color=green><b>[Camión]</b></color> Toda la mercadería fue descargada. El camión se va.");
        }
    }

    public bool CamionEstaVacio()
    {
        return Totales.enlatados <= 0 &&
               Totales.especias <= 0 &&
               Totales.carne <= 0 &&
               Totales.pasta <= 0 &&
               Totales.pizza <= 0 &&
               Totales.fritos <= 0;
    }
}
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;

// public class SacarCamion : MonoBehaviour, interacuar
// {
    


//     public IniciarPc ScriptMenu;
//     public GameObject UI;
//     public GameObject PopUp;
//     public TextMeshProUGUI Agarrado;

//     public Valores Jugador;
//     public Tienda Totales;
//     public MovimientoCamion movingObject; 

//     private int enlatados = 20;
//     private int especias = 15;
//     private int carne = 20;
//     private int pasta = 20;
//     private int pizza = 10;
//     private int fritos = 10;
    
//     public bool puedeinteract()
//     {
//         if(ScriptMenu.MenuAbierto==false && Totales.cantidad!=0){
//         return true;
//         }
//         else{
//             return false;
//         }
//     } 
//     public void SalirRecog()
//     {
//         ScriptMenu.MenuAbierto=false;
//         PopUp.SetActive(false);
//         if(Totales.cantidad==0){
//             Totales.ActualizarUI();
//             movingObject.MoveToSecondPosition();
            
//         }
//     }
//     public void interact()
//     {
//         if (!puedeinteract()) return; 
//         AgarrarProd(true);
//     }


//     private void AgarrarProd(bool abierto)
//     {
//         if (ScriptMenu.MenuAbierto = abierto )
//         {
//             if(Jugador.cantidad==0){
//                 PopUp.SetActive(true);
//                 if (Totales.enlatados>0){
//                     if (Totales.enlatados>5){
//                         Agarrado.text = "Recogiste: una caja de\n"+5*enlatados+" productos enlatados";
//                         Jugador.enlatados+=5*enlatados;
//                         Jugador.cantidad+=5*enlatados;
//                         Totales.enlatados-=5;
//                         Totales.cantidad-=5;
                        
//                     }
//                     else
//                     {
//                         Agarrado.text = "Recogiste: una caja de\n"+Totales.enlatados*enlatados+" productos enlatados";
//                         Jugador.enlatados+=Totales.enlatados*enlatados;
//                         Jugador.cantidad+=Totales.enlatados*enlatados;
//                         Totales.cantidad-=Totales.enlatados;
//                         Totales.enlatados-=Totales.enlatados;
//                     }
//                 }
//                 else if (Totales.especias>0){
//                     if (Totales.especias>5){
//                         Agarrado.text = "Recogiste: una caja de\n"+5*especias+" productos especias";
//                         Jugador.especias+=5*especias;
//                         Jugador.cantidad+=5*especias;
//                         Totales.especias-=5;
//                         Totales.cantidad-=5;
//                     }
//                     else
//                     {
//                         Agarrado.text = "Recogiste: una caja de\n"+Totales.especias*especias+" especias";
//                         Jugador.especias+=Totales.especias*especias;
//                         Jugador.cantidad+=Totales.especias*especias;
                        
//                         Totales.cantidad-=Totales.especias;
//                         Totales.especias-=Totales.especias;
//                     }
//                 }
//                 else if (Totales.carne>0){
//                     if (Totales.carne>5){
//                         Agarrado.text = "Recogiste: una caja de\n"+5*carne+" carne";
//                         Jugador.carne+=5*carne;
//                         Jugador.cantidad+=5*carne;
//                         Totales.carne-=5;
//                         Totales.cantidad-=5;
//                     }
//                     else
//                     {
//                         Agarrado.text = "Recogiste: una caja de\n"+Totales.carne*carne+" carne";
//                         Jugador.carne+=Totales.carne*carne;
//                         Jugador.cantidad+=Totales.carne*carne;
                        
//                         Totales.cantidad-=Totales.carne;
//                         Totales.carne-=Totales.carne;
//                     }
//                 }
//                 else if (Totales.pasta>0){
//                     if (Totales.pasta>5){
//                         Agarrado.text = "Recogiste: una caja de\n"+5*pasta+" pasta";
//                         Jugador.pasta+=5*pasta;
//                         Jugador.cantidad+=5*pasta;
//                         Totales.pasta-=5;
//                         Totales.cantidad-=5;
//                     }
//                     else
//                     {
//                         Agarrado.text = "Recogiste: una caja de\n"+Totales.pasta*pasta+" pasta";
//                         Jugador.pasta+=Totales.pasta*pasta;
//                         Jugador.cantidad+=Totales.pasta*pasta;
                        
//                         Totales.cantidad-=Totales.pasta;
//                         Totales.pasta-=Totales.pasta;
//                     }
//                 }
//                 else if (Totales.pizza>0){
//                     if (Totales.pizza>5){
//                         Agarrado.text = "Recogiste: una caja de\n"+5*pizza+" pizzas";
//                         Jugador.pizza+=5*pizza;
//                         Jugador.cantidad+=5*pizza;
//                         Totales.pizza-=5;
//                         Totales.cantidad-=5;
//                     }
//                     else
//                     {
//                         Agarrado.text = "Recogiste: una caja de\n"+Totales.pizza*pizza+" pizzas";
//                         Jugador.pizza+=Totales.pizza*pizza;
//                         Jugador.cantidad+=Totales.pizza*pizza;
                        
//                         Totales.cantidad-=Totales.pizza;
//                         Totales.pizza-=Totales.pizza;
//                     }
//                 }
//                 else if (Totales.fritos>0){
//                     if (Totales.fritos>5){
//                         Agarrado.text = "Recogiste: una caja de\n"+5*fritos+" fritos";
//                         Jugador.fritos+=5*fritos;
//                         Jugador.cantidad+=5*fritos;
//                         Totales.fritos-=5;
//                         Totales.cantidad-=5;
//                     }
//                     else
//                     {
//                         Agarrado.text = "Recogiste: una caja de\n"+Totales.fritos*fritos+" fritos";
//                         Jugador.fritos+=Totales.fritos*fritos;
//                         Jugador.cantidad+=Totales.fritos*fritos;
                        
//                         Totales.cantidad-=Totales.fritos;
//                         Totales.fritos-=Totales.fritos;
//                     }
//                 }
//                 var canvas = UI.GetComponent<Canvas>();
//                 canvas.enabled = false;
//             }
//         }
//     } 
// }
