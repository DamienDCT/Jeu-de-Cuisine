using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class BuildSystem : MonoBehaviour
{
    [SerializeField] private Transform sphereCursor;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private Transform rectangleCursor;
    [SerializeField] private Transform displayGridPlane;
    [SerializeField] private Grid grid;

    private GameObject currentObjectSelected;
    private int currentObjectSelectedIndex;

    private List<GameObject> placedFurnitures = new();

    private GridData furnitureData;

    private GameObject previewObject;
    [SerializeField] private Material previewMaterial; 
    [SerializeField] private Material unvalidatePreviewMaterial;

    private bool isCurrentlyBuilding = false;

    private Dictionary<Direction, Vector3Int> offsetByDirection = new();
    private Dictionary<Direction, float> rotationByDirection = new();

    [SerializeField] private Direction currentPlacementDirection;

    private void Awake()
    {
        currentPlacementDirection = Direction.NORTH;
        furnitureData = new GridData();
        previewObject = null;

        offsetByDirection.Add(Direction.NORTH, new Vector3Int(0, 0, 0));
        offsetByDirection.Add(Direction.EAST, new Vector3Int(1, 0, 1));
        offsetByDirection.Add(Direction.SOUTH, new Vector3Int(0, 0, 1));
        offsetByDirection.Add(Direction.WEST, new Vector3Int(1, 0, 0));

        rotationByDirection.Add(Direction.NORTH, 0f);
        rotationByDirection.Add(Direction.EAST, 90f);
        rotationByDirection.Add(Direction.SOUTH, 180f);
        rotationByDirection.Add(Direction.WEST, 270f);

    }

    private void Start()
    {
        BuildUI.Instance.OnBuildChanged += BuildUI_OnBuildChanged;
        PlayerManagerInput.Instance.GetPlayerInput().actions["Rotate"].performed += OnRotateStructure;
    }

    private void OnRotateStructure(InputAction.CallbackContext obj)
    {
        if (previewObject == null)
            return;

        currentPlacementDirection = GetNextDirection(currentPlacementDirection);
        previewObject.transform.Rotate(new Vector3(0f, 90f, 0f));
    }

    private void BuildUI_OnBuildChanged(object sender, int index)
    {
        currentObjectSelected = ObjectDatabase.Instance.GetPrefabByIndex(index);
    //    EnableBuild();
        currentObjectSelectedIndex = index;
        PreparePreview();
    }

    private void Update()
    {
/*        if(!isCurrentlyBuilding)
        {
            return;
        }*/

        Vector3 pos = inputManager.GetSelectedPosition();

        Vector3Int gridPosition = grid.WorldToCell(pos);
        rectangleCursor.position = grid.CellToWorld(gridPosition);

        Vector3Int offsetPosition = offsetByDirection[currentPlacementDirection];
        float rotation = rotationByDirection[currentPlacementDirection];

        if (currentObjectSelected != null && previewObject != null)
        {
            previewObject.transform.position = gridPosition + new Vector3(0f, 0.08f, 0f) + offsetPosition;
            bool placementValidity = CheckPlacementValidity(gridPosition, currentObjectSelectedIndex);
            UpdatePreviewValidity(placementValidity);
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Bloque si le curseur est sur un élément UI
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (currentObjectSelected != null)
            {
                PlaceStructure();
                return;
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            DisableBuild();
        }
        //sphereCursor.position = pos;
    }

    private Direction GetNextDirection(Direction currentDirection)
    {
        // Cast the current direction to an integer, add 1, and use modulus to cycle
        return (Direction)(((int)currentDirection + 1) % Enum.GetNames(typeof(Direction)).Length);
    }

    private void UpdatePreviewValidity(bool placementValidity)
    {
        if (previewObject == null)
            return;

        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        if (placementValidity)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material = previewMaterial;
            }
        } else
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material = unvalidatePreviewMaterial;
            }
        }
    }

    public void DisableBuild()
    {
        isCurrentlyBuilding = false;
        currentObjectSelected = null;
        currentObjectSelectedIndex = -1;

        if(previewObject != null )
        {
            Destroy(previewObject);
            previewObject = null;
        }

        rectangleCursor.gameObject.SetActive(false);
        displayGridPlane.gameObject.SetActive(false);

        PlayerManagerInput.Instance.SwapActionMap("MovementActionMap");
    }

    public void EnableBuild()
    {
        isCurrentlyBuilding = true;
        rectangleCursor.gameObject.SetActive(true);
        displayGridPlane.gameObject.SetActive(true);
        PlayerManagerInput.Instance.SwapActionMap("BuildActionMap");
    }

    private void PreparePreview()
    {
        if (currentObjectSelected == null)
            return;
        currentPlacementDirection = Direction.NORTH;
        previewObject = Instantiate(currentObjectSelected);

        // Désactive tous les colliders de la preview
        Collider[] colliders = previewObject.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
            col.enabled = false;

        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
            renderer.material = previewMaterial;
    }

    private void PlaceStructure()
    {
        Vector3 pos = inputManager.GetSelectedPosition();

        Vector3Int gridPosition = grid.WorldToCell(pos);
        bool placementValidity = CheckPlacementValidity(gridPosition, currentObjectSelectedIndex);

        if(!placementValidity)
        {
            return;
        }

        rectangleCursor.position = grid.CellToWorld(gridPosition);
        GameObject newObject = Instantiate(currentObjectSelected);
        Vector3Int offsetPosition = offsetByDirection[currentPlacementDirection];
        newObject.transform.rotation = previewObject.transform.rotation;
        newObject.transform.position = gridPosition + offsetPosition;

        placedFurnitures.Add(newObject);
        ObjectData data = ObjectDatabase.Instance.GetObjectDataOfIndex(currentObjectSelectedIndex);
        Debug.Log(gridPosition);
        furnitureData.AddObjectAt(
            gridPosition,
            data.size,
            data.ID,
            placedFurnitures.Count + 1,
            currentPlacementDirection,
            offsetPosition);

        if (newObject.TryGetComponent<BaseCounter>(out BaseCounter baseCounter))
        {
            baseCounter.SetPlacedDirection(currentPlacementDirection);
        }
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int index)
    {
        return furnitureData.CanPlaceObjectAt(gridPosition, ObjectDatabase.Instance.GetObjectDataOfIndex(index).size, currentPlacementDirection, offsetByDirection[currentPlacementDirection]);
    }
}

[System.Serializable]
public enum Direction
{
    NORTH,
    SOUTH,
    EAST,
    WEST
}