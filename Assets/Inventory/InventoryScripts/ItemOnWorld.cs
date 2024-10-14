using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld : MonoBehaviour
{
    public Item thisItem;
    public Inventory playerInventory;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //ʰȡ
            AddNewItem();
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// ʰȡ��Ʒ
    /// </summary>
    private void AddNewItem()
    {
        if (!playerInventory.itemList.Contains(thisItem))
        {
            //playerInventory.itemList.Add(thisItem);
            //����SlotUI
            //InventoryManager.CreateNewSlotUI(thisItem);
            for(int i = 0; i< playerInventory.itemList.Count; i++)
            {
                //ռ�ÿո��Ӹ���Item
                if(playerInventory.itemList[i] == null)
                {
                    playerInventory.itemList[i] = thisItem;
                    break;
                }
            }
        }
        else
        {
            thisItem.itemHeld += 1;
        }

        InventoryManager.RefreshAllSlotUI();
    }
}
