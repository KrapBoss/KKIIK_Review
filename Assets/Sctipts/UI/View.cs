using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Ű����, ���콺, ��ġ�� �̺�Ʈ�� ������Ʈ�� ���� �� �ִ� ����� ����
using UnityEngine.UI;

public class View : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    CameraMoveEffect moveEffect;

    [SerializeField] RectTransform eye_rect;
    [SerializeField] RectTransform slider_rect;

    Vector2 BeginPos;

    [SerializeField] float rotSize;
    float slider_size;


    [Space]
    [Header("�����̴��� ��ġ")]
    public float StandBy_anchorPos;
    public float InGame_anchorPos;

    [SerializeField]
    Animator anim;

    private void Start()
    {
        moveEffect = FindObjectOfType<CameraMoveEffect>();
        eyeImage = GetComponent<Image>();

        slider_size = slider_rect.rect.width *0.5f;

        slider_rect.gameObject.SetActive(false);

        GameManager.instance.Action_Standby_Playing += StandBy_Playing;
        GameManager.instance.Action_Finish_StandBy += _StandBy;
        GameManager.instance.Action_Gameover_StandBy += _StandBy;

        GameManager.instance.Action_Playing_Gameover += _Gameover;
    }

    //��ġ/Ŭ�� ����
    public void OnPointerDown(PointerEventData eventData)
    {
        ViewIconInit();
        BeginPos = eye_rect.position;
        slider_rect.gameObject.SetActive(true);
    }

    float delta_x;

    Vector2 temp_pos; // ȸ���� ���� ���� ���� �����ϱ� ���� ����
    //�巡�� ��
    public void OnDrag(PointerEventData eventData)
    {
        delta_x = eventData.position.x - BeginPos.x;

        if (delta_x > slider_size) delta_x = slider_size;
        else if (-delta_x > slider_size) delta_x = -slider_size;

        float delta_t = delta_x / slider_size;
        moveEffect.RotateView(delta_t);

        temp_pos.Set(BeginPos.x + delta_x, BeginPos.y);

        eye_rect.position = temp_pos;
    }

    //��ġ ����
    public void OnPointerUp(PointerEventData eventData)
    {
        ViewIconInit();
        slider_rect.gameObject.SetActive(false);

        moveEffect.RotateView(0);

        Debug.Log("�巡�� ����");
    }

    void ViewIconInit()
    {
        if (GameManager.instance.GameState.Equals(GAMESTATE.STANDBY))
        {
            eye_rect.anchoredPosition = new Vector2(0, StandBy_anchorPos);
            slider_rect.anchoredPosition = new Vector2(0, StandBy_anchorPos);
        }
        else
        {
            eye_rect.anchoredPosition = new Vector2(0, InGame_anchorPos);
            slider_rect.anchoredPosition = new Vector2(0, InGame_anchorPos);
        }

    }

    public void StandBy_Playing()
    {
        StartCoroutine(DelayCroutine(new Vector2(0, InGame_anchorPos)));
    }
    public void _StandBy()
    {
        eye_rect.gameObject.SetActive(true);
        StartCoroutine(DelayCroutine(new Vector2(0, StandBy_anchorPos)));
    }

    Image eyeImage;
    IEnumerator DelayCroutine(Vector2 v)
    {
        eyeImage.raycastTarget = false;

        anim.SetTrigger("Close");

        yield return new WaitForSeconds(0.2f);

        eye_rect.anchoredPosition = v;
        slider_rect.anchoredPosition = v;

        anim.SetTrigger("Open");

        eyeImage.raycastTarget = true;
    }

    public void _Gameover()
    {
        eye_rect.gameObject.SetActive(false);
        slider_rect.gameObject.SetActive(false);
    }
}
