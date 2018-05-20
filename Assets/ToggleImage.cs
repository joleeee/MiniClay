using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleImage : MonoBehaviour {

    Sprite primary;
    public Sprite secondary;
    Image image;
    public Icon icon;

    // Use this for initialization
    void Start()
    {
        image = GetComponent<Image>();
        primary = image.sprite;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(LuaManager.Main.icon == icon)
        {
            image.sprite = secondary;
        }
        else
        {
            image.sprite = primary;
        }
    }

    bool sprite = true;
    public void ToggeButton()
    {
        LuaManager.Main.SetIcon(icon);
    }
}
