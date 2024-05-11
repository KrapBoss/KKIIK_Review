using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGame_Item : MonoBehaviour
{
    [Space]
    [Header("아이템 타입")]
    public CODE_ITEM code;

    [Space]
    [Header("현재 아이템 쿨타임, 선택 등을 나타냄")]
    public Image img_select;

    [Space]
    [Header("각 상황에 따른 컬러")]
    [SerializeField]Color color_select;
    [SerializeField]Color color_active;
    [SerializeField]Color color_cool;

    bool isCool = false;//쿨타임중인지를 나타냄
    bool isActive = false;//활성화되어있는지를 나타냄

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

    //아이템을 활성화하면서 활성화시간 적용
    void Active()
    {
        img_select.color = color_active;

        activeTime = Items.instance.Active(code);
        time = 0.0f;

        isActive = true;
    }

    //활성화를 한 후에 쿨타임을 적용한다.
    void CoolTime()
    {
        img_select.color = color_cool;

        coolTime = Items.instance.DeActive(code);
        time = coolTime;

        isCool = true;
    }
}
