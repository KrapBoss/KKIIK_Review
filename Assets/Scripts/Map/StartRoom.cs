using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartRoom : MonoBehaviour
{
    [Header("���� ���� ������ ��ġ")]
    [SerializeField]float Create_Z;
    [SerializeField]float Monster_Z;
    public StartDoor[] startDoor;

    
    //���� ���� ���� ������ �Ѱ��ش�.
    public void StackDoors()
    {
        for(int i = 0;i < startDoor.Length; i++)
        {
            TouchAction.instance.StackDoor(startDoor[i]);
        }
    }

    // ���� ���� �� ���� ���� �����Ѵ�.
    public void StartSetting()
    {
        //1. �� ����
        for (int i = 0; i < startDoor.Length; i++) startDoor[i].SetActive(null);

        //2. �� ���� ����
        StackDoors();
    }



    #region GetSet
    public float GetStart_Z()   // �� ������ ���۵Ǵ� ��ġ�� �Ѱ���
    {
        return Create_Z;
    }

    public float GetMonster_Z()
    {
        return Monster_Z;
    }
    #endregion 
    /*
     �������� ���ึ�� ������Ʈ �߰�
     */
}
