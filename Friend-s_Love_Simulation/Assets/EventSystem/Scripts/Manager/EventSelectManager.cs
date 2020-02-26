using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class EventSelectManager : MonoBehaviour
{
    public static EventSelectManager instance;

    public enum SELE_State
    {
        STANDBY, WAITING, DISPLAYING, BUTTON_EFFECT, PUSH_WAITING, NEXT, AUTO_PUSH, SETTING
    }

    public SELE_State SEL_state = SELE_State.STANDBY;
    public GameObject CanvasObject;
    public GameObject prefab_Select;

    private EventObject cullentEvent;
    private List<Select_Button_Prefab> select_Button_s;

    private int selected = -1;

    private TalkEventManager TeManager;
    //private int[] testValues = {0,1,2,3,4,5,6,7,8,9 };

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
        TeManager = TalkEventManager.instance;
        select_Button_s = new List<Select_Button_Prefab>();



    }

    // Update is called once per frame
    void Update()
    {
        if (SEL_state != SELE_State.STANDBY)
        {
            switch (SEL_state)
            {
                case SELE_State.WAITING:
                    SEL_Waiting();
                    break;
                case SELE_State.DISPLAYING:
                    SEL_Displaying();
                    break;
                case SELE_State.PUSH_WAITING:
                    SEL_Push_Waiting();
                    break;
                case SELE_State.BUTTON_EFFECT:
                    SEL_Select_Effect();
                    break;
                case SELE_State.NEXT:
                    SEL_Next();
                    break;
                case SELE_State.SETTING:
                    break;
            }
        }
    }

    private void SEL_Waiting()
    {
        cullentEvent = TeManager.Get_CullentEvent();
        int select_id = 0; //ID

        for (int i=0; i < cullentEvent.choices.Count; i++)
        {
            bool eval;
            if (ValuesManager.instance != null)
            {
                eval = cullentEvent.parsers[i].Eval(ValuesManager.instance.Get_Values());
            }
            else
            {
                eval = false;
            }

            //Debug.Log("エラーコード: " + cullentEvent.parsers[i].errorCode);
            //Debug.Log("結果: "+eval);

            GameObject o = Instantiate(prefab_Select);
            o.transform.parent = CanvasObject.transform;
            o.transform.localPosition = new Vector3(0, i * -68, 0);
            if (!eval)
            {
                o.SetActive(false);
            }

            Select_Button_Prefab select = o.GetComponent<Select_Button_Prefab>();
            select.Set_Prefab(cullentEvent.choices[i], select_id++); //初期設定

            select_Button_s.Add(select); //リストに設定
            
        }

        SEL_state = SELE_State.DISPLAYING;
    }

    private void SEL_Displaying()
    {
        SEL_state = SELE_State.PUSH_WAITING;
    }

    private void SEL_Push_Waiting()
    {
        if (selected != -1)
        {
            SEL_state = SELE_State.BUTTON_EFFECT;
            select_Button_s[selected].PlayAnimation();
        }
    }

    private void SEL_Select_Effect()
    {
        var sbp = select_Button_s[selected];


        if (!sbp.Check_AnimeState("button_push_animation"))
        {
            while (select_Button_s.Count > 0)
            {
                var so = select_Button_s[0].gameObject;
                select_Button_s.RemoveAt(0);
                Destroy(so);
            }

            SEL_state = SELE_State.NEXT;
        }
    }

    private void SEL_Next()
    {
        LogManager.instance.Add_LogSel(cullentEvent.choices.ToArray(), selected);
        TeManager.Skip_Select(cullentEvent, selected);
        Reset_SelectM();
    }

    /// <summary>
    /// 選択肢を実行する
    /// </summary>
    /// <param name="button_Prefab"></param>
    public void Action_Select(Select_Button_Prefab button_Prefab)
    {
        if (select_Button_s.Contains(button_Prefab))
        {
            selected = button_Prefab.GetID();
        }
        else
        {
            Debug.Log("失敗");
        }
    }

    /// <summary>
    /// EventSelectManagerをリセットする
    /// </summary>
    public void Reset_SelectM()
    {
        cullentEvent = null;
        select_Button_s.Clear();
        selected = -1;
        SEL_state = SELE_State.STANDBY;
    }

}
