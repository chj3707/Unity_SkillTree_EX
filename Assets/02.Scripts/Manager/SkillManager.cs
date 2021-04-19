using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 전체적으로 사용할 변수
[SerializeField]
public static class SkillValue
{
    public static int s_Remaing_Point = 20; // 남은 스킬 포인트
    public static int[] s_Skill_CountArr = new int[3]; // 현재 배운 스킬 개수

    public static int s_ActiveSkillCount = 12; // 능력당 엑티브 스킬 개수
    public static int s_PassiveSkillCount = 6; // 능력당 패시브 스킬 개수

    public static int s_Min_RequirePoint = 3; // 스킬 최소 요구 포인트
    public static int s_Max_RequirePoint = 8; // 스킬 최대 요구 포인트
}

public class SkillManager : MonoBehaviour
{
    
    private List<Skill_Info> m_Skill_List = new List<Skill_Info>(); // 스킬 리스트
    private List<GameObject> m_CopyBtnList = new List<GameObject>(); // 스킬 버튼 복사용 

    private static Skill_Info[,] m_SkillDataArr = null; // 스킬셋 별로 패시브 스킬 정보 저장해둘 2차원 배열

    public GameObject m_SkillObj = null; // 버튼

    public Text m_Point = null; // 남은 포인트
    public Text[] m_CountArr = null; // 사용 포인트 배열

    private int m_Count = 0;
    private bool m_ISdata = false;

    void Awake()
    {
        m_SkillObj.SetActive(false);
        UpdateUI();
    }

    void Start()
    {
        
    }

    // 오브젝트 복사해서 정보 세팅
    public void Set_Skill(Transform p_clicktrans, int p_index, Skill_Info[] p_info)
    {
        // 저장된 데이터를 새로 로드 경우
        if (p_info != null) 
        {
            // 태그에 따라 데이터 정리
            if (p_clicktrans.CompareTag("Active"))
            {
                foreach(var item in p_info)
                {
                    if (item.skill_type == E_SkillType.Active)
                        m_Skill_List.Add(item);
                }
            }
            if (p_clicktrans.CompareTag("Passive"))
            {
                foreach (var item in p_info)
                {
                    if (item.skill_type == E_SkillType.Passive)
                        m_Skill_List.Add(item);
                }
            }

            // 오브젝트 복사
            for (int i = 0; i < m_Skill_List.Count; i++)
            {
                GameObject copyobj = GameObject.Instantiate(m_SkillObj) as GameObject;
                copyobj.SetActive(true);
                copyobj.name = string.Format("Skill_{0}", i);
                copyobj.transform.SetParent(p_clicktrans);
                copyobj.GetComponent<Skill>().m_Info = m_Skill_List[i];

                m_CopyBtnList.Add(copyobj); // 리스트에 추가
            }
            Set_Skill_Image(m_CopyBtnList, p_index); // 리스트 보내서 이미지 세팅
            m_Skill_List.Clear();

            return; // 밑에 코드 실행 안함
        }

        // 새로 설정 하는 경우
        // 태그에 따라 데이터 베이스에서 스킬 리스트 가져옴
        if (p_clicktrans.CompareTag("Active"))
        {
            m_Skill_List = DataBase.Instance.GetSkillInfo(E_SkillType.Active);
        }
        if (p_clicktrans.CompareTag("Passive"))
        {
            m_Skill_List = DataBase.Instance.GetSkillInfo(E_SkillType.Passive);
        }

        // 하위에 복사된 인스턴스들에 접근해 스킬 정보 할당
        for (int i = 0; i < m_Skill_List.Count; i++)
        {
            GameObject copyobj = GameObject.Instantiate(m_SkillObj) as GameObject;
            copyobj.SetActive(true);
            copyobj.name = string.Format("Skill_{0}", i);
            copyobj.transform.SetParent(p_clicktrans);
            copyobj.GetComponent<Skill>().m_Info = m_Skill_List[i];
            
            m_CopyBtnList.Add(copyobj); // 리스트에 추가

        }
        
        Set_Skill_Image(m_CopyBtnList, p_index); // 리스트 보내서 이미지 세팅
        m_Skill_List.Clear();

    }

