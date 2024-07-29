using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



/*
 * 화면을 터치했을 때의 이벤트를 비교해서 MapManager로 정보를 넘겨준다.
 */
public class TouchAction : MonoBehaviour
{
    public static TouchAction instance;
    enum Cicle
    {
        Idle =0,
        RightUp,
        LeftUp,
        LeftDown,
        RightDown
    }
   

    //현재 터치되는 좌표 값을 넘겨준다.
    byte x = 44, y = 44;
    //사이클 판단을 위한 간격
    float cinter;

    //사이클에 대한 변수
    int cicleBif = 0;

    //화면 터치 사이 판정 간격
    public int interval = 40;

    Vector2 deltaPos  = Vector2.zero;

    //현재 모션의 범위에 따라 진행상황을 미리 저장. 
    Cicle[,] cicleArr = new Cicle[2,2]{ {Cicle.RightUp, Cicle.LeftUp},
                                        {Cicle.RightDown , Cicle.LeftDown } };
    Cicle currCicleState;

    Touch touch;//터치 이벤트를 받을 변수

    //현재 행해진 Touch 액션을 지정한 것이다.
    DoorType currActionType;
    //현재 도어에 대한 이벤트 실행을 완료했을 경우 중복을 방지한다.
    //bool isActionEnd = false;

    //사이클 횟수 판단에 필요한 갯수 // 기본 = 2
    int cicle_clearNum = 2;
    //사이클 횟수를 세아리는 변수
    int cicle_currNum = 0;

    public float penltyTime = 0.5f;

    //큐에 문에 대한 정보를 생성된 순서대로 저정할 것이다.
    Queue<ActivityObject> queue_activityObjects = new Queue<ActivityObject>();
    //플레이어 앞에 있는 문의 정보를 담을 변수
    ActivityObject activityObject = null;


    //재시작 시 문의 오입력을 방지
    bool enableTouch = false;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        cinter = interval / 2;

        EventManager.instance.Action_Panelty += Panelty;

        GameManager.instance.Action_Finish_StandBy += Finish_Standby;
        GameManager.instance.Action_Gameover_StandBy += Gameover_Standby;
        GameManager.instance.Action_Playing_Gameover += Playing_Gameover;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void Update()
    {
        if(Input.touchCount >= 1 && enableTouch)
        {
            TouchMotion();
        }
    }

    //시작버튼을 눌렀을 경우에 터치가 가능하도록 한다.
    public void StartButton()
    {
        GameManager.instance.GameStateChange(GAMESTATE.PLAYING);    //게임 시작

        OpenForwardDoor();

        StartCoroutine(DelayEnableTouchCroutine());
    }

    IEnumerator DelayEnableTouchCroutine()
    {
        yield return new WaitForSeconds(0.2f);
        cicle_currNum = 0;       //사이클 터치의 입력 값 초기화
        deltaPos = Vector2.zero;       //시작 전 입력이 된 값을 초기화해준다.
        enableTouch = true; // 터치가 가능해짐
    }

    public void Panelty()
    {
        Debug.LogWarning($"TOUCHACTION :: Panelty Combo = 0");

        EventManager.instance.EventCombo(false);

        enableTouch = false;

        StartCoroutine(DelayEnableTouchCroutine());
    }


    //바로 앞에 있는 문을 연다
    public void OpenForwardDoor()
    {
        if (queue_activityObjects.Count > 0)
        {
            activityObject = queue_activityObjects.Peek();  //플레이어 바로 앞에 있는 문을 대입
            activityObject.Opening();                       //문을 열고 시작함.
            queue_activityObjects.Dequeue();
        }
    }


    //배열을 초기화해줌
    public void Finish_Standby()
    {
        Debug.Log("TouchAction -> Finish_Standby");
        queue_activityObjects.Clear();
        cicle_currNum = 0;       //사이클 터치의 입력 값 초기화
        deltaPos = Vector2.zero;       //시작 전 입력이 된 값을 초기화해준다.
        enableTouch = false;        //터치 불가
        isAuto = false;
    }

    //배열을 초기화 하지 않는다.
    public void Gameover_Standby()
    {
        Debug.Log("TouchAction -> Gameover_StandBy");
        cicle_currNum = 0;       //사이클 터치의 입력 값 초기화
        deltaPos = Vector2.zero;       //시작 전 입력이 된 값을 초기화해준다.

        isAuto = false;
    }

    //게임 중 오버
    public void Playing_Gameover()
    {
        Debug.Log("TouchAction -> Playing_Gameover");

        //터치가 불가능 하도록 함
        enableTouch = false;
    }

    //생성된 문을 삽입한다.
    public void StackDoor(ActivityObject _door)
    {
        queue_activityObjects.Enqueue(_door);
    }

