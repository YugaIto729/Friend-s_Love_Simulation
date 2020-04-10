using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Image_Event_Prefab : MonoBehaviour
{
    private RectTransform rectTransform;
    public int num_image;
    public Image image;


    public void Set_Image(int num, Vector2 poss, Vector2 size,Sprite sprite)
    {
        var image = GetComponent<Image>();
        image.sprite = sprite;
        num_image = num;
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(sprite.bounds.size.x * size.x * 100, sprite.bounds.size.y * size.y * 100);
        rectTransform.localPosition = poss;
    }
}
