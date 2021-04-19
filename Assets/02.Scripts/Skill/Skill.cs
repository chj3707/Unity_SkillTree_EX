using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// https://docs.unity3d.com/kr/2019.4/Manual/SupportedEvents.html 이벤트 트리거 호출 정리
// 스킬 버튼 관리하는 스크립트
public class Skill : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerClickHandler
{
    [SerializeField]
    public Skill_Info m_Info = new Skill_Info();

    public EventManager m_EManager = null; // 이벤트 매니저

    // 툴팁, 캔버스 크기
    RectTransform m_ActiveToolTip = null; 
    RectTransform m_PassiveToolTip = null;
    CanvasScaler m_CanvasScaler = null;

    void Awake()
    {
        
    }

    void Start()
    {
        // 엑티브 스킬 툴팁의 RectTransform에 접근
        m_ActiveToolTip = ActiveToolTip.Instance.transform.GetChild(0).GetComponent<RectTransform>();

        // 패시브 스킬 툴팁의 RectTransform에 접근
        m_PassiveToolTip = PassiveToolTip.Instance.transform.GetChild(0).GetComponent<RectTransform>();

        // 툴팁이 캔버스 밖으로 나가지 못하도록 설정 할것
        m_CanvasScaler = FindObjectOfType<CanvasScaler>();
    }


    public void OnPointerEnter(PointerEventData p_eventdata)
    {
        // 툴팁 생성 위치 설정 :: 스킬 바로 옆으로 나오도록 설정함
        Vector2 tooltip;
        if (m_Info.skill_type == E_SkillType.Active)
            tooltip = new Vector2(m_ActiveToolTip.rect.width * 0.5f, m_ActiveToolTip.rect.height * -0.5f); // 엑티브
        else
            tooltip = new Vector2(m_PassiveToolTip.rect.width * 0.5f, m_PassiveToolTip.rect.height * -0.5f); // 패시브

        Vector2 skill = new Vector2(this.GetComponent<RectTransform>().rect.width * 0.5f, this.GetComponent<RectTransform>().rect.height * 0.5f);
        Vector2 posvec = new Vector2(transform.position.x, transform.position.y) + tooltip + skill;

        // 툴팁 활성화, 위치 설정, UI 업데이트
        switch (m_Info.skill_type)
        {
            case E_SkillType.Active:
                ActiveToolTip.Instance.gameObject.SetActive(true);
                ActiveToolTip.Instance.transform.position = SetToolTipPosition(posvec, m_Info.skill_type, skill);
                ActiveToolTip.Instance.UpdateUI(m_Info);
                break;
            case E_SkillType.Passive:
                PassiveToolTip.Instance.gameObject.SetActive(true);
                PassiveToolTip.Instance.transform.position = SetToolTipPosition(posvec, m_Info.skill_type, skill);
                PassiveToolTip.Instance.UpdateUI(m_Info);
                break;
        }
    }

