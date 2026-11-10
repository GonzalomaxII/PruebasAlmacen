using UnityEngine;

public class Valores : MonoBehaviour
{
    public int Plata = 10000;
    public int EstadoDelivery = 0;

    [Header("Caja que lleva encima")]
    [Tooltip("Indica si el jugador actualmente transporta un bulto")]
    public bool tieneCaja = false;
    [Tooltip("Tipo de producto: Enlatados, Especias, Carne, Pasta, Pizza, Fritos")]
    public string tipoCaja = "";
    [Tooltip("Cantidad de unidades que trae la caja en mano")]
    public int unidadesCaja = 0;

    [Header("Inventario general")]
    public int enlatados = 0;
    public int especias = 0;
    public int carne = 0;
    public int pasta = 0;
    public int pizza = 0;
    public int fritos = 0;
    public int cantidad = 0;
}
