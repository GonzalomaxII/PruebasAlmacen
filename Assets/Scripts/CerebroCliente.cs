using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CerebroCliente : MonoBehaviour
{
    [Header("Referencias")]
    public ClienteNPC cuerpoNPC; 

    [Header("Configuración de Compras")]
    public List<Producto> catalogoProductos;
    public int minimoProductos = 1;
    public int maximoProductos = 5;
    public float tiempoAgarrarProducto = 2f;
    private List<(Producto producto, int cantidad)> listaDeseos;
    private int paciencia = 10;

    void Start()
    {
        // Si olvidaste asignar el cuerpo, lo busca automáticamente
        if (cuerpoNPC == null) cuerpoNPC = GetComponent<ClienteNPC>();

        GenerarListaDeseos();
        StartCoroutine(RutinaDeCompras());
    }

    void GenerarListaDeseos()
    {
        listaDeseos = new List<(Producto, int)>();
        int cant = Random.Range(minimoProductos, maximoProductos + 1); // Calcula cuántos tipos de productos va a buscar (ej: 3)
        List<Producto> disponibles = new List<Producto>(catalogoProductos);

        for (int i = 0; i < cant && disponibles.Count > 0; i++) 
        {
            // AGREGA EL DESEO: Elige el producto al azar, y le asigna cuántas UNIDADES quiere.
            int idx = Random.Range(0, disponibles.Count);
            listaDeseos.Add((disponibles[idx], Random.Range(1, 5)));
            disponibles.RemoveAt(idx);
        }
    }

    IEnumerator RutinaDeCompras()
    {
        // Un pequeño delay inicial para asegurar que el pathfinding cargó
        yield return new WaitForSeconds(1f);

        foreach (var deseo in listaDeseos)
        {
            yield return StartCoroutine(BuscarEIrAlEstante(deseo.producto, deseo.cantidad));
            if (paciencia <= 0) break; // Si se enojó, corta la compra
        }

        if (paciencia > 0)
        {
            Debug.Log("Terminó de comprar, buscando cajero...");
            yield return StartCoroutine(IrAlCajero());
        }
        else
        {
            Debug.Log("Cliente enojado, se va del local (y se lleva lo que agarró)...");
            yield return StartCoroutine(IrALaSalida());
        }
    }
    IEnumerator IrAlCajero()
    {
        Cajero cajero = FindObjectOfType<Cajero>();

        if (cajero != null)
        {
            // En vez de ir a la celda fija, le pide al cajero que le asigne su lugar en la fila
            Vector2Int miLugar = cajero.UnirseAFila(this);

            cuerpoNPC.IrA(miLugar);
            yield return new WaitWhile(() => cuerpoNPC.estaMoviendose);

            Debug.Log($"<color=cyan><b>[Caja]</b></color> Cliente posicionado en la fila, esperando su turno.");
        }
        else
        {
            Debug.LogError("¡No se encontró el Cajero!");
        }
    }

    IEnumerator IrALaSalida()
    {
        // CAMBIO 1: Usamos FindObjectsOfType (con 's' al final) para obtener un Array con todos los puntos
        PuntoSalida[] salidas = FindObjectsOfType<PuntoSalida>();

        // Verificamos que haya al menos 1 punto de salida en la escena
        if (salidas.Length > 0)
        {
            int indiceAleatorio = Random.Range(0, salidas.Length);
            PuntoSalida salidaElegida = salidas[indiceAleatorio];

            cuerpoNPC.IrA(salidaElegida.celdaSalida); // Le dice al cuerpo que camine hacia la celda de la salida que tocó en el sorteo

            yield return new WaitWhile(() => cuerpoNPC.estaMoviendose);

            Debug.Log("El cliente se alejó lo suficiente y desaparece.");
            gameObject.SetActive(false); 
        }
        else
        {
            Debug.LogError("¡No hay ningún PuntoSalida en la escena!");
        }
    }

    // El Cajero va a llamar a esta función cuando el que estaba adelante termine de pagar
    public void AvanzarEnFila(Vector2Int nuevaPosicion)
    {
        // Al darle una nueva orden, tu Pathfinding automáticamente hará que dé un paso al frente
        cuerpoNPC.IrA(nuevaPosicion);
    }
    
    IEnumerator BuscarEIrAlEstante(Producto prod, int cantDeseada)
    {
        // 1. Busca los estantes 
        List<Estante> estantes = new List<Estante>();
        foreach (Estante e in FindObjectsOfType<Estante>())
        {
            if (e.producto != null && e.producto.tipoEstante == prod.tipoEstante)
                estantes.Add(e);
        }

        if (estantes.Count == 0) yield break;

        // 2. Intenta ir a uno de los estantes
        foreach (Estante estante in estantes)
        {
            Vector2Int celdaDestino = MapaCliente.Instance.MundoACelda(estante.transform.position);
            
            // Le da la orden al cuerpo
            cuerpoNPC.IrA(celdaDestino);

            // LA MAGIA: El cerebro espera en pausa hasta que el cuerpo termine de caminar
            yield return new WaitWhile(() => cuerpoNPC.estaMoviendose);

            // ¿Es el producto correcto Y tiene la cantidad exacta (o más) de lo que pide?
            if (estante.producto == prod && estante.stockActual >= cantDeseada)
            {
                Debug.Log($"<color=green><b>[Interactuando]</b></color> El cliente encontró {prod.nombreProducto}. Agarrando unidades...");
    
                yield return new WaitForSeconds(tiempoAgarrarProducto); 
                
                int obtenido = estante.RetirarStock(cantDeseada);
                Debug.Log($"Compró {obtenido} de {prod.nombreProducto} con éxito.");
                
                break; // Rompe el bucle de estantes y pasa al siguiente producto de su lista de deseos
            }
            else
            {
                // Si el estante tiene otro producto (búsqueda a ciegas) o no tiene la cantidad exacta
                paciencia--;
                yield return new WaitForSeconds(tiempoAgarrarProducto); 
                if (estante.producto == prod)
                {
                    Debug.Log($"El estante tenía {prod.nombreProducto} pero NO la cantidad exacta. " +
                              $"Quería {cantDeseada} y hay {estante.stockActual}. Paciencia: {paciencia}");
                }
                else
                {
                    Debug.Log($"Revisó {estante.gameObject.name} pero tenía otra cosa ({estante.producto.nombreProducto}). Paciencia: {paciencia}");
                }
                if (paciencia <= 0)
                {
                    Debug.Log($"<color=red><b>[Enojado]</b></color> El cliente perdió la paciencia en medio de la búsqueda.");
                    break; 
                }
            }
        }
    }
}
