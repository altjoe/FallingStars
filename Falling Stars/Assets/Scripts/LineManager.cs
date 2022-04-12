using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{   
    // Start is called before the first frame update
    public GameObject pivot_prefab;
    public GameObject line_prefab;
    public Transform start_line;
    public Transform finish_line;
    public float barrier_speed = 0.001f;

    List<Barrier> barriers = new List<Barrier>();
    float margin;
    void Start()
    {
        Vector2 size = pivot_prefab.transform.localScale;
        margin = size.magnitude * 0.9f;
        Barrier first_barrier = new Barrier(3, new Vector2(0, 6), line_prefab, pivot_prefab, margin);
        barriers.Add(first_barrier);
    }

    void Update()
    {
        if (barriers.Count > 0) {
            create_barrier();
            move_barriers();
        }
    }

    void move_barriers(){
        foreach (Barrier b in barriers) {
            Vector2 loc = b.pivot.transform.position;
            loc.y -= barrier_speed;
            b.pivot.transform.position = loc;
        }
    }

    void create_barrier() {
        if (barriers[barriers.Count - 1].pivot.transform.position.y <= start_line.position.y){            
            Vector2 new_pivot_loc = barriers[barriers.Count - 1].get_next_pivot();
            Barrier new_barrier = new Barrier(3, new_pivot_loc, line_prefab, pivot_prefab, margin);
            barriers.Add(new_barrier);
        }
    }

    class Barrier {
        public int ysize = 1;
        public GameObject line;
        public GameObject pivot;
        
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
