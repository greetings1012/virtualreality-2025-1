using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOutScript : MonoBehaviour
{
    [SerializeField]
    private Material fadeMaterial;

    private float fadeTime = 4.0F;
    private float currTime = 0.0F;

    private bool flag = false;
    private Color destColor = Color.black;

    public void AnimateFadeOut()
    {
        currTime = 0.0F;
        flag = true;
    }

    public void DestroyAFOCommand()
    {
        currTime = 0.0F;
        flag = false;
    }

    private void Update()
    {
        if (flag)
        {
            currTime += Time.deltaTime;

            if (currTime >= fadeTime)
            {
                flag = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!flag)
        {
            Graphics.Blit(src, dest);
            return;
        }

        fadeMaterial.SetColor("destColor", destColor);
        fadeMaterial.SetFloat("lerpT", currTime / fadeTime);

        if (fadeMaterial != null)
        {
            Graphics.Blit(src, dest, fadeMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
