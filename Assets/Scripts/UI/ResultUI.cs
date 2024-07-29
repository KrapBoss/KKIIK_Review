using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultUI : MonoBehaviour
{
    //��� �̹������� ������ �̹��� ������Ʈ
    public GameObject Gameover_Finish;
    public GameObject Playing_Finish;

    //���� �޺��� ���� ���� �������� ����
    public TMP_Text txt_combo_color;
    
    //�� ���ڸ� ������ ������
    public TMP_Text txt_score;
    public TMP_Text txt_combo;
    public TMP_Text txt_bcoin;

    public TMP_Text txt_time;

    //���� ��� ���� ������ ������ ����
    public TMP_Text txt_result;

    //��ŵ �� �ð� ����
    bool skip;

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    //Ȱ��ȭ �Ǹ� ������ �ջ��ؼ� ��������.
    private void OnEnable()
    {
        skip = false;

        float combo_mul = 1.0f; // �޺� �⺻ ���  =1
        int score = UIManager.instance.inGameUI.GetScore();     //���� ����� ������
        int combo = EventManager.instance.ComboQuater;
        float b_coin = 0.1f; // ���� ��� ���

        //�ؽ�Ʈ �ʱ�ȭ
        txt_combo_color.text = "";
        txt_score.text = "";
        txt_combo.text = "";
        txt_result.text = "";
        txt_bcoin.text = $"| BCOIN x{b_coin}";

        DataManager.instance.SetMoney((int)(score * combo_mul * b_coin));

        StartCoroutine(ResultCroutine(score, combo, b_coin, combo_mul));
        StartCoroutine(DisableCroutine(5.0f));
    }

    WaitForSeconds result_time;
    // �����̸� �༭ �ϳ��� ��� ���� ǥ���� �ش�.
    IEnumerator ResultCroutine(int score, int combo, float b_coin, float combo_mul)
    {
        result_time = new WaitForSeconds(0.7f);


        //�̹��� Ȱ��ȭ
        if (GameManager.instance.GameState.Equals(GAMESTATE.GAMEOVER))
        {
            Gameover_Finish.SetActive(true);
            Playing_Finish.SetActive(false);
        }
        else
        {
            Gameover_Finish.SetActive(false);
            Playing_Finish.SetActive(true);
        }


        Color c = Color.white;
        switch (combo)
        {
            case 0: //�ƹ��͵� �ܰ赵 �ö��� ����.
                break;
            case 1: //1�ܰ� �޺�
                c = Color.green;
                combo_mul = 1.2f;
                break;
            case 2: //2�ܰ� �޺�
                combo_mul = 1.5f;
                c = Color.blue;
                break;
            case 3:  //3�ܰ� �޺�
                combo_mul = 2.0f;
                c = Color.red;
                break;
            default: //4�ܰ� �̻�
                combo_mul = 3.0f;
                c = Color.gray;
                break;
        }
        txt_combo_color.color = c;
        txt_combo_color.text = $"| COMBO x{combo_mul}";


        yield return result_time;

        txt_score.text = string.Format("{0:#,###}", score);
        yield return result_time;

        txt_combo.text = string.Format("{0:#,###}", score * combo_mul);
        yield return result_time;


        txt_result.text = string.Format("{0:#,###}", (int)(score * combo_mul * b_coin));
        yield return result_time;


        skip = true;
    }

    WaitForSeconds disable_time = new WaitForSeconds(1f);
    IEnumerator DisableCroutine(float time)
    {
        float t =time;
        txt_time.text = string.Format("{0:0}",t);
        while (t >0.0f)
        {
            yield return disable_time;
            t -= 1f;
            txt_time.text = string.Format("{0:0}", t);
        }

        GameManager.instance.GameStateChange(GAMESTATE.STANDBY);// ������ ��� ���·� �����.
        this.gameObject.SetActive(false);
    }


    public void ResultButton()
    {
        StartCoroutine(ResultButtonCroutine());
    }

    IEnumerator ResultButtonCroutine()
    {
        result_time = new WaitForSeconds(0.05f);

        yield return new WaitUntil(() => skip);

        yield return new WaitForSeconds(1f);

        GameManager.instance.GameStateChange(GAMESTATE.STANDBY);// ������ ��� ���·� �����.
        this.gameObject.SetActive(false);
    }
}
