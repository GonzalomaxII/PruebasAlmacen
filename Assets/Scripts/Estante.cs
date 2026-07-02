using UnityEngine;

public class Estante : MonoBehaviour
{
    [Header("Configuración del Mueble")]
    [Tooltip("Define qué tipo de mueble es este objeto: 2=Estante Naranja, 3=Heladera, 4=Freezer")]
    public int tipoMueble; 

    [Header("Configuración del Producto")]
    public Producto producto; // Guarda un único tipo de producto
    public int stockMaximo = 10;
    public int stockActual = 10;

    [Header("Posición de Interacción")]
    public Vector2Int celdaParaComprar; 

    // Variable oculta para recordar el último producto válido y poder restaurarlo si hay un error
    private Producto ultimoProductoValido;

    public bool TieneStock() => stockActual > 0;

    private void OnValidate()
    {
        // Si el script se recarga y ya tiene un producto asignado válido de antes, lo recordamos
        if (ultimoProductoValido == null && producto != null && producto.tipoEstante == tipoMueble)
        {
            ultimoProductoValido = producto;
        }

        if (producto != null)
        {
            // Si el tipo del producto NO coincide con el tipo de este mueble
            if (producto.tipoEstante != tipoMueble)
            {
                Debug.LogError("Asignación incorrecta de producto. Tipo de estante no coincide");
                
                // RESTAURACIÓN: En vez de volverlo null, le devolvemos lo que tenía antes
                producto = ultimoProductoValido; 
            }
            else
            {
                // Si el producto nuevo sí es válido, actualizamos nuestro registro
                ultimoProductoValido = producto;
            }
        }
        else
        {
            // Si vaciaste el estante a propósito en el inspector (arrastrando "None")
            ultimoProductoValido = null;
        }
    }

    // VALIDACIÓN 2: Para el Gameplay (Si el jugador cambia productos jugando)
    public bool IntentarAsignarProducto(Producto nuevoProducto)
    {
        // Permitir vaciar el estante voluntariamente
        if (nuevoProducto == null)
        {
            producto = null;
            stockActual = 0;
            ultimoProductoValido = null;
            return true;
        }

        // Validar si el tipo de producto coincide con el tipo de mueble
        if (nuevoProducto.tipoEstante == tipoMueble)
        {
            producto = nuevoProducto;
            stockActual = stockMaximo; 
            ultimoProductoValido = nuevoProducto;
            return true; // Cambio exitoso
        }

        // Si NO coincide, tira el print de error y retorna false
        Debug.LogError($"<color=orange><b>[ACCIÓN RECHAZADA EN JUEGO]</b></color> No se pudo colocar '{nuevoProducto.nombreProducto}' " +
                       $"en el mueble <b>{gameObject.name}</b>. Los tipos no coinciden (Mueble: {tipoMueble} | Producto: {nuevoProducto.tipoEstante}). " +
                       $"El estante conserva su producto actual: {(producto != null ? producto.nombreProducto : "Ninguno")}.");
        
        return false; 
    }
  
    public int RetirarStock(int cantidadPedida) // El cliente llama a esto para llevarse mercadería
    {
        int cantidadDada = Mathf.Min(cantidadPedida, stockActual);
        stockActual -= cantidadDada;
        return cantidadDada;
    }
}