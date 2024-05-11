using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemInfo
{
    public float time_active { get; protected set; }
    public float time_cool { get; protected set; }

    public bool isActive { get; protected set; }   //�������� Ȱ��ȭ ����

    public abstract void Active();//�������� �������� ���
    public abstract void DeActive();//������ ��� �� ������ ȿ�� ����
    public void Init() { isActive = false; Debug.LogWarning("������ �ʱ�ȭ"); }
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
            Debug.Log("������ ��� :: Item_Rush");
            isActive = true;

            //�÷��̾� �ӵ� ���� �� ���� �νð� ������.
            PlayerController.instance.Item_Rush(true);
            
        }

        public override void DeActive()
        {
            isActive = false;
            Debug.Log("������ ��Ȱ�� :: Item_Rush");

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
            Debug.Log("������ ��� :: Item_Auto");
            isActive = true;

            //� �Է��� �ص� �ڵ����� ���� ���� ������ �� �� �ֵ��� ����
            TouchAction.instance.Item_Auto(true);
        }

        public override void DeActive()
        {
            isActive = false;
            Debug.Log("������ ��Ȱ�� :: Item_Auto");

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
            Debug.Log("������ ��� :: Item_Recovery");
            isActive = true;
            //Ȱ��ȭ �Ǿ��ִ� ���ȿ��� �ڵ����� ��Ȱ
        }

        public override void DeActive()
        {
            isActive = false;
            Debug.Log("������ ��Ȱ�� :: Item_Recovery");
        }
    }
}

