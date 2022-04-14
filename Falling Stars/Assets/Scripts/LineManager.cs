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

    private GameLevel difficulty;
    List<Barrier> barriers = new List<Barrier>();
    float margin;
    float line_size;

    void Start()
    {
        difficulty = GameLevel.Easy;

        Vector2 size = pivot_prefab.transform.localScale;
        margin = size.magnitude * 0.9f;
        line_size = line_prefab.transform.localScale.y;

        Barrier new_barrier = new Barrier(new Vector2(0, start_line.localPosition.y), difficulty, line_size, margin, 360);
        new_barrier.visualize(line_prefab, pivot_prefab);
        barriers.Add(new_barrier);
    }   

    void Update()
    {
        if (barriers.Count > 0) {
            foreach (Barrier b in barriers) {
                b.move(barrier_speed);
            }
            if (barriers[barriers.Count - 1].pivot.localPosition.y < start_line.localPosition.y) {
                new_barrier();
            }
        }
    }

    void new_barrier(){
        Barrier prev_barrier = barriers[barriers.Count-1];
        Barrier new_barrier = new Barrier(prev_barrier.next_pt, difficulty, line_size, margin, prev_barrier.angle);
        new_barrier.visualize(line_prefab, pivot_prefab);
        barriers.Add(new_barrier);
    }

    enum GameLevel {Easy, Medium, Hard, Insane};

    class Barrier {
        public Vector2 pivot_pt;
        public Vector2 line_pt;
        public Vector2 next_pt;
        public float angle;
        public float line_scale;

        public Transform line;
        public Transform pivot;

        public Barrier(Vector2 position, GameLevel level, float line_size, float margin, float prev_angle) {
            pivot_pt = position;

            do {
                angle = rand_diff_rotate(level);
                line_scale = get_line_length(level);

                next_pt = new Vector2(0, line_scale * line_size + margin * 2);
                next_pt = Quaternion.Euler(0, 0, angle) * next_pt;
                next_pt += pivot_pt;
            } while (!meets_contraints(prev_angle, angle, next_pt)); 

            line_pt = new Vector2(0, (line_scale * line_size)/2 + margin);
            line_pt = Quaternion.Euler(0, 0, angle) * line_pt;
            line_pt += pivot_pt;
        }

        public bool meets_contraints(float prev_angle, float curr_angle, Vector2 test_pt) {
            if (next_pt.x < -2.6 || next_pt.x > 2.6) {
                return false;
            }

            if (curr_angle >= 0 && prev_angle == curr_angle - 180) {
                return false;
            } else if (curr_angle < 0 && prev_angle == curr_angle + 180) {
                return false;
            }
            return true;
        }



        public void visualize(GameObject l, GameObject p) {
            line = Instantiate(l, line_pt, Quaternion.Euler(0, 0, angle)).transform;
            Vector2 line_local_scale = line.transform.localScale;
            line_local_scale.y *= line_scale;
            line.transform.localScale = line_local_scale;

            pivot = Instantiate(p, pivot_pt, Quaternion.identity).transform;
        }

        public float get_line_length(GameLevel level) {
            if (level == GameLevel.Easy) {
                return Random.Range(7, 13);
            }
            return 5f;
        }

        public float rand_diff_rotate(GameLevel level) {
            if (level == GameLevel.Easy) {
                return Random.Range(-9, 10) * 10;
            }
            return 0f;
        }

        public void move(float speed) {
            Vector2 line_pos = line.localPosition;
            line_pos.y -= speed;
            line.localPosition = line_pos;

            Vector2 pivot_pos = pivot.localPosition;
            pivot_pos.y -= speed;
            pivot.localPosition = pivot_pos;

            next_pt.y -= speed;
        }
    }
}
