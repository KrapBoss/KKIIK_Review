using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;


public class StandBy_SelectItemNum : MonoBehaviour
{
    [SerializeField] Image image_item;
    [SerializeField] TMP_Text txt_num;
    int currentNum; // ���� ������ ����

    [Space]
    [Header("������ �̹���")]
    [SerializeField] Image m_image;


    bool isChest;       //���ڿ� �������� �ִ� ���
    int[] item_max = new int[(int)(CODE_ITEM.MAX_NUM)];     //�� ��Ȳ�� ���� ������ �ִ밪�� ������ ����.

    [Space]
    [Header("�� ��Ȳ���� ���� ų UI")]
    [SerializeField] GameObject chest_ui;
    [SerializeField] GameObject buy_ui;

    int sel_item; // ���� ���õ� �������� ����

    [SerializeField]StandBy_Shop parent;

    [SerializeField] TMP_Text txt_buyMoney;


    //�� �����ۺ� �ִ� ������ �������� ���� �ʱ�ȭ
    public void Init_Chest(int[] _item_max)
    {
        isChest = true;

        chest_ui.SetActive(true);
        buy_ui.SetActive(false);

        currentNum = 0;     //���� ������ �������� ����
        txt_num.text = "0"; //ǥ�Ⱚ �ʱ�ȭ
        m_image.sprite = parent.sprite_item[0]; //�ʱ� �̹��� ����
        sel_item = 0;

        Array.Copy(_item_max, item_max, _item_max.Length);
    }

    //�������� �춧 ������ ���ϱ� ����.
    public void Init_Buy(CODE_ITEM item, int money)
    {
        isChest = false;

        chest_ui.SetActive(false);
        buy_ui.SetActive(true);

        currentNum = 0;
        txt_num.text = "0";
        sel_item = (int)item;   //���� ���õ� ������ ��ȣ�� ����.
        item_max[sel_item] = (money / 1000);    //�ִ� ������ ���� �ִ´�.
        m_image.sprite = parent.sprite_item[sel_item];//���� ���� �̹����� ������

        txt_buyMoney.text = string.Format("{0:#,##0}", currentNum * 1000);    //���� �����
    }






    /// <summary>
    /// ///////////////////////Button
    /// </summary>
    public void LeftButton_NUM()
    {
        if(currentNum > 0)
        {
            currentNum--;
            txt_num.text = currentNum.ToString();

            if (!isChest)
            {
                txt_buyMoney.text = string.Format("{0:#,##0}", currentNum * 1000);
            }
        }
    }
    public void RightButton_NUM()
    {
        //���� �������� �ִ� �������� ���� ���
        if(currentNum < item_max[sel_item])
        {
            currentNum++;
            txt_num.text = currentNum.ToString();

            //���� �������� ������ ��� �ִ� ������ �������� ���� ��츦 �Ǵ�
            if (!isChest)
            {
                txt_buyMoney.text = string.Format("{0:#,##0}",currentNum * 1000);
            }
        }
    }

    public void LeftButton_Item()
    {
        sel_item--;
        if (sel_item < 0) sel_item = (int)CODE_ITEM.MAX_NUM - 1;

        m_image.sprite = parent.sprite_item[sel_item];

        //������ �ʱ�ȭ
        currentNum = 0;
        txt_num.text = "0"; //ǥ�Ⱚ �ʱ�ȭ
    }
    public void RightButton_Item()
    {
        sel_item++;
        if (sel_item > ((int)CODE_ITEM.MAX_NUM - 1)) sel_item = 0;

        m_image.sprite = parent.sprite_item[sel_item];


        currentNum = 0;
        txt_num.text = "0"; //ǥ�Ⱚ �ʱ�ȭ
    }

    //���� ���� ���� �����Ѵ�.
    public void ApplyButton()
    {
        if(isChest)parent.SelectChest((CODE_ITEM)sel_item, currentNum);
        else { parent.BuyItem((CODE_ITEM)sel_item, currentNum); }

        ExitButton();
    }

    public void ExitButton()
    {
        this.gameObject.SetActive(false);
    }
}
