using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveToolTip : MonoBehaviour
{
    [Header("스킬명")]public Text m_SkillName = null;
    [Header("능력")] public Text m_AblityType = null;
    [Header("활력")] public Text m_SkillCost = null;
    [Header("사거리")] public Text m_SkillRange = null;
    [Header("시전 시간")] public Text m_SkillCastT = null;
    [Header("쿨타임")] public Text m_SkillDelayT = null;
    [Header("스킬 설명")] public Text m_SkillDescription = null;
    [Header("요구 조건")] public Text m_SkillRequirePoint = null;

    private static ActiveToolTip m_Instance = null;

    public static ActiveToolTip Instance
    {
        get
        {
            if (m_Instance == null)
            {
                Debug.Log("Instance is null");
                return null;
            }

            return m_Instance;
        }
    }

    void Awake()
    {
        // 싱글톤
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else
        {
            // 해당 씬에 이미 인스턴스가 존재하면 해당 인스턴스 삭제
            Destroy(gameObject);
        }

        // 씬 전환 파괴 방지
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void UpdateUI(Skill_Info p_info)
    {
        
        string ablitystr = ReturnTypeString.GetAblityString(p_info.ablity_type); // 타입 보내서 해당 능력 가져오기

        m_SkillName.text = string.Format("{0}", p_info.skill_name); // 이름
        m_AblityType.text = ablitystr; // 능력
        m_SkillCost.text = string.Format("활력 {0}", p_info.skill_cost); // 스킬 소모량

        // 시전 시간
        if(p_info.skill_casttime == 0f)
            m_SkillCastT.text = "즉시 시전";
        else
            m_SkillCastT.text = string.Format("시전 시간 {0}초", p_info.skill_casttime);


        // 사거리
        if (p_info.skill_range == 0f)
            m_SkillRange.text = "유효거리(자신)";
        else
            m_SkillRange.text = string.Format("유효거리 0~{0}m", p_info.skill_range);

        m_SkillDelayT.text = string.Format("{0}초 후 재사용 가능", p_info.skill_delaytime); // 쿨타임
        m_SkillRequirePoint.text = string.Format("배우기 요구 조건[{0}] 능력 {1}레벨", ablitystr, p_info.skill_level_requirements); // 조건
        m_SkillDescription.text = string.Format("{0}", p_info.skill_description); // 스킬 설명
    }

    void Update()
    {
        
    }
}
