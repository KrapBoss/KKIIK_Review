using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��ġ ������ ���� Ű�⸸ �ϸ� �Ǿ "GameManager"�� �� ����.

public class StandingParticle : MonoBehaviour
{
    ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    //���� �� �� �Է� ���� ��ġ���� ī�޶� ���̱� ���� ������
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

    //�÷��̾ �׾��ų� �ʹ� ��ġ�� �̵� �� �ڸ� ���� �� ����
    public void SetPosition(float z)
    {
        transform.position = new Vector3(0,0, z + offsetZ);
        PlayStandingParticle();
    }
}
