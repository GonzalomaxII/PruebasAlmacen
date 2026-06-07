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
    public CanvasGroup textCanvasGroup; // NUEVO: Para controlar los clics del texto

    [Header("Configuración")]
    public int totalPages = 10;
    public int canvasWidth = 512;
    public int canvasHeight = 512;

    private int _currentPage = 0;

    void Start()
    {
        notebookPanel.SetActive(false);
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

    // NUEVO: Función para el botón que cambia entre dibujar y escribir
    public void CambiarModo()
    {
        // Alterna entre activado (true) y desactivado (false)
        textCanvasGroup.blocksRaycasts = !textCanvasGroup.blocksRaycasts;
        textInput.interactable = textCanvasGroup.blocksRaycasts;
    }

    private void SaveCurrentPage()
    {
        PageDataSaver.SavePage(_currentPage, drawingCanvas.GetTexture(), textInput.text);
    }

    private void LoadCurrentPage()
    {
        var (tex, text) = PageDataSaver.LoadPage(_currentPage, canvasWidth, canvasHeight);
        drawingCanvas.LoadTexture(tex);
        textInput.text = text;
        pageLabel.text = $"Página {_currentPage + 1} / {totalPages}";
    }
}
