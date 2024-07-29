using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * ����� �����ϰ�, ������ ����.
*/
public class Ways : MonoBehaviour
{
    float way_length =0; //������ ���� �� ���̸� ��Ÿ����.
    string MAP_NAME = null;

    List<BaseInfo> list_ways = new List<BaseInfo>();

    Transform playerT;

    private void Start()
    {
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public bool Create(int createNum, string mapName, int quater)        //���� �������ش�.
    {
        string _mapName = $"{mapName}/{ mapName}Way{quater}";
        if (MAP_NAME == _mapName) return false;//���� �̹� ���� ��� ���� X
        Clear();

        //**�б����� �ҷ����� ���� �߰�

        Debug.LogWarning($"�����մϴ�. {_mapName}");
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

    //������Ʈ�� ��
    void Clear()
    {
        if (list_ways.Count == 0) return;

        foreach(BaseInfo g in list_ways)
        {
            Destroy(g.gameObject);
        }
        list_ways.Clear();
    }
    
    //������ ���� �Ա��� �������� ��ġ�� �Ѵ�.
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
    //���� �÷��̾� ��ġ�� ���� ���������� ������Ʈ�Ѵ�.
    //���� ���۰� ���ÿ� ����ǰ� ���� ����� ���ÿ� �������.
    public void RunUpdate()     //==> Action���� �����ų ����
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

    //���ᰡ �ʿ� ���� 
    public void RunUpdateStop()
    {
        StopAllCoroutines();
    }
}
