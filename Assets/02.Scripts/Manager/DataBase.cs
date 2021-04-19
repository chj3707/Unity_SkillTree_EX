using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class StrObjDic<T1, T2> : SerializeDictionary<string, object> { } // 직렬화 딕셔너리 클래스 상속 받은 제네릭 클래스(처음에 CSV 로드 확인용으로 만듬)

// Json 사용을 위한 직렬화 배열 클래스
[Serializable]
public class SerializeArray
{
    public Skill_Info[] Data;


    public SerializeArray(Skill_Info[] p_data)
    {
        Data = p_data;
    }
}

// 직렬화한 정보 저장,로드 할때 사용할 클래스
[Serializable]
public class JsonData
{
    public List<SerializeArray> J_Data;

    public JsonData(List<SerializeArray> p_data)
    {
        J_Data = p_data;
    }
}

// CSV파일을 로드하여 스킬 정보를 저장하고, 유저가 만든 스킬트리 세이브,로드
public class DataBase : MonoBehaviour
{
    [SerializeField]
    public List<StrObjDic<string, object>> m_Data = new List<StrObjDic<string, object>>(); // 딕셔너리 형식 데이터

    private static DataBase m_Instance = null;

    [SerializeField]
    private List<Skill_Info> m_Skill_List = new List<Skill_Info>(); // 스킬 저장할 리스트
    
    private Sprite[] m_SpriteArr; // 이미지 저장할 배열

    private List<SerializeArray> m_SaveDataList = new List<SerializeArray>();
    private JsonData m_LoadDataList;

