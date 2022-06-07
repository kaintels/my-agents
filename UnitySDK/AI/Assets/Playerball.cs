using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class Playerball : Agent
{
    Rigidbody rigid;
    new AudioSource audio;
    public int playerpoint;
    public GameLogic manager;
    bool isjump;
    public Transform Target;

    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log("새로운 신 등장");
        isjump = false;
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    public override void OnEpisodeBegin()
    {

        // Agent가 플랫폼 외부로 떨어지면(Y 좌표가 0이하가 되면), angularVelocity/velocity=0으로, 위치를 초기 좌표로 리셋
        if (this.transform.localPosition.y < 0)
        {
            this.rigid.angularVelocity = Vector3.zero;
            this.rigid.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // Target을 Random.value함수를 활용해서 새로운 무작위 위치에 이동
        Target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target/Agent의 위치 정보 수집
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent의 velocity 정보 수집
        sensor.AddObservation(rigid.velocity.x);
        sensor.AddObservation(rigid.velocity.z);
    }
    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        // Agent가 Target쪽으로 이동하기 위해 X, Z축으로의 Force를 정의
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rigid.AddForce(controlSignal * forceMultiplier);

        // Agent와 Target사이의 거리를 측정
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Target에 도달하는 경우 (거리가 1.42보다 작은 경우) Episode 종료
        if (distanceToTarget < 1.42)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // 플랫폼 밖으로 나가면 Episode 종료
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

}
