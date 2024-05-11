using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;


public class StandBy_SelectItemNum : MonoBehaviour
{
    [SerializeField] Image image_item;
    [SerializeField] TMP_Text txt_num;
    int currentNum; // 현재 선택한 개수

    [Space]
    [Header("아이템 이미지")]
    [SerializeField] Image m_image;


    bool isChest;       //상자에 아이템을 넣는 경우
    int[] item_max = new int[(int)(CODE_ITEM.MAX_NUM)];     //각 상황에 따른 아이템 최대값을 가지고 있음.

    [Space]
    [Header("각 상황에서 끄고 킬 UI")]
    [SerializeField] GameObject chest_ui;
    [SerializeField] GameObject buy_ui;

    int sel_item; // 현재 선택된 아이템을 저장

    [SerializeField]StandBy_Shop parent;

    [SerializeField] TMP_Text txt_buyMoney;


    //각 아이템별 최대 개수를 가져오고 값들 초기화
    public void Init_Chest(int[] _item_max)
    {
        isChest = true;

        chest_ui.SetActive(true);
        buy_ui.SetActive(false);

        currentNum = 0;     //현재 선택한 아이템의 개수
        txt_num.text = "0"; //표기값 초기화
        m_image.sprite = parent.sprite_item[0]; //초기 이미지 지정
        sel_item = 0;

        Array.Copy(_item_max, item_max, _item_max.Length);
    }

    //아이템을 살때 개수를 정하기 위함.
    public void Init_Buy(CODE_ITEM item, int money)
    {
        isChest = false;

        chest_ui.SetActive(false);
        buy_ui.SetActive(true);

        currentNum = 0;
        txt_num.text = "0";
        sel_item = (int)item;   //현재 선택된 아이템 번호를 넣음.
        item_max[sel_item] = (money / 1000);    //최대 가능한 값을 넣는다.
        m_image.sprite = parent.sprite_item[sel_item];//현재 지정 이미지를 가져옴

        txt_buyMoney.text = string.Format("{0:#,##0}", currentNum * 1000);    //돈을 계산함
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
        //현재 아이템의 최대 갯수보다 작을 경우
        if(currentNum < item_max[sel_item])
        {
            currentNum++;
            txt_num.text = currentNum.ToString();

            //현재 선택지가 상자일 경우 최대 아이템 개수보다 작을 경우를 판단
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

        //개수값 초기화
        currentNum = 0;
        txt_num.text = "0"; //표기값 초기화
    }
    public void RightButton_Item()
    {
        sel_item++;
        if (sel_item > ((int)CODE_ITEM.MAX_NUM - 1)) sel_item = 0;

        m_image.sprite = parent.sprite_item[sel_item];


        currentNum = 0;
        txt_num.text = "0"; //표기값 초기화
    }

    //현재 구한 값을 적용한다.
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
