using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawableImage : MonoBehaviour {

    public int widthPixels;
    public int heightPixels;
    public Vector2 pixelDimentions { get { return new Vector2(widthPixels, heightPixels); } }

    public bool colorStart = false;

    public RawImage Image;

    void Start()
    {
        Image = GetComponent<RawImage>();
        Texture2D t = new Texture2D(widthPixels, heightPixels, TextureFormat.RGBA32, false);
        Color[] c = new Color[widthPixels * heightPixels];
        Color currentColor = new Color(Random.value, Random.value, Random.value);
        for (int i = 0; i < widthPixels * heightPixels; i++)
        {
            if (colorStart)
            {
                if(i % (widthPixels * 7) == 0)
                    currentColor = new Color(Random.value, Random.value, Random.value);
                c[i] = currentColor;
            }
            else
            {
                c[i] = Color.white;
            }
        }
        t.SetPixels(c);
        t.filterMode = FilterMode.Point;
        t.Apply(false);
        Image.texture = t;
        Image.color = Color.white;
    }

    public void Draw(Vector2Int pixel, Color color)
    {
        Texture2D tex = (Texture2D)Image.texture;
        tex.SetPixel(pixel.x, pixel.y, color);
        tex.filterMode = FilterMode.Point;
        tex.Apply(true);
        Image.texture = tex;
    }

    public void Draw(Vector2 pixel, Color color)
    {
        Draw(new Vector2Int((int)pixel.x, (int)pixel.y), color);
    }

    public void SetColors(Color[] colors)
    {
        Texture2D t = ((Texture2D)Image.texture);
        t.SetPixels(colors);
        t.Apply();
        Image.texture = t;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
