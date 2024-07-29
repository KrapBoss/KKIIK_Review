using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 게임을 진행 하면서 발생하는 이벤트에 대해 한곳에 모아놓고 사용한다.
/// 이벤트를 발생에 필요한 데이터들을 모아놓는다.
/// </summary>
public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    //콤보와 관련된 이벤트
    [Header("콤보와 관련 변수")]
    public int[] ComboSteps = new int[] {0 ,10, 30, 60, 100 };//콤보로 판단되는 단계를 지정한다. //0은 초기화 개념이다.
    public Action<int> Action_Combo { get; set; }        //콤보에 관하여 지속적으로 발생시켜야 하는 이벤트
    public Action<int> Action_ComboQuater { get; set; }  //콤보 분기점
    public int ComboQuater { get; private set; }         //최고로 진행된 콤보의 분기점을 나타낸다.
    int MaxCombo;                                        //콤보 최고점수
    int combo;                                           //현재 진행된 콤보의 수


    //패널티에 관련한 함수
    public Action Action_Panelty { get; set; }

    //카메라 변수
    Transform tf_camera;

    #region StartSetting
    void Awake()
    {
        if (instance == null) instance = this;
    }
    void Start()
    {
        //set Values
        tf_camera = Camera.main.transform;
        
        GameManager.instance.Action_Gameover_StandBy += Gameover_StatandBy;
        GameManager.instance.Action_Playing_Gameover += Playing_Gameover;

        GameManager.instance.Action_Standby_Playing += Init;

        Action_Panelty += () => EventCameraShake(1.0f, 0.05f); 
    }
    #endregion

    //값 초기화
    public void Gameover_StatandBy()
    {
        ComboQuater = 0;
        MaxCombo = 0;
        combo = 0;
    }

    public void Playing_Gameover()
    {
        combo = 0;
    }

    void Init() //게임 시작 시 초기화에 관련하여
    {
        combo = 0;
        MaxCombo = 0;
        ComboQuater = 0;

        if(Action_Combo != null)Action_Combo(combo);
        if (Action_ComboQuater != null)Action_ComboQuater(ComboQuater);
    }

    //콤보에 해당하는 이벤트를 적용한다.
    public void EventCombo(bool _success)
    {
        combo = _success ? combo + 1 : 0;
        Action_Combo(combo);

        if(combo > MaxCombo) MaxCombo = combo;

        //콤보가 분기점을 지난다면 해당 분기점에 해당하는 이벤트를 발생시킨다.
        for(int i = 0; i< ComboSteps.Length; i++)
        {
            if (combo == ComboSteps[i])
            {
                if(ComboQuater < i)ComboQuater = i;
                Action_ComboQuater(i);
                break;
            }
        }
    }


    //카메라 흔들기 이벤트 관련
    IEnumerator cameraShakeCroutine = null;
    public void EventCameraShake(float _shakeTime, float _power)
    {
        if(cameraShakeCroutine != null)
        {
            StopCoroutine(cameraShakeCroutine);
            cameraShakeCroutine = null;
        }
        cameraShakeCroutine = CameraShakeCroutine(_shakeTime , _power);
        StartCoroutine(cameraShakeCroutine);
    }
    IEnumerator CameraShakeCroutine(float _shakeTime, float _power)
    {
        float x, y;
        float t = _shakeTime;

        while (t > 0)
        {
            x = Random.Range(-_power, _power);
            y = Random.Range(-_power, _power);

            tf_camera.transform.localPosition = new Vector3(x, y, tf_camera.transform.localPosition.z);

            t -= Time.deltaTime;
            yield return null;
        }
    }
}
