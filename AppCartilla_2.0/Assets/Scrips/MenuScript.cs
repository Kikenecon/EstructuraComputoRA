using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{

    public GameObject message;
    public GameObject menu;

    public GameObject messagemulti;
    public GameObject menumulti;


    private bool showStartMessage=true;
    private string showStartMessageDataName = "MessageScaner";

    private bool showStartMessageMulti=true;
    private string showStartMessageDataNameMulti = "MessageMultimedia";

    void Start()
    {
        showStartMessage = PlayerPrefs.GetInt(showStartMessageDataName,1)==1;

        showStartMessageMulti = PlayerPrefs.GetInt(showStartMessageDataNameMulti,1)==1;

         if(message == null){

             messagemulti.SetActive(showStartMessageMulti);
             menumulti.SetActive(!showStartMessageMulti);

         }else{
             message.SetActive(showStartMessage);
              menu.SetActive(!showStartMessage);
         }
    }

    public void ShowMessageAgainToggle(bool b)
    {
        showStartMessage = !b; 
    }

    public void ShowMessageAgainToggleMulti(bool b)
    {
        showStartMessageMulti = !b; 
    }

    public void StartMessageOkButton()
    {
        if (!showStartMessage)
        {
            PlayerPrefs.SetInt(showStartMessageDataName,0);
        }
        message.SetActive(false);
        menu.SetActive(true);
    }

    public void StartMessageOkButtonMulti()
    {
        if (!showStartMessageMulti)
        {
            PlayerPrefs.SetInt(showStartMessageDataNameMulti,0);
        }
        messagemulti.SetActive(false);
        menumulti.SetActive(true);
    }

    public void RestoreDefaultConfiguration()
    {
        PlayerPrefs.DeleteKey(showStartMessageDataName);
        PlayerPrefs.DeleteKey(showStartMessageDataNameMulti);
    }

}
