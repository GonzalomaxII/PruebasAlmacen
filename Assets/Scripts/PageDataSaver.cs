using System.IO;
using UnityEngine;

[System.Serializable]
public class PageData
{
    public string text = "";
    // La imagen se guarda como PNG separado
}

public static class PageDataSaver
{
    // Carpeta base: algo como /Android/data/com.tuJuego/files/notebook/
    private static string BasePath => 
        Path.Combine(Application.persistentDataPath, "notebook_v2");

    public static void SavePage(int pageIndex, Texture2D drawing, string text)
    {
        Directory.CreateDirectory(BasePath);

        // Guardar imagen
        byte[] pngBytes = drawing.EncodeToPNG();
        File.WriteAllBytes(ImagePath(pageIndex), pngBytes);

        // Guardar texto
        PageData data = new PageData { text = text };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(TextPath(pageIndex), json);
    }

    public static (Texture2D drawing, string text) LoadPage(int pageIndex, int width, int height)
    {
        Texture2D tex = new Texture2D(width, height);

        if (File.Exists(ImagePath(pageIndex)))
        {
            byte[] bytes = File.ReadAllBytes(ImagePath(pageIndex));
            tex.LoadImage(bytes);
        }
        else
        {
            // Página en blanco
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) 
                pixels[i] = Color.clear; //un color que equivale a transparente. Ahora, Unity creará un "vidrio" sobre tu cuaderno. Las líneas negras se pintarán sobre ese vidrio, dejando ver el bloc de notas que está debajo.
            tex.SetPixels(pixels);
            tex.Apply();
        }

        string text = "";
        if (File.Exists(TextPath(pageIndex)))
        {
            string json = File.ReadAllText(TextPath(pageIndex));
            text = JsonUtility.FromJson<PageData>(json).text;
        }

        return (tex, text);
    }

    private static string ImagePath(int i) => Path.Combine(BasePath, $"page_{i}.png");
    private static string TextPath(int i)  => Path.Combine(BasePath, $"page_{i}.json");
}
