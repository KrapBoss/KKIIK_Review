using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * ����, ����, ���� ���� ����
 * */
public class Option : MonoBehaviour
{
    public Slider slider_bgm;   //�����
    public Slider slider_sound; //ȿ����


    float sound = 0;
    //������ �ڵ����� �����͸� �ҷ��� ����
    private void OnEnable()
    {
        OptionValue v = DataManager.instance.LoadOptionValue();

        slider_bgm.value = (v.BGM <= 1.0f) ? v.BGM : 1.0f;
        slider_sound.value = (v.SOUND <= 1.0f) ? v.SOUND : 1.0f;

        sound = slider_sound.value;
    }

    /*�Ʒ� �Լ��� ��ư�� ���� ����*/
    //���� ���� ��ư�� ������ ���
    public void ExitButton()
    {
        Debug.Log("ExitButton");
        UIManager.instance.standbyUI.ActiveExitPanel();
    }
    
    //����� �ɼ� ����
    public void ApplyButton()
    {
        Debug.Log("ApplyButton");
        OptionValue v;

        v.BGM = slider_bgm.value;
        v.SOUND = slider_sound.value;

        DataManager.instance.SaveOptionValue(v);

        if (sound != slider_sound.value)
        {
            Debug.Log("sound ����");
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
