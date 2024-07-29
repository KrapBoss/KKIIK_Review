using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveEffect : MonoBehaviour
{
    [SerializeField]GameObject obj_camera;              // ī�޶� ������Ʈ�� ����
    [SerializeField]Vector3 maxViewRotation;            //View�� ���� ȸ�� �ִ�
    
    //�⺻ �����ǰ� ȸ����
    [SerializeField]Vector3 default_position;
    [SerializeField]Vector3 default_rotation;

    [SerializeField]Vector3 offset_cameraMoving;//ī�޶� �ȴ� ȿ���� �� ����
    [SerializeField]float cameraMoveSpeed = 1;//ī�޶� �ʴ� �̵��ϱ� ���� ���ǵ�//default = 1;

    //ī�޶� �ȱ⸦ ���� �̵� ������ ��Ÿ��.
    Vector3[] adjustCameraRange;

    //�̵� �� ������ ����
    int stateNum;//Left - Down - Right�� ���¸� ��Ÿ��.0 1 2 3

    //ī�޶� ������ �ӽ� ����
    Vector3 vibPos;

    Animator anim; //��� ��� ������ �ִϸ��̼�

    [Space]
    [Header("ī�޶� ����� ����")]
    [SerializeField]float vibOffset;

    [Header("�׾��� �� ȸ�� ��")] [SerializeField]Vector3 vibRot;

    [Space]
    [Header("PenaltyEffect ��鸱 ����")]
    public float shake_x = 0.2f;
    public float shake_y = 0.2f;
    public float shake_duration = 0.5f;
    bool isShaking = false;

    [Space]
    [Header("Rush �¿� ����� Y ��")]
    public int rot_rushY = 6;

    [Space]
    [Header("FOV ����")]
    public int default_FOV = 90;
    Camera CAMERA;


    private void Start()
    {
        //EventManager.instance.Action_Panelty += PenaltyEffect;

        GameManager.instance.Action_ALLClearDoor += AllClearDoor;
        GameManager.instance.Action_Finish_StandBy +=  GameSetting;
        GameManager.instance.Action_Gameover_StandBy += GameSetting;
        GameManager.instance.Action_Playing_Gameover += Playing_Gameover;
        GameManager.instance.Action_Standby_Playing += StandBy_Playing;

        CAMERA = Camera.main;
        anim = GetComponent<Animator>();

        //�ȴ� ȿ�� ����
        //default - left - default - right
        adjustCameraRange = new Vector3[] {
            new Vector3(default_position.x, offset_cameraMoving.y, default_position.z), new Vector3(offset_cameraMoving.x * -1 ,default_position.y ,default_position.z),
            new Vector3(default_position.x, offset_cameraMoving.y, default_position.z), new Vector3(offset_cameraMoving.x ,default_position.y ,default_position.z)
        };
    }

    //������ �������̶�� ȭ�� �������� Ȱ��ȭ�Ѵ�.
    private void Update()
    {
        if(GameManager.instance.GameState.Equals(GAMESTATE.PLAYING) && !isShaking)
        {
            StepState();
        }
    }

    //�ȴ� ȿ���� ���� �غ�
    public void GameSetting()
    {
        Debug.Log("GAMESETING CAMERAEFFECT");

        //ī�޶� ������ ȿ���� �����Ͽ��ش�.
        StopAllCoroutines();

        //����Ǿ��� ī�޶��� �ʱ�ȭ
        transform.localPosition = default_position;
        transform.localRotation = Quaternion.Euler(default_rotation);
        stateNum = Random.Range(0, 2) == 0 ? 1 : 3;

        CAMERA.fieldOfView = default_FOV;

        anim.enabled = true;
        isRush = false;
        rush_value = 1.0f;

        StartCoroutine(IdleCoroutine());
    }

    float distance =0;
    Vector3 v;
    //�ȴ� ȿ��.
    void StepState()
    {
        //������ ��� �� �¿� �������� ó���� ������ ���ο� ��ġ���� �����Ѵ�.
        if (isRush && (stateNum == 1 || stateNum == 3))
        {
            v.Set(adjustCameraRange[stateNum].x * rush_value, adjustCameraRange[stateNum].y,adjustCameraRange[stateNum].z);
            //transform.localPosition = Vector3.Lerp(transform.localPosition, v, cameraMoveSpeed * Time.deltaTime);
        }
        else
        {
            v = adjustCameraRange[stateNum];
            //transform.localPosition = Vector3.Lerp(transform.localPosition, adjustCameraRange[stateNum], cameraMoveSpeed * Time.deltaTime);
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, v, cameraMoveSpeed * Time.deltaTime);
        distance = Vector3.Magnitude(v-transform.localPosition);

        //�÷��̾� �ӵ��� ������ ������ 2����� �߼Ҹ��� ����.
        if (distance < 0.01f)
        {
            stateNum = ++stateNum % adjustCameraRange.Length;
            if(stateNum == 0 || stateNum == 2)
            {
                SoundManager.instance.FootStep();
            }
        }

    }

    //����� �ӵ��� ���� ī�޶��� �ȴ� ��� Speed�� �����ϴ� ��
    public void SetCameraMoveSpeed()
    {
        //Debug.Log("ī�޶� ������ �ӵ� ����");
        cameraMoveSpeed = PlayerController.instance.GetAllSpeed() *2.0f;
    }


    WaitForSeconds playerDieWaitTime = new WaitForSeconds(0.05f);
    //�÷��̾ ������ ������ ī�޶� ȿ��
    public void Playing_Gameover()
    {
        StartCoroutine(PlayerDieCroutine());
    }

    IEnumerator PlayerDieCroutine()
    {
        float time = 1.0f;
        transform.localRotation = Quaternion.Euler(vibRot);

        while(time > 0)
        {
            time -= Time.deltaTime +0.05f;

            //���� ȿ��
            vibPos.Set(Random.Range(-vibOffset, vibOffset)+default_position.x, Random.Range(-vibOffset, vibOffset)+default_position.y, default_position.z);
            transform.localPosition = vibPos;

            yield return playerDieWaitTime;
        }
    }


    WaitForSeconds idleWaitTime = new WaitForSeconds(0.5f);
    WaitForSeconds animPlayWaitTime = new WaitForSeconds(0.05f);
    //�θ��� �Ÿ��� ȿ���� �ֱ� ���� �ڷ�ƾ
    IEnumerator IdleCoroutine()      //���� �ð� ������ �� ī�޶� Idle ��� ����
    {
        while (GameManager.instance.GameState != GAMESTATE.PLAYING)
        {
            yield return idleWaitTime;
            bool range = (Random.Range(0, 101) < 2);
            if (range)
            {
                string direction = Random.Range(0, 1) == 0 ? "Left" : "Right";
                //anim.SetTrigger(direction);
                

                for(int i =0;i<60; i++)
                {
                    yield return animPlayWaitTime;
                }
            }
            //�¿� �������� ������
            if (GameManager.instance.GameState == GAMESTATE.PLAYING)
            {
                break;
            }
        }

        //anim.enabled = false;

        transform.rotation = Quaternion.Euler(default_rotation);
    }

    #region ***********ACTION***********

    public void StandBy_Playing()
    {
        anim.enabled = false; 
        StartCoroutine(ToDefaultRotation());
    }
    WaitForSeconds default_rotTime = new WaitForSeconds(0.1f);
    IEnumerator ToDefaultRotation()
    {
        float delta = default_rotation.z - transform.localRotation.z;
        float currentZ = transform.localRotation.z;
        while(Mathf.Abs(transform.localRotation.z - default_rotation.z) > 0.1f)
        {
            transform.localRotation = Quaternion.Euler(0,0,currentZ + delta * 0.1f);
            yield return default_rotTime;
        }
    }
    #endregion

    Vector3 temp_ViewRot;
    //������ �޾ƿ� ȸ������ �ش�.
    public void RotateView(float t) 
    {
        if(t < 0) { temp_ViewRot.Set(maxViewRotation.x, -maxViewRotation.y, -maxViewRotation.z); }
        else { temp_ViewRot = maxViewRotation; }

        Vector3 rotate = Vector3.Lerp(Vector3.zero,temp_ViewRot, Mathf.Abs(t));

        obj_camera.transform.rotation = Quaternion.Euler(rotate);
    }


    //��� ���� Ŭ�����ϰ� �Ǹ� �ʵ� ���� �並 �ø���.
    public void AllClearDoor()
    {
        float max_speed = GameManager.instance.GetMaximumSpeed();
        float fov= default_FOV + (PlayerController.instance.GetSpeed()/max_speed) * 15;
        StartCoroutine(FOVCroutine(fov));
    }
    WaitForSeconds fovTime = new WaitForSeconds(0.1f);
    IEnumerator FOVCroutine(float f)
    {
        while (CAMERA.fieldOfView < f)
        {
            CAMERA.fieldOfView += 1f;
            yield return fovTime;
        }
    }


    #region ITEM
    bool isRush = false;
    float rush_value = 1.0f;
    public void Item_Rush(bool active)
    {
        if (active)
        {
            isRush = true;
            rush_value = 2.5f;
            float max_speed = GameManager.instance.GetMaximumSpeed();
            float fov = default_FOV + (PlayerController.instance.GetAllSpeed() / max_speed) * 15;
            SetCameraMoveSpeed();
            StartCoroutine(FOVCroutine(fov));
        }
        else
        {
            isRush = false;
            rush_value = 1.0f;
            float max_speed = GameManager.instance.GetMaximumSpeed();
            float fov = default_FOV + (PlayerController.instance.GetSpeed() / max_speed) * 15;
            SetCameraMoveSpeed();
            CAMERA.fieldOfView = fov;
            transform.localRotation = Quaternion.Euler(default_rotation);
        }
    }
    #endregion
}
