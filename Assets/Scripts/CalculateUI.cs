using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class CalculateUI : MonoBehaviour
    {
        public RectTransform rectHolder;
        public RectTransform rectTarget;
        public RectTransform rectObject;
        public float resultX;
        public float resultY;
        public Vector2 rectPos;
        public Vector2 offsetMin;
        public Vector2 offsetMax;

        private void Start()
        {
            rectPos = rectObject.anchoredPosition;

            offsetMin = rectObject.offsetMin;

            offsetMax = rectObject.offsetMax;
        }
        public void Calculate()
        {
            var xRectObject = Mathf.Abs(rectObject.anchoredPosition.x);
            var yRectObject = Mathf.Abs(rectObject.anchoredPosition.y);

            Debug.Log("Position X: " + xRectObject);
            Debug.Log("Position Y: " + yRectObject);

            float calculateXPoint = (xRectObject * 2) + rectObject.rect.width;
            resultX = rectHolder.rect.width / calculateXPoint;

            float calculateYPoint = (yRectObject * 2) + rectObject.rect.height;
            resultY = rectHolder.rect.height / calculateYPoint;
            rectObject.transform.position = rectTarget.transform.position;
            var sp = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTarget.transform.position);
            var camSp = Camera.main.WorldToScreenPoint(rectTarget.transform.position);
            Debug.Log($"Screen Point {sp}");
            Debug.Log($"Cam Screen Point{camSp}");
            if (resultX < 1 || resultY < 1)
            {
                
            }
         
        }
    }
}