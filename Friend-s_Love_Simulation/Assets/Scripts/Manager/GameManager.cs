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
    public List<ItemData> itemsList;

    public GamaState gamaState = GamaState.GAME_SCHOOL;
    public StatusData statusData = new StatusData();
    //====================================================

    private string[] sceneName =
    {
        "MAIN", "Massage_Scene"
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
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
        
    }

    /// <summary>
    /// データベースを読み込む
    /// </summary>
    private void Read_DataBase()
    {
        //var RID = Resources.Load("ScriptableObjects/Image_Data", typeof(Image_Data)) as Image_Data;
        //var RSE = Resources.Load("ScriptableObjects/EventSouce_Data", typeof(EventSouce_Data)) as EventSouce_Data;
        var RIT = Resources.Load("ScriptableObjects/Item_Data Base", typeof(Item_DataBase)) as Item_DataBase;

        //EventSouceList = RSE.Get_EventSouce();
        //ImagesList = RID.Get_Images();
        itemsList = RIT.Get_Items();
    }

    private void Starter()
    {
        foreach(string s in sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(s);
            if (!scene.IsValid())
            {
                SceneManager.LoadScene(s, LoadSceneMode.Additive);
            }
        }
    }
}

public enum GamaState
{
    TITLE, WAITING_SCHOOL, GAME_SCHOOL, WAITING_HOUSE, GAME_HOUSE, WAITING_MEAL, GAME_MEAL
}

public class StatusData
{
    public int[] statusValue =
    {
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
    };
    public List<ItemData_Ins> itemList = new List<ItemData_Ins>(); //このリストにアイテムのクラスを入れる。
}

public class ItemData_Ins
{
    [SerializeField]
    /// <summary> アイテムの名前 </summary>
    public string itemName = "Item";

    [SerializeField]
    /// <summary> アイテムの説明 </summary>
    public string description = "Description";

    [SerializeField]
    /// <summary> アイテムイベントのファイル名 </summary>
    private string eventName = "";

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
    }


    public void Use()
    {
        Debug.LogFormat("{0}を使用しました", itemName);
        //TalkEventManager.instance.EventStart(eventName);
    }
}
