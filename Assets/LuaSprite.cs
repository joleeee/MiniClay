using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaSprite/* : MonoBehaviour*/ {

    public Texture2D texture;
    public int scale;
    public float x;
    public float y;
    public Rect screenRect;

    public LuaSprite(Texture2D texture, float x, float y, int scale = 1)
    {
        this.texture = texture;
        this.x = x;
        this.y = y;
        this.scale = scale;
    }

	public void Draw(){
        screenRect = new Rect(x, y, texture.width, texture.height);
        Graphics.DrawTexture(screenRect, texture);
	}
	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
