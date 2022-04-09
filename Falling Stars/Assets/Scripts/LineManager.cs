using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{   
    // Start is called before the first frame update
    public GameObject star;
    public GameObject line;

    List<GameObject> stars = new List<GameObject>();
    List<GameObject> empties = new List<GameObject>();
    List<GameObject> lines = new List<GameObject>();

    void Start()
    {
        GameObject pivot = Instantiate(star, Vector3.zero, Quaternion.identity);
        Vector2 size = pivot.transform.localScale;
        float margin = size.magnitude * 0.9f;
        Barrier barrier = new Barrier(3, line, pivot, margin);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    class Barrier {
        public int ysize = 1;
        GameObject l;
        
        public Barrier(int size, GameObject line, GameObject parent, float margin) {
            l = Instantiate(line, Vector3.zero, Quaternion.identity);
            Vector3 lscale = l.transform.localScale;
            lscale.y *= size;
            l.transform.localScale = lscale;
            Vector3 loc = l.transform.localPosition;
            loc.y += (lscale.y / 2) + margin;
            l.transform.localPosition = loc;
            l.transform.parent = parent.transform;
        }
    }
}
