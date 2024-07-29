using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 1. 문이다.
 */
public class Door : ActivityObject
{ 
    public override void SetActive(DoorInfo _info)
    {
        StopAllCoroutines();

        //이전 패턴에 대한 정보가 존재한다면, 비활성화
        if (info != null) type_obj[(int)info.TYPE].SetActive(false);

        m_collider.enabled = true;

        info = _info;

        anim.SetTrigger(HASH_IDLE);

        //위치 지정
        transform.position = new Vector3(transform.position.x, transform.position.y, info.Z_POSITION);

        //패턴 표시
        type_obj[(int)info.TYPE].SetActive(true);

        ACTIVE = true;  //문이 활성화 되었는지 확인한다.

        TouchAction.instance.StackDoor(this);   //활성화된 스크립트 순서대로 쌓는다.
    }

    public override void Opening()
    {
        base.Opening();
        //플레이어가 너무 멀어지면, 속도가 증가

        anim.SetTrigger(HASH_OPEN);

        SoundManager.instance.DoorOpen();
        PlayerController.instance.DoorOpen();
        Monster.instance.DoorOpenSpeed();

        UIManager.instance.inGameUI.OpenDoor(); // 문을 열어 점수를 추가한다.

        if (info.isLast) // 마지막 문을 경우 문을 새로 생성
        {
            GameManager.instance.Action_ALLClearDoor();
            MapManager.instance.AddActivityObject();
        }

        StartCoroutine(ReBatchCroutine());
    }
    WaitForSeconds time_rebacth = new WaitForSeconds(0.1f);
    IEnumerator ReBatchCroutine()//플레이어 시야에 보이지 않을 때만 배치가 가능하도록 함.
    {
        while (true)
        {
            yield return time_rebacth;
            if ((PlayerController.instance.transform.position.z - transform.position.z) > MapManager.instance.reBatchinterval)
            {
                Debug.Log("ReBatch");
                ACTIVE = false;     //재 배치가 가능하도록 함.
                break;
            }
        }
    }
}
