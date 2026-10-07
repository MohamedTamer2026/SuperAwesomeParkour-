using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSequence : MonoBehaviour
{
    [Header("UI")]
    public Animator fadeAnimator;
    public GameObject endText;

    [Header("Player")]
    public MonoBehaviour playerController;

    bool finished = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish") && !finished)
        {
            finished = true;
            StartCoroutine(EndRoutine());
        }
    }

    System.Collections.IEnumerator EndRoutine()
    {
        yield return new WaitForSeconds(1f);

        if (playerController != null)
            playerController.enabled = false;

        if (fadeAnimator != null)
            fadeAnimator.SetTrigger("Fade");

        yield return new WaitForSeconds(2f);

        if (endText != null)
            endText.SetActive(true);

        yield return new WaitForSeconds(4f);

        SceneManager.LoadScene(0);   // loads scene index 0
    }
}