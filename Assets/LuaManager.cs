using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MoonSharp.Interpreter;
using System;

public class LuaManager : MonoBehaviour {
    public Texture2D texture;

    public static List<LuaSprite> spriteQueue;

    Script script;

	// Use this for initialization
	void Start () {
        spriteQueue = new List<LuaSprite>();
        script = new Script();
        script.DoString(File.ReadAllText(Application.streamingAssetsPath + "/script.lua"));
        script.Globals["spr"] = (Action<int, int>)Spr; //Action if a void
        script.Globals["time"] = (Func<float>)DeltaTime; //Func if returning a value
        script.Globals["restart"] = (Action)LoadScript;
        script.Globals["btn"] = (Func<int,bool>)GetKey;
        script.Globals["print"] = (Action<string>)print;
    }

	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.R))
        {
            LoadScript();
        }
        
        script.Call(script.Globals["Update"], Time.deltaTime);
	}

    void LoadScript()
    {
        script = new Script();
        script.DoString(File.ReadAllText(Application.dataPath + "/script.lua"));
        script.Globals["spr"] = (Action<int, int>)Spr; //Action if a void
        script.Globals["time"] = (Func<float>)DeltaTime;
        script.Globals["btn"] = (Func<int, bool>)GetKey;
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

	void OnGUI(){
        if (!Event.current.type.Equals(EventType.Repaint))
            return;
        foreach (var sprite in spriteQueue)
        {
            sprite.Draw();
        }
        spriteQueue.Clear();
    }
}
