using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//단순한 이동, 죽음처리
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    //플레이어 스폰 위치
    public Vector3 SPAWM_POSITION;  //플레이어가 초기에 생성되는 위치
    private Vector3 SAVE_POSITION;   //플레이어가 죽으면 위치가 저장되는 변수

    //플레이어의 이동 속도를 정한다.
    [SerializeField]
    float SPEED = 1.0f;
    float MUL_SPEED = 1.0f;            //스피드에 임시로 곱해지는 배수
    float PENALTY_SPEED = 1.0f;     //패널티로 느리게 만들 배수값

    [SerializeField] CameraMoveEffect cameraEffect;

    [SerializeField] MoveParticle ps_move;  //움직일 때 먼지 효과

    //속도가 증가할 때마다 바꿔주면 좋지 않기 때문에 0.5값이 증가할 때마다 바꿔주겠음.
    float particleSpeed = 0;
    float changeRange = 0.5f;

    [Space]
    [Header("속도값")]
    float limitSpeed;

    bool isRush = false;    //Rush아이템 사용을 했는가?

    float desub = 0;
    float currentSpeed => SPEED * MUL_SPEED * PENALTY_SPEED;      //속도를 추적할 변수

    private void Awake()
    {
        if (instance == null)instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    void Start()
    {
        GameManager.instance.Action_Finish_StandBy += GameSetting;
        GameManager.instance.Action_Gameover_StandBy += Gameover_StandBy;

        EventManager.instance.Action_Panelty += PenaltyEffect;
    }

    public bool isTest = true;

    //Update is called once per framer
    void Update()
    {
        if (GameManager.instance.GameState.Equals(GAMESTATE.PLAYING) && !isTest)
        {
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }
    }


    //게임 시작 DEFAULT POSITION으로 위치 설정할 셋팅
    public void GameSetting()
    {
        //Debug.Log("GAMESEETING_PLAYERCONTROLLER");
        //**추후 스테이지에 따라 플레이어 속도를 셋팅한다.
        SPEED = 2;
        MUL_SPEED = 1;
        PENALTY_SPEED = 1.0f;
        isRush = false;

        //제한 속도를 가져와 셋팅
        limitSpeed = GameManager.instance.GetMaximumSpeed();
        Debug.LogWarning($"LIMIT SPEED : {limitSpeed}");

        //초기위치 셋팅
        transform.position = SPAWM_POSITION;    //초기 위치에 지정

        //카메라 움직임 셋팅
        cameraEffect.SetCameraMoveSpeed();

    }

    public void Gameover_StandBy()  //죽은 후 제자리 재시작할 때
    {
        Debug.Log("Player_Gameover->StandBy");
        //위치 지정
        transform.position = new Vector3(SPAWM_POSITION.x,SPAWM_POSITION.y,-SPEED + SAVE_POSITION.z);

        MUL_SPEED = 1;
        PENALTY_SPEED = 1.0f;
        //카메라 움직임 주기
        cameraEffect.SetCameraMoveSpeed();
    }

    //문을 열면 속도를 증가시킴
    public void DoorOpen()
    {
        //SPEED += _speed;
        if(SPEED > limitSpeed)
        {
            SPEED += 0.01f;
        }
        else
        {
            SPEED += 0.1f;
        }

        cameraEffect.SetCameraMoveSpeed();
    }

    //플레이어가 죽었을 경우를 나타낼 수 있음
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MONSTER"))    //죽음
        {
            StopAllCoroutines();
            this.SAVE_POSITION = other.transform.position;
            SoundManager.instance.AttackMonster();
            GameManager.instance.GameStateChange(GAMESTATE.GAMEOVER);
        }

        if (other.CompareTag("DOOR"))   // 속도 감속
        {
            if (isRush)
            {
                TouchAction.instance.OpenForwardDoor();
                SoundManager.instance.BumpDoor();
            }
            else
            {
                TouchAction.instance.OpenForwardDoor();
                SoundManager.instance.BumpDoor();
                EventManager.instance.Action_Panelty();
            }
        }
    }

    //먼지 paticle 속도 변경을 위한 비교 함수
    void ChangeParticleSpeed()
    {
        desub = currentSpeed - particleSpeed;
        if( desub >= changeRange || desub <= -changeRange)
        {
            ps_move.SetSpeed(currentSpeed);
            particleSpeed = currentSpeed;
        }
    }
    
    public float GetSpeed() { return SPEED; }
    public float GetAllSpeed() { return (currentSpeed > 1.0f)?currentSpeed : 1.0f; }
    public float GetPositionZ() { return transform.position.z; }

    //잠시 감속시킨다.
    public void PenaltyEffect()
    {
        Debug.Log("패널티 플레이어 컨트롤러");
        PENALTY_SPEED = 0.0f;
        cameraEffect.SetCameraMoveSpeed();      //카메라 움직임 속도 변경

        StartCoroutine(PenaltyCroutine());
    }

    WaitForSeconds penaltyTime = new WaitForSeconds(0.2f);
    IEnumerator PenaltyCroutine()
    {
        while (PENALTY_SPEED < 1.0f)
        {
            yield return penaltyTime;
            PENALTY_SPEED += 0.1f;
            cameraEffect.SetCameraMoveSpeed();
            //Debug.LogWarning($"속도 패널티 {PENALTY_SPEED}");
        }

        PENALTY_SPEED = 1.0f;
        cameraEffect.SetCameraMoveSpeed();
    }


    #region ITEM
    public void Item_Rush(bool active)
    {
        if (active)
        {
            isRush = true;
            MUL_SPEED = 2;

            cameraEffect.Item_Rush(active);
        }
        else
        {
            isRush = false;
            MUL_SPEED = 1;
            TouchAction.instance.OpenForwardDoor();

            cameraEffect.Item_Rush(active);

            //몬스터 재배치
            Monster.instance.ReBatch(GetSpeed());
        }
    }
    #endregion
}
