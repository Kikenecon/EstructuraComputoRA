using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class QuestionsManager : Singleton<QuestionsManager>
{
    public static Action OnNewQuestionLoaded;
    public static Action OnAnswerProvided;
    public static Action OnQuestionsCompleted; // Evento para el panel de resultados

    public Transform CorrectImage;
    public Transform IncorrectImage;
    public QuestionUI Question;

    private CategoryGameManager _categorygameManager;
    private string _currentCategory;
    private QuestionModel _currentQuestion;
    private int _correctAnswers = 0; // Contador de respuestas correctas
    private List<QuestionModel> _answeredQuestions = new List<QuestionModel>(); // Registro de preguntas respondidas

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
        LoadNextQuestion();
    }

    void LoadNextQuestion()
    {
        _currentQuestion = _categorygameManager.GetQuestionForCategory(_currentCategory);
        if (_currentQuestion != null)
        {
            _answeredQuestions.Add(_currentQuestion); // Registra la pregunta actual
            Question.PopulateQuestion(_currentQuestion);
        }
        else if (_answeredQuestions.Count >= 3) // Si se respondieron 3 preguntas
        {
            ShowResults();
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
        if (isCorrect) _correctAnswers++;

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
        result.Append(resultTransform.DOScale(endValue: 1, duration: 0.5f).SetEase(Ease.OutBack));
        result.AppendInterval(1f);
        result.Append(resultTransform.DOScale(endValue: 0, duration: 0.2f).SetEase(Ease.Linear));
        result.AppendCallback(LoadNextQuestion);
    }

    void ShowResults()
    {
        OnQuestionsCompleted?.Invoke(); // Activa el panel de resultados
    }

    // Propiedades públicas para acceder a los datos
    public int CorrectAnswersCount => _correctAnswers;
    public List<QuestionModel> AnsweredQuestions => _answeredQuestions;

    // Método para reiniciar y volver a WheelScreen
    public void ResetAndReturnToWheel()
    {
        _correctAnswers = 0;
        _answeredQuestions.Clear();
        PanelManager.Instance.ShowPanel("WheelScreen", PanelShowBehaviour.HIDE_PREVIOUS);
    }
}