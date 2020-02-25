using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HousePart : MonoBehaviour
{
    public ItemData displayItem;
    public StatusData statusData;

    public GameObject ItemPanel;
    public Transform canvas;

    private List<ItemData_Ins> displayed_List;

    // Start is called before the first frame update
    void Start()
    {


        statusData = new StatusData() {
            itemList = new List<ItemData_Ins>()
            {
                new ItemData_Ins(0),new ItemData_Ins(1),new ItemData_Ins(0),new ItemData_Ins(1),
            }
        };
        GameManager.instance.statusData = statusData;

        displayed_List = new List<ItemData_Ins>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; i < statusData.itemList.Count ; i++)
        //foreach(ItemData_Ins Items in statusData.itemList)
        {
            if (!displayed_List.Contains(statusData.itemList[i]))
            {
                displayed_List.Add(statusData.itemList[i]);
                var o = Instantiate(ItemPanel, transform.position, Quaternion.identity);
               
                o.GetComponent<ItemPanel>().Set(statusData.itemList[i], this);
                o.transform.parent = canvas;

                //生成したパネルの移動
                var pos = o.transform.localPosition;

                var siz = o.transform.localScale;
                siz.x = 1.7f;
                siz.y = 1.7f;
                siz.z = 1.0f;

                pos.x = (i % 3) * (188) - 188;
                pos.y = (i / 3) * (-195) + 95;
                pos.z = 0;

                o.transform.localPosition = pos;
                o.transform.localScale = siz;
            }
        }
    }

    public void Remove2(ItemData_Ins itemData)
    {
        displayed_List.Remove(itemData);
    }
}
