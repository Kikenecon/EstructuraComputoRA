using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class QuestionsManager : Singleton<QuestionsManager>
{
    private int _totalQuestions = 0;
    private int _correctAnswers = 0;
    private int _questionsAnswered = 0;


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
            Debug.LogError("No se pudo obtener una categoría válida.");
            return;
        }

        // Obtiene el total de preguntas de la categoría actual
        _totalQuestions = _categorygameManager.TriviaConfiguration.Categories
            .Find(cat => cat.CategoryName == _currentCategory)?.Questions.Count ?? 0;

        _correctAnswers = 0;
        _questionsAnswered = 0;
        LoadNextQuestion();
    }

    void LoadNextQuestion()
    {
        _currentQuestion = _categorygameManager.GetQuestionForCategory(_currentCategory);
        if (_currentQuestion != null)
        {
            Debug.Log($"Pregunta cargada: {_currentQuestion.Question}");
            Question.PopulateQuestion(_currentQuestion);
        }
        else
        {
            Debug.LogWarning("No se pudo cargar ninguna pregunta.");
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
        _questionsAnswered++;
        if (isCorrect)
        {
            _correctAnswers++;
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

        result.AppendCallback(() =>
        {
            if (_questionsAnswered >= _totalQuestions)
            {
                ShowFinalScreen();
            }
            else
            {
                LoadNextQuestion();
            }
        });
    }

    void ShowFinalScreen()
    {
        // Guardar el resultado en PlayerPrefs (opcional)
        PlayerPrefs.SetInt("CorrectAnswers", _correctAnswers);
        PlayerPrefs.SetInt("TotalQuestions", _totalQuestions);
        PlayerPrefs.Save();

        PanelManager.Instance.ShowPanel("FinalScreen", PanelShowBehaviour.HIDE_PREVIOUS);
        OnQuestionsCompleted?.Invoke();
    }

}