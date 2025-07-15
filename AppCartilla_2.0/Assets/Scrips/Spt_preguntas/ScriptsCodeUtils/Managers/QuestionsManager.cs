using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class QuestionsManager : Singleton<QuestionsManager>
{
    public static Action OnNewQuestionLoaded;
    public static Action OnAnswerProvided;
    public static Action OnQuestionsCompleted;

    public Transform CorrectImage;

    public Transform IncorrectImage;

    public QuestionUI Question;

    private CategoryGameManager _categorygameManager;

    private string _currentCategory;

    private QuestionModel _currentQuestion;

    /*private void Start()
    {
        //Cache a reference
        _categorygameManager = CategoryGameManager.Instance;

        _currentCategory = _categorygameManager.GetCurrentCategory();

        LoadNextQuestion();
    }     Codigo Anterior*/
    private void Start()
    {
        _categorygameManager = CategoryGameManager.Instance;
        if (_categorygameManager == null)
        {
            Debug.LogError("CategoryGameManager no encontrado.");
            return;
        }
        _currentCategory = _categorygameManager.GetCurrentCategory();
        if (string.IsNullOrEmpty(_currentCategory))
        {
            Debug.LogError("No se pudo obtener una categor�a v�lida.");
            return;
        }
        LoadNextQuestion();
    }

    void LoadNextQuestion()
    {
        _currentQuestion = _categorygameManager.GetQuestionForCategory(_currentCategory);

        if(_currentQuestion != null)
        {
           
            Question.PopulateQuestion(_currentQuestion);
        }
        OnNewQuestionLoaded?.Invoke();
    } 

    public bool AnswerQuestion(int answerIndex)
    {
        if (_currentQuestion == null)
        {
            Debug.LogError("No hay pregunta actual cargada.");
            return false;
        }
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
