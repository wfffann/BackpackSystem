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
    private int currentItemID;//��ǰ��Ʒ��ID

    public GameObject discardItemPrefab;//��������Ʒ

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;//��ȡSlot�ļ�

        currentItemID = originalParent.GetComponent<SlotUI>().slotID;

        transform.SetParent(transform.parent.parent);//��Gridƽ��

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
            //û��Item���ⲻ����SetActive Ϊ False
            if (eventData.pointerCurrentRaycast.gameObject.name == "Item Image")
            {
                //����Item�ĸ���ΪSlot��eventData.pointerCurrentRaycast.gameObject.transform.parent�� Slot
                transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform.parent.parent);
                //����Item��λ��ΪSlot������λ��
                transform.position = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.position;

                //itemList����Ʒ�洢λ�øı�
                var temp = myBag.itemList[currentItemID];//���ر�����ӦID������Item
                                                         //����������Item����Ϊ����Item
                myBag.itemList[currentItemID] =
                    myBag.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotUI>().slotID];
                myBag.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotUI>().slotID] = temp;//��Ʒ�Ե�


                //�����滻��Item�ĸ���ΪԭSlot����λ��
                eventData.pointerCurrentRaycast.gameObject.transform.parent.position = originalParent.position;
                eventData.pointerCurrentRaycast.gameObject.transform.parent.SetParent(originalParent);

                GetComponent<CanvasGroup>().blocksRaycasts = true;
                return;
            }

            if (eventData.pointerCurrentRaycast.gameObject.transform.name == "Slot(Clone)")
            {
                //����ֱ�ӹ��ڼ�⵽Slot����
                transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform);
                transform.position = eventData.pointerCurrentRaycast.gameObject.transform.position;

                //itemList����Ʒ�洢λ�øı�
                myBag.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotUI>().slotID] = myBag.itemList[currentItemID];

                if (eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotUI>().slotID != currentItemID)
                {
                    myBag.itemList[currentItemID] = null;
                }


                GetComponent<CanvasGroup>().blocksRaycasts = true;
                return;
            }
        }
        else//�����Ӧ���Ǳ��������
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));

            //ʵ����������Item
            var tmpGameObject = Instantiate(discardItemPrefab, worldPos, Quaternion.identity);
            //��ӽű�
            var tmepItemOnWorld = tmpGameObject.AddComponent<ItemOnWorld>();
            tmepItemOnWorld.thisItem = myBag.itemList[currentItemID];
            myBag.itemList[currentItemID] = null;//ɾ��������Item

            tmepItemOnWorld.playerInventory = myBag;

            tmpGameObject.AddComponent<BoxCollider2D>().isTrigger = true;

            Destroy(this.gameObject);

            //TODO:
            //Slot�����Itemɾ����
        }

        //�����κ�λ�ö���λ
        //transform.SetParent(originalParent);
        //transform.position = originalParent.position;
        //GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
