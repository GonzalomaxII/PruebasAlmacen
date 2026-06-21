using UnityEngine;
using UnityEngine.Tilemaps;

public class MapaCliente : MonoBehaviour
{
    public static MapaCliente Instance;

    public Tilemap paredesTilemap;
    public Tilemap pisoTilemap;

    [Header("Tiles de referencia")]
    public TileBase[] tilesDeEstante;
    public TileBase[] tilesDeHeladera;
    public TileBase[] tilesDeFreezer;
    public TileBase[] tilesDeCajero;

    public int[,] mapa { get; private set; }
    public int ancho { get; private set; }
    public int alto { get; private set; }
    public Vector2Int origen { get; private set; } // esquina inferior-izquierda en coords de Tilemap

    void Awake()
    {
        Instance = this;
        GenerarMapa();
    }

    void GenerarMapa()
    {
        BoundsInt bounds = pisoTilemap.cellBounds;
        ancho = bounds.size.x;
        alto = bounds.size.y;
        origen = new Vector2Int(bounds.xMin, bounds.yMin);

        mapa = new int[ancho, alto];

        for (int x = 0; x < ancho; x++)
        {
            for (int y = 0; y < alto; y++)
            {
                Vector3Int celda = new Vector3Int(origen.x + x, origen.y + y, 0);
                mapa[x, y] = ClasificarCelda(celda);
            }
        }

        Debug.Log($"Mapa generado: {ancho}x{alto}, origen {origen}");
    }

    int ClasificarCelda(Vector3Int celda)
    {
        bool esPared = paredesTilemap.HasTile(celda);
        TileBase tilePiso = pisoTilemap.GetTile(celda);

        if (esPared || tilePiso == null) return 0;
        if (Contiene(tilesDeEstante, tilePiso)) return 2;
        if (Contiene(tilesDeHeladera, tilePiso)) return 3;
        if (Contiene(tilesDeFreezer, tilePiso)) return 4;
        if (Contiene(tilesDeCajero, tilePiso)) return 5;
        return 1;
    }

    bool Contiene(TileBase[] lista, TileBase tile)
    {
        foreach (var t in lista) if (t == tile) return true;
        return false;
    }

    public bool EsCaminable(int x, int y)
    {
        if (x < 0 || x >= ancho || y < 0 || y >= alto) return false;
        return mapa[x, y] == 1;
    }

    public Vector3 CeldaAMundo(int x, int y)
    {
        Vector3Int celdaTilemap = new Vector3Int(origen.x + x, origen.y + y, 0);
        return pisoTilemap.GetCellCenterWorld(celdaTilemap);
    }

    public Vector2Int MundoACelda(Vector3 posicionMundo)
    {
        Vector3Int celdaTilemap = pisoTilemap.WorldToCell(posicionMundo);
        return new Vector2Int(celdaTilemap.x - origen.x, celdaTilemap.y - origen.y);
    }
}
