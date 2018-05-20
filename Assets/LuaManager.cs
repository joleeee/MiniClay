using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MoonSharp.Interpreter;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.UI;

public class LuaManager : MonoBehaviour {
    public Texture2D texture;
    public Texture2D white;
    public Material whiteMat;
    public Material mat;
    public Texture2D icons;
    public Texture2D cropped;
    Texture2D sprites;

    int xTile = 0;
    int yTile = 0;

    Text TextBox;
    InputField InputField;
    DrawableImage spriteImageScript;
    RawImage spriteImage;
    DrawableImage spriteMapScript;
    RawImage spriteMap;

    public static LuaManager Main;
    string luaScript;

    public static int WIDTH = 128;
    public static int HEIGHT = 72;

    public Icon icon = Icon.Code;

    public static List<LuaSprite> spriteQueue;

    public static Vector2 scale;

    Script script;
    List<Color> colors = new List<Color>();

    // Use this for initialization
    void Start() {
        Main = this;
        TextBox = GameObject.Find("ScriptText").GetComponent<Text>();
        InputField = GameObject.Find("ScriptField").GetComponent<InputField>();
        InputField.text = File.ReadAllText(Application.streamingAssetsPath + "/script.lua");
        LoadScript(InputField.text);
        //luaScript = File.ReadAllText(Application.streamingAssetsPath + "/script.lua");

        /*List<Color> co = new List<Color>();
        for (int i = 0; i < 64; i++)
        {
            co.Add(Color.cyan);
        }
        spriteImage = GameObject.Find("SpriteImage").GetComponent<Image>();
        spriteImage.sprite.texture.SetPixels(0, 0, 8, 8, co.ToArray());*/
        spriteImage = GameObject.Find("SpriteImage").GetComponent<RawImage>();
        spriteImageScript = GameObject.Find("SpriteImage").GetComponent<DrawableImage>();
        spriteMapScript = GameObject.Find("AllSprites").GetComponent<DrawableImage>();

        ReadAllColors();
        ColorBlock c = InputField.colors;
        c.normalColor = colors[0];
        c.highlightedColor = colors[0];
        InputField.colors = c;
        TextBox.GetComponent<Text>().color = colors[3];
    }
    
	void Update () {
        InputField.text = InputField.text.ToUpper();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(icon == Icon.Play)
            {
                SetIcon(Icon.Code);
            }
            else
            {
                SetIcon(Icon.Play);
            }
        }else if (Input.GetKeyDown(KeyCode.F1))
        {
            icon = Icon.Code;
        }else if (Input.GetKeyDown(KeyCode.F2))
        {
            icon = Icon.Draw;
        }else if (Input.GetKeyDown(KeyCode.F3))
        {
            icon = Icon.Level;
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            icon = Icon.Sound;
        }
        bool toSet = false;
        bool spriteImageActive = false;
        switch (icon)
        {
            case Icon.Play:
                script.Call(script.Globals["update"]);
                toSet = false;
                break;
            case Icon.Code:
                toSet = true;
                break;
            case Icon.Draw:
                spriteImageActive = true;
                
                
                //SpriteMap\\
                if (Input.GetMouseButton(0))
                {
                    Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    #region getSpriteBlock
                    Vector2 sbPosition = spriteMapScript.Image.rectTransform.anchoredPosition;
                    Vector2 sbDimentions = spriteMapScript.Image.rectTransform.sizeDelta;
                    sbPosition.x -= sbDimentions.x / 2;
                    sbPosition.y -= sbDimentions.y / 2;
                    

                    if (mouse.x > sbPosition.x && mouse.x < sbPosition.x + sbDimentions.x && mouse.y > sbPosition.y && mouse.y < sbPosition.y + sbDimentions.y)
                    {
                        Vector2 mouseRel = mouse - sbPosition;
                        print(mouseRel / 8);
                        xTile = Mathf.FloorToInt(mouseRel.x / 8);
                        yTile = Mathf.FloorToInt(mouseRel.y / 8);
                        print(xTile + "." + yTile);
                    }
                    //SpriteMap\\
                    
                    //SpriteImage\\
                    Vector2 siPosition = spriteImage.rectTransform.anchoredPosition;
                    Vector2 siDimentions = spriteImage.rectTransform.sizeDelta;
                    siPosition.x -= siDimentions.x / 2;
                    siPosition.y -= siDimentions.y / 2;

                    if (mouse.x > siPosition.x && mouse.x < siPosition.x + siDimentions.x && mouse.y > siPosition.y && mouse.y < siPosition.y + siDimentions.y)
                    {
                        Vector2 mouseRel = mouse - siPosition;
                        Vector2 pixel = mouseRel / (spriteImage.rectTransform.sizeDelta / spriteImageScript.pixelDimentions);
                        pixel.x += xTile * 8;
                        pixel.y += yTile * 8;
                        spriteMapScript.Draw(pixel, Color.green);
                    }
                    Color[] c;
                    spriteImageScript.SetColors(c = ((Texture2D)spriteMapScript.Image.texture).GetPixels(xTile * 8, yTile * 8, 8, 8));
                }

                #endregion
                break;
            case Icon.Level:
                break;
            case Icon.Sound:
                break;
            default:
                break;
        }

        spriteImageScript.gameObject.SetActive(spriteImageActive);
        InputField.gameObject.SetActive(toSet);

        
        //print(xCord + "." + yCord);
        


