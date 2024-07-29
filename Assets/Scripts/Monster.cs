using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public static Monster instance;

    [SerializeField] float start_time = 3f; //출발 시간 딜레이

    bool isActive = false;//몬스터를 움직여도 되는가?

    float speed = 0.0f;

    Transform player;
    Collider m_collider;

    AudioSource a_source;
    public AudioClip[] clip_laugth;
    public AudioClip clip_die;
    public AudioClip clip_active;

    CameraMoveEffect cameraEffect;
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null) { instance = this; }

        a_source = GetComponent<AudioSource>();
        player = GameObject.FindWithTag("Player").transform;
        cameraEffect = GameObject.FindObjectOfType<CameraMoveEffect>();
        m_collider = GetComponent<Collider>();
        m_collider.enabled = false;

        GameManager.instance.Action_Standby_Playing += StandBy_Playing;
        GameManager.instance.Action_Playing_Gameover += _Gameover;
        EventManager.instance.Action_Panelty += Panelty;
    }

    private void Update()
    {
        if (isActive)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }  

    //몬스터의 위치를 맵의 시작 위치를 받아와 진행
    public void StandBy_Playing()
    {
        m_collider.enabled = false;

        if (GameManager.instance.oneDie)    //이미 죽었던 경우 Playing -  Gameover - standBy
        {
            transform.position = player.position;
        }
        else
        {
            transform.position = MapManager.instance.GetStartMapMonsterPosition();
        }

        StartCoroutine(MoveCroutine());
    }

    WaitForSeconds waitTime = new WaitForSeconds(0.2f);
    IEnumerator MoveCroutine()  // 딜레이 후 몬스터를 움직임.
    {
        while(player.position.z < transform.position.z)
        {
            yield return waitTime;
        }

        yield return new WaitForSeconds(start_time);

        EventManager.instance.EventCameraShake(2.0f, 0.05f);
        m_collider.enabled = true;
        speed = PlayerController.instance.GetSpeed();

        SoundManager.instance.ActiveMonster();

        a_source.clip = clip_active;
        a_source.Play();
        
        isActive = true;
    }

    public void DoorOpenSpeed()
    {
        speed = PlayerController.instance.GetSpeed();
    }

    public void _Gameover() // 죽었을 경우에 처리하기 위함.
    {
        isActive = false;
        m_collider.enabled = false;
    }

    public void Panelty()
    {
        if(isActive)
        {
            int idx = Random.Range(0, clip_laugth.Length);
            a_source.clip = clip_laugth[idx];
            a_source.Play();
        }
    }

    public void ReBatch(float speed)
    {
        if (isActive)//몬스터가 활성화 되어있는 상태일 경우에만 재배치
        {
            transform.position = player.position - new Vector3(0, 0, speed * 3);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            a_source.clip = clip_die;
            a_source.Play();
        }
    }
}
