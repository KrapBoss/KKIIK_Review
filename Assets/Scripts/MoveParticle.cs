using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�÷��̾� ���ǵ忡 ���谡 �Ǳ� ������ PlayerController�� ������Ŵ
//GameManager���� ���� Ŵ

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


    //���� ���ǵ�� renderer�� ScaleSpeed�� �������ش�.
    public void SetSpeed(float PlayerSpeed)
    {
        ps.startSpeed = PlayerSpeed * 0.2f;
        psr.lengthScale = PlayerSpeed * 0.5f;
    }


    //���� ���� �� ��ƼŬ�� ������ �����Ѵ�.
    public void PlayMoveParticle()
    {
        ps.Play();
    }

    //���� ���� �� ��ƼŬ�� ������ �����Ѵ�.
    public void StopMoveParticle()
    {
        ps.Stop();
    }
}
