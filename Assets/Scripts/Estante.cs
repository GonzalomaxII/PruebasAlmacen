using UnityEngine;

public class Estante : MonoBehaviour
{
    [Header("Configuración del Producto")]
    public Producto producto; // Qué producto vende este estante específico
    public int stockMaximo = 10;
    public int stockActual = 10;

    public bool TieneStock() => stockActual > 0;

    // El cliente llama a esto para llevarse mercadería
    public int RetirarStock(int cantidadPedida)
    {
        int cantidadDada = Mathf.Min(cantidadPedida, stockActual);
        stockActual -= cantidadDada;
        return cantidadDada;
    }
}
