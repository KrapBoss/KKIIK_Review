using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 사운드, 진동, 종료 등을 지정
 * */
public class Option : MonoBehaviour
{
    public Slider slider_bgm;   //배경음
    public Slider slider_sound; //효과음


    float sound = 0;
    //켜지면 자동으로 데이터를 불러와 셋팅
    private void OnEnable()
    {
        OptionValue v = DataManager.instance.LoadOptionValue();

        slider_bgm.value = (v.BGM <= 1.0f) ? v.BGM : 1.0f;
        slider_sound.value = (v.SOUND <= 1.0f) ? v.SOUND : 1.0f;

        sound = slider_sound.value;
    }

    /*아래 함수는 버튼에 직접 삽입*/
    //게임 종료 버튼을 눌렀을 경우
    public void ExitButton()
    {
        Debug.Log("ExitButton");
        UIManager.instance.standbyUI.ActiveExitPanel();
    }
    
    //변경된 옵션 저장
    public void ApplyButton()
    {
        Debug.Log("ApplyButton");
        OptionValue v;

        v.BGM = slider_bgm.value;
        v.SOUND = slider_sound.value;

        DataManager.instance.SaveOptionValue(v);

        if (sound != slider_sound.value)
        {
            Debug.Log("sound 변경");
            sound = slider_sound.value;
            SoundManager.instance.FootStep();
        }
    }

    public void CancleButton()
    {
        Debug.Log("CancleButton");
        this.gameObject.SetActive(false);
    }
}
