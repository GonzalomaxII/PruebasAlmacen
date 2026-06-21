using UnityEngine;
using UnityEngine.Tilemaps;
using System.Text;

public class TilemapDetector : MonoBehaviour
{
    public Tilemap paredesTilemap;
    public Tilemap pisoTilemap;

    [Header("Arrastrá acá los tile assets correspondientes")]
    public TileBase[] tilesDeEstante;   // naranja
    public TileBase[] tilesDeHeladera;  // azul/celeste oscuro
    public TileBase[] tilesDeFreezer;   // celeste claro
    public TileBase[] tilesDeCajero;    // rojo
    // Todo lo demás (marrón, verde, blanco) en pisoTilemap se considera piso normal (1)

    [ContextMenu("Generar Matriz en Console")]
    public void GenerarMatriz()
    {
        BoundsInt bounds = pisoTilemap.cellBounds;
        int ancho = bounds.size.x;
        int alto = bounds.size.y;

        int[,] matriz = new int[alto, ancho];

        for (int y = 0; y < alto; y++)
        {
            for (int x = 0; x < ancho; x++)
            {
                Vector3Int celda = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);

                bool esPared = paredesTilemap.HasTile(celda);
                TileBase tilePiso = pisoTilemap.GetTile(celda);

                int valor;

                if (esPared)
                {
                    valor = 0; // Pared
                }
                else if (tilePiso == null)
                {
                    valor = 0; // Nada pintado, no transitable
                }
                else if (Contiene(tilesDeEstante, tilePiso))
                {
                    valor = 2; // Estante
                }
                else if (Contiene(tilesDeHeladera, tilePiso))
                {
                    valor = 3; // Heladera
                }
                else if (Contiene(tilesDeFreezer, tilePiso))
                {
                    valor = 4; // Freezer
                }
                else if (Contiene(tilesDeCajero, tilePiso))
                {
                    valor = 5; // Cajero
                }
                else
                {
                    valor = 1; // Piso normal (marrón, verde o blanco)
                }

                matriz[alto - 1 - y, x] = valor;
            }
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"// Tamaño detectado: {ancho} columnas x {alto} filas");
        sb.AppendLine($"// Origen del Tilemap: ({bounds.xMin}, {bounds.yMin})");
        sb.AppendLine("public static int[,] mapaCliente = new int[,]");
        sb.AppendLine("{");

        for (int y = 0; y < alto; y++)
        {
            sb.Append("    {");
            for (int x = 0; x < ancho; x++)
            {
                sb.Append(matriz[y, x]);
                if (x < ancho - 1) sb.Append(",");
            }
            sb.AppendLine("},");
        }

        sb.AppendLine("};");

        Debug.Log(sb.ToString());
    }

    bool Contiene(TileBase[] lista, TileBase tile)
    {
        foreach (var t in lista)
        {
            if (t == tile) return true;
        }
        return false;
    }
}
