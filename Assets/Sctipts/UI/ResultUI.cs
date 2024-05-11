using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultUI : MonoBehaviour
{
    //결과 이미지에서 보여줄 이미지 오브젝트
    public GameObject Gameover_Finish;
    public GameObject Playing_Finish;

    //최종 콤보에 따라 색을 지정해줄 변수
    public TMP_Text txt_combo_color;
    
    //각 숫자를 보여줄 변수들
    public TMP_Text txt_score;
    public TMP_Text txt_combo;
    public TMP_Text txt_bcoin;

    public TMP_Text txt_time;

    //최종 결과 코인 개수를 보여줄 변수
    public TMP_Text txt_result;

    //스킵 시 시간 조절
    bool skip;

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    //활성화 되면 점수를 합산해서 집계해줌.
    private void OnEnable()
    {
        skip = false;

        float combo_mul = 1.0f; // 콤보 기본 배수  =1
        int score = UIManager.instance.inGameUI.GetScore();     //현재 얻어진 점수값
        int combo = EventManager.instance.ComboQuater;
        float b_coin = 0.1f; // 최종 결산 배수

        //텍스트 초기화
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
    // 딜레이를 줘서 하나씩 결과 값을 표현해 준다.
    IEnumerator ResultCroutine(int score, int combo, float b_coin, float combo_mul)
    {
        result_time = new WaitForSeconds(0.7f);


        //이미지 활성화
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
            case 0: //아무것도 단계도 올라가지 못함.
                break;
            case 1: //1단계 콤보
                c = Color.green;
                combo_mul = 1.2f;
                break;
            case 2: //2단계 콤보
                combo_mul = 1.5f;
                c = Color.blue;
                break;
            case 3:  //3단계 콤보
                combo_mul = 2.0f;
                c = Color.red;
                break;
            default: //4단계 이상
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

        GameManager.instance.GameStateChange(GAMESTATE.STANDBY);// 게임을 대기 상태로 만든다.
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

        GameManager.instance.GameStateChange(GAMESTATE.STANDBY);// 게임을 대기 상태로 만든다.
        this.gameObject.SetActive(false);
    }
}
