using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CerebroCliente : MonoBehaviour
{
    [Header("Referencias")]
    public ClienteNPC cuerpoNPC; // Arrastra tu script ClienteNPC aquí

    [Header("Configuración de Compras")]
    public List<Producto> catalogoProductos;
    public int minimoProductos = 1;
    public int maximoProductos = 5;
    
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
        int cant = Random.Range(minimoProductos, maximoProductos + 1);
        List<Producto> disponibles = new List<Producto>(catalogoProductos);

        for (int i = 0; i < cant && disponibles.Count > 0; i++)
        {
            int idx = Random.Range(0, disponibles.Count);
            listaDeseos.Add((disponibles[idx], Random.Range(1, 4)));
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
            // TODO: Lógica para ir al cajero
        }
        else
        {
            Debug.Log("Cliente enojado, se va del local...");
            // TODO: Lógica para ir a la salida
        }
    }

    IEnumerator BuscarEIrAlEstante(Producto prod, int cantDeseada)
    {
        // 1. Busca los estantes (simplificado por ahora)
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

            // LÓGICA DE STOCK (Ocurre recién cuando el NPC está físicamente en el estante)
            if (estante.producto == prod && estante.TieneStock())
            {
                int obtenido = estante.RetirarStock(cantDeseada);
                Debug.Log($"Agarró {obtenido} de {prod.nombreProducto}");
                break; // Encontró lo que quería, pasa al siguiente producto de la lista
            }
            else
            {
                paciencia--;
                Debug.Log($"No había suficiente {prod.nombreProducto}. Paciencia: {paciencia}");
            }
        }
    }
}
