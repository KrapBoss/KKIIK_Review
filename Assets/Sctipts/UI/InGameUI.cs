using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    [Header("ž������Ʈ")]
    public GameObject obj_TopInfo;
    [Header("���൵")]
    public Slider slider_progressbar;
    [Header("����")]
    public TMP_Text txt_score;
    public TMP_Text txt_mulSocre;

    [Space]
    [Header("Die")]
    [SerializeField]InGame_Dead cs_dead;

    [Space]
    [Header("Fenalty UI")]
    [SerializeField] Image image_fenalty;

    [Space]
    [Header("��� ȭ��")]
    [SerializeField] GameObject resultUI;

    [Space]
    [Header("������")]
    [SerializeField] GameObject itemUI;

    [Space]
    [Header("�޺�")]
    [SerializeField] TMP_Text txt_combo;
    RectTransform rect_combo;

    int score = 0; // ���� ����Ǵ� ����
    
    int created_door = 0; // ������� ���� ������ �����Ѵ�.
    int count_openDoor = 0; // ���� �� Ƚ���� ī��Ʈ �Ѵ�.

    //�⺻ ������ �������� ��
    float default_mulScore = 0.125f; // �� �ڸ������� ����
    float apply_mul_score;

    //�⺻������ �߰��Ǵ� ����
    readonly int default_sub_score = 100;

    // Start is called before the first frame update
    void Start()
    {
        //Set Values
        rect_combo = txt_combo.GetComponent<RectTransform>();


        //Set Event
        EventManager.instance.Action_Panelty += PenaltyEffect;               //�г�Ƽ �̺�Ʈ �Լ� 
        EventManager.instance.Action_Combo += EventCombo;                   //�������� �̺�Ʈ

        GameManager.instance.Action_ALLClearDoor += AllClearDoor;
        GameManager.instance.Action_Playing_Gameover += Playing_Gameover;       //�÷��̾� ���
        GameManager.instance.Action_Gameover_Finish += Gameover_Finish;
    }

    //�����鼭 ��Ȱ��ȭ
    private void OnDisable()
    {
        Debug.Log("InGameUI Disable");

        cs_dead.gameObject.SetActive(false);
        itemUI.SetActive(false);

        StopAllCoroutines();
        increaseProgressCoroutine = null;
    }

    public void GameSetting()      //������ �� �������ش�.
    {
        //������ ���ǿ��� ������ �� �ʱ�ȭ�� ���ش�.
        if (!GameManager.instance.oneDie)
        {
            Debug.Log("�ΰ��� ���� �ʱ�ȭ");

            resultUI.SetActive(false);
            obj_TopInfo.SetActive(true);

            created_door = MapManager.instance.GetCreatedObject();  //������� ����.

            count_openDoor = 0; //���� �� Ƚ���� ī��Ʈ�Ѵ�.
            score = 0;          //������ �����Ѵ�.
            increaseProgress_SubValue = 0.005f; //�⺻ ������
            txt_score.text = score.ToString();  //������ å��

            apply_mul_score = Mathf.Floor(default_mulScore * 1000) * 0.001f;
            txt_mulSocre.text = string.Format("{0:0.00}", apply_mul_score);

            slider_progressbar.value = 0;   //���� ���̴� ����� �ʱ�ȭ
        }

        itemUI.SetActive(true);
        cs_dead.gameObject.SetActive(false);
    }

    public void Playing_Gameover()//���� �� �׾��� ���
    {
        //Die �г� Ȱ��ȭ
        cs_dead.gameObject.SetActive(true);
        itemUI.SetActive(false);

        //���� �г� Ȱ��ȭ
        cs_dead.gameObject.SetActive(true);
        cs_dead.Init();

        //Progress�� �� ä��
        if(increaseProgressCoroutine != null)
        {
            StopAllCoroutines();
            slider_progressbar.value = (count_openDoor / (float)created_door);
        }
        
    }

    //���ӿ��� Ż�� �� �̾��ϱ� X
    public void Gameover_Finish()
    {
        //��� ȭ�� Ȱ��ȭ
        resultUI.SetActive(true);

        itemUI.SetActive(false);
        obj_TopInfo.SetActive(false);
        cs_dead.gameObject.SetActive(false);
    }

    ////����� ������ ���� ����
    IEnumerator increaseProgressCoroutine = null;   // ���� �ڷ�ƾ�� �ѱ� ����
    float value_increase_progress = 0;              //������� ��ǥ�� [�ش� ���� �����ص� ������� ������]
    float increaseProgress_SubValue = 0.001f;                 //������� ������


    //���ھ ���� ����
    public void OpenDoor()          //���� ������ �� ī��Ʈ�� �Ѵ�.
    {
        count_openDoor++;
        score += (int)(default_sub_score * apply_mul_score);
        txt_score.text = string.Format("{0:#,###}", score);

        Progress();

        // ���� �� ������ ������� ���� ���ٸ� =[��� Ŭ����], ���൵�� �����ϴ� ���� ����
        if (count_openDoor >= created_door) 
        {
            increaseProgress_SubValue = 0.01f;
        }
    }

    void Progress() // ����� ����
    {
        //������� ���Ѵ�.
        float v_progress = (count_openDoor / (float)created_door);
        if (count_openDoor == created_door) v_progress = 1;

        //����� ������ ���� �ڷ�ƾ
        if (increaseProgressCoroutine == null)   //����� �ڷ�ƾ�� ���� ���� �ʴٸ� ����
        {
            increaseProgressCoroutine = IncreaseProgressCroutine();
            value_increase_progress = v_progress;
            StartCoroutine(increaseProgressCoroutine);
        }
        else    //�̹� �ڷ�ƾ�� ���� �ִٸ� ��ǥ���� �����Ѵ�.
        {
            value_increase_progress = v_progress;
        }
    }

    WaitForSeconds waitTime_progress = new WaitForSeconds(0.03f);
    IEnumerator IncreaseProgressCroutine() // ���൵ ������ �ε巴�� �ϱ� ����.
    {
        float slider_value = slider_progressbar.value;
        while (value_increase_progress > slider_value)
        {
            slider_value += increaseProgress_SubValue;
            slider_progressbar.value = slider_value;

            //Debug.Log($"����� �ڷ�ƾ �۵� �� {value_increase_progress} | {slider_value}");

            yield return waitTime_progress;
        }

        //��ƾ�� ������ �ڷ�ƾ�� �� ��ü�� �����.
        increaseProgressCoroutine = null;
    }

    //���� �ش��ϴ� ������ ���� ���� ������ �ش� UI �ʱ�ȭ
    public void AllClearDoor()
    {
        if (increaseProgressCoroutine != null)
        {
            StopCoroutine(increaseProgressCoroutine);
            increaseProgressCoroutine = null;
        }

        slider_progressbar.value = 1;
        StartCoroutine(AllClearDoorCroutine());
    }
    IEnumerator AllClearDoorCroutine()
    {
        yield return new WaitForSeconds(1.0f);
        //���α׷����� �ʱ�ȭ
        created_door = MapManager.instance.GetCreatedObject();
        count_openDoor = 0;

        increaseProgress_SubValue = 0.005f; //�⺻ ������
        slider_progressbar.value = 0;
    }


    //�̺�Ʈ�� �����Ѵ�.
    public void EventCombo(int _combo)
    {
        txt_combo.text = _combo.ToString();
    }

    //�޺��� �ش��ϴ� �б����� �Ѱܹ޴� ��쿡 �ش��Ѵ�.
    public void EventComboQuater(int _quater)
    {
        
    }

    void PenaltyEffect()
    {
        image_fenalty.gameObject.SetActive(true);

        Invoke("DelayPenaltyEffect", 0.5f);
    }
    void DelayPenaltyEffect()
    {
        image_fenalty.gameObject.SetActive(false);
    }

    public int GetScore()
    {
        return score;
    }
}
