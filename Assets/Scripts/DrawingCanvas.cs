using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawingCanvas : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public RawImage displayImage;  // RawImage en el UI que muestra el dibujo
    public Color brushColor = Color.black;
    public int brushSize = 5;

    private Texture2D _canvas;
    private RectTransform _rectTransform;

    public int Width  => _canvas.width;
    public int Height => _canvas.height;

    void Awake()
    {
        _rectTransform = displayImage.GetComponent<RectTransform>();
    }

    // Llamado por NotebookManager al cargar una página
    public void LoadTexture(Texture2D tex)
    {
        _canvas = tex;
        displayImage.texture = _canvas;
    }

    // Devuelve la textura actual para guardarla
    public Texture2D GetTexture() => _canvas;

    public void OnPointerDown(PointerEventData eventData) => Draw(eventData);
    public void OnDrag(PointerEventData eventData) => Draw(eventData);

    private void Draw(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform, eventData.position,
            eventData.pressEventCamera, out Vector2 localPoint)) return;

        // Convertir posición local a coordenadas de textura
        Rect rect = _rectTransform.rect;
        float u = (localPoint.x - rect.x) / rect.width;
        float v = (localPoint.y - rect.y) / rect.height;

        int px = Mathf.RoundToInt(u * _canvas.width);
        int py = Mathf.RoundToInt(v * _canvas.height);

        PaintCircle(px, py);
        _canvas.Apply();
    }

    private void PaintCircle(int cx, int cy)
    {
        for (int x = -brushSize; x <= brushSize; x++)
        for (int y = -brushSize; y <= brushSize; y++)
        {
            if (x * x + y * y <= brushSize * brushSize)
                _canvas.SetPixel(cx + x, cy + y, brushColor);
        }
    }
}
