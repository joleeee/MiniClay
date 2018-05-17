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
        script.DoString(File.ReadAllText(Application.dataPath + "/script.lua"));
        script.Globals["spr"] = (Action<int, int>)Spr; //Action if a void
        script.Globals["GetInput"] = (Func<string, float>)GetInput; //Func if returning a value
        script.Globals["time"] = (Func<float>)DeltaTime;
        script.Globals["restart"] = (Action)LoadScript;
        script.Globals["r"] = (Func<bool>)GetR;

    }

	
	// Update is called once per frame
	void Update () {
        spriteQueue.Clear();
        if (loadScript)
        {
            loadScript = false;
            script = new Script();
            script.DoString(File.ReadAllText(Application.dataPath + "/script.lua"));
            script.Globals["spr"] = (Action<int, int>)Spr; //Action if a void
            script.Globals["GetInput"] = (Func<string, float>)GetInput; //Func if returning a value
            script.Globals["time"] = (Func<float>)DeltaTime;
        }
        
        script.Call(script.Globals["Update"], Time.deltaTime);
	}

    bool GetR()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

    bool loadScript = false;
    void LoadScript()
    {
        loadScript = true;
        script = new Script();
        script.DoString(File.ReadAllText(Application.dataPath + "/script.lua"));
        script.Globals["spr"] = (Action<int, int>)Spr; //Action if a void
        script.Globals["GetInput"] = (Func<string, float>)GetInput; //Func if returning a value
        script.Globals["time"] = (Func<float>)DeltaTime;
    }

    float DeltaTime()
    {
        return Time.deltaTime;
    }

    void Spr(int x, int y)
    {
        spriteQueue.Add(new LuaSprite(texture, x, y));
    }

    float GetInput(string axis)
    {
        return Input.GetAxisRaw(axis);
    }

	void OnRenderObject(){
        foreach (var sprite in spriteQueue)
        {
            sprite.Draw();
        }
        
	}
}
