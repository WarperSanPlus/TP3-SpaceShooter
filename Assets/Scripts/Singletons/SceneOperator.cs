using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Singletons
{
    public class SceneOperator : Singleton<SceneOperator>
    {
        public static void LoadSceneAsync(string sceneName) 
            => Instance.StartCoroutine(Instance.LoadScene(sceneName));

        // https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.html
        private IEnumerator LoadScene(string sceneName)
        {
            //Begin to load the Scene you specify
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

            //Don't let the Scene activate until you allow it to
            asyncOperation.allowSceneActivation = false;

            //When the load is still in progress, output the Text and progress bar
            while (!asyncOperation.isDone)
            {
                // Check if the load has finished
                if (asyncOperation.progress < 0.9f)
                    continue;

                asyncOperation.allowSceneActivation = true;

                yield return null;
            }
        }
    }

    public class Scenes
    {
        public const string HIGHSCORE = "HighScore";
    }
}
