using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDoor : ActivityObject
{
    [Space]
    [Header("발소리를 작게 만든다.")]
    public bool isSlowFootstep =false;    

    public override void Opening()
    {
        base.Opening();
        anim.SetTrigger("Open");

        SoundManager.instance.DoorOpen();
        PlayerController.instance.DoorOpen();

        if (isSlowFootstep) SoundManager.instance.FootSlow(); // 발소리 작게
        else { StartCoroutine(FootSlowStepRestoreCroutine()); }
    }


    public override void SetActive(DoorInfo _info)
    {
        anim.SetTrigger("Idle");

        m_collider.enabled = true;

        //문을 터치를 할 경우 열리도록 한다.
        if(_info == null)info = new DoorInfo();
    }

    //발걸음 소리가 느려져 있을 때 다시 원래대로 되돌린다.
    IEnumerator FootSlowStepRestoreCroutine()
    {
        // 플레이어 위치가 나의 위치보다 앞에 있을 경우에 소리를 원상복구
        yield return new WaitUntil(() => (transform.position.z < PlayerController.instance.GetPositionZ()));

        SoundManager.instance.FootRestor();
    }
}
