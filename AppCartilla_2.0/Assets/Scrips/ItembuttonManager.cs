using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItembuttonManager : MonoBehaviour
{

    private string itemName;
    private Sprite itemImage;
    private GameObject item3DModel;
    private ARInterantionManager interantionManager;

    public string ItemName {
        set
        {
            itemName = value;

        }
    }


    public Sprite ItemImage { set => itemImage = value; }

    public GameObject Item3DModel { set => item3DModel = value; }



    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text= itemName;
        transform.GetChild(1).GetComponent<RawImage>().texture = itemImage.texture;


        var button = GetComponent<Button>();
        button.onClick.AddListener(GameManager.instance.ARPosition);
        button.onClick.AddListener(Create3DModel);

        interantionManager = FindObjectOfType<ARInterantionManager>();
    }

    private void Create3DModel()
    {
        interantionManager.Item3DModel = Instantiate(item3DModel);
    }

    
}
