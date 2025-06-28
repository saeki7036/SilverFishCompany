using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Unity�̃G�f�B�^�[���N�����ꂽ�Ƃ��ɏ���������鑮��
[InitializeOnLoad]
public class CopipeView : EditorWindow
{
    // ���j���[�ɃG�f�B�^�E�B���h�E��ǉ��iEditor > OriginalEditerPanels > CopipeView�j
    [MenuItem("Editor/OriginalEditerPanels/CopipeView")]
    public static void ShowWindow()
    {
        // �E�B���h�E���J���i���ɊJ���Ă���΃t�H�[�J�X����j
        GetWindow<CopipeView>("CopipeView");
    }

    // �z�u����GameObject
    private GameObject targetPrefab;

    // ��n�_�ɂ��z�u���邩�ǂ����̃t���O
    private bool includeBasePosition;

    // �I�u�W�F�N�g�̖��O��ύX���邩�ǂ����̃t���O
    private bool renameInstance;

    // �z�u���i�e�������j
    private Vector3Int copyCount;

    // �e�I�u�W�F�N�g�Ԃ̊Ԋu
    private Vector3 offsetPerAxis;

    // �I�u�W�F�N�g�̔z�u��ʒu
    private Vector3 originPosition;

    // �G�f�B�^�E�B���h�E��GUI�`�揈��
    private void OnGUI()
    {
        using (new EditorGUILayout.VerticalScope())
        {
            // �ΏۃI�u�W�F�N�g�̃t�B�[���h
            targetPrefab = EditorGUILayout.ObjectField("�v���n�u", targetPrefab, typeof(GameObject), true) as GameObject;
           
            // �z�u��_�ɃI�u�W�F�N�g��z�u���邩�ǂ���
            includeBasePosition = EditorGUILayout.Toggle("��ʒu�ɂ��z�u", includeBasePosition);

            // �e�������̔z�u�Ԋu
            offsetPerAxis = EditorGUILayout.Vector3Field("�z�u�Ԋu", offsetPerAxis);

            // �z�u�̊�ʒu
            originPosition = EditorGUILayout.Vector3Field("��ʒu", originPosition);

            // �e�������̔z�u��
            copyCount = EditorGUILayout.Vector3IntField("�R�s�[��", copyCount);

            // ���̐��������Ȃ�
            for (int i = 0; i < 3; i++)
            {
                if (copyCount[i] < 0)
                    copyCount[i] = 0;
            }

            // ���������I�u�W�F�N�g�̖��O��ύX���邩�ǂ���
            renameInstance = EditorGUILayout.Toggle("�I�u�W�F�N�g�̖��O��ύX", renameInstance);

            // �u�����v�{�^����\�����A�����ꂽ��z�u���������s
            if (GUILayout.Button("����"))
            {
                if (targetPrefab != null)
                {
                    GenerateCopies(includeBasePosition, copyCount, offsetPerAxis, originPosition, targetPrefab);
                }
            }
        }
    }

    // �I�u�W�F�N�g��z�u���鏈��
    private void GenerateCopies(bool includeOrigin, Vector3Int countPerAxis, Vector3 spacingPerAxis, Vector3 startPosition, GameObject prefabToInstantiate)
    {
        // �z�u�����[���Ȃ牽�����Ȃ�
        if (countPerAxis == Vector3Int.zero) return;

        // ��������ʒu��ێ����郊�X�g
        List<Vector3> spawnPositions = new List<Vector3>();

        // �O�d���[�v�őS�����ɔz�u�ʒu���v�Z
        for (int x = countPerAxis.x; x >= 0; x--)
        {

            if (IsInitialPositionSkip(x, countPerAxis.x))
                continue;

            for (int y = countPerAxis.y; y >= 0; y--)
            {

                if (IsInitialPositionSkip(y, countPerAxis.y))
                    continue;

                for (int z = countPerAxis.z; z >= 0; z--)
                {

                    if (IsInitialPositionSkip(z, countPerAxis.z))
                        continue;

                    // �z�u���W���v�Z
                    Vector3 spawnPos = new Vector3(
                        startPosition.x + spacingPerAxis.x * x,
                        startPosition.y + spacingPerAxis.y * y,
                        startPosition.z + spacingPerAxis.z * z);

                    // �d��������邽�߁A���ɒǉ����ꂽ���W�łȂ���΃��X�g�ɒǉ�
                    if (!spawnPositions.Contains(spawnPos))
                        spawnPositions.Add(spawnPos);
                }
            }
        }

        // �z�u��_�ɃI�u�W�F�N�g��z�u���Ȃ��ݒ�̏ꍇ�A���X�g����폜
        if (!includeOrigin) spawnPositions.Remove(startPosition);

        // �I�u�W�F�N�g�𐶐�
        CreateObjects(prefabToInstantiate, spawnPositions);

        Debug.Log(spawnPositions.Count + " �������܂����B");
    }

    // �e���̍ő�l�ilimit�j�ɒB�����Ƃ��ɍŏ��̃��[�v���X�L�b�v���邽�߂̊֐�
    private bool IsInitialPositionSkip(int index, int max)
    {
        return index == max && index > 0;
    }

    // ���ۂ�GameObject���w��ʒu�ɕ����������鏈��
    private void CreateObjects(GameObject prefabToInstantiate, List<Vector3> spawnPositions)
    {
        foreach (Vector3 position in spawnPositions)
        {
            // �v���n�u���e���W�ɃG�f�B�^��ɐ����i�����N�ێ��j
            
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabToInstantiate);

            // �v���n�u�ɖ��O��ύX
            if (renameInstance)
            {
                instance.name += "_x" + position.x.ToString() +
                                 "_y" + position.y.ToString() +
                                 "_z" + position.z.ToString();
            }
                
            Undo.RegisterCreatedObjectUndo(instance, "Create Prefab Instance");
            instance.transform.position = position;

            // �C���X�^���X�����̏ꍇ�͉�
            //Instantiate(prefabToInstantiate, position, Quaternion.identity);
        }
    }
}
