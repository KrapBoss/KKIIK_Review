using UnityEngine;

public class BaseInfo : MonoBehaviour
{
    public string NAME;//���� ������Ʈ�� �̸�
    public int SIZE_Z;  //���� ������Ʈ ������
    [HideInInspector] public float CURRENT_Z;   //���� ������Ʈ�� ��ġ + ������

    public void SetPostion(float z)
    {
        CURRENT_Z = z + SIZE_Z;
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }
}
