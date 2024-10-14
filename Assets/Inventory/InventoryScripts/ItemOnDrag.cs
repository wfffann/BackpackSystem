using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemOnDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    public Inventory myBag;
    private int currentItemID;//当前物品的ID

    public GameObject discardItemPrefab;//丢弃的物品

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;//获取Slot的级

        currentItemID = originalParent.GetComponent<SlotUI>().slotID;

        transform.SetParent(transform.parent.parent);//与Grid平级

        transform.position = eventData.position;

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        //Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(eventData.pointerCurrentRaycast.gameObject != null)
        {
            //没有Item则检测不到（SetActive 为 False
            if (eventData.pointerCurrentRaycast.gameObject.name == "Item Image")
            {
                //设置Item的父级为Slot（eventData.pointerCurrentRaycast.gameObject.transform.parent是 Slot
                transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform.parent.parent);
                //设置Item的位置为Slot父级的位置
                transform.position = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.position;

                //itemList的物品存储位置改变
                var temp = myBag.itemList[currentItemID];//返回背包对应ID索引的Item
                                                         //将背包力的Item覆盖为鼠标的Item
                myBag.itemList[currentItemID] =
                    myBag.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotUI>().slotID];
                myBag.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotUI>().slotID] = temp;//物品对调


                //设置替换的Item的父级为原Slot级和位置
                eventData.pointerCurrentRaycast.gameObject.transform.parent.position = originalParent.position;
                eventData.pointerCurrentRaycast.gameObject.transform.parent.SetParent(originalParent);

                GetComponent<CanvasGroup>().blocksRaycasts = true;
                return;
            }

            if (eventData.pointerCurrentRaycast.gameObject.transform.name == "Slot(Clone)")
            {
                //否则直接挂在检测到Slot下面
                transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform);
                transform.position = eventData.pointerCurrentRaycast.gameObject.transform.position;

                //itemList的物品存储位置改变
                myBag.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotUI>().slotID] = myBag.itemList[currentItemID];

                if (eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotUI>().slotID != currentItemID)
                {
                    myBag.itemList[currentItemID] = null;
                }


                GetComponent<CanvasGroup>().blocksRaycasts = true;
                return;
            }
        }
        else//如果对应的是背包的外界
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));

            //实例化丢弃的Item
            var tmpGameObject = Instantiate(discardItemPrefab, worldPos, Quaternion.identity);
            //添加脚本
            var tmepItemOnWorld = tmpGameObject.AddComponent<ItemOnWorld>();
            tmepItemOnWorld.thisItem = myBag.itemList[currentItemID];
            myBag.itemList[currentItemID] = null;//删除背包的Item

            tmepItemOnWorld.playerInventory = myBag;

            tmpGameObject.AddComponent<BoxCollider2D>().isTrigger = true;

            Destroy(this.gameObject);

            //TODO:
            //Slot里面的Item删除了
        }

        //其他任何位置都归位
        //transform.SetParent(originalParent);
        //transform.position = originalParent.position;
        //GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
