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
/// Resource에서 소리를 가져와 해당 소리들을 저장해 놓는다.
/// instance변수로 호출되면 해당하는 사운드의 소리를 출력해준다.
/// </summary>
/// 규칙*******
/// BGM1 은 평상시 배경음
/// BGM2 는 특별한 배경음이다.
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("각 스테이지별 사운드 수량을 적으시오. MAX (2,2,2)")]
    //각 스테이지별 플레이 가능한 사운드 갯수
    [SerializeField]
    SoundQuantity[] soundQuantities;

    //현재의 맵 단계
    int stageStep = 0;

    //audioSource를 담을 변수들
    AudioSource[] sound_bgm; 
    AudioSource[] sound_door;
    AudioSource[] sound_foot;
    AudioSource sound_doorSmash;
    AudioSource sound_monster_smash;
    AudioSource sound_monster_Active;

    //1번 BGM 재생 여부
    bool BGMPlaying = false;

    OptionValue optionValue;

    readonly int count_bgm = 2; //BGM 사운드의 개수
    readonly int count_door = 2; // 문 사운드 개수
    readonly int count_foot = 2; //발자국 소리의 개수

    private void Awake()
    {
        if (instance == null) instance = this;

        sound_bgm = new AudioSource[count_bgm];
        sound_door = new AudioSource[count_door];
        sound_foot = new AudioSource[count_foot];

        //오브젝트 생성
        CreateObj();
        SetStartSetting();
    }


    //게임 생성 시 초반에 생성시킬 게임 오브젝트를 설정한다.
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

    //맵의 이름을 받아와 이전 소리 장르와 다르면 해당 소리를 새로 불러온다.
    //사운드 이름 규칙
    /// <summary>
    /// sound_[MapName]_[종류]n
    /// BGM/DOOR/FOOT
    /// 경로 > $"{stageName}/Sound/sound_{stageName}[종류]n"
    /// </summary>
    public void SetStartSetting()
    {
        //배경음 동작 중일 경우 STOP
        if (BGMPlaying) { BGMStop(); }

        int i;

        string stageName = GameManager.instance.GetStageName();


        //브금 2
        for (i = 0; i < soundQuantities[stageStep].BGM; i++)
        {
            sound_bgm[i].clip = Resources.Load<AudioClip>($"{stageName}/Sound/sound_{stageName}_BGM{i + 1}");
            sound_bgm[i].volume = soundQuantities[stageStep].BGM1_Volum;
            //Debug.LogWarning("SOUND :: "+soundQuantities[stageStep].BGM1_Volum);
        }

        //문소리 2개
        for (i = 0; i < soundQuantities[stageStep].Door; i++)
        {
            sound_door[i].clip = Resources.Load<AudioClip>($"{stageName}/Sound/sound_{stageName}_DOOR{i + 1}");

            sound_door[i].volume = soundQuantities[stageStep].DoorVolum;
        }

        //발자국 소리 3개
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

        ////이전에 불러온 맵과 다르다면 다시 불러온다.
        //if (GameManager.instance.GetStep() != stageStep)
        //{
        //    stageStep = GameManager.instance.GetStep();
        //    string stageName = GameManager.instance.GetStageName();
        //}
    }


    #region 발소리
    int footNumber = 0;
    //발자국 소리를 내기 위한 것
    public void FootStep()
    {
        int _foot = Random.Range(0, soundQuantities[stageStep].Foot);
        if (_foot == footNumber) footNumber = (footNumber + 1) % soundQuantities[stageStep].Foot;
        else footNumber = _foot;

        sound_foot[footNumber].Play();
    }

    //발소리를 제거
    public void FootStepStop()
    {
        for(int i =0; i < soundQuantities[stageStep].Foot; i++)
        {
            sound_foot[i].Stop();
        }
    }

    //발소리를 느리게 만든다.
    public void FootSlow()
    {
        //Debug.Log("SOUND :: 발소리가 작아짐");
        for (int i = 0; i < count_foot; i++)
        {
            sound_foot[i].volume *= 0.2f;
        }
    }

    //발소리 원상복구
    public void FootRestor()
    {
        Debug.Log("SOUND :: 발소리가 커짐");
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

    //문과 부딪혔을 때 나는 소리
    public void BumpDoor()
    {
        FootStepStop();
        sound_doorSmash.Play();
    }
    public void ActiveMonster()
    {
        sound_monster_Active.Play();
    }

    
    //BGM 재생
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


    //사운드 소리에 대한 값들을 대입한다.
    public void SettingSound(OptionValue _value)
    {
        optionValue = _value;

        //Debug.Log("사운드 값이 변경됩니다.");
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

        Debug.Log("사운드를 셋팅함");

        BGMPlay();
    }
}
