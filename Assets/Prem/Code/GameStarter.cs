using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    public string Scene = "Main";
    public void StartGame()
    {
        // ��Ŵ Scene ���� (����¹ "GameScene" �繪��� Scene �ͧ�س)
        SceneManager.LoadScene(Scene);
    }

    public void QuitGame()
    {
        // �Դ��
        Application.Quit();

        // ����Ѻ��÷��ͺ� Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}