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

    string realText;

    Color drawColor;

    Text TextBox;
    Text TextOverlay;
    InputField InputField;
    DrawableImage spriteImageScript;
    RawImage spriteImage;
    DrawableImage spriteMapScript;
    GameObject UIGO;
    GameObject DrawGO;
    GameObject CodeGO;
    GameObject AudioGO;
    GameObject LevelGO;
    RawImage ColorSelectorSprite;
    RawImage AudioSprite;
    DrawableImage AudioSpriteScript;
    Text InputText;
    public static LuaManager Main;
    string luaScript;

    public static int WIDTH = 128;
    public static int HEIGHT = 72;

    public Icon icon = Icon.Draw;

    public static List<LuaSprite> spriteQueue;

    public static Vector2 scale;

    public List<GameObject> Sprites = new List<GameObject>();

    Script script;
    List<Color> colors = new List<Color>();
    List<String> rawColors = new List<string>();

    // Use this for initialization
    void Start() {
        Main = this;
        TextBox = GameObject.Find("ScriptText").GetComponent<Text>();
        InputField = GameObject.Find("ScriptField").GetComponent<InputField>();
        InputField.text = File.ReadAllText(Application.streamingAssetsPath + "/script.lua");
        TextOverlay = GameObject.Find("TextOverlay").GetComponent<Text>();
        ColorSelectorSprite = GameObject.Find("ColorSelector").GetComponent<RawImage>();
        UIGO = GameObject.Find("UICat");
        DrawGO = GameObject.Find("DrawCat");
        CodeGO = GameObject.Find("CodeCat");
        AudioGO = GameObject.Find("AudioCat");
        LevelGO = GameObject.Find("LevelCat");
        InputText = GameObject.Find("ScriptText").GetComponent<Text>();
        spriteImage = GameObject.Find("SpriteImage").GetComponent<RawImage>();
        spriteImageScript = GameObject.Find("SpriteImage").GetComponent<DrawableImage>();
        spriteMapScript = GameObject.Find("AllSprites").GetComponent<DrawableImage>();
        spriteMapScript.Init();
        AudioSpriteScript = GameObject.Find("AudioImage").GetComponent<DrawableImage>();
        AudioSprite = GameObject.Find("AudioImage").GetComponent<RawImage>();

        ReadAllColors();
        ColorBlock c = InputField.colors;
        c.normalColor = colors[0];
        c.highlightedColor = colors[0];
        InputField.selectionColor = colors[3];
        InputField.colors = c;
        TextBox.GetComponent<Text>().color = colors[3];
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        LoadScript(InputField.text);

        realText = InputField.text;
        Color[] cs = new Color[6];
        Texture2D tex = new Texture2D(3, 2);
        for (int i = 0; i < 6; i++)
        {
            cs[i] = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            //print(cs[i].r);
        }
        tex.SetPixels(cs);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        ColorSelectorSprite.GetComponent<DrawableImage>().Init();
        ColorSelectorSprite.texture = tex;
    }

    string ColorTextReplace(string text, string textToReplace, string color)
    {
        text = text.Replace(textToReplace, "<color=#" + color + "ff>" + textToReplace + "</color>");
        //print("<color=#" + color + "ff>" + textToReplace + "</color>");
        return text;
    }
    
	void Update () {
        InputField.text = InputField.text.ToUpper();
        string text = InputText.text;
        text = ColorTextReplace(text, "=", rawColors[4]);
        text = ColorTextReplace(text, "FUNCTION ", rawColors[5]);
        text = ColorTextReplace(text, "BTN ", rawColors[5]);
        //text = ColorTextReplace(text, "FUNCTION ", rawColors[5]);
        text = ColorTextReplace(text, "(", rawColors[6]);
        text = ColorTextReplace(text, ")", rawColors[6]);
        TextOverlay.text = text;

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
        bool Code = false;
        bool Draw = false;
        bool ui = true;
        bool Level = false;
        bool Audio = false;
        bool clearGOs = true;
        switch (icon)
        {
            case Icon.Play:
                script.Call(script.Globals["update"]);
                clearGOs = false;
                ui = false;
                break;
            case Icon.Code:
                Code = true;
                break;
            case Icon.Draw:
                Draw = true;

                #region draw
                //SpriteMap\\
                if (Input.GetMouseButton(0))
                {
                    Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
                        spriteMapScript.Draw(pixel, drawColor);
                    }
                    Color[] c;
                    spriteImageScript.SetColors(c = ((Texture2D)spriteMapScript.Image.texture).GetPixels(xTile * 8, yTile * 8, 8, 8));
                    //SpriteImage\\

                    //ColorSelector\\
                    Vector2 csPosition = ColorSelectorSprite.rectTransform.anchoredPosition;
                    Vector2 csDimentions = ColorSelectorSprite.rectTransform.sizeDelta;
                    csPosition.x -= csDimentions.x / 2;
                    csPosition.y -= csDimentions.y / 2;

                    if (mouse.x > csPosition.x && mouse.x < csPosition.x + csDimentions.x && mouse.y > csPosition.y && mouse.y < csPosition.y + csDimentions.y)
                    {
                        Vector2 mouseRel = mouse - csPosition;
                        Vector2 pixel = mouseRel / (ColorSelectorSprite.rectTransform.sizeDelta / ColorSelectorSprite.GetComponent<DrawableImage>().pixelDimentions);
                        drawColor = (((Texture2D) ColorSelectorSprite.texture).GetPixel((int)pixel.x, (int)pixel.y));
                        //print(pixel);
                    }
                    

                    //ColorSelector\\
                }
                #endregion

                break;
            case Icon.Level:
                Level = true;
                break;
            case Icon.Sound:
                Audio = true;
                if (Input.GetMouseButton(0))
                {
                    Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 position = AudioSprite.rectTransform.anchoredPosition;
                    Vector2 dimentions = AudioSprite.rectTransform.sizeDelta;
                    
                    position.x -= dimentions.x / 2;
                    position.y -= dimentions.y / 2;

                    if (mouse.x > position.x && mouse.x < position.x + dimentions.x && mouse.y > position.y && mouse.y < position.y + dimentions.y)
                    {
                        Vector2 mouseRel = mouse - position;
                        Vector2 pixel = mouseRel / (AudioSprite.rectTransform.sizeDelta / AudioSpriteScript.pixelDimentions);
                        //print(pixel);

                        for (int i = 0; i < AudioSpriteScript.pixelDimentions.y; i++)
                        {
                            AudioSpriteScript.Draw(new Vector2Int((int)pixel.x, i), colors[0]);
                        }

                        for (int i = 0; i < pixel.y; i++)
                        {
                            AudioSpriteScript.Draw(new Vector2Int((int)pixel.x, i), colors[4]);
                        }

                        /*drawColor = (((Texture2D)AudioSprite.texture).GetPixel((int)pixel.x, (int)pixel.y));
                        print(pixel);*/
                    }
                }
                break;
            default:
                break;
        }
        if (clearGOs)
            ClearSprites();
        DrawGO.SetActive(Draw);
        UIGO.SetActive(ui);
        CodeGO.SetActive(Code);
        //InputField.gameObject.SetActive(Code);
        LevelGO.SetActive(Level);
        AudioGO.SetActive(Audio);
        //Debug.LogError(1 / Time.deltaTime);

        
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
                /*foreach (var sprite in spriteQueue)
                {
                    sprite.Draw();
                }
                spriteQueue.Clear();*/
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
    void ClearSprites()
    {
        List<GameObject> newSprites = new List<GameObject>(Sprites);
        for (int i = 0; i < Sprites.Count; i++)
        {
            Destroy(Sprites[i]);
            Destroy(Sprites[i].gameObject);
            newSprites.Remove(Sprites[i]);
        }
        Sprites = newSprites;
    }

    void LoadScript(string scriptString)
    {
        spriteQueue = new List<LuaSprite>();
        script = new Script();
        script.DoString(scriptString.ToLower());
        script.Globals["spr"] = (Func<int, int, int, LuaSprite>)Spr; //Action if a void
        script.Globals["time"] = (Func<float>)DeltaTime; //Func if returning a value
        //script.Globals["restart"] = (Action)LoadScript;
        script.Globals["btn"] = (Func<int, bool>)GetKey;
        script.Globals["print"] = (Action<string>)print;
        UserData.RegisterType<LuaSprite>();
        UserData.RegisterType<Debug>();
        ClearSprites();
        script.Call(script.Globals["start"]);
    }
    #region luaFunctions
    float DeltaTime()
    {
        return Time.deltaTime;
    }

    LuaSprite Spr(int id, int x, int y)
    {
        print("spr called");
        Texture2D tex = new Texture2D(8, 8);
        tex.SetPixels(((Texture2D)spriteMapScript.Image.texture).GetPixels((id % 8) * 8, Mathf.FloorToInt(id/8)*8, 8, 8));
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        GameObject go = new GameObject();
        go.AddComponent(typeof(LuaSprite));
        LuaSprite spr = go.GetComponent<LuaSprite>();
        spr.Init(tex, new Vector2(x, y));
        Sprites.Add(go);
        return spr;
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
                var values = line.Split(';');
                float r, g, b;
                int num = (int)long.Parse(values[1], NumberStyles.HexNumber);
                r = (num & 0xFF0000) >> 16;
                g = (num & 0xFF00) >> 8;
                b = num & 0xFF;

                //print(r + "." + g + "." + b);
                colors.Add(new Color(r / 255f, g / 255f, b / 255f));
                rawColors.Add(values[1]);
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