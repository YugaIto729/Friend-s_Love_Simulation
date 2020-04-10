using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackScroll : MonoBehaviour
{
    public static bool isScrolling = true;

    [SerializeField]
    private GameObject backImg;

    [SerializeField]
    private Transform point;

    [SerializeField]
    private float len = 0;

    [SerializeField]
    private float speed = 0.02f;

    [SerializeField]
    private float buff = 0;


    [SerializeField]
    private List<GameObject> backObjects;

    // Start is called before the first frame update
    void Start()
    {
        var sr = backImg.GetComponent<SpriteRenderer>();
        var s = sr.sprite;
        len = s.bounds.size.x * backImg.transform.localScale.x;
        len = 12.46f;

    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    private void Update()
    {
        //生成
        if (buff >= len )
        {
            buff = 0;
            //var t = backImg.transform;
            var o = Instantiate(backImg);
            backObjects.Add(o);
            o.transform.localPosition = point.localPosition + new Vector3(-speed * Time.deltaTime , 0, 0);
            o.transform.parent = transform;
        }

        if (backObjects.Count > 4)
        {
            var o = backObjects[0];
            backObjects.Remove(o);
            Destroy(o);
        }

        if (isScrolling)
        {
            transform.localPosition += new Vector3(-speed * Time.deltaTime, 0, 0);
            buff += speed * Time.deltaTime;
        }
    }



}