        scale = new Vector2(Screen.width / WIDTH, Screen.height / HEIGHT);
        
        
    }

    void OnGUI()
    {
        if (!Event.current.type.Equals(EventType.Repaint))
            return;


        #region UI
        //Draw(white, new Rect(0, 0, WIDTH, HEIGHT), new Rect(0, 0, white.width, white.height), colors[0]);
        

        /*if (icon != Icon.Play)
        {
            Rect sourceRect = new Rect(0, 7 * 8, icons.width / 5, icons.height / 2);
            Rect screenRect = new Rect(0, 0, icons.width / 5 / 8, icons.height / 2 / 8);
            screenRect.x *= scale.x;
            screenRect.y *= scale.y;

            for (int i = 0; i < 5; i++)
            {
                if (HEIGHT * scale.y - Input.mousePosition.y < sourceRect.y && Input.mousePosition.x > sourceRect.x && Input.mousePosition.x < sourceRect.x + sourceRect.width && Input.GetMouseButton(0) || (int)icon == i)
                {
                    icon = (Icon)i;
                    Rect r = new Rect(sourceRect);
                    r.y -= 7 * 8;
                    Draw(icons, screenRect, r, Color.white);
                }
                else
                {
                    Draw(icons, screenRect, sourceRect, Color.white);
                }


                sourceRect.x += 7 * 8;
                screenRect.x += 7;
            }
        }*/
        #endregion

        switch (icon)
        {
            case Icon.Play:
                foreach (var sprite in spriteQueue)
                {
                    sprite.Draw();
                }
                spriteQueue.Clear();
                break;
            case Icon.Code:
                break;
            case Icon.Draw:
                break;
            case Icon.Level:
                break;
            case Icon.Sound:
                break;
            default:
                break;
        }
    }

    void LoadScript(string scriptString)
    {
        spriteQueue = new List<LuaSprite>();
        script = new Script();
        script.DoString(scriptString.ToLower());
        script.Globals["spr"] = (Action<int, int, int>)Spr; //Action if a void
        script.Globals["time"] = (Func<float>)DeltaTime; //Func if returning a value
        //script.Globals["restart"] = (Action)LoadScript;
        script.Globals["btn"] = (Func<int, bool>)GetKey;
        script.Globals["print"] = (Action<string>)print;
    }
    #region luaFunctions
    float DeltaTime()
    {
        return Time.deltaTime;
    }

    void Spr(int id, int x, int y)
    {
        Texture2D tex = new Texture2D(8, 8);
        tex.SetPixels(((Texture2D)spriteMapScript.Image.texture).GetPixels((id % 8) * 8, Mathf.FloorToInt(id/8)*8, 8, 8));
        spriteQueue.Add(new LuaSprite(tex, x, y));
    }

    void Cls()
    {
        Camera.main.clearFlags = CameraClearFlags.Color;
    }

    bool GetKey(int key)
    {
        switch (key)
        {
            case 0:
                return Input.GetKey(KeyCode.UpArrow);
            case 1:
                return Input.GetKey(KeyCode.DownArrow);
            case 2:
                return Input.GetKey(KeyCode.LeftArrow);
            case 3:
                return Input.GetKey(KeyCode.RightArrow);
            default:
                return false;
        }
    }
    #endregion

    #region draw
    public void DrawText(string text)
    {
        if (text == null)
            return;
        text = text.ToLower();
        string[] lines = Regex.Split(text, "\r\n|\r|\n");
        int x = 0;
        int y = 7;
        for (int y2 = 0; y2 < lines.Length; y2++)
        {
            string line = lines[y2];
            for (int x2 = 0; x2 < line.Length; x2++)
            {
                DrawChar(x + x2 * 4, y + y2 * 6, line[x2]);
                //print("Drawing" + line[x2]);
            }
        }
        foreach (string line in lines)
        {
            foreach (char c in line)
            {

            }
        }
    }

    char[] alpha = "abcdefghijklmnopqrstuvwxyz1234567890?!()[]{}.,:;-_+=".ToCharArray();
    public Texture2D alphaTexture;
    public void DrawChar(int x, int y, char c)
    {
        for (int i = 0; i < alpha.Length; i++)
        {
            char letter = alpha[i];
            if (c == letter)
            {
                Draw(alphaTexture, new Rect(x, y, 3, 5), new Rect(i * 3 * 8, 0, 3*8, 5*8), colors[3]);
            }
        }
    }

    public void Draw(Texture2D texture, Rect screenRect, Rect sourceRect, Color color)
    {
        mat.SetColor("_Color", color);
        screenRect.x *= scale.x;
        screenRect.width *= scale.x;
        screenRect.y *= scale.y;
        screenRect.height *= scale.y;
        cropped = new Texture2D((int)sourceRect.width, (int)sourceRect.height, TextureFormat.RGBAFloat, false);
        for (int x = 0; x < sourceRect.width; x++)
        {
            for (int y = 0; y < sourceRect.height; y++)
            {
                cropped.SetPixel(x, y, texture.GetPixel(x + (int)sourceRect.x, y + (int)sourceRect.y));
            }
        }
        cropped.Apply();
        GUI.DrawTexture(screenRect, cropped, ScaleMode.StretchToFill, true, 0, color, 0, 0);
    }
    #endregion

    public void SetIcon(Icon i)
    {
        icon = i;
        if(icon == Icon.Play)
        {
            InputField.DeactivateInputField();
            LoadScript(InputField.text);
        }
    }

    public void ReadAllColors()
    {
        using (var reader = new StreamReader(Application.streamingAssetsPath + "/colors.csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                float r, g, b;
                int num = (int)long.Parse(values[1], NumberStyles.HexNumber);
                r = (num & 0xFF0000) >> 16;
                g = (num & 0xFF00) >> 8;
                b = num & 0xFF;

                colors.Add(new Color(r / 255f, g / 255f, b / 255f));
            }
        }
    }


    
}

public enum Icon
{
    Play,
    Code,
    Draw,
    Level,
    Sound
}