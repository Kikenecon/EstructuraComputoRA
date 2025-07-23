using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResultScreen : MonoBehaviour
{
    public TextMeshProUGUI ScoreText; // Para "respondiste X/3 preguntas correctas"
    public TextMeshProUGUI CorrectAnswersText; // Para listar respuestas correctas
    public Button ReturnButton; // Botón para volver a WheelScreen

    private QuestionsManager _questionsManager;

    private void Start()
    {
        _questionsManager = QuestionsManager.Instance;
        if (_questionsManager == null)
        {
            Debug.LogError("QuestionsManager no encontrado.");
            return;
        }

        // Suscribirse al evento de completado
        QuestionsManager.OnQuestionsCompleted += ShowResults;
        gameObject.SetActive(false); // Ocultar al inicio
    }

    private void OnDestroy()
    {
        QuestionsManager.OnQuestionsCompleted -= ShowResults;
    }

    void ShowResults()
    {
        if (ScoreText == null || CorrectAnswersText == null || ReturnButton == null)
        {
            Debug.LogError("Uno o más componentes no están asignados en ResultScreen.");
            return;
        }
        gameObject.SetActive(true);
        int totalQuestions = 3;
        ScoreText.text = $"Respondiste {_questionsManager.CorrectAnswersCount}/{totalQuestions} preguntas correctas";

        string correctAnswers = "Respuestas correctas:\n";
        foreach (var question in _questionsManager.AnsweredQuestions)
        {
            correctAnswers += $"- {question.Question}: {question.Answer1} (Índice {question.CorrectAnswerIndex})\n";
        }
        CorrectAnswersText.text = correctAnswers;

        ReturnButton.onClick.AddListener(OnReturnButtonClicked);
    }

    void OnReturnButtonClicked()
    {
        _questionsManager.ResetAndReturnToWheel();
        gameObject.SetActive(false); // Ocultar el panel
    }
}