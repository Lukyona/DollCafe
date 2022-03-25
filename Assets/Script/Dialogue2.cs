using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue2 : MonoBehaviour
{

    // Update is called once per frame
    void Update() //화면의 아무 곳이나 터치하면 다음 대사 나타남
    {
        if (UI_Assistant1.instance.talking && UI_Assistant1.instance.stop == 0 && Input.GetMouseButtonDown(0))
        {
            UI_Assistant1.instance.OpenDialogue2();
        }
        
    }
}
