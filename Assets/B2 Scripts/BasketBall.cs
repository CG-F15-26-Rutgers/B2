using UnityEngine;
using System.Collections;
using TreeSharpPlus;

public class BasketBall : MonoBehaviour {

    Rigidbody ball;

    void Start()
    {
        ball = GetComponent<Rigidbody>();
		ball.isKinematic = true;
        ball.useGravity = false;
    }

    public Node invokeThrow(Val<Vector3> direction)
    {
        return new LeafInvoke(() => throwball(direction));
    }

    public RunStatus throwball(Val<Vector3> direction)
    {
        transform.parent = null;
		ball.isKinematic = false;
        ball.useGravity = true;
        ball.velocity = -direction.Value * 2.1f;
        
        return RunStatus.Success;
    }
}
