using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ValuesManager : MonoBehaviour
{
    public static ValuesManager instance;

    private int[] values;
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
        Set_Texts(ts);
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


    public void Set_Values(int[] vs)
    {
        values = vs;
    }

    public bool Set_Value(int index, int value)
    {
        Debug.LogFormat("[ValusManager] {0}番の変数に{1}を代入", index, value);


        if (index < values.Length)
        {

            values[index] = value;
            return true;
        }
        return false;
    }

    public int[] Get_Values()
    {
        return values;
    }

    public int Get_Value(int index)
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

    private int[] Startup(int max)
    {
        int[] vs = new int[max];

        for(int i=0; i < vs.Length - 1; i++)
        {
            vs[i] = 0;
        }

        return vs;
    }
}
