using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

// 버튼 클릭 이벤트 관리 :: 능력 선택, 초기화
public class EventManager : MonoBehaviour
{
    private GameObject[] m_Ablity_SelectBtnArr = null; // 능력 선택 버튼
    public GameObject[] m_Ablity_ObjArr = null; // 능력 창
    private List<GameObject> m_ResetBtn_List = new List<GameObject>(); // 초기화 버튼
    private E_AblityType[] m_Ablity_TypeArr = null; // (능력창)능력 타입

    public SkillManager m_SkillManager = null; // 클릭 이벤트로 조절하기 위해서 가져옴

    Skill_Info[,] m_SkillSetAll; // 2차원 배열로 저장해서 DataBase로 넘길것

    public Transform m_SaveScreen = null; // 세이브,로드 화면
    public InputField m_InputField = null;
    private bool m_IsSave = false; // 세이브 버튼 클릭
    private bool m_IsLoad = false; // 로드 버튼 클릭
    

    void Start()
    {
        m_Ablity_ObjArr = GameObject.FindGameObjectsWithTag("AblityWindow"); // 능력창 태그 찾아서 배열에 저장
        m_Ablity_SelectBtnArr = GameObject.FindGameObjectsWithTag("AblityBtn"); // 능력 선택 버튼 태그 찾아서 배열에 저장
        m_Ablity_TypeArr = new E_AblityType[m_Ablity_ObjArr.Length]; // 배열 크기 할당

        for(int i=0; i<m_Ablity_SelectBtnArr.Length; i++)
        {
            // 스킬셋 별로 초기화 버튼 이름 설정
            GameObject tempobj = m_Ablity_ObjArr[i].transform.GetChild(0).gameObject; // 스킬셋
            m_ResetBtn_List.Add(tempobj.transform.GetComponentInChildren<Button>().gameObject); // 초기화 버튼 리스트에 추가
            m_ResetBtn_List[i].name = string.Format("{0} {1}", m_ResetBtn_List[i].name, i + 1); // 이름 설정
            
            // 스킬셋 별로 능력 선택 버튼 이름 설정
            for (int j = 0; j < (int)E_AblityType.Max; j++)
            {
                m_Ablity_SelectBtnArr[i].transform.GetChild(j).name = 
                    string.Format("{0} {1}", m_Ablity_SelectBtnArr[i].transform.GetChild(j).name, i + 1);
            }
            
            // 능력창 별로 능력 타입 배열로 저장
            m_Ablity_TypeArr[i] = m_Ablity_ObjArr[i].GetComponent<Ablity_Info>().m_Ablity.ablity;
        }
    }
    
