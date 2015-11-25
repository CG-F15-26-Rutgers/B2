using UnityEngine;
using System.Collections;
using TreeSharpPlus;

public class BasketBall : MonoBehaviour {

    Rigidbody ball;

    void Start()
    {
        ball = GetComponent<Rigidbody>();
    }


    void OnInteractionStart(Transform t)
    {
        ball.isKinematic = true;
        ball.useGravity = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(transform.forward * Time.deltaTime);
        }

    }


    public Node ST_throw(Val<Vector3> direction)
    {
        return new LeafInvoke(() => throwball(direction));
    }

    public RunStatus throwball(Val<Vector3> direction)
    {
        transform.parent = null;
        ball.velocity = direction.Value * 2.1f;
        ball.isKinematic = false;
        ball.useGravity = false;
        return RunStatus.Success;
    }
}
