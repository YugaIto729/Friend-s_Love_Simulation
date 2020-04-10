using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Select_Button_Prefab : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] int pointFont = 32;

    public int select_id = -1;
    private EventSelectManager EsManager;
    private RectTransform rectTransform;
    private Animator animator;
    private Image image;

    public string DisplayText = "Text";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        EsManager = EventSelectManager.instance;
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = DisplayText;
        rectTransform.sizeDelta = new Vector2(text.text.Length * pointFont + 10, rectTransform.sizeDelta.y);
    }

    public void Push_Button()
    {
        //Debug.Log("image: " + image);
        //Debug.Log("Push: " + select_id);
        EventSelectManager.instance.Action_Select(this);
    }

    
    public void Over_Button()
    {
        image.color = new Color(1, 0.5f, 0.5f);
    }

    public void Exit_Button()
    {
        image.color = new Color(1, 1, 1);
    }

    public void Set_Prefab(string mess,int index)
    {
        DisplayText = mess;
        select_id = index;

    }

    public int GetID()
    {
        return select_id;
    }

    /// <summary>
    /// アニメーションを再生する
    /// </summary>
    public void PlayAnimation()
    {
        if (animator == null) animator = GetComponent<Animator>();

        animator.SetBool("playedAnime", true);
    }

    //"button_push_animation"
    /// <summary>
    /// アニメーションの状態を確認する
    /// </summary>
    /// <param name="stateName"></param>
    /// <returns></returns>
    public bool Check_AnimeState(string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    public void Move_Display(Vector3 target, float time, int i)
    {

        StartCoroutine(C_middle(target, time, i));
    }

    private IEnumerator C_middle(Vector3 target, float time, int i)
    {
        yield return new WaitForSeconds(i * 0.1f);

        StartCoroutine(C_Displaying(target, time));
        //StartCoroutine(C_Roteto(time));
    }

    private IEnumerator C_Displaying(Vector3 target, float time)
    {
        float rTime = Random.Range(0.5f, 1.5f);
        Vector3 rVector = new Vector3(Random.Range(-300, 300), Random.Range(-300, 300), 0);
        transform.localPosition = new Vector3(target.x-1000, target.y, target.z) + rVector;

        for (int i = 0; i < (time + rTime) * 60; i++)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, target, i / ((time + rTime) * 60));

            yield return null;
        }

    }

    private IEnumerator C_Roteto(float time)
    {
        Vector3 roteto = new Vector3(0 ,0 ,Random.Range(-5, 5));

        for (int i = 0; i < time * 60; i++)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(roteto), i / (time * 60));

            yield return null;
        }
    }

}
