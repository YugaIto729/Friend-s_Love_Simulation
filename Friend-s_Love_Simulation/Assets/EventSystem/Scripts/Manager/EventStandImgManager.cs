using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class EventStandImgManager : MonoBehaviour
{
    public static EventStandImgManager instance;

    public enum SIMG_State
    {
        STANDBY, SET, REMOVE, MOVE, FADE, REVERSE, ROTETO, FOCUS, ANIME, STOPANIME, DEPTH, COLOR
    }
    public SIMG_State SIMG_state = SIMG_State.STANDBY;
    public GameObject CanvasObject;
    public GameObject[] prefab_Sprite;

    private bool onSIMG_Coroutine = false;
    private TalkEventManager TeManager;
    private Dictionary<int, StandImage_Prefab> simageDict = new Dictionary<int, StandImage_Prefab>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        TeManager = TalkEventManager.instance;
    }

    private void Update()
    {
        if (SIMG_state != SIMG_State.STANDBY)
        {
            switch (SIMG_state)
            {
                case SIMG_State.SET:
                    SIMG_ImageSet();
                    break;
                case SIMG_State.MOVE:
                    SIMG_Move_Waiting();
                    break;
                case SIMG_State.REMOVE:
                    SIMG_Remove_Waiting();
                    break;
                case SIMG_State.FADE:
                    SIMG_Fade_Waiting();
                    break;
                case SIMG_State.REVERSE:
                    SIMG_Reverse();
                    break;
                case SIMG_State.ROTETO:
                    SIMG_Rot_Waiting();
                    break;
                case SIMG_State.FOCUS:
                    SIMG_Focus();
                    break;
                case SIMG_State.ANIME:
                    SIMG_Anime();
                    break;
                case SIMG_State.STOPANIME:
                    SIMG_StopAnime();
                    break;
                case SIMG_State.DEPTH:
                    SIMG_Depth();
                    break;
                case SIMG_State.COLOR:
                    SIMG_Color();
                    break;
            }
        }
    }


    /// <summary>
    /// 画像配置
    /// </summary>
    private void SIMG_ImageSet()
    {
        var eo = TeManager.Get_CullentEvent();
        if (prefab_Sprite[eo.image_num] != null)
        {
            GameObject o;

            if (simageDict.ContainsKey(eo.image_num)) //管理番号の画像が存在する場合
            {
                o = simageDict[eo.image_num].gameObject;
            }
            else
            {
                o = Instantiate(prefab_Sprite[eo.image_num]);
            }

            o.transform.parent = CanvasObject.transform;
            //o.SetActive(false);
            var iep = o.GetComponent<StandImage_Prefab>();
            switch (eo.register2)
            {

                case 0:
                    iep.Set_Image(eo.sprite_num[1], eo.sprite_num[3], eo.sprite_num[4], eo.sprite_num[5]);
                    break;
                case 1:
                    {
                        var simg_c = new StandImage_Prefab_Config
                        {
                            index_effect_b = eo.sprite_num[1],
                            index_standImage = eo.sprite_num[2],
                            index_effect_m = eo.sprite_num[3],
                            index_brows = eo.sprite_num[4],
                            index_eyes = eo.sprite_num[5],
                            index_mouth = eo.sprite_num[6],
                            index_effect_f = eo.sprite_num[7]
                        };

                        iep.Set_Image(simg_c);
                    }
                    break;
                case 2:
                    iep.Set_Image(eo.sprite_num[1], eo.sprite_num[3], eo.sprite_num[4], eo.sprite_num[5], eo.image_point, eo.image_scale);
                    break;
                case 3:
                    iep.Set_Image(new StandImage_Prefab_Config
                    {
                        index_effect_b = eo.sprite_num[1],
                        index_standImage = eo.sprite_num[2],
                        index_effect_m = eo.sprite_num[3],
                        index_brows = eo.sprite_num[4],
                        index_eyes = eo.sprite_num[5],
                        index_mouth = eo.sprite_num[6],
                        index_effect_f = eo.sprite_num[7]
                    });
                    break;
            }
            if (!simageDict.ContainsKey(eo.image_num))
                simageDict.Add(eo.image_num, iep);
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        SIMG_state = SIMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    /// <summary>
    /// 画像消去待ち
    /// </summary>
    private void SIMG_Remove_Waiting()
    {
        if (!onSIMG_Coroutine)
        {
            SIMG_Remove();
        }
    }

    /// <summary>
    /// 画像消去
    /// </summary>
    private void SIMG_Remove()
    {
        var eo = TeManager.Get_CullentEvent();

        if (eo.image_num == -1)
        {
            List<StandImage_Prefab> list = new List<StandImage_Prefab>();

            foreach (var ed in simageDict)
            {
                var edo = ed.Value;
                list.Add(edo);
            }

            foreach (var l in list)
            {
                Destroy(l.gameObject);
            }

            simageDict.Clear();
        }

        if (simageDict.ContainsKey(eo.image_num))
        {
            var edo = simageDict[eo.image_num];
            simageDict.Remove(eo.image_num);
            Destroy(edo.gameObject);

        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        SIMG_state = SIMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void SIMG_Move_Waiting()
    {
        if (!onSIMG_Coroutine)
        {
            SIMG_Move();
        }
    }

    /// <summary>
    /// 画像を移動する
    /// </summary>
    private void SIMG_Move()
    {
        var eo = TeManager.Get_CullentEvent();
        if (simageDict.ContainsKey(eo.image_num))
        {
            var imageO = simageDict[eo.image_num];

            switch (eo.register1)
            {
                case 0:
                case 1:
                    {
                        Vector2 start = imageO.transform.localPosition;
                        Vector2 goal = start + eo.image_point;
                        StartCoroutine(C_SIMG_Move(imageO, goal, eo.seconds, eo.register1));
                    }
                    break;
                case 2:
                case 3:
                    {
                        Vector2 start = imageO.transform.localPosition;
                        Vector2 goal = eo.image_point;
                        StartCoroutine(C_SIMG_Move(imageO, goal, eo.seconds, eo.register1));
                    }
                    break;
                default:
                    SIMG_Move_End();
                    break;
            }

            if (eo.register1 == 1 || eo.register1 == 3)
            {
                SIMG_Move_End();
            }
        }
        else
        {
            SIMG_Move_End();
        }

    }

    /// <summary>
    /// 画像を移動し終える
    /// </summary>
    private void SIMG_Move_End()
    {
        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        SIMG_state = SIMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    /// <summary>
    /// 画像移動中
    /// </summary>
    /// <param name="event_Prefab"></param>
    /// <param name="goal"></param>
    /// <param name="time"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    private IEnumerator C_SIMG_Move(StandImage_Prefab event_Prefab, Vector2 goal, float time, int mode)
    {
        onSIMG_Coroutine = true;
        Vector2 start = event_Prefab.transform.localPosition;
        if (time < 0) time = 0;

        for (int i = 0; i <= 100 * time; i++)
        {
            yield return new WaitForSeconds(0.01f);
            event_Prefab.transform.localPosition = Vector2.Lerp(start, goal, i / (time * 100));
            event_Prefab.transform.localPosition = new Vector3(event_Prefab.transform.localPosition.x, event_Prefab.transform.localPosition.y, event_Prefab.transform.localPosition.z);
        }

        if (mode == 0 || mode == 2)
        {
            SIMG_Move_End();
        }
        onSIMG_Coroutine = false;
    }

    private void SIMG_Fade_Waiting()
    {

        if (!onSIMG_Coroutine)
        {
            SIMG_Fade();
        }
    }

    private void SIMG_Fade()
    {
        var eo = TeManager.Get_CullentEvent();
        if (simageDict.ContainsKey(eo.image_num))
        {
            var image = simageDict[eo.image_num];

            switch (eo.register1)
            {
                case 0:
                case 1:
                    {
                        var oc = new List<Color>();
                        var ic = new List<Color>();
                        foreach (var sr in image.image_r)
                        {
                            ic.Add(new Color(sr.color.r, sr.color.g, sr.color.b, 1));
                            oc.Add(new Color(sr.color.r, sr.color.g, sr.color.b, 0));
                            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
                        }

                        StartCoroutine(C_SIMG_FadeIn(image.image_r, oc.ToArray(), ic.ToArray(), eo.seconds, eo.register1));
                    }
                    break;
                case 2:
                case 3:
                    {
                        var oc = new List<Color>();
                        var ic = new List<Color>();
                        foreach (var sr in image.image_r)
                        {
                            ic.Add(new Color(sr.color.r, sr.color.g, sr.color.b, 1));
                            oc.Add(new Color(sr.color.r, sr.color.g, sr.color.b, 0));
                            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
                        }

                        StartCoroutine(C_SIMG_FadeOut(image.image_r, oc.ToArray(), ic.ToArray(), eo.seconds, eo.register1));
                    }
                    break;
                default:
                    SIMG_Fade_End();
                    break;
            }

            if (eo.register1 == 1 || eo.register1 == 3)
            {
                SIMG_Fade_End();
            }
        }
        else
        {
            SIMG_Fade_End();
        }

    }

    private IEnumerator C_SIMG_FadeIn(SpriteRenderer[] image, Color[] oc, Color[] ic, float time, int mode)
    {
        bool dp = false;
        onSIMG_Coroutine = true;
        if (time < 0) { time = 0; dp = true; }

        for (int i = 0; i <= 100 * time; i++)
        {
            yield return new WaitForSeconds(0.01f);
            for (int j=0; j < image.Length; j++)
            {
                image[j].color = Color.Lerp(oc[j], ic[j], (float)i / (time * 100));
            }
        }

        if (dp) {
            for (int j = 0; j < image.Length; j++) {
                image[j].color = ic[j];
            }
        }

        if (mode == 0)
        {
            SIMG_Fade_End();
        }
        onSIMG_Coroutine = false;
    }

    private IEnumerator C_SIMG_FadeOut(SpriteRenderer[] image, Color[] oc, Color[] ic, float time, int mode)
    {
        bool dp = false;
        onSIMG_Coroutine = true;
        if (time < 0) { time = 0; dp = true; }

        for (int i = 0; i <= 100 * time; i++)
        {
            yield return new WaitForSeconds(0.01f);
            for (int j=0; j < image.Length; j++)
            {
                image[j].color = Color.Lerp(ic[j], oc[j], (float)i / (time * 100));
            }
        }

        if (dp) {
            for (int j = 0; j < image.Length; j++) {
                image[j].color = oc[j];
            }
        }

        if (mode == 2)
        {
            SIMG_Fade_End();
        }
        onSIMG_Coroutine = false;
    }

    private void SIMG_Fade_End()
    {
        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        SIMG_state = SIMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void SIMG_Reverse()
    {
        var eo = TeManager.Get_CullentEvent();
        if (simageDict.ContainsKey(eo.image_num))
        {
            var image = simageDict[eo.image_num];
            image.transform.localScale = new Vector3(-image.transform.localScale.x, image.transform.localScale.y, image.transform.localScale.z);
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        SIMG_state = SIMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void SIMG_Rot_Waiting()
    {
        if (!onSIMG_Coroutine)
        {
            SIMG_Rot();
        }
    }

    private void SIMG_Rot()
    {
        var eo = TeManager.Get_CullentEvent();
        if (simageDict.ContainsKey(eo.image_num))
        {
            var image = simageDict[eo.image_num];

            StartCoroutine(C_SIMG_Rot(image.transform, eo.register3, eo.seconds, eo.register1));

            if (eo.register1 == 1)
            {
                SIMG_Rot_End();
            }
        }
        else
        {
            SIMG_Rot_End();
        }
    }



    private IEnumerator C_SIMG_Rot(Transform transform, float rot, float time, int mode)
    {
        bool dp = false;
        onSIMG_Coroutine = true;
        if (time < 0) { time = 0; dp = true; }

        var start = transform.localRotation;
        var end = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(0, 0, rot));

        for (int i = 0; i <= 100 * time; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.localRotation = Quaternion.Lerp(start, end, (float)i / (time * 100));
        }

        if (dp) {
            transform.localRotation = end;
        }

        if (mode == 0)
        {
            SIMG_Rot_End();
        }
        onSIMG_Coroutine = false;
    }

    private void SIMG_Rot_End()
    {
        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        SIMG_state = SIMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void SIMG_Focus()
    {
        var eo = TeManager.Get_CullentEvent();
        if (simageDict.ContainsKey(eo.image_num))
        {
            var image = simageDict[eo.image_num];

            if (eo.register1 == 0)
            {
                foreach (var s in image.image_r)
                {
                    s.color = new Color(1f, 1f, 1f, 1);
                }
            }
            else if (eo.register1 == 1)
            {
                foreach (var s in image.image_r)
                {
                    s.color = new Color(0.6f, 0.6f, 0.6f, 1);
                }
            }
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        SIMG_state = SIMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void SIMG_Anime()
    {
        var eo = TeManager.Get_CullentEvent();
        if (simageDict.ContainsKey(eo.image_num))
        {
            var image = simageDict[eo.image_num];
            image.Set_Animation(eo.register2);
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        SIMG_state = SIMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void SIMG_StopAnime()
    {
        var eo = TeManager.Get_CullentEvent();
        if (simageDict.ContainsKey(eo.image_num))
        {
            var image = simageDict[eo.image_num];
            image.Set_Animation(0);
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        SIMG_state = SIMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void SIMG_Depth()
    {
        var eo = TeManager.Get_CullentEvent();

        if (simageDict.ContainsKey(eo.image_num))
        {

            var image = simageDict[eo.image_num];
            //Debug.Log(image.gameObject.name);
            image.transform.localPosition = new Vector3(image.transform.localPosition.x, image.transform.localPosition.y, eo.register3);
            //Debug.Log(image.transform.localPosition);
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        SIMG_state = SIMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void SIMG_Color()
    {
        var eo = TeManager.Get_CullentEvent();

        if (simageDict.ContainsKey(eo.image_num))
        {

            var image = simageDict[eo.image_num];
            image.Set_Color(eo.color);
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        SIMG_state = SIMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }
}

