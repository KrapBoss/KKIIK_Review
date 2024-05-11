using UnityEngine;
using System.Collections;

public class DataManager : MonoBehaviour
{
    SaveData SAVEDATA;


    public static DataManager instance;

    readonly string BGM = "BGM";
    readonly string SOUND = "SOUND";

    OptionValue optionValue;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        SAVEDATA = new SaveData();
        if (!GameManager.instance.test)
        {
            LoadData();
        }

        //���带 ũ�⸦ �ҷ��� �������ش�.
        SoundManager.instance.SettingSound(LoadOptionValue());
    }

    private void OnDestroy()
    {
        instance = null;
    }

    #region SAVE/LOAD
    void DataSave()
    {
        Debug.LogError("DataManager : DATASAVE");

        if (!GameManager.instance.test)
        {
            UIManager.instance.Save();

            string sd = JsonUtility.ToJson(SAVEDATA as DATA);

            GoogleManager.instance.Save(sd);
        }
    }

    public bool loadComplete { get; private set; }
    void LoadData()
    {
        Debug.LogError("DataManager : DATA LOAD ");
        loadComplete = false;
        StartCoroutine(LoadDataCroutine());
    }
    IEnumerator LoadDataCroutine()
    {
        //�α��� �� ���¸� �Ǵ��Ѵ�.
        if (!GoogleManager.instance.isLogin)
        {
            yield return new WaitUntil(() => GoogleManager.instance.isLogin);
            Debug.LogWarning("DATAMANAGE :: Logined");
        }

        GoogleManager.instance.Load();
        yield return new WaitUntil (() => GoogleManager.instance.isLoaded);
        Debug.LogWarning("DATAMANAGE :: DataLoaded");

        SAVEDATA.SetData(GoogleManager.instance.TempData);
        loadComplete = true;
    }
    #endregion

    #region Option SAVE
    //���� ���� ���� �����´�.
    public OptionValue LoadOptionValue()
    {
        //Debug.Log("LoadOptionValue");
        if (!PlayerPrefs.HasKey(BGM))   //�ʱⰪ�� ���� ��� ����
        {
            PlayerPrefs.SetFloat(BGM, 1.0f);
            PlayerPrefs.SetFloat(SOUND, 1.0f);
        }

        optionValue.BGM = PlayerPrefs.GetFloat(BGM);
        optionValue.SOUND = PlayerPrefs.GetFloat(SOUND);

        return optionValue;
    }

    public void SaveOptionValue(OptionValue _value)
    {
        //Debug.Log($"SaveOptionValue{_value.BGM} // {_value.SOUND}");
        PlayerPrefs.SetFloat(BGM, _value.BGM);
        PlayerPrefs.SetFloat(SOUND, _value.SOUND);

        //ȿ������ �������� �ʾҴٸ�, �Ҹ� �׽�Ʈ X
        if (optionValue.SOUND.Equals(_value.SOUND))
        {
            SoundManager.instance.SettingSound(_value);
            return;
        }

        SoundManager.instance.SettingSound(_value);
        //�Ҹ� ũ�� ����
    }
    #endregion

    #region GET/SET/IS
    public void SetStage(int s) // �������� Ŭ����� �����Ѵ�.
    {
        if ((s - SAVEDATA.STAGE) != 1) //1 �̻��� ����  ������ ��� �Ÿ���.
        {
            Debug.LogWarning("���������� �߸� ����Ǿ����ϴ�.");
            Application.Quit();
            return;
        }
        else
        {
            Debug.LogWarning("���������� ����Ǿ����ϴ�.");
            SAVEDATA.SetStage(s);
            DataSave();
        }
    }

    public int GetStage()
    {
        Debug.Log("DATAMANAGER::GETSTAGE");
        return SAVEDATA.STAGE;
    }

    public void SetMoney(int m)
    {
        SAVEDATA.SetMoney(m);
        DataSave();
    }

    public int GetMoney()
    {
        //Debug.LogWarning($"DATAMANAGER::GETMONEY {SAVEDATA.MONEY}");
        return SAVEDATA.MONEY;
    }

    public  int[] GetItems()
    {
        return SAVEDATA.GetItem().Clone() as int[];
    }
    public int GetItem(CODE_ITEM code)
    {
        return SAVEDATA.GetItem()[(int)code];
    }

    //�������� ���� ������ ������Ų��.
    public void SetITem(CODE_ITEM code, int num)
    {
        if (num > 100) return;
        SAVEDATA.SetItem(code, num);

        DataSave();
    }
    #endregion
}