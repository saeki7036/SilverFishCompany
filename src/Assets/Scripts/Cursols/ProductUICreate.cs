using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProductUICreate : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer contentSpriteShadow;

    [SerializeField]
    BeltDrawing beltDrawing;

    [SerializeField]
    Color enabledColor = Color.blue;

    [SerializeField]
    Color disabledColor = Color.cyan;

    GameObject contentPrehab;

    GridContent gridContent;

    bool CreateFlag;

    List<ItemRequest> requests;

    float GridAdjustScale => GridMapManager.Instance.GridAdjustScale();

    Vector2Int MaxMapSize => GridMapManager.Instance.mapSize;

    public  bool IsCreated()=> !CreateFlag && !beltDrawing.GetDrawFlag();

    void ResetSpritePos()
    {
        int OutCameraPosValue = -10;

        transform.position = new Vector3Int()
        {
            x = OutCameraPosValue,
            y = OutCameraPosValue,
            z = 0
        };
    }

    public void SetCreateContent(List<ItemRequest> list,GameObject gameObject = null, Sprite sprite = null)
    {
        CreateFlag = (gameObject != null && sprite != null);

        requests = new List<ItemRequest>(list);
        contentPrehab = gameObject;
        contentSpriteShadow.sprite = sprite;

        if (CreateFlag == false)
            return;

        gridContent = contentPrehab.GetComponent<GridContent>();

        BeltSetting();
    }

    void BeltSetting()
    {
        if (gridContent.IsBelt())
        {
            beltDrawing.SetDrawFlag(true);
            contentSpriteShadow.enabled = false;
            CreateFlag = false;
        }
        else
        {
            beltDrawing.SetDrawFlag(false);
            contentSpriteShadow.enabled = true;
        }   
    }

    void EmptyContent()
    {
        contentSpriteShadow.enabled = false;
        contentSpriteShadow.sprite = null;
        gridContent = null;
        beltDrawing.SetDrawFlag(false);
        CreateFlag = false;
        requests = new List<ItemRequest>();
    }

    public void InputRegister(MouseController input)
    {
        input.LeftDownEvent += DragSpriteRenderer;
        input.LeftClickEvent += SetCreateTransform;
        input.LeftUpEvent += CreateProduct;
    }

    bool CheckItemRequests() => ItemManager.Instance.CanConsumeAll(requests);

    bool ConsumeItemRequests() => ItemManager.Instance.ItemConsumeAll(requests);
   
    Vector2Int Cursol2DInt(Vector3 mouseWorldDownPos) => new Vector2Int()
    {
        x = Mathf.RoundToInt(mouseWorldDownPos.x),
        y = Mathf.RoundToInt(mouseWorldDownPos.y)
    };

    bool IsInGridMap(Vector3 mouseWorldPos)
    {
        Vector2Int GridRange = gridContent.Content.GridSize - Vector2Int.one;  

        if (mouseWorldPos.x < -GridAdjustScale ||
            MaxMapSize.x - GridAdjustScale < mouseWorldPos.x + GridRange.x)
            return false;

        if (mouseWorldPos.y < -GridAdjustScale ||
            MaxMapSize.y - GridAdjustScale < mouseWorldPos.y + GridRange.y)
            return false;

        return true;
    }

    bool IsCanCreateTile(Vector2Int cursolPos)
    {
        TileType tileType = gridContent.Content.CanCreateTileType();

        if(tileType == TileType.None)
            return true;

        var gridMap = GridMapManager.Instance;

        for (int x = cursolPos.x; x < cursolPos.x + gridContent.Content.GridSize.x; x++)
        {
            for (int y = cursolPos.y; y < cursolPos.y + gridContent.Content.GridSize.y; y++)
            {
                Vector2Int vector2Int = new Vector2Int()
                {
                    x = x,
                    y = y,
                };

                if(!gridMap.IsInBounds(vector2Int))
                    return false;

                if(!gridMap.GetCell(vector2Int).SameTileType(tileType))
                    return false;
            }
        }
     
        return true;
    }

    void DragSpriteRenderer(Vector3 mouseWorldDownPos)
    {
        if (!CreateFlag)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            CreateFlag = false;
            return;
        }

        Vector2Int cursol2DInt = Cursol2DInt(mouseWorldDownPos);

        transform.position = new Vector3Int()
        {
            x = cursol2DInt.x,
            y = cursol2DInt.y,
            z = 0
        };
    }

    void SetCreateTransform(Vector3 mouseWorldPos)
    {
        if (!CreateFlag)
            return;

        bool isInMap = IsInGridMap(mouseWorldPos);

        Vector2Int cursol2DInt = Cursol2DInt(mouseWorldPos);

        bool isCanCreate = false;

        if (isInMap)
        {
            isCanCreate = IsCanCreateTile(cursol2DInt);
        }

        if (isInMap && isCanCreate && CheckItemRequests())
            contentSpriteShadow.color = enabledColor;
        else
            contentSpriteShadow.color = disabledColor;

        transform.position = new Vector3Int()
        {
            x = cursol2DInt.x,
            y = cursol2DInt.y,
            z = 0
        };
    }

    void CreateProduct(Vector3 mouseWorldUpPos)
    {
        if (!CreateFlag) 
        {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!IsInGridMap(mouseWorldUpPos))
        {
            return;
        }

        Vector2Int cursol2DInt = Cursol2DInt(mouseWorldUpPos);

        if (!IsCanCreateTile(cursol2DInt))
        {
            return;
        }
        if(!CheckItemRequests() || !ConsumeItemRequests())
        {
            return;
        }

        transform.position = new Vector3Int()
        {
            x = cursol2DInt.x,
            y = cursol2DInt.y,
            z = 0
        };
        
        GameObject Content = Instantiate(contentPrehab, transform.position, Quaternion.identity);

        EmptyContent();

        ResetSpritePos();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateFlag = false;
        requests = new List<ItemRequest>();
    }
}
