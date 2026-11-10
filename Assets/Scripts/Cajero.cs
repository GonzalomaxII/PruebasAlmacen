using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cajero : MonoBehaviour
{
    [Header("Configuración de la Fila")]
    [Tooltip("La celda exacta frente a la caja (quien está pagando)")]
    public Vector2Int celdaParaPagar; 

    [Tooltip("Hacia dónde crece la fila")]
    public Vector2Int direccionFila = new Vector2Int(1, 0);

    [Header("Interacción del Jugador")]
    public Transform jugador;
    public Valores valoresJugador;
    public float tiempoParaAtender = 2f;

    [Header("Barra de Carga (Sprites)")]
    [Tooltip("El SpriteRenderer del objeto BarraAtencion en la escena")]
    public SpriteRenderer barraRenderer;

    [Tooltip("Arrastrá los 5 sprites cortados en orden: de vacío a lleno")]
    public Sprite[] framesBarra;

    [Tooltip("Separación vertical de la barra por encima del NPC")]
    public Vector3 offsetBarra = new Vector3(0f, 1.2f, 0f);

    private List<CerebroCliente> filaDeClientes = new List<CerebroCliente>();
    private float progresoAtencion = 0f;

    void Awake()
    {
        if (barraRenderer != null)
            barraRenderer.gameObject.SetActive(false);
    }

    void Update()
    {
        // 1. Validar si hay un cliente listo en la caja
        if (filaDeClientes.Count == 0) return;
        CerebroCliente primerCliente = filaDeClientes[0];
        if (!primerCliente.esperandoEnCaja) return;

        // 2. Validar posición del jugador y tecla E presionada
        bool jugadorEnPosicion = jugador != null && MapaCliente.Instance.EstaAlLadoDeCajero(jugador.position);
        bool presionandoE = Keyboard.current != null && Keyboard.current.eKey.isPressed;

        if (jugadorEnPosicion && presionandoE)
        {
            progresoAtencion += Time.deltaTime;
            float porcentaje = Mathf.Clamp01(progresoAtencion / tiempoParaAtender);

            ActualizarBarraVisual(primerCliente.transform.position, porcentaje);

            if (progresoAtencion >= tiempoParaAtender)
            {
                progresoAtencion = 0f;
                OcultarBarra();
                AtenderSiguiente();
            }
        }
        else if (progresoAtencion > 0f)
        {
            // Reseteo inmediato si suelta la tecla antes de finalizar
            progresoAtencion = 0f;
            OcultarBarra();
        }
    }

    private void ActualizarBarraVisual(Vector3 posicionCliente, float porcentaje)
    {
        if (barraRenderer == null || framesBarra == null || framesBarra.Length == 0) return;

        if (!barraRenderer.gameObject.activeSelf)
            barraRenderer.gameObject.SetActive(true);

        // Ubica la barra sobre la cabeza del cliente actual
        barraRenderer.transform.position = posicionCliente + offsetBarra;

        // Mapea el porcentaje de 0 a 1 a los índices del arreglo de sprites
        int indiceFrame = Mathf.Clamp(Mathf.FloorToInt(porcentaje * framesBarra.Length), 0, framesBarra.Length - 1);
        barraRenderer.sprite = framesBarra[indiceFrame];
    }

    private void OcultarBarra()
    {
        if (barraRenderer != null)
            barraRenderer.gameObject.SetActive(false);
    }

    public Vector2Int UnirseAFila(CerebroCliente clienteNuevo)
    {
        filaDeClientes.Add(clienteNuevo);
        int posicion = filaDeClientes.Count - 1; 
        return CalcularCelda(posicion);
    }

    public void AtenderSiguiente()
    {
        if (filaDeClientes.Count > 0)
        {
            CerebroCliente clienteQueSeVa = filaDeClientes[0];
            filaDeClientes.RemoveAt(0);

            int totalCobrado = clienteQueSeVa.CalcularTotalCarrito();
            
            if (valoresJugador != null)
            {
                valoresJugador.Plata += totalCobrado;
                Debug.Log($"<color=yellow><b>[Caja]</b></color> Pagó ${totalCobrado}. Saldo actual: ${valoresJugador.Plata}");
            }

            clienteQueSeVa.SerAtendidoYRetirarse();

            for (int i = 0; i < filaDeClientes.Count; i++)
            {
                Vector2Int nuevaCelda = CalcularCelda(i);
                filaDeClientes[i].AvanzarEnFila(nuevaCelda);
            }
        }
    }

    private Vector2Int CalcularCelda(int lugarEnFila)
    {
        return celdaParaPagar + (direccionFila * lugarEnFila);
    }
}