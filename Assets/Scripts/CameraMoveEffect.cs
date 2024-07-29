using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveEffect : MonoBehaviour
{
    [SerializeField]GameObject obj_camera;              // 카메라 오브젝트를 받음
    [SerializeField]Vector3 maxViewRotation;            //View로 보는 회전 최댓값
    
    //기본 포지션과 회전값
    [SerializeField]Vector3 default_position;
    [SerializeField]Vector3 default_rotation;

    [SerializeField]Vector3 offset_cameraMoving;//카메라가 걷는 효과를 낼 범위
    [SerializeField]float cameraMoveSpeed = 1;//카메라가 초당 이동하기 위한 스피드//default = 1;

    //카메라 걷기를 위한 이동 범위를 나타냄.
    Vector3[] adjustCameraRange;

    //이동 시 패턴을 지정
    int stateNum;//Left - Down - Right의 상태를 나타냄.0 1 2 3

    //카메라 떨림을 임시 저장
    Vector3 vibPos;

    Animator anim; //대기 모션 보여줄 애니메이션

    [Space]
    [Header("카메라 흔들임 세기")]
    [SerializeField]float vibOffset;

    [Header("죽었을 때 회전 값")] [SerializeField]Vector3 vibRot;

    [Space]
    [Header("PenaltyEffect 흔들릴 범위")]
    public float shake_x = 0.2f;
    public float shake_y = 0.2f;
    public float shake_duration = 0.5f;
    bool isShaking = false;

    [Space]
    [Header("Rush 좌우 흔들임 Y 축")]
    public int rot_rushY = 6;

    [Space]
    [Header("FOV 설정")]
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

        //걷는 효과 구역
        //default - left - default - right
        adjustCameraRange = new Vector3[] {
            new Vector3(default_position.x, offset_cameraMoving.y, default_position.z), new Vector3(offset_cameraMoving.x * -1 ,default_position.y ,default_position.z),
            new Vector3(default_position.x, offset_cameraMoving.y, default_position.z), new Vector3(offset_cameraMoving.x ,default_position.y ,default_position.z)
        };
    }

    //게임이 진행중이라면 화면 움직임을 활성화한다.
    private void Update()
    {
        if(GameManager.instance.GameState.Equals(GAMESTATE.PLAYING) && !isShaking)
        {
            StepState();
        }
    }

    //걷는 효과를 위한 준비
    public void GameSetting()
    {
        Debug.Log("GAMESETING CAMERAEFFECT");

        //카메라 움직임 효과를 제거하여준다.
        StopAllCoroutines();

        //변경되었던 카메라값을 초기화
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
    //걷는 효과.
    void StepState()
    {
        //아이템 사용 중 좌우 움직임을 처리할 때에는 새로운 위치값을 적용한다.
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

        //플레이어 속도가 증가할 때마다 2배수로 발소리를 낸다.
        if (distance < 0.01f)
        {
            stateNum = ++stateNum % adjustCameraRange.Length;
            if(stateNum == 0 || stateNum == 2)
            {
                SoundManager.instance.FootStep();
            }
        }

    }

    //변경된 속도에 따라 카메라의 걷는 모션 Speed를 조절하는 것
    public void SetCameraMoveSpeed()
    {
        //Debug.Log("카메라 움직임 속도 변경");
        cameraMoveSpeed = PlayerController.instance.GetAllSpeed() *2.0f;
    }


    WaitForSeconds playerDieWaitTime = new WaitForSeconds(0.05f);
    //플레이어가 죽으면 실행할 카메라 효과
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

            //진동 효과
            vibPos.Set(Random.Range(-vibOffset, vibOffset)+default_position.x, Random.Range(-vibOffset, vibOffset)+default_position.y, default_position.z);
            transform.localPosition = vibPos;

            yield return playerDieWaitTime;
        }
    }


    WaitForSeconds idleWaitTime = new WaitForSeconds(0.5f);
    WaitForSeconds animPlayWaitTime = new WaitForSeconds(0.05f);
    //두리번 거리는 효과를 주기 위한 코루틴
    IEnumerator IdleCoroutine()      //일정 시간 딜레이 후 카메라 Idle 모션 지정
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
            //좌우 돌려보기 끝내기
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
    //비율을 받아와 회전값을 준다.
    public void RotateView(float t) 
    {
        if(t < 0) { temp_ViewRot.Set(maxViewRotation.x, -maxViewRotation.y, -maxViewRotation.z); }
        else { temp_ViewRot = maxViewRotation; }

        Vector3 rotate = Vector3.Lerp(Vector3.zero,temp_ViewRot, Mathf.Abs(t));

        obj_camera.transform.rotation = Quaternion.Euler(rotate);
    }


    //모든 문을 클리어하게 되면 필드 오브 뷰를 늘린다.
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
