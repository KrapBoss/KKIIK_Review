using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemInfo
{
    public float time_active { get; protected set; }
    public float time_cool { get; protected set; }

    public bool isActive { get; protected set; }   //아이템의 활성화 상태

    public abstract void Active();//아이템을 실질적인 사용
    public abstract void DeActive();//아이템 사용 후 아이템 효과 제거
    public void Init() { isActive = false; Debug.LogWarning("아이템 초기화"); }
}


namespace Item
{

    public class Item_Rush : ItemInfo
    {
        internal Item_Rush()
        {
            time_active = 5.0f;
            time_cool = 20.0f;
            isActive = false;
        }

        public override void Active()
        {
            Debug.Log("아이템 사용 :: Item_Rush");
            isActive = true;

            //플레이어 속도 증가 및 문을 부시고 나가기.
            PlayerController.instance.Item_Rush(true);
            
        }

        public override void DeActive()
        {
            isActive = false;
            Debug.Log("아이템 비활성 :: Item_Rush");

            PlayerController.instance.Item_Rush(false);
        }
    }

    public class Item_Auto : ItemInfo
    {
        internal Item_Auto()
        {
            time_active = 3.0f;
            time_cool = 20.0f;
            isActive = false;
        }

        public override void Active()
        {
            Debug.Log("아이템 사용 :: Item_Auto");
            isActive = true;

            //어떤 입력을 해도 자동으로 문을 열고 앞으로 갈 수 있도록 구현
            TouchAction.instance.Item_Auto(true);
        }

        public override void DeActive()
        {
            isActive = false;
            Debug.Log("아이템 비활성 :: Item_Auto");

            TouchAction.instance.Item_Auto(false);
        }
    }

    public class Item_Recovery : ItemInfo
    {
        internal Item_Recovery()
        {
            time_active = 0.0f;
            time_cool = 0.0f;
            isActive = true;
        }

        public override void Active()
        {
            Debug.Log("아이템 사용 :: Item_Recovery");
            isActive = true;
            //활성화 되어있는 동안에는 자동으로 부활
        }

        public override void DeActive()
        {
            isActive = false;
            Debug.Log("아이템 비활성 :: Item_Recovery");
        }
    }
}

