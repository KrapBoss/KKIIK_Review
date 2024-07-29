using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * 길들을 생성하고, 가지고 있음.
*/
public class Ways : MonoBehaviour
{
    float way_length =0; //생성된 길의 총 길이를 나타낸다.
    string MAP_NAME = null;

    List<BaseInfo> list_ways = new List<BaseInfo>();

    Transform playerT;

    private void Start()
    {
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public bool Create(int createNum, string mapName, int quater)        //길을 생성해준다.
    {
        string _mapName = $"{mapName}/{ mapName}Way{quater}";
        if (MAP_NAME == _mapName) return false;//맵이 이미 같을 경우 생성 X
        Clear();

        //**분기점을 불러오는 변수 추가

        Debug.LogWarning($"생성합니다. {_mapName}");
        GameObject way = Resources.Load<GameObject>($"{_mapName}");
        if (way == null) return false;

        for (int i = 0; i < createNum; i++)
        {
            list_ways.Add(Instantiate<GameObject>(way, transform).GetComponent<BaseInfo>());
            list_ways[i].gameObject.SetActive(false);
        }

        MAP_NAME = _mapName;
        way = null;
        Resources.UnloadUnusedAssets();
        return true;
    }

    //오브젝트가 존
    void Clear()
    {
        if (list_ways.Count == 0) return;

        foreach(BaseInfo g in list_ways)
        {
            Destroy(g.gameObject);
        }
        list_ways.Clear();
    }
    
    //땅들을 방의 입구를 기준으로 배치를 한다.
    public void Bacth(float bacth_z)
    {
        for(int i = 0; i < list_ways.Count; i++)
        {
            list_ways[i].SetPostion(bacth_z + list_ways[i].SIZE_Z * i);
            list_ways[i].gameObject.SetActive(true);
        }
        way_length = list_ways[list_ways.Count - 1].transform.position.z + list_ways[list_ways.Count - 1].SIZE_Z;
    }


    float size_update = 0;
    //길을 플레이어 위치에 따라 지속적으로 업데이트한다.
    //게임 시작과 동시에 실행되고 게임 종료와 동시에 사라진다.
    public void RunUpdate()     //==> Action으로 실행시킬 예정
    {
        size_update = list_ways[0].SIZE_Z * 2;
        StartCoroutine(RunUpdateCroutine());
    }
    WaitForSeconds time_run = new WaitForSeconds(0.5f);
    IEnumerator RunUpdateCroutine()
    {
        while (true)
        {
            yield return time_run;
            for (int i = 0; i < list_ways.Count; i++)
            {
                if (playerT.position.z > (list_ways[i].CURRENT_Z +size_update))
                {
                    list_ways[i].SetPostion(way_length);
                    way_length += list_ways[i].SIZE_Z;
                }
            }
        }
    }

    //종료가 됨에 따라 
    public void RunUpdateStop()
    {
        StopAllCoroutines();
    }
}
