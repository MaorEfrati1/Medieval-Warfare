using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeclareWin : MonoBehaviour
{
    [SerializeField] Text winText;

    // a function that is used to expose the "win title" when the game is over and than return to the main menu
    public IEnumerator OnTeamWin(string winningTeam)
    {
        winText.text = winningTeam + " WON!";
        winText.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        winText.gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