    //터치입력을 받아들일 수 있도록 함
    void TouchMotion()
    {
        //해당 액션이 끝났을 경우 스크립트 종료
        //if (isActionEnd) return;

        if (queue_activityObjects.Count < 1) return;

        touch = Input.GetTouch(0);
        activityObject = queue_activityObjects.Peek();//첫번째 요소를 대입 = 제일먼저 생성된 문


        Debug.Log("TouchMotion");

        //초기 터치 시작시 값들을 초기화 시켜준다.
        if (touch.phase == TouchPhase.Began)
        {
            cicleBif = 0;
            currCicleState = Cicle.Idle;
            deltaPos = Vector2.zero;
            x = 44;
            y = 44;
        }
        else
        if (touch.phase == TouchPhase.Moved)
        {
            deltaPos += touch.deltaPosition;
            if (activityObject.GetDoorType() == DoorType.Cicle) { CycleAction(); }
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            if (activityObject.GetDoorType() != DoorType.Cicle)//위 아래 양옆 모션을 얻음
            {
                if (deltaPos.y > interval && Mathf.Abs(deltaPos.x) < deltaPos.y) currActionType = DoorType.Up;
                else if (-deltaPos.y > interval && Mathf.Abs(deltaPos.x) < -deltaPos.y) currActionType = DoorType.Down;
                else if (deltaPos.x > interval) currActionType = DoorType.Right;
                else if (-deltaPos.x > interval) currActionType = DoorType.Left;
                else currActionType = DoorType.Touch;

                if (isAuto)
                {
                    //isActionEnd = true;
                    activityObject.Opening();
                    queue_activityObjects.Dequeue();
                }
                else//해당 이벤트의 동작이 맞다면 문에 대한 판단을 실행한다.
                if (activityObject.GetDoorType().Equals(currActionType))
                {
                    activityObject.Opening();
                    queue_activityObjects.Dequeue();
                    EventManager.instance.EventCombo(true);
                }
                else        //동작 입력이 맞지 않는 경우
                {
                    EventManager.instance.EventCombo(false);
                }
            }
        }
    }

    //회전에 대한 처리와 해당 회전에 대한 상태를 판단해준다.
    void CycleAction()
    {
        //Debug.Log($"deltaPOs = {cicleDeltaPos}");
        Cicle _temp = CurrentCycleCheck();
        //Debug.Log($"Cicle = {_temp}");

        //*******사이클 애니메이션을 실행할 때마다, door에 손잡이를 회전하는 코드 추가

        //현재 좌표와 이전 좌표의 차를 가져온다.
        switch (currCicleState)
        {
            //해당 상태가 초기 상태라면 값을 주고 사이클 1회 판단.
            case Cicle.Idle:
                if (_temp != Cicle.Idle)
                {
                    currCicleState = _temp;
                    deltaPos = Vector2.zero;
                    cicleBif++;
                }
                break;
            case Cicle.RightUp:
                if(_temp == Cicle.LeftUp)
                {
                    currCicleState = _temp;
                    deltaPos = Vector2.zero;
                    cicleBif++;
                }
                else
                {
                    if (deltaPos.x > 0) deltaPos.x = 0.0f;
                }
                break;
            case Cicle.LeftUp:
                if (_temp == Cicle.LeftDown)
                {
                    currCicleState = _temp;
                    deltaPos = Vector2.zero;
                    cicleBif++;
                }
                else
                {
                    if (deltaPos.y > 0) deltaPos.y = 0.0f;
                }
                break;
            case Cicle.LeftDown:
                if (_temp == Cicle.RightDown)
                {
                    currCicleState = _temp;
                    deltaPos = Vector2.zero;
                    cicleBif++;
                }
                else
                {
                    if (deltaPos.x < 0) deltaPos.x = 0.0f;
                }
                break;
            case Cicle.RightDown:
                if (_temp == Cicle.RightUp)
                {
                    currCicleState = _temp;
                    deltaPos = Vector2.zero;
                    cicleBif++;
                }
                else
                {
                    if (deltaPos.y < 0) deltaPos.y = 0.0f;
                }
                break;
        }
        
        if(cicleBif == 4)
        {
            //Door의 손잡이 회전을 위한 함수
            cicleBif = 0;
            
            //사이클 액션이 완료되면 바로 액션을 실행한다.
            if (++cicle_currNum == cicle_clearNum)
            {
                //mapManager.DoorClear();
                //isActionEnd = true;
            }
            Debug.Log($"현재 사이클 횟수 : {cicle_currNum}");
        }
    }

    // 현재 사이클 동작에 대한 4분기점으로 판단을 위한 것
    Cicle CurrentCycleCheck()
    {

        //X값에 대한 판단
        //x가 양수일때
        if (deltaPos.x > cinter) y = 0;
        else if (-deltaPos.x > cinter) y = 1;
        //Debug.Log($"X : {x}");

        //Y값에 대한 판단
        //y값이 양수 일때

        if (deltaPos.y > cinter) x = 0;
        else if (-deltaPos.y > cinter) x = 1;
        //Debug.Log($"y : {y}");
        //Debug.Log(cicleArr[x, y]);

        //값이 없는 경우 캔슬
        if (x == 44 || y == 44)
        {
            Debug.Log("Cicle Idle");
            return Cicle.Idle;
        }

        
        return cicleArr[x,y];
    }


    #region ITEM
    bool isAuto = false;
    public void Item_Auto(bool active)
    {
        if (active)
        {
            isAuto = true;
        }
        else
        {
            isAuto = false;
        }
    }
    #endregion
}
