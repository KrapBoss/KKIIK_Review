using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 문에 대한 기본적인 변수와 동작을 가진다.
 */

[SerializeField]
abstract public class ActivityObject : MonoBehaviour
{
    [Header("변수를 넣어주세요.")]
    public string NAME;//현재 오브젝트의 이름
    public Animator anim;//문에 해당하는 애니메이션

    [Space]
    [Header("패턴을 넣어주세요. 1.left / 2.right / 3.up / 4. down")]
    public GameObject[] type_obj;//DoorType에 해당하는 오브젝트
    public bool ACTIVE = false; //문이 활성화 되었는가?

    public Collider m_collider;

    protected DoorInfo info = null;//문들의 정보를 가짐.

    protected readonly int HASH_SPEED = Animator.StringToHash("Speed");
    protected readonly int HASH_OPEN = Animator.StringToHash("Open");
    protected readonly int HASH_IDLE = Animator.StringToHash("Idle");

    abstract public void SetActive(DoorInfo _info);//오브젝트를 활성화할 때 사용
    virtual public void Opening()
    {
        m_collider.enabled = false;
        anim.SetFloat(HASH_SPEED, PlayerController.instance.GetAllSpeed());
        //Debug.Log($"문을 열었음 {PlayerController.instance.GetSpeed()}");
    }// 문이 열릴 경우 실행할 내용을 정의한다.
    public DoorType GetDoorType() { return info.TYPE; }//문의 타입 정보를 가져온다.
    public bool IsLast() { return info.isLast; }
}

/*
 * 문들을 생성하고 관리
 */
public class Doors : MonoBehaviour
{
    List<ActivityObject> list_doors = new List<ActivityObject>();
    string MAP_NAME = null;

    public bool Create(int createNum, string mapName, int quater)        //길을 생성해준다.
    {
        //**분기점을 불러오는 변수 추가

        string _mapName = $"{mapName}/{ mapName}Door{ quater}";
        if (MAP_NAME == _mapName) return false;//맵이 이미 같을 경우 생성 X
        Clear();

        Debug.LogWarning($"생성합니다. {_mapName}");
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

    //기존 맵에 대한 오브젝트가 존재할 경우 제거 
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

    //자동으로 문을 업데이트 한다.
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
                //문이 비활성화, 남은 문이 있을경우
                if ((!list_doors[i].ACTIVE) && MapManager.instance.IsRemainDoor())
                {
                    list_doors[i].gameObject.SetActive(true);
                    //다음 문에 대한 정보를 가지고 활성화 시킨다.
                    list_doors[i].SetActive(MapManager.instance.GetNextDoor());
                    //Debug.LogWarning("SetActive" + i);
                }
            }
        }
    }

    //게임이 종료가 되면 업데이트를 종료한다.
    public void RunUpdateStop(bool initialize)
    {
        //Debug.Log("DOORS ->RunUpdateStop");
        StopAllCoroutines();

        if (initialize)
        {
            for (int i = 0; i < list_doors.Count; i++)//전부 비활성화 처리
            {
                list_doors[i].ACTIVE = false;
            }
        }
    }

    //모든 문이 비활성화 되어 있을 경우를 반환함
    public bool IsAllDeActive()
    {
        for( int i  = 0;i<list_doors.Count; i++)
        {
            if (list_doors[i].ACTIVE) return false;
        }
        return true;
    }
}
