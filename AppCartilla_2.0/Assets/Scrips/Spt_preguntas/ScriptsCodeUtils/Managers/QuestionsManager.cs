using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionsManager : Singleton<QuestionsManager>
{
    public QuestionUI Question;

    public string CategoryName;

    private CategoryGameManager _categorygameManager;

    private QuestionModel _currentQuestion;

    private void Start()
    {
        //Cache a reference
        _categorygameManager = CategoryGameManager.Instance;
        LoadNextQuestion();
    }

    void LoadNextQuestion()
    {
        _currentQuestion = _categorygameManager.GetQuestionForCategory(CategoryName);

        if(_currentQuestion != null)
        {
            Question.PopulateQuestion(_currentQuestion);
        }
    }

    public bool AnswerQuestion(int answerIndex)
    {
        return _currentQuestion.CorrectAnswerIndex == answerIndex;
    }
}
