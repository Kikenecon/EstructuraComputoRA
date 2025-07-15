using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // Agrega esta línea para DOTween

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
        int anglePerCategory = 360 / Categories.Count; // 45 grades
        return Categories[(int)(angle / anglePerCategory)];
    }
}