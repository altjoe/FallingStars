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

    Information info;

    void Start()
    {
       info = new Information(line_prefab.transform.localScale.x, pivot_prefab.transform.localScale.x * 0.9f, start_line.localPosition.y, finish_line.localPosition.y);

       create_barrier();
    }   

    void Update()
    {
        if (info.all_barriers.Count > 0){
            info.move();
            if (info.next_pt.y < start_line.localPosition.y) {
                create_barrier();
            }
        }
    }

    public enum GameLevel {Easy, Medium, Hard, Insane};

    public void create_barrier(){
        Barrier new_barrier = new Barrier(info);
        new_barrier.Visualize(pivot_prefab, line_prefab);
        info.add_barrier(new_barrier);
    }

    public class Barrier {
        public GameObject pivot_gameobject;
        public GameObject line_gameobject;
        public Transform pivot_trans;
        public Transform line_trans;
        public Vector2 pivot_pt;
        public Vector2 next_pt;
        public Vector2 line_pt;

        public AngleLength angle_linescaler;

        public Barrier(Information info) {
            this.pivot_pt = info.next_pt;
            this.next_pt = get_next_pt(info, info.get_angle_lengths());
            this.line_pt = this.pivot_pt + (this.next_pt - this.pivot_pt)/2f;

            info.next_pt = this.next_pt;
        }

        public void Visualize(GameObject pivot_prefab, GameObject line_prefab) {
            this.pivot_gameobject = Instantiate(pivot_prefab, pivot_pt, Quaternion.identity);
            this.line_gameobject = Instantiate(line_prefab, line_pt, Quaternion.identity);

            Vector2 current_line_scale = this.line_gameobject.transform.localScale;
            current_line_scale.y *= this.angle_linescaler.length;
            this.line_gameobject.transform.localScale = current_line_scale;

            this.line_gameobject.transform.rotation = Quaternion.Euler(0,0, this.angle_linescaler.angle);

            this.pivot_trans = pivot_gameobject.transform;
            this.line_trans = line_gameobject.transform;
        }

        public void move(float speed){
            Vector2 p_loc = pivot_trans.localPosition;
            p_loc.y -= speed;
            pivot_trans.localPosition = p_loc;

            Vector2 l_loc = line_trans.localPosition;
            l_loc.y -= speed;
            line_trans.localPosition = l_loc;

            next_pt.y -= speed;
            pivot_pt.y -= speed;
        }

        public void DestroyObjects(){
            DestroyImmediate(pivot_gameobject);
            DestroyImmediate(line_gameobject);
        }

        private Vector2 get_next_pt(Information info, List<AngleLength> angle_lengths){
            int angle_len_index = Random.Range(0, angle_lengths.Count);
            this.angle_linescaler = angle_lengths[angle_len_index];
            // this.angle_linescaler = test_angle_length;

            Vector2 test_pt = new Vector2(0, info.get_next_y_dist(this.angle_linescaler.length));
            test_pt = Quaternion.Euler(0, 0, this.angle_linescaler.angle) * test_pt;
            test_pt += pivot_pt;

            if (!passes_contraints(test_pt, info)){
                angle_lengths.RemoveAt(angle_len_index);
                test_pt = get_next_pt(info, angle_lengths);
            }

            return test_pt;
        }

        private bool passes_contraints(Vector2 test_pt, Information info){
            if (Mathf.Abs(test_pt.x) > 2.5){
                return false;
            } else if (this.angle_linescaler.angle - 180 == info.latest_angle || this.angle_linescaler.angle + 180 == info.latest_angle){
                return false;
            } else if (Mathf.Abs(test_pt.x) > 2 && Mathf.Abs(this.angle_linescaler.angle) > 90) {
                return false;
            } else if (info.level != GameLevel.Easy) {
                if (info.latest_angle > 0){
                    float opposite_latest = info.latest_angle - 180;
                    if (this.angle_linescaler.angle <= opposite_latest && this.angle_linescaler.angle >= -180) {
                        return false;
                    }
                } else if (info.latest_angle < 0) {
                    float opposite_latest = info.latest_angle + 180;
                    if (this.angle_linescaler.angle >= opposite_latest && this.angle_linescaler.angle <= 180) {
                        return false;
                    }
                } 
            } 

            foreach (Barrier b in info.all_barriers) {
                if (intersect(b.pivot_pt, b.next_pt, this.pivot_pt - (test_pt - this.pivot_pt) * 0.001f, test_pt)) {
                    return false;
                } else if (perp_dist(test_pt, b.pivot_pt, b.next_pt) < 0.3){
                    return false;
                } else if (perp_dist(b.pivot_pt, this.pivot_pt, test_pt) < 0.3){
                    return false;
                }
            }

            return true;
        }

         public bool ccw(Vector2 A, Vector2 B, Vector2 C){
            return (C.y-A.y) * (B.x-A.x) > (B.y-A.y) * (C.x-A.x);
        }

        public bool intersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D) {
            return ccw(A,C,D) != ccw(B,C,D) && ccw(A,B,C) != ccw(A,B,D);
        }

        public float perp_dist(Vector2 test_pt, Vector2 pivot_pt, Vector2 next_pt) {
            // Vector2 test_pt, Vector2 pivot_pt, Vector2 next_pt
            float A = test_pt.x - pivot_pt.x;
            float B = test_pt.y - pivot_pt.y;
            float C = next_pt.x - pivot_pt.x;
            float D = next_pt.y - pivot_pt.y;

            float dot = A * C + B * D;
            float len_sq = C * C + D * D;
            float param = -1f;

            if (len_sq != 0) //in case of 0 length line
                param = dot / len_sq;

            float xx;
            float yy;

            if (param < 0) {
                xx = pivot_pt.x;
                yy = pivot_pt.y;
            }
            else if (param > 1) {
                xx = next_pt.x;
                yy = next_pt.y;
            }
            else {
                xx = pivot_pt.x + param * C;
                yy = pivot_pt.y + param * D;
            }

            float dx = test_pt.x - xx;
            float dy = test_pt.y - yy;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }
    }

   

    public class Information {
        public List<Barrier> all_barriers = new List<Barrier>();
        public List<Transform> all_transforms = new List<Transform>();
        private List<AngleLength> easy = new List<AngleLength>();
        public List<AngleLength> medium = new List<AngleLength>();
        public List<AngleLength> hard = new List<AngleLength>();

        public float line_prefab_scale;
        public float latest_angle;
        public float margin;
        public Vector2 next_pt;
        public GameLevel level;
        public float barrier_speed = 0.02f;
        public float end_y;
        public List<AngleLength> temp;

        public Information(float line_prefab_scale, float margin, float start_y, float end_y){
            next_pt = new Vector2(0, start_y);
            this.level = GameLevel.Hard;
            this.end_y = end_y;
            create_angle_length_options();
            this.line_prefab_scale = line_prefab_scale;
            this.margin = margin;
        }

        public float get_next_y_dist(float line_length) {
            return (line_length * this.line_prefab_scale) + (this.margin * 2);
        }

        public List<AngleLength> get_angle_lengths(){
            if (this.level == GameLevel.Easy) {
                print("Easy Mode");
                return new List<AngleLength>(this.easy);
            } else if (this.level == GameLevel.Medium){
                print("Medium Mode");
                return new List<AngleLength>(this.medium);
            } else {
                print("Hard Mode");
                return new List<AngleLength>(this.hard);
            }
            // return new List<AngleLength>(this.easy);
        }

        

        public void create_angle_length_options(){
            for (int i = -90; i <= 90; i += 5){
                for (int j = 7; j <= 15; j++) {
                    this.easy.Add(new AngleLength(i, j));
                }
            }
            for (int i = -130; i <= 130; i += 5){
                for (int j = 7; j <= 12; j++) {
                    this.medium.Add(new AngleLength(i, j));
                }
            }
            for (int i = -180; i <= 180; i += 5){
                for (int j = 5; j <= 10; j++) {
                    this.hard.Add(new AngleLength(i, j));
                }
            }
        }

        public void add_barrier(Barrier b){
            this.all_barriers.Add(b);
            this.all_transforms.Add(b.pivot_gameobject.transform);
            this.all_transforms.Add(b.line_gameobject.transform);
            this.next_pt = b.next_pt;
            this.latest_angle = b.angle_linescaler.angle;
        }

        public void move(){
            foreach (Barrier b in all_barriers) {
                b.move(barrier_speed);
            }

            next_pt.y -= barrier_speed;
        }

        public bool game_over(float y_pos) {
            if (y_pos < end_y){
                return true;
            }
            return false;
        }
    }

    public class AngleLength {
        public float angle;
        public float length;

        public AngleLength(float angle, float length){
            this.angle = angle;
            this.length = length;
        }
    }
}
