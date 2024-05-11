using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ܼ��� �̵�, ����ó��
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    //�÷��̾� ���� ��ġ
    public Vector3 SPAWM_POSITION;  //�÷��̾ �ʱ⿡ �����Ǵ� ��ġ
    private Vector3 SAVE_POSITION;   //�÷��̾ ������ ��ġ�� ����Ǵ� ����

    //�÷��̾��� �̵� �ӵ��� ���Ѵ�.
    [SerializeField]
    float SPEED = 1.0f;
    float MUL_SPEED = 1.0f;            //���ǵ忡 �ӽ÷� �������� ���
    float PENALTY_SPEED = 1.0f;     //�г�Ƽ�� ������ ���� �����

    [SerializeField] CameraMoveEffect cameraEffect;

    [SerializeField] MoveParticle ps_move;  //������ �� ���� ȿ��

    //�ӵ��� ������ ������ �ٲ��ָ� ���� �ʱ� ������ 0.5���� ������ ������ �ٲ��ְ���.
    float particleSpeed = 0;
    float changeRange = 0.5f;

    [Space]
    [Header("�ӵ���")]
    float limitSpeed;

    bool isRush = false;    //Rush������ ����� �ߴ°�?

    float desub = 0;
    float currentSpeed => SPEED * MUL_SPEED * PENALTY_SPEED;      //�ӵ��� ������ ����

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


    //���� ���� DEFAULT POSITION���� ��ġ ������ ����
    public void GameSetting()
    {
        //Debug.Log("GAMESEETING_PLAYERCONTROLLER");
        //**���� ���������� ���� �÷��̾� �ӵ��� �����Ѵ�.
        SPEED = 2;
        MUL_SPEED = 1;
        PENALTY_SPEED = 1.0f;
        isRush = false;

        //���� �ӵ��� ������ ����
        limitSpeed = GameManager.instance.GetMaximumSpeed();
        Debug.LogWarning($"LIMIT SPEED : {limitSpeed}");

        //�ʱ���ġ ����
        transform.position = SPAWM_POSITION;    //�ʱ� ��ġ�� ����

        //ī�޶� ������ ����
        cameraEffect.SetCameraMoveSpeed();

    }

    public void Gameover_StandBy()  //���� �� ���ڸ� ������� ��
    {
        Debug.Log("Player_Gameover->StandBy");
        //��ġ ����
        transform.position = new Vector3(SPAWM_POSITION.x,SPAWM_POSITION.y,-SPEED + SAVE_POSITION.z);

        MUL_SPEED = 1;
        PENALTY_SPEED = 1.0f;
        //ī�޶� ������ �ֱ�
        cameraEffect.SetCameraMoveSpeed();
    }

    //���� ���� �ӵ��� ������Ŵ
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

    //�÷��̾ �׾��� ��츦 ��Ÿ�� �� ����
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MONSTER"))    //����
        {
            StopAllCoroutines();
            this.SAVE_POSITION = other.transform.position;
            SoundManager.instance.AttackMonster();
            GameManager.instance.GameStateChange(GAMESTATE.GAMEOVER);
        }

        if (other.CompareTag("DOOR"))   // �ӵ� ����
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

    //���� paticle �ӵ� ������ ���� �� �Լ�
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

    //��� ���ӽ�Ų��.
    public void PenaltyEffect()
    {
        Debug.Log("�г�Ƽ �÷��̾� ��Ʈ�ѷ�");
        PENALTY_SPEED = 0.0f;
        cameraEffect.SetCameraMoveSpeed();      //ī�޶� ������ �ӵ� ����

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
            //Debug.LogWarning($"�ӵ� �г�Ƽ {PENALTY_SPEED}");
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

            //���� ���ġ
            Monster.instance.ReBatch(GetSpeed());
        }
    }
    #endregion
}
