using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    InputManager inputManager;
    void Start()
    {
        inputManager = FindAnyObjectByType<InputManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
