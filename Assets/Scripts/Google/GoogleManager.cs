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

    public void Login()     //로그인
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();    //저장기능활성화
        PlayGamesPlatform.InitializeInstance(config);

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        Debug.LogWarning("구글 로그인 요청");
        if (PlayGamesPlatform.Instance.localUser.authenticated == false)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    isLogin = true;
                    Debug.LogWarning("구글 로그인 성공");
                }
                else
                {
                    Application.Quit();
                }
            });
        }
    }



    #region Save / Load
    ISavedGameMetadata myGame;//게임 저장 정보를 가져올 메타 데이터

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

    void SaveData(ISavedGameMetadata game, string data)             //메타 데이터를 지정하여 
    {                 
        myData_Binary = Encoding.UTF8.GetBytes(data);               //String을 Byte 타입으로 엔코딩

        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build(); //저장을 위한 객체 빌드
        ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, update, myData_Binary, SaveCallBack);//데이터 저장
    }

    private void SaveCallBack(SavedGameRequestStatus status, ISavedGameMetadata game)   //Save 콜백 함수
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
        //파일명을 넘겨주면서 저장되어 있는 게임 메타데이터를 가져온다.
        ((PlayGamesPlatform)Social.Active).SavedGame.
                OpenWithAutomaticConflictResolution("save", DataSource.ReadCacheOrNetwork, 
                                                    ConflictResolutionStrategy.UseLastKnownGood, LoadGame);
    }

    void LoadGame(SavedGameRequestStatus status, ISavedGameMetadata game)       //Load 콜백함수
    {
        if (status == SavedGameRequestStatus.Success)                           //메타 데이터가 가져와졌다면, 형식 변환
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

    void LoadData(ISavedGameMetadata game)      //***Load로 불러온 메타 데이터를 불러와 변환한다.
    {
        Debug.LogWarning("DataLoad 3");
        ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, LoadDataCallBack);        //변환
    }


    void LoadDataCallBack(SavedGameRequestStatus status, byte[] LoadedData)
    {
        if (status == SavedGameRequestStatus.Success)                       //데이터 변환이 성공했다면,
        {
            Debug.LogWarning("DataLoad 4");
            try
            {
                myData_String = Encoding.UTF8.GetString(LoadedData);        //Byte To String
                DATA d = JsonUtility.FromJson<DATA>(myData_String);         //Json으로 원래 데이터 포맷으로 변경시킨다.

                if (d == null)
                {
                    Debug.LogError("GOOGLE 데이터가 없습니다.");
                    d = new SaveData();
                }
                else
                {
                    Debug.LogError("GOOGLE 데이터 로드 성공");
                }

                TempData = d;
            }
            catch (Exception e)                                             //데이터 로드시 없을 경우 데이터를 새로 생성
            {
                Debug.LogError($"Error : {e.Message}  from GoogleManager.cs");

                DATA d = new SaveData();                                        //새로운 데이터를 만든다, 초기화.
                Debug.LogError("GOOGLE 데이터 로드 예회 발생 : NEW 데이터 생성 ");
            }

            isLoaded = true;                                                //로드가 끝났음을 알려준다.
        }
        else
        {
            Debug.LogError("GOOGLE ReadBineary_Failed");
        }
    }

    #endregion

}
