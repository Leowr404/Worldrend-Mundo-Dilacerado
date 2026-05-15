using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadingManager : MonoBehaviour
{
    public static string sceneToLoad;

    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text percentText; // ou TMP_Text se usar TextMesh Pro

    void Start()
    {
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneToLoad);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            progressBar.value = progress;
            percentText.text = (int)(progress * 100) + "%";

            if (op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
