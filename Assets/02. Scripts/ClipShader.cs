using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BoxClipMaterialSetter : MonoBehaviour
{
    [SerializeField]
    private Material originMaterial;

    private GameObject targetBoxObject;

    public static GameObject FindInSceneRecursive(string targetName)
    {
        var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var rootObj in rootObjects)
        {
            var result = FindRecursive(rootObj.transform, targetName);
            if (result != null)
                return result;
        }
        return null;
    }

    private static GameObject FindRecursive(Transform parent, string targetName)
    {
        if (parent.name == targetName)
            return parent.gameObject;

        foreach (Transform child in parent)
        {
            var found = FindRecursive(child, targetName);
            if (found != null)
                return found;
        }
        return null;
    }

    void Start()
    {
        UpdateTargetBox();
    }

    void UpdateTargetBox()
    {
        if (transform.parent.parent.name == "BlockListContent")
        {
            targetBoxObject = FindInSceneRecursive("BlockListPanel");
        }
        else if (transform.parent.parent.name == "BlockOrderContent")
        {
            targetBoxObject = FindInSceneRecursive("BlockOrderPanel");
        }
        else if (transform.parent.parent.name == "BlockPanel")
        {
            targetBoxObject = FindInSceneRecursive("BlockPanel");
        }
        else if (transform.parent.parent.name == "BlgMsgContent")
        {
            targetBoxObject = FindInSceneRecursive("BlgMsg");
        }
        else if (transform.parent.parent.name == "ForExpContent")
        {
            targetBoxObject = FindInSceneRecursive("ForExpPanel");
        }
        else if (transform.parent.parent.name == "MvMsgContent")
        {
            targetBoxObject = FindInSceneRecursive("MvMsg");
        }
    }

    void Update()
    {
        var mat = GetComponent<Renderer>().material;

        BoxCollider box = targetBoxObject.GetComponent<BoxCollider>();
        
        if (box == null) return;

        mat.SetTexture("_MainTex", originMaterial.GetTexture("_BaseMap"));
        mat.SetColor("_Color", originMaterial.GetColor("_BaseColor"));
        mat.SetInt("_Always", 0);

        if(transform.parent.parent.name == "BlockPanel")
        {
            mat.SetInt("_Always", 1);
        }
        else if (transform.parent.parent.name == "BlgMsgContent" || transform.parent.parent.name == "MvMsgContent") // for문 상세 내역
        {
            mat.SetInt("_Always", 1);
            transform.parent.rotation = targetBoxObject.transform.rotation;
        }

        mat.SetMatrix("_TargetWorldToLocal", targetBoxObject.transform.worldToLocalMatrix);
        mat.SetVector("_TargetBoxHalfSize", new Vector4(0.5F, 0.5F, 0.5F, 0.0F));
        mat.SetVector("_TargetBoxCenter", new Vector4(box.center.x, box.center.y, box.center.z, 1.0F));
    }
}