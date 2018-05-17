using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour {

    public Texture texture;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnRenderObject () {
        Graphics.DrawTexture(new Rect(0, 0, 8, 8), texture);
	}
}
