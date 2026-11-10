using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Estante : MonoBehaviour, interacuar
{
    [Header("Configuración del Mueble")]
    [Tooltip("2 = Estante, 3 = Heladera, 4 = Freezer")]
    public int tipoMueble = 2; 

    [Header("Configuración del Producto")]
    public Producto producto;
    public int stockMaximo = 10;
    public int stockActual = 10;

    [Header("Catálogo de Referencia")]
    [Tooltip("Arrastrá los ScriptableObjects de los productos para identificar la caja que lleva el jugador")]
    public List<Producto> catalogoProductos;

    [Header("Posición de Interacción NPCs")]
    public Vector2Int celdaParaComprar; 

    [Header("UI - Feedback en Pantalla")]
    public IniciarPc ScriptMenu;
    public GameObject PopUp;
    public TextMeshProUGUI Agarrado;
    [Tooltip("Opcional: Arrastrá el botón de cerrar/aceptar del PopUp para vincularlo solo")]
    public Button botonCerrarPopUp;

    public Valores jugador;

    private Producto ultimoProductoValido;

    public bool TieneStock() => stockActual > 0;

    private void OnValidate()
    {
        if (ultimoProductoValido == null && producto != null && producto.tipoEstante == tipoMueble)
        {
            ultimoProductoValido = producto;
        }

        if (producto != null)
        {
            if (producto.tipoEstante != tipoMueble)
            {
                Debug.LogError($"[ERROR DE ASIGNACIÓN] En {gameObject.name}: '{producto.nombreProducto}' no pertenece a este mueble.");
                producto = ultimoProductoValido; 
            }
            else
            {
                ultimoProductoValido = producto;
            }
        }
        else
        {
            ultimoProductoValido = null;
        }
    }

    public bool puedeinteract()
    {
        if (ScriptMenu == null) return true;
        return !ScriptMenu.MenuAbierto;
    }

    public void interact()
    {
        if (!puedeinteract()) return;

        if (jugador == null)
            jugador = FindObjectOfType<Valores>();

        // 1. Validar si el jugador no tiene ninguna caja en mano
        if (!jugador.tieneCaja && jugador.unidadesCaja == 0)
        {
            if (producto != null)
            {
                MostrarMensaje($"Mueble: {ObtenerNombreMueble(tipoMueble)}\n" +
                               $"Producto asignado: {producto.nombreProducto}\n" +
                               $"Necesitás una caja de este producto para reponer.");
            }
            else
            {
                MostrarMensaje($"Este {ObtenerNombreMueble(tipoMueble).ToLower()} está vacío.\n" +
                               $"Traé una caja adecuada para asignarle mercadería.");
            }
            return;
        }

        // 2. Si el estante está vacío de fábrica, intentar asignarle el producto de la caja
        if (producto == null)
        {
            Producto prodAsignar = BuscarProductoPorNombre(jugador.tipoCaja);

            if (prodAsignar == null)
            {
                MostrarMensaje("Error: No se reconoce el producto que llevás en la caja.");
                return;
            }

            // ERROR: El tipo del producto no corresponde a este mueble
            if (prodAsignar.tipoEstante != tipoMueble)
            {
                string muebleActual = ObtenerNombreMueble(tipoMueble);
                string muebleRequerido = ObtenerNombreMueble(prodAsignar.tipoEstante);

                MostrarMensaje($"<color=red>¡TIPO INCORRECTO!</color>\n\n" +
                               $"No podés colocar '{prodAsignar.nombreProducto}' en este/a {muebleActual}.\n" +
                               $"Este producto requiere: <b>{muebleRequerido}</b>.");
                return;
            }

            // Asignación válida
            producto = prodAsignar;
            ultimoProductoValido = prodAsignar;
            stockActual = 0; // Comienza listo para recibir la carga
        }

        // 3. ERROR: El estante ya tenía un producto asignado y la caja es de otro distinto
        if (!producto.nombreProducto.Equals(jugador.tipoCaja, System.StringComparison.OrdinalIgnoreCase))
        {
            MostrarMensaje($"<color=red>¡PRODUCTO INCORRECTO!</color>\n\n" +
                           $"Este mueble es exclusivo para: <b>{producto.nombreProducto}</b>.\n" +
                           $"Estás intentando colocar una caja de: {jugador.tipoCaja}.");
            return;
        }

        // 4. Validar si el estante ya no admite más unidades
        int espacioLibre = stockMaximo - stockActual;
        if (espacioLibre <= 0)
        {
            MostrarMensaje($"El estante de {producto.nombreProducto} ya está completamente lleno.\n" +
                           $"Guardá la caja de nuevo en el depósito.");
            return;
        }

        // 5. ÉXITO: Reposición de mercadería
        int aReponer = Mathf.Min(jugador.unidadesCaja, espacioLibre);
        stockActual += aReponer;
        jugador.unidadesCaja -= aReponer;
        jugador.cantidad = jugador.unidadesCaja;

        // La caja se vació por completo
        if (jugador.unidadesCaja == 0)
        {
            jugador.tieneCaja = false;
            jugador.tipoCaja = "";
            jugador.cantidad = 0;

            MostrarMensaje($"<color=green>¡ÉXITO!</color>\n\n" +
                           $"Colocaste {producto.nombreProducto} en el/la {ObtenerNombreMueble(tipoMueble).ToLower()}.\n" +
                           $"La caja quedó vacía y la desechaste.");
        }
        // A la caja aún le quedan elementos
        else
        {
            MostrarMensaje($"<color=green>¡ÉXITO!</color>\n\n" +
                           $"Repusiste {producto.nombreProducto} en el mueble.\n" +
                           $"La caja todavía tiene mercadería adentro.");
        }
    }

    private string ObtenerNombreMueble(int tipo)
    {
        switch (tipo)
        {
            case 2: return "Estante";
            case 3: return "Heladera";
            case 4: return "Freezer";
            default: return "Mueble";
        }
    }

    private Producto BuscarProductoPorNombre(string nombre)
    {
        if (catalogoProductos == null) return null;
        foreach (Producto p in catalogoProductos)
        {
            if (p.nombreProducto.Equals(nombre, System.StringComparison.OrdinalIgnoreCase))
                return p;
        }
        return null;
    }

    private void MostrarMensaje(string mensaje)
    {
        if (ScriptMenu != null) ScriptMenu.MenuAbierto = true;
        if (PopUp != null) PopUp.SetActive(true);
        if (Agarrado != null) Agarrado.text = mensaje;

        // Si asignaste el botón de cerrar, le configuramos el clic automáticamente
        if (botonCerrarPopUp != null)
        {
            botonCerrarPopUp.onClick.RemoveAllListeners();
            botonCerrarPopUp.onClick.AddListener(CerrarAviso);
        }
    }

    public void CerrarAviso()
    {
        if (PopUp != null) PopUp.SetActive(false);
        if (ScriptMenu != null) ScriptMenu.MenuAbierto = false;
    }

    public int RetirarStock(int cantidadPedida)
    {
        int cantidadDada = Mathf.Min(cantidadPedida, stockActual);
        stockActual -= cantidadDada;
        return cantidadDada;
    }
}