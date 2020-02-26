using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class FadeManager : MonoBehaviour
{
    public static FadeManager instance;

    public enum F_State
    {
        STANDBY, FADE
    }

    public F_State f_state = F_State.STANDBY;

    [SerializeField]
    private Image image_mid;
    private bool onFade_Coroutine = false;
    private TalkEventManager TeManager;

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

    }

    // Start is called before the first frame update
    void Start()
    {
        TeManager = TalkEventManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (f_state != F_State.STANDBY)
        {
            switch (f_state)
            {
                case F_State.FADE:
                    Fade();
                    break;
            }
        }
    }

    public void Fade()
    {
        if (!onFade_Coroutine)
        {
            var eo = TeManager.Get_CullentEvent();

            Debug.Log(eo);
            StartCoroutine(C_Fade(eo.seconds, image_mid.color, eo.color, eo.register1));

            if (eo.register1 == 1)
            {
                End_Fade();
            }
        }
    }

    private IEnumerator C_Fade(float second, Color start, Color goal, int mode)
    {
        onFade_Coroutine = true;
        if (mode > 1) mode = 0;

        if (second > 0)
        {
            Debug.Log("start: " + start);
            Debug.Log("goal: " + goal);
            for (int i = 0; i <= second * 100; i++)
            {
                Debug.Log("i: "+i/(second*100));
                image_mid.color = Color.Lerp(start, goal, i / (second * 100));
                yield return new WaitForSeconds(0.01f);
            }

            if (mode == 0)
            {
                End_Fade();
            }
        }
        else
        {
            image_mid.color = goal;
            End_Fade();
        }

        onFade_Coroutine = false;
    }

    private void End_Fade()
    {
        TeManager.cullentEvent = EventType.SYSTEMWAIT;
        f_state = F_State.STANDBY;

        TeManager.Increment_EventCounter();

    }

}
