using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog_button : MonoBehaviour
{
    public int indexOfValue = 121; //初期値はダイアログ専用予約番号
    public string assignment = "";

    public void Push_Button()
    {
        if (ValuesManager.instance != null) {

            var parser = new Parser();
            parser.Start_Value(assignment);
            if (parser.isParsered_value)
            {
                ValuesManager.instance.Set_Value(indexOfValue, parser.Eval_Value(ValuesManager.instance.Get_Values()));
            }
        }
    }
}
