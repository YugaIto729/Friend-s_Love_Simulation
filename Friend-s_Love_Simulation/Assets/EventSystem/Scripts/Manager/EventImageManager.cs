using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class EventImageManager : MonoBehaviour
{
    public static EventImageManager instance;

    public enum IMG_State
    {
        STANDBY, SET, REMOVE, MOVE, FADE, REVERSE, ROTETO
    }
    public IMG_State IMG_state = IMG_State.STANDBY;
    public GameObject CanvasObject;
    public GameObject prefab_Sprite;

    private bool onIMG_Coroutine = false;
    private TalkEventManager TeManager;
    private Dictionary<int, Image_Event_Prefab> imageDict = new Dictionary<int, Image_Event_Prefab>();

    //public List<Sprite> ImagesList;
    [SerializeField]
    private string ResoucePass = "Img/";


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
        if (IMG_state != IMG_State.STANDBY)
        {
            switch (IMG_state)
            {
                case IMG_State.SET:
                    IMG_ImageSet();
                    break;
                case IMG_State.MOVE:
                    IMG_Move_Waiting();
                    break;
                case IMG_State.REMOVE:
                    IMG_Remove_Waiting();
                    break;
                case IMG_State.FADE:
                    IMG_Fade_Waiting();
                    break;
                case IMG_State.REVERSE:
                    IMG_Reverse();
                    break;
                case IMG_State.ROTETO:
                    IMG_Rot_Waiting();
                    break;
            }
        }
    }

    private Sprite Get_SpriteAsset(string pass)
    {
        Sprite sprite = Resources.Load<Sprite>(ResoucePass + pass);
        return sprite;
    }


    /// <summary>
    /// 画像配置
    /// </summary>
    private void IMG_ImageSet()
    {
        var eo = TeManager.Get_CullentEvent();
        Sprite sprite = Get_SpriteAsset(eo.name);

        if (sprite != null)
        {
            if (imageDict.ContainsKey(eo.image_num)) //管理番号の画像が存在する場合
            {
                var o = imageDict[eo.image_num];
                o.transform.parent = CanvasObject.transform;
                var iep = o.GetComponent<Image_Event_Prefab>();
                iep.Set_Image(eo.image_num, eo.image_point, eo.image_scale, sprite);
            }
            else
            {
                var o = Instantiate(prefab_Sprite);
                o.transform.parent = CanvasObject.transform;
                var iep = o.GetComponent<Image_Event_Prefab>();
                iep.Set_Image(eo.image_num, eo.image_point, eo.image_scale, sprite);
                imageDict.Add(eo.image_num, iep);
            }
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        IMG_state = IMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    /// <summary>
    /// 画像消去待ち
    /// </summary>
    private void IMG_Remove_Waiting()
    {
        if (!onIMG_Coroutine)
        {
            IMG_Remove();
        }
    }

    /// <summary>
    /// 画像消去
    /// </summary>
    private void IMG_Remove()
    {
        var eo = TeManager.Get_CullentEvent();

        if (eo.image_num == -1)
        {
            List<Image_Event_Prefab> list = new List<Image_Event_Prefab>();

            foreach (var ed in imageDict)
            {
                var edo = ed.Value;
                list.Add(edo);
            }

            foreach (var l in list)
            {
                Destroy(l.gameObject);
            }

            imageDict.Clear();
        }

        if (imageDict.ContainsKey(eo.image_num))
        {
            var edo = imageDict[eo.image_num];
            imageDict.Remove(eo.image_num);
            Destroy(edo.gameObject);

        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        IMG_state = IMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void IMG_Move_Waiting()
    {
        if (!onIMG_Coroutine)
        {
            IMG_Move();
        }
    }

    /// <summary>
    /// 画像を移動する
    /// </summary>
    private void IMG_Move()
    {
        var eo = TeManager.Get_CullentEvent();
        if (imageDict.ContainsKey(eo.image_num))
        {
            var imageO = imageDict[eo.image_num];

            switch (eo.register1)
            {
                case 0:
                case 1:
                    {
                        Vector2 start = imageO.transform.localPosition;
                        Vector2 goal = start + eo.image_point;
                        StartCoroutine(C_IMG_Move(imageO, goal, eo.seconds, eo.register1));
                    }
                    break;
                case 2:
                case 3:
                    {
                        Vector2 start = imageO.transform.localPosition;
                        Vector2 goal = eo.image_point;
                        StartCoroutine(C_IMG_Move(imageO, goal, eo.seconds, eo.register1));
                    }
                    break;
                default:
                    IMG_Move_End();
                    break;
            }

            if (eo.register1 == 1 || eo.register1 == 3)
            {
                IMG_Move_End();
            }
        }
        else
        {
            IMG_Move_End();
        }
        
    }

    /// <summary>
    /// 画像を移動し終える
    /// </summary>
    private void IMG_Move_End()
    {
        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        IMG_state = IMG_State.STANDBY;

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
    private IEnumerator C_IMG_Move(Image_Event_Prefab event_Prefab, Vector2 goal, float time, int mode)
    {
        onIMG_Coroutine = true;
        Vector2 start = event_Prefab.transform.localPosition;
        if (time < 0) time = 0;

        for (int i = 0; i <= 100 * time; i++)
        {
            yield return new WaitForSeconds(0.01f);
            event_Prefab.transform.localPosition = Vector2.Lerp(start, goal, (float)i / (time * 100));
        }

        if (mode == 0 || mode == 2)
        {
            IMG_Move_End();
        }
        onIMG_Coroutine = false;
    }

    private void IMG_Fade_Waiting()
    {
        
        if (!onIMG_Coroutine)
        {
            Debug.Log("IMG_Fade_Waiting");
            IMG_Fade();
        }
    }

    private void IMG_Fade()
    {
        var eo = TeManager.Get_CullentEvent();
        if (imageDict.ContainsKey(eo.image_num))
        {
            var image = imageDict[eo.image_num].image;
            var oc = new Color(image.color.r, image.color.g, image.color.b, 0);
            var ic = new Color(image.color.r, image.color.g, image.color.b, 1);

            switch (eo.register1)
            {
                case 0:
                case 1:
                    image.color = oc;
                    StartCoroutine(C_IMG_FadeIn(image, oc, ic, eo.seconds, eo.register1));
                    break;
                case 2:
                case 3:
                    image.color = ic;
                    StartCoroutine(C_IMG_FadeOut(image, oc, ic, eo.seconds, eo.register1));
                    break;
                default:
                    IMG_Fade_End();
                    break;
            }

            if (eo.register1 == 1 || eo.register1 == 3) {
                IMG_Fade_End();
            }
        }  else {
            IMG_Fade_End();
        }

    }

    private IEnumerator C_IMG_FadeIn(Image image, Color oc, Color ic, float time, int mode)
    {

        onIMG_Coroutine = true;
        if (time < 0) time = 0;

        for (int i = 0; i <= 100 * time; i++)
        {
            yield return new WaitForSeconds(0.01f);
            image.color = Color.Lerp(oc, ic, (float)i / (time * 100));
        }

        if (mode == 0)
        {
            IMG_Move_End();
        }
        onIMG_Coroutine = false;
    }

    private IEnumerator C_IMG_FadeOut(Image image, Color oc, Color ic, float time, int mode)
    {
        onIMG_Coroutine = true;

        for (int i = 0; i < 100 * time; i++)
        {
            yield return new WaitForSeconds(0.01f);
            image.color = Color.Lerp(ic, oc, (float)i / (time * 100));
        }

        if (mode == 2)
        {
            IMG_Move_End();
        }
        onIMG_Coroutine = false;
    }

    private void IMG_Fade_End()
    {
        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        IMG_state = IMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void IMG_Reverse()
    {
        var eo = TeManager.Get_CullentEvent();
        if (imageDict.ContainsKey(eo.image_num))
        {
            var image = imageDict[eo.image_num];
            image.transform.localScale = new Vector2(-image.transform.localScale.x, image.transform.localScale.y);
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        IMG_state = IMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void IMG_Rot_Waiting()
    {
        if (!onIMG_Coroutine)
        {
            IMG_Rot();
        }
    }

    private void IMG_Rot()
    {
        var eo = TeManager.Get_CullentEvent();
        if (imageDict.ContainsKey(eo.image_num))
        {
            var image = imageDict[eo.image_num];

            StartCoroutine(C_IMG_Rot(image.transform, eo.register3, eo.seconds, eo.register1));

            if (eo.register1 == 1)
            {
                IMG_Rot_End();
            }
        }
        else
        {
            IMG_Rot_End();
        }
    }

    private IEnumerator C_IMG_Rot(Transform transform, float rot, float time, int mode)
    {
        onIMG_Coroutine = true;
        if (time < 0) time = 0;

        var start = transform.localRotation;
        var end = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(0, 0, rot));

        for (int i = 0; i <= 100 * time; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.localRotation = Quaternion.Lerp(start, end, (float)i / (time * 100));
        }

        if (mode == 0)
        {
            IMG_Rot_End();
        }
        onIMG_Coroutine = false;
    }

    private void IMG_Rot_End()
    {
        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        IMG_state = IMG_State.STANDBY;

        TeManager.Increment_EventCounter();
    }
}
