using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class BackImgManager : MonoBehaviour
{
    public static BackImgManager instance;

    public enum BI_State
    {
        STANDBY, SET, MOVE, REMOVE, ROTETO, DEPTH
    }
    public BI_State BI_state = BI_State.STANDBY;
    public GameObject[] ImageObject;
    public Sprite[] prefab_Sprite;

    private bool onIB_Coroutine = false;
    private TalkEventManager TeManager;

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
        if (BI_state != BI_State.STANDBY)
        {
            switch (BI_state)
            {
                case BI_State.SET:
                    BI_ImageSet();
                    break;
                case BI_State.MOVE:
                    BI_Move_Waiting();
                    break;
                case BI_State.REMOVE:
                    BI_Remove_Waiting();
                    break;
                case BI_State.ROTETO:
                    BI_Rot_Waiting();
                    break;
                case BI_State.DEPTH:
                    BI_Depth();
                    break;
            }
        }
    }


    /// <summary>
    /// 画像配置
    /// </summary>
    private void BI_ImageSet()
    {
        var eo = TeManager.Get_CullentEvent();
        if (prefab_Sprite[eo.register2] != null)
        {
            if (ImageObject[eo.register2] != null)
            {
                var sr = ImageObject[eo.image_num].GetComponent<SpriteRenderer>();

                sr.sprite = prefab_Sprite[eo.register2];
                ImageObject[eo.register2].transform.localPosition = new Vector3(eo.image_point.x, eo.image_point.y, ImageObject[eo.register2].transform.localPosition.z);
                ImageObject[eo.register2].transform.localScale = new Vector3(eo.image_scale.x, eo.image_scale.y, 1);
            }
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        BI_state = BI_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    /// <summary>
    /// 画像消去待ち
    /// </summary>
    private void BI_Remove_Waiting()
    {
        if (!onIB_Coroutine)
        {
            BI_Remove();
        }
    }

    /// <summary>
    /// 画像消去
    /// </summary>
    private void BI_Remove()
    {
        var eo = TeManager.Get_CullentEvent();

        if (ImageObject[eo.register2] != null)
        {
            ImageObject[eo.register2].GetComponent<SpriteRenderer>().sprite = null;
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        BI_state = BI_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void BI_Move_Waiting()
    {
        if (!onIB_Coroutine)
        {
            BI_Move();
        }
    }

    /// <summary>
    /// 画像を移動する
    /// </summary>
    private void BI_Move()
    {
        var eo = TeManager.Get_CullentEvent();
        if (ImageObject[eo.register2] != null)
        {
            var imageO = ImageObject[eo.register2];

            switch (eo.register1)
            {
                case 0:
                case 1:
                    {
                        Vector2 start = imageO.transform.localPosition;
                        Vector2 goal = start + eo.image_point;
                        StartCoroutine(C_BI_Move(imageO, goal, eo.seconds, eo.register1));
                    }
                    break;
                case 2:
                case 3:
                    {
                        Vector2 start = imageO.transform.localPosition;
                        Vector2 goal = eo.image_point;
                        StartCoroutine(C_BI_Move(imageO, goal, eo.seconds, eo.register1));
                    }
                    break;
                default:
                    BI_Move_End();
                    break;
            }

            if (eo.register1 == 1 || eo.register1 == 3)
            {
                BI_Move_End();
            }
        }
        else
        {
            BI_Move_End();
        }

    }

    /// <summary>
    /// 画像を移動し終える
    /// </summary>
    private void BI_Move_End()
    {
        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        BI_state = BI_State.STANDBY;

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
    private IEnumerator C_BI_Move(GameObject event_Prefab, Vector2 b_goal, float time, int mode)
    {
        onIB_Coroutine = true;
        Vector3 start = event_Prefab.transform.localPosition;
        Vector3 goal = new Vector3(b_goal.x, b_goal.y, event_Prefab.transform.localPosition.z);
        if (time > 0)
        {
            for (int i = 0; i <= 100 * time; i++)
            {
                yield return new WaitForSeconds(0.01f);
                event_Prefab.transform.localPosition = Vector3.Lerp(start, goal, i / (time * 100));
            }
        }
        else
        {
            event_Prefab.transform.localPosition = goal;
        }

        if (mode == 0 || mode == 2)
        {
            BI_Move_End();
        }
        onIB_Coroutine = false;
    }



    private void BI_Rot_Waiting()
    {
        if (!onIB_Coroutine)
        {
            BI_Rot();
        }
    }

    private void BI_Rot()
    {
        var eo = TeManager.Get_CullentEvent();
        if (ImageObject[eo.register2] != null)
        {
            var image = ImageObject[eo.register2];

            StartCoroutine(C_BI_Rot(image.transform, eo.register3, eo.seconds, eo.register1));

            if (eo.register1 == 1)
            {
                BI_Rot_End();
            }
        }
        else
        {
            BI_Rot_End();
        }
    }

    private void BI_Depth()
    {
        var eo = TeManager.Get_CullentEvent();

        if (ImageObject[eo.register2] != null)
        {

            var image = ImageObject[eo.register2];
            Debug.Log(image.gameObject.name);
            image.transform.localPosition = new Vector3(image.transform.localScale.x, image.transform.localScale.y, eo.register3);
            Debug.Log(image.transform.localPosition);
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        BI_state = BI_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private IEnumerator C_BI_Rot(Transform transform, float rot, float time, int mode)
    {
        bool dp = false;
        onIB_Coroutine = true;
        if (time < 0) { time = 0; dp = true; }

        var start = transform.localRotation;
        var end = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(0, 0, rot));

        for (int i = 0; i <= 100 * time; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.localRotation = Quaternion.Lerp(start, end, (float)i / (time * 100));
        }

        if (dp)
        {
            transform.localRotation = end;
        }

        if (mode == 0)
        {
            BI_Rot_End();
        }
        onIB_Coroutine = false;
    }

    private void BI_Rot_End()
    {
        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        BI_state = BI_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

}
