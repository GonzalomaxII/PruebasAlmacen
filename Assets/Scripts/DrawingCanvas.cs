using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawingCanvas : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public RawImage displayImage;  
    public Color brushColor = Color.black;

    [Header("Grosor de Pincel y Goma")]
    [Tooltip("Grosor del lápiz. Proba valores entre 10 y 25 para un trazo más visible")]
    public int brushSize = 12;      // Aumentado de 5 a 12 por defecto
    public int eraserSize = 30;    
    public int tamanoMinimo = 2;
    public int tamanoMaximo = 40;

    private Texture2D _canvas;
    private RectTransform _rectTransform;

    public int Width  => _canvas.width;
    public int Height => _canvas.height;

    void Awake()
    {
        _rectTransform = displayImage.GetComponent<RectTransform>();
    }

    void Update()
    {
        // Opcional: regular grosor con la rueda del mouse mientras dibujas
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            brushSize = Mathf.Clamp(brushSize + (int)scroll * 2, tamanoMinimo, tamanoMaximo);
        }
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
        int currentSize;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            currentColor = brushColor;
            currentSize = brushSize;   
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            currentColor = Color.clear;
            currentSize = eraserSize;  
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

        PaintCircle(px, py, currentColor, currentSize);
        _canvas.Apply();
    }

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