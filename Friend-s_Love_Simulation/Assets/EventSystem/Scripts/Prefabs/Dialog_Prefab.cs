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

    public void Set_Default_Dialog(string b1, string b2, string text)
    {
        int mode = 0;

        if (b1 == "") { mode++; }
        if (b2 == "") { mode++; }

        if (mode == 0) //ボタン二つ
        {
            mainText.text = text;
            button_objects[0].GetComponentInChildren<Text>().text = b1;
            button_objects[1].GetComponentInChildren<Text>().text = b2;
        }
        else if (mode == 1) //ボタン一つ
        {
            mainText.text = text;
            if (b1 != "")
            {
                button_objects[0].GetComponentInChildren<Text>().text = b1;
                button_objects[1].SetActive(false);
            }

            if (b2 != "")
            {
                button_objects[0].GetComponentInChildren<Text>().text = b2;
                button_objects[1].SetActive(false);
            }
        }
    }

}
