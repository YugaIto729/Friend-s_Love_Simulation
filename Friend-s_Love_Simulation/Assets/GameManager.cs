using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //public List<ScriptEvent> EventSouceList;
    //public List<Sprite> ImagesList;
    public Item_DataBase database_item;

    [HideInInspector]
    public List<ItemData> itemsList;

    public GamaState gameState = GamaState.TITLE;
    public StatusData statusData = new StatusData();
    //====================================================

    /// <summary>
    /// はじめからを選択した時のイベント
    /// </summary>
    private const string eventPass_Start = "ev_start";
    private const string eventPass_name_input = "ev_name_input";

    private string[] sceneName =
    {
        "MAIN", "Massage_Scene"
    };

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

        Read_DataBase();　//データベース読み込み
        //Starter();
    }

    // Start is called before the first frame update
    void Start()
    {
        /*foreach(var i in statusData.itemList)//foreach文を利用することで、ItemListの中を順番に取り出すことが出来る
        {
            //string a = i.name;
        }
        statusData.itemList[0].Use();*/
    }

    // Update is called once per frame
    void Update()
    {
        ChangeScene_Update();
    }

    /// <summary>
    /// データベースを読み込む
    /// </summary>
    private void Read_DataBase()
    {
        
        //var RID = Resources.Load("ScriptableObjects/Image_Data", typeof(Image_Data)) as Image_Data;
        //var RSE = Resources.Load("ScriptableObjects/EventSouce_Data", typeof(EventSouce_Data)) as EventSouce_Data;
        //var RIT = Resources.Load("ScriptableObjects/Item_Data Base", typeof(Item_DataBase)) as Item_DataBase;

        //EventSouceList = RSE.Get_EventSouce();
        //ImagesList = RID.Get_Images();
        //itemsList = RIT.Get_Items();

        itemsList = database_item.Get_Items();

    }

    private void Starter()
    {
        foreach (string s in sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(s);
            if (!scene.IsValid())
            {
                SceneManager.LoadScene(s, LoadSceneMode.Additive);
            }
        }
    }

    /// <summary>
    /// ゲームロード処理
    /// </summary>
    public void GameLoad()
    {
        //セーブデータをロード
    }

    /// <summary>
    /// ゲーム初期化処理
    /// </summary>
    public void GameStartSetting()
    {
        //ValueManagerの数値を初期化
        ValuesManager.instance.StartSetting(256, 64);
        //ステータスデータを初期化する
        statusData = new StatusData();
    }

    /// <summary>
    /// ゲーム開始処理
    /// </summary>
    public void GameStarting()
    {
        //ロードまたは初期化後に呼び出すメソッド

        //現在の時間帯を確認
        int date = ValuesManager.instance.Get_Value(1);
        CollPart_date(date);

    }

    /// <summary>
    /// シーンを呼び出す
    /// </summary>
    public void CollPart_date(int date)
    {
        int time = date % 6;

        switch (time)
        {
            case 0: //朝(学校パート直前)
            //case 1: //午前休み時間
            //case 2: //昼休み
            //case 3: //午後休み時間
                gameState = GamaState.WAITING_SCHOOL;
                break;
            case 4: //放課後(家パート直前)
                gameState = GamaState.WAITING_HOUSE;
                break;
            case 5: //夜(家パート直後)
                gameState = GamaState.WAITING_NIGHT;
                break;
        }
    }

    private void ChangeScene_Update()
    {
        //int date = ValuesManager.instance.Get_Value(1);
        //int flag = ValuesManager.instance.Get_Value(2); //パート間進行フラグ
        ValuesManager vm = ValuesManager.instance;

        //Debug.Log(gameState);

        if (!TalkEventManager.instance.isReservation)
        {
            switch (gameState)
            {
                case GamaState.NEWGAME:
                    //はじめからを選択した後の処理
                    TalkEventManager.instance.EventReservation(eventPass_name_input);
                    TalkEventManager.instance.EventReservation(eventPass_Start);

                    gameState = GamaState.EVENTING_SCHOOL;
                    break;

                case GamaState.EVENTING_SCHOOL:
                    //学校パート前のイベント実行
                    EventCheckManager.instance.DoEventCheck(vm.Get_Value(1));
                    vm.Set_Value(1, vm.Get_Value(1) + 1);
                    
                    gameState = GamaState.WAITING_SCHOOL;
                    break;
                case GamaState.WAITING_SCHOOL:
                    //学校パートの準備

                    SceneManager.LoadSceneAsync("School_Scene", LoadSceneMode.Additive);
                    TalkEventManager.instance.EventReservation("ev_start_school");

                    gameState = GamaState.GAME_SCHOOL;
                    break;
                case GamaState.GAME_SCHOOL:
                    //学校パート



                    break;
                case GamaState.EVENTING_HOUSE:
                    vm.Set_Value(1, vm.Get_Value(1) + 1);

                    Debug.Log("date: " + vm.Get_Value(1));
                    EventCheckManager.instance.DoEventCheck(vm.Get_Value(1));
                    gameState = GamaState.WAITING_HOUSE;
                    break;
                case GamaState.WAITING_HOUSE:
                    //家パートの準備
                    

                    gameState = GamaState.GAME_HOUSE;
                    break;
                case GamaState.GAME_HOUSE:
                    //家パート


                    break;
                case GamaState.EVENTING_NIGHT:
                    //夜
                    vm.Set_Value(1, vm.Get_Value(1) + 1);

                    EventCheckManager.instance.DoEventCheck(vm.Get_Value(1));
                    gameState = GamaState.WAITING_NIGHT;

                    break;
                case GamaState.WAITING_NIGHT:
                    //夜の準備

                    gameState = GamaState.GAME_NIGHT;
                    break;
                case GamaState.GAME_NIGHT:
                    //夜(リザルト)
            
                    break;
            }
        }
    }

    public void ChangeHouse()
    {
        gameState = GamaState.EVENTING_HOUSE;
    }

    public void ChengeNight()
    {
        gameState = GamaState.EVENTING_NIGHT;
    }

}

