using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LuaSprite : MonoBehaviour {

    public Texture2D texture;
    public float x { get { return transform.position.x; } set { transform.position = new Vector2(value, transform.position.y); } }
    public float y { get { return transform.position.y; } set { transform.position = new Vector2(transform.position.x, value); } }
    public int scale;
    SpriteRenderer spriteRend;

    public void Init(Texture2D texture, Vector2 position)
    {
        this.texture = texture;
        this.x = position.x;
        this.y = position.y;
        this.spriteRend = GetComponent<SpriteRenderer>();
        Sprite sprite = Sprite.Create(texture, new Rect(x, y, texture.width, texture.height), Vector2.one / 2, 1);
        spriteRend.sprite = sprite;
    }

    void Start()
    {
        
    }

    /*public LuaSprite(Texture2D texture, float x, float y, int scale = 1)
    {
        this.texture = texture;
        this.x = x;
        this.y = y;
        this.scale = scale;
    }*/

	/*public void Draw(){
        screenRect = new Rect(x * LuaManager.scale.x, y * LuaManager.scale.y, texture.width * LuaManager.scale.x, texture.height * LuaManager.scale.y);
        Graphics.DrawTexture(screenRect, texture);
	}*/
	

	// Use this for initialization
	
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