    // 정보가 저장되어 있는 오브젝트 리스트를 받아 이미지 세팅
    void Set_Skill_Image(List<GameObject> p_skillobjlist, int p_index)
    {
        Image image;
        Text text;
        Color enabled_true = new Color(255, 255, 255, 1f); 
        Color enabled_false = new Color(255, 255, 255, 0.5f); // 이미지 색상 반투명
        Skill skill;

        foreach (var item in p_skillobjlist)
        {
            image = item.GetComponent<Image>(); // 스킬 오브젝트의 이미지에 접근
            text = item.GetComponentInChildren<Text>(); // 스킬 오브젝트의 텍스트에 접근
            skill = item.GetComponent<Skill>(); // 스킬 오브젝트의 스킬 클래스에 접근

            image.sprite = skill.m_Info.skill_sprite; // 저장 되어있는 스프라이트로 이미지 설정
            
            // 색상 설정 (활성화 상태에 따라)
            if (skill.m_Info.skill_IS_enabled)
                image.color = enabled_true;
            else
                image.color = enabled_false;

            // 텍스트 설정
            if (skill.m_Info.skill_point_requirements == 0)
                text.text = ""; // 요구 포인트가 0이면 아무것도 넣지 않음
            else
                text.text = string.Format("{0}", skill.m_Info.skill_point_requirements); // 요구하는 스킬 포인트
        }

        // 엑티브, 패시브 두번 받아야 되서 설정해둠
        ++m_Count;
        m_ISdata = m_Count == 2 ? true : false;
        if (m_ISdata) 
        {
            m_Count = 0;
            m_ISdata = false;

            // 동적 할당
            if (m_SkillDataArr == null)
            {
                GameObject[] objarr = GameObject.FindGameObjectsWithTag("AblityWindow");
                m_SkillDataArr = new Skill_Info[objarr.Length, SkillValue.s_PassiveSkillCount]; // [스킬셋 개수,패시브 스킬 개수]
            }

            int j = 0;
            // 배열에 데이터 저장
            for (int i=0; i<m_CopyBtnList.Count; i++)
            {
                if(m_CopyBtnList[i].GetComponent<Skill>().m_Info.skill_type == E_SkillType.Passive)
                {
                    m_SkillDataArr[p_index, j] = m_CopyBtnList[i].GetComponent<Skill>().m_Info; // 패시브 스킬 찾아서 배열에 저장
                    ++j;
                }               
            }

            m_CopyBtnList.Clear(); // 버튼 리스트 클리어
            DataBase.Instance.DestroyListData(); // 데이터 베이스에 저장 되어 있는 스킬 리스트 데이터 삭제
        }
    }

    // 초기화 버튼 눌린 스킬셋 초기화, 변수값 조정
    public void Destroy_Skill_List(Transform p_clicktrans, int p_index)
    {
        if (p_clicktrans.childCount == 0)
        {
            return;
        }
        int count = 0;

        count = p_clicktrans.childCount; // 하위 오브젝트 개수

        SkillValue.s_Remaing_Point += SkillValue.s_Skill_CountArr[p_index]; // 사용 포인트만큼 남은 포인트 더하기
        SkillValue.s_Skill_CountArr[p_index] = 0; // 사용 포인트 초기화
        UpdateUI(); // UI 업데이트

        for (int i = 0; i < count; i++) 
        {
            GameObject.Destroy(p_clicktrans.GetChild(i).gameObject); // 스킬 오브젝트 삭제
        }

        for (int i = 0; i < m_SkillDataArr.GetLength(1); i++)
        {
            m_SkillDataArr[p_index, i] = null; // 해당 스킬셋 스킬정보 배열 초기화
        }

        m_CopyBtnList.Clear(); // 리스트 초기화
        
    }


    // 스킬 데이터 배열의 index번째 스킬 타입 반환
    public E_AblityType GetAblityType(int p_index)
    {
        return m_SkillDataArr[p_index, 0].ablity_type; 
    }

    // 스킬 데이터 배열의 index번째 스킬 배열 반환
    public static Skill_Info[] GetSkillDataArr(int p_index)
    {
        Skill_Info[] ret_info = new Skill_Info[m_SkillDataArr.GetLength(1)];

        for(int i=0; i<ret_info.Length; i++)
        {
            ret_info[i] = m_SkillDataArr[p_index, i];
        }

        return ret_info;
    }

    // 스킬 데이터 배열의 index번째 스킬 개수 반환
    public static int GetSkillDataLength(int p_index)
    {
        return m_SkillDataArr.GetLength(1);
    }

    public void UpdateUI()
    {
        m_Point.text = string.Format("기술 포인트 : {0}", SkillValue.s_Remaing_Point);

        for (int i = 0; i < m_CountArr.Length; i++)
        {
            m_CountArr[i].text = string.Format("사용 포인트 : {0}", SkillValue.s_Skill_CountArr[i]);
        }
    }

    void Update()
    {
        
    }
}
