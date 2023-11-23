using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeComponent : MonoBehaviour {
    // Start is called before the first frame update
    private RectTransform rectTransform;

    // [SerializeField]
    private float shake_speed = 1f;
    // [SerializeField]
    private float radius = 1000f;

    private Vector2 nextPosition;
    private Vector2 originPosition;
    public bool shake = false;

    private bool ret = false;

    void Start () {
        rectTransform = this.GetComponent<RectTransform> ();
        originPosition = rectTransform.anchoredPosition;

        //float angle = Random.Range (0f, 6.28f);
        //nextPosition = new Vector2 (originPosition.x, Random.Range (0f, 10f));
    }

    // Update is called once per frame
    void Update () {

        if (shake == false) {
           
            rectTransform.anchoredPosition = Vector2.MoveTowards (rectTransform.anchoredPosition, originPosition, shake_speed);
            return;
        }
       
        //float step = shake_speed * Time.deltaTime;
        //rectTransform.anchoredPosition = Vector2.MoveTowards (rectTransform.anchoredPosition, originPosition + Random.insideUnitCircle * radius, step);

         rectTransform.anchoredPosition = Vector2.MoveTowards (rectTransform.anchoredPosition, nextPosition, shake_speed);

          Vector2 d1 = rectTransform.anchoredPosition - nextPosition;

          if (d1.magnitude < 0.01) {

              if (ret == true) {
                  nextPosition = originPosition;
              } else {
                  nextPosition = new Vector2 (originPosition.x, Random.Range (-5f, 5f));
              }
              ret = !ret;
          }
    }

}