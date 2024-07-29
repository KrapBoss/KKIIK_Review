using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartRoom : MonoBehaviour
{
    [Header("문과 땅이 생성될 위치")]
    [SerializeField]float Create_Z;
    [SerializeField]float Monster_Z;
    public StartDoor[] startDoor;

    
    //시작 문에 대한 정보를 넘겨준다.
    public void StackDoors()
    {
        for(int i = 0;i < startDoor.Length; i++)
        {
            TouchAction.instance.StackDoor(startDoor[i]);
        }
    }

    // 게임 시작 시 시작 맵을 정리한다.
    public void StartSetting()
    {
        //1. 문 정렬
        for (int i = 0; i < startDoor.Length; i++) startDoor[i].SetActive(null);

        //2. 문 정보 전달
        StackDoors();
    }



    #region GetSet
    public float GetStart_Z()   // 맵 생성이 시작되는 위치를 넘겨줌
    {
        return Create_Z;
    }

    public float GetMonster_Z()
    {
        return Monster_Z;
    }
    #endregion 
    /*
     스테이지 진행마다 오브젝트 추가
     */
}
