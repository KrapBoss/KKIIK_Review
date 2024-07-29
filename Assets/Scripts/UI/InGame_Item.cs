using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGame_Item : MonoBehaviour
{
    [Space]
    [Header("������ Ÿ��")]
    public CODE_ITEM code;

    [Space]
    [Header("���� ������ ��Ÿ��, ���� ���� ��Ÿ��")]
    public Image img_select;

    [Space]
    [Header("�� ��Ȳ�� ���� �÷�")]
    [SerializeField]Color color_select;
    [SerializeField]Color color_active;
    [SerializeField]Color color_cool;

    bool isCool = false;//��Ÿ���������� ��Ÿ��
    bool isActive = false;//Ȱ��ȭ�Ǿ��ִ����� ��Ÿ��

    private void OnEnable()
    {
        Color c = img_select.color;
        c.a = 0;
        img_select.color = c;

        isActive = false;
        isCool = false;

        Items.instance.Init(code);
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { ClickButton(); });
    }

    float time = 0.0f;
    float activeTime = 0.0f;
    float coolTime = 0.0f;
    private void Update()
    {
        if (isActive)
        {
            img_select.fillAmount = (time / activeTime);

            time += Time.deltaTime;

            if (time > activeTime)
            {
                isActive = false;
                img_select.fillAmount = 1;
                CoolTime();
            }
        }
        if (isCool)
        {
            img_select.fillAmount = (time / coolTime);

            time -= Time.deltaTime;

            if (time < 0.0f)
            {
                isCool = false;
                img_select.fillAmount = 0;
            }
        }
    }

    void ClickButton()
    {
        if (isCool || isActive) return;
        if (Items.instance.IsActive()) return;

        Active();
    }

    //�������� Ȱ��ȭ�ϸ鼭 Ȱ��ȭ�ð� ����
    void Active()
    {
        img_select.color = color_active;

        activeTime = Items.instance.Active(code);
        time = 0.0f;

        isActive = true;
    }

    //Ȱ��ȭ�� �� �Ŀ� ��Ÿ���� �����Ѵ�.
    void CoolTime()
    {
        img_select.color = color_cool;

        coolTime = Items.instance.DeActive(code);
        time = coolTime;

        isCool = true;
    }
}
