using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog_Name_ctrl : MonoBehaviour
{
    public GameObject familyFields;
    public GameObject firstFields;
    public GameObject button;

    private InputField family;
    private InputField first;

    private string text1 ="";
    private string text2 ="";

    // Start is called before the first frame update
    void Start()
    {
        family = familyFields.GetComponent<InputField>();
        first = firstFields.GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if (family.text != "" && first.text != "")
        {
            button.SetActive(true);
        }
        else
        {
            button.SetActive(false);
        }

        if (text1 != family.text)
        {
            ValuesManager.instance.Set_Text(1, family.text);
        }

        if (text2 != first.text)
        {
            ValuesManager.instance.Set_Text(2, first.text);
        }

        text1 = family.text;
        text2 = first.text;
    }
}
