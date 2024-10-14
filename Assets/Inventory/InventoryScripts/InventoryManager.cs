using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;

    public Inventory myBag;
    public GameObject slotGrid;
    //public SlotUI slotUIPrefab;
    public GameObject emptySlotUI;
    public Text itemInfromation;

    public List<GameObject> slotUIList = new List<GameObject>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }

        instance = this;
    }

    private void OnEnable()
    {
        RefreshAllSlotUI();
        instance.itemInfromation.text = "";
    }

    public static void UpdateItemInfo(string itemDescription)
    {
        instance.itemInfromation.text = itemDescription;
    }

    /// <summary>
    /// 拾取物品后创建SlotUI
    /// </summary>
    /// <param name="item"></param>
    //public static void CreateNewSlotUI(Item item)
    //{
    //    SlotUI newSlotUI = Instantiate(instance.slotUIPrefab, instance.slotGrid.transform.position, Quaternion.identity);
    //    newSlotUI.gameObject.transform.SetParent(instance.slotGrid.transform);

    //    //Item数据传输给UI
    //    newSlotUI.slotItem = item;
    //    newSlotUI.slotImage.sprite = item.itemSprite;
    //    newSlotUI.slotNum.text = item.itemHeld.ToString();
    //}

    public static void RefreshAllSlotUI()
    {
        //销毁所有的SlotUI
        for(int i = 0; i < instance.slotGrid.transform.childCount; i++)
        {
            if(instance.slotGrid.transform.childCount == 0)
            {
                break;
            }

            Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
            instance.slotUIList.Clear();//清空列表
        }

        //创建背包里面的所有Item的SlotUI
        for(int i = 0; i < instance.myBag.itemList.Count; i++)
        {
            //CreateNewSlotUI(instance.myBag.itemList[i]);
            instance.slotUIList.Add(Instantiate(instance.emptySlotUI));
            instance.slotUIList[i].transform.SetParent(instance.slotGrid.transform);
            instance.slotUIList[i].GetComponent<SlotUI>().slotID = i;//ID
            instance.slotUIList[i].GetComponent<SlotUI>().SetupSlot(instance.myBag.itemList[i]);
        }
    }
}
