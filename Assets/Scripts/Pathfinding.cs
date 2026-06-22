using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance;

    private Cell[,] cells;
    private int ancho;
    private int alto;

    void Awake()
    {
        Instance = this;
    }

    void GenerarGridDesdeMapa()
    {
        var mapaCliente = MapaCliente.Instance;
        ancho = mapaCliente.ancho;
        alto = mapaCliente.alto;

        cells = new Cell[ancho, alto];

        for (int x = 0; x < ancho; x++)
        {
            for (int y = 0; y < alto; y++)
            {
                cells[x, y] = new Cell
                {
                    position = new Vector2Int(x, y),
                    isWall = !mapaCliente.EsCaminable(x, y)
                };
            }
        }
    }

    public List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int endPos)
    {
        // Regeneramos la grilla de costos en cada búsqueda para no arrastrar
        // valores de búsquedas anteriores (varios clientes pueden pedir paths distintos)
        GenerarGridDesdeMapa();

        // Si el destino no es caminable (estante, heladera, etc.),
        // busca automáticamente la celda adyacente válida
        if (!MapaCliente.Instance.EsCaminable(endPos.x, endPos.y))
        {
            Vector2Int celdaAdyacente = ObtenerCeldaAdyacenteValida(endPos);
            
            if (celdaAdyacente == Vector2Int.one * -1) // indicador de "no encontrada"
            {
                Debug.LogWarning($"No hay celda adyacente válida al destino {endPos}");
                return null;
            }

            Debug.Log($"Destino no caminable {endPos}, usando celda adyacente {celdaAdyacente}");
            endPos = celdaAdyacente;
        }

        List<Vector2Int> cellsToSearch = new List<Vector2Int> { startPos };
        HashSet<Vector2Int> searchedCells = new HashSet<Vector2Int>();
        List<Vector2Int> finalPath = new List<Vector2Int>();

        cells[startPos.x, startPos.y].gCost = 0;
        cells[startPos.x, startPos.y].hCost = GetDistance(startPos, endPos);
        cells[startPos.x, startPos.y].fCost = cells[startPos.x, startPos.y].hCost;

        while (cellsToSearch.Count > 0)
        {
            Vector2Int cellToSearch = cellsToSearch[0];

            foreach (Vector2Int pos in cellsToSearch)
            {
                Cell c = cells[pos.x, pos.y];
                Cell actual = cells[cellToSearch.x, cellToSearch.y];

                if (c.fCost < actual.fCost ||
                    (c.fCost == actual.fCost && c.hCost < actual.hCost))
                {
                    cellToSearch = pos;
                }
            }

            cellsToSearch.Remove(cellToSearch);
            searchedCells.Add(cellToSearch);

            if (cellToSearch == endPos)
            {
                Cell pathCell = cells[endPos.x, endPos.y];

                while (pathCell.position != startPos)
                {
                    finalPath.Add(pathCell.position);
                    pathCell = cells[pathCell.connection.x, pathCell.connection.y];
                }

                finalPath.Add(startPos);
                finalPath.Reverse(); // queda ordenado de inicio a fin
                return finalPath;
            }

            SearchCellNeighbors(cellToSearch, endPos, cellsToSearch, searchedCells);
        }

        Debug.Log("Path not found");
        return null;
    }
    private Vector2Int ObtenerCeldaAdyacenteValida(Vector2Int celdaNoTransitable)
    {
        // Chequea las 4 direcciones adyacentes (arriba, abajo, izquierda, derecha)
        Vector2Int[] direcciones = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // arriba
            new Vector2Int(0, -1),  // abajo
            new Vector2Int(1, 0),   // derecha
            new Vector2Int(-1, 0)   // izquierda
        };

        foreach (Vector2Int dir in direcciones)
        {
            Vector2Int adyacente = celdaNoTransitable + dir;

            // Verifica que esté dentro del mapa y sea caminable
            if (adyacente.x >= 0 && adyacente.x < ancho &&
                adyacente.y >= 0 && adyacente.y < alto &&
                MapaCliente.Instance.EsCaminable(adyacente.x, adyacente.y))
            {
                return adyacente;
            }
        }

        // Si no encuentra ninguna adyacente válida, devuelve un indicador especial
        Debug.LogWarning($"No hay celda adyacente válida a {celdaNoTransitable}");
        return Vector2Int.one * -1;
    }

    private void SearchCellNeighbors(Vector2Int cellPos, Vector2Int endPos,
        List<Vector2Int> cellsToSearch, HashSet<Vector2Int> searchedCells)
    {
        for (int x = cellPos.x - 1; x <= cellPos.x + 1; x++)
        {
            for (int y = cellPos.y - 1; y <= cellPos.y + 1; y++)
            {
                if (x == cellPos.x && y == cellPos.y) continue; // se salta a sí misma
                if (x < 0 || x >= ancho || y < 0 || y >= alto) continue; // fuera del mapa

                Vector2Int neighborPos = new Vector2Int(x, y);

                if (searchedCells.Contains(neighborPos)) continue;
                if (cells[x, y].isWall) continue;

                int gCostToNeighbor = cells[cellPos.x, cellPos.y].gCost + GetDistance(cellPos, neighborPos);

                if (gCostToNeighbor < cells[x, y].gCost || !cellsToSearch.Contains(neighborPos))
                {
                    Cell neighbourNode = cells[x, y];

                    neighbourNode.connection = cellPos;
                    neighbourNode.gCost = gCostToNeighbor;
                    neighbourNode.hCost = GetDistance(neighborPos, endPos);
                    neighbourNode.fCost = neighbourNode.gCost + neighbourNode.hCost;

                    if (!cellsToSearch.Contains(neighborPos))
                    {
                        cellsToSearch.Add(neighborPos);
                    }
                }
            }
        }
    }

    private int GetDistance(Vector2Int a, Vector2Int b)
    {
        int distX = Mathf.Abs(a.x - b.x);
        int distY = Mathf.Abs(a.y - b.y);

        // Distancia diagonal: 14 por paso diagonal, 10 por paso recto (proporción ~√2)
        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }
}

public class Cell
{
    public Vector2Int position;
    public bool isWall;
    public int gCost;
    public int hCost;
    public int fCost;
    public Vector2Int connection;
}
