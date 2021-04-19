using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveToolTip : MonoBehaviour
{
    [Header("스킬명")] public Text m_SkillName = null;
    [Header("능력")] public Text m_AblityType = null;
    [Header("스킬 설명")] public Text m_SkillDescription = null;
    [Header("요구 조건")] public Text m_SkillRequirePoint = null;

    private static PassiveToolTip m_Instance = null;

    public static PassiveToolTip Instance
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

        m_SkillDescription.text = string.Format("{0}", p_info.skill_description); // 스킬 설명

        m_SkillRequirePoint.text = string.Format("배우기 요구 조건[{0}] 강화 포인트 {1} 이상", ablitystr, p_info.skill_point_requirements);
    }

    void Update()
    {
        
    }
}
