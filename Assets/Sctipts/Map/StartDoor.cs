using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDoor : ActivityObject
{
    [Space]
    [Header("�߼Ҹ��� �۰� �����.")]
    public bool isSlowFootstep =false;    

    public override void Opening()
    {
        base.Opening();
        anim.SetTrigger("Open");

        SoundManager.instance.DoorOpen();
        PlayerController.instance.DoorOpen();

        if (isSlowFootstep) SoundManager.instance.FootSlow(); // �߼Ҹ� �۰�
        else { StartCoroutine(FootSlowStepRestoreCroutine()); }
    }


    public override void SetActive(DoorInfo _info)
    {
        anim.SetTrigger("Idle");

        m_collider.enabled = true;

        //���� ��ġ�� �� ��� �������� �Ѵ�.
        if(_info == null)info = new DoorInfo();
    }

    //�߰��� �Ҹ��� ������ ���� �� �ٽ� ������� �ǵ�����.
    IEnumerator FootSlowStepRestoreCroutine()
    {
        // �÷��̾� ��ġ�� ���� ��ġ���� �տ� ���� ��쿡 �Ҹ��� ���󺹱�
        yield return new WaitUntil(() => (transform.position.z < PlayerController.instance.GetPositionZ()));

        SoundManager.instance.FootRestor();
    }
}
