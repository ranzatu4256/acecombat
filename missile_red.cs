using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// RollerAgent
public class missile_red : MonoBehaviour
{
    public GameObject unit;
    public GameObject target; // 注視したいオブジェクトをInspectorから入れておく
    public GameObject stock;
    public GameObject stock_other;
    public GameObject score;

    // Update is called once per frame
    void Update()
    {
        Rigidbody rBody = GetComponent<Rigidbody>();
        // ターゲット方向のベクトルを取得
        Vector3 relativePos = target.transform.position - this.transform.position;
        // 方向を、回転情報に変換
        Quaternion rotation = Quaternion.LookRotation(relativePos);

        if (stock.CompareTag("empty"))
        {
            float speed = 0.01f;
            transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, speed);
            //transform.Rotate(new Vector3(180f, 180f, 0f));
            GetComponent<Rigidbody>().AddForce(transform.forward * 200f, ForceMode.Force);

            Transform myTransform = this.transform;
            Vector3 worldAngle = myTransform.eulerAngles;
            worldAngle.y = 90.0f;
            worldAngle.z = 90.0f;
            myTransform.eulerAngles = worldAngle;
        }

        if (stock.CompareTag("ready"))
        {
            transform.rotation = unit.transform.rotation;
            this.transform.localPosition = unit.transform.position;
        }

        rBody.velocity = Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        //接触したオブジェクトのタグ
        if (other.CompareTag("Blue"))
        {
            stock.tag = "ready";
            stock_other.tag = "ready";
            score.tag = "win_red";
        }

    }
}