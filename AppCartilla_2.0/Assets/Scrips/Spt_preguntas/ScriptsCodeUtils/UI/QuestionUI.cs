using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionUI : MonoBehaviour
{
    public TextMeshProUGUI Questionlabel;

    public TextMeshProUGUI Answer1Label;

    public TextMeshProUGUI Answer2Label;
    
    public TextMeshProUGUI Answer3Label;

    public TextMeshProUGUI Answer4Label;

    public void PopulateQuestion(QuestionModel questionModel)
    {
        Questionlabel.text = questionModel.Question;

        Answer1Label.text = questionModel.Answer1;

        Answer2Label.text = questionModel.Answer2;

        Answer3Label.text = questionModel.Answer3;
        
        Answer4Label.text = questionModel.Answer4;
    }
}