using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitlabDoor : MonoBehaviour
{
    public string targetScene = "ProfessorlabScene";
    public Vector2 spawnPositionInTarget = new Vector2(0, 0);
    public string playerTag = "Player";

    private bool loading = false; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (loading) return;
        if (!string.IsNullOrEmpty(playerTag) && !other.CompareTag(playerTag)) return;

        loading = true;

        TransitionData.hasSpawn = true;
        TransitionData.nextSpawnPos = spawnPositionInTarget;

        StartCoroutine(LoadNext());
    }
    private System.Collections.IEnumerator LoadNext()
    {
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }

}
