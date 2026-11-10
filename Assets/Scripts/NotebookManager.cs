using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotebookManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject notebookPanel;
    public DrawingCanvas drawingCanvas;
    public TMP_InputField textInput;
    public TMP_Text pageLabel;
    public CanvasGroup textCanvasGroup; 

    [Header("Configuración")]
    public int totalPages = 10;
    public int canvasWidth = 512;
    public int canvasHeight = 512;

    [Header("Modo de Prueba")]
    [Tooltip("Si está activo, no carga datos guardados al iniciar y deja todas las páginas vacías")]
    public bool iniciarSiempreEnBlanco = true;

    private int _currentPage = 0;
    private Texture2D[] _texturasPaginas;
    private string[] _textosPaginas;

    void Start()
    {
        notebookPanel.SetActive(false);

        // Inicializar arreglos de memoria para las páginas de la sesión
        _texturasPaginas = new Texture2D[totalPages];
        _textosPaginas = new string[totalPages];

        for (int i = 0; i < totalPages; i++)
        {
            if (iniciarSiempreEnBlanco)
            {
                _texturasPaginas[i] = CrearTexturaLimpia();
                _textosPaginas[i] = "";
            }
            else
            {
                var (tex, text) = PageDataSaver.LoadPage(i, canvasWidth, canvasHeight);
                _texturasPaginas[i] = tex;
                _textosPaginas[i] = text;
            }
        }
    }

    public void OpenNotebook()
    {
        notebookPanel.SetActive(true);
        LoadCurrentPage();
    }

    public void CloseNotebook()
    {
        SaveCurrentPage();
        notebookPanel.SetActive(false);
    }

    public void NextPage()
    {
        if (_currentPage >= totalPages - 1) return;
        SaveCurrentPage();
        _currentPage++;
        LoadCurrentPage();
    }

    public void PreviousPage()
    {
        if (_currentPage <= 0) return;
        SaveCurrentPage();
        _currentPage--;
        LoadCurrentPage();
    }

    public void CambiarModo()
    {
        textCanvasGroup.blocksRaycasts = !textCanvasGroup.blocksRaycasts;
        textInput.interactable = textCanvasGroup.blocksRaycasts;
    }

    private void SaveCurrentPage()
    {
        _texturasPaginas[_currentPage] = drawingCanvas.GetTexture();
        _textosPaginas[_currentPage] = textInput.text;

        if (!iniciarSiempreEnBlanco)
        {
            PageDataSaver.SavePage(_currentPage, drawingCanvas.GetTexture(), textInput.text);
        }
    }

    private void LoadCurrentPage()
    {
        drawingCanvas.LoadTexture(_texturasPaginas[_currentPage]);
        textInput.text = _textosPaginas[_currentPage];
        pageLabel.text = $"Página {_currentPage + 1} / {totalPages}";
    }

    // Crea un lienzo nuevo transparente listo para dibujar
    private Texture2D CrearTexturaLimpia()
    {
        Texture2D tex = new Texture2D(canvasWidth, canvasHeight, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        Color[] pixelesVacios = new Color[canvasWidth * canvasHeight];
        for (int i = 0; i < pixelesVacios.Length; i++)
        {
            pixelesVacios[i] = Color.clear;
        }

        tex.SetPixels(pixelesVacios);
        tex.Apply();
        return tex;
    }
}