    // 데이터 베이스에 접근할 수 있는 프로퍼티
    public static DataBase Instance
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
        if(m_Instance == null)
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
        
    }

    void Update()
    {
        
    }

    public void JsonSaveData(Skill_Info[,] p_infoarr, string p_string)
    {
        int count = 0;
        
        while(true)
        {
            // 직렬화 배열 클래스 동적 할당 (2차원 배열값 1차원 배열에 저장하고 리스트에 넣을것)
            SerializeArray tempinfo = new SerializeArray(new Skill_Info[p_infoarr.GetLength(1)]);

            for (int i=0; i<p_infoarr.GetLength(1); i++)
            {
                if (p_infoarr[count, i] == null) // 능력 선택 하지않은 경우 예외 처리
                {
                    m_SaveDataList.Add(new SerializeArray(new Skill_Info[p_infoarr.GetLength(1)])); // 메모리 할당만 함(빈 정보)
                    break;
                }
                
                tempinfo.Data[i] = p_infoarr[count, i]; // 받은 2차원 배열값 1차원 배열에 저장
                
                if (i + 1 == p_infoarr.GetLength(1)) // 2차원 배열의 count 번째 배열값을 tempinfo에 다 넣으면
                {
                    m_SaveDataList.Add(tempinfo); // tempinfo값 리스트에 저장
                }
            }
            ++count;

            if (count == p_infoarr.GetLength(0)) // 2차원 배열 값을 다 넣으면 종료
            {
                break;
            }
        }

        JsonData OrganizedData = new JsonData(m_SaveDataList);

        //https://docs.unity3d.com/kr/530/Manual/StreamingAssets.html 스트리밍 에셋
        // p_string.json으로 저장
        File.WriteAllText(Application.streamingAssetsPath + "/SaveData/" + p_string + ".json", JsonUtility.ToJson(OrganizedData));

        Debug.LogFormat("{0}.json 저장!!", p_string);
        
        m_SaveDataList.Clear(); // 리스트 초기화
    }

    public Skill_Info[,] JsonLoadData(Skill_Info[,] p_infoarr, string p_string)
    {
        string tempstr = File.ReadAllText(Application.streamingAssetsPath + "/SaveData/" + p_string + ".json");

        m_LoadDataList = JsonUtility.FromJson<JsonData>(tempstr); // 해당 파일 데이터 가져오기

        Skill_Info[,] ret_arr = new Skill_Info[p_infoarr.GetLength(0), p_infoarr.GetLength(1)]; // 반환 해줄 데이터 
        for (int i = 0; i < p_infoarr.GetLength(0); i++)
        {
            for (int j = 0; j < p_infoarr.GetLength(1); j++)
            {
                if (m_LoadDataList.J_Data[i].Data[j].skill_sprite == null) // 헤더? 를 읽어 들여 null값이 아닌 값이 들어옴 그래서 스프라이트를 기준으로 빈창인지 체크함
                {
                    break;
                }
                ret_arr[i, j] = m_LoadDataList.J_Data[i].Data[j]; // 로드한 정보 할당
                // Debug.Log(m_LoadDataList.J_Data[i].Data[j].skill_name);
            }
        }

        Debug.LogFormat("{0}.json 로드!!", p_string);

        return ret_arr;
    }



    // CSV 데이터 로드 (매개 변수로 파일 이름을 받아 데이터 베이스에 로드)
    public void CSVDataLoad(string p_file)
    {
        m_Data = CSVReader.Read(p_file);

        for (int i = 0; i < m_Data.Count; i++)
        {
            Skill_Info info = new Skill_Info();

            // 데이터 파싱해서 할당
            info.skill_index = int.Parse(m_Data[i]["스킬 번호"].ToString());
            info.ablity_type = (E_AblityType)Enum.Parse(typeof(E_AblityType), m_Data[i]["능력"].ToString());
            info.skill_type = (E_SkillType)Enum.Parse(typeof(E_SkillType), m_Data[i]["스킬 타입"].ToString());
            info.skill_name = m_Data[i]["스킬명"].ToString();
            info.skill_cost = int.Parse(m_Data[i]["마나 소모량"].ToString());
            info.skill_range = float.Parse(m_Data[i]["사거리"].ToString());
            info.skill_delaytime = float.Parse(m_Data[i]["쿨타임"].ToString());
            info.skill_casttime = float.Parse(m_Data[i]["시전 시간"].ToString());
            info.skill_description = m_Data[i]["스킬 설명"].ToString();
            info.skill_point_requirements = int.Parse(m_Data[i]["요구 강화 포인트"].ToString());
            info.skill_level_requirements = int.Parse(m_Data[i]["요구 능력 레벨"].ToString());
            info.skill_IS_enabled = bool.Parse(m_Data[i]["스킬 활성화 상태"].ToString());

  
            string path = String.Format("Sprite/{0}", info.ablity_type); // 이미지 경로(능력 별로 나눠둠)
            m_SpriteArr = Resources.LoadAll<Sprite>(path); // 이미지 로드해서 배열에 저장
            info.skill_sprite = m_SpriteArr[i];
            
            m_Skill_List.Add(info); // 리스트에 저장

            //Debug.LogFormat(
            //    "스킬 번호 : {0}" +
            //    "능력 : {1}\n" +
            //    "스킬 타입 : {2}\n" +
            //    "스킬명 : {3}\n" +
            //    "마나 소모량 : {4}\n" +
            //    "사거리 : {5}\n" +
            //    "쿨타임 : {6}\n" +
            //    "시전 시간 : {7}\n" +
            //    "스킬 설명 : {8}\n" +
            //    "요구 강화 포인트 : {9}\n" +
            //    "요구 능력 레벨 : {10}\n" +
            //    "스킬 활성화 상태 : {11}\n",
            //    m_Data[i]["스킬 번호"],
            //    m_Data[i]["능력"],
            //    m_Data[i]["스킬 타입"],
            //    m_Data[i]["스킬명"],
            //    m_Data[i]["마나 소모량"],
            //    m_Data[i]["사거리"],
            //    m_Data[i]["쿨타임"],
            //    m_Data[i]["시전 시간"],
            //    m_Data[i]["스킬 설명"],
            //    m_Data[i]["요구 강화 포인트"],
            //    m_Data[i]["요구 능력 레벨"],
            //    m_Data[i]["스킬 활성화 상태"]);
        }

    }

    // 엑티브, 패시브 스킬 리스트 리턴해주는 함수
    public List<Skill_Info> GetSkillInfo(E_SkillType p_type)
    {
        List<Skill_Info> skill_list = new List<Skill_Info>();
        if(m_Skill_List == null)
        {
            Debug.Log("저장된 스킬 정보가 없습니다.");
            return null;
        }
        foreach(var item in m_Skill_List)
        {
            if (item.skill_type == p_type)
            {
                skill_list.Add(item); // 매개변수로 받은 스킬 타입과 같은 스킬 리스트에 저장해서 리턴
            }
        }
        return skill_list;
    }

    // 스킬 리스트 데이터 삭제
    public void DestroyListData()
    {
        m_Skill_List.Clear();
    }

    // 엑티브, 패시브 스킬 개수 리턴해주는 함수
    public int GetSkillCount(E_SkillType p_type)
    {
        if (m_Skill_List == null)
        {
            Debug.Log("저장된 스킬 정보가 없습니다.");
            return 0;
        }

        int count = 0;

        foreach(var item in m_Skill_List)
        {
            if(item.skill_type == p_type)
            {
                ++count; // 매개변수로 받은 스킬 타입에 따라 개수 세서 리턴
            }
        }

        return count;
    }
}
