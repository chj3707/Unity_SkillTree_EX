using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MyAblity
{
    public E_AblityType ablity;

    // 생성자
    public MyAblity()
    {
        ablity = E_AblityType.None;
    }

    // 세팅 함수
    public void SetAblity(E_AblityType p_ablity)
    {
        ablity = p_ablity;
    }
}

// 능력창 별로 능력 타입 설정
public class Ablity_Info : MonoBehaviour
{
    public MyAblity m_Ablity;


    void Awake()
    {
        m_Ablity = new MyAblity(); // 초기화
        
    }
    void Start()
    {
        
    }

    
    void Update()
    {

    }
}
