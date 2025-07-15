using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AnswerUI : MonoBehaviour
{
    public Image CorrectImage;

    public Image IncorrectImage;

    public int AnswerIndex;

    private bool _canBeClicked = true;

    private void OnEnable()
    {
        QuestionsManager.OnNewQuestionLoaded += ResetValues;
        QuestionsManager.OnAnswerProvided += AnswerProvided;
    }

    private void OnDisable()
    {
        QuestionsManager.OnNewQuestionLoaded -= ResetValues;
        QuestionsManager.OnAnswerProvided -= AnswerProvided;
    }

    public void OnAnswerClicked()
    {
        if (_canBeClicked && CorrectImage != null && IncorrectImage != null)
        {
            bool result = QuestionsManager.Instance.AnswerQuestion(AnswerIndex);
            if (result)
            {
                CorrectImage.DOFade(endValue: 1, duration: 0.5f);
            }
            else
            {
                IncorrectImage.DOFade(endValue: 1, duration: 0.5f);
            }
        }
    }

    void AnswerProvided()
    {
        _canBeClicked = false;
    }

    void ResetValues()
    {
        Debug.Log($"Reseteando imágenes para {gameObject.name}");
        CorrectImage.DOFade(endValue: 0, duration: .2f);
        IncorrectImage.DOFade(endValue: 0, duration: .2f);
        _canBeClicked = true;
    }

}
