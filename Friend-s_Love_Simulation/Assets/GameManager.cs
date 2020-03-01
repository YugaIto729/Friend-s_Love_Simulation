using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //public List<ScriptEvent> EventSouceList;
    public List<Sprite> ImagesList;

    public GamaState gamaState = GamaState.GAME_SCHOOL;
    public StatusData statusData;
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

        //Read_DataBase();　//データベース読み込み
        //Starter();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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

        //EventSouceList = RSE.Get_EventSouce();
        //ImagesList = RID.Get_Images();
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
    /// <summary> 時間(日にちと時間帯) </summary>
    public int date = 0;
}
