using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    public enum DM_State { STANDBY, SET, SET_DEFAULT, WAIT, REMOVE}
    public DM_State DM_state = DM_State.STANDBY;
    public GameObject CanvasObject;

    public GameObject[] Dialog_Prefab;

    private TalkEventManager TeManager;
    private Dictionary<int, GameObject> DialogDict = new Dictionary<int, GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        TeManager = TalkEventManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (DM_state != DM_State.STANDBY)
        {
            switch (DM_state)
            {
                case DM_State.SET:
                    Set_Update();
                    break;
                case DM_State.SET_DEFAULT:
                    Set_Default_Update();
                    break;
                case DM_State.WAIT:
                    Wait_Update();
                    break;
                case DM_State.REMOVE:
                    Remove_Update();
                    break;
            }
        }
    }

    private void Set_Default_Update()
    {
        var eo = TeManager.Get_CullentEvent();
        GameObject o;


        if (!DialogDict.ContainsKey(eo.register2))
        {
            o = Instantiate(Dialog_Prefab[0]);

            DialogDict.Add(0, o);
        }
        else
        {
            o = DialogDict[0];
        }

        Dialog_Prefab prefab = o.GetComponent<Dialog_Prefab>();
        o.transform.parent = CanvasObject.transform;
        prefab.Set_Default_Dialog(eo.choices[0], eo.choices[1], eo.message);


        var r = Dialog_Prefab[0].transform.localPosition;
        o.transform.localPosition = new Vector3(r.x, r.y, r.z);


        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        DM_state = DM_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void Set_Update()
    {
        var eo = TeManager.Get_CullentEvent();
        GameObject o;

        if (eo.image_num < Dialog_Prefab.Length)
        {

            if (!DialogDict.ContainsKey(eo.register2))
            {
                o = Instantiate(Dialog_Prefab[eo.image_num]);
                
                DialogDict.Add(eo.register2, o);
            }
            else
            {
                o = DialogDict[eo.register2];
            }

            Dialog_Prefab prefab = o.GetComponent<Dialog_Prefab>();
            o.transform.parent = CanvasObject.transform;
            prefab.Set();

            if (eo.register1 == 1)
            {
                var r = Dialog_Prefab[eo.image_num].transform.localPosition;
                o.transform.localPosition = new Vector3(eo.image_point.x, eo.image_point.y, r.z);
            }
            else
            {
                var r = Dialog_Prefab[eo.image_num].transform.localPosition;
                o.transform.localPosition = new Vector3(r.x, r.y, r.z);

            }
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        DM_state = DM_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    private void Wait_Update()
    {
        var eo = TeManager.Get_CullentEvent();
        var parser = eo.parsers[0];

        if (parser.Eval(ValuesManager.instance.Get_Values()))
        {
            TeManager.cullentEvent = EventType.SYSTEMWAIT;
            DM_state = DM_State.STANDBY;

            TeManager.Increment_EventCounter();
        }
    }

    private void Remove_Update()
    {
        var eo = TeManager.Get_CullentEvent();
        if (DialogDict.ContainsKey(eo.register2))
        {
            var o = DialogDict[eo.register2];
            DialogDict.Remove(eo.register2);
            Destroy(o);
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        DM_state = DM_State.STANDBY;

        TeManager.Increment_EventCounter();
    }
}
