using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // línea para DOTween

public class WheelUI : MonoBehaviour
{
    public List<string> Categories;

    public Transform PlayButton;
    public Transform Wheel;
    public float RotateDuration;
    public int AmountRotations;

    public void SpinWheel()
    {
        float randomAngle = Random.Range(0, 360);
        CategoryGameManager.Instance.SetCurrentCategory(GetLandedCategory(randomAngle));
        float rotateAngle = (360 * AmountRotations) + randomAngle;
        Wheel.DOLocalRotate(endValue: new Vector3(0, 0, rotateAngle * -1), RotateDuration, RotateMode.FastBeyond360).onComplete += WheelFinishedRotating;
    }

    private void WheelFinishedRotating()
    {
        PlayButton.DOScale(Vector3.one, duration: 0.5f).SetEase(Ease.OutBack);
    }

    public string GetLandedCategory(float angle)
    {
        int anglePerCategory = 360 / Categories.Count;
        int index = Mathf.FloorToInt(angle / anglePerCategory);
        Debug.Log($"Ángulo recibido: {angle}, categoría elegida: {Categories[index]}");
        if (index >= Categories.Count) index = Categories.Count - 1; // prevenir desbordamiento
        return Categories[index];
    }

}