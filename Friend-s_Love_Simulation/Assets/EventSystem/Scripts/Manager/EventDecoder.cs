using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public sealed class EventDecoder : MonoBehaviour
{
    public static EventDecoder instance;

    private const string basefile = "MassageTexts/";
    public string[] textData;
    private List<EventObject> eventPages;
    private bool isBrunching = false;
    private List<EventObject> HeadSelect;

    private bool isOuted = false; //同期デコーダ用　出力許可
    private bool isError = false; //同期デコーダ用　エラー検出

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }

        HeadSelect = new List<EventObject>();
    }

    private void Start()
    {
        //Get_EventObjects("Test");
        
    }

    /// <summary>
    /// イベントを取得する
    /// </summary>
    /// <param name="eventPass"></param>
    /// <returns></returns>
    public List<EventObject> Get_EventObjects(string eventPass)
    {
        eventPages = new List<EventObject>();
       
        if (LoadTextLine(eventPass)) //ファイル読み込み試行
        {
            foreach (string s in textData) //一行ずつ読み取る
            {
                if (!Regex.IsMatch(s, @"^(#.*)"))
                {
                    Decode(SplitText(s));

                }
            }
            /*
            foreach (var e in eventPages)
            {
                Debug.Log("event: " + e.eventType);
            }
            */
        }

        return eventPages;
    }

    /// <summary>
    /// イベントをダイレクトで取得する
    /// </summary>
    /// <param name="eventPass"></param>
    /// <returns></returns>
    public List<EventObject> Get_EventObjects_D(string data)
    {
        eventPages = new List<EventObject>();

        textData = data.Split('\n');

        foreach (string s in textData) //一行ずつ読み取る
        {
            if (!Regex.IsMatch(s, @"^(#.*)"))
            {
                Decode(SplitText(s));

            }
        }

        return eventPages;
    }

    /// <summary>
    /// イベントを同期的に取得する
    /// </summary>
    /// <param name="eventPass"></param>
    public void Decode_EventObject_Sync(string eventPass)
    {
        eventPages = new List<EventObject>();
        StartCoroutine(C_Decode_EventObject_Sync(eventPass));
    }

    private IEnumerator C_Decode_EventObject_Sync(string eventPass)
    {
        if (LoadTextLine(eventPass)) //ファイル読み込み試行
        {
            foreach (string s in textData) //一行ずつ読み取る
            {
                yield return null;

                if (!Regex.IsMatch(s, @"^(#.*)"))
                {
                    Decode(SplitText(s));

                }
            }

            foreach (var e in eventPages)
            {
                Debug.Log("event: " + e.eventType);
            }
            isOuted = true;
            yield break;
        }
        isError = true;

    }

    public List<EventObject> Get_EventObjects_Sync()
    {
        if (isOuted)
        {
            isOuted = false;
            return eventPages;
        }
        return null;
    }

    public bool Get_Error_Sync()
    {
        return isError;
    }


    /// <summary>
    /// テキストをロードし、一行ごとのテキストデータを読み出す
    /// </summary>
    /// <param name="textName"></param>
    private bool LoadTextLine(string textName)
    {
        try
        {
            TextAsset textasset = Resources.Load(basefile + textName, typeof(TextAsset)) as TextAsset;
            string TextLines = textasset.text; //テキスト全体をstring型で入れる変数を用意して入れる
            textData = TextLines.Split('\n'); //Splitで一行づつを代入した1次配列を作成
        }
        catch
        {
            return false;
        }
        return true;
    }

    /// <summary> 正規表現を用いてタブとスペースで区切る </summary>
    /// <param name="text_event"></param>
    /// <returns></returns>
    private string[] SplitText(string text_event)
    {
        List<string> outTexts = new List<string>();
        string buff = "";
        bool gmode = false;

        foreach (char s in text_event)
        {
            if (s == '"') //ダブルクォーテーションで囲んだ物は
            {
                gmode = !gmode;

            }
            else if (Regex.IsMatch("" + s, @"\s") && !gmode) {
                if (buff != "")
                {
                    outTexts.Add(buff);
                   
                    buff = "";
                }
            } else {
                buff += s;
            }
        }

        if (buff != "")
        {
            outTexts.Add(buff);
        }

        return outTexts.ToArray();
    }

    private void Add_EventObject(EventObject @event)
    {
        eventPages.Add(@event);

        /*
        if (isBrunching)
        {
            Buf_EventObjects.Add(@event);
        }
        else
        {
            eventPages.Add(@event);
        }
        */
    }

    /// <summary>
    /// 命令を読み取り各命令オブジェクトに変換する
    /// </summary>
    /// <param name="eventText"></param>
    private void Decode(string[] eventText)
    {
        int time = eventText.Length;
        
        if (eventText.Length > 0)
        {
            //try
            {
                switch (eventText[0])
                {
                    case "MESS":
                        if (time == 3)
                        {
                            Decode_Message(eventText[2], eventText[1], 0);
                        }
                        else if (time == 2)
                        {
                            Decode_Message("NONE", eventText[1], 0);
                            //Debug.Log("Decode Mess" + eventText[1]);
                        }
                        else if (time == 4)
                        {
                            Decode_Message(eventText[2], eventText[1], int.Parse(eventText[3]));
                        }
                        break;
                    case "MESSOFF":
                        if (time == 1)
                        {
                            Decode_Mess_Delete();
                        }
                        break;
                    case "MESSON":
                        if (time == 1)
                        {
                            Decode_Mess_Display();
                        }
                        break;
                    case "EVENTEND":
                        if (time == 1)
                        {
                            Decode_EventEnd();
                        }
                        break;
                    case "IMG_SET":
                        if (time == 5)
                        {
                            Decode_ImageSet(int.Parse(eventText[1]), eventText[2], new Vector2(float.Parse(eventText[3]), float.Parse(eventText[4])), new Vector2(1, 1));
                        }
                        else if (time == 7)
                        {
                            Decode_ImageSet(int.Parse(eventText[1]), eventText[2], new Vector2(float.Parse(eventText[3]), float.Parse(eventText[4])), new Vector2(float.Parse(eventText[5]), float.Parse(eventText[6])));

                        }
                        break;
                    case "IMG_REMOVE":
                        if (time == 2)
                        {
                            Decode_ImageRemove(int.Parse(eventText[1]));
                        }
                        break;
                    case "IMG_MOVE":
                        if (time == 6)
                        {
                            Decode_ImageMove(int.Parse(eventText[1]), float.Parse(eventText[2]), new Vector2(float.Parse(eventText[3]), float.Parse(eventText[4])), int.Parse(eventText[5]));
                        }
                        break;
                    case "IMG_FADE":
                        if (time == 4)
                        {
                            Decode_ImageFade(int.Parse(eventText[1]), float.Parse(eventText[2]), int.Parse(eventText[3]));
                        }
                        break;
                    case "IMG_REV":
                        if (time == 2)
                        {
                            Decode_ImageReverse(int.Parse(eventText[1]));
                        }
                        break;
                    case "IMG_ROT":
                        if (time == 5)
                        {
                            Decode_ImageRoteto(int.Parse(eventText[1]), float.Parse(eventText[2]), float.Parse(eventText[3]), int.Parse(eventText[4]));
                        }
                        break;
                    case "SIMG_SET":
                        if (time == 6)
                        {
                            //1 1 4
                            int[] vs =
                            {
                            -1, int.Parse(eventText[2]), -1, int.Parse(eventText[3]), int.Parse(eventText[4]), int.Parse(eventText[5]), -1
                        };
                            Decode_StandImgSet(int.Parse(eventText[1]), vs, new Vector2(0, 0), new Vector2(0, 0), 0);
                        }
                        else if (time == 9)
                        {
                            //1 1 7 
                            int[] vs =
                            {
                            int.Parse(eventText[2]), int.Parse(eventText[3]), int.Parse(eventText[4]), int.Parse(eventText[5]), int.Parse(eventText[6]), int.Parse(eventText[7]), int.Parse(eventText[8])
                        };
                            Decode_StandImgSet(int.Parse(eventText[1]), vs, new Vector2(0, 0), new Vector2(0, 0), 1);

                        }
                        else if (time == 10)
                        {
                            //1 1 4 4
                            int[] vs =
                            {
                            -1, int.Parse(eventText[2]), -1, int.Parse(eventText[3]), int.Parse(eventText[4]), int.Parse(eventText[5]), -1
                        };
                            Decode_StandImgSet(int.Parse(eventText[1]), vs, new Vector2(float.Parse(eventText[6]), float.Parse(eventText[7])), new Vector2(float.Parse(eventText[8]), float.Parse(eventText[9])), 2);

                        }
                        else if (time == 13)
                        {
                            //1 1 7 4
                            int[] vs = { int.Parse(eventText[2]), int.Parse(eventText[3]), int.Parse(eventText[4]), int.Parse(eventText[5]), int.Parse(eventText[6]), int.Parse(eventText[7]), int.Parse(eventText[8]) };
                            Decode_StandImgSet(int.Parse(eventText[1]), vs, new Vector2(float.Parse(eventText[9]), float.Parse(eventText[10])), new Vector2(float.Parse(eventText[11]), float.Parse(eventText[12])), 3);

                        }
                        break;
                    case "SIMG_MOVE":
                        if (time == 6)
                        {
                            Decode_StandImgMove(int.Parse(eventText[1]), float.Parse(eventText[2]), new Vector2(float.Parse(eventText[3]), float.Parse(eventText[4])), int.Parse(eventText[5]));
                        }
                        break;
                    case "SIMG_FADE":
                        if (time == 4)
                        {
                            Decode_StandImgFade(int.Parse(eventText[1]), float.Parse(eventText[2]), int.Parse(eventText[3]));
                        }
                        break;
                    case "SIMG_FOCUS":
                        if (time == 3)
                        {
                            Decode_StandImgFocus(int.Parse(eventText[1]), int.Parse(eventText[2]));
                        }
                        break;
                    case "SIMG_REV":
                        if (time == 2)
                        {
                            Decode_StandImgReverse(int.Parse(eventText[1]));
                        }
                        break;
                    case "SIMG_ROT":
                        if (time == 5)
                        {
                            Decode_StandImgRoteto(int.Parse(eventText[1]), float.Parse(eventText[2]), float.Parse(eventText[3]), int.Parse(eventText[4]));
                        }
                        break;
                    case "SIMG_REMOVE":
                        if (time == 2)
                        {
                            Decode_StandImgRemove(int.Parse(eventText[1]));
                        }
                        break;
                    case "SIMG_ANIME":
                        if (time == 3)
                        {
                            Decode_StandImgAnime(int.Parse(eventText[1]), int.Parse(eventText[2]));
                        }
                        break;
                    case "SIMG_STOPANIME":
                        if (time == 2)
                        {
                            Decode_StandImgStopAni(int.Parse(eventText[1]));
                        }
                        break;
                    case "SIMG_DEPTH":
                        if (time == 3)
                        {
                            Decode_StandImg_Depth(int.Parse(eventText[1]), float.Parse(eventText[2]));
                        }
                        break;
                    case "SIMG_COLOR":
                        if (time == 6)
                        {
                            Decode_StandImg_Color(int.Parse(eventText[1]), new Color(float.Parse(eventText[2]), float.Parse(eventText[3]), float.Parse(eventText[4]), float.Parse(eventText[5])));
                        }
                        break;
                    case "SELECT":
                        if (time == 2)
                        {
                            Decode_Select(eventText[1], "0==0");
                        }
                        else if (time == 3)
                        {
                            Decode_Select(eventText[1], eventText[2]);
                        }
                        break;
                    case "SLELSE":
                        if (time == 2)
                        {
                            Decode_Select_else(eventText[1], "0==0");
                        }
                        else if (time == 3)
                        {
                            Decode_Select_else(eventText[1], eventText[2]);
                        }
                        break;
                    case "ENDSELECT":
                        if (time == 1)
                        {
                            Decode_Select_end();
                        }
                        break;
                    case "BRANCH":
                        if (time == 2)
                        {
                            Decode_Branch(eventText[1]);
                        }
                        break;
                    case "ELSEBRANCH":
                        if (time == 1)
                        {
                            Decode_Branch_else();
                        }
                        break;
                    case "ENDBRANCH":
                        if (time == 1)
                        {
                            Decode_Branch_end();
                        }
                        break;
                    case "LABEL":
                        if (time == 2)
                        {
                            Decode_Label(eventText[1]);
                        }
                        break;
                    case "GOTO":
                        if (time == 2)
                        {
                            Decode_Goto(eventText[1]);
                        }
                        break;
                    case "WAIT":
                        if (time == 2)
                        {
                            Decode_Wait(float.Parse(eventText[1]));
                        }
                        break;
                    case "FADE":
                        if (time == 4)
                        {
                            Color color;
                            switch (eventText[2])
                            {
                                case "BLACK":
                                    color = Color.black;
                                    break;
                                case "WHITE":
                                    color = Color.white;
                                    break;
                                default:
                                    color = Color.black;
                                    break;
                            }

                            int mode = 0;
                            switch (eventText[3])
                            {
                                case "WAIT":
                                    mode = 0;
                                    break;
                                case "NOWAIT":
                                    mode = 1;
                                    break;
                                default:
                                    mode = 0;
                                    break;
                            }

                            Decode_Fade(float.Parse(eventText[1]), color, mode);


                        }
                        else if (time == 7)
                        {
                            int mode;
                            switch (eventText[6])
                            {
                                case "WAIT":
                                    mode = 0;
                                    break;
                                case "NOWAIT":
                                    mode = 1;
                                    break;
                                default:
                                    mode = 0;
                                    break;
                            }

                            Color color = new Color(float.Parse(eventText[2]), float.Parse(eventText[3]), float.Parse(eventText[4]), float.Parse(eventText[5]));
                            Decode_Fade(float.Parse(eventText[1]), color, mode);
                        }
                        break;
                    case "BACKIMG_SET":
                        if (time == 7)
                        {
                            Decode_BackImg_Set(int.Parse(eventText[1]), eventText[2], new Vector2(float.Parse(eventText[3]), float.Parse(eventText[4])), new Vector2(float.Parse(eventText[5]), float.Parse(eventText[6])));
                        }
                        else if (time == 5)
                        {
                            Decode_BackImg_Set(int.Parse(eventText[1]), eventText[2], new Vector2(float.Parse(eventText[3]), float.Parse(eventText[4])), new Vector2(1, 1));
                        }
                        break;
                    case "BACKIMG_MOVE":
                        if (time == 6)
                        {
                            Decode_BackImg_Move(int.Parse(eventText[1]), float.Parse(eventText[2]), new Vector2(float.Parse(eventText[3]), float.Parse(eventText[4])), eventText[5]);
                        }
                        break;
                    case "BACKIMG_ROT":
                        if (time == 5)
                        {
                            Decode_BackImg_Rot(int.Parse(eventText[1]), float.Parse(eventText[2]), float.Parse(eventText[3]), eventText[4]);
                        }
                        break;
                    case "BACKIMG_REMOVE":
                        if (time == 2)
                        {
                            Decode_BackImg_Remove(int.Parse(eventText[1]));
                        }
                        break;
                    case "VALUE_SET":
                        if (time == 3)
                        {
                            Decode_Value_Set(int.Parse(eventText[1]), eventText[2]);
                        }
                        break;
                    case "TEXT_SET":
                        if (time == 3)
                        {
                            Decode_Value_T_Set(int.Parse(eventText[1]), eventText[2]);
                        }
                        break;
                    case "BGMPLAY":
                        if (time == 3)
                        {
                            Decode_BGM_Play(eventText[1], float.Parse(eventText[2]));
                        }
                        break;
                    case "BGMFADE":
                        if (time == 5)
                        {
                            int mode = 0;
                            switch (eventText[4])
                            {
                                case "NOWAIT":
                                    mode = 1;
                                    break;
                            }

                            Decode_BGM_Fade(eventText[1], float.Parse(eventText[2]), float.Parse(eventText[3]), mode);
                        }
                        break;
                    case "BGMVOL":
                        if (time == 3)
                        {
                            Decode_BGM_Volume(int.Parse(eventText[1]), float.Parse(eventText[2]));
                        }
                        break;
                    case "BGMSTOP":
                        if (time == 1)
                        {
                            Decode_BGM_Stop();
                        }
                        break;
                    case "BGMCONT":
                        if (time == 1)
                        {
                            Decode_BGM_Continue();
                        }
                        break;
                    case "SOUND":
                        if (time == 4)
                        {
                            Decode_SoundPlay(eventText[1], float.Parse(eventText[2]), float.Parse(eventText[3]));
                        }
                        break;
                    case "DIALOG_SET":
                        if (time == 3)
                        {
                            Decode_Dialog_Set(int.Parse(eventText[1]), int.Parse(eventText[2]), new Vector2(), 0);
                        }
                        else if (time == 5)
                        {
                            Decode_Dialog_Set(int.Parse(eventText[1]), int.Parse(eventText[2]), new Vector2(float.Parse(eventText[3]), float.Parse(eventText[4])), 1);
                        }
                        break;
                    case "DIALOG_WAIT":
                        if (time == 2)
                        {
                            Decode_Dialog_Wait(eventText[1]);
                        }
                        break;
                    case "DIALOG_REMOVE":
                        if (time == 2)
                        {
                            Decode_Dialog_Remove(int.Parse(eventText[1]));
                        }
                        break;
                    case "DIALOG":
                        if (time == 3)
                        {
                            Decode_Dialog_Def(eventText[1], eventText[2], "");
                        }
                        else if (time == 4)
                        {
                            Decode_Dialog_Def(eventText[1], eventText[2], eventText[3]);
                        }
                        break;
                    case "ELSE_DIALOG":
                        if (time == 1)
                        {
                            Decode_Dialog_Def_else();
                        }
                        break;
                    case "END_DIALOG":
                        if (time == 1)
                        {
                            Decode_Dialog_Def_end();
                        }
                        break;
                    case "SCH_SET":
                        if (time == 6)
                        {
                            Decode_Schdule_Set(eventText[1], int.Parse(eventText[2]), int.Parse(eventText[3]), eventText[4], eventText[5]);
                        }
                        else if(time == 5)
                        {
                            Decode_Schdule_Set(eventText[1], int.Parse(eventText[2]), int.Parse(eventText[3]), eventText[4], "0==0");

                        }
                        break;
                    case "SCH_TALK_SET":
                        if (time == 5)
                        {
                            Decode_Schdule_Set("TALK", int.Parse(eventText[1]), int.Parse(eventText[2]), eventText[3], eventText[4]);

                        }
                        else if(time == 4)
                        {
                            Decode_Schdule_Set("TALK", int.Parse(eventText[1]), int.Parse(eventText[2]), eventText[3], "0==0");

                        }
                        break;
                    case "SCH_ENCO_SET":
                        if (time == 4)
                        {
                            Decode_Schdule_Set("ENCO", int.Parse(eventText[1]), int.Parse(eventText[2]), "", eventText[3]);

                        }
                        else if (time == 3)
                        {
                            Decode_Schdule_Set("ENCO", int.Parse(eventText[1]), int.Parse(eventText[2]), "", "0==0");

                        }
                        break;
                }
            }
            //catch
            {

            }
        }
    }



    //=================================================================================

    #region メッセージ系

    /// <summary>
    /// メッセージの命令オブジェクトを追加する
    /// </summary>
    private void Decode_Message(string name, string massage, int m_mode)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.MESSAGE,
            message = massage,
            register1 = m_mode,
        };

        if (name == "NONE") {
            eventPage.name = "";
        } else {
            eventPage.name = name;
        }
        Add_EventObject(eventPage);
    }

    private void Decode_Mess_Delete()
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.MESS_DELETE,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_Mess_Display()
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.MESS_DISPLAY,
        };

        Add_EventObject(eventPage);
    }

    #endregion

    #region システム系
    private void Decode_Wait(float seconds)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.WAIT,
            seconds = seconds,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_EventEnd()
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.EVENTEND
        };

        Add_EventObject(eventPage);
    }

    private void Decode_Fade(float seconds, Color color, int mode)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.FADE,
            seconds = seconds,
            color = color,
            register1 = mode,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_BackImg_Set(int num, string pass, Vector2 point, Vector2 size)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.BACK_SET,
            register2 = num,
            name = pass,
            image_point = point,
            image_scale = size,
        };

        Add_EventObject(eventPage);

    }

    private void Decode_BackImg_Move(int num, float seconds, Vector2 point, string mode)
    {
        int m;
        switch (mode)
        {
            case "WAIT_ADD":
                m = 0;
                break;
            case "NOWAIT_ADD":
                m = 1;
                break;
            case "WAIT_TARGET":
                m = 2;
                break;
            case "NOWAIT_TARGET":
                m = 3;
                break;
            default:
                m = 0;
                break;
        }

        EventObject eventPage = new EventObject
        {
            eventType = EventType.BACK_MOVE,
            register1 = m,
            register2 = num,
            seconds = seconds,
            image_point = point,
        };

        Add_EventObject(eventPage);

    }

    private void Decode_BackImg_Rot(int num, float seconds, float rot, string mode)
    {
        int m;
        switch (mode)
        {
            case "WAIT":
                m = 0;
                break;
            case "NOWAIT":
                m = 1;
                break;
            default:
                m = 0;
                break;
        }

        EventObject eventPage = new EventObject
        {
            eventType = EventType.BACK_ROT,
            seconds = seconds,
            register1 = m,
            register2 = num,
            register3 = rot,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_BackImg_Remove(int num)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.BACK_REMOVE,
            register2 = num,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_Value_Set(int num, string formale)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.VALUE_SET,
            register2 = num,
            message = formale,
            parsers = new List<Parser>(),
        };
        Parser parser = new Parser();
        parser.Start_Value(formale);
        eventPage.parsers.Add(parser);

        Add_EventObject(eventPage);
    }

    private void Decode_Value_T_Set(int num, string text)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.VALUE_S_SET,
            register2 = num,
            message = text,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_Schdule_Set(string type, int heroineID, int date, string pass, string formale)
    {

        EventObject eventPage = new EventObject
        {
            eventType = EventType.SCH_SET,
            name = type,
            register2 = heroineID,
            register1 = date,
            message = pass,
            register5 = formale,
        };

        Add_EventObject(eventPage);
    }

    #endregion
    #region 画像系
    /// <summary>
    /// 画像設置の命令オブジェクトを追加する
    /// </summary>
    /// <param name="num"></param>
    /// <param name="num_sp"></param>
    /// <param name="point"></param>
    private void Decode_ImageSet(int num, string sp, Vector2 point, Vector2 size)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.IMAGE_SET,
            image_num = num,
            name = sp,
            image_point = point,
            image_scale = size
        };

        Add_EventObject(eventPage);
    }

    /// <summary>
    /// 画像削除の命令オブジェクトを追加する
    /// </summary>
    /// <param name="num"></param>
    private void Decode_ImageRemove(int num)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.IMAGE_REMOVE,
            image_num = num
        };

        Add_EventObject(eventPage);
    }

    /// <summary>
    /// 画像を移動の命令オブジェクトを追加する
    /// </summary>
    /// <param name="num"></param>
    /// <param name="point"></param>
    /// <param name="mode"></param>
    private void Decode_ImageMove(int num, float seconds, Vector2 point, int mode)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.IMAGE_MOVE,
            image_num = num,
            image_point = point,
            register1 = mode,
            seconds = seconds
        };

        Add_EventObject(eventPage);
    }

    /// <summary>
    /// 画像のフェードイン・アウトの命令オブジェクトを追加する
    /// </summary>
    /// <param name="num"></param>
    /// <param name="seconds"></param>
    /// <param name="mode"></param>
    private void Decode_ImageFade(int num, float seconds, int mode)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.IMAGE_FADE,
            image_num = num,
            register1 = mode,
            seconds = seconds
        };

        Add_EventObject(eventPage);
    }

    /// <summary>
    /// 画像を反転させる命令オブジェクトを追加する
    /// </summary>
    /// <param name="num"></param>
    private void Decode_ImageReverse(int num)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.IMAGE_REVERSE,
            image_num = num
        };

        Add_EventObject(eventPage);
    }

    /// <summary>
    /// 画像を回転させる命令オブジェクトを追加する
    /// </summary>
    /// <param name="num"></param>
    /// <param name="rot"></param>
    private void Decode_ImageRoteto(int num, float time,  float rot, int mode)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.IMAGE_ROT,
            image_num = num,
            seconds = time,
            register1 = mode,
            register3 = rot
        };

        Add_EventObject(eventPage);
    }
    #endregion
    #region 立ち絵系
    /// <summary>
    /// 立ち絵を生成する命令オブジェクトを追加する
    /// </summary>
    /// <param name="num"></param>
    /// <param name="sprite_num"></param>
    /// <param name="poss"></param>
    /// <param name="size"></param>
    /// <param name="smode"></param>
    private void Decode_StandImgSet(int num, int[] sprite_num, Vector2 poss, Vector2 size, int smode)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.SIMG_SET,
            image_num = num,
            sprite_num = sprite_num, 
            image_point = poss,
            image_scale = size,
            register2 = smode,
            //0: time=6
            //1: time=9
            //2: time=10
            //3: time=13
        };
        Add_EventObject(eventPage);
    }

    /// <summary>
    /// 立ち絵を移動させる命令オブジェクトを追加する
    /// </summary>
    /// <param name="num"></param>
    /// <param name="seconds"></param>
    /// <param name="point"></param>
    /// <param name="mode"></param>
    private void Decode_StandImgMove(int num, float seconds, Vector2 point, int mode)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.SIMG_MOVE,
            image_num = num,
            image_point = point,
            register1 = mode,
            seconds = seconds
        };

        Add_EventObject(eventPage);
    }

    /// <summary>
    /// 立ち絵をフェードイン/フェードアウトさせる命令オブジェクトを追加する
    /// </summary>
    /// <param name="num"></param>
    /// <param name="seconds"></param>
    /// <param name="mode"></param>
    private void Decode_StandImgFade(int num, float seconds,  int mode)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.SIMG_FADING,
            image_num = num,
            register1 = mode,
            seconds = seconds
        };

        Add_EventObject(eventPage);
    }

    private void Decode_StandImgFocus(int num, int mode)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.SIMG_FOCUSING,
            image_num = num,
            register1 = mode,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_StandImgReverse(int num)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.SIMG_REVERSE,
            image_num = num
        };

        Add_EventObject(eventPage);
    }

    private void Decode_StandImgRoteto(int num, float time, float rot, int mode)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.SIMG_ROT,
            image_num = num,
            seconds = time,
            register1 = mode,
            register3 = rot
        };

        Add_EventObject(eventPage);
    }

    private void Decode_StandImgRemove(int num)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.SIMG_REMOVE,
            image_num = num
        };

        Add_EventObject(eventPage);
    }

    private void Decode_StandImgAnime(int num, int index)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.SIMG_ANIME,
            image_num = num,
            register2 = index,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_StandImgStopAni(int num)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.SIMG_STANIM,
            image_num = num,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_StandImg_Depth(int num, float z)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.SIMG_DEPTH,
            image_num = num,
            register3 = z,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_StandImg_Color(int num, Color color)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.SIMG_COLOR,
            image_num = num,
            color = color,
        };

        Add_EventObject(eventPage);
    }
    #endregion
    #region 分岐系

    private void Decode_Select(string message, string condition)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.SELECT,
            choices = new List<string> { message},
            parsers = new List<Parser> { new Parser() }, //条件式
            choiceEvents = new List<EventObject>(), //選択肢の基点イベントを入れていく
            
        };

        eventPage.parsers[0].Start(condition); //事前解析を行う
        eventPage.choiceEvents.Add(eventPage); //基点イベントに自分を追加する

        Add_EventObject(eventPage);
        HeadSelect.Add(eventPage); //ヘッダーイベントに設定
    }

    private void Decode_Select_else(string message, string condition)
    {

        EventObject eventPage = new EventObject
        {
            eventType = EventType.SLELSE,
            choiceEvents = new List<EventObject> { HeadSelect[HeadSelect.Count - 1] },　//ヘッダー(親)に自分を追加
        };

        if (HeadSelect[HeadSelect.Count - 1].eventType == EventType.SELECT)　//イベントタイプがSELECT
        {
            HeadSelect[HeadSelect.Count - 1].choices.Add(message); //ヘッダーイベント

            Parser parser = new Parser();
            parser.Start(condition);
            HeadSelect[HeadSelect.Count - 1].parsers.Add(parser); //条件式をヘッダーイベントに追加

            HeadSelect[HeadSelect.Count - 1].choiceEvents.Add(eventPage); //このイベントをヘッダーの基点イベントリストに追加

            Add_EventObject(eventPage);
        }
    }

    private void Decode_Select_end()
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.ENDSELECT,
            choiceEvents = new List<EventObject> { HeadSelect[HeadSelect.Count - 1] }, //ヘッダー(親)に自分を追加
        };

        if (HeadSelect[HeadSelect.Count - 1].eventType == EventType.SELECT)
        {

            Add_EventObject(eventPage);
            HeadSelect[HeadSelect.Count - 1].choiceEvents.Add(eventPage); //ヘッダーイベントに基点イベントを追加(末尾イベント)
            HeadSelect.RemoveAt(HeadSelect.Count - 1); //ヘッダーイベントをリストから削除、階層を上げる
        }
        
    }

    private void Decode_Branch(string condition)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.BRANCH,
            parsers = new List<Parser>() { new Parser()},
            choiceEvents = new List<EventObject>()
        };
        eventPage.parsers[0].Start(condition); //事前解析を行う
        eventPage.choiceEvents.Add(eventPage); //基点イベントに自分を追加する

        Add_EventObject(eventPage);
        HeadSelect.Add(eventPage);
    }

    private void Decode_Branch_else()
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.ELSEBRANCH,
            choiceEvents = new List<EventObject> { HeadSelect[HeadSelect.Count - 1] },
        };

        if (HeadSelect[HeadSelect.Count - 1].eventType == EventType.BRANCH)
        {
            HeadSelect[HeadSelect.Count - 1].choiceEvents.Add(eventPage); //このイベントをヘッダーの基点イベントリストに追加
            Add_EventObject(eventPage);
        }

    }

    private void Decode_Branch_end()
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.ENDBRANCH,
            choiceEvents = new List<EventObject> { HeadSelect[HeadSelect.Count - 1] },
        };

        if (HeadSelect[HeadSelect.Count - 1].eventType == EventType.BRANCH)
        {

            Add_EventObject(eventPage);
            HeadSelect[HeadSelect.Count - 1].choiceEvents.Add(eventPage); //このイベントをヘッダーの基点イベントリストに追加
            HeadSelect.RemoveAt(HeadSelect.Count - 1); //ヘッダーイベントをリストから削除、階層を下げる
        }

    }

    private void Decode_Label(string label)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.LABEL,
            message = label,
        };
        Add_EventObject(eventPage);
    }

    private void Decode_Goto(string label)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.GOTO,
            message = label,
        };
        Add_EventObject(eventPage);
    }
    #endregion
    #region 音系

    private void Decode_BGM_Play(string pass, float pitch)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.BGMPLAY,
            name = pass,
            register3 = pitch,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_BGM_Fade(string pass, float pitch, float seconds , int mode)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.BGMFADE,
            seconds = seconds,
            register1 = mode,
            name = pass,
            register3 = pitch,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_BGM_Volume(int num, float volume)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.BGMPLAY,
            register3 = volume,
        };

        //Add_EventObject(eventPage);
    }

    private void Decode_BGM_Stop()
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.BGMSTOP,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_BGM_Continue()
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.BGMCONT,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_SoundPlay(string pass, float pitch, float volume)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.SOUND,
            name = pass,
            register3 = pitch,
            register4 = volume,
        };

        Add_EventObject(eventPage);
    }
    #endregion
    #region ダイアログ系
    private void Decode_Dialog_Set(int num, int num_dialog, Vector2 point, int mode)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.DIALOG_SET,
            register1 = mode,
            register2 = num,
            image_num = num_dialog,
            image_point = point
        };

        Add_EventObject(eventPage);
    }

    private void Decode_Dialog_Wait(string f)
    {
        Parser parser = new Parser(f);

        EventObject eventPage = new EventObject
        {
            eventType = EventType.DIALOG_WAIT,
            parsers = new List<Parser>() { parser },
        };

        Add_EventObject(eventPage);
    }

    private void Decode_Dialog_Remove(int num)
    {
        EventObject eventPage = new EventObject
        {
            eventType = EventType.DIALOG_REMOVE,
            register2 = num,
        };

        Add_EventObject(eventPage);
    }

    private void Decode_Dialog_Def(string mess, string select1, string select2)
    {
        int m = 0;
        if (select1 == "" || select2 == "")
        {
            m = 1;
        }

        EventObject eventPage = new EventObject
        {
            eventType = EventType.DIALOG_DEFUALT,
            message = mess,
            choices = new List<string>() { select1, select2 }
        };

        Add_EventObject(eventPage);
        //========================================================
        Decode_Value_Set(121, "0");
        Decode_Dialog_Wait("[121] != 0");
        Decode_Dialog_Remove(0);
        if (m == 0)
        {
            Decode_Branch("[121]==1");
        }
    }

    private void Decode_Dialog_Def_else()
    {
        Decode_Branch_end();
        Decode_Branch("[121]==2");
    }

    private void Decode_Dialog_Def_end()
    {
        Decode_Branch_end();
    }

    #endregion 



    //================================================================================================

}


