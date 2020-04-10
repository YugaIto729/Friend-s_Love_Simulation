using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SchoolPartManager : MonoBehaviour
{
    public static SchoolPartManager instance;

    public enum SPM_State
    {
        WAITING, ENCOUNTING, EVENT ,ENCOUNTED, ENDING,
    }

    public SPM_State state = SPM_State.WAITING;

    private int time = 0;
    private int setedTime = 0;



    public GameObject[] charObjects;
    public ParameterEditData[] parametrs;
    public GameObject[] EffectObjects;


    [SerializeField]
    private Transform scrollObject;
    [SerializeField]
    private Transform hiroinePoint;
    private List<DateAndEvent> events = new List<DateAndEvent>();

    private Transform target;
    [SerializeField]
    private Transform hero;

    private Vector3 heroFirstPoss;
    private bool onCoroutine = false;
    private bool onCoroutine2 = false;

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
        
        heroFirstPoss = hero.localPosition;

        //好感度上昇定数は 0.2
        ValuesManager.instance.Set_Value(30, 0.2f);

        ValuesManager.instance.Set_Value(31, 10);
        ValuesManager.instance.Set_Value(32, 10);
        ValuesManager.instance.Set_Value(33, 10);
        ValuesManager.instance.Set_Value(34, 10);
        ValuesManager.instance.Set_Value(35, 10);
        ValuesManager.instance.Set_Value(36, 10);
        ValuesManager.instance.Set_Value(37, 10);
        ValuesManager.instance.Set_Value(38, 10);
        ValuesManager.instance.Set_Value(39, 10);
        ValuesManager.instance.Set_Value(40, 10);

    }

    // Update is called once per frame
    void Update()
    {

        time = ValuesManager.instance.Get_Value(1) % 6;

        if ( 1 <= time && time <= 3)
        {
            SchoolUpdate();
        }

        if (state == SPM_State.ENDING)
        {
            if (!onCoroutine)
            {
                StartCoroutine(Ending());
            }

        }

    }

    private void SchoolUpdate()
    {
        if (hiroinePoint == null) return;
        if (scrollObject == null) return;


        //最初のイベントチェック
        if (setedTime != time)
        {
            var el = EventCheckManager.instance.EventCheck(ValuesManager.instance.Get_Value(1));
            events.Clear();
            Debug.Log("EC: " + el.Count);

            foreach(var e in el)
            {
                //if (e.type == DateAndEvent_Type.ENCOUNT)
                {
                    events.Add(e);
                }
            }

            setedTime = time;
        }


        if (events.Count != 0)
        {
            switch (state)
            {
                case SPM_State.WAITING:
                    {
                        if (!events[0].parser.Eval(ValuesManager.instance.Get_Values()))
                        {
                            events.RemoveAt(0);
                            break;
                        }

                        int id = events[0].heroineID;
                        GameObject o = Instantiate(charObjects[id]);
                        o.transform.localPosition = hiroinePoint.localPosition;
                        o.transform.parent = scrollObject;
                        target = o.transform;

                        state = SPM_State.ENCOUNTING;
                    }
                    break;
                case SPM_State.ENCOUNTING:
                    {
                        float distance = Mathf.Pow(target.position.x - hero.position.x, 2) + Mathf.Pow(target.position.y - hero.position.y, 2);
                        //Debug.Log("distance: " + Mathf.Sqrt(distance));

                        //distance = Vector3.Distance()

                        if (Mathf.Pow(4, 2) <= distance && distance < Mathf.Pow(6, 2))
                        {
                            if (!onCoroutine)
                            {
                                StartCoroutine(MoveHero());
                            }

                        }
                        if (0 <= distance && distance < Mathf.Pow(3, 2))
                        {
                            Debug.Log("10以内");
                            BackScroll.isScrolling = false;

                            state = SPM_State.EVENT;
                        }
                    }
                    break;
                case SPM_State.EVENT:
                    {
                        if (!onCoroutine)
                        {
                            Debug.Log("イベント");
                            StartCoroutine(EncountEvent());
                        }
                    }
                    break;
                case SPM_State.ENCOUNTED:
                    {
                        if (target == null)
                        {

                            events.RemoveAt(0);
                            state = SPM_State.WAITING;
                        }
                        else
                        {
                            float distance = Mathf.Pow(target.position.x - hero.position.x, 2) + Mathf.Pow(target.position.y - hero.position.y, 2);

                            if (Mathf.Pow(7, 2) <= distance)
                            {
                                Debug.Log(20);
                                Transform o = target;
                                target = null;
                                Destroy(o.gameObject);

                                events.RemoveAt(0);
                                state = SPM_State.WAITING;
                            }
                        }
                    }
                    break;
            }
        }
        else
        {
            //Debug.Log("[SPM]date: " + ValuesManager.instance.Get_Value(1) % 6);
            if (ValuesManager.instance.Get_Value(1) % 6 == 3)
            {
                state = SPM_State.ENDING;
                return;
            }
            ValuesManager.instance.Set_Value(1, ValuesManager.instance.Get_Value(1) + 1);

        }

        
    }

    private IEnumerator MoveHero()
    {
        onCoroutine = true;

        Vector3 start = hero.position;
        for (int i=0; i <= 100; i++)
        {
            //Debug.Log((float)i/100);
            hero.position = Vector3.Lerp(start, new Vector3(hero.position.x, target.position.y, hero.position.z), (float)i/100 );

            yield return new WaitForSeconds(0.01f);
        }

        onCoroutine = false;
    }

    private IEnumerator EncountEvent()
    {
        onCoroutine = true;

        Debug.Log("Type:" + events[0].type);

        if (events[0].type == DateAndEvent_Type.TALK)
        {
            var image = FadeManager.instance.Get_FadeImage();

            for (int i = 0; i <= 100; i++)
            {
                //Debug.Log("i: "+i/(second*100));
                image.color = Color.Lerp(Color.clear, Color.black, (float)i / 100);
                yield return new WaitForSeconds(0.01f);
            }

            var o = target;
            target = null;
            Destroy(o.gameObject);

            TalkEventManager.instance.EventReservation(events[0].eventPass);

            while (true)
            {
                yield return null;
                if (!TalkEventManager.instance.isReservation && !TalkEventManager.instance.eventMode)
                {

                    break;
                }
            }

            for (int i = 0; i <= 100; i++)
            {
                //Debug.Log("i: "+i/(second*100));
                image.color = Color.Lerp(Color.black, Color.clear, (float)i / 100);
                yield return new WaitForSeconds(0.01f);
            }

        }
        else if (events[0].type == DateAndEvent_Type.ENCOUNT)
        {

            var list = new List<float>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}; //10個
            int id = events[0].heroineID;
            var vs = parametrs[id].vs;
            for (int i = 0; i < vs.Length; i++)
            {
                Parser parser = new Parser();
                parser.Start_Value(vs[i]);
                Debug.Log(vs[i]);
                list[i] = parser.Eval_Value_outFlaot(ValuesManager.instance.Get_Values());
            }

            int sum = 0;

            foreach(int v in list) sum += v; //合計

            Debug.Log(string.Format("{0} : {1}", id , sum));

            onCoroutine2 = false;
            StartCoroutine(Effecting(list));
            while (true)
            {
                if (onCoroutine2)
                {
                    onCoroutine2 = false;
                    break;
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    break;
                }
                yield return null;
            }
        }




        //通路に戻す
        state = SPM_State.ENCOUNTED;
        BackScroll.isScrolling = true;
        Vector3 start = hero.position;

        for (int i = 0; i <= 100; i++)
        {
            hero.position = Vector3.Lerp(start, heroFirstPoss, (float)i / 100);

            yield return new WaitForSeconds(0.01f);
        }
        onCoroutine = false;
    }

    private IEnumerator Effecting(List<float> addParmater)
    {
        foreach(float f in addParmater)
        {
            GameObject targetEffect;

            if (f <= -5)
            {
                targetEffect = EffectObjects[3];
            }
            else if(-5 < f && f < 0)
            {
                targetEffect = EffectObjects[1];
            }
            else if (0 < f && f <= 5)
            {
                targetEffect = EffectObjects[0];
            }
            else if (5 < f && f <= 10)
            {
                targetEffect = EffectObjects[5];
            }
            else if (10 < f && f <= 15)
            {
                targetEffect = EffectObjects[4];
            }
            else if(15 < f)
            {
                targetEffect = EffectObjects[2];
            }
            else
            {
                targetEffect = null;
            }


            GameObject o = null;
            SpriteRenderer sr = null;
            Vector3 startP = new Vector3();

            if (targetEffect != null)
            {
                o = Instantiate(targetEffect);
                sr = o.GetComponent<SpriteRenderer>();
                o.transform.position = target.position + new Vector3(0.5f, 1, -1);
                o.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                //var startC = sr.color;
                startP = o.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), o.transform.position.z);
            }

            GameObject to = Instantiate(EffectObjects[6]);
            var srt = to.GetComponent<SpriteRenderer>();
            if (Random.Range(0, 2) == 0) {
                to.transform.localScale = new Vector3(0.5f, 0.5f, -2); 
                to.transform.position = target.position + new Vector3(1, 1.5f, target.transform.position.z) + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
                ;
            }
            else
            {
                to.transform.localScale = new Vector3(-0.5f, 0.5f, -2);
                to.transform.position = target.position + new Vector3(-5, 1.5f, target.transform.position.z) + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
            }

            StartCoroutine(Effecting_part(to, srt, to.transform.position, 100));
            if (f != 0) StartCoroutine(Effecting_part(o, sr, startP, 20));

            yield return new WaitForSeconds(1f);
        }

        onCoroutine2 = true;
    }

    private IEnumerator Effecting_part(GameObject o, SpriteRenderer sr, Vector3 startP, int maxIndex)
    {
        for (int i = 0; i <= maxIndex; i++)
        {
            Debug.Log("SpriteRenderer: " + sr);
            sr.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), (float)i / maxIndex);
            o.transform.position = Vector3.Lerp(startP, startP + new Vector3(0, 1, 0), (float)i / maxIndex);
            yield return null;
        }
        Destroy(o);
    }


    private IEnumerator Ending()
    {
        onCoroutine = true;
        var image = FadeManager.instance.Get_FadeImage();

        for (int i = 0; i <= 100; i++)
        {
            //Debug.Log("i: "+i/(second*100));
            image.color = Color.Lerp(Color.clear, Color.black, (float)i / 100);
            yield return new WaitForSeconds(0.01f);
        }

        GameManager.instance.gameState = GamaState.EVENTING_HOUSE;

        SceneManager.UnloadSceneAsync("School_Scene");

        onCoroutine = false;
    }

    private void EndPart()
    {
        //UnityEditor.EditorApplication.isPaused = true;
        state = SPM_State.ENDING;
    }

    /// <summary>
    /// キャラにエフェクトを出す
    /// </summary>
    /// <param name="charObject"></param>
    private void CharEffect(GameObject charObject)
    {

    }
}
