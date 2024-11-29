using UnityEngine;

public class TextureScale
{
    public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
    {
        Texture2D newTex = new Texture2D(newWidth, newHeight, tex.format, true);
        float ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
        float ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
        for (int y = 0; y < newHeight; y++)
        {
            int yy = Mathf.FloorToInt(y * ratioY);
            int yy1 = yy + 1;
            float yLerp = y * ratioY - yy;

            for (int x = 0; x < newWidth; x++)
            {
                int xx = Mathf.FloorToInt(x * ratioX);
                int xx1 = xx + 1;
                float xLerp = x * ratioX - xx;

                Color bl = tex.GetPixel(xx, yy);
                Color br = tex.GetPixel(xx1, yy);
                Color tl = tex.GetPixel(xx, yy1);
                Color tr = tex.GetPixel(xx1, yy1);

                Color pixel = Color.Lerp(Color.Lerp(bl, br, xLerp), Color.Lerp(tl, tr, xLerp), yLerp);
                newTex.SetPixel(x, y, pixel);
            }
        }
        newTex.Apply();
        tex.Reinitialize(newWidth, newHeight);
        tex.SetPixels(newTex.GetPixels());
        tex.Apply();
    }
}
