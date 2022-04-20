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
            } else if (info.level != GameLevel.Easy) {
                if (test_pt.x > 0 && this.angle_linescaler.angle > 90){
                    return false;
                } else if (test_pt.x < 0 && this.angle_linescaler.angle < -90){
                    return false;
                }
            }
            return true;
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
            this.level = GameLevel.Medium;
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
            }
            return new List<AngleLength>(this.easy);
        }

        

        public void create_angle_length_options(){
            for (int i = -90; i <= 90; i += 10){
                for (int j = 7; j <= 15; j++) {
                    this.easy.Add(new AngleLength(i, j));
                }
            }
            for (int i = -130; i <= 130; i += 10){
                for (int j = 7; j <= 12; j++) {
                    this.medium.Add(new AngleLength(i, j));
                }
            }
            for (int i = -180; i <= 180; i += 10){
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
            for (int i = all_transforms.Count - 1; i >= 0; i--) {
                Vector2 b_loc = all_transforms[i].localPosition;
                b_loc.y -= barrier_speed;
                all_transforms[i].localPosition = b_loc;
                
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
