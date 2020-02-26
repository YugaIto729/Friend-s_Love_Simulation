using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    public enum DM_State { STANDBY, SET, SET_DEFAULT, WAIT}
    public DM_State DM_state = DM_State.STANDBY;

    public GameObject Dialog_Prefab;

    private TalkEventManager TeManager;

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
                    break;
                case DM_State.SET_DEFAULT:
                    break;
                case DM_State.WAIT:
                    break;
            }
        }
    }

    private void Set_Default_Update()
    {
        var eo = TeManager.Get_CullentEvent();

        var o = Instantiate(Dialog_Prefab);
        //o.GetComponent<Dialog_Prefab>().Set_Default_Dialog()
    }

}
