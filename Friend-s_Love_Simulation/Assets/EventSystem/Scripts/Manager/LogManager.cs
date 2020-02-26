using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class LogManager : MonoBehaviour
{
    public static LogManager instance;

    private List<LogData> logList = new List<LogData>();
    private const int logLimit = 40;
    private const int sizeFont = 40;

    [SerializeField]
    private GameObject LogWindow;

    [SerializeField]
    private GameObject display_Text;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Display_Log()
    {
        if (!LogWindow.activeSelf)
        {
            LogWindow.SetActive(true);
            LoadLog();
        }
    }

    public void NoDisplay_Log()
    {
        if (LogWindow.activeSelf)
        {
            LogWindow.SetActive(false);
        }
    }

    private void LoadLog()
    {
        RectTransform rect = display_Text.GetComponent<RectTransform>();
        float lenght = 0;
        string text = "";

        foreach(var l in logList)
        {
            switch (l.type)
            {
                case LogData.Type.MESSAGE:
                    lenght += sizeFont + 4;
                    text += string.Format("{0} 「{1}」\n",l.logName , l.logMessage);

                    break;
                case LogData.Type.SELECT:
                    {
                        int count = l.selectMess.Length;

                        lenght += count * (sizeFont + 4);
                        foreach(string s in l.selectMess)
                        {
                            if (s == l.logMessage)
                            {
                                text += string.Format(">\t{0}\n", s);
                            }
                            else
                            {
                                text += string.Format("\t{0}\n", s);
                            }
                        }
                    }
                    break;
            }
        }

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, lenght);
        display_Text.GetComponent<Text>().text = text;

    }

    public void Add_LogMess(string mess, string name)
    {
        var data = new LogData()
        {
            type = LogData.Type.MESSAGE,
            logMessage = mess,
            logName = name,
        };

        logList.Add(data);

        if (logList.Count > logLimit)
        {
            logList.RemoveAt(0);
        }
    }

    public void Add_LogSel(string[] select, int index)
    {
        var data = new LogData()
        {
            type = LogData.Type.SELECT,
            selectMess = select,
            logMessage = select[index],
        };

        logList.Add(data);

        if (logList.Count > logLimit)
        {
            logList.RemoveAt(0);
        }
    }
}

public class LogData
{
    public enum Type { MESSAGE, SELECT }
    public Type type = Type.MESSAGE;
    public string logName = "";
    public string logMessage = "";
    public string[] selectMess;

}
