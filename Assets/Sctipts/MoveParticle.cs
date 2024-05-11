using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//플레이어 스피드에 연계가 되기 때문에 PlayerController에 참조시킴
//GameManager에서 끄고 킴

#pragma warning disable 0414
#pragma warning disable 0618
public class MoveParticle : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystemRenderer psr;


    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        psr = GetComponent<ParticleSystemRenderer>();
    }


    //기존 스피드와 renderer의 ScaleSpeed를 변경해준다.
    public void SetSpeed(float PlayerSpeed)
    {
        ps.startSpeed = PlayerSpeed * 0.2f;
        psr.lengthScale = PlayerSpeed * 0.5f;
    }


    //게임 시작 시 파티클의 동작을 시작한다.
    public void PlayMoveParticle()
    {
        ps.Play();
    }

    //게임 종료 시 파티클의 동작을 중지한다.
    public void StopMoveParticle()
    {
        ps.Stop();
    }
}
