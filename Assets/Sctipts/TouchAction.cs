using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



/*
 * ȭ���� ��ġ���� ���� �̺�Ʈ�� ���ؼ� MapManager�� ������ �Ѱ��ش�.
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
   

    //���� ��ġ�Ǵ� ��ǥ ���� �Ѱ��ش�.
    byte x = 44, y = 44;
    //����Ŭ �Ǵ��� ���� ����
    float cinter;

    //����Ŭ�� ���� ����
    int cicleBif = 0;

    //ȭ�� ��ġ ���� ���� ����
    public int interval = 40;

    Vector2 deltaPos  = Vector2.zero;

    //���� ����� ������ ���� �����Ȳ�� �̸� ����. 
    Cicle[,] cicleArr = new Cicle[2,2]{ {Cicle.RightUp, Cicle.LeftUp},
                                        {Cicle.RightDown , Cicle.LeftDown } };
    Cicle currCicleState;

    Touch touch;//��ġ �̺�Ʈ�� ���� ����

    //���� ������ Touch �׼��� ������ ���̴�.
    DoorType currActionType;
    //���� ��� ���� �̺�Ʈ ������ �Ϸ����� ��� �ߺ��� �����Ѵ�.
    //bool isActionEnd = false;

    //����Ŭ Ƚ�� �Ǵܿ� �ʿ��� ���� // �⺻ = 2
    int cicle_clearNum = 2;
    //����Ŭ Ƚ���� ���Ƹ��� ����
    int cicle_currNum = 0;

    public float penltyTime = 0.5f;

    //ť�� ���� ���� ������ ������ ������� ������ ���̴�.
    Queue<ActivityObject> queue_activityObjects = new Queue<ActivityObject>();
    //�÷��̾� �տ� �ִ� ���� ������ ���� ����
    ActivityObject activityObject = null;


    //����� �� ���� ���Է��� ����
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

    //���۹�ư�� ������ ��쿡 ��ġ�� �����ϵ��� �Ѵ�.
    public void StartButton()
    {
        GameManager.instance.GameStateChange(GAMESTATE.PLAYING);    //���� ����

        OpenForwardDoor();

        StartCoroutine(DelayEnableTouchCroutine());
    }

    IEnumerator DelayEnableTouchCroutine()
    {
        yield return new WaitForSeconds(0.2f);
        cicle_currNum = 0;       //����Ŭ ��ġ�� �Է� �� �ʱ�ȭ
        deltaPos = Vector2.zero;       //���� �� �Է��� �� ���� �ʱ�ȭ���ش�.
        enableTouch = true; // ��ġ�� ��������
    }

    public void Panelty()
    {
        Debug.LogWarning($"TOUCHACTION :: Panelty Combo = 0");

        EventManager.instance.EventCombo(false);

        enableTouch = false;

        StartCoroutine(DelayEnableTouchCroutine());
    }


    //�ٷ� �տ� �ִ� ���� ����
    public void OpenForwardDoor()
    {
        if (queue_activityObjects.Count > 0)
        {
            activityObject = queue_activityObjects.Peek();  //�÷��̾� �ٷ� �տ� �ִ� ���� ����
            activityObject.Opening();                       //���� ���� ������.
            queue_activityObjects.Dequeue();
        }
    }


    //�迭�� �ʱ�ȭ����
    public void Finish_Standby()
    {
        Debug.Log("TouchAction -> Finish_Standby");
        queue_activityObjects.Clear();
        cicle_currNum = 0;       //����Ŭ ��ġ�� �Է� �� �ʱ�ȭ
        deltaPos = Vector2.zero;       //���� �� �Է��� �� ���� �ʱ�ȭ���ش�.
        enableTouch = false;        //��ġ �Ұ�
        isAuto = false;
    }

    //�迭�� �ʱ�ȭ ���� �ʴ´�.
    public void Gameover_Standby()
    {
        Debug.Log("TouchAction -> Gameover_StandBy");
        cicle_currNum = 0;       //����Ŭ ��ġ�� �Է� �� �ʱ�ȭ
        deltaPos = Vector2.zero;       //���� �� �Է��� �� ���� �ʱ�ȭ���ش�.

        isAuto = false;
    }

    //���� �� ����
    public void Playing_Gameover()
    {
        Debug.Log("TouchAction -> Playing_Gameover");

        //��ġ�� �Ұ��� �ϵ��� ��
        enableTouch = false;
    }

    //������ ���� �����Ѵ�.
    public void StackDoor(ActivityObject _door)
    {
        queue_activityObjects.Enqueue(_door);
    }

    //��ġ�Է��� �޾Ƶ��� �� �ֵ��� ��
    void TouchMotion()
    {
        //�ش� �׼��� ������ ��� ��ũ��Ʈ ����
        //if (isActionEnd) return;

        if (queue_activityObjects.Count < 1) return;

        touch = Input.GetTouch(0);
        activityObject = queue_activityObjects.Peek();//ù��° ��Ҹ� ���� = ���ϸ��� ������ ��


        Debug.Log("TouchMotion");

        //�ʱ� ��ġ ���۽� ������ �ʱ�ȭ �����ش�.
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
            if (activityObject.GetDoorType() != DoorType.Cicle)//�� �Ʒ� �翷 ����� ����
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
                else//�ش� �̺�Ʈ�� ������ �´ٸ� ���� ���� �Ǵ��� �����Ѵ�.
                if (activityObject.GetDoorType().Equals(currActionType))
                {
                    activityObject.Opening();
                    queue_activityObjects.Dequeue();
                    EventManager.instance.EventCombo(true);
                }
                else        //���� �Է��� ���� �ʴ� ���
                {
                    EventManager.instance.EventCombo(false);
                }
            }
        }
    }

    //ȸ���� ���� ó���� �ش� ȸ���� ���� ���¸� �Ǵ����ش�.
    void CycleAction()
    {
        //Debug.Log($"deltaPOs = {cicleDeltaPos}");
        Cicle _temp = CurrentCycleCheck();
        //Debug.Log($"Cicle = {_temp}");

        //*******����Ŭ �ִϸ��̼��� ������ ������, door�� �����̸� ȸ���ϴ� �ڵ� �߰�

        //���� ��ǥ�� ���� ��ǥ�� ���� �����´�.
        switch (currCicleState)
        {
            //�ش� ���°� �ʱ� ���¶�� ���� �ְ� ����Ŭ 1ȸ �Ǵ�.
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
            //Door�� ������ ȸ���� ���� �Լ�
            cicleBif = 0;
            
            //����Ŭ �׼��� �Ϸ�Ǹ� �ٷ� �׼��� �����Ѵ�.
            if (++cicle_currNum == cicle_clearNum)
            {
                //mapManager.DoorClear();
                //isActionEnd = true;
            }
            Debug.Log($"���� ����Ŭ Ƚ�� : {cicle_currNum}");
        }
    }

    // ���� ����Ŭ ���ۿ� ���� 4�б������� �Ǵ��� ���� ��
    Cicle CurrentCycleCheck()
    {

        //X���� ���� �Ǵ�
        //x�� ����϶�
        if (deltaPos.x > cinter) y = 0;
        else if (-deltaPos.x > cinter) y = 1;
        //Debug.Log($"X : {x}");

        //Y���� ���� �Ǵ�
        //y���� ��� �϶�

        if (deltaPos.y > cinter) x = 0;
        else if (-deltaPos.y > cinter) x = 1;
        //Debug.Log($"y : {y}");
        //Debug.Log(cicleArr[x, y]);

        //���� ���� ��� ĵ��
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
