using NUnit;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BeltDrawing : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    GameObject BeltPrehab;

    const int ClampMin = 0;

    Vector2Int maxMapSize => GridMapManager.Instance.MaxMapSize;

    float GridAdjustScale => GridMapManager.Instance.GridAdjustScale();

    List<Vector3Int> SelectedPosList = new List<Vector3Int>();
    Dictionary<Vector3Int, int> posIndexMap = new Dictionary<Vector3Int, int>();

    bool OnGridMap;

    Vector3Int currentPos = new(-1, -1, 0);

    public void InputRegister(MouseController input)
    {
        input.LeftDownEvent += BeltDrawSetup;
        input.LeftClickEvent += DrawingBelt;
        input.LeftUpEvent += DrowBeltGenerate;
    }

    Vector3Int GetMapGridInt(Vector3 mouseWorldPos)
    {
        // �C���f�b�N�X����̎擾�̂��� -1 �����Ă���
        Vector2Int maxMapIndex = maxMapSize - Vector2Int.one;

        return new()
        {
            x = Mathf.Clamp(Mathf.RoundToInt(mouseWorldPos.x), ClampMin, maxMapIndex.x),
            y = Mathf.Clamp(Mathf.RoundToInt(mouseWorldPos.y), ClampMin, maxMapIndex.y),
            z = 0,
        };
    }

    int ManhattanDistance2D(Vector3Int gridPos, Vector3Int targetPos)
    {
        int x = Mathf.Abs(targetPos.x - gridPos.x);
        int y = Mathf.Abs(targetPos.y - gridPos.y);

        return x + y;
    }

    bool IsInGridMap(Vector3 mouseWorldDownPos)
    {
        if(mouseWorldDownPos.x < -GridAdjustScale || 
            maxMapSize.x - GridAdjustScale < mouseWorldDownPos.x)
            return false;

        if(mouseWorldDownPos.y < -GridAdjustScale || 
            maxMapSize.y - GridAdjustScale < mouseWorldDownPos.y)
            return false;

        return true;
    }



    // 2�̃O���b�h���W���c���̓����݂̂őΊp���ɋ߂Â��Ȃ���q���o�H�𐶐����鏈��
    List<Vector3Int> CorrectionBeltLine(Vector3Int endPos, Vector3Int startPos)
    {
        // �u���[���n���̐����A���S���Y��(Bresenham's Line Algorithm)���΂߈ړ��֎~�Ŏ���
        List<Vector3Int> path = new List<Vector3Int>();

        // �ړ��ʂ̐�Βl�i�����j
        int distance_x = Mathf.Abs(endPos.x - startPos.x);
        int distance_y = Mathf.Abs(endPos.y - startPos.y);

        // x�����Ey�����ɐi�ނׂ������i+1 or -1�j
        int step_x = startPos.x < endPos.x ? 1 : -1;
        int step_y = startPos.y < endPos.y ? 1 : -1;

        // �J�n���W�̃R�s�[
        int current_x = startPos.x;
        int current_y = startPos.y;

        // �덷�̏����l�ix������y�����̋��������g���j
        int errorValue = distance_x - distance_y;

        // �I�_�ɓ��B����܂Ń��[�v
        while (current_x != endPos.x || current_y != endPos.y)
        {          
            // �덷��2�{���Ĕ�r�i�����Ŏ΂߂̌X�����ߎ��j
            int errorDouble = errorValue * 2;

            // ���ݒn��1�}�X���ǉ��i�c�܂��͉��̂����ꂩ�j
            // ----------------------------------------
            // 1�}�X�i�ޏ���
            // ----------------------------------------
            // �u�덷 �~ 2�v�� -distance_y ���� distance_x �Ɣ�r����
            // �ʕ����ɐi�ނׂ��X���ɑ΂��āA�i�ޕ����̃Y�����܂����e�͈͓��Ȃ̂ŁA
            // �i�ޕ����ɐi��ł��΂߂̐��ɋ߂Â���Ɣ��f����B
            //
            // �܂�F
            //  - �덷���傫���Ȃ�O�ɐi��
            //  - ���͕ʐi�ޕ������i�ޕ�����D�悷�ׂ��i�K
            //
            // �� 1�}�X�ړ����āA�덷���甽�Ε���������������
            if (errorDouble > -distance_y)
            {
                // x������1�}�X�ړ�

                // Y�����������Č덷���X�V�iX�֐i�񂾕��Y����j
                errorValue -= distance_y;

                // X���W��1�}�X�i�߂�i�����E�j
                current_x += step_x;

                // �V�����ʒu�����X�g�ɒǉ�
                path.Add(new Vector3Int(current_x, current_y, 0));
            }
            else if (errorDouble < distance_x)
            {
                // y������1�}�X�ړ�

                // X�����������Č덷���X�V�iY�֐i�񂾕��Y����j
                errorValue += distance_x;

                // Y���W��1�}�X�i�߂�i�ォ���j
                current_y += step_y;

                // �V�����ʒu�����X�g�ɒǉ�
                path.Add(new Vector3Int(current_x, current_y, 0));
            }
        }

        return path;
    }





    void BeltDrawSetup(Vector3 mouseWorldDownPos)
    {     
        SelectedPosList = new List<Vector3Int>();
        posIndexMap = new Dictionary<Vector3Int, int>();

        // ���C�������_���[��S�����i�`������Z�b�g�j
        lineRenderer.positionCount = 0;

        OnGridMap = IsInGridMap(mouseWorldDownPos);

        if (OnGridMap)
        {
            Vector3Int gridPos = GetMapGridInt(mouseWorldDownPos);

            posIndexMap.Add(gridPos, SelectedPosList.Count);
            SelectedPosList.Add(gridPos);
            currentPos = gridPos;
        } 
    }

    // �x���g�i�I�����C���j��`�悷�鏈���B�}�E�X�̃��[���h���W�����ɑI�����C���̍X�V���s���B
    void DrawingBelt(Vector3 mouseWorldPos)
    {
        if(!OnGridMap)     
            return;
        
        // �}�E�X�̃��[���h���W���O���b�h���W�i�����j�ɕϊ�
        Vector3Int gridPos = GetMapGridInt(mouseWorldPos);

        // ���łɉ��炩�̈ʒu���I������Ă�ꍇ�ɏ��������s
        if (SelectedPosList.Count != 0)
        {
            // ����̈ʒu���O��̈ʒu�Ɠ����ꍇ�ɏ������I��
            if (gridPos == currentPos)       
                return;

            // ����̈ʒu����Ίp�����W�O�U�O�ɕ␳�����o�H���擾
            List<Vector3Int> correctionPathList = CorrectionBeltLine(gridPos, currentPos);

            // �o�H1�}�X���ƂɂɃO���b�h���W������
            foreach (Vector3Int nextPos in correctionPathList)
            {
                // ���łɑI�����X�g��ɂ��̃O���b�h���W���܂܂�Ă����ꍇ�i���[�g�������߂��悤�ȑ���j
                if (posIndexMap.TryGetValue(nextPos, out int count))
                {
                    // �Y���ʒu�ȍ~�̃f�[�^�����ׂč폜����i�����߂��j
                    for (int i = SelectedPosList.Count - 1; i >= count && i >= 0; i--)
                    {
                        Vector3Int removalTargetPos = SelectedPosList[i];
                        SelectedPosList.RemoveAt(i);
                        posIndexMap.Remove(removalTargetPos);
                    }
                }

                // �܂��I�����X�g�Ɋ܂܂�Ă��Ȃ����W�̏ꍇ�A�V���ɒǉ�
                if (!posIndexMap.ContainsKey(nextPos))
                {
                    // ���W���}�b�v�ɓo�^���A�I�����X�g�ɂ��ǉ�
                    posIndexMap.Add(nextPos, SelectedPosList.Count);
                    SelectedPosList.Add(nextPos);

                    // ���݈ʒu���X�V
                    currentPos = nextPos;
                }
            }

            // ���C�������_���[���X�V���ĕ`��𔽉f
            UpdateLineRenderer();
        }   
    }

    void DrowBeltGenerate(Vector3 mouseWorldUpPos)
    {
        if (BeltPrehab == null)
            return;

        List<GameObject> BeltList = new List<GameObject>();

        foreach (Vector3Int posInt in SelectedPosList)
        {
            GameObject BeltObject = Instantiate(BeltPrehab,posInt,Quaternion.identity);
            BeltList.Add(BeltObject);
        }

        GridMapManager.Instance.BeltSetting(SelectedPosList, BeltList);

        // LineRenderer�̕`���������
        lineRenderer.positionCount = 0;
    }

    // ���C�������_���[���X�V���ĕ`������鏈��
    void UpdateLineRenderer()
    {
        lineRenderer.positionCount = SelectedPosList.Count;

        // Vector3Int �� Vector3 �ϊ����ݒ�
        for (int i = 0; i < SelectedPosList.Count; i++)
        {
            // ���������������ꍇ��Z��+0.5f�Ȃǂ���
            lineRenderer.SetPosition(i, SelectedPosList[i]);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnGridMap = false;
        // lineRenderer = GetComponent<LineRenderer>();

        // LineRenderer�̏����ݒ�i�C�Ӂj
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }
}
