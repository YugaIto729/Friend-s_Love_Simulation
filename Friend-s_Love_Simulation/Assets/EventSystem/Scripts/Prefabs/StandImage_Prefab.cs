using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StandImage_Prefab : MonoBehaviour
{
    public Vector2 defaultSize = new Vector2(1, 1);
    [SerializeField] private GameObject[] image_gameObjects;
    public int[] index = new int[7];
    public SpriteRenderer[] image_r = new SpriteRenderer[7];

    private Animator anime;

    [SerializeField] private Sprite[] sprites_si;
    [SerializeField] private Sprite[] sprites_ey;
    [SerializeField] private Sprite[] sprites_br;
    [SerializeField] private Sprite[] sprites_mo;
    [SerializeField] private Sprite[] sprites_ef;

    private void Awake()
    {
        for (int i = 0; i < image_gameObjects.Length; i++)
        {
            image_r[i] = image_gameObjects[i].GetComponent<SpriteRenderer>();
        }

        anime = GetComponent<Animator>();
    }

    private int Chack_Index(int index, Sprite[] sprites)
    {
        if (index > sprites.Length)
        {
            return 0;
        }
        return index;
    }

    public void Set_Image(int stand, int brows, int eyes, int mouth)
    {
        var spc = new StandImage_Prefab_Config() {
            index_standImage = stand, index_brows = brows, index_eyes = eyes, index_mouth = mouth
        };

        Set_Image(spc);
    }

    public void Set_Image(int stand, int brows, int eyes, int mouth, Vector2 poss, Vector2 size)
    {
        transform.localPosition = new Vector3(poss.x, poss.y, transform.localPosition.z);
        transform.localScale = new Vector3(size.x, size.y, 1) ;
        Set_Image(stand, brows, eyes, mouth);
    }

    public void Set_Image(StandImage_Prefab_Config config, Vector2 poss, Vector2 size)
    {
        transform.localPosition = new Vector3(poss.x, poss.y, transform.localPosition.z);
        transform.localScale = new Vector3(size.x, size.y, 1);
        Set_Image(config);
    }

    public void Set_Image(StandImage_Prefab_Config config)
    {
        if (config.index_effect_b != -1)
        {
            image_r[0].sprite = sprites_ef[Chack_Index(config.index_effect_b, sprites_ef)];
        }
        else
        {
            image_r[0].sprite = null;
        }

        if (config.index_standImage != -1)
        {
            image_r[1].sprite = sprites_si[Chack_Index(config.index_standImage, sprites_si)];
        }
        else
        {
            image_r[1].sprite = null;
        }

        if (config.index_effect_m != -1)
        {
            image_r[2].sprite = sprites_ef[Chack_Index(config.index_effect_m, sprites_ef)];
        }
        else
        {
            image_r[2].sprite = null;
        }

        if (config.index_mouth != -1)
        {
            image_r[3].sprite = sprites_mo[Chack_Index(config.index_mouth, sprites_mo)];
        }
        else
        {
            image_r[3].sprite = null;
        }

        if (config.index_eyes != -1)
        {
            image_r[4].sprite = sprites_ey[Chack_Index(config.index_eyes, sprites_ey)];
        }
        else
        {
            image_r[4].sprite = null;
        }

        if (config.index_brows != -1)
        {
            image_r[5].sprite = sprites_br[Chack_Index(config.index_brows, sprites_br)];
        }
        else
        {
            image_r[5].sprite = null;
        }

        if (config.index_effect_f != -1)
        {
            image_r[6].sprite = sprites_ef[Chack_Index(config.index_effect_f, sprites_ef)];
        }
        else
        {
            image_r[6].sprite = null;
        }
    }
    
    public void Set_Color(Color color)
    {
        foreach(SpriteRenderer im in image_r)
        {
            im.color = color;
        }
    }

    public void Set_Animation(int state)
    {
        anime.SetInteger("animeState", state);
    }
}

public enum SI_IndexType
{
    STANDIMAGE, EYES, BROWS, MOUTH, EFFECT_BACK, EFFECT_MIDDLE, EFFECT_FRONT
}


public class StandImage_Prefab_Config
{
    public int index_standImage = 0;
    public int index_eyes = 0;
    public int index_brows = 0;
    public int index_mouth = 0;
    public int index_effect_b = -1;
    public int index_effect_m = -1;
    public int index_effect_f = -1;
}