    // 버튼 클릭 처리(초기화, 능력 선택)
    void BtnClickProcess(E_AblityType p_ablitytype)
    {
        bool is_reset = false;
        bool is_set = false;

        // https://openplay.tistory.com/7 EventSystem 참고
        GameObject tempobj = EventSystem.current.currentSelectedGameObject; // 현재 버튼 클릭한 게임 오브젝트
        // Debug.Log(tempobj.name); // 클릭한 오브젝트 이름 확인용

        // 초기화
        for (int i = 0; i < m_Ablity_ObjArr.Length; i++)
        {
            // 현재 클릭한 버튼오브젝트와 같은 이름의 오브젝트가 있는지 확인
            if (m_ResetBtn_List[i].name == tempobj.name)
            {
                GameObject skillset = m_Ablity_ObjArr[i].transform.GetChild(0).gameObject; // 선택한 능력창의 스킬셋
                
                // 전투, 지속 스킬창 위치 보내서 스킬 초기화
                for (int j = 0; j < (int)E_SkillType.Max; j++)
                {
                    m_SkillManager.Destroy_Skill_List(skillset.transform.Find("스킬창").GetChild(j), i);
                }

                skillset.SetActive(false); // 스킬셋 비활성화
                m_Ablity_SelectBtnArr[i].SetActive(true); // 버튼 활성화
                m_Ablity_TypeArr[i] = E_AblityType.None; // 해당 능력창 타입 초기화

                is_reset = true;
                break;
            }
        }
        
        // 초기화 버튼일시 아래 부분 처리하지 않고 넘어감
        if (is_reset)
        {
            return;
        }

        bool is_exist = false;
        int count = 0;
        // 능력 선택 
        for (int i = 0; i < m_Ablity_ObjArr.Length; i++)
        {
            for (int j = 0; j < (int)E_AblityType.Max; j++)
            {
                if (m_Ablity_SelectBtnArr[i].transform.GetChild(j).name == tempobj.name) // 버튼 클릭한 게임 오브젝트 스킬셋만 활성화 하기 위함
                {
                    for(int k=0; k< m_Ablity_TypeArr.Length; k++) // 이미 있는 능력인지 확인
                    {
                        if (m_Ablity_TypeArr[k] == E_AblityType.None) // 능력창 선택한게 있는지 확인
                        {
                            ++count;
                            if (count == m_Ablity_TypeArr.Length)
                            {
                                break; // 능력창 3개다 선택하지 않은 경우
                            }
                                
                            continue;
                        }

                        if(m_Ablity_TypeArr[k] == p_ablitytype) // 선택한 능력창이 있는 경우
                        {
                            is_exist = true;
                            DataBase.Instance.DestroyListData(); // 이미 있는 능력이므로 로드한 데이터 삭제
                            Debug.Log("이미 있는 능력 입니다.");
                            break;
                        }                   
                    } // k for문
                    if (is_exist) // 능력이 이미 있으면 함수 종료
                        break;

                    GameObject obj = m_Ablity_ObjArr[i].transform.GetChild(0).gameObject; // 선택한 능력창의 스킬셋
                    obj.SetActive(true); // 스킬셋 활성화
                    m_Ablity_SelectBtnArr[i].SetActive(false); // 버튼 비활성화

                    // 전투, 지속 스킬창 위치 보내서 스킬 세팅
                    for (int k = 0; k < (int)E_SkillType.Max; k++) 
                    {
                        m_SkillManager.Set_Skill(obj.transform.Find("스킬창").GetChild(k), i, null);
                    }

                    // 능력창 능력 타입 설정
                    m_Ablity_ObjArr[i].GetComponent<Ablity_Info>().m_Ablity.SetAblity(m_SkillManager.GetAblityType(i));
                    m_Ablity_TypeArr[i] = m_Ablity_ObjArr[i].GetComponent<Ablity_Info>().m_Ablity.ablity;

                    is_set = true;
                    break;
                }
            }
            if (is_set || is_exist)
                break;
        }
    }

    // 스킬 셋 인덱스 번호를 리턴해주는 함수(스킬 클릭 이벤트때 호출함)
    public int GetSkillSetIndex(Transform p_trans)
    {
        int retval = 0;
        bool flag = false;
        Transform[] transarr = p_trans.GetComponentsInParent<Transform>(); // 클릭한 스킬의 상위 컴포넌트 저장

        for(int i=0; i<m_Ablity_ObjArr.Length; i++)
        {
            for (int j = 0; j < transarr.Length; j++)
            {
                if (m_Ablity_ObjArr[i].name == transarr[j].name) // 클릭한 스킬 상위 컴포넌트에서 같은 이름이 있다는건 해당 인덱스에 속한다는 것
                {
                    retval = i;
                    flag = true;
                    break; // j for문
                }
            }
            if (flag)
                break;
        }
        
        return retval;
    }

