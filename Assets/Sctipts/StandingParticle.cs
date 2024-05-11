using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//위치 설정과 끄고 키기만 하면 되어서 "GameManager"에 들어가 있음.

public class StandingParticle : MonoBehaviour
{
    ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    //생성 될 시 입력 받은 위치보다 카메라에 보이기 위한 오프셋
    [SerializeField]
    float offsetZ = 1;

    public void PlayStandingParticle()
    {
        ps.Play();
    }

    public void StopStandingParticle()
    {
        ps.Pause();
    }

    //플레이어가 죽었거나 초반 위치로 이동 시 자리 지정 후 실행
    public void SetPosition(float z)
    {
        transform.position = new Vector3(0,0, z + offsetZ);
        PlayStandingParticle();
    }
}
