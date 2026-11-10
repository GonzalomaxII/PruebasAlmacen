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
    public List<(Producto producto, int cantidad)> carrito { get; private set; }
    public bool esperandoEnCaja { get; private set; }

    private int paciencia = 10;
    
    void OnEnable()
    {
        if (cuerpoNPC == null) cuerpoNPC = GetComponent<ClienteNPC>();

        paciencia = 10;
        esperandoEnCaja = false;

        GenerarListaDeseos();
        StartCoroutine(RutinaDeCompras());
    }

    void GenerarListaDeseos()
    {
        listaDeseos = new List<(Producto, int)>();
        carrito = new List<(Producto, int)>();

        int cant = Random.Range(minimoProductos, maximoProductos + 1);
        List<Producto> disponibles = new List<Producto>(catalogoProductos);

        for (int i = 0; i < cant && disponibles.Count > 0; i++) 
        {
            int idx = Random.Range(0, disponibles.Count);
            listaDeseos.Add((disponibles[idx], Random.Range(1, 5)));
            disponibles.RemoveAt(idx);
        }
    }

    IEnumerator RutinaDeCompras()
    {
        yield return new WaitForSeconds(1f);

        foreach (var deseo in listaDeseos)
        {
            yield return StartCoroutine(BuscarEIrAlEstante(deseo.producto, deseo.cantidad));
            if (paciencia <= 0) break;
        }

        if (paciencia > 0 && carrito.Count > 0)
        {
            yield return StartCoroutine(IrAlCajero());
        }
        else
        {
            yield return StartCoroutine(IrALaSalida());
        }
    }

    IEnumerator IrAlCajero()
    {
        Cajero cajero = FindObjectOfType<Cajero>();

        if (cajero != null)
        {
            Vector2Int miLugar = cajero.UnirseAFila(this);
            cuerpoNPC.IrA(miLugar);
            yield return new WaitWhile(() => cuerpoNPC.estaMoviendose);

            esperandoEnCaja = true;
        }
        else
        {
            Debug.LogError("¡No se encontró el Cajero!");
        }
    }

    public void AvanzarEnFila(Vector2Int nuevaPosicion)
    {
        StartCoroutine(RutinaAvanzar(nuevaPosicion));
    }

    private IEnumerator RutinaAvanzar(Vector2Int nuevaPosicion)
    {
        esperandoEnCaja = false;
        cuerpoNPC.IrA(nuevaPosicion);
        yield return new WaitWhile(() => cuerpoNPC.estaMoviendose);
        esperandoEnCaja = true;
    }

    public void SerAtendidoYRetirarse()
    {
        esperandoEnCaja = false;
        StartCoroutine(IrALaSalida());
    }

    public int CalcularTotalCarrito()
    {
        int suma = 0;
        foreach (var item in carrito)
        {
            suma += item.producto.precio * item.cantidad;
        }
        return suma;
    }

    IEnumerator IrALaSalida()
    {
        PuntoSalida[] salidas = FindObjectsOfType<PuntoSalida>();

        if (salidas.Length > 0)
        {
            int indiceAleatorio = Random.Range(0, salidas.Length);
            PuntoSalida salidaElegida = salidas[indiceAleatorio];

            cuerpoNPC.IrA(salidaElegida.celdaSalida);
            yield return new WaitWhile(() => cuerpoNPC.estaMoviendose);

            gameObject.SetActive(false); 
        }
        else
        {
            Debug.LogError("¡No hay ningún PuntoSalida en la escena!");
        }
    }
    
    IEnumerator BuscarEIrAlEstante(Producto prod, int cantDeseada)
    {
        List<Estante> estantes = new List<Estante>();
        foreach (Estante e in FindObjectsOfType<Estante>())
        {
            if (e.producto != null && e.producto.tipoEstante == prod.tipoEstante)
                estantes.Add(e);
        }

        if (estantes.Count == 0) yield break;

        foreach (Estante estante in estantes)
        {
            Vector2Int celdaDestino = MapaCliente.Instance.MundoACelda(estante.transform.position);
            cuerpoNPC.IrA(celdaDestino);
            yield return new WaitWhile(() => cuerpoNPC.estaMoviendose);

            if (estante.producto == prod && estante.stockActual >= cantDeseada)
            {
                yield return new WaitForSeconds(tiempoAgarrarProducto); 
                int obtenido = estante.RetirarStock(cantDeseada);
                carrito.Add((prod, obtenido));
                break;
            }
            else
            {
                paciencia--;
                yield return new WaitForSeconds(tiempoAgarrarProducto); 
                if (paciencia <= 0) break;
            }
        }
    }
}
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI; // Para controlar la barra arriba del cliente

