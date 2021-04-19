using System;
using UnityEngine;

// 스킬 정보 클래스
[Serializable]
public class Skill_Info
{
    [Header("스킬 번호")] public int skill_index;
    [Header("능력")] public E_AblityType ablity_type;
    [Header("스킬 타입")] public E_SkillType skill_type;
    [Header("스킬명")] public string skill_name;
    [Header("마나 소모량")] public int skill_cost;
    [Header("사거리")] public float skill_range;
    [Header("쿨타임")] public float skill_delaytime;
    [Header("시전 시간")] public float skill_casttime;
    [Header("스킬 설명")] public string skill_description;
    [Header("요구 강화 포인트")] public int skill_point_requirements;
    [Header("요구 능력 레벨")] public int skill_level_requirements;
    [Header("스킬 활성화 상태")] public bool skill_IS_enabled;
    [Header("Sprite")] public Sprite skill_sprite;

    public Skill_Info() { }
}
