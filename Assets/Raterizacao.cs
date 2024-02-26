using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class Raterizacao : MonoBehaviour
{
    Ray ray;
    Texture2D tex;
    public float cameraSize;
    public Transform light1;
    [Range(0, 30)]
    public float specular;
    [Range(0, 1)]
    public float ambinet;
    public Color iluminacaoFundo;


    // Start is called before the first frame update
    void Start()
    {
        ray= new Ray(transform.position,Vector3.forward);
        Renderer rend = GetComponent<Renderer>();
        tex = new Texture2D(300, 300);
        tex.filterMode = FilterMode.Trilinear;
        rend.material.mainTexture = tex;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StartCoroutine("RenderScene");
        }
        Debug.DrawRay(ray.origin,ray.direction * 5f,Color.red);
    }
    IEnumerator RenderScene()
    {
        for(int y = 0; y< tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                float px = ((float)x / tex.width) * cameraSize - cameraSize * 0.5f;
                float py = ((float)y / tex.height) * cameraSize - cameraSize * 0.5f;
                ray.origin = new Vector3(px, py, 0);
                if(Physics.Raycast(ray,out RaycastHit hit))
                {
                    Color c = BlinnPhong(hit);
                    tex.SetPixel(x,y,c);
                }
                else
                {
                    tex.SetPixel(x, y, iluminacaoFundo);
                }
                tex.Apply();
            }
            yield return new WaitForSeconds(0.00001f);


        }
    }
    Color BlinnPhong(RaycastHit hit)
    {
        Color hitColor = hit.transform.GetComponent<MeshRenderer>().material.color;
        Vector3 L = (light1.position - hit.point).normalized;
        Vector3 N = hit.normal;
        Vector3 V = (transform.position - hit.point).normalized;
        Vector3 H = (L + V).normalized;
        float NdotH= Mathf.Max(0,Vector3.Dot(N,H));
        float diff= Mathf.Max(0,Vector3.Dot(N,L));
        float spec = Mathf.Pow(NdotH,(specular * 2+1));
        return ambinet * hitColor + diff * hitColor + spec * Color.white;
    }
}
