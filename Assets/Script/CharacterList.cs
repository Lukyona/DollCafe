using UnityEngine;

// [System.Serializable] // 클래스나 구조체를 에디터 인스펙터에 노출
// public class TextList
// {
//     public string[] MHtext;
// }

[System.Serializable]
public class FaceList
{
    public GameObject[] face;
}

public class CharacterList : MonoBehaviour //캐릭터 메뉴 힌트 메세지
{
    public static CharacterList instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
   // public TextList[] MHTextList; //문자열 배열을 가지고 있는 배열
    public FaceList[] CharacterFaceList; //오브젝트 배열을 가지고 있는 배열
}