using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static System.TimeZoneInfo;

public class CanvasModifier : MonoBehaviour
{
    public Canvas canvas;              // Reference to the Screen Space - Camera canvas
    public RectTransform uiElement;   // UI element to transition
    public Camera uiCamera;           // The camera assigned to the canvas

    public void ToggleZoomIn()
    {
        ZoomIn(new Vector3(0f, 325f, 0f));
    }    
    
    public void ToggleZoomOut()
    {
        ZoomOut(new Vector3(0f, 0f, 0f), new Vector3(0f, 0.743f, 0.185f));
    }

    public void ZoomIn(Vector3 worldPosition)
    {
        Vector3 newScale = new Vector3(1.5f, 1.5f, 1f);
        Vector3 newPosition = uiElement.localPosition + new Vector3(0f, 0.2f, -0.8f);

        LeanTween.scale(uiElement, newScale, 0.05f);
        LeanTween.moveLocal(uiElement.gameObject, newPosition, 0.05f).setOnComplete(() => {
            uiElement.localScale = newScale;
            uiElement.localPosition = newPosition;
        });
/*        uiElement.transform.localScale = new Vector3(1.5f, 1.5f, 1f);*/
    }

    public void ZoomOut(Vector3 targetWorldPosition, Vector3 canvasWorldPosition)
    {
        // Step 1: Convert World Position to Screen Space
        Vector3 screenPosition = uiCamera.WorldToScreenPoint(targetWorldPosition);

        Vector3 newPosition = uiElement.localPosition + new Vector3(0f, -0.2f, 0.8f);

        LeanTween.scale(uiElement, Vector3.one, 0.05f);
        LeanTween.moveLocal(uiElement.gameObject, newPosition, 0.05f).setOnComplete(() => {
            uiElement.localScale = Vector3.one;
            uiElement.localPosition = Vector3.zero;
        });
    }
}

// 29 pickle slice
// 8 tomato slice

