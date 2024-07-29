using UnityEngine;
using System.Collections;
using TMPro;

/*
 * �������� ������ ������ ǥ�����ִ� ��ũ��Ʈ
 * �ִϸ��̼����� 1.0 / 0.5/ 0.5 �� ���� ����
 */
public class StandBy_StageInfo : MonoBehaviour
{
    [SerializeField]Animator anim;
    [SerializeField]TMP_Text txt;

    int ShowMapName = Animator.StringToHash("SHOWMAPNAME");
    int HideMapName = Animator.StringToHash("HIDEMAPNAME");
    int ShowStageNumber = Animator.StringToHash("SHOWSTAGENUMBER");

    //Ȱ��ȭ�� �� �ڵ����� ����
    public void Setting()
    {
        Debug.Log("StandBy_StageInfo");
        StartCoroutine(AnimationCroutine());
    }

    WaitForSeconds waitTime = new WaitForSeconds(1f);
    IEnumerator AnimationCroutine()
    {
        //ȭ���� ������� �ִϸ��̼� ����ȴ�.
        yield return new WaitUntil(() => !UIManager.instance.fadein);

        txt.text = GameManager.instance.GetStageName();
        anim.SetTrigger(ShowMapName);
        yield return waitTime;
        yield return waitTime;

        //���⼭ �̹� �� �������� ������ �ٲ ���̵尡 �ȴ�.
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
