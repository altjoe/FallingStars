using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{   
    // Start is called before the first frame update
    public GameObject pivot_prefab;
    public GameObject line_prefab;

    List<Transform> pivots = new List<Transform>();

    void Start()
    {
        Vector2 size = pivot_prefab.transform.localScale;
        float margin = size.magnitude * 0.9f;
        Barrier barrier = new Barrier(3, new Vector2(0, 2), line_prefab, pivot_prefab, margin);
        barrier.rotate(90);

        Vector2 next_pt = barrier.get_next_pivot();
        Barrier next_barrier = new Barrier(3, next_pt, line_prefab, pivot_prefab, margin);
        next_barrier.rotate(45);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    class Barrier {
        public int ysize = 1;
        GameObject line;
        GameObject pivot;
        
        public Barrier(int size, Vector2 position, GameObject l, GameObject p, float margin) {
            pivot = Instantiate(p, Vector2.zero, Quaternion.identity);
            line = Instantiate(l, Vector2.zero, Quaternion.identity);

            Vector3 lscale = l.transform.localScale;
            lscale.y *= size;
            line.transform.localScale = lscale;
            Vector3 loc = l.transform.localPosition;
            loc.y += (lscale.y / 2) + margin;
            line.transform.localPosition = loc;
            line.transform.parent = pivot.transform;

            Transform nextPivotPt = pivot.transform.GetChild(0);
            Vector3 next_pivot_loc = nextPivotPt.localPosition;
            next_pivot_loc.y += loc.y * 2 + size;
            pivot.transform.GetChild(0).localPosition = next_pivot_loc;

            pivot.transform.localPosition = position;
        }

        public void rotate(float degree) {
            pivot.transform.eulerAngles = new Vector3(0, 0, degree);
        }

        public Vector2 get_next_pivot() {
            return pivot.transform.GetChild(0).position;
        }
    }
}
