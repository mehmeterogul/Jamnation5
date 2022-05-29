using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] Animator transition;
    bool coroutineActive = false;

    public void StartGame()
    {
        if (coroutineActive) return;
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        FadeOut();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Level01");
    }

    public void LoadNextLevel()
    {
        if (coroutineActive) return;
        StartCoroutine(LoadNextLevelCoroutine());
    }

    IEnumerator LoadNextLevelCoroutine()
    {
        yield return new WaitForSeconds(1f);
        FadeOut();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadCurrentLevel()
    {
        if (coroutineActive) return;
        StartCoroutine(LoadCurrentLevelCoroutine());
    }

    IEnumerator LoadCurrentLevelCoroutine()
    {
        yield return new WaitForSeconds(1f);
        FadeOut();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void FadeOut()
    {
        coroutineActive = true;
        transition.SetTrigger("fadeOut");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
