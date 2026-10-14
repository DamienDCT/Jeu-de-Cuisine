using UnityEngine;

public class PlateCounter : BaseCounter
{
    [SerializeField] private Transform detectStartPoint;
    [SerializeField] private LayerMask plateStockageLayerMask;

    [Header("Prefabs")]
    [SerializeField] private Plate platePrefab;

    [Header("Opening animation")]
    [SerializeField] private Transform couvercle;

    private PlateStockage linkedPlateStockage;
    private bool canGivePlate = false;

    private void Start()
    {
        SearchingPlateStockage();
    }

    public override void Interact()
    {
        if (!canGivePlate)
        {
            Debug.Log("we can't give plate now");
            return;
        }


        if(linkedPlateStockage != null)
        {
            if(linkedPlateStockage.IsStockageAvailable())
            {
                // Faire spawn une assiette
                Plate plate = Instantiate(platePrefab, linkedPlateStockage.GetParentTransform(), false);
                canGivePlate = false;
                plate.transform.localPosition = Vector3.zero;
                linkedPlateStockage.AddPlate(plate);
                LeanTween.rotateX(couvercle.gameObject, -15f, 0.4f).setEaseOutExpo().setOnComplete(() =>
                {
                    LeanTween.rotateX(couvercle.gameObject, -90f, 0.4f).setEaseOutExpo().setOnComplete(() =>
                    {
                        canGivePlate = true;
                    });
                });
            }
        } else
        {
            Debug.Log("le link plate stockage est null");
        }
    }

    private void SearchingPlateStockage()
    {
        RaycastHit hit;

        if (placedDirection == Direction.NORTH || placedDirection == Direction.SOUTH)
        {
            if (Physics.Raycast(detectStartPoint.position, Vector3.right, out hit, plateStockageLayerMask))
            {
                SetupPlateStockage(hit);
            } else if (Physics.Raycast(detectStartPoint.position, Vector3.left, out hit, plateStockageLayerMask))
            {
                SetupPlateStockage(hit);
            }            
        } else
        {
            if (Physics.Raycast(detectStartPoint.position, Vector3.forward, out hit, plateStockageLayerMask))
            {
                SetupPlateStockage(hit);
            }
            else if (Physics.Raycast(detectStartPoint.position, Vector3.back, out hit, plateStockageLayerMask))
            {
                SetupPlateStockage(hit);
            }
        }
/*            Debug.DrawRay(detectStartPoint.position, Vector3.right, Color.blue);
        Debug.DrawRay(detectStartPoint.position, Vector3.left, Color.green);
        Debug.DrawRay(detectStartPoint.position, Vector3.back, Color.orange);
        Debug.DrawRay(detectStartPoint.position, Vector3.forward, Color.red);*/
    }

    private void SetupPlateStockage(RaycastHit hit)
    {
        PlateStockage plateStockage = hit.transform.GetComponent<PlateStockage>();
        Debug.Log(hit.transform.gameObject.name);

        if (plateStockage != null)
        {
            this.linkedPlateStockage = plateStockage;
            this.linkedPlateStockage.SetPlateCounter(this);
            canGivePlate = true;
        }
    }

    protected override void PlaceIngredientOn(Ingredient ingredientOn)
    {
        Debug.Log("testtestst");
        return;
    }
}
