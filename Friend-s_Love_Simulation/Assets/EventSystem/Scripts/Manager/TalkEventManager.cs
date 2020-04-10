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

    /// <summary> 現在予約中のイベントソース </summary>
    public List<string> reservation_List = new List<string>();
    public bool isReservation = false;

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

        //EventStart("Test6");
        //EventReservation("Test_Audio");
        //EventReservation("Test4");
        //EventReservation("Test3");

    }

    // Update is called once per frame
    void Update()
    {
        EventDo_Reservation();

        if (cullentEvent == EventType.SYSTEMWAIT)
        {
            Decoding(Get_CullentEvent());
        }

        /*
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
        */
    }

    /// <summary>
    /// 予約されたイベントを実行する
    /// </summary>
    private void EventDo_Reservation()
    {
        if (cullentEvent == EventType.STANDBY)
        {
            if (reservation_List.Count > 0)
            {
                isReservation = true;

                string s = reservation_List[0];
                EventStart(s);
                reservation_List.RemoveAt(0);
            }
            else
            {
                isReservation = false;
            }
        }
    }

    /// <summary>
    /// イベントを予約する
    /// </summary>
    /// <param name="pass"></param>
    public void EventReservation(string pass)
    {
        reservation_List.Add(pass);
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
    public void EventStart(string eventPass, bool setable, int mode)
    {
        if (cullentEvent == EventType.STANDBY && !eventMode)
        {
            switch (mode) {
                case 0:
                    //イベントを読み込む
                    eventBuff = EventDecoder.instance.Get_EventObjects(eventPass);
                    break;
                case 1:
                    eventBuff = EventDecoder.instance.Get_EventObjects_D(eventPass);
                    break;
            }

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
        EventStart(eventPass, false, 0);
    }

    public void EventStart(string data, int mode)
    {
        EventStart(data, false, mode);
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

    public void EventStart_D(string data)
    {

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
            case EventType.DIALOG_SET:
                if (DialogManager.instance != null)
                {
                    DialogManager.instance.DM_state = DialogManager.DM_State.SET;
                    cullentEvent = EventType.DIALOG_SET;
                }
                break;
            case EventType.DIALOG_WAIT:
                if (DialogManager.instance != null)
                {
                    DialogManager.instance.DM_state = DialogManager.DM_State.WAIT;
                    cullentEvent = EventType.WAIT;
                }
                break;
            case EventType.DIALOG_REMOVE:
                if (DialogManager.instance != null)
                {
                    DialogManager.instance.DM_state = DialogManager.DM_State.REMOVE;
                    cullentEvent = EventType.DIALOG_REMOVE;
                }
                break;
            case EventType.DIALOG_DEFUALT:
                if (DialogManager.instance != null)
                {
                    DialogManager.instance.DM_state = DialogManager.DM_State.SET_DEFAULT;
                    cullentEvent = EventType.DIALOG_DEFUALT;
                }
                break;
            case EventType.SCH_SET:
                if (EventCheckManager.instance != null)
                {
                    Debug.Log("おわあああああああ");
                    EventCheckManager.instance.ECM_state = EventCheckManager.ECM_State.SET;
                    cullentEvent = EventType.SCH_SET;
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
            //Debug.Log("終了");
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

public sealed class EventSource
{
    public EventSource(List<EventObject> events)
    {
        eventObjects = events;
    }

    public List<EventObject> eventObjects;
} 
