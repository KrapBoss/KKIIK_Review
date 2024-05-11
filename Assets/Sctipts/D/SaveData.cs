using System.Linq;
using System;

[Serializable]
public  enum CODE_DECO1 // 데코레이션 종류별로 정의
{
    DEFAULT,
    MAX_NUM//생성될 배열 개수
}

[Serializable]
public  enum CODE_ITEM //아이템의 이름을 정의
{
    RUSH,
    AUTO,
    RECOVERY,
    MAX_NUM //마지막 배열 개수
}

[Serializable]
public  struct STATUS
{
    float RUN_SOCRE_MUL;    //달리기 배수
    float COIN_RESULT_MUL;  //결산 코인 배수
}

[Serializable]
public  struct OptionValue //사운드 옵션 값
{
    public float BGM;
    public float SOUND;
}

[Serializable]
public class DATA
{
    public int STAGE;             //현재 스테이지

    public int MONEY;        //재화

    public bool[] DOOR_DECOS_1;   //획득한 데코레이션
    public int[] ITEMS;       //아이템 종류별 개수
    public STATUS STATUS;     //업그레이드 가능한 능력치
    public CODE_DECO1 SELECT_DECO1;// 선택되어 있는 데코레이션
}

[Serializable]
internal class SaveData : DATA
{
    internal SaveData() // 기본 설정
    {
        this.STAGE = 0;
        this.MONEY = 10000;
        this.DOOR_DECOS_1 = Enumerable.Repeat(false, (int)CODE_DECO1.MAX_NUM).ToArray();
        this.ITEMS = Enumerable.Repeat(1, (int)CODE_ITEM.MAX_NUM).ToArray();
        this.SELECT_DECO1 = CODE_DECO1.DEFAULT;
    }

    internal void SetData(DATA d)   //저장된 데이터 삽입
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