using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParameterGage : MonoBehaviour
{
    [SerializeField]
    private const float maxBar = 100;

    [SerializeField]
    private Image barImage1;

    [SerializeField]
    private Image barImage2;

    [SerializeField]
    private Text mainValue;

    [SerializeField]
    private Text subValue;

    /// <summary> パラメーターバーの値 </summary>
    public float barCounter = 0;
    /// <summary> パラメーターバーの値追加分 </summary>
    public float barAddCounter = 0;

    private void Update()
    {
        barImage1.fillAmount = barCounter/maxBar;
        barImage2.fillAmount = barAddCounter/maxBar;
        mainValue.text = barAddCounter.ToString();

        float v = barAddCounter - barCounter;
        string subs;
        if (v >= 0)
        {
            subs = "+" + v.ToString();
        }
        else
        {
            subs = "-" + v.ToString();
        }
        subValue.text = subs;

    }

}
