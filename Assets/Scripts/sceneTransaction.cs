

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


class SceneTransaction:Singleton<SceneTransaction>
{
    public Image cover;
    
    private new void Awake() {
        base.Awake();
    }
    public static void Trans()
    {
        instance.StartCoroutine(((SceneTransaction)instance).ShowCover());
    }
    public IEnumerator ShowCover(float animationTime=0.5f)
    {
        this.cover.color = new Color(this.cover.color.r, this.cover.color.g, this.cover.color.b, 0f);
        cover.gameObject.SetActive(true);
        float alphaPerSecond = 1f / animationTime;
        float alphaPerFrame = alphaPerSecond * Time.deltaTime;

        float passedTime = 0f;
        while(passedTime < animationTime)
        {
            this.cover.color = new Color(this.cover.color.r, this.cover.color.g, this.cover.color.b, this.cover.color.a + alphaPerFrame);
            passedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        this.cover.color = new Color(this.cover.color.r, this.cover.color.g, this.cover.color.b, 1f);
        this.SolveSceneUI((gameObject)=>Destroy(gameObject));
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        while(!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        passedTime = 0;
        while(passedTime < animationTime)
        {
            this.cover.color = new Color(this.cover.color.r, this.cover.color.g, this.cover.color.b, this.cover.color.a - alphaPerFrame);
            passedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        this.cover.color = new Color(this.cover.color.r, this.cover.color.g, this.cover.color.b, 0f);
    }

    public void SolveSceneUI(Action<GameObject> func)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("SceneUI");
        foreach(GameObject obj in objs)
        {
            func(obj);
        }
    }
}