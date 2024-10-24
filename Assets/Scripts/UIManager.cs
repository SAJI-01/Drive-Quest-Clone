using UnityEngine;

public class UIManager : MonoBehaviour
{
    private void Awake()
    {
        #if !UNITY_EDITOR
            Application.targetFrameRate = 60;
        #endif
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}