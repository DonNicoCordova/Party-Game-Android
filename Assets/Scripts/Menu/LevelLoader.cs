using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;
    public Animator transition;
    public float transitionTime = 1f;
    public Sprite[] transitionImages;
    public Image transitionImageController;
    public bool visible;
    public void Start()
    {
        if (Instance != null)
        {
            return;
        } else
        {
            Instance = this;
            visible = false;
        }
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator LoadLevel(string sceneName)
    {
        int index = Random.Range(0, transitionImages.Length);
        transitionImageController.sprite = transitionImages[index];
        transition.ResetTrigger("FadeIn");
        transition.SetTrigger("FadeOut");
        visible = true;
        yield return new WaitForSeconds(transitionTime);
        PhotonNetwork.LoadLevel(sceneName);
    }
    public void FadeIn()
    {
        visible = false;
        transition.ResetTrigger("FadeOut");
        transition.SetTrigger("FadeIn");
    }
}
