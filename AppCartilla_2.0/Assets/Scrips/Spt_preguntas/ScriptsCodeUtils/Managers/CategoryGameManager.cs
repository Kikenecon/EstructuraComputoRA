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

    List<int> _askedQuestionIndex = new List<int>();

    public QuestionModel GetQuestionForCategory(string categoryName)
    {
        CategoryModel categoryModel = TriviaConfiguration.Categories.FirstOrDefault(predicate: category => category.CategoryName == categoryName);
        Debug.Log($"Buscando categoría: {categoryName}");
        if (categoryModel != null && categoryModel.Questions != null && categoryModel.Questions.Count > 0)
        {
            if (_askedQuestionIndex.Count >= categoryModel.Questions.Count)
            {
                _askedQuestionIndex.Clear();
            }

            int randomIndex = Random.Range(0, categoryModel.Questions.Count);
            while (_askedQuestionIndex.Contains(randomIndex) && _askedQuestionIndex.Count < categoryModel.Questions.Count)
            {
                randomIndex = Random.Range(0, categoryModel.Questions.Count);
            }

            _askedQuestionIndex.Add(randomIndex);
            return categoryModel.Questions[randomIndex];
        }
        return null;
        

    }


    public void SetCurrentCategory(string categoryName)
    {
        _currentCategory = categoryName;
        _askedQuestionIndex.Clear();
    }

    public string GetCurrentCategory()
    {
        return _currentCategory;
    }
}