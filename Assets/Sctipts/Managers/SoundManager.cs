using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public struct SoundQuantity
{
    public string name;
    public int BGM;
    public int Door;
    public int Foot;

    [Range(0, 1.0f)]
    public float BGM1_Volum;
    [Range(0, 1.0f)]
    public float FootVolum;
    [Range(0, 1.0f)]
    public float DoorVolum;

    public float FootPitch;
}

public struct AudioInfo
{
    public string name;

    [Range(0.0f,1.0f)]
    public float volume;

}

/// <summary>
/// Resource���� �Ҹ��� ������ �ش� �Ҹ����� ������ ���´�.
/// instance������ ȣ��Ǹ� �ش��ϴ� ������ �Ҹ��� ������ش�.
/// </summary>
/// ��Ģ*******
/// BGM1 �� ���� �����
/// BGM2 �� Ư���� ������̴�.
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("�� ���������� ���� ������ �����ÿ�. MAX (2,2,2)")]
    //�� ���������� �÷��� ������ ���� ����
    [SerializeField]
    SoundQuantity[] soundQuantities;

    //������ �� �ܰ�
    int stageStep = 0;

    //audioSource�� ���� ������
    AudioSource[] sound_bgm; 
    AudioSource[] sound_door;
    AudioSource[] sound_foot;
    AudioSource sound_doorSmash;
    AudioSource sound_monster_smash;
    AudioSource sound_monster_Active;

    //1�� BGM ��� ����
    bool BGMPlaying = false;

    OptionValue optionValue;

    readonly int count_bgm = 2; //BGM ������ ����
    readonly int count_door = 2; // �� ���� ����
    readonly int count_foot = 2; //���ڱ� �Ҹ��� ����

    private void Awake()
    {
        if (instance == null) instance = this;

        sound_bgm = new AudioSource[count_bgm];
        sound_door = new AudioSource[count_door];
        sound_foot = new AudioSource[count_foot];

        //������Ʈ ����
        CreateObj();
        SetStartSetting();
    }


    //���� ���� �� �ʹݿ� ������ų ���� ������Ʈ�� �����Ѵ�.
    void CreateObj()
    {
        int i;
        GameObject _go = null;

        for (i =0; i < count_bgm; i++)
        {
             _go = new GameObject($"Sound_BGM {i}");
            sound_bgm[i] = _go.AddComponent<AudioSource>();
            sound_bgm[i].loop = true;
            sound_bgm[i].playOnAwake = false;
            _go.transform.SetParent(transform);
        }

        for (i = 0; i < count_door; i++)
        {
            _go = new GameObject($"Sound_Door {i}");
            sound_door[i] = _go.AddComponent<AudioSource>();
            sound_door[i].loop = false;
            _go.transform.SetParent(transform);
        }

        for ( i =0; i< count_foot; i++)
        {
            _go = new GameObject($"Sound_Foot {i}");
            sound_foot[i] = _go.AddComponent<AudioSource>();
            sound_foot[i].loop = false;
            sound_foot[i].playOnAwake = false;
            _go.transform.SetParent(transform);
        }


        _go = new GameObject($"Sound_Smash");
        sound_doorSmash = _go.AddComponent<AudioSource>();
        sound_doorSmash.loop = false;
        _go.transform.SetParent(transform);

        _go = new GameObject($"Sound_Monster_Smash");
        sound_monster_smash = _go.AddComponent<AudioSource>();
        sound_monster_smash.loop = false;
        _go.transform.SetParent(transform);

        _go = new GameObject($"Sound_Monster_Active");
        sound_monster_Active = _go.AddComponent<AudioSource>();
        sound_monster_Active.loop = false;
        _go.transform.SetParent(transform);
    }

    //���� �̸��� �޾ƿ� ���� �Ҹ� �帣�� �ٸ��� �ش� �Ҹ��� ���� �ҷ��´�.
    //���� �̸� ��Ģ
    /// <summary>
    /// sound_[MapName]_[����]n
    /// BGM/DOOR/FOOT
    /// ��� > $"{stageName}/Sound/sound_{stageName}[����]n"
    /// </summary>
    public void SetStartSetting()
    {
        //����� ���� ���� ��� STOP
        if (BGMPlaying) { BGMStop(); }

        int i;

        string stageName = GameManager.instance.GetStageName();


        //��� 2
        for (i = 0; i < soundQuantities[stageStep].BGM; i++)
        {
            sound_bgm[i].clip = Resources.Load<AudioClip>($"{stageName}/Sound/sound_{stageName}_BGM{i + 1}");
            sound_bgm[i].volume = soundQuantities[stageStep].BGM1_Volum;
            //Debug.LogWarning("SOUND :: "+soundQuantities[stageStep].BGM1_Volum);
        }

        //���Ҹ� 2��
        for (i = 0; i < soundQuantities[stageStep].Door; i++)
        {
            sound_door[i].clip = Resources.Load<AudioClip>($"{stageName}/Sound/sound_{stageName}_DOOR{i + 1}");

            sound_door[i].volume = soundQuantities[stageStep].DoorVolum;
        }

        //���ڱ� �Ҹ� 3��
        for (i = 0; i < soundQuantities[stageStep].Foot; i++)
        {
            sound_foot[i].clip = Resources.Load<AudioClip>($"{stageName}/Sound/sound_{stageName}_FOOT{i + 1}");
            sound_foot[i].pitch = soundQuantities[stageStep].FootPitch;
            sound_foot[i].volume = soundQuantities[stageStep].FootVolum;
        }

        sound_doorSmash.clip = Resources.Load<AudioClip>($"{stageName}/Sound/sound_{stageName}_SMASH");

        sound_monster_smash.clip = Resources.Load<AudioClip>($"{stageName}/Sound/sound_{stageName}_Monster_SMASH");

        sound_monster_Active.clip = Resources.Load<AudioClip>($"{stageName}/Sound/sound_{stageName}_Monster_ACTIVE");

        BGMPlaying = false;

        Resources.UnloadUnusedAssets();

        ////������ �ҷ��� �ʰ� �ٸ��ٸ� �ٽ� �ҷ��´�.
        //if (GameManager.instance.GetStep() != stageStep)
        //{
        //    stageStep = GameManager.instance.GetStep();
        //    string stageName = GameManager.instance.GetStageName();
        //}
    }


    #region �߼Ҹ�
    int footNumber = 0;
    //���ڱ� �Ҹ��� ���� ���� ��
    public void FootStep()
    {
        int _foot = Random.Range(0, soundQuantities[stageStep].Foot);
        if (_foot == footNumber) footNumber = (footNumber + 1) % soundQuantities[stageStep].Foot;
        else footNumber = _foot;

        sound_foot[footNumber].Play();
    }

    //�߼Ҹ��� ����
    public void FootStepStop()
    {
        for(int i =0; i < soundQuantities[stageStep].Foot; i++)
        {
            sound_foot[i].Stop();
        }
    }

    //�߼Ҹ��� ������ �����.
    public void FootSlow()
    {
        //Debug.Log("SOUND :: �߼Ҹ��� �۾���");
        for (int i = 0; i < count_foot; i++)
        {
            sound_foot[i].volume *= 0.2f;
        }
    }

    //�߼Ҹ� ���󺹱�
    public void FootRestor()
    {
        Debug.Log("SOUND :: �߼Ҹ��� Ŀ��");
        for (int i = 0; i < count_foot; i++)
        {
            sound_foot[i].volume = soundQuantities[stageStep].FootVolum * optionValue.SOUND;
        }
    }
    #endregion

    public void BGMStop()
    {
        for (int i = 0; i < soundQuantities[stageStep].BGM; i++)
        {
            sound_bgm[i].Stop();
            BGMPlaying = false;
        }
    }

    public void AttackMonster()
    {
        FootStepStop();
        sound_monster_smash.Play();
    }

    //���� �ε����� �� ���� �Ҹ�
    public void BumpDoor()
    {
        FootStepStop();
        sound_doorSmash.Play();
    }
    public void ActiveMonster()
    {
        sound_monster_Active.Play();
    }

    
    //BGM ���
    public void BGMPlay()
    {
        if(!BGMPlaying)
        {
            BGMPlaying = true;
            sound_bgm[0].Play();
        }
    }

    public void DoorOpen()
    {
        sound_door[Random.Range(0, soundQuantities[stageStep].Door)].Play();
    }


    //���� �Ҹ��� ���� ������ �����Ѵ�.
    public void SettingSound(OptionValue _value)
    {
        optionValue = _value;

        //Debug.Log("���� ���� ����˴ϴ�.");
        int i;
        for(i =0;i<count_bgm; i++)
        {
            sound_bgm[i].volume = _value.BGM * soundQuantities[stageStep].BGM1_Volum;
        }
        for(i = 0; i< count_door; i++)
        {
            sound_door[i].volume = _value.SOUND * soundQuantities[stageStep].DoorVolum;
        }
        for(i =0;i<count_foot; i++)
        {
            sound_foot[i].volume = _value.SOUND * soundQuantities[stageStep].FootVolum;
        }

        Debug.Log("���带 ������");

        BGMPlay();
    }
}
