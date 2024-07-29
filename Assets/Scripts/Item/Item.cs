
/*
 * 아이템에 대한 기본 정보를 정의한다.
 */

using Item;
using System;

public class Items
{
    static Items _instance;
    public static Items instance {
        get { 
            if(_instance == null)
            {
                _instance = new Items();
            }
            return _instance;
         }
    }

    ItemInfo[] items = new ItemInfo[]
        {
            new Item_Rush(),
            new Item_Auto()
        };

    //지속시간을 반환하면서 아이템 효과를 활성화 시킨다.
    public float Active(CODE_ITEM code)
    {
        int idx = (int)code;

        items[idx].Active();

        return items[idx].time_active;
    }

    //쿨타임을 반환하면서 기존 효과를 제거한다.
    public float DeActive(CODE_ITEM code)
    {
        int idx = (int)code;

        items[idx].DeActive();

        return items[idx].time_cool;
    }


    //활성화 아이템이 있다면 다른 아이템 비활성을 위한 것.
    public bool IsActive()
    {
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i].isActive) return true;
        }
        return false;
    }

    //게임 시작 시 아이템 활성화 상태 초기화
    public void Init(CODE_ITEM code)
    {
        items[(int)code].Init();
    }
}