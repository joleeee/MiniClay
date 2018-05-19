using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MoonSharp.Interpreter;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Diagnostics;
using UnityEditor;

public class LuaManager : MonoBehaviour {
    public Texture2D texture;
    public Texture2D white;
    public Material whiteMat;
    public Material mat;
    public Texture2D icons;
    public Texture2D cropped;

    RenderTexture rend;

    string text = "Hello There!";
    string displayText;
    int mouseIndex = 0;

    string luaScript;

    public static int WIDTH = 240;
    public static int HEIGHT = 135;

    public Icons selectedIcon = Icons.Play;

    public static List<LuaSprite> spriteQueue;

    public static Vector2 scale;

    Script script;
    List<Color> colors = new List<Color>();

    // Use this for initialization
    void Start() {
        luaScript = File.ReadAllText(Application.streamingAssetsPath + "/script.lua");
        LoadScript(luaScript);
        //Application.targetFrameRate = 60;
        text = File.ReadAllText(Application.streamingAssetsPath + "/script.lua");

        /*EditorUtility.DisplayDialog("Load script", "Please load a lua script.", "OK");

        string path = EditorUtility.OpenFilePanel("Load script", Application.streamingAssetsPath, "lua");
        if (path.Length != 0)
        {
            text = File.ReadAllText(path);
        }*/

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
    
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(selectedIcon == Icons.Play)
            {
                selectedIcon = Icons.Code;
            }
            else
            {
                selectedIcon = Icons.Play;
                LoadScript(text);
            }
        }else if (Input.GetKeyDown(KeyCode.F1))
        {
            selectedIcon = Icons.Code;
        }else if (Input.GetKeyDown(KeyCode.F2))
        {
            selectedIcon = Icons.Draw;
        }else if (Input.GetKeyDown(KeyCode.F3))
        {
            selectedIcon = Icons.Level;
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            selectedIcon = Icons.Sound;
        }

        switch (selectedIcon)
        {
            case Icons.Play:
                script.Call(script.Globals["update"]);
                break;
            case Icons.Code:
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    mouseIndex--;
                if (Input.GetKeyDown(KeyCode.RightAlt))
                    mouseIndex++;
                Mathf.Clamp(mouseIndex, 0, Mathf.Infinity);

                

                foreach (char c in Input.inputString)
                {
                    if (c == '\b') // has backspace/delete been pressed?
                    {
                        if (text.Length != 0)
                        {
                            //text = text.Substring(0, mouseIndex - 1) + text.Substring(mouseIndex - 1, text.Length - mouseIndex - 1);
                            text = text.Remove(mouseIndex - 1, 1);
                            mouseIndex--;
                            if (mouseIndex < 0)
                                mouseIndex = 0;
                        }
                    }
                    else
                    {
                        text = text.Insert(mouseIndex, c.ToString());
                        mouseIndex++;
                    }
                }
                if (Input.GetMouseButton(0))
                {
                    int mouseLine = Mathf.FloorToInt((HEIGHT * scale.y - Input.mousePosition.y - 7) / 6);
                    int mouseCol = Mathf.RoundToInt(Input.mousePosition.x * scale.x / 4);
                    print("col" + mouseCol + "lin" + mouseLine);

                    int index = 0;
                    string[] lines = Regex.Split(text, "\r\n|\r|\n");
                    for (int y = 0; y < lines.Length; y++)
                    {
                        string line = lines[y];
                        for (int x = 0; x < line.Length; x++)
                        {
                            if (mouseLine == y && mouseCol == x)
                            {
                                print("IT WORKS!");
                                mouseIndex = index;
                            }
                            index++;
                        }
                        index += 2;
                    }
                }
                

                displayText = text.Insert(mouseIndex, "_");
                break;
            case Icons.Draw:
                break;
            case Icons.Level:
                break;
            case Icons.Sound:
                break;
            default:
                break;
        }
        

        
        

        scale = new Vector2(Screen.width / WIDTH, Screen.height / HEIGHT);
        
        
    }

    void LoadScript(string scriptString)
    {
        scriptString = scriptString.ToLower();
        spriteQueue = new List<LuaSprite>();
        script = new Script();
        script.DoString(scriptString);
        script.Globals["spr"] = (Action<int, int>)Spr; //Action if a void
        script.Globals["time"] = (Func<float>)DeltaTime; //Func if returning a value
        //script.Globals["restart"] = (Action)LoadScript;
        script.Globals["btn"] = (Func<int, bool>)GetKey;
        script.Globals["print"] = (Action<string>)print;
    }

    float DeltaTime()
    {
        return Time.deltaTime;
    }

    void Spr(int x, int y)
    {
        spriteQueue.Add(new LuaSprite(texture, x, y));
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

    void OnGUI() {
        if (!Event.current.type.Equals(EventType.Repaint))
            return;


        #region UI
        Draw(white, new Rect(0, 0, WIDTH, HEIGHT), new Rect(0, 0, white.width, white.height), colors[0]);

        if (selectedIcon != Icons.Play)
        {
            Rect sourceRect = new Rect(0, 7 * 8, icons.width / 5, icons.height / 2);
            Rect screenRect = new Rect(0, 0, icons.width / 5 / 8, icons.height / 2 / 8);
            screenRect.x *= scale.x;
            screenRect.y *= scale.y;

            for (int i = 0; i < 5; i++)
            {
                if (HEIGHT * scale.y - Input.mousePosition.y < sourceRect.y && Input.mousePosition.x > sourceRect.x && Input.mousePosition.x < sourceRect.x + sourceRect.width && Input.GetMouseButton(0) || (int)selectedIcon == i)
                {
                    selectedIcon = (Icons)i;
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
        }
        #endregion

        switch (selectedIcon)
        {
            case Icons.Play:
                foreach (var sprite in spriteQueue)
                {
                    sprite.Draw();
                }
                spriteQueue.Clear();
                break;
            case Icons.Code:
                DrawText(displayText);
                break;
            case Icons.Draw:
                break;
            case Icons.Level:
                break;
            case Icons.Sound:
                break;
            default:
                break;
        }
    }

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
    
}

public enum Icons
{
    Play,
    Code,
    Draw,
    Level,
    Sound
}