using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemPanel : MonoBehaviour
{
    public ItemData_Ins ItemData_;

    public GameObject itemPanel;

    [SerializeField]
    public GameObject ItemName;
    [SerializeField]
    public GameObject ItemDescription;
    [SerializeField]
    public ButtonManager button;

    Text ItemNameText;
    Text ItemDescriptionText;

    private HousePart house;
    private bool removeFlag;

    // Start is called before the first frame update
    void Start()
    {

        removeFlag = false;

        //テストアイテム(後で消す)
        //statusData.itemList.Add(itemData);

        //子オブジェクトをtransformとして取得
        //itemName = transform.Find("Item Name");
        //itemDescriprion = transform.Find("Item Description");

        //transformからgameobjectを取得
        //ItemName = itemName.gameObject;
        //ItemDescription = itemDescriprion.gameObject;

    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log("house: " + house.name);

        Debug.Log(removeFlag);

        if(removeFlag == true)
        {
            Debug.Log("if文");

            GameManager.instance.statusData.itemList.Remove(ItemData_);

            house.Remove2(ItemData_);

            removeFlag = false;

            Destroy(itemPanel);

            Debug.Log("remove");
        }
    }

    public void Set(ItemData_Ins itemData_Ins, HousePart housePart)
    {
        house = housePart;
        ItemData_ = itemData_Ins;

        button.itemdata = itemData_Ins;

        //gameObject内のtextコンポーネントを取得
        ItemNameText = ItemName.GetComponent<Text>();
        ItemDescriptionText = ItemDescription.GetComponent<Text>();

        //text上書き
        ItemNameText.text = itemData_Ins.itemName;
        ItemDescriptionText.text = itemData_Ins.description;
    }

    public void Remove()
    {
        Debug.Log("house: ");

        removeFlag = true;
    }
}
