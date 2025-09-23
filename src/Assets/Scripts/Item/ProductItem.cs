using UnityEngine;

/// <summary>
/// 生産アイテムを管理するクラス
/// アイテムの移動、レベルアップ、オブジェクト管理を行う
/// </summary>
public class ProductItem
{
    int level;
    ItemCategory category;
    GameObject itemObject;

    Vector3 moveTargetPos;
    Vector3 moveBeforePos;

    float maxTimeCount;
    float currentTimeCount;

    bool isMoveFlag;
    bool isUpdateFlag;

    ItemInformation nextLevelInfomation;

    // コンストラクタ

    /// <summary>
    /// ProductItemコンストラクタ - アイテム情報から初期化
    /// </summary>
    /// <param name="information">アイテム情報</param>
    /// <param name="CreateObjectPos">生成位置</param>
    /// <param name="MaxTimeCount">移動時間の最大値</param>
    public ProductItem(ItemInformation information, Vector2Int CreateObjectPos, float MaxTimeCount)
    {
        level = information.GetItemLevel();
        category = information.GetItemCategory();
        maxTimeCount = MaxTimeCount;
        isMoveFlag = false;
        isUpdateFlag = false;
        currentTimeCount = 0f;

        // Vector2IntからVector3に変換してオブジェクト生成位置を設定
        Vector3 instantiatePos = new Vector3
        { 
            x = CreateObjectPos.x,
            y = CreateObjectPos.y,
            z = 0,
        };

        moveBeforePos = instantiatePos;

        // アイテムオブジェクトを生成
        itemObject = GameObject.Instantiate(information.GetItemPrehab(), instantiatePos, Quaternion.identity);

        nextLevelInfomation = null;
    }

    // Getter,プロパティ

    /// <summary>
    /// アイテムカテゴリを取得
    /// </summary>
    /// <returns>アイテムカテゴリ</returns>
    public ItemCategory GetCategory() => category;

    /// <summary>
    /// アイテムレベルを取得
    /// </summary>
    /// <returns>現在のレベル</returns>
    public int GetLevel() => level;

    /// <summary>
    /// 次のレベルの情報を設定
    /// </summary>
    /// <param name="info">次レベルのアイテム情報</param>
    public void SetNextLevelInfo(ItemInformation info) => nextLevelInfomation = info;

    /// <summary>
    /// アイテムが移動中かどうかを判定
    /// </summary>
    /// <returns>移動中の場合true</returns>
    public bool IsItemMove() => isMoveFlag;

    /// <summary>
    /// アイテムがアップデート中かどうかを判定
    /// </summary>
    /// <returns>アップデート中の場合true</returns>
    public bool IsItemUpdate() => isUpdateFlag;

    /// <summary>
    /// 次レベル情報が設定されているかを判定
    /// </summary>
    /// <returns>設定されている場合true</returns>
    public bool IsSetNextLevelInfo() => nextLevelInfomation != null;

    /// <summary>
    /// アイテムオブジェクトが存在しないかを判定
    /// </summary>
    /// <returns>オブジェクトがない場合true</returns>
    public bool IsEnptyItemObject() => itemObject == null;

    /// <summary>
    /// 指定したアイテム情報と同じカテゴリで近いレベルかを判定
    /// </summary>
    /// <param name="itemInformation">比較するアイテム情報</param>
    /// <param name="difference">レベル差（デフォルト1）</param>
    /// <returns>条件を満たす場合true</returns>
    public bool IsSameCategoryAndNearLevel(ItemInformation itemInformation, int difference = 1)
    {
        if (itemInformation == null)
           return false;

        // カテゴリが異なる場合はfalse
        if (itemInformation.GetItemCategory() != category)
            return false;
        //Debug.Log(itemInformation.GetItemLevel()+"=="+ level + "+" + difference);

        // レベル差をチェック
        return itemInformation.GetItemLevel() != level + difference;
    }

    /// <summary>
    /// アイテムの移動処理 - Lerpを使用してスムーズに移動
    /// </summary>
    /// <param name="addTimeCount">追加する時間</param>
    public void ItemMovement(float addTimeCount)
    {
        if (isMoveFlag == false)
            return;

        // 時間を加算
        currentTimeCount += addTimeCount;

        // 移動の進行度を0-1の範囲で計算
        float Lerptime = Mathf.Clamp01(currentTimeCount / maxTimeCount);

        // 線形補間で位置を更新
        itemObject.transform.position = Vector3.Lerp(moveBeforePos, moveTargetPos, Lerptime);

        // 移動完了時の処理
        if (Lerptime >= 1f)
        {
            moveBeforePos = moveTargetPos;
            
            if (nextLevelInfomation == null)
                isMoveFlag = false;
            else
                isUpdateFlag = true;
        }
    }

    /// <summary>
    /// アイテムオブジェクトを破棄
    /// </summary>
    public void ItemObjectDestroy()
    {
        GameObject.Destroy(itemObject);
        itemObject = null;
    }

    /// <summary>
    /// アイテムの移動設定 - 移動先と移動開始フラグを設定
    /// </summary>
    /// <param name="targetPos">移動先座標</param>
    public void ItemMoveSetting(Vector3 targetPos)
    {
        if (isMoveFlag) 
            return;

        currentTimeCount = 0f;
        moveTargetPos = targetPos;
        isMoveFlag = true;
    }

    /// <summary>
    /// 次レベルへのアップグレード処理
    /// 現在のオブジェクトを破棄し、新しいレベルのオブジェクトを生成
    /// </summary>
    public void NextLevelSetting()
    {
        Vector3 itemPos = itemObject.transform.position;

        // 現在のオブジェクトを破棄
        ItemObjectDestroy();

        // 次レベルの情報で更新
        level = nextLevelInfomation.GetItemLevel();

        // 新しいレベルのオブジェクトを同じ位置に生成
        itemObject = GameObject.Instantiate(nextLevelInfomation.GetItemPrehab(), itemPos, Quaternion.identity);

        // 状態をリセット
        nextLevelInfomation = null;

        isMoveFlag = false;
        isUpdateFlag = false;
    }
}
