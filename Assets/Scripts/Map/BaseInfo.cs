using UnityEngine;

public class BaseInfo : MonoBehaviour
{
    public string NAME;//현재 오브젝트의 이름
    public int SIZE_Z;  //현재 오브젝트 사이즈
    [HideInInspector] public float CURRENT_Z;   //현재 오브젝트의 위치 + 사이즈

    public void SetPostion(float z)
    {
        CURRENT_Z = z + SIZE_Z;
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }
}