// public class CerebroCliente : MonoBehaviour
// {
//     [Header("Referencias")]
//     public ClienteNPC cuerpoNPC; 

//     [Header("UI Barra de Atención")]
//     [Tooltip("El objeto padre de la barra arriba del cliente (para prenderlo/apagarlo)")]
//     public GameObject objetoBarraUI;
//     [Tooltip("La imagen de relleno (Image Type: Filled)")]
//     public Image rellenoBarraUI;

//     [Header("Configuración de Compras")]
//     public List<Producto> catalogoProductos;
//     public int minimoProductos = 1;
//     public int maximoProductos = 5;
//     public float tiempoAgarrarProducto = 2f;

//     private List<(Producto producto, int cantidad)> listaDeseos;
    
//     // Guarda únicamente los productos que el cliente sí encontró y agarró del estante
//     public List<(Producto producto, int cantidad)> carrito { get; private set; }
//     public bool esperandoEnCaja { get; private set; }

//     private int paciencia = 10;
    
//     void OnEnable()
//     {
//         if (cuerpoNPC == null) cuerpoNPC = GetComponent<ClienteNPC>();

//         paciencia = 10;
//         esperandoEnCaja = false;
//         ActualizarBarraAtencion(0f, false);

//         GenerarListaDeseos();
//         StartCoroutine(RutinaDeCompras());
//     }

//     void GenerarListaDeseos()
//     {
//         listaDeseos = new List<(Producto, int)>();
//         carrito = new List<(Producto, int)>(); // Vaciamos el carrito al entrar

//         int cant = Random.Range(minimoProductos, maximoProductos + 1);
//         List<Producto> disponibles = new List<Producto>(catalogoProductos);

//         for (int i = 0; i < cant && disponibles.Count > 0; i++) 
//         {
//             int idx = Random.Range(0, disponibles.Count);
//             listaDeseos.Add((disponibles[idx], Random.Range(1, 5)));
//             disponibles.RemoveAt(idx);
//         }
//     }

//     IEnumerator RutinaDeCompras()
//     {
//         yield return new WaitForSeconds(1f);

//         foreach (var deseo in listaDeseos)
//         {
//             yield return StartCoroutine(BuscarEIrAlEstante(deseo.producto, deseo.cantidad));
//             if (paciencia <= 0) break;
//         }

//         // Si no perdió la paciencia y agarró al menos 1 producto, va a la fila de la caja
//         if (paciencia > 0 && carrito.Count > 0)
//         {
//             yield return StartCoroutine(IrAlCajero());
//         }
//         else
//         {
//             Debug.Log("Cliente se va del local sin pasar por caja...");
//             yield return StartCoroutine(IrALaSalida());
//         }
//     }

//     IEnumerator IrAlCajero()
//     {
//         Cajero cajero = FindObjectOfType<Cajero>();

//         if (cajero != null)
//         {
//             Vector2Int miLugar = cajero.UnirseAFila(this);

//             cuerpoNPC.IrA(miLugar);
//             yield return new WaitWhile(() => cuerpoNPC.estaMoviendose);

