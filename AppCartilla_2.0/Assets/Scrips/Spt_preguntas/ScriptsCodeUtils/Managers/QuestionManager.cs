using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionsManager : Singleton<QuestionsManager>
{
    public QuestionUI Question;

    public string CategoryName;

    private CategoryGameManager _categorygameManager;

    private void OnEnable()
    {
        //Cache a reference
        _categorygameManager = CategoryGameManager.Instance;
        LoadNextQuestion();
    }

    void LoadNextQuestion()
    {
        var newQuestion = _categorygameManager.GetQuestionForCategory(CategoryName);
        if(newQuestion != null)
        {
            Question.PopulateQuestion(new QuestionModel());
        }
    }
}