public enum EventType
{
    STANDBY = 0,
    SYSTEMWAIT = 1,
    WAIT = 3, EVENTEND = 4,
    LABEL = 5, GOTO = 6,
    FADE = 7,
    SCH_SET = 8,
    IMAGE_SET = 10, IMAGE_REMOVE, IMAGE_MOVE, IMAGE_FADE, IMAGE_REVERSE,IMAGE_ROT, 
    BGMPLAY = 20, BGMFADE ,BGMVOL, BGMSTOP, BGMCONT,
    SOUND = 30,
    SELECT = 40, SLELSE, ENDSELECT, BRANCH, ELSEBRANCH, ENDBRANCH,
    SIMG_SET = 50, SIMG_REMOVE, SIMG_MOVE, SIMG_ROT, SIMG_FOCUSING, SIMG_FADING, SIMG_REVERSE, SIMG_ANIME, SIMG_STANIM, SIMG_DEPTH, SIMG_COLOR,
    MESSAGE = 70, MESS_DELETE, MESS_DISPLAY,
    BACK_SET = 80, BACK_MOVE, BACK_ROT, BACK_REMOVE, BACK_DEPTH,
    VALUE_SET = 90, VALUE_S_SET,
    DIALOG_SET = 100, DIALOG_WAIT, DIALOG_REMOVE, DIALOG_DEFUALT,

}

public class EventObject
{
    /// <summary> イベントの種類 </summary>
    public EventType eventType = 0;
    public int register1 = 0;
    public int register2 = 0;
    public float seconds = 0;
    public float register3 = 0;
    public float register4 = 0;
    public string register5 = "";

    //=====メッセージ===========================================

    /// <summary> 名前 </summary>
    public string name = "";
    /// <summary> メッセージ </summary>
    public string message = "";

    //=====イメージ=============================================

    /// <summary> 画像 </summary>
    public Sprite sprite;
    /// <summary> 管理番号 </summary>
    public int  image_num = 0;
    /// <summary> 座標 </summary>
    public Vector2 image_point;
    /// <summary> サイズ </summary>
    public Vector2 image_scale;
    /// <summary> 色 </summary>
    public Color color;

    //=====選択肢===============================================

    /// <summary> 選択肢 </summary>
    public List<string> choices;
    /// <summary> 選択肢を選んだ時のイベント </summary>
    public List<EventObject> choiceEvents;
    /// <summary> 発動条件 </summary>
    public List<Parser> parsers;


    //=====ヒロインの立ち絵=====================================

    /// <summary> 画像番号 </summary>
    public int[] sprite_num;
    /// <summary> アニメーションタグ </summary>


}


