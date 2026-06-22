using UnityEngine;

public class ControladorPrueba : MonoBehaviour
{
    [Header("Arrastra aquí a tu NPC")]
    public ClienteNPC npcAControlar;

    [Header("Coordenada a la que quieres que vaya")]
    public Vector2Int celdaDestino = new Vector2Int(13, 9);

    void Start()
    {
        // Usamos un pequeño retraso de 1 segundo antes de mandarlo.
        // Esto asegura que tus scripts de Mapa y Pathfinding tengan tiempo de inicializarse primero.
        Invoke("MoverNPC", 1f);
    }

    void MoverNPC()
    {
        if (npcAControlar != null)
        {
            Debug.Log($"Prueba: Mandando al NPC a la celda {celdaDestino}");
            npcAControlar.IrA(celdaDestino);
        }
        else
        {
            Debug.LogWarning("¡Falta asignar el NPC en el inspector!");
        }
    }
}
