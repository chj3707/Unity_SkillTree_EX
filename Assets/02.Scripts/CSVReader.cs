using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions; // Regex 정규표현식
using UnityEngine;

// CSV 파일 파싱
// c# @기호 https://spaghetti-code.tistory.com/17
// CSV 파일 읽기 https://mentum.tistory.com/214
public class CSVReader
{
    // regex 정규표현식 정리 https://hamait.tistory.com/342
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))"; 
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r"; // 운영체재별 줄바꿈(enter키)
    static char[] TRIM_CHARS = { '\"' };


    // 인스펙터 창에 노출하기 위해 Dictionary를 StrObjDic 제네릭 클래스로 바꿈
    //List<StrObjDic<string, object>> 변수 = CSVReader.Read("파일 이름");
    public static List<StrObjDic<string, object>> Read(string file)
    {
        var list = new List<StrObjDic<string, object>>(); 
        TextAsset data = Resources.Load(file) as TextAsset; // 형변환 

        // 줄바꿈 별로 분할 해서 문자열 배열 형식으로 변수에 할당
        var lines = Regex.Split(data.text, LINE_SPLIT_RE); // public static string[] Split(string input, string pattern); 리턴형

        if (lines.Length <= 1) return list; // csv 데이터를 분할한 문자열 배열이 Header(설명)만 존재

        var header = Regex.Split(lines[0], SPLIT_RE); // Header(설명)라인 분할
        
        for (var i = 1; i < lines.Length; i++) // 읽어온 파일의 Header를 제외한 라인 개수 만큼 반복
        {

            var values = Regex.Split(lines[i], SPLIT_RE); // "" 문자열 안의 , 무시
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new StrObjDic<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", ""); // " 문자 제거 , 문자열 치환 \\ 문자 제거

                value = value.Replace("<br>", "\n"); // 추가된 부분. 개행문자를 \n대신 <br>로 사용한다.
                value = value.Replace("<c>", ",");

                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n)) // int 형으로 파싱
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f)) // float 형으로 파싱
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue; // 파싱한 데이터 할당
            }
            list.Add(entry); // 리스트에 추가 후 리스트 리턴
        }
        return list;
    }
}
