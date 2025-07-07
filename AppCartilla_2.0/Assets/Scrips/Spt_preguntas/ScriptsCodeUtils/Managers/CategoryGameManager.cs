using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CategoryGameManager : Singleton<CategoryGameManager>
{
    void Start()
    {
        QuestionModel question = GetQuestionForCategory("NombreDeUnaCategoria");
        if (question != null)
        {
            Debug.Log($"Pregunta encontrada: {question.Question}");
        }
        else
        {
            Debug.Log("No se encontr� la categor�a o no tiene preguntas.");
        }
    }

    public GameModel TriviaConfiguration;

    public QuestionModel GetQuestionForCategory(string categoryName)
    {
        CategoryModel categoryModel = TriviaConfiguration.Categories.FirstOrDefault(predicate: category => category.CategoryName == categoryName);
        if(categoryModel != null)
        {
            return categoryModel.Questions[0];
        }
        return null;
    }
}
