using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public GridData()
    {
        placedObjects.Add(new Vector3Int(5, 0, -5), null);
    }

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex, Direction direction, Vector3Int offsetPosition)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize, direction, offsetPosition, true);

       /* Debug.Log(gridPosition);
        Debug.Log(objectSize);
        Debug.Log(direction);
        Debug.Log(offsetPosition);*/

        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach(var position in positionToOccupy) 
        {

            if (placedObjects.ContainsKey(position))
                throw new Exception("Dictionary already contains");
            placedObjects[position] = data;
        }
    }

    public List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize, Direction direction, Vector3Int offsetPosition, bool flag)
    {
        List<Vector3Int> positions = new List<Vector3Int>();

        Vector2Int _objectSize = Vector2Int.zero;
      /*  if(direction == Direction.WEST || direction == Direction.SOUTH)
        {
            int _x = objectSize.x;
            int _y = objectSize.y;
            objectSize.x = _y;
            objectSize.y = _x;
        }*/
        switch(direction)
        {
            case Direction.SOUTH:
                _objectSize.x = objectSize.x;
                _objectSize.y = -objectSize.y;
                break;
            case Direction.WEST:
                _objectSize.x = -objectSize.x;
                _objectSize.y = objectSize.y;
                break;
            case Direction.EAST:
                _objectSize.x = -objectSize.x;
                _objectSize.y = -objectSize.y;
                break;
            case Direction.NORTH:
            default:
                _objectSize.x = objectSize.x;
                _objectSize.y = objectSize.y;
                break;
        }


        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                Vector3Int adjustedPosition;
                switch (direction)
                {
                    case Direction.NORTH: // No rotation
                        adjustedPosition = gridPosition + new Vector3Int(x, 0, y);
                        break;

                    case Direction.SOUTH: // 90 degrees
                        adjustedPosition = gridPosition + new Vector3Int(y, 0, -x);
                        break;

                    case Direction.EAST: // 180 degrees
                        adjustedPosition = gridPosition + new Vector3Int(-x, 0, -y);
                        break;

                    case Direction.WEST: // 270 degrees
                        adjustedPosition = gridPosition + new Vector3Int(-y, 0, x);
                        break;

                    default:
                        adjustedPosition = gridPosition + new Vector3Int(x, 0, y);
                        break;
                }

                positions.Add(adjustedPosition);
            }
        }
        if(flag)
        {
            foreach(Vector3Int position in positions)
            {
                Debug.Log(position);
            }
        }

        return positions;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize, Direction direction, Vector3Int offsetPosition)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize, direction, offsetPosition, false);
        foreach(var position in positionToOccupy)
        {
            if(placedObjects.ContainsKey(position))
            {
                return false;
            }
        }
        return true;
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;

    public int ID { get; private set; }
    public int placedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int ID, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        this.ID = ID;
        this.placedObjectIndex = placedObjectIndex;
    }
}