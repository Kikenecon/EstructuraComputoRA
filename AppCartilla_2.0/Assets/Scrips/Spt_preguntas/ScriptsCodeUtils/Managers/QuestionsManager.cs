using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class QuestionsManager : Singleton<QuestionsManager>
{
    public static Action OnNewQuestionLoaded;

    public static Action OnAnswerProvided;

    public Transform CorrectImage;

    public Transform IncorrectImage;

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
        OnNewQuestionLoaded?.Invoke();
    }

    public bool AnswerQuestion(int answerIndex)
    {
        OnAnswerProvided?.Invoke();

        bool isCorrect = _currentQuestion.CorrectAnswerIndex == answerIndex;

        if (isCorrect)
        {
            TweenResult(CorrectImage);
        }   
        else
        {
            TweenResult(IncorrectImage);
        }
        return isCorrect;
    }

    void TweenResult(Transform resultTransform)
    {
        Sequence result = DOTween.Sequence();
        result.Append(resultTransform.DOScale(endValue: 1, duration: .5f).SetEase(Ease.OutBack)); //escala de 0 a 1 
        result.AppendInterval(1f);
        result.Append(resultTransform.DOScale(endValue: 0, duration: .2f).SetEase(Ease.Linear)); //
        result.AppendCallback(LoadNextQuestion);
    }
}
