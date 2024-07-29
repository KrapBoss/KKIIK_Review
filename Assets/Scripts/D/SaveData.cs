using System.Linq;
using System;

[Serializable]
public  enum CODE_DECO1 // ���ڷ��̼� �������� ����
{
    DEFAULT,
    MAX_NUM//������ �迭 ����
}

[Serializable]
public  enum CODE_ITEM //�������� �̸��� ����
{
    RUSH,
    AUTO,
    RECOVERY,
    MAX_NUM //������ �迭 ����
}

[Serializable]
public  struct STATUS
{
    float RUN_SOCRE_MUL;    //�޸��� ���
    float COIN_RESULT_MUL;  //��� ���� ���
}

[Serializable]
public  struct OptionValue //���� �ɼ� ��
{
    public float BGM;
    public float SOUND;
}

[Serializable]
public class DATA
{
    public int STAGE;             //���� ��������

    public int MONEY;        //��ȭ

    public bool[] DOOR_DECOS_1;   //ȹ���� ���ڷ��̼�
    public int[] ITEMS;       //������ ������ ����
    public STATUS STATUS;     //���׷��̵� ������ �ɷ�ġ
    public CODE_DECO1 SELECT_DECO1;// ���õǾ� �ִ� ���ڷ��̼�
}

[Serializable]
internal class SaveData : DATA
{
    internal SaveData() // �⺻ ����
    {
        this.STAGE = 0;
        this.MONEY = 10000;
        this.DOOR_DECOS_1 = Enumerable.Repeat(false, (int)CODE_DECO1.MAX_NUM).ToArray();
        this.ITEMS = Enumerable.Repeat(1, (int)CODE_ITEM.MAX_NUM).ToArray();
        this.SELECT_DECO1 = CODE_DECO1.DEFAULT;
    }

    internal void SetData(DATA d)   //����� ������ ����
    {
        STAGE = d.STAGE;
        MONEY = d.MONEY;
        DOOR_DECOS_1 = d.DOOR_DECOS_1;
        this.ITEMS = d.ITEMS;
        this.SELECT_DECO1 = d.SELECT_DECO1;
    }

    internal void SetStage(int s) { this.STAGE = s; }
    internal void SetMoney(int m) { this.MONEY += m; }
    internal void SetDoorDeco1(int idx) { this.DOOR_DECOS_1[idx] = true; }
    internal int[] GetItem(){     return ITEMS;    }
    internal void SetItem(CODE_ITEM _code, int num)
    {
        int code = (int)_code;
        if((ITEMS[code] + num) < 0)
        {
            ITEMS[code] = 0;
            return;
        }

        ITEMS[code] += num;
    }
}