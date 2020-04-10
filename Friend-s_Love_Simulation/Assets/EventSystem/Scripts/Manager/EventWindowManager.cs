using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

public sealed class EventWindowManager : MonoBehaviour
{
    public static EventWindowManager instance;
    private TalkEventManager TeManager;

    public enum MS_State
    {
        STANDBY, WAITING, DISPLAYING, DISPLAYED, DISPLAYED2, INTERVALSKIP, NEXT, DELETE, W_DISPLAY
    }

    private List<string> list_endcode = new List<string>();

    /// <summary> メッセージシステムの状態 </summary>
    public MS_State MS_state = MS_State.STANDBY;
    /// <summary> 現在表示しているメッセージ </summary>　
    private string messageDisplay = "";
    /// <summary> 名前表示 </summary>
    private string message_name = "";
    /// <summary> 表示させる文字群 </summary>
    private string[] messageSplit;

    /// <summary> メッセージ許可 </summary>
    //public bool massageApproval = false;
    /// <summary> スキップ有効 </summary>
    public bool message_skip = false;
    /// <summary> オート有効 </summary>
    public bool message_auto = false;
    /// <summary> 進行用のフラグ </summary>
    public bool message_nextFlag = false;

    /// <summary> コルーチン管理フラグ </summary>
    private bool onMS_Coroutine = false;

    /// <summary> メッセージを表示済かどうか </summary>
    private bool isDisplayed = false;

    public GameObject MS_message_tO;
    public GameObject MS_message_nO;
    public GameObject MS_BackWindow;
    public Sprite[] sprite_back;

    private Text MS_message_text;
    private Text MS_message_name;
    private Image MS_back_image;
    private Animator MS_window_animator;
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

        MS_message_text = MS_message_tO.GetComponentInChildren<Text>();
        MS_message_name = MS_message_nO.GetComponentInChildren<Text>();
        MS_back_image = MS_BackWindow.GetComponent<Image>();

        MS_window_animator = MS_BackWindow.GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if (MS_state != MS_State.STANDBY)
        {
            MS_message_text.text = messageDisplay + Get_EndCode();
            MS_message_name.text = message_name;

            //Debug.Log("["+MS_state);

            switch (MS_state)
            {
                case MS_State.WAITING:
                    MS_Waiting();
                    break;
                case MS_State.DISPLAYING:
                    MS_Displaying();
                    break;
                case MS_State.DISPLAYED:
                    MS_Displayed();
                    break;
                case MS_State.DISPLAYED2:
                    MS_Displayed2();
                    break;
                case MS_State.INTERVALSKIP:
                    MS_IntervalSkip();
                    break;
                case MS_State.NEXT:
                    MS_Next();
                    break;
                case MS_State.DELETE:
                    MS_Delete();
                    break;
                case MS_State.W_DISPLAY:
                    MS_Window_Display();
                    break;
            }
        }