    // 세이브 버튼 클릭
    public void _ON_SaveBtnClick()
    {
        int count = 0;
        bool is_null = false;
        int skill_amount = SkillValue.s_ActiveSkillCount + SkillValue.s_PassiveSkillCount;

        m_SkillSetAll = new Skill_Info[m_Ablity_ObjArr.Length, skill_amount];

        for (int i = 0; i < m_Ablity_ObjArr.Length; i++)
        {
            if (m_Ablity_ObjArr[i].GetComponentInChildren<Skill>() == null) // 활성화 된 스킬창인지 확인
            {
                ++count;
                Debug.LogFormat("{0}번째 능력창 정보 없음", i + 1);
                if (count == m_Ablity_ObjArr.Length)
                {
                    Debug.Log("아무 정보 없음");
                    is_null = true;
                    break;
                }
                continue;
            }
            Skill[] temparr = new Skill[skill_amount];
            temparr = m_Ablity_ObjArr[i].GetComponentsInChildren<Skill>(); // 배열에 정보 할당
            // 정보가 있을 때만 실행 됨
            for (int j = 0; j < skill_amount; j++)
            {
                m_SkillSetAll[i,j] = temparr[j].m_Info; // 배열에 정보 할당
            }
        }
        // 정보 없으면 세이브 처리 하지 않음
        if (is_null)
        {
            Debug.Log("정보가 없어 저장하지 않습니다.");
            return;
        }
        m_IsSave = true;
        m_SaveScreen.gameObject.SetActive(true);
        m_InputField.placeholder.GetComponent<Text>().text = "파일 이름 입력후 Enter키를 누르면 저장됩니다.";
    }

    // 로드 버튼 클릭
    public void _ON_LoadBtnClick()
    {
        m_IsLoad = true;
        m_SaveScreen.gameObject.SetActive(true);
        m_InputField.placeholder.GetComponent<Text>().text = "읽어드릴 파일 이름을 입력 후 Enter키를 눌러주세요.";
    }

