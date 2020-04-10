using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;

public class EventCheckManager : MonoBehaviour
{
    public static EventCheckManager instance;
    private Dictionary<int, List<DateAndEvent>> scheduleDict = new Dictionary<int, List<DateAndEvent>>();
    private List<Add_Schedule_Save> add_Schedule_Saves = new List<Add_Schedule_Save>();

    public enum ECM_State
    {
        STANDBY, SET
    }

    public ECM_State ECM_state = ECM_State.STANDBY;
    private TalkEventManager TeManager;

    /// <summary>
    /// スケジュールファイル名
    /// </summary>
    public string fileName="";

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
        LoadSchedule(); //CSVファイルから読み込む

    }

    // Update is called once per frame
    void Update()
    {
        if (ECM_state != ECM_State.STANDBY)
        {
            switch (ECM_state)
            {
                case ECM_State.SET:
                    ECM_ScheduleSet();
                    break;
            }
        }

    }
    
    private void ECM_ScheduleSet()
    {
        Debug.Log("うわあああああ");
        var eo = TeManager.Get_CullentEvent();

        switch (eo.name)
        {
            case "TALK":
                Add_Event_Schdule(DateAndEvent_Type.TALK, eo.register2, eo.register1, eo.message, eo.register5);

                break;
            case "ENCO":
                Add_Encount_Schdule(eo.register2, eo.register1, eo.register5);

                break;
        }

        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        ECM_state = ECM_State.STANDBY;

        TeManager.Increment_EventCounter();
    }

    /// <summary>
    /// 指定した日付時間値のイベントを確認する
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public List<DateAndEvent> EventCheck(int date)
    {
        if (scheduleDict.ContainsKey(date))
        {
            return scheduleDict[date];
        }
        return new List<DateAndEvent>();
    }

    /// <summary>
    /// 指定された日付時間値のイベントを予約する。
    /// </summary>
    /// <param name="date"></param>
    public void DoEventCheck(int date)
    {
        if (scheduleDict.ContainsKey(date)) {

            foreach (var dae in scheduleDict[date])
            {
                if (dae.type == DateAndEvent_Type.TALK) //イベントタイプがTALK
                {
                    if (dae.parser.Eval(ValuesManager.instance.Get_Values())) //設定されている条件
                    {
                        TalkEventManager.instance.EventReservation(dae.eventPass);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 指定した日にちと時間のイベントを確認する
    /// </summary>
    /// <param name="day"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public List<DateAndEvent> EventCheck(int day, int time)
    {
        int date = (day - 1) * 6 + time;
        return EventCheck(date);

    }


    private void TestCSV(string fileName)
    {
        string debugS = "";

        var s = LoadTable(fileName);
        foreach (var sl in s)
        {
            foreach (string sh in sl)
            {
                debugS += sh + "\t";
            }

            debugS += "\n";
        }
        Debug.Log("table: \n" + debugS);
    }

    /// <summary>
    /// スケジュールを読み込む
    /// </summary>
    private void LoadSchedule()
    {
        string debugS = "";
        var sl = LoadTable(fileName);

        for (int i=1; i < sl.Count; i++)
        {
            for(int j = 2; j < sl[i].Length; j++)
            {

                debugS += sl[i][j] + "\t";
                if (sl[i][j] != "")
                {
                    //Debug.LogFormat("[date] {0}:{1}", i - 1, j - 2);
                    Decode(EraseSpace(sl[i][j]), i-1, j-2);
                }
            }

            debugS += "\n";
        }

        //Debug.Log("table: \n" + debugS);
    }

    private void Decode(string code, int date, int time)
    {
        Match match_talk = Regex.Match(code, @"!talk\(.+?\)");
        if (match_talk.Success)
        {
            string rs = match_talk.Value;

            string[] s = rs.Substring(6, rs.Length-1-6).Split(':');
            string eventName = string.Format("ev_{0}_{1}", date, time);

            //Debug.Log("pass: " + s[0]);

            if (s.Length == 1)
            {
                Decoding_talk(eventName, date, time, s[0], "0==0");
            }
            else if (s.Length == 2)
            {
                Debug.Log("parser: " + s[1]);
                Decoding_talk(eventName, date, time, s[0], s[1]);
            }

            //Debug.LogFormat("talk[ {0} ]", match_talk.Value);
        }

        //===============================================================================================

        Match match_enc = Regex.Match(code, @"!encount(\(.+?\)|)");
        if (match_enc.Success)
        {
            string rs = match_enc.Value;
            string eventName = string.Format("ev_{0}_{1}", date, time);

            if (rs.Length == 8)
            {
                Decoding_enco(eventName, time, date, "0==0");
            }
            else if(rs.Length > 8)
            {
                string s = rs.Substring(9, rs.Length - 1 - 9);
                //Debug.Log("parser: " + s);
                Decoding_enco(eventName, time ,date, s);
            }
            //Debug.LogFormat("encount[ {0} ]", match_enc.Value);
        }

    }

    /// <summary>
    /// セーブデータから追加分スケジュールを登録する
    /// </summary>
    /// <param name="schlist"></param>
    public void Load_Schedule_SavaData(List<Add_Schedule_Save> schlist)
    {
        foreach(var s in schlist)
        {
            Set_Schedule(s.date, s.eaEvent);
        }
    }

    /// <summary>
    /// イベントをスケジュールに登録する(入力が日にちと時間)
    /// </summary>
    /// <param name="type"></param>
    /// <param name="hID"></param>
    /// <param name="day"></param>
    /// <param name="time"></param>
    /// <param name="pass"></param>
    /// <param name="formale"></param>
    public void Add_Event_Schdule(DateAndEvent_Type type, int hID, int day, int time, string pass, string formale)
    {
        Add_Event_Schdule(type, hID, (day-1)*6+time, pass, formale);
    }

    /// <summary>
    /// イベントをスケジュールに登録する(入力が日付時間数)
    /// </summary>
    /// <param name="type">イベントの種類</param>
    /// <param name="hID">ヒロインの番号</param>
    /// <param name="date"></param>
    /// <param name="pass"></param>
    /// <param name="formale"></param>
    public void Add_Event_Schdule(DateAndEvent_Type type, int hID, int date, string pass, string formale)
    {
        Parser parser = new Parser(formale);

        DateAndEvent dateAndEvent = new DateAndEvent()
        {
            type = type,
            date = date,
            heroineID = hID,
            eventPass = pass,
            parser = parser,
        };

        Set_Schedule(date, dateAndEvent);
        add_Schedule_Saves.Add(new Add_Schedule_Save(){
            date = date,
            eaEvent = dateAndEvent
        });
    }

    /// <summary>
    /// 遭遇イベントをスケジュールに登録する
    /// </summary>
    /// <param name="hID"></param>
    /// <param name="date"></param>
    /// <param name="formale"></param>
    public void Add_Encount_Schdule(int hID, int date, string formale)
    {
        Add_Event_Schdule(DateAndEvent_Type.ENCOUNT, hID, date, "", formale);
    }

    /// <summary>
    /// 会話イベントをスケジュールに登録する
    /// </summary>
    /// <param name="hID"></param>
    /// <param name="date"></param>
    /// <param name="pass"></param>
    /// <param name="formale"></param>
    public void Add_Talk_Schdule(int hID, int date,string pass ,string formale)
    {
        Add_Event_Schdule(DateAndEvent_Type.TALK, hID, date, pass, formale);
    }



    //====================================================================================

    private void Decoding_talk(string tname ,int date,int id , string pass, string parserFormale)
    {
        Parser parser = new Parser(parserFormale);

        DateAndEvent dateAndEvent = new DateAndEvent()
        {
            type = DateAndEvent_Type.TALK,
            date = date,
            heroineID = id,
            eventPass = pass,
            parser = parser,
        };

        Set_Schedule(date, dateAndEvent);
    }

    private void Decoding_enco(string tname, int id, int date, string parserFormale)
    {
        Parser parser = new Parser(parserFormale);

        Debug.Log("EventType: " + DateAndEvent_Type.ENCOUNT);
        DateAndEvent dateAndEvent = new DateAndEvent()
        {
            type = DateAndEvent_Type.ENCOUNT,
            eventName = tname,
            heroineID = id,
            date = date,
            parser = parser,
        };

        Set_Schedule(date, dateAndEvent);
    }

    /// <summary>
    /// スケジュールを登録する
    /// </summary>
    /// <param name="date"></param>
    /// <param name="dateAndEvent"></param>
    private void Set_Schedule(int date, DateAndEvent dateAndEvent)
    {
        if (scheduleDict.ContainsKey(date))
        {
            scheduleDict[date].Add(dateAndEvent);
        }
        else
        {
            scheduleDict.Add(date, new List<DateAndEvent>() {dateAndEvent });
        }
    }

    /// <summary>
    /// 空白文字を除外する
    /// </summary>
    /// <param name="test"></param>
    /// <returns></returns>
    private string EraseSpace(string test)
    {
        string outText = "";

        foreach (char c in test)
        {
            if (!Regex.IsMatch(c.ToString(), @"\s"))
            {
                outText += c.ToString();
            }
        }

        return outText;
    }

    /// <summary>
    /// CSVを読み込む
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public List<string[]> LoadTable(string fileName)
    {
        TextAsset csvFile; // CSVファイル
        List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト;

        csvFile = Resources.Load("ScheduleCSV/" + fileName) as TextAsset; // Resouces下のCSV読み込み
        StringReader reader = new StringReader(csvFile.text);

        // , で分割しつつ一行ずつ読み込み
        // リストに追加していく
        while (reader.Peek() != -1) // reader.Peaekが-1になるまで
        {
            string line = reader.ReadLine(); // 一行ずつ読み込み
            csvDatas.Add(line.Split(',')); // , 区切りでリストに追加
        }

        // csvDatas[行][列]を指定して値を自由に取り出せる
        //Debug.Log(csvDatas[0][1]);

        return csvDatas;
    }


}

public enum DateAndEvent_Type
{
    NONE, TALK, ENCOUNT
}

public class DateAndEvent
{
    public string eventName = "";
    public DateAndEvent_Type type = DateAndEvent_Type.NONE;
    public int date = 0;

    /// <summary>
    /// TALK用、イベントパス
    /// </summary>
    public string eventPass = "";

    /// <summary>
    /// ENCOUNT用、ヒロインID
    /// </summary>
    public int heroineID = 0;

    /// <summary>
    /// 発生する条件
    /// </summary>
    public Parser parser;
}
