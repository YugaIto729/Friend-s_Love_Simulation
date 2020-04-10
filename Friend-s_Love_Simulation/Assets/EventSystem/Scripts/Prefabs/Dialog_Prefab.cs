using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog_Prefab : MonoBehaviour
{
    public GameObject[] button_objects;
    public Text mainText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set()
    {

    }

    public void Set_Default_Dialog(string b1, string b2, string text)
    {
        int mode = 0;
        Set_Text(text);

        if (b1 == "") { mode++; }
        if (b2 == "") { mode++; }

        if (mode == 0) //ボタン二つ
        {
            button_objects[0].GetComponentInChildren<Text>().text = EventWindowManager.ReplaceCode(b1);
            button_objects[1].GetComponentInChildren<Text>().text = EventWindowManager.ReplaceCode(b2);
        }
        else if (mode == 1) //ボタン一つ
        {
            if (b1 != "")
            {
                button_objects[0].GetComponentInChildren<Text>().text = EventWindowManager.ReplaceCode(b1);
                button_objects[1].SetActive(false);
            }

            if (b2 != "")
            {
                button_objects[0].GetComponentInChildren<Text>().text = EventWindowManager.ReplaceCode(b2);
                button_objects[1].SetActive(false);
            }
        }
    }

    private void Set_Text(string text)
    {
        string t = EventWindowManager.ReplaceCode(text);

        if (t == "NONE")
        {
            mainText.transform.gameObject.SetActive(false);
        }

        mainText.text = t;
    }

}
