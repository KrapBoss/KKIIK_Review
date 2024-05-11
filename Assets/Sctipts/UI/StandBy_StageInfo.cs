using UnityEngine;
using System.Collections;
using TMPro;

/*
 * 스테이지 정보를 가져와 표현해주는 스크립트
 * 애니메이션으로 1.0 / 0.5/ 0.5 초 간격 조절
 */
public class StandBy_StageInfo : MonoBehaviour
{
    [SerializeField]Animator anim;
    [SerializeField]TMP_Text txt;

    int ShowMapName = Animator.StringToHash("SHOWMAPNAME");
    int HideMapName = Animator.StringToHash("HIDEMAPNAME");
    int ShowStageNumber = Animator.StringToHash("SHOWSTAGENUMBER");

    //활성화될 시 자동으로 셋팅
    public void Setting()
    {
        Debug.Log("StandBy_StageInfo");
        StartCoroutine(AnimationCroutine());
    }

    WaitForSeconds waitTime = new WaitForSeconds(1f);
    IEnumerator AnimationCroutine()
    {
        //화면이 띄워지면 애니메이션 실행된다.
        yield return new WaitUntil(() => !UIManager.instance.fadein);

        txt.text = GameManager.instance.GetStageName();
        anim.SetTrigger(ShowMapName);
        yield return waitTime;
        yield return waitTime;

        //여기서 이미 맵 스테이지 정보가 바뀌보 하이드가 된다.
        anim.SetTrigger(HideMapName);
        yield return waitTime;

        txt.text = GameManager.instance.CURRENT_STAGE.ToString();
        anim.SetTrigger(ShowStageNumber);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
