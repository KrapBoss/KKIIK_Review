using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ���� ���� �⺻���� ������ ������ ������.
 */

[SerializeField]
abstract public class ActivityObject : MonoBehaviour
{
    [Header("������ �־��ּ���.")]
    public string NAME;//���� ������Ʈ�� �̸�
    public Animator anim;//���� �ش��ϴ� �ִϸ��̼�

    [Space]
    [Header("������ �־��ּ���. 1.left / 2.right / 3.up / 4. down")]
    public GameObject[] type_obj;//DoorType�� �ش��ϴ� ������Ʈ
    public bool ACTIVE = false; //���� Ȱ��ȭ �Ǿ��°�?

    public Collider m_collider;

    protected DoorInfo info = null;//������ ������ ����.

    protected readonly int HASH_SPEED = Animator.StringToHash("Speed");
    protected readonly int HASH_OPEN = Animator.StringToHash("Open");
    protected readonly int HASH_IDLE = Animator.StringToHash("Idle");

    abstract public void SetActive(DoorInfo _info);//������Ʈ�� Ȱ��ȭ�� �� ���
    virtual public void Opening()
    {
        m_collider.enabled = false;
        anim.SetFloat(HASH_SPEED, PlayerController.instance.GetAllSpeed());
        //Debug.Log($"���� ������ {PlayerController.instance.GetSpeed()}");
    }// ���� ���� ��� ������ ������ �����Ѵ�.
    public DoorType GetDoorType() { return info.TYPE; }//���� Ÿ�� ������ �����´�.
    public bool IsLast() { return info.isLast; }
}

/*
 * ������ �����ϰ� ����
 */
public class Doors : MonoBehaviour
{
    List<ActivityObject> list_doors = new List<ActivityObject>();
    string MAP_NAME = null;

    public bool Create(int createNum, string mapName, int quater)        //���� �������ش�.
    {
        //**�б����� �ҷ����� ���� �߰�

        string _mapName = $"{mapName}/{ mapName}Door{ quater}";
        if (MAP_NAME == _mapName) return false;//���� �̹� ���� ��� ���� X
        Clear();

        Debug.LogWarning($"�����մϴ�. {_mapName}");
        GameObject door = Resources.Load<GameObject>(_mapName);
        if (door == null) return false;

        for (int i = 0; i < createNum; i++)
        {
            list_doors.Add(Instantiate<GameObject>(door, transform).GetComponent<Door>());
            list_doors[i].gameObject.SetActive(false);
        }

        MAP_NAME = _mapName;
        door = null;
        Resources.UnloadUnusedAssets();
        return true;
    }

    //���� �ʿ� ���� ������Ʈ�� ������ ��� ���� 
    void Clear()
    {
        if (list_doors.Count == 0) return;

        foreach (Door g in list_doors)
        {
            Destroy(g.gameObject);
        }
        list_doors.Clear();
        Resources.UnloadUnusedAssets();
    }

    //�ڵ����� ���� ������Ʈ �Ѵ�.
    public void RunUpdate()
    {
        StartCoroutine(RunUpdateCroutine());
    }
    WaitForSeconds time_run = new WaitForSeconds(0.2f);
    IEnumerator RunUpdateCroutine()
    {
        while (true)
        {
            yield return time_run;
            for(int i =0;i < list_doors.Count; i++)
            {
                //���� ��Ȱ��ȭ, ���� ���� �������
                if ((!list_doors[i].ACTIVE) && MapManager.instance.IsRemainDoor())
                {
                    list_doors[i].gameObject.SetActive(true);
                    //���� ���� ���� ������ ������ Ȱ��ȭ ��Ų��.
                    list_doors[i].SetActive(MapManager.instance.GetNextDoor());
                    //Debug.LogWarning("SetActive" + i);
                }
            }
        }
    }

    //������ ���ᰡ �Ǹ� ������Ʈ�� �����Ѵ�.
    public void RunUpdateStop(bool initialize)
    {
        //Debug.Log("DOORS ->RunUpdateStop");
        StopAllCoroutines();

        if (initialize)
        {
            for (int i = 0; i < list_doors.Count; i++)//���� ��Ȱ��ȭ ó��
            {
                list_doors[i].ACTIVE = false;
            }
        }
    }

    //��� ���� ��Ȱ��ȭ �Ǿ� ���� ��츦 ��ȯ��
    public bool IsAllDeActive()
    {
        for( int i  = 0;i<list_doors.Count; i++)
        {
            if (list_doors[i].ACTIVE) return false;
        }
        return true;
    }
}
