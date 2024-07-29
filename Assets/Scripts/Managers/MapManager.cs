using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

//문에 대한 상태를 나타내준다.
public enum DoorType
{
    Left = 0,
    Right,
    Up,
    Down,
    Cicle,
    Touch
}
//문에 대한 배치 정보를 가진다.
[System.Serializable]
public class DoorInfo
{
    public DoorType TYPE;//문의 타입
    public float Z_POSITION;//문이 생성되는 좌표
    public float ADDSPEED { get; private set; }//추가되는 스피드

    public int NUMBER;//문의 정보가 생성된 순서

    public bool isLast { private set; get; } = false; //마지막 오브젝트일 경우를 나타냄.

    //각 문 별로 가속 시킬 속도
    static int seqNum = -1;
    //아무것도 변수가 없는 문일 경우
    public DoorInfo() 
    {
        TYPE = DoorType.Touch;
        NUMBER = -99;
        ADDSPEED = 0;
        Z_POSITION = 0;
    }

    //문 정보를 생성하면서, 랜덤한 속도를 지정한다.
    public DoorInfo(int stage,float create_z, int typeRange = 2, float _addSpeed =0.1f)
    {
        TYPE = (DoorType)Random.Range(0, typeRange);

        NUMBER = ++seqNum;

        ADDSPEED = _addSpeed;

        Z_POSITION = create_z;
    }

    public void ItIsLastDoor() { isLast = true; }
}

/*
 * 맵을 전체적으로 생성한다.
 *맵에 대한 생성과 초기화 등의 일을 담당한다.
 *TouchManager에 현재 클리어 해야 될 게임 정보를 넘겨주고, 문 통과 정보를 받는다.
 */
public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    //최대 생성 되어야 하는 갯수
    public int MaxFloorCount = 2;
    public int MaxDoorCount = 10;

    #region 문과 관련된 변수
    //처음 문이 생성되는 플레이어와의 간격
    public float door_betweenDefault = 4.0f;
    //문이 생성되는 기본적인 간격을 나타냄.
    [SerializeField]
    //float door_interval= 2.0f;
    
    //문에 대한 정보를 담을 변수
    List<DoorInfo> door_info = new List<DoorInfo>();

    ////문의 간격의 최소값
    //[Range(0.8f, 1.2f)]
    //[SerializeField] float door_intervalRandomMin = 0.9f;
    ////최대간격 범위 값
    //[SerializeField] float door_randomIntervalMinus = 0.3f;
    ////문 간격값 최대값
    //[SerializeField] float door_initInterRandomMax = 1.5f;
    #endregion

    Ways ways;
    Doors doors;
    StartRoom room;

    string MAP_NAME = null;

    int batched_index;//설치된 문의 갯수를 넘겨줌.

    public readonly float reBatchinterval = 0.3f;
    private void Awake()
    {
        if (instance == null) instance = this;

        //touchController = FindObjectOfType<TouchAction>();
        //player = FindObjectOfType<PlayerController>();

        ways = GetComponentInChildren<Ways>();
        doors = GetComponentInChildren<Doors>();
    }

    private void Start()
    {
        GameManager.instance.Action_Finish_StandBy +=  GameSetting;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    //처음 시작 방 생성한다.
    void CreateStartRoom(string mapName)
    {
        if (mapName == MAP_NAME) return;//맵이 같을 경우 생성 X

        Debug.LogWarning($"생성합니다. {mapName}/{ mapName}StartRoom");
        GameObject _room = Resources.Load<GameObject>($"{mapName}/{mapName}StartRoom");
        room = Instantiate(_room, transform).GetComponent<StartRoom>();

        MAP_NAME = mapName;
    }

    public void GameSetting()  //게임 시작 시 맵을 생성, 난이도 설정
    {
        Debug.Log("GAMESETTING MAPMANAGER");

        string mapName = GameManager.instance.GetStageName();

        ways.RunUpdateStop();
        doors.RunUpdateStop(true);

        //맵생성
        CreateStartRoom(mapName);       //룸
        ways.Create(10, mapName, 0);     //길
        doors.Create(15, mapName, 0);   //문

        room.StartSetting();            //시작 문 정렬
        ways.Bacth(room.GetStart_Z());  //길을 배치하는데 시작위치를 넘겨준다.

        CreateActivityObject(room.GetStart_Z());

        //맵 구성요소 자동 배치
        ways.RunUpdate();
        doors.RunUpdate();
    }

    //게임을 재시작했을 경우
    public void Finish_StandBy()
    {

    }

    //죽었을 경우
    public void Playing_Gameover()
    {

    }


    //새로운 활성오브젝트를 추가하여 다음 스테이지를 지정한다.
    public void AddActivityObject()
    {
        Debug.LogWarning("새로운 오브젝트를 추가합니다.");

        //배치 비활성화
        doors.RunUpdateStop(false);

        CreateActivityObject(door_info[door_info.Count - 1].Z_POSITION + PlayerController.instance.GetSpeed() * 3);

        StartCoroutine(DelayRunUpdate(0.2f));
    }

    IEnumerator DelayRunUpdate(float t)
    {
        while (!doors.IsAllDeActive())
        {
            yield return new WaitForSeconds(t);
        }
        //문만 업데이트
        doors.RunUpdate();
    }

    //문 생성 좌표를 받아 활성오브젝트를 생성한다.
    void CreateActivityObject(float z)
    {
        door_info.Clear();
        batched_index = 0;  // 현재 배치된 번호
        DoorType type = GameManager.instance.GetCurrentType();
        Debug.Log($"현재 맵의 타입을 받아옵니다. {type}");

        for (int i = 1; i <= 50; i++)
        {
            door_info.Add(new DoorInfo(0, z + i * 3, (int)type +1));
        }

        door_info[door_info.Count - 1].ItIsLastDoor();
    }

    #region GET/SET/IS

    //다음으로 설치될 문을 가져옵니다.
    public DoorInfo GetNextDoor()
    {
        return door_info[batched_index++];
    }
    //현재 남아있는 문이 있느냐?
    public bool IsRemainDoor()
    {
        return (batched_index >= door_info.Count) ? false : true;
    }

    public int GetCreatedObject()  //만들어져 있는 오브젝트를 받아온다.
    {
        Debug.Log($"***DoorCreatedCount{door_info.Count}");
        return door_info.Count;
    }

    //현재의 문이 마지막 문이 맡는지 판단한다.
    public bool IsLastDoor(int seq)
    {
        return door_info[door_info.Count - 1].NUMBER.Equals(seq);
    }

    public Vector3 GetStartMapMonsterPosition()
    {
        return new Vector3(0,0,room.GetMonster_Z());
    }
    #endregion
}
