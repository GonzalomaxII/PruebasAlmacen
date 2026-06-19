using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawingCanvas : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public RawImage displayImage;  // RawImage en el UI que muestra el dibujo
    public Color brushColor = Color.black;
    public int brushSize = 5;
    public int eraserSize = 25;    // NUEVO: Tamaño independiente para la goma

    private Texture2D _canvas;
    private RectTransform _rectTransform;

    public int Width  => _canvas.width;
    public int Height => _canvas.height;

    void Awake()
    {
        _rectTransform = displayImage.GetComponent<RectTransform>();
    }

    public void LoadTexture(Texture2D tex)
    {
        _canvas = tex;
        displayImage.texture = _canvas;
    }

    public Texture2D GetTexture() => _canvas;

    public void OnPointerDown(PointerEventData eventData) => Draw(eventData);
    public void OnDrag(PointerEventData eventData) => Draw(eventData);

    private void Draw(PointerEventData eventData)
    {
        Color currentColor;
        int currentSize; // NUEVO: Variable para saber qué tamaño usar

        // 1. Decidir color y tamaño según el clic
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            currentColor = brushColor;
            currentSize = brushSize;   // Lápiz
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            currentColor = Color.clear;
            currentSize = eraserSize;  // Goma
        }
        else
        {
            return; 
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform, eventData.position,
            eventData.pressEventCamera, out Vector2 localPoint)) return;

        Rect rect = _rectTransform.rect;
        float u = (localPoint.x - rect.x) / rect.width;
        float v = (localPoint.y - rect.y) / rect.height;

        int px = Mathf.RoundToInt(u * _canvas.width);
        int py = Mathf.RoundToInt(v * _canvas.height);

        // 2. Enviamos el color y el tamaño elegidos
        PaintCircle(px, py, currentColor, currentSize);
        _canvas.Apply();
    }

    // 3. Ahora PaintCircle usa "radius" en lugar de brushSize
    private void PaintCircle(int cx, int cy, Color colorToPaint, int radius)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    int targetX = cx + x;
                    int targetY = cy + y;

                    if (targetX >= 0 && targetX < _canvas.width && targetY >= 0 && targetY < _canvas.height)
                        _canvas.SetPixel(targetX, targetY, colorToPaint);
                }
            }
        }
    }
}