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
        Debug.Log("image: " + image);
        Debug.Log("Push: " + select_id);
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
}
