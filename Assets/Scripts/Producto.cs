using UnityEngine;

[CreateAssetMenu(fileName = "NuevoProducto", menuName = "Supermercado/Producto")]
public class Producto : ScriptableObject
{
    public string nombreProducto;
    
    [Tooltip("2 = Estante Naranja, 3 = Heladera, 4 = Freezer")]
    public int tipoEstante; 
}
