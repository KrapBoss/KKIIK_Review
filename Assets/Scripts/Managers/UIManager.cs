using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public bool fadein 
    {
        get; private set;    
    } = false;


    AudioSource audiosource;
    [Space]
    [Header("버튼 터치 사운드")]
    public AudioClip sound_click;
    public AudioClip sound_option;
    public AudioClip sound_cancle;


    //Loading
    [Space]
    [Header("Loading UI")]
    public GameObject go_fade;
    public TMP_Text txt_fade;



    [Space]
    [Header("Save UI")]
    [SerializeField] GameObject image_save;
    [SerializeField] Animation save_anim;

    public StandbyUI standbyUI{ private set; get; }
    public InGameUI inGameUI{ private set; get; }

    private void Awake()
    {
        instance = this;

        audiosource = GetComponent<AudioSource>();
        standbyUI = GetComponentInChildren<StandbyUI>();
        inGameUI = GetComponentInChildren<InGameUI>();

        image_save.SetActive(false);
    }
    private void Start()
    {
        GameManager.instance.Action_Finish_StandBy += StandBySetting;
        GameManager.instance.Action_Standby_Playing += InGameSetting;
        GameManager.instance.Action_Gameover_StandBy += StandBySetting;
    }
    private void OnDestroy()
    {
        instance = null;
    }

    public void StandBySetting()
    {
        Debug.Log("Active StandbyUI");

        //대기실 UI활성화 셋팅
        standbyUI.gameObject.SetActive(true);
        //인게임 비활성화
        inGameUI.gameObject.SetActive(false);

        standbyUI.GameSetting();
    }

    ///스텐드 함수와 인게임 함수를 구분하여 정의
    public void InGameSetting()
    {

        //인게임 활성화
        inGameUI.gameObject.SetActive(true);
        //대기실 비활성화
        standbyUI.gameObject.SetActive(false);

        inGameUI.GameSetting();//셋팅
    }

    public void Save()
    {
        image_save.SetActive(true);
        StartCoroutine(SaveCroutine());
    }

    WaitForSeconds save_time = new WaitForSeconds(1.0f);
    IEnumerator SaveCroutine()
    {
        save_anim.Play();
        yield return save_time;

        save_anim.Play();
        yield return save_time;

        image_save.SetActive(false);
    }
    #region SelectSound
    void SelectSound()
    {
        audiosource.clip = sound_click;
        audiosource.Play();
    }

    void SelectOption()
    {
        audiosource.clip = sound_option;
        audiosource.Play();
    }

    void SelectCancle()
    {
        audiosource.clip = sound_cancle;
        audiosource.Play();
    }
    #endregion

    #region LoadingDisplay

    public void ALLFadeIn()                 //화면 가리개
    {
        if (fadein == true) return;
        go_fade.SetActive(true);
        fadein = true;

        StartCoroutine(FadeInTextCroutine());
    }

    WaitForSeconds txtWaitTime = new WaitForSeconds(0.2f);
    IEnumerator FadeInTextCroutine()            //텍스트 로딩을 표시
    {
        while (true)
        {
            txt_fade.text = "LOADING";
            for(int i = 0; i < 3; i++)
            {
                txt_fade.text += ".";
                yield return txtWaitTime;
            }
        }
    }
    

    public void ALLFadeOut()
    {
        fadein = false;
        StopAllCoroutines();
        go_fade.SetActive(false);
    }
    #endregion
}
