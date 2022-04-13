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
    public Transform left_bound;
    public Transform right_bound;

    public float barrier_speed = 0.001f;
    private GameLevel difficulty;

    List<Barrier> barriers = new List<Barrier>();
    float margin;
    void Start()
    {
        difficulty = GameLevel.Easy;
        Vector2 size = pivot_prefab.transform.localScale;
        margin = size.magnitude * 0.9f;

        Barrier first_barrier = new Barrier(difficulty, new Vector2(0, start_line.transform.localPosition.y), margin);
        first_barrier.create_visuals(line_prefab, pivot_prefab);
        // Barrier first_barrier = new Barrier(3, new Vector2(0, start_line.transform.localPosition.y), line_prefab, pivot_prefab, margin);
        // barriers.Add(first_barrier);
    }

    void Update()
    {
        // if (barriers.Count > 0) {
        //     create_barrier();
        //     move_barriers();
        // }
    }

    void move_barriers(){
        foreach (Barrier b in barriers) {
            Vector2 loc = b.pivot.transform.position;
            loc.y -= barrier_speed;
            b.pivot.transform.position = loc;
        }
    }

    // void create_barrier() {
    //     if (barriers[barriers.Count - 1].pivot.transform.position.y <= start_line.position.y){            
    //         Vector2 new_pivot_loc = barriers[barriers.Count - 1].get_next_pivot();
    //         Barrier new_barrier = new Barrier(3, new_pivot_loc, line_prefab, pivot_prefab, margin);
    //         Vector2 next_pivot = new_barrier.get_next_pivot();
    //         while (next_pivot.x > left_bound.localPosition.x && next_pivot.x < right_bound.localPosition.x) {
    //             new_barrier = new Barrier(3, new_pivot_loc, line_prefab, pivot_prefab, margin);
    //             next_pivot = new_barrier.get_next_pivot();
    //         }
    //         new_barrier.rand_diff_rotate(difficulty);
    //         barriers.Add(new_barrier);
    //     }
    // }

    enum GameLevel {Easy, Medium, Hard, Insane};

    class Barrier {
        public int ysize = 1;
        public GameObject line;
        public GameObject pivot;

        public Vector2 next_pt;
        public Vector2 loc;
        public float angle;
        public float line_length;
        public float margin;

        public void Barrier_v1(int size, Vector2 position, GameObject l, GameObject p, float margin) {
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

            create_edge(pivot.transform.position, next_pivot_loc);
        }





        public Barrier(GameLevel level, Vector2 position, float m) {
            loc = position;
            margin = m;
            line_length = get_line_length(level);
            angle = rand_diff_rotate(level);
            Debug.Log(angle);
            next_pt = new Vector2(0f, line_length + margin * 2);
            next_pt = Quaternion.Euler(0, 0, angle) * next_pt;
        }

        public void create_visuals(GameObject l, GameObject p) {
            pivot = Instantiate(p, Vector2.zero, Quaternion.identity);
            line = Instantiate(l, Vector2.zero, Quaternion.identity);

            Vector3 lscale = l.transform.localScale;
            lscale.y *= line_length;
            line.transform.localScale = lscale;

            Vector2 line_pos = (next_pt - loc) + loc;
            line.transform.localPosition = line_pos;
            line.transform.parent = pivot.transform;

            pivot.transform.GetChild(0).localPosition = next_pt;

        }





        public float get_line_length(GameLevel level) {
            if (level == GameLevel.Easy) {
                return 5f;
            }
            return 5f;
        }

        private void create_edge(Vector2 start, Vector2 end) {
            EdgeCollider2D edge = pivot.AddComponent(typeof(EdgeCollider2D)) as EdgeCollider2D;
            edge.points = new Vector2[] {start, end};
        }

        public void rotate(float degree) {
            pivot.transform.eulerAngles = new Vector3(0, 0, degree);
        }

        public float rand_diff_rotate(GameLevel level) {
            if (level == GameLevel.Easy) {
                return Random.Range(-9, 10) * 10;
            }
            return 0f;
        }

        public Vector2 get_next_pivot() {
            return pivot.transform.GetChild(0).position;
        }
    }
}
