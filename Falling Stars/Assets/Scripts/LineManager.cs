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

        Barrier new_barrier = new Barrier(new Vector2(0, start_line.localPosition.y), difficulty, line_size, margin, 360, barriers);
        new_barrier.visualize(line_prefab, pivot_prefab);
        barriers.Add(new_barrier);
        
    }   

    void Update()
    {
        if (barriers.Count > 0) {
            bool remove = false;
            for (int i = 0; i < barriers.Count; i++) {
                barriers[i].move(barrier_speed);
                if (barriers[0].pivot.transform.localPosition.y <= finish_line.transform.localPosition.y) {
                    barriers[i].destroy();
                    remove = true;
                    break;
                }
            }
            if (remove) {barriers.RemoveAt(0);}

            if (barriers[barriers.Count - 1].pivot.localPosition.y < start_line.localPosition.y) {
                new_barrier();
            }
        }
    }

    void new_barrier(){
        Barrier prev_barrier = barriers[barriers.Count-1];
        Barrier new_barrier = new Barrier(prev_barrier.next_pt, difficulty, line_size, margin, prev_barrier.angle, barriers);
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

        public GameObject line_go;
        public GameObject pivot_go;
        public EdgeCollider2D edge;

        public Transform line;
        public Transform pivot;

        public Barrier(Vector2 position, GameLevel level, float line_size, float margin, float prev_angle, List<Barrier> barriers) {
            pivot_pt = position;
            GameLevel l = level;

            // if (Mathf.Abs(position.x) > 2 || Mathf.Abs(prev_angle) > 90) {
            //     l = GameLevel.Easy;
            // }

            // int count = 0;
            // do {
            //     angle = rand_diff_rotate(l);
            //     line_scale = get_line_length(l);

            //     next_pt = new Vector2(0, line_scale * line_size + margin * 2);
            //     next_pt = Quaternion.Euler(0, 0, angle) * next_pt;
            //     next_pt += pivot_pt;

            //     count += 1;
            //     if (count > 100) {
            //         Debug.Log("Taking too long");
            //         break;
            //     }
            // } while (!meets_contraints(prev_angle, angle, next_pt) || count > 100); 

            next_pt = find_next_pt(get_angles(l), get_line_lengths(l), line_size, margin, barriers, prev_angle);


            line_pt = new Vector2(0, (line_scale * line_size)/2 + margin);
            line_pt = Quaternion.Euler(0, 0, angle) * line_pt;
            line_pt += pivot_pt;
        }

        public bool meets_contraints(float prev_angle, float curr_angle, Vector2 test_pt) {
            if (Mathf.Abs(next_pt.x) > 2.6) {
                return false;
            } else if (curr_angle >= 0 && prev_angle == curr_angle - 180) {
                return false;
            } else if (curr_angle < 0 && prev_angle == curr_angle + 180) {
                return false;
            }

            return true;
        }

        public Vector2 find_next_pt(List<int> angles, List<int> lengths, float line_size, float margin, List<Barrier> barriers, float prev_angle) {
            int angle_index = Random.Range(0, angles.Count);
            int length_index = Random.Range(0, lengths.Count);
            angle = angles[angle_index];
            line_scale = lengths[length_index];

            Vector2 test_pt = new Vector2(0, line_scale * line_size + margin * 2);
            test_pt = Quaternion.Euler(0, 0, angle) * test_pt;
            test_pt += pivot_pt;

            foreach (Barrier b in barriers) {
                if (!meets_contraints(prev_angle, angle, test_pt)){
                    angles.RemoveAt(angle_index);
                    test_pt = find_next_pt(angles, lengths, line_size, margin, barriers, prev_angle);
                    break;
                }
            }
            
            return test_pt;
        }


        public bool ccw(Vector2 A, Vector2 B, Vector2 C){
            return (C.y-A.y) * (B.x-A.x) > (B.y-A.y) * (C.x-A.x);
        }

        public bool intersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D) {
            return ccw(A,C,D) != ccw(B,C,D) && ccw(A,B,C) != ccw(A,B,D);
        }

        public void visualize(GameObject l, GameObject p) {
            line_go = Instantiate(l, line_pt, Quaternion.Euler(0, 0, angle));
            line = line_go.transform;
            Vector2 line_local_scale = line.transform.localScale;
            line_local_scale.y *= line_scale;
            line.transform.localScale = line_local_scale;

            pivot_go = Instantiate(p, pivot_pt, Quaternion.identity);
            pivot = pivot_go.transform;
            
            edge = pivot_go.GetComponent<EdgeCollider2D>();
            edge.points = new Vector2[] {Vector2.zero, (next_pt - pivot_pt) * (1f / pivot.localScale.x)};
        }

        public List<int> get_line_lengths(GameLevel level) {
            if (level == GameLevel.Easy) {
                return new List<int>() {9, 10, 11, 12, 13};
            } else if (level == GameLevel.Medium) {
                return new List<int>() {6, 7, 8, 9, 10, 11, 12, 13};
            }
            return new List<int>();
        }

        public List<int> get_angles(GameLevel level) {
            if (level == GameLevel.Easy) {
                return new List<int>() {-90, -80, -70, -60, -50, -40, -30, -20, -10, 0, 10, 20, 30, 40, 50, 60, 70, 80, 90};
            } else if (level == GameLevel.Medium) {
                return new List<int>() {-180, -170, -160, -150, -140, -130, -120, -110, -100, -90, -80, -70, -60, -50, -40, -30, -20, -10, 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180};
            }

            return new List<int>() {-90, -80, -70, -60, -50, -40, -30, -20, -10, 0, 10, 20, 30, 40, 50, 60, 70, 80, 90};
        }

        public void destroy() {
            Destroy(pivot_go);
            Destroy(line_go);
        }

        public void move(float speed) {
            Vector2 line_pos = line.localPosition;
            line_pos.y -= speed;
            line.localPosition = line_pos;

            Vector2 pivot_pos = pivot.localPosition;
            pivot_pos.y -= speed;
            pivot.localPosition = pivot_pos;

            next_pt.y -= speed;
            pivot_pt.y -= speed;
        }
    }
}
