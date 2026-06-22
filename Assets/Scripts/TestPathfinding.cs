using UnityEngine;
using System.Collections.Generic;

public class TestPathfinding : MonoBehaviour
{
    public Vector2Int posicionInicio;
    public Vector2Int posicionDestino;

    private List<Vector2Int> camino;

    void Start()
    {
        camino = Pathfinding.Instance.FindPath(posicionInicio, posicionDestino);

        if (camino != null)
        {
            Debug.Log($"Camino encontrado con {camino.Count} pasos");
        }
        else
        {
            Debug.Log("No se encontró camino");
        }
    }

    void OnDrawGizmos()
    {
        if (camino == null || MapaCliente.Instance == null) return;

        Gizmos.color = Color.magenta;
        foreach (Vector2Int celda in camino)
        {
            Vector3 pos = MapaCliente.Instance.CeldaAMundo(celda.x, celda.y);
            Gizmos.DrawCube(pos, Vector3.one * 0.5f);
        }
    }
}