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
        Barrier barrier = new Barrier(3, line);
        
    }

    class Barrier {
        public int ysize = 1;
        GameObject l;
        double margin = 0.5;
        
        public Barrier(int size, GameObject line) {
            l = Instantiate(line, new Vector3(0, 0, 0), Quaternion.identity);
            Vector3 lscale = l.transform.localScale;
            lscale.y *= size;
            l.transform.localScale = lscale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
