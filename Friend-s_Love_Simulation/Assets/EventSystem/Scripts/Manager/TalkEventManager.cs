using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public sealed class TalkEventManager : MonoBehaviour
{
    public static TalkEventManager instance;
    /// <summary> 現在実行しているイベントのindex </summary>
    [HideInInspector]public int culletIndex=0;

    /// <summary> 現在実行しているイベント </summary>
    public EventType cullentEvent = EventType.STANDBY;　//0のとき、待機状態

    /// <summary> イベント実行モード </summary>
    public bool eventMode = false;

    /// <summary> 現在のイベントデータ </summary>
    public List<EventObject> eventBuff;

    /// <summary> 単体イベント実行を許可する </summary>
    public bool isSetable = true;



    //=====================================================================================================================

    private EventWindowManager EwManager;

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

    // Start is called before the first frame update
    void Start()
    {
        EwManager = EventWindowManager.instance;

        //-----------------------------------------------

        EventStart("Test6");

        /*
        EventStart_Message("突然の死！！！！", "");
        EventStart_SetImage(0, 0, new Vector2(0, 0));
        EventStart_MoveImage(0, 2, new Vector2(50, 50), Mode_ImageMove.ADDPOINT);
        EventStart_FadeImage(0, 1, Mode_FadeImage.FADEOUT);
        EventStart_FadeImage(0, 1, Mode_FadeImage.FADEIN);
        EventStart_ReverseImage(0);
        EventStart_RotetoImage(0, 1, 5, Mode_RotImage.WAIT);
        EventStart_RemoveImage(0);
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (cullentEvent == EventType.SYSTEMWAIT)
        {
            Decoding(Get_CullentEvent());
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            EventStart("Test");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            EventStart("Test2");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            EventStart("Test3");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            EventStart("Test4");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            EventStart("Test5");
        }
    }

    #region 登録がクソめんどい

    /// <summary>
    /// イベントを同期的に開始させる
    /// </summary>
    /// <param name="eventPass"></param>
    public void EventStart_Sync(string eventPass)
    {
        StartCoroutine(C_EventStart_Sync(eventPass, false));
    }

    public void EventStart_Sync(string eventPass, bool setable)
    {
        StartCoroutine(C_EventStart_Sync(eventPass, setable));
    }

    private IEnumerator C_EventStart_Sync(string eventPass, bool setable)
    {
        if (cullentEvent == EventType.STANDBY && !eventMode)
        {
            EventDecoder.instance.Decode_EventObject_Sync(eventPass);

            while (true)
            {
                Debug.Log("<LOADING>");

                if (EventDecoder.instance.Get_Error_Sync()) yield break;

                eventBuff = EventDecoder.instance.Get_EventObjects_Sync();

                if (eventBuff != null && eventBuff.Count != 0) //イベント読み込み成功
                {
                    cullentEvent = EventType.SYSTEMWAIT;
                    eventMode = true;
                    isSetable = setable;
                    yield break;
                }
                yield return null;
            }
        }
    }




    /// <summary>
    /// イベントを開始させる
    /// </summary>
    /// <param name="eventPass"></param>
    public void EventStart(string eventPass, bool setable)
    {
        if (cullentEvent == EventType.STANDBY && !eventMode)
        {
            //イベントを読み込む
            eventBuff = EventDecoder.instance.Get_EventObjects(eventPass);
            //foreach (var a in eventBuff) Debug.Log(a.eventType);

            if (eventBuff != null && eventBuff.Count != 0) //イベント読み込み成功
            {
                cullentEvent = EventType.SYSTEMWAIT;
                eventMode = true;
                isSetable = setable;
            }
        }
    }

    /// <summary>
    /// イベントを開始させる
    /// </summary>
    /// <param name="eventPass"></param>
    public void EventStart(string eventPass)
    {
        EventStart(eventPass, false);
    }

    /// <summary>
    /// イベントを開始させる
    /// </summary>
    /// <param name="eventPass"></param>
    public void EventStart(List<EventObject> eventList, bool setable)
    {
        if (cullentEvent == EventType.STANDBY && !eventMode)
        {
            //イベントを読み込む
            eventBuff = eventList;
            //foreach (var a in eventBuff) Debug.Log(a.eventType);

            if (eventBuff != null && eventBuff.Count != 0) //イベント読み込み成功
            {
                cullentEvent = EventType.SYSTEMWAIT;
                eventMode = true;
                isSetable = setable;
            }
        }
    }

    /// <summary>
    /// イベントを開始させる
    /// </summary>
    /// <param name="eventPass"></param>
    public void EventStart(List<EventObject> eventList)
    {
        EventStart(eventList, false);
    }

    /// <summary>
    /// メッセージイベントを開始させる
    /// </summary>
    /// <param name="massage"></param>
    /// <param name="name"></param>
    public void EventStart_Message(string massage, string name)
    {
        if (isSetable)
        {

            EventObject eventObject = new EventObject
            {
                eventType = EventType.MESSAGE,
                name = name,
                message = massage
            };

            if (!eventMode)
            {
                eventBuff = new List<EventObject> { eventObject };
                cullentEvent = EventType.SYSTEMWAIT;
                eventMode = true;
            }
            else
            {
                eventBuff.Add(eventObject);
            }
        }
    }

    /// <summary>
    /// 画像を配置するイベントを開始する
    /// </summary>
    /// <param name="num"></param>
    /// <param name="num_image"></param>
    /// <param name="point"></param>
    /// <param name="size"></param>
    public void EventStart_SetImage(int num, int num_image, Vector2 point, Vector2 size)
    {
        if (isSetable)
        {
            EventObject eventObject = new EventObject
            {
                eventType = EventType.IMAGE_SET,
                image_num = num,
                sprite = GameManager.instance.ImagesList[num_image],
                image_point = point,
                image_scale = size
             };

            if (!eventMode)
            {
                eventBuff = new List<EventObject> { eventObject };
                cullentEvent = EventType.SYSTEMWAIT;
                eventMode = true;
            }
            else
            {
                eventBuff.Add(eventObject);
            }
        }
    }

    /// <summary>
    /// 画像を配置するイベントを開始する
    /// </summary>
    /// <param name="num"></param>
    /// <param name="num_image"></param>
    /// <param name="point"></param>
    public void EventStart_SetImage(int num, int num_image, Vector2 point)
    {
        EventStart_SetImage(num, num_image, point, new Vector2(100, 100));
    }

    /// <summary>
    /// 画像を配置するイベントを開始する
    /// </summary>
    /// <param name="num"></param>
    /// <param name="sprite"></param>
    /// <param name="point"></param>
    /// <param name="size"></param>
    public void EventStart_SetImage(int num, Sprite sprite, Vector2 point, Vector2 size)
    {
        if (isSetable)
        {
            EventObject eventObject = new EventObject
            {
                eventType = EventType.IMAGE_SET,
                image_num = num,
                sprite = sprite,
                image_point = point,
                image_scale = size
            };

            if (!eventMode)
            {
                eventBuff = new List<EventObject> { eventObject };
                cullentEvent = EventType.SYSTEMWAIT;
                eventMode = true;
            }
            else
            {
                eventBuff.Add(eventObject);
            }
        }
    }

    /// <summary>
    /// 画像を配置するイベントを開始する
    /// </summary>
    /// <param name="num"></param>
    /// <param name="sprite"></param>
    /// <param name="point"></param>
    public void EventStart_SetImage(int num, Sprite sprite, Vector2 point)
    {
        EventStart_SetImage(num, sprite, point, new Vector2(100, 100));
    }

    public enum Mode_ImageMove {
        ///<summary>座標を加算</summary>
        ADDPOINT,
        ///<summary>座標を加算(待ち無し)</summary>
        ADDPOINT_NOWAIT,
        ///<summary>指定座標に移動</summary>
        TARGETPOINT,
        ///<summary>指定座標に移動(待ち無し)</summary>
        TARGETPOINT_NOWAIT
    }
    /// <summary>
    /// 画像を移動させるイベントを開始する
    /// </summary>
    /// <param name="num"></param>
    /// <param name="time"></param>
    /// <param name="point"></param>
    /// <param name="mode"></param>
    public void EventStart_MoveImage(int num, float time, Vector2 point, Mode_ImageMove mode)
    {
        if (isSetable)
        {
            EventObject eventObject = new EventObject
            {
                eventType = EventType.IMAGE_MOVE,
                image_num = num,
                seconds = time,
                image_point = point,
                register1 = (int)mode,
            };

            if (!eventMode)
            {
                eventBuff = new List<EventObject> { eventObject };
                cullentEvent = EventType.SYSTEMWAIT;
                eventMode = true;
            }
            else
            {
                eventBuff.Add(eventObject);
            }
        }
    }

    public enum Mode_FadeImage
    {
        /// <summary> フェードイン </summary>
        FADEIN,
        /// <summary> フェードイン(待ち無し) </summary>
        FADEIN_NOWAIT,
        /// <summary> フェードアウト </summary>
        FADEOUT,
        /// <summary> フェードアウト(待ち無し) </summary>
        FADEOUT_NOWAIT,

    }

    /// <summary>
    /// 画像をフェードイン/フェードアウトさせるイベントを開始する
    /// </summary>
    /// <param name="num"></param>
    /// <param name="time"></param>
    /// <param name="mode"></param>
    public void EventStart_FadeImage(int num, float time, Mode_FadeImage mode)
    {
        if (isSetable)
        {
            EventObject eventObject = new EventObject
            {
                eventType = EventType.IMAGE_FADE,
                image_num = num,
                seconds = time,
                register1 = (int)mode,
            };

            if (!eventMode)
            {
                eventBuff = new List<EventObject> { eventObject };
                cullentEvent = EventType.SYSTEMWAIT;
                eventMode = true;
            }
            else
            {
                eventBuff.Add(eventObject);
            }
        }
    }

    /// <summary>
    /// 画像を左右反転させるイベントを開始する
    /// </summary>
    /// <param name="num"></param>
    public void EventStart_ReverseImage(int num)
    {
        if (isSetable)
        {
            EventObject eventObject = new EventObject
            {
                eventType = EventType.IMAGE_REVERSE,
                image_num = num,
            };

            if (!eventMode)
            {
                eventBuff = new List<EventObject> { eventObject };
                cullentEvent = EventType.SYSTEMWAIT;
                eventMode = true;
            }
            else
            {
                eventBuff.Add(eventObject);
            }
        }
    }

    public enum Mode_RotImage
    {
        /// <summary> 待ちあり </summary>
        WAIT,
        /// <summary> 待ち無し </summary>
        NOWAIT,
    }

    /// <summary>
    /// 画像を回転させるイベントを開始する
    /// </summary>
    /// <param name="num"></param>
    /// <param name="time"></param>
    /// <param name="rot"></param>
    /// <param name="mode"></param>
    public void EventStart_RotetoImage(int num, int time, float rot, Mode_RotImage mode)
    {
        if (isSetable)
        {
            EventObject eventObject = new EventObject
            {
                eventType = EventType.IMAGE_ROT,
                seconds = time,
                register3 = rot,
                image_num = num,
                register1 = (int)mode
            };

            if (!eventMode)
            {
                eventBuff = new List<EventObject> { eventObject };
                cullentEvent = EventType.SYSTEMWAIT;
                eventMode = true;
            }
            else
            {
                eventBuff.Add(eventObject);
            }
        }
    }

    /// <summary>
    /// 画像を削除するイベントを開始する
    /// </summary>
    /// <param name="num"></param>
    public void EventStart_RemoveImage(int num)
    {
        if (isSetable)
        {
            EventObject eventObject = new EventObject
            {
                eventType = EventType.IMAGE_REMOVE,
                image_num = num,
            };

            if (!eventMode)
            {
                eventBuff = new List<EventObject> { eventObject };
                cullentEvent = EventType.SYSTEMWAIT;
                eventMode = true;
            }
            else
            {
                eventBuff.Add(eventObject);
            }
        }
    }

    #endregion
    //==========================================================================================

    /// <summary>
    /// イベントオブジェクトを読み込み、開始させる
    /// </summary>
    private void Decoding(EventObject eventObject)
    {
        switch (eventObject.eventType)
        {
            case EventType.MESSAGE:
                if (EventWindowManager.instance != null)
                {
                    EventWindowManager.instance.MS_state = EventWindowManager.MS_State.WAITING;
                    cullentEvent = EventType.MESSAGE;
                }
                break;
            case EventType.MESS_DELETE:
                if (EventWindowManager.instance != null)
                {
                    EventWindowManager.instance.MS_state = EventWindowManager.MS_State.DELETE;
                    cullentEvent = EventType.MESS_DELETE;
                }
                break;
            case EventType.MESS_DISPLAY:
                if (EventWindowManager.instance != null)
                {
                    EventWindowManager.instance.MS_state = EventWindowManager.MS_State.W_DISPLAY;
                    cullentEvent = EventType.MESS_DISPLAY;
                }
                break;
            case EventType.EVENTEND:
                {
                    cullentEvent = EventType.STANDBY;
                    culletIndex = 0;
                    eventMode = false;
                    isSetable = true;
                }
                break;
            case EventType.IMAGE_SET:
                if (EventImageManager.instance != null)
                {
                    EventImageManager.instance.IMG_state = EventImageManager.IMG_State.SET;
                    cullentEvent = EventType.IMAGE_SET;
                }
                break;
            case EventType.IMAGE_REMOVE:
                if (EventImageManager.instance != null)
                {
                    EventImageManager.instance.IMG_state = EventImageManager.IMG_State.REMOVE;
                    cullentEvent = EventType.IMAGE_REMOVE;
                }
                break;
            case EventType.IMAGE_MOVE:
                if (EventImageManager.instance != null)
                {
                    EventImageManager.instance.IMG_state = EventImageManager.IMG_State.MOVE;
                    cullentEvent = EventType.IMAGE_MOVE;
                }
                break;
            case EventType.IMAGE_FADE:
                if (EventImageManager.instance != null)
                {
                    EventImageManager.instance.IMG_state = EventImageManager.IMG_State.FADE;
                    cullentEvent = EventType.IMAGE_FADE;
                }
                break;
            case EventType.IMAGE_REVERSE:
                if (EventImageManager.instance != null)
                {
                    EventImageManager.instance.IMG_state = EventImageManager.IMG_State.REVERSE;
                    cullentEvent = EventType.IMAGE_REVERSE;
                }
                break;
            case EventType.IMAGE_ROT:
                if (EventImageManager.instance != null)
                {
                    EventImageManager.instance.IMG_state = EventImageManager.IMG_State.ROTETO;
                    cullentEvent = EventType.IMAGE_ROT;
                }
                break;
            case EventType.SIMG_SET:
                if (EventStandImgManager.instance != null)
                {
                    EventStandImgManager.instance.SIMG_state = EventStandImgManager.SIMG_State.SET;
                    cullentEvent = EventType.SIMG_SET;
                }
                break;
            case EventType.SIMG_MOVE:
                if (EventStandImgManager.instance != null)
                {
                    EventStandImgManager.instance.SIMG_state = EventStandImgManager.SIMG_State.MOVE;
                    cullentEvent = EventType.SIMG_MOVE;
                }
                break;
            case EventType.SIMG_FADING:
                if (EventStandImgManager.instance != null)
                {
                    EventStandImgManager.instance.SIMG_state = EventStandImgManager.SIMG_State.FADE;
                    cullentEvent = EventType.SIMG_FADING;
                }
                break;
            case EventType.SIMG_FOCUSING:
                if (EventStandImgManager.instance != null)
                {
                    EventStandImgManager.instance.SIMG_state = EventStandImgManager.SIMG_State.FOCUS;
                    cullentEvent = EventType.SIMG_FOCUSING;
                }
                break;
            case EventType.SIMG_REVERSE:
                if (EventStandImgManager.instance != null)
                {
                    EventStandImgManager.instance.SIMG_state = EventStandImgManager.SIMG_State.REVERSE;
                    cullentEvent = EventType.SIMG_REVERSE;
                }
                break;
            case EventType.SIMG_ROT:
                if (EventStandImgManager.instance != null)
                {
                    EventStandImgManager.instance.SIMG_state = EventStandImgManager.SIMG_State.ROTETO;
                    cullentEvent = EventType.SIMG_ROT;
                }
                break;
            case EventType.SIMG_REMOVE:
                if (EventStandImgManager.instance != null)
                {
                    EventStandImgManager.instance.SIMG_state = EventStandImgManager.SIMG_State.REMOVE;
                    cullentEvent = EventType.SIMG_REMOVE;
                }
                break;
            case EventType.SIMG_ANIME:
                if (EventStandImgManager.instance != null)
                {
                    EventStandImgManager.instance.SIMG_state = EventStandImgManager.SIMG_State.ANIME;
                    cullentEvent = EventType.SIMG_ANIME;
                }
                break;
            case EventType.SIMG_STANIM:
                if (EventStandImgManager.instance != null)
                {
                    EventStandImgManager.instance.SIMG_state = EventStandImgManager.SIMG_State.STOPANIME;
                    cullentEvent = EventType.SIMG_STANIM;
                }
                break;
            case EventType.SIMG_DEPTH:
                if (EventStandImgManager.instance != null)
                {
                    EventStandImgManager.instance.SIMG_state = EventStandImgManager.SIMG_State.DEPTH;
                    cullentEvent = EventType.SIMG_DEPTH;
                }
                break;
            case EventType.SIMG_COLOR:
                if (EventStandImgManager.instance != null)
                {
                    EventStandImgManager.instance.SIMG_state = EventStandImgManager.SIMG_State.COLOR;
                    cullentEvent = EventType.SIMG_COLOR;
                }
                break;
            case EventType.SELECT:
                if (EventSelectManager.instance != null)
                {
                    EventSelectManager.instance.SEL_state = EventSelectManager.SELE_State.WAITING;
                    cullentEvent = EventType.SELECT;
                }
                break;
            case EventType.SLELSE:
            case EventType.ENDSELECT:
                {
                    End_Select(Get_CullentEvent().choiceEvents[0]);
                }
                break;
            case EventType.BRANCH:
                Skip_Branch(Get_CullentEvent());
                break;
            case EventType.ELSEBRANCH:
            case EventType.ENDBRANCH:
                End_branch(Get_CullentEvent().choiceEvents[0]);
                break;
            case EventType.LABEL:
                Increment_EventCounter();
                break;
            case EventType.GOTO:
                Goto(Get_CullentEvent().message);
                break;
            case EventType.WAIT:
                cullentEvent = EventType.WAIT;
                StartCoroutine(C_Waiting(Get_CullentEvent().seconds));
                break;
            case EventType.FADE:
                if (FadeManager.instance != null)
                {
                    FadeManager.instance.f_state = FadeManager.F_State.FADE;
                    cullentEvent = EventType.FADE;
                }
                break;
            case EventType.BACK_SET:
                if (BackImgManager.instance != null)
                {
                    BackImgManager.instance.BI_state = BackImgManager.BI_State.SET;
                    cullentEvent = EventType.BACK_SET;
                }
                break;
            case EventType.BACK_MOVE:
                if (BackImgManager.instance != null)
                {
                    BackImgManager.instance.BI_state = BackImgManager.BI_State.MOVE;
                    cullentEvent = EventType.BACK_MOVE;
                }
                break;
            case EventType.BACK_ROT:
                if (BackImgManager.instance != null)
                {
                    BackImgManager.instance.BI_state = BackImgManager.BI_State.ROTETO;
                    cullentEvent = EventType.BACK_ROT;
                }
                break;
            case EventType.BACK_REMOVE:
                if (BackImgManager.instance != null)
                {
                    BackImgManager.instance.BI_state = BackImgManager.BI_State.REMOVE;
                    cullentEvent = EventType.BACK_REMOVE;
                }
                break;
            case EventType.VALUE_SET:
                if (ValuesManager.instance != null)
                {
                    ValuesManager.instance.VM_state = ValuesManager.VM_State.SET_VALUE;
                    cullentEvent = EventType.VALUE_SET;
                }
                break;
            case EventType.VALUE_S_SET:
                if (ValuesManager.instance != null)
                {
                    ValuesManager.instance.VM_state = ValuesManager.VM_State.SET_TEXT;
                    cullentEvent = EventType.VALUE_S_SET;
                }
                break;
            case EventType.BGMPLAY:
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.AM_state = AudioManager.AM_State.PLAY;
                    cullentEvent = EventType.BGMPLAY;
                }
                break;
            case EventType.BGMFADE:
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.AM_state = AudioManager.AM_State.FADE;
                    cullentEvent = EventType.BGMFADE;
                }
                break;
            case EventType.BGMSTOP:
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.AM_state = AudioManager.AM_State.STOP;
                    cullentEvent = EventType.BGMSTOP;
                }
                break;
            case EventType.BGMVOL:
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.AM_state = AudioManager.AM_State.VOLUME;
                    cullentEvent = EventType.BGMVOL;
                }
                break;
            case EventType.BGMCONT:
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.AM_state = AudioManager.AM_State.CONTINUE;
                    cullentEvent = EventType.BGMCONT;
                }
                break;
            case EventType.SOUND:
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.AM_state = AudioManager.AM_State.SOUND;
                    cullentEvent = EventType.SOUND;
                }
                break;
        }
    }

    /// <summary>
    /// 現在のイベントを取得する。
    /// </summary>
    /// <returns></returns>
    public EventObject Get_CullentEvent()
    {
        //Debug.Log("現在のイベント番号: " + culletIndex);
        return eventBuff[culletIndex];
    }

    /// <summary>
    /// cullentIndexをインクリメントして、イベント終了判定を行う。
    /// </summary>
    /// <returns></returns>
    public bool Increment_EventCounter()
    {
        culletIndex++;
        
        if (culletIndex >= eventBuff.Count)
        {
            Debug.Log("終了");
            cullentEvent = EventType.STANDBY;
            culletIndex = 0;
            eventMode = false;
            isSetable = true;
            return true;
        }
        return false;
    }

    public void Skip_Select(EventObject @event, int index)
    {
        if (@event.eventType == EventType.SELECT) {
            EventObject target;

            if (@event.choiceEvents.Count <= index) { Debug.LogError("indexの値がおかしい"); return;}
            target = @event.choiceEvents[index];

            while (true)
            {
                if (eventBuff.Count <= culletIndex) //見つからなかった
                {
                    Debug.LogError("SELECT_ELSE探索失敗");
                    Increment_EventCounter();
                    break;
                }

                if (Get_CullentEvent() == target)
                {
                    cullentEvent = EventType.SYSTEMWAIT;
                    culletIndex++;
                    break;
                }

                culletIndex++;
            }
        }

    }

    public void End_Select(EventObject @event)
    {
        if (@event.eventType == EventType.SELECT)
        {
            var target = @event.choiceEvents[@event.choiceEvents.Count - 1];

            while (true)
            {
                if (eventBuff.Count <= culletIndex) //見つからなかった
                {
                    Debug.LogError("SELECT_ELSE探索失敗");
                    Increment_EventCounter();
                    break;
                }

                if (Get_CullentEvent() == target)
                {
                    cullentEvent = EventType.SYSTEMWAIT;
                    Increment_EventCounter();
                    break;
                }

                culletIndex++;
            }


        }
    }

    public void Skip_Branch(EventObject @event)
    {
        if (ValuesManager.instance == null) return;

        if (@event.eventType== EventType.BRANCH)
        {
            int index;
            bool branch = @event.parsers[0].Eval(ValuesManager.instance.Get_Values());
            if (branch)
            {
                index = 0;
            }
            else
            {
                index = 1;
            }
            var target = @event.choiceEvents[index];

            while (true)
            {
                if (eventBuff.Count <= culletIndex) //見つからなかった
                {
                    Debug.LogError("SELECT_ELSE探索失敗");
                    cullentEvent = EventType.SYSTEMWAIT;
                    Increment_EventCounter();
                    
                    break;
                }

                if (Get_CullentEvent() == target)
                {
                    cullentEvent = EventType.SYSTEMWAIT;
                    Increment_EventCounter();
                    break;
                }

                culletIndex++;
            }
        }
    }

    public void End_branch(EventObject @event)
    {
        if (@event.eventType == EventType.BRANCH)
        {
            var target = @event.choiceEvents[@event.choiceEvents.Count - 1];

            while (true)
            {
                if (eventBuff.Count <= culletIndex) //見つからなかった
                {
                    Debug.LogError("SELECT_ELSE探索失敗");
                    cullentEvent = EventType.SYSTEMWAIT;
                    Increment_EventCounter();
                    break;
                }

                if (Get_CullentEvent() == target)
                {
                    cullentEvent = EventType.SYSTEMWAIT;
                    culletIndex++;
                    break;
                }

                culletIndex++;
            }
        }
    }

    public void Goto(string label)
    {
        int index = 0;
        while (true)
        {
            if (eventBuff.Count <= index)
            {
                Debug.Log("探索失敗");
                cullentEvent = EventType.SYSTEMWAIT;
                Increment_EventCounter();
                break;
            }

            if(eventBuff[index].eventType == EventType.LABEL) {
                if (eventBuff[index].message == label)
                {
                    //Debug.Log("見つけた");
                    cullentEvent = EventType.SYSTEMWAIT;
                    culletIndex = index;
                    Increment_EventCounter();
                    break;
                }
            }

            index++;
        }
    }

    private IEnumerator C_Waiting(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        cullentEvent = EventType.SYSTEMWAIT;
        Increment_EventCounter();
    }
}
