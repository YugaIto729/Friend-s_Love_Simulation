using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneButton : MonoBehaviour
{
    private IEnumerator MoveNewGame()
    {
        var image = FadeManager.instance.Get_FadeImage();

        for (int i = 0; i <= 100; i++)
        {
            //Debug.Log("i: "+i/(second*100));
            image.color = Color.Lerp(Color.clear, Color.black, (float)i / 100);
            yield return new WaitForSeconds(0.01f);
        }

        GameManager.instance.GameStartSetting();
        //GameManager.instance.GameStarting();
        GameManager.instance.gameState = GamaState.NEWGAME;

        SceneManager.UnloadSceneAsync("Title_Scene");
    }

    public void StartNewGame()
    {
        StartCoroutine(MoveNewGame());
        
    }

    public void StartLoadGame()
    {

    }
}
