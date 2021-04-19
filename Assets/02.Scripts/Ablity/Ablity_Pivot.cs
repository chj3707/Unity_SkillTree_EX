using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ablity_Pivot : MonoBehaviour
{
    public GameObject m_Ablity_Window_PtypeObj = null;
    public List<GameObject> m_Ablity_WinList = new List<GameObject>();

    
    void Awake()
    {
        m_Ablity_Window_PtypeObj.SetActive(false);

        // 능력창 복사해서 리스트에 저장
        for (int i = 0; i < 3; i++)
        {
            GameObject copyobj = GameObject.Instantiate(m_Ablity_Window_PtypeObj);
            copyobj.SetActive(true);
            copyobj.name = string.Format("능력창_{0}", i + 1);
            copyobj.transform.SetParent(this.transform);

            m_Ablity_WinList.Add(copyobj);
        }

        foreach(var item in m_Ablity_WinList)
        {
            item.transform.GetChild(0).gameObject.SetActive(false); // 스킬 셋 비활성화
        }

        
    }

    void Start()
    {
                
    }


    void Update()
    {
        
    }
}
