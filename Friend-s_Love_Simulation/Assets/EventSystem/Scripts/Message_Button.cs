using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class Message_Button : MonoBehaviour
{
    private EventWindowManager EwManager;
    public enum State { Auto, Skip, Log, OffLog, Menu }
    public State state;

    [SerializeField]
    private Toggle toggle;

    private void Start()
    {
        EwManager = EventWindowManager.instance;
    }

    private void Update()
    {

    }

    public void Auto_Button()
    {
        EwManager.message_auto = !EwManager.message_auto;
        EwManager.message_skip = false;
        //if (toggle.isOn) { toggle.isOn = false; GetComponent<Toggle>().isOn = true; }
        
        //Debug.Log("Auto");
    }

    public void Skip_Button()
    {
        
        EwManager.message_skip = !EwManager.message_skip;
        EwManager.message_auto = false;
        //if (toggle.isOn) { toggle.isOn = false; GetComponent<Toggle>().isOn = true; }

        //Debug.Log("Skip");
    }

    public void Log_Button()
    {
        //Debug.Log("Log");
        LogManager.instance.Display_Log();
    }

    public void LogOff_Button()
    {
        //Debug.Log("OFFLog");
        LogManager.instance.NoDisplay_Log();
    }

    public void Menu_Button()
    {
        Debug.Log("Menu");
    }
}
