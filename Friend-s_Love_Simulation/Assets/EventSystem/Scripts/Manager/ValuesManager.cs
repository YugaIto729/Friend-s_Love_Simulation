using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ValuesManager : MonoBehaviour
{
    public static ValuesManager instance;

    private float[] values;
    private string[] texts;
    public VM_State VM_state = VM_State.STANDBY;

    private TalkEventManager TeManager;

    public enum VM_State
    {
        STANDBY, SET_VALUE, SET_TEXT,
    } 

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

        int[] vs = { 256, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
        Set_Values(Startup(255));
        string[] ts = { "愚者", "魔術師", "戦車", "女教皇", "皇帝" };
        Set_Texts(Startup_str(64));
    }

    private void Update()
    {
        if (VM_state != VM_State.STANDBY)
        {
            switch (VM_state)
            {
                case VM_State.SET_VALUE:
                    VM_Set_Value();
                    break;
                case VM_State.SET_TEXT:
                    VM_Set_Text();
                    break;

            }
        }
    }



    private void VM_Set_Value()
    {
        var eo = TeManager.Get_CullentEvent();

        var parser = eo.parsers[0];
        var v = parser.Eval_Value(values);
        
        if (!Set_Value(eo.register2, v))
        {
            Debug.LogErrorFormat("[ValusManager] 代入に失敗");
        }
        //Debug.Log("values[10]: " + values[10]);
        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        VM_state = VM_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void VM_Set_Text()
    {
        var eo = TeManager.Get_CullentEvent();

        Debug.LogFormat("[ValusManager] {0}番の文字列変数に{1}を代入", eo.register2, eo.message);

        if (!Set_Text(eo.register2, eo.message))
        {
            Debug.LogErrorFormat("[ValusManager] 代入に失敗");
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        VM_state = VM_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    /// <summary>
    /// 数値の初期化
    /// </summary>
    public void StartSetting(int valueMax, int stringMax)
    {
        Set_Values(Startup(valueMax));
        Set_Texts(Startup_str(stringMax));
    }


    public void Set_Values(float[] vs)
    {
        values = vs;
    }

    public bool Set_Value(int index, float value)
    {
        if (index < values.Length)
        {
            Debug.LogFormat("[ValusManager] {0}番の変数に{1}を代入", index, value);

            values[index] = value;
            return true;
        }
        return false;
    }

    public float[] Get_Values()
    {
        return values;
    }

    public int Get_Value(int index)
    {
        if (index < values.Length)
        {
            return (int)values[index];
        }
        else
        {
            return 0;
        }
    }

    public float Get_Value_Float(int index)
    {
        if (index < values.Length)
        {
            return values[index];
        }
        else
        {
            return 0;
        }
    }

    public void Set_Texts(string[] texts)
    {
        this.texts = texts;
    }

    public bool Set_Text(int index, string text)
    {
        if (index < texts.Length)
        {
            Debug.LogFormat("[ValusManager] {0}番の文字列変数に{1}を代入", index, text);

            texts[index] = text;
            return true;
        }
        return false;
    }

    public string[] Get_Texts()
    {
        return texts;
    }

    public string Get_Text(int index)
    {
        if (index < texts.Length)
        {
            return texts[index];
        }
        return "";
    }

    private float[] Startup(int max)
    {
        float[] vs = new float[max];

        for(int i=0; i < vs.Length - 1; i++)
        {
            vs[i] = 0;
        }

        return vs;
    }

    private string[] Startup_str(int max)
    {
        string[] vs = new string[max];

        for (int i = 0; i < vs.Length - 1; i++)
        {
            vs[i] = "";
        }

        return vs;
    }
}

