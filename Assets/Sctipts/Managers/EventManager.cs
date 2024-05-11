using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// ������ ���� �ϸ鼭 �߻��ϴ� �̺�Ʈ�� ���� �Ѱ��� ��Ƴ��� ����Ѵ�.
/// �̺�Ʈ�� �߻��� �ʿ��� �����͵��� ��Ƴ��´�.
/// </summary>
public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    //�޺��� ���õ� �̺�Ʈ
    [Header("�޺��� ���� ����")]
    public int[] ComboSteps = new int[] {0 ,10, 30, 60, 100 };//�޺��� �ǴܵǴ� �ܰ踦 �����Ѵ�. //0�� �ʱ�ȭ �����̴�.
    public Action<int> Action_Combo { get; set; }        //�޺��� ���Ͽ� ���������� �߻����Ѿ� �ϴ� �̺�Ʈ
    public Action<int> Action_ComboQuater { get; set; }  //�޺� �б���
    public int ComboQuater { get; private set; }         //�ְ�� ����� �޺��� �б����� ��Ÿ����.
    int MaxCombo;                                        //�޺� �ְ�����
    int combo;                                           //���� ����� �޺��� ��


    //�г�Ƽ�� ������ �Լ�
    public Action Action_Panelty { get; set; }

    //ī�޶� ����
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

    //�� �ʱ�ȭ
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

    void Init() //���� ���� �� �ʱ�ȭ�� �����Ͽ�
    {
        combo = 0;
        MaxCombo = 0;
        ComboQuater = 0;

        if(Action_Combo != null)Action_Combo(combo);
        if (Action_ComboQuater != null)Action_ComboQuater(ComboQuater);
    }

    //�޺��� �ش��ϴ� �̺�Ʈ�� �����Ѵ�.
    public void EventCombo(bool _success)
    {
        combo = _success ? combo + 1 : 0;
        Action_Combo(combo);

        if(combo > MaxCombo) MaxCombo = combo;

        //�޺��� �б����� �����ٸ� �ش� �б����� �ش��ϴ� �̺�Ʈ�� �߻���Ų��.
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


    //ī�޶� ���� �̺�Ʈ ����
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
