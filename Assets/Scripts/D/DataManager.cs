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

        //사운드를 크기를 불러와 지정해준다.
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
        //로그인 된 상태를 판단한다.
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
    //사운드 설정 값을 가져온다.
    public OptionValue LoadOptionValue()
    {
        //Debug.Log("LoadOptionValue");
        if (!PlayerPrefs.HasKey(BGM))   //초기값이 없을 경우 대입
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

        //효과음을 변경하지 않았다면, 소리 테스트 X
        if (optionValue.SOUND.Equals(_value.SOUND))
        {
            SoundManager.instance.SettingSound(_value);
            return;
        }

        SoundManager.instance.SettingSound(_value);
        //소리 크기 셋팅
    }
    #endregion

    #region GET/SET/IS
    public void SetStage(int s) // 스테이지 클리어마다 저장한다.
    {
        if ((s - SAVEDATA.STAGE) != 1) //1 이상의 수로  증가할 경우 거른다.
        {
            Debug.LogWarning("스테이지가 잘못 저장되었습니다.");
            Application.Quit();
            return;
        }
        else
        {
            Debug.LogWarning("스테이지가 저장되었습니다.");
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

    //아이템을 개당 개수를 증가시킨다.
    public void SetITem(CODE_ITEM code, int num)
    {
        if (num > 100) return;
        SAVEDATA.SetItem(code, num);

        DataSave();
    }
    #endregion
}