    // 툴팁 잘림 방지 함수
    Vector2 SetToolTipPosition(Vector2 p_posvec, E_SkillType p_type, Vector2 p_skillvec)
    {
        Vector2 retvec = new Vector2();
        float width;
        float height; // 툴팁이 잘릴수 있는 최대값 width, height

        // 스킬 타입 별로 잘림 방지 처리
        switch(p_type)
        {
            case E_SkillType.Active:
                width = p_posvec.x + (m_ActiveToolTip.rect.width * 0.5f);
                height = p_posvec.y + (m_ActiveToolTip.rect.height * 0.5f);

                if (width > m_CanvasScaler.referenceResolution.x)
                {
                    // 오른쪽
                    retvec = new Vector2(p_posvec.x - m_ActiveToolTip.rect.width - (p_skillvec.x * 2f), p_posvec.y);
                    return retvec;
                }
                break;
            case E_SkillType.Passive:
                width = p_posvec.x + (m_PassiveToolTip.rect.width * 0.5f);
                height = p_posvec.y + (m_PassiveToolTip.rect.height * 0.5f);
                if (width > m_CanvasScaler.referenceResolution.x) 
                {
                    if (height < m_CanvasScaler.referenceResolution.y)
                    {
                        // 오른쪽 아래
                        retvec = new Vector2(p_posvec.x - m_PassiveToolTip.rect.width - (p_skillvec.x * 2f),
                            p_posvec.y + m_PassiveToolTip.rect.height - (p_skillvec.x * 2f));
                        return retvec;
                    }

                    // 오른쪽
                    retvec = new Vector2(p_posvec.x - m_PassiveToolTip.rect.width - (p_skillvec.x * 2f), p_posvec.y);
                    return retvec;
                }
                if (height < m_CanvasScaler.referenceResolution.y)
                {
                    // 아래
                    retvec = new Vector2(p_posvec.x, p_posvec.y + m_PassiveToolTip.rect.height - (p_skillvec.x * 2f));
                    return retvec;
                }
                break;
        }

        retvec = p_posvec;
        return retvec;

    }
    public void OnPointerExit(PointerEventData p_eventdata)
    {
        ActiveToolTip.Instance.gameObject.SetActive(false);
        PassiveToolTip.Instance.gameObject.SetActive(false);
    }

    // 포인터를 누르고 뗄 때 호출(엑티브 스킬)
    public void OnPointerClick(PointerEventData p_eventdata)
    {
        int index = m_EManager.GetSkillSetIndex(this.transform); // 클릭한 스킬이 속한 스킬셋의 인덱스 번호

        // 클릭해도 무시하고 리턴
        // 패시브 스킬, 이미 배운 스킬, 남은 포인트, 스킬 요구 포인트 보다 스킬포인트를 적게 사용한 경우
        if (m_Info.skill_type == E_SkillType.Passive ||
            m_Info.skill_IS_enabled == true ||
            SkillValue.s_Remaing_Point == 0 ||
            SkillValue.s_Skill_CountArr[index] < m_Info.skill_point_requirements)
        {
            return;
        }

        m_Info.skill_IS_enabled = true; // 스킬 활성화 상태 true
        ++SkillValue.s_Skill_CountArr[index]; // index 번째 사용 포인트 +1
        --SkillValue.s_Remaing_Point; // 남은 포인트 -1

        m_EManager.m_SkillManager.UpdateUI(); // UI 업데이트
        UpdatePassiveSkill(index); // 패시브 스킬 업데이트
    }

    // 패시브 스킬은 엑티브 스킬 일정 개수 찍으면 자동으로 활성화
    void UpdatePassiveSkill(int p_index)
    {
        // 요구 포인트 범위(최소,최대)를 벗어나면 반환
        if (SkillValue.s_Skill_CountArr[p_index] < SkillValue.s_Min_RequirePoint ||
            SkillValue.s_Skill_CountArr[p_index] > SkillValue.s_Max_RequirePoint)
        {
            return;
        }

        // (스킬을 배운 스킬셋 위치의) 패시브 스킬 데이터 가져오기 
        Skill_Info[] info = new Skill_Info[SkillManager.GetSkillDataLength(p_index)]; 
        info = SkillManager.GetSkillDataArr(p_index);
        for (int i = 0; i < info.Length; i++)
        {
            if (SkillValue.s_Skill_CountArr[p_index] == info[i].skill_point_requirements)
            {
                info[i].skill_IS_enabled = true;
                break;
            }
        }

    }

    // 이미지 활성화(투명화 조절)
    void ImageActivate()
    {
        Image image = this.gameObject.GetComponent<Image>();
        Color color = new Color(255, 255, 255, 1f);

        image.color = color;
    }

    void Update()
    {
        // 스킬 활성화 하면 이미지 활성화
        if (!m_Info.skill_IS_enabled)
        {
            return;
        }

        ImageActivate();
    }

}
