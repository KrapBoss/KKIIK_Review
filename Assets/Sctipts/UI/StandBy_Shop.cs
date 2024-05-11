using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
/*
 * Shop UI�� ��´�.
 */
public class StandBy_Shop : MonoBehaviour
{
    [Header("������ ��´�.")]
    [SerializeField] Button btn_exit;
    [SerializeField] TMP_Text txt_coin;


    [Space]
    [Header("���� ����")]
    [SerializeField] Button btn_chest;
    [SerializeField] Button btn_random;


    [SerializeField] Button[] btn_BuyCoin;
    [SerializeField] Button[] btn_BuyAd;
    [SerializeField] TMP_Text[] txt_ItemNum;

    [Space]
    [Header("������ �̹���")]
    public Sprite[] sprite_item;

    [Space]
    [Header("������ ���� ����â")]
    [SerializeField] StandBy_SelectItemNum cs_selBox;

    //������ ���ð� ���õ� �������� �ڵ带 ����
    bool isSelectItems; //���� �������� ���ڿ� ���õǾ� �ִ°�?
    int[] items_num;    //�������� ���� ����


    //���� �̱� ���õ� ����
    CODE_ITEM chestItem;//���� ���ڿ� ���õ� �������� ��Ÿ��.
    int chestItem_num;  //���� ������ �������� ����

    private void Start()
    {
        btn_exit.onClick.AddListener(() => { gameObject.SetActive(false); });
        //���ڰ��õ� �ɼ��� �����Ѵ�.
        btn_chest.onClick.AddListener(() => 
            { cs_selBox.gameObject.SetActive(true); cs_selBox.Init_Chest(items_num); });
        //�������� �������� ����
        btn_random.onClick.AddListener(() => { RandomItem(); });

        for(int i = 0; i< btn_BuyCoin.Length; i++)
        {
            CODE_ITEM code = ((CODE_ITEM)i);
            btn_BuyCoin[i].onClick.AddListener(() => { BuyItemCoin(code); });
            btn_BuyAd[i].onClick.AddListener(() => { BuyItemAd(code); });
        }
    }

    //ó�� ���� �� �ʱ�ȭ
    public void Setting()
    {
        btn_random.image.sprite = sprite_item[(int)CODE_ITEM.MAX_NUM];
        cs_selBox.gameObject.SetActive(false);

        Reload();

        isSelectItems = false;
    }


    //�������� ������ ���� ����
    void Reload()
    {
        items_num = DataManager.instance.GetItems();

        for (int i = 0; i < btn_BuyAd.Length; i++)
        {
            txt_ItemNum[i].text = string.Format("{0}", items_num[i]);
        }

        txt_coin.text = string.Format("{0:#,##0}", DataManager.instance.GetMoney());
    }

    //�������� �̴´�.
    void RandomItem()
    {
        if (!isSelectItems) {
            cs_selBox.gameObject.SetActive(true);
            cs_selBox.Init_Chest(items_num);
            return; 
        } //�������� ���õǾ� ���� �ʴٸ� ŵ

        //���� ���� �������� ���ٸ� ŵ
        int mul = 0;
        int rand = Random.Range(0, 100);
        if(rand < 1) { Debug.Log("1"); mul = 9; }           //���� ������ �������� ���̴�.
        else if(rand < 3) { Debug.Log("3"); mul = 4; }
        else if(rand < 5) { Debug.Log("5"); mul = 3; }
        else if(rand < 11) { Debug.Log("11"); mul = 2; }
        else if(rand < 20) { Debug.Log("20"); mul = 1; }
        else if(rand < 50) { Debug.Log("50"); mul = 0; }
        else { Debug.Log("��"); mul = -1; }

        Debug.LogWarning($"�����մϴ� ��÷! / {mul} * {chestItem_num} = {mul * chestItem_num}");

        //����
        DataManager.instance.SetITem(chestItem, mul * chestItem_num);

        //�ʱ�ȭ
        Setting();
    }

    //�������� ��� �������� Ȱ��ȭ��Ŵ
    void BuyItemCoin( CODE_ITEM code)
    {
        Debug.Log("Coin" + code);
        cs_selBox.gameObject.SetActive(true);
        cs_selBox.Init_Buy(code, DataManager.instance.GetMoney());
    }

    void BuyItemAd(CODE_ITEM code)
    {
        Debug.Log("AD"+code);

        //���� ��û �� ���� ����
        StartCoroutine(BuyItemAdCroutine(code));
    }

    IEnumerator BuyItemAdCroutine(CODE_ITEM code)
    {
        AdMobManager.instance.ShowRewardedAd();

        yield return new WaitUntil(() =>  AdMobManager.instance.isRewardEnd );

        //����
        DataManager.instance.SetITem(code, 1);

        Reload();
    }
    

    //���õ� �����۰� ���ŵ� ������ �Ѱܹ���.
    public void BuyItem(CODE_ITEM item, int num)
    {
        if (num < 1) return;
        //���� �������� �����ϰ�, ���� ���ҽ�Ŵ.
        int money = -(num*1000);

        DataManager.instance.SetMoney(money);
        DataManager.instance.SetITem(item, num);

        Reload();
    }

    //���õ� �����۰� ���õ� ������ �Ѱܹ���
    public void SelectChest(CODE_ITEM item, int num)
    {
        if (num <= 0) return;   // ���õ� ������ ���ٸ� �ƹ��͵� ������ �ȵȴ�.

        isSelectItems = true;
        chestItem = item;
        chestItem_num = num;

        //���õ� �̹����� ����
        btn_random.image.sprite = sprite_item[(int)chestItem];
    }
}
