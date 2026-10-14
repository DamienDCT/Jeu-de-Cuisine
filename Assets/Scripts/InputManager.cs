using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private Camera _camera;

    [SerializeField] private LayerMask placementLayerMask;
    private Vector3 lastPosition;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public Vector3 GetSelectedPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _camera.nearClipPlane;
        Ray ray = _camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
    /*public Vector3 GetSelectedPosition()
    {
        // Bloque si la souris est sur l'UI
        if (EventSystem.current.IsPointerOverGameObject())
            return lastPosition;

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }*/

}