        //キー入力
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MS_Push_NextFlag();
        }

        //イベントが終了していたらウインドウを非表示にする
        if (!TeManager.eventMode)
        {
            //message_auto = false;
            //message_skip = false;

            if (MS_message_tO.activeSelf)
            {
                MS_NoDisplayWindow();
            }
        }

    }

    public void MS_Push_NextFlag()
    {
        if (MS_state == MS_State.DISPLAYING || MS_state == MS_State.DISPLAYED || MS_state == MS_State.DISPLAYED2)
        {
            message_nextFlag = true;
        }
    }


    /// <summary>
    /// ウインドウを出す
    /// </summary>
    public void MS_DisplayWindow(int state)
    {
        switch (state)
        {
            case 0:
                MS_message_nO.SetActive(true);
                MS_message_tO.SetActive(true);
                break;
            case 1:
                MS_message_nO.SetActive(false);
                MS_message_tO.SetActive(true);
                break;
        }
    }

    public void MS_DisplayWindow()
    {
        int index = 0;
        if (message_name != "")
        {
            index = 1;
        }

        MS_DisplayWindow(index);
    }

    /// <summary>
    /// ウインドウを消す
    /// </summary>
    public void MS_NoDisplayWindow()
    {
        MS_message_nO.SetActive(false);
        MS_message_tO.SetActive(false);
        messageDisplay = "";
        message_name = "";
    }

    /// <summary>
    /// 待機状態
    /// </summary>
    private void MS_Waiting()
    {
        if (TeManager.Get_CullentEvent().eventType != EventType.MESSAGE) //メッセージ以外のタイプだった場合
        {
            TeManager.cullentEvent = EventType.SYSTEMWAIT;
            MS_state = MS_State.STANDBY;
            return;
        }

        //名前の有無によってウインドウを表示/非表示する
        if (TeManager.Get_CullentEvent().name == "")
        {
            MS_DisplayWindow(1);
        }
        else
        {
            MS_DisplayWindow(0);
        }

        int mode = TeManager.Get_CullentEvent().register1;
        if (mode-1 < sprite_back.Length)
        {
            if (mode == 0)
            {
                MS_BackWindow.SetActive(false);
            }
            else if (mode > 0)
            {
                MS_BackWindow.SetActive(true);
                MS_back_image.sprite = sprite_back[mode-1];
            }
        }

        //アニメーションを開始
        //Debug.Log(MS_window_animator);
        //MS_window_animator.SetTrigger("next");

        //次のイベントオブジェクトを読み込む
        message_name = ReplaceCode(TeManager.Get_CullentEvent().name);
        messageSplit = MS_Split();
        message_nextFlag = false;

        foreach(string s in messageSplit)
        {
            //Debug.Log("Waiting: " + s);
        }

        if (messageDisplay != "")
        {
            messageDisplay = "";
        }

        isDisplayed = false;
        MS_state = MS_State.DISPLAYING;
    }

    /// <summary>
    /// 表示中状態
    /// </summary>
    private void MS_Displaying()
    {
        if (!onMS_Coroutine) StartCoroutine(C_MS_Displaying());
    }

    private IEnumerator C_MS_Displaying()
    {
        onMS_Coroutine = true;

        foreach (string ms in messageSplit)
        {
            if (message_skip)
            {
                break;
            }

            if (message_nextFlag)
            {
                message_nextFlag = false;
                break;
            }
            EndCoding(ms);
            messageDisplay += SplitMessage(ms);
            AudioManager.instance.SE_Play("button40" ,1,1);

            yield return new WaitForSeconds(0.05f);
        }
        MS_state = MS_State.DISPLAYED;
        onMS_Coroutine = false;
    }

    private string SplitMessage(string ms)
    {
        if (ValuesManager.instance != null)
        {
            if (Regex.IsMatch(ms, @"<v\d{1,4}>"))
            {
                string m = "";
                for (int i = 2; i < ms.Length - 1; i++)
                {
                    m += ms[i];
                }
                int v = (int)ValuesManager.instance.Get_Value(int.Parse(m));

                return v.ToString();
            }

            if (Regex.IsMatch(ms, @"<vf\d{1,4}>"))
            {
                string m = "";
                for (int i = 2; i < ms.Length - 1; i++)
                {
                    m += ms[i];
                }
                float v = ValuesManager.instance.Get_Value(int.Parse(m));

                return v.ToString();
            }
        }
        return ms;
    }

    /// <summary>
    /// 表示済み状態
    /// </summary>
    private void MS_Displayed()
    {
        MS_DisplayAll();

        if (!onMS_Coroutine) StartCoroutine(C_MS_Displayed());

        if (message_skip)
        {
            MS_state = MS_State.INTERVALSKIP;
        }

        if (message_nextFlag)
        {
            MS_state = MS_State.NEXT;
        }
    }

    private IEnumerator C_MS_Displayed()
    {
        onMS_Coroutine = true;
        for (int ci = 0; ci < 60; ci++)
        {
            if (MS_state != MS_State.DISPLAYED)
            {
                onMS_Coroutine = false;
                yield break;
            }
            yield return null;
        }
        isDisplayed = true;
        MS_state = MS_State.DISPLAYED2;
        onMS_Coroutine = false;
    }

    /// <summary>
    /// メッセージを全表示する
    /// </summary>
    private void MS_DisplayAll()
    {
        if (isDisplayed) return;

        messageDisplay = "";
        foreach(string a in messageSplit)
        {
            messageDisplay += a;
        }

        isDisplayed = true;
        list_endcode.Clear(); //末尾文字を表示しない
    }

    /// <summary>
    /// 表示済2
    /// </summary>
    private void MS_Displayed2()
    {
        if (message_nextFlag || message_auto)
        {
            MS_state = MS_State.NEXT;
        }

    }

    /// <summary>
    /// スキップのための間
    /// </summary>
    private void MS_IntervalSkip()
    {
        if (!onMS_Coroutine) StartCoroutine(C_MS_IntervalSkip());
    }

    private IEnumerator C_MS_IntervalSkip()
    {
        onMS_Coroutine = true;
        yield return null;

        MS_state = MS_State.NEXT;
        onMS_Coroutine = false;
    }

    private void MS_Next()
    {
        LogManager.instance.Add_LogMess(messageDisplay, message_name);

        if (TeManager.Increment_EventCounter()) //イベント終了
        {
            MS_NoDisplayWindow();
            MS_state = MS_State.STANDBY;

            return;
        }

        MS_state = MS_State.WAITING;
    }

    private void MS_Delete()
    {
        var eo = TeManager.Get_CullentEvent();

        MS_NoDisplayWindow();

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        MS_state = MS_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void MS_Window_Display()
    {
        var eo = TeManager.Get_CullentEvent();

        MS_DisplayWindow();

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        MS_state = MS_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private string[] MS_Split()
    {
        List<string> outList = new List<string>();
        string mass = TeManager.Get_CullentEvent().message;
        bool gmode = false;
        string buff = "";

        mass = ReplaceCode(mass);

        foreach (char c in mass)
        {
            if (c == '<')
            {
                gmode = true;
                buff += c;
            }
            else if (c == '>')
            {
                gmode = false;
                buff += c;
                outList.Add(EscCode(buff));
                buff = "";
            }
            else
            {
                buff += c;
                if (!gmode)
                {
                    outList.Add(buff);
                    buff = "";
                }
            }
        }

        return outList.ToArray();
    }

    private string EscCode(string buff)
    {
        switch (buff)
        {
            case @"<\n>":
                return "\n";
            default:
                return buff;
        }
    }
    
    private void EndCoding(string code)
    {
        //Debug.Log("EndCoding: " + code);
        switch (code)
        {
            case @"<b>":
                list_endcode.Add("</b>");
                break;
            case @"<i>":
                list_endcode.Add("</i>");
                break;
            case @"</b>":
                list_endcode.Remove("</b>");
                break;
            case @"</i>":
                list_endcode.Remove("</i>");
                break;
            case @"</size>":
                list_endcode.Remove("</size>");
                break;
            case @"</color>":
                list_endcode.Remove("</color>");
                break;
        }

        if (Regex.IsMatch(code, @"<size=([0-9]+)"))
        {
            list_endcode.Add("</size>");
        }

        if (Regex.IsMatch(code, @"<color=.+>"))
        {
            list_endcode.Add("</color>");
        }
    }

    private string Get_EndCode()
    {
        string list = "";
        
        for (int i = 0; i < list_endcode.Count; i++)
        {
            list += list_endcode[list_endcode.Count- 1 - i];
        }
        return list;
    }

    public static string ReplaceCode(string mass)
    {
        string outText = mass;

        //値を書き換える
        if (ValuesManager.instance != null)
        {
            MatchCollection match_values = Regex.Matches(mass, @"<v=\d{1,4}>");
            foreach (Match m in match_values)
            {
                string ms = "";
                for (int i = 3; i < m.Value.Length - 1; i++)
                {
                    ms += m.Value[i];
                }
                int index = int.Parse(ms);

                Regex regex = new Regex(m.Value);
                outText = regex.Replace(outText, ValuesManager.instance.Get_Value(index).ToString());
            }

            MatchCollection match_string = Regex.Matches(mass, @"<s=\d{1,4}>");
            foreach (Match m in match_string)
            {
                string ms = "";
                for (int i = 3; i < m.Value.Length - 1; i++)
                {
                    ms += m.Value[i];
                }
                int index = int.Parse(ms);

                Regex regex = new Regex(m.Value);
                outText = regex.Replace(outText, ValuesManager.instance.Get_Text(index));
            }

            MatchCollection match_float = Regex.Matches(mass, @"<f=\d{1,4}>");
            foreach (Match m in match_string)
            {
                string ms = "";
                for (int i = 3; i < m.Value.Length - 1; i++)
                {
                    ms += m.Value[i];
                }
                int index = int.Parse(ms);

                Regex regex = new Regex(m.Value);
                outText = regex.Replace(outText, ValuesManager.instance.Get_Value_Float(index).ToString());
            }
        }

        return outText;
    }
}
