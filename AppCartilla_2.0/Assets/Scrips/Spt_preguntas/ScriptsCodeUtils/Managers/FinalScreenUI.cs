using UnityEngine;
using TMPro;

public class FinalScreenUI : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    private void OnEnable()
    {
        int correct = PlayerPrefs.GetInt("CorrectAnswers", 0);
        int total = PlayerPrefs.GetInt("TotalQuestions", 0);
        resultText.text = $"Respondiste {correct} / {total} preguntas correctamente";
    }


}