public enum GamaState
{
    TITLE, NEWGAME, WAITING_SCHOOL, EVENTING_SCHOOL, GAME_SCHOOL, WAITING_HOUSE, EVENTING_HOUSE, GAME_HOUSE, EVENTING_NIGHT, WAITING_NIGHT, GAME_NIGHT
}

public class StatusData
{
    public int[] statusValue =
    {
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
    };
    public string[] statusText =
    {
        "", ""
    };

    /// <summary>
    /// 所持しているアイテムのリスト
    /// </summary>
    public List<ItemData_Ins> itemList = new List<ItemData_Ins>(); //このリストにアイテムのクラスを入れる。

    /// <summary>
    /// 静的スケジュール表には無い追加スケジュール
    /// </summary>
    public List<Add_Schedule_Save> add_Schedule_Saves = new List<Add_Schedule_Save>(); 


}

public class Add_Schedule_Save
{
    public int date;
    public DateAndEvent eaEvent;
}

public class ItemData_Ins
{
    /// <summary> アイテムの名前 </summary>
    public string itemName = "Item";

    /// <summary> アイテムの説明 </summary>
    public string description = "Description";

    /// <summary> アイテムイベントのファイル名 </summary>
    private string eventName = "";

    /// <summary> アイテムの実行モード </summary>
    private int mode = 0;

    /// <summary> アイテムのイベント実行内容 </summary>
    private string eventData = "";

    public ItemData_Ins(int index)
    {
        ItemData item;
        if (GameManager.instance.itemsList.Count >= index)
        {
            item = GameManager.instance.itemsList[index];
        }
        else
        {
            item = GameManager.instance.itemsList[0];
        }

        itemName = item.itemName;
        description = item.description;
        eventName = item.eventName;
        mode = item.eventMode;
        eventData = item.eventData;

    }


    public void Use()
    {
        Debug.LogFormat("{0}を使用しました", itemName);

        switch (mode)
        {
            case 0:
                TalkEventManager.instance.EventStart(eventName, 0);
                break;
            case 1:
                TalkEventManager.instance.EventStart(eventData, 1);
                break;
        }

        //TalkEventManager.instance.EventStart(eventName);
    }
}