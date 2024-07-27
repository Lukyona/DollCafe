using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextWriter : MonoBehaviour 
{
    private static TextWriter tw;

    public class TextWriterSingle
    {
        private Text uiText; // 화면에 표시되는 텍스트 오브젝트
        private string textToWrite; // 문자열 내용
        private float timer;
        private int characterIndex;

        float speed = 0.03f;

        public TextWriterSingle(Text uiText, string textToWrite)
        {
            this.uiText = uiText;
            this.textToWrite = textToWrite;
            characterIndex = 0;
        }

        public bool Update()  //returns true on complete
        {
            timer -= Time.deltaTime;
            while (timer <= 0f)
            {
                //display next character
                timer += speed;
                characterIndex++;
                string text = textToWrite.Substring(0, characterIndex); // 0부터 characterIndex까지의 문자열 = 플레이어에게 보여질 텍스트, 기본적으로 검은색

                text += "<color=white>" + textToWrite.Substring(characterIndex) + "</color>"; 
                // 하얀색으로 characterIndex부터 문자 끝까지를 text에 더함(대사창과 같은 색으로 만들어 보이지 않게 함)
                // 왼쪽부터 차례대로 텍스트가 나오는 것처럼 보이게 하기 위해 text는 대사 전체를 담고 있지만 characterIndex를 이용해 순서대로 하나씩 보이게(검은색으로 나타나게) 만듬

                uiText.text = text;
                if (characterIndex >= textToWrite.Length) //대사 출력 완료, true 반환해서 textWriterSingle 지우기
                {
                    return true;
                }
            }
            return false;
        }

        public Text GetUIText()
        {
            return uiText;
        }

        public bool IsActive() // 아직 대사 출력 중인지
        {
            return characterIndex < textToWrite.Length;
        }

        public void WriteAllAndDestroy() // 출력 스킵, 즉시 대사 전부 출력
        {
            uiText.text = textToWrite;
            characterIndex = textToWrite.Length;
            RemoveWriter_Static(uiText); // 수동으로 지우기
        }
    }

    private TextWriterSingle textWriterSingle;

    private void Awake()
    {
        tw = this;
        textWriterSingle = null;
    }

    public static TextWriterSingle AddWriter_Static(Text uiText, string textToWrite)
    {
        return tw.AddWriter(uiText, textToWrite);
    }

    private TextWriterSingle AddWriter(Text uiText, string textToWrite)
    {
        textWriterSingle = new TextWriterSingle(uiText, textToWrite);
        return textWriterSingle;
    }

    public static void RemoveWriter_Static(Text uiText)
    {
        tw.RemoveWriter(uiText);
    }

    private void RemoveWriter(Text uiText) // 수동으로 지우기, 대사 출력 스킵했을 경우 실행
    {
        if (textWriterSingle.GetUIText() == uiText)
        {
            textWriterSingle = null;
        }
    }

    private void Update() // 대사 출력이 정상적으로 완료되면 자동으로 비움
    {
        if(textWriterSingle != null)
        {
            bool completeWriting = textWriterSingle.Update(); // 대사 출력 완료하면 true
            if (completeWriting)
            {
                textWriterSingle = null;
            }
        }
            
    }
}