//             esperandoEnCaja = true; // Ya llegó y está quieto en su lugar de la fila
//             Debug.Log($"<color=cyan><b>[Caja]</b></color> Cliente posicionado en la fila, esperando su turno.");
//         }
//         else
//         {
//             Debug.LogError("¡No se encontró el Cajero!");
//         }
//     }

//     // Llamado por Cajero.AtenderSiguiente() para los clientes que estaban atrás en la fila
//     public void AvanzarEnFila(Vector2Int nuevaPosicion)
//     {
//         StartCoroutine(RutinaAvanzar(nuevaPosicion));
//     }

//     private IEnumerator RutinaAvanzar(Vector2Int nuevaPosicion)
//     {
//         esperandoEnCaja = false;
//         cuerpoNPC.IrA(nuevaPosicion);
//         yield return new WaitWhile(() => cuerpoNPC.estaMoviendose);
//         esperandoEnCaja = true; // Ya dio el paso al frente y está listo para ser atendido
//     }

//     // Llamado por Cajero.AtenderSiguiente() para el cliente que acaba de pagar
//     public void SerAtendidoYRetirarse()
//     {
//         esperandoEnCaja = false;
//         ActualizarBarraAtencion(0f, false);
//         StartCoroutine(IrALaSalida()); // Reutilizamos tu lógica de irse al PuntoSalida y desactivarse
//     }

//     public int CalcularTotalCarrito()
//     {
//         int suma = 0;
//         foreach (var item in carrito)
//         {
//             suma += item.producto.precio * item.cantidad;
//         }
//         return suma;
//     }

//     public void ActualizarBarraAtencion(float progreso, bool visible)
//     {
//         if (objetoBarraUI != null) objetoBarraUI.SetActive(visible);
//         if (rellenoBarraUI != null) rellenoBarraUI.fillAmount = Mathf.Clamp01(progreso);
//     }

//     IEnumerator IrALaSalida()
//     {
//         PuntoSalida[] salidas = FindObjectsOfType<PuntoSalida>();

//         if (salidas.Length > 0)
//         {
//             int indiceAleatorio = Random.Range(0, salidas.Length);
//             PuntoSalida salidaElegida = salidas[indiceAleatorio];

//             cuerpoNPC.IrA(salidaElegida.celdaSalida);

//             yield return new WaitWhile(() => cuerpoNPC.estaMoviendose);

//             Debug.Log("El cliente se alejó lo suficiente y desaparece.");
//             gameObject.SetActive(false); 
//         }
//         else
//         {
//             Debug.LogError("¡No hay ningún PuntoSalida en la escena!");
//         }
//     }
    
//     IEnumerator BuscarEIrAlEstante(Producto prod, int cantDeseada)
//     {
//         List<Estante> estantes = new List<Estante>();
//         foreach (Estante e in FindObjectsOfType<Estante>())
//         {
//             if (e.producto != null && e.producto.tipoEstante == prod.tipoEstante)
//                 estantes.Add(e);
//         }

//         if (estantes.Count == 0) yield break;

//         foreach (Estante estante in estantes)
//         {
//             Vector2Int celdaDestino = MapaCliente.Instance.MundoACelda(estante.transform.position);
            
//             cuerpoNPC.IrA(celdaDestino);
//             yield return new WaitWhile(() => cuerpoNPC.estaMoviendose);

//             if (estante.producto == prod && estante.stockActual >= cantDeseada)
//             {
//                 yield return new WaitForSeconds(tiempoAgarrarProducto); 
                
//                 int obtenido = estante.RetirarStock(cantDeseada);
                
//                 // Lo anotamos en el carrito real para cobrarle en la caja
//                 carrito.Add((prod, obtenido));
//                 break;
//             }
//             else
//             {
//                 paciencia--;
//                 yield return new WaitForSeconds(tiempoAgarrarProducto); 
//                 if (paciencia <= 0)
//                 {
//                     break; 
//                 }
//             }
//         }
//     }
// }