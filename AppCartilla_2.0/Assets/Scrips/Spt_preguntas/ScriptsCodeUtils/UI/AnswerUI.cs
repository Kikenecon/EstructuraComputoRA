using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerUI : MonoBehaviour
{
    public int AnswerIndex;

    public void OnAnswerClicked()
    {
        //Debug.Log(message: "Answer was Clicked!");
        bool result = QuestionsManager.Instance.AnswerQuestion(AnswerIndex);
        Debug.Log(result);
    }
}//me quede en 20:35
