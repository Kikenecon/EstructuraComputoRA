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
            Debug.Log("No se encontró la categoría o no tiene preguntas.");
        }
    }

    public GameModel TriviaConfiguration;
    private string _currentCategory;

    public QuestionModel GetQuestionForCategory(string categoryName)
    {
        CategoryModel categoryModel = TriviaConfiguration.Categories.FirstOrDefault(predicate: category => category.CategoryName == categoryName);
        if (categoryModel != null && categoryModel.Questions != null && categoryModel.Questions.Count > 0)
        {
            return categoryModel.Questions[0];
        }
        return null;
    }

    public void SetCurrentCategory(string categoryName)
    {
        _currentCategory = categoryName;
    }

    public string GetCurrentCategory()
    {
        return _currentCategory; 
    }
}