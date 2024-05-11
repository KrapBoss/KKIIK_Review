using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Text;
using UnityEngine.UI;


public class GoogleManager : MonoBehaviour
{
    public static GoogleManager instance;

    //public Text txt;

    bool login = false;
    bool loaded = false;
    public bool isLogin
    {
        get { return login; }
        private set { login = value; }
    }

    public bool isLoaded
    {
        get { return loaded; }
        private set { loaded = value; }
    }

    public DATA TempData
    {
        get;
        private set;
    }

    private void Awake()
    {
        instance = this;
    }

    public void Login()     //�α���
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();    //������Ȱ��ȭ
        PlayGamesPlatform.InitializeInstance(config);

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        Debug.LogWarning("���� �α��� ��û");
        if (PlayGamesPlatform.Instance.localUser.authenticated == false)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    isLogin = true;
                    Debug.LogWarning("���� �α��� ����");
                }
                else
                {
                    Application.Quit();
                }
            });
        }
    }



    #region Save / Load
    ISavedGameMetadata myGame;//���� ���� ������ ������ ��Ÿ ������

    string myData_String;
    byte[] myData_Binary;


    //////////
    // SAVE //
    //////////
    public void Save(string d)
    {
        Debug.LogError($"GOOGLE Save{myGame.Description} / {myGame.Filename}");
        SaveData(myGame , d);
    }

    void SaveData(ISavedGameMetadata game, string data)             //��Ÿ �����͸� �����Ͽ� 
    {                 
        myData_Binary = Encoding.UTF8.GetBytes(data);               //String�� Byte Ÿ������ ���ڵ�

        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build(); //������ ���� ��ü ����
        ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, update, myData_Binary, SaveCallBack);//������ ����
    }

    private void SaveCallBack(SavedGameRequestStatus status, ISavedGameMetadata game)   //Save �ݹ� �Լ�
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.LogWarning("GOOGLE Success Save");
            Load();
        }
        else
        {
            Debug.LogError($"GOOGLE Save Failed{game.Description} / {myGame.Filename}");
        }
    }


    //////////
    // LOAD //
    //////////
    
    public  void Load()
    {
        Debug.LogWarning("DataLoad 1");
        isLoaded = false;
        //���ϸ��� �Ѱ��ָ鼭 ����Ǿ� �ִ� ���� ��Ÿ�����͸� �����´�.
        ((PlayGamesPlatform)Social.Active).SavedGame.
                OpenWithAutomaticConflictResolution("save", DataSource.ReadCacheOrNetwork, 
                                                    ConflictResolutionStrategy.UseLastKnownGood, LoadGame);
    }

    void LoadGame(SavedGameRequestStatus status, ISavedGameMetadata game)       //Load �ݹ��Լ�
    {
        if (status == SavedGameRequestStatus.Success)                           //��Ÿ �����Ͱ� ���������ٸ�, ���� ��ȯ
        {
            Debug.LogWarning("DataLoad 2");
            myGame = game;
            LoadData(myGame);
        }
        else
        {
            Debug.LogWarning("DataLoad 2 : Faild LoadGame");
        }
    }

    void LoadData(ISavedGameMetadata game)      //***Load�� �ҷ��� ��Ÿ �����͸� �ҷ��� ��ȯ�Ѵ�.
    {
        Debug.LogWarning("DataLoad 3");
        ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, LoadDataCallBack);        //��ȯ
    }


    void LoadDataCallBack(SavedGameRequestStatus status, byte[] LoadedData)
    {
        if (status == SavedGameRequestStatus.Success)                       //������ ��ȯ�� �����ߴٸ�,
        {
            Debug.LogWarning("DataLoad 4");
            try
            {
                myData_String = Encoding.UTF8.GetString(LoadedData);        //Byte To String
                DATA d = JsonUtility.FromJson<DATA>(myData_String);         //Json���� ���� ������ �������� �����Ų��.

                if (d == null)
                {
                    Debug.LogError("GOOGLE �����Ͱ� �����ϴ�.");
                    d = new SaveData();
                }
                else
                {
                    Debug.LogError("GOOGLE ������ �ε� ����");
                }

                TempData = d;
            }
            catch (Exception e)                                             //������ �ε�� ���� ��� �����͸� ���� ����
            {
                Debug.LogError($"Error : {e.Message}  from GoogleManager.cs");

                DATA d = new SaveData();                                        //���ο� �����͸� �����, �ʱ�ȭ.
                Debug.LogError("GOOGLE ������ �ε� ��ȸ �߻� : NEW ������ ���� ");
            }

            isLoaded = true;                                                //�ε尡 �������� �˷��ش�.
        }
        else
        {
            Debug.LogError("GOOGLE ReadBineary_Failed");
        }
    }

    #endregion

}