    void Update()
    {
        // 저장 화면이 활성화 되었을때 키 입력 받아 저장
        if (m_SaveScreen.gameObject.activeSelf)
        {
            if (m_IsSave)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    string path = Application.streamingAssetsPath + "/SaveData/" + m_InputField.text + ".json";
                    if (File.Exists(path)) // 이미 있는 이름의 파일인지 확인
                    {
                        Debug.Log("같은 이름의 파일이 존재합니다.");
                        m_InputField.text = "";
                        return;
                    }
                    DataBase.Instance.JsonSaveData(m_SkillSetAll, m_InputField.text); // 데이터 베이스로 보내서 세이브 처리
                    m_SaveScreen.gameObject.SetActive(false);
                    m_IsSave = false;
                    m_InputField.text = "";
                }
            }
            if (m_IsLoad)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    
                    string path = Application.streamingAssetsPath + "/SaveData/" + m_InputField.text + ".json";
                    if (File.Exists(path)) // 파일이 있는지 확인
                    {
                        int skill_amount = SkillValue.s_ActiveSkillCount + SkillValue.s_PassiveSkillCount; // 스킬 개수
                        m_SkillSetAll = new Skill_Info[m_Ablity_ObjArr.Length, skill_amount]; // 메모리 할당 [능력창 개수, 스킬 총 개수]

                        m_SkillSetAll = DataBase.Instance.JsonLoadData(m_SkillSetAll, m_InputField.text); // 데이터 베이스로 보내서 데이터 로드
                        ReLoadWindow(); // 스킬창 재구성 (로드한 스킬트리로 화면 세팅 해줌)
                        m_SaveScreen.gameObject.SetActive(false);
                        m_IsLoad = false;
                        m_InputField.text = "";
                    }
                    m_InputField.text = "";
                }
            }
        }

    }

    // 저장해둔 스킬셋을 로드하여 화면을 재구성
    public void ReLoadWindow()
    {
        // 로드 전에 열려있던 창들 정리
        for (int i = 0; i < m_Ablity_ObjArr.Length; i++) // 능력창 개수
        {
            GameObject skillset = m_Ablity_ObjArr[i].transform.GetChild(0).gameObject; // 각 능력창의 스킬창
            for (int j = 0; j < (int)E_SkillType.Max; j++)
            {
                m_SkillManager.Destroy_Skill_List(skillset.transform.Find("스킬창").GetChild(j), i); // 스킬 매니저로 보내서 삭제
            }
            skillset.SetActive(false); // 스킬창 비활성화
            m_Ablity_SelectBtnArr[i].SetActive(true); // 버튼 활성화
            m_Ablity_TypeArr[i] = E_AblityType.None; // 해당 능력창 타입 초기화
        }

        // 가져온 로드 파일 세팅
        for (int i = 0; i < m_SkillSetAll.GetLength(0); i++)
        {
            if (m_SkillSetAll[i, 0] == null)  // 능력 선택 하지 않은 능력창
            {
                continue;
            }
            int count = 0; // 활성화 스킬 개수
            Skill_Info[] tempinfo = new Skill_Info[m_SkillSetAll.GetLength(1)]; // 2차원 배열에 있는값 1차원 배열로 정리
            for (int j = 0; j < m_SkillSetAll.GetLength(1); j++)
            {
                tempinfo[j] = m_SkillSetAll[i, j];

                if (tempinfo[j].skill_type == E_SkillType.Active &&
                    tempinfo[j].skill_IS_enabled) // 저장된 스킬이 활성화 상태면 카운트 +1
                {
                    count++;
                }
            }
            

            GameObject skillset = m_Ablity_ObjArr[i].transform.GetChild(0).gameObject; // 각 능력창의 스킬창
            for (int k = 0; k < (int)E_SkillType.Max; k++)
            {
                m_SkillManager.Set_Skill(skillset.transform.Find("스킬창").GetChild(k), i, tempinfo); // 1차원 배열 스킬매니저로 보내서 세팅
            }

            SkillValue.s_Skill_CountArr[i] = count; // 사용 포인트 설정
            SkillValue.s_Remaing_Point -= SkillValue.s_Skill_CountArr[i]; // 남은 포인트에서 제거
            m_SkillManager.UpdateUI(); // UI 업데이트

            m_Ablity_ObjArr[i].transform.GetChild(0).gameObject.SetActive(true); // 스킬창 활성화
            m_Ablity_SelectBtnArr[i].SetActive(false); // 버튼 비활성화
        }

    }

    // 격투 버튼 클릭
    public void _ON_BattlerageBtnClick()
    {
        DataBase.Instance.CSVDataLoad("CSV/Battlerage_Info"); // CSV 파일 데이터 로드
        BtnClickProcess(E_AblityType.Battlerage);
    }

    // 야성 버튼 클릭
    public void _ON_AcheryBtnClick()
    {
        DataBase.Instance.CSVDataLoad("CSV/Achery_Info"); // CSV 파일 데이터 로드
        BtnClickProcess(E_AblityType.Achery);
    }

    // 마법 버튼 클릭
    public void _ON_SorceryBtnClick()
    {
        DataBase.Instance.CSVDataLoad("CSV/Sorcery_Info"); // CSV 파일 데이터 로드
        BtnClickProcess(E_AblityType.Sorcery);
    }

    // 낭만 버튼 클릭
    public void _ON_SongcraftBtnClick()
    {
        DataBase.Instance.CSVDataLoad("CSV/Songcraft_Info"); // CSV 파일 데이터 로드
        BtnClickProcess(E_AblityType.Songcraft);
    }

    // 사명 버튼 클릭
    public void _ON_ShadowpalyBtnClick()
    {
        DataBase.Instance.CSVDataLoad("CSV/Shadowpaly_Info"); // CSV 파일 데이터 로드
        BtnClickProcess(E_AblityType.Shadowpaly);
    }

    // 의지 버튼 클릭
    public void _ON_AuramancyBtnClick()
    {
        DataBase.Instance.CSVDataLoad("CSV/Auramancy_Info"); // CSV 파일 데이터 로드
        BtnClickProcess(E_AblityType.Auramancy);
    }

    // 초기화 버튼 클릭
    public void _ON_ResetBtnClick()
    {
        BtnClickProcess(E_AblityType.None);
    }

    // 세이브 로드 화면에서 나가기
    public void _ON_ExitBtnClick()
    {
        m_SaveScreen.gameObject.SetActive(false);
    }

    // 종료
    public void _ON_CloseAppBtnClick()
    {
        Application.Quit();
    }
}
