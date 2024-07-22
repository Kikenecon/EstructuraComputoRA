using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataManger : MonoBehaviour
{
    [SerializeField] private List<Item> Items = new List<Item>();
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private ItembuttonManager itemButtonManager;


    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnItemsMenu += CreateButtons;
    }

    private void CreateButtons()
    {
        foreach(var item in Items) {
            {
                ItembuttonManager itemButton;
                itemButton = Instantiate(itemButtonManager, buttonContainer.transform);
                itemButton.ItemName = item.ItemName;
                itemButton.ItemImage = item.ItemImagen;
                itemButton.Item3DModel = item.Item3DModel;
                itemButton.name = item.ItemName;
            }
            
            GameManager.instance.OnItemsMenu -= CreateButtons;
        }
    }

}
