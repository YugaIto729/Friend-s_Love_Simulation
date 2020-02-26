using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Image))]
public sealed class Massage_MouseOver_Click : MonoBehaviour
{
    public void Click_Massage()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EventWindowManager.instance.MS_Push_NextFlag();
        }
    }
}
