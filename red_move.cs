using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine.UI;

public class red_move : Agent
{
    Rigidbody rBody;
    public GameObject my_missile1;
    public GameObject my_missile2;

    public GameObject en_missile1;
    public GameObject en_missile2;

    public GameObject enemy;

    public GameObject stock1;
    public GameObject stock2;

    public GameObject score;

    public float lapseTime1;
    public float lapseTime2;

    // 初期化時に呼ばれる
    public override void Initialize()
    {
        this.rBody = GetComponent<Rigidbody>();
        lapseTime1 = 10.0f;
        lapseTime2 = 10.0f;
    }

    // エピソード開始時に呼ばれる
    public override void OnEpisodeBegin()
    {
    }

    // 観察取得時に呼ばれる
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(this.transform.localRotation);

        sensor.AddObservation(my_missile1.transform.localPosition);
        sensor.AddObservation(my_missile1.transform.localRotation);
        sensor.AddObservation(my_missile2.transform.localPosition);
        sensor.AddObservation(my_missile2.transform.localRotation);

        sensor.AddObservation(en_missile1.transform.localPosition);
        sensor.AddObservation(en_missile1.transform.localRotation);
        sensor.AddObservation(en_missile2.transform.localPosition);
        sensor.AddObservation(en_missile2.transform.localRotation);

        sensor.AddObservation(enemy.transform.localPosition);
        sensor.AddObservation(enemy.transform.localRotation);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;
        int action = actions.DiscreteActions[0];

        if (action == 1) rotateDir.y = 1f;
        if (action == 2) rotateDir.y = -1f;
        transform.Rotate(rotateDir, Time.deltaTime * 100f);

        if (stock1.CompareTag("ready"))
        {
            if (action == 3)
            {
                stock1.tag = "empty";
                lapseTime1 = 0.0f;
            }
        }

        if (stock2.CompareTag("ready"))
        {
            if (action == 4)
            {
                stock2.tag = "empty";
                lapseTime2 = 0.0f;
            }
        }
    }

    void Update()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * 80f, ForceMode.Force);
        rBody.velocity = Vector3.zero;

        if (stock1.CompareTag("ready"))
        {
            stock1.GetComponent<Renderer>().material.color = Color.blue;
        }
        else
        {
            stock1.GetComponent<Renderer>().material.color = Color.red;
            lapseTime1 += Time.deltaTime;
            if (lapseTime1 >= 10)
            {
                stock1.tag = "ready";
                lapseTime1 = 0.0f;
            }
        }

        if (stock2.CompareTag("ready"))
        {
            stock2.GetComponent<Renderer>().material.color = Color.blue;
        }
        else
        {
            stock2.GetComponent<Renderer>().material.color = Color.red;
            lapseTime2 += Time.deltaTime;
            if (lapseTime2 >= 10)
            {
                stock2.tag = "ready";
                lapseTime2 = 0.0f;
            }
        }
    }

    //OnTriggerEnter関数
    //接触したオブジェクトが引数otherとして渡される
    void OnTriggerEnter(Collider other)
    {
        //接触したオブジェクトのタグ
        if (other.CompareTag("map_end"))
        {
            this.transform.localPosition = new Vector3(22f, -22f, 0.0f);
            enemy.transform.localPosition = new Vector3(-22f, 22f, 0.0f);
            en_missile1.transform.localPosition = new Vector3(-22f, 22f, 0.0f);
            en_missile2.transform.localPosition = new Vector3(-22f, 22f, 0.0f);

            stock1.tag = "ready";
            stock2.tag = "ready";

            lapseTime1 = 10.0f;
            lapseTime2 = 10.0f;

            score.tag = "crush_red";

            this.AddReward(-0.2f);
            EndEpisode();
        }

        if (other.CompareTag("blue_attack"))
        {
            this.transform.localPosition = new Vector3(22f, -22f, 0.0f);
            enemy.transform.localPosition = new Vector3(-22f, 22f, 0.0f);
            en_missile1.transform.localPosition = new Vector3(-22f, 22f, 0.0f);
            en_missile2.transform.localPosition = new Vector3(-22f, 22f, 0.0f);

            Transform myTransform = this.transform;
            Vector3 worldAngle = myTransform.eulerAngles;
            worldAngle.x = -135.0f;
            worldAngle.y = 90.0f;
            worldAngle.z = 90.0f;
            myTransform.eulerAngles = worldAngle;

            stock1.tag = "ready";
            stock2.tag = "ready";

            lapseTime1 = 10.0f;
            lapseTime2 = 10.0f;

            this.AddReward(-1f);
            EndEpisode();
        }
    }

    // ヒューリスティックモードの行動決定時に呼ばれる
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;
        actions[0] = 0;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            actions[0] = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            actions[0] = 2;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            actions[0] = 3;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            actions[0] = 4;
        }
    }
}

//mlagents-learn config/acecombat.yaml --run-id=acecombat --env=apps/acecombat --force