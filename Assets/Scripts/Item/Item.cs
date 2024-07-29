
/*
 * �����ۿ� ���� �⺻ ������ �����Ѵ�.
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

    //���ӽð��� ��ȯ�ϸ鼭 ������ ȿ���� Ȱ��ȭ ��Ų��.
    public float Active(CODE_ITEM code)
    {
        int idx = (int)code;

        items[idx].Active();

        return items[idx].time_active;
    }

    //��Ÿ���� ��ȯ�ϸ鼭 ���� ȿ���� �����Ѵ�.
    public float DeActive(CODE_ITEM code)
    {
        int idx = (int)code;

        items[idx].DeActive();

        return items[idx].time_cool;
    }


    //Ȱ��ȭ �������� �ִٸ� �ٸ� ������ ��Ȱ���� ���� ��.
    public bool IsActive()
    {
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i].isActive) return true;
        }
        return false;
    }

    //���� ���� �� ������ Ȱ��ȭ ���� �ʱ�ȭ
    public void Init(CODE_ITEM code)
    {
        items[(int)code].Init();
    }
}