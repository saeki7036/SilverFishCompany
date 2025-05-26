using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BeltDrawing : MonoBehaviour
{
    [SerializeField]
    Vector2Int maxMapSize = new(59, 59);

    [SerializeField]
    LineRenderer lineRenderer;

    const int ClampMin = 0;

    List<Vector3Int> SelectedPosList = new List<Vector3Int>();
    Dictionary<Vector3Int,int> posIndexMap = new Dictionary<Vector3Int, int>();


    Vector3Int currentPos = new(-1, -1, 0);

    public void InputRegister(MouseController input)
    {
        input.LeftDownEvent += BeltDrawSetup;
        input.LeftClickEvent += DrawingBelt;
        input.LeftUpEvent += OnMouseUp;
    }

    Vector3Int GetMapPosInt(Vector3 mouseWorldPos) => new()
    {
        x = Mathf.Clamp(Mathf.RoundToInt(mouseWorldPos.x), ClampMin, maxMapSize.x),
        y = Mathf.Clamp(Mathf.RoundToInt(mouseWorldPos.y), ClampMin, maxMapSize.y),
        z = 0,
    };

    void BeltDrawSetup(Vector3 mouseWorldDownPos)
    {
        Vector3Int gridPos = GetMapPosInt(mouseWorldDownPos);

        SelectedPosList = new List<Vector3Int>();
        posIndexMap = new Dictionary<Vector3Int, int>();

        posIndexMap.Add(gridPos, SelectedPosList.Count);
        SelectedPosList.Add(gridPos);
        currentPos = gridPos;
    }

    void DrawingBelt(Vector3 mouseWorldPos)
    {
        Vector3Int gridPos = GetMapPosInt(mouseWorldPos);

        if(SelectedPosList.Count != 0 && gridPos != currentPos)
        {
            if (posIndexMap.TryGetValue(gridPos, out int count))
            {
                for (int i = SelectedPosList.Count - 1; i >= count && i >= 0; i--)
                {
                    Vector3Int removalTargetPos = SelectedPosList[i];
                    SelectedPosList.RemoveAt(i);
                    posIndexMap.Remove(removalTargetPos);
                }
            }
            if (!posIndexMap.ContainsKey(gridPos))
            {
                posIndexMap.Add(gridPos, SelectedPosList.Count);
                SelectedPosList.Add(gridPos);
                currentPos = gridPos;
            }

            UpdateLineRenderer();
        }   
    }

    void OnMouseUp(Vector3 mouseWorldUpPos)
    {

    }

    void UpdateLineRenderer()
    {
        lineRenderer.positionCount = SelectedPosList.Count;

        // Vector3Int Å® Vector3 ïœä∑ÇµÇ¬Ç¬ê›íË
        for (int i = 0; i < SelectedPosList.Count; i++)
        {
            // çÇÇ≥í≤êÆÇµÇΩÇ¢èÍçáÇÕYÇ‚ZÇ…+0.5fÇ»Ç«Ç∑ÇÈ
            lineRenderer.SetPosition(i, SelectedPosList[i]);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // lineRenderer = GetComponent<LineRenderer>();

        // LineRendererÇÃèâä˙ê›íËÅiîCà”Åj
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }
}
