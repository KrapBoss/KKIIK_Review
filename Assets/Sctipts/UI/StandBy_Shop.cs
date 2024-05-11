using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
/*
 * Shop UI를 담는다.
 */
public class StandBy_Shop : MonoBehaviour
{
    [Header("코인을 담는다.")]
    [SerializeField] Button btn_exit;
    [SerializeField] TMP_Text txt_coin;


    [Space]
    [Header("랜덤 상자")]
    [SerializeField] Button btn_chest;
    [SerializeField] Button btn_random;


    [SerializeField] Button[] btn_BuyCoin;
    [SerializeField] Button[] btn_BuyAd;
    [SerializeField] TMP_Text[] txt_ItemNum;

    [Space]
    [Header("아이템 이미지")]
    public Sprite[] sprite_item;

    [Space]
    [Header("아이템 개수 선택창")]
    [SerializeField] StandBy_SelectItemNum cs_selBox;

    //아이템 선택과 선택된 아이템의 코드를 받음
    bool isSelectItems; //현재 아이템이 상자에 선택되어 있는가?
    int[] items_num;    //아이템의 소유 개수


    //상자 뽑기 관련된 변수
    CODE_ITEM chestItem;//현재 상자에 선택된 아이템을 나타냄.
    int chestItem_num;  //현재 선택한 아이템의 개수

    private void Start()
    {
        btn_exit.onClick.AddListener(() => { gameObject.SetActive(false); });
        //상자관련된 옵션을 설정한다.
        btn_chest.onClick.AddListener(() => 
            { cs_selBox.gameObject.SetActive(true); cs_selBox.Init_Chest(items_num); });
        //아이템을 랜덤으로 뽑음
        btn_random.onClick.AddListener(() => { RandomItem(); });

        for(int i = 0; i< btn_BuyCoin.Length; i++)
        {
            CODE_ITEM code = ((CODE_ITEM)i);
            btn_BuyCoin[i].onClick.AddListener(() => { BuyItemCoin(code); });
            btn_BuyAd[i].onClick.AddListener(() => { BuyItemAd(code); });
        }
    }

    //처음 열릴 때 초기화
    public void Setting()
    {
        btn_random.image.sprite = sprite_item[(int)CODE_ITEM.MAX_NUM];
        cs_selBox.gameObject.SetActive(false);

        Reload();

        isSelectItems = false;
    }


    //아이템의 개수와 코인 갱신
    void Reload()
    {
        items_num = DataManager.instance.GetItems();

        for (int i = 0; i < btn_BuyAd.Length; i++)
        {
            txt_ItemNum[i].text = string.Format("{0}", items_num[i]);
        }

        txt_coin.text = string.Format("{0:#,##0}", DataManager.instance.GetMoney());
    }

    //아이템을 뽑는다.
    void RandomItem()
    {
        if (!isSelectItems) {
            cs_selBox.gameObject.SetActive(true);
            cs_selBox.Init_Chest(items_num);
            return; 
        } //아이템이 선택되어 있지 않다면 킵

        //만약 현재 아이템이 없다면 킵
        int mul = 0;
        int rand = Random.Range(0, 100);
        if(rand < 1) { Debug.Log("1"); mul = 9; }           //기존 값에서 더해지는 것이다.
        else if(rand < 3) { Debug.Log("3"); mul = 4; }
        else if(rand < 5) { Debug.Log("5"); mul = 3; }
        else if(rand < 11) { Debug.Log("11"); mul = 2; }
        else if(rand < 20) { Debug.Log("20"); mul = 1; }
        else if(rand < 50) { Debug.Log("50"); mul = 0; }
        else { Debug.Log("꽝"); mul = -1; }

        Debug.LogWarning($"축하합니다 당첨! / {mul} * {chestItem_num} = {mul * chestItem_num}");

        //저장
        DataManager.instance.SetITem(chestItem, mul * chestItem_num);

        //초기화
        Setting();
    }

    //코인으로 사는 아이템을 활성화시킴
    void BuyItemCoin( CODE_ITEM code)
    {
        Debug.Log("Coin" + code);
        cs_selBox.gameObject.SetActive(true);
        cs_selBox.Init_Buy(code, DataManager.instance.GetMoney());
    }

    void BuyItemAd(CODE_ITEM code)
    {
        Debug.Log("AD"+code);

        //광고 시청 후 보상 지급
        StartCoroutine(BuyItemAdCroutine(code));
    }

    IEnumerator BuyItemAdCroutine(CODE_ITEM code)
    {
        AdMobManager.instance.ShowRewardedAd();

        yield return new WaitUntil(() =>  AdMobManager.instance.isRewardEnd );

        //저장
        DataManager.instance.SetITem(code, 1);

        Reload();
    }
    

    //선택된 아이템과 구매된 개수를 넘겨받음.
    public void BuyItem(CODE_ITEM item, int num)
    {
        if (num < 1) return;
        //현재 아이템을 저장하고, 돈을 감소시킴.
        int money = -(num*1000);

        DataManager.instance.SetMoney(money);
        DataManager.instance.SetITem(item, num);

        Reload();
    }

    //선택된 아이템과 선택된 개수를 넘겨받음
    public void SelectChest(CODE_ITEM item, int num)
    {
        if (num <= 0) return;   // 선택된 개수가 없다면 아무것도 적용이 안된다.

        isSelectItems = true;
        chestItem = item;
        chestItem_num = num;

        //선택된 이미지를 삽입
        btn_random.image.sprite = sprite_item[(int)chestItem];
    }
}
