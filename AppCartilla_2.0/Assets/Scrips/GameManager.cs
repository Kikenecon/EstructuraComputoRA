using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{

public event Action OnMainMenu; 
public event Action OnItemsMenu; 
public event Action OnARPosition;

public static GameManager instance;

private void Awake()
{
    if(instance!=null && instance !=this){
        Destroy(gameObject);
    }else{
        instance=this;
    }
}  
   
    void Start()
    {
       MainMenu(); 
    }

    public void MainMenu()
    {
        OnMainMenu?.Invoke();
        Debug.Log("menu activado");
    }

    public void ItemsMenu()
    {
        OnItemsMenu?.Invoke();
        Debug.Log("Intems menu Activado");
    }

    public void ARPosition()
    {
        OnARPosition?.Invoke();
        Debug.Log("AR position Activado");
    }


}
