using UnityEngine;


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
    public FaceList[] CharacterFaceList; //오브젝트 배열을 가지고 있는 배열
}