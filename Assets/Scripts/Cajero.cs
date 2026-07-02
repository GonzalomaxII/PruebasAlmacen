using System.Collections.Generic;
using UnityEngine;

public class Cajero : MonoBehaviour
{
    [Header("Configuración de la Fila")]
    [Tooltip("La celda exacta frente a la caja (el lugar de quien está pagando)")]
    public Vector2Int celdaParaPagar; 

    [Tooltip("Hacia dónde crece la fila. Ej: (0, -1) hace que la fila vaya hacia abajo en Y")]
    public Vector2Int direccionFila = new Vector2Int(0, -1);

    // Lista para recordar qué clientes están esperando y en qué orden
    private List<CerebroCliente> filaDeClientes = new List<CerebroCliente>();

    // El cliente llama a esto para pedir su lugar en la fila
    public Vector2Int UnirseAFila(CerebroCliente clienteNuevo)
    {
        filaDeClientes.Add(clienteNuevo);
        
        // Su posición en la fila es el índice actual (si es el primero, es 0; el segundo, 1, etc.)
        int posicion = filaDeClientes.Count - 1; 

        return CalcularCelda(posicion);
    }

    // Esto lo llamaremos más adelante cuando el jugador atienda al cliente
    public void AtenderSiguiente()
    {
        if (filaDeClientes.Count > 0)
        {
            // El primero de la fila ya pagó, lo sacamos de la lista
            CerebroCliente clienteQueSeVa = filaDeClientes[0];
            filaDeClientes.RemoveAt(0);

            // TODO: Acá le diremos al clienteQueSeVa que camine a la salida del local

            // Como se liberó el primer lugar, hacemos que el resto dé un paso al frente
            for (int i = 0; i < filaDeClientes.Count; i++)
            {
                Vector2Int nuevaCelda = CalcularCelda(i);
                filaDeClientes[i].AvanzarEnFila(nuevaCelda);
            }
        }
    }

    // Calcula la coordenada exacta sumando lugares hacia atrás
    private Vector2Int CalcularCelda(int lugarEnFila)
    {
        // Si la dirección es (0, -1) y es el 3er cliente (lugarEnFila = 2):
        // celdaParaPagar + (0, -2) = ¡Dos baldosas más abajo!
        return celdaParaPagar + (direccionFila * lugarEnFila);
    }
}
