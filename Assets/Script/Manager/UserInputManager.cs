using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputManager : MonoBehaviour
{
    public static UserInputManager instance;

    bool canTouch = true;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCanTouch(bool value)
    {
        canTouch = value;
    }

    public bool CanTouch()
    {
        return canTouch;
    }

    public void SetCanTouchTrue() // Invoke용
    {
        canTouch = true;
    }
}
