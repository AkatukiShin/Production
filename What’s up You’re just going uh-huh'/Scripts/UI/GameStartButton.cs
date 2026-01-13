using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButton : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            //CriFsPlugin.FinalizeLibrary(); //FinalizeLibraryを毎フレーム呼び出すとバグるらしい(金井)
            SceneManager.LoadScene("InGame");
        }
    }
}
