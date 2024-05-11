using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

//���� ���� ���¸� ��Ÿ���ش�.
public enum DoorType
{
    Left = 0,
    Right,
    Up,
    Down,
    Cicle,
    Touch
}
//���� ���� ��ġ ������ ������.
[System.Serializable]
public class DoorInfo
{
    public DoorType TYPE;//���� Ÿ��
    public float Z_POSITION;//���� �����Ǵ� ��ǥ
    public float ADDSPEED { get; private set; }//�߰��Ǵ� ���ǵ�

    public int NUMBER;//���� ������ ������ ����

    public bool isLast { private set; get; } = false; //������ ������Ʈ�� ��츦 ��Ÿ��.

    //�� �� ���� ���� ��ų �ӵ�
    static int seqNum = -1;
    //�ƹ��͵� ������ ���� ���� ���
    public DoorInfo() 
    {
        TYPE = DoorType.Touch;
        NUMBER = -99;
        ADDSPEED = 0;
        Z_POSITION = 0;
    }

    //�� ������ �����ϸ鼭, ������ �ӵ��� �����Ѵ�.
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
 * ���� ��ü������ �����Ѵ�.
 *�ʿ� ���� ������ �ʱ�ȭ ���� ���� ����Ѵ�.
 *TouchManager�� ���� Ŭ���� �ؾ� �� ���� ������ �Ѱ��ְ�, �� ��� ������ �޴´�.
 */
public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    //�ִ� ���� �Ǿ�� �ϴ� ����
    public int MaxFloorCount = 2;
    public int MaxDoorCount = 10;

    #region ���� ���õ� ����
    //ó�� ���� �����Ǵ� �÷��̾���� ����
    public float door_betweenDefault = 4.0f;
    //���� �����Ǵ� �⺻���� ������ ��Ÿ��.
    [SerializeField]
    //float door_interval= 2.0f;
    
    //���� ���� ������ ���� ����
    List<DoorInfo> door_info = new List<DoorInfo>();

    ////���� ������ �ּҰ�
    //[Range(0.8f, 1.2f)]
    //[SerializeField] float door_intervalRandomMin = 0.9f;
    ////�ִ밣�� ���� ��
    //[SerializeField] float door_randomIntervalMinus = 0.3f;
    ////�� ���ݰ� �ִ밪
    //[SerializeField] float door_initInterRandomMax = 1.5f;
    #endregion

    Ways ways;
    Doors doors;
    StartRoom room;

    string MAP_NAME = null;

    int batched_index;//��ġ�� ���� ������ �Ѱ���.

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

    //ó�� ���� �� �����Ѵ�.
    void CreateStartRoom(string mapName)
    {
        if (mapName == MAP_NAME) return;//���� ���� ��� ���� X

        Debug.LogWarning($"�����մϴ�. {mapName}/{ mapName}StartRoom");
        GameObject _room = Resources.Load<GameObject>($"{mapName}/{mapName}StartRoom");
        room = Instantiate(_room, transform).GetComponent<StartRoom>();

        MAP_NAME = mapName;
    }

    public void GameSetting()  //���� ���� �� ���� ����, ���̵� ����
    {
        Debug.Log("GAMESETTING MAPMANAGER");

        string mapName = GameManager.instance.GetStageName();

        ways.RunUpdateStop();
        doors.RunUpdateStop(true);

        //�ʻ���
        CreateStartRoom(mapName);       //��
        ways.Create(10, mapName, 0);     //��
        doors.Create(15, mapName, 0);   //��

        room.StartSetting();            //���� �� ����
        ways.Bacth(room.GetStart_Z());  //���� ��ġ�ϴµ� ������ġ�� �Ѱ��ش�.

        CreateActivityObject(room.GetStart_Z());

        //�� ������� �ڵ� ��ġ
        ways.RunUpdate();
        doors.RunUpdate();
    }

    //������ ��������� ���
    public void Finish_StandBy()
    {

    }

    //�׾��� ���
    public void Playing_Gameover()
    {

    }


    //���ο� Ȱ��������Ʈ�� �߰��Ͽ� ���� ���������� �����Ѵ�.
    public void AddActivityObject()
    {
        Debug.LogWarning("���ο� ������Ʈ�� �߰��մϴ�.");

        //��ġ ��Ȱ��ȭ
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
        //���� ������Ʈ
        doors.RunUpdate();
    }

    //�� ���� ��ǥ�� �޾� Ȱ��������Ʈ�� �����Ѵ�.
    void CreateActivityObject(float z)
    {
        door_info.Clear();
        batched_index = 0;  // ���� ��ġ�� ��ȣ
        DoorType type = GameManager.instance.GetCurrentType();
        Debug.Log($"���� ���� Ÿ���� �޾ƿɴϴ�. {type}");

        for (int i = 1; i <= 50; i++)
        {
            door_info.Add(new DoorInfo(0, z + i * 3, (int)type +1));
        }

        door_info[door_info.Count - 1].ItIsLastDoor();
    }

    #region GET/SET/IS

    //�������� ��ġ�� ���� �����ɴϴ�.
    public DoorInfo GetNextDoor()
    {
        return door_info[batched_index++];
    }
    //���� �����ִ� ���� �ִ���?
    public bool IsRemainDoor()
    {
        return (batched_index >= door_info.Count) ? false : true;
    }

    public int GetCreatedObject()  //������� �ִ� ������Ʈ�� �޾ƿ´�.
    {
        Debug.Log($"***DoorCreatedCount{door_info.Count}");
        return door_info.Count;
    }

    //������ ���� ������ ���� �ô��� �Ǵ��Ѵ�.
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
