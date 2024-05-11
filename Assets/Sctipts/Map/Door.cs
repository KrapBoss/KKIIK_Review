using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 1. ���̴�.
 */
public class Door : ActivityObject
{ 
    public override void SetActive(DoorInfo _info)
    {
        StopAllCoroutines();

        //���� ���Ͽ� ���� ������ �����Ѵٸ�, ��Ȱ��ȭ
        if (info != null) type_obj[(int)info.TYPE].SetActive(false);

        m_collider.enabled = true;

        info = _info;

        anim.SetTrigger(HASH_IDLE);

        //��ġ ����
        transform.position = new Vector3(transform.position.x, transform.position.y, info.Z_POSITION);

        //���� ǥ��
        type_obj[(int)info.TYPE].SetActive(true);

        ACTIVE = true;  //���� Ȱ��ȭ �Ǿ����� Ȯ���Ѵ�.

        TouchAction.instance.StackDoor(this);   //Ȱ��ȭ�� ��ũ��Ʈ ������� �״´�.
    }

    public override void Opening()
    {
        base.Opening();
        //�÷��̾ �ʹ� �־�����, �ӵ��� ����

        anim.SetTrigger(HASH_OPEN);

        SoundManager.instance.DoorOpen();
        PlayerController.instance.DoorOpen();
        Monster.instance.DoorOpenSpeed();

        UIManager.instance.inGameUI.OpenDoor(); // ���� ���� ������ �߰��Ѵ�.

        if (info.isLast) // ������ ���� ��� ���� ���� ����
        {
            GameManager.instance.Action_ALLClearDoor();
            MapManager.instance.AddActivityObject();
        }

        StartCoroutine(ReBatchCroutine());
    }
    WaitForSeconds time_rebacth = new WaitForSeconds(0.1f);
    IEnumerator ReBatchCroutine()//�÷��̾� �þ߿� ������ ���� ���� ��ġ�� �����ϵ��� ��.
    {
        while (true)
        {
            yield return time_rebacth;
            if ((PlayerController.instance.transform.position.z - transform.position.z) > MapManager.instance.reBatchinterval)
            {
                Debug.Log("ReBatch");
                ACTIVE = false;     //�� ��ġ�� �����ϵ��� ��.
                break;
            }
        }
    }
}
