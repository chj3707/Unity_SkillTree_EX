// 능력 타입
public enum E_AblityType
{
    None = -1,
    Battlerage, // 격투
    Achery, // 야성
    Sorcery, // 마법
    Shadowpaly, // 사명
    Songcraft, // 낭만
    Auramancy, // 의지

    Max
}

// 스킬 타입
public enum E_SkillType
{ 
    Active = 0, // 액티브 스킬
    Passive, // 패시브 스킬

    Max
}

public static class ReturnTypeString
{
    public static string GetAblityString(E_AblityType p_type)
    {
        string retstr = "";

        switch(p_type)
        {
            case E_AblityType.Battlerage:
                retstr = "격투";
                break;
            case E_AblityType.Achery:
                retstr = "야성";
                break;
            case E_AblityType.Sorcery:
                retstr = "마법";
                break;
            case E_AblityType.Songcraft:
                retstr = "낭만";
                break;
            case E_AblityType.Shadowpaly:
                retstr = "사명";
                break;
            case E_AblityType.Auramancy:
                retstr = "의지";
                break;
        }

        return retstr;
    }
}