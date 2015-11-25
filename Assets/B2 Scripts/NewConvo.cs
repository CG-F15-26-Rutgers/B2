using UnityEngine;
using System.Collections;
using TreeSharpPlus;
using RootMotion.FinalIK;

public class NewConvo : MonoBehaviour {

    public GameObject person1;
    public GameObject person2;
    public GameObject deltrese;
    public GameObject backboard;
    public GameObject basketball;
    public Transform DeltreseGoTo;
    public FullBodyBipedEffector Effector;
    public FullBodyBipedEffector shake;
    public InteractionObject shake1;
    public InteractionObject shake2;
    public InteractionObject pokeball;


    private BehaviorAgent agent;


    void Start()
    {
       
    }

    void Update () {

        Val<Vector3> P1Pos = Val.V(() => person1.transform.position);
        Val<Vector3> P2Pos = Val.V(() => person2.transform.position);
        Val<Vector3> P3Pos = Val.V(() => deltrese.transform.position);

        if (Input.GetKeyDown(KeyCode.E) == true)
        {
            agent = new BehaviorAgent(conversationTree(P1Pos, P2Pos, P3Pos));
            BehaviorManager.Instance.Register(agent);
            agent.StartBehavior();
        }
    }


    #region
    //
    //Root Node
    //
    protected Node conversationTree(Val<Vector3> P1Pos, Val<Vector3> P2Pos, Val<Vector3> P3Pos)
    {
        return new Sequence(new SequenceParallel(DeltreseThinking(), OrientAndWave(P1Pos, P2Pos), WalkAndTalk(P1Pos, P2Pos)), DeltresePickUpBall(),
                            new SequenceParallel(DeltreseWalkTo(DeltreseGoTo), ThatDamnDeltrese(P3Pos)),
                            DeltreseShootBall(P3Pos),
                            CallDeltrese(P1Pos, P2Pos, P3Pos),
							EOC(deltrese.transform.position));
    }
    #endregion
	
    #region
    //
    //Children/Non-Leaf Nodes
    //

	protected Node EOC(Val<Vector3> deltresePos)
	{
		// End of conversation, call deltrese over and make fun of her, she cri evertiem
		
		return new Sequence(new LeafTrace("Interaction"),
							new SequenceParallel(person1.GetComponent<BehaviorMecanim>().Node_OrientTowards(deltresePos),
												 person1.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("Wave", 1000),
												 person2.GetComponent<BehaviorMecanim>().Node_OrientTowards(deltresePos),
												 person2.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("Wave", 1000)),
							new SequenceParallel(person1.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("beingcocky", 5000),
												 person2.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("beingcocky", 5000)),
							new SequenceParallel(person1.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("cheer", 3000),
												 person2.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("clap", 3000),
												 deltrese.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("cry", 3000)));
	}
		
    protected Node OrientAndWave( Val<Vector3> P1Pos, Val<Vector3> P2Pos )
    {
        return new Sequence(person1.GetComponent<BehaviorMecanim>().Node_OrientTowards(P2Pos),
                            P1Wave(),
                            person2.GetComponent<BehaviorMecanim>().Node_OrientTowards(P1Pos),
                            P2Wave());
    }
	
	
    protected Node WalkAndTalk( Val<Vector3> P1Pos , Val<Vector3> P2Pos)
    {
        return new Sequence(person2.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(P1Pos, 2.5f),
                            person2.GetComponent<BehaviorMecanim>().Node_OrientTowards(P1Pos), 
                            person1.GetComponent<BehaviorMecanim>().Node_OrientTowards(P2Pos),
                            Talk());
    }


    protected Node CallDeltrese(Val<Vector3> P1Pos, Val<Vector3> P2Pos, Val<Vector3> P3Pos)
    {
        return new Sequence(eyeContact(P1Pos, P2Pos),
                            person2.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("think", 3000),
                            deltrese.GetComponent<BehaviorMecanim>().Node_OrientTowards(P1Pos),
                            deltrese.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("Wave", 3000),
                            deltrese.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(P2Pos, 3.0f),
                            Argue(P1Pos, P2Pos, P3Pos));
    }
    #endregion



    #region
    //
    //Control Nodes/Leaf Nodes
    //
    protected Node P1Wave()
    {
        return new Sequence(person1.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("Wave", 1000));
    }
    

    protected Node P2Wave()
    {
        return new Sequence(person2.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("Wave", 1000));
    }


    protected Node P1isBored()
    {
        return new Sequence( person1.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("texting", 5000) );
    }


    protected Node DeltreseThinking()
    {
        return new Sequence(deltrese.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("think", 10000));
    }

    
    protected Node DeltreseShootBall(Val<Vector3> P3Pos)
    {
        Vector3 targetDifference = DeltreseGoTo.position - backboard.transform.position;
        // Sequence throwAnimation = new Sequence(deltrese.GetComponent<BehaviorMecanim>().Node_BodyAnimation("throw", true),
        //                                     new LeafWait(2500),
        //                                     deltrese.GetComponent<BehaviorMecanim>().Node_BodyAnimation("throw", true));
        
        return new Sequence(SequenceParallel(new LeafTrace("throwBall"),
											 basketball.GetComponent<BasketBall>().invokeThrow(targetDifference),
											 deltrese.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("Wave", 1000)),
							deltrese.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("cry", 1000);
    }
     

    protected Node ST_BodyAnim(GameObject guy, string gName, long length)
    {
        Val<string>name = Val.V(() => gName);
        Val<bool>start = Val.V(() => true);
        return new Sequence(guy.GetComponent<BehaviorMecanim>().Node_BodyAnimation(name, start),
                            new LeafWait(1000),
                            guy.GetComponent<BehaviorMecanim>().Node_BodyAnimation(name, false));
    }
     

    protected Node HandShake()
    {
        Val<FullBodyBipedEffector> effecting = Val.V(() => shake);
        Val<InteractionObject> Shake1 = Val.V(() => shake1);
        Val<InteractionObject> Shake2 = Val.V(() => shake2);
        return new SequenceParallel(new Sequence(new LeafTrace("Interaction"),
												 person1.GetComponent<BehaviorMecanim>().Node_StartInteraction(effecting, Shake2),
												 new LeafWait(1000)),
                                    new Sequence(new LeafTrace("Interaction"),
												 person2.GetComponent<BehaviorMecanim>().Node_StartInteraction(effecting, Shake1),
												 new LeafWait(1000)));
    }


    protected Node DeltresePickUpBall()
    {
        Val<FullBodyBipedEffector> effecting = Val.V(() => Effector);
        Val<InteractionObject> Pokeball = Val.V(() => pokeball);
        
        return new Sequence(new LeafTrace("Interaction"),
                            deltrese.GetComponent<BehaviorMecanim>().Node_StartInteraction(effecting, Pokeball),
                            new LeafWait(1000));
    }

    protected Node ThatDamnDeltrese(Val<Vector3> P3Pos)
    {        
        return new Sequence(person1.GetComponent<BehaviorMecanim>().Node_HeadLook(P3Pos),
                            person2.GetComponent<BehaviorMecanim>().Node_HeadLook(P3Pos));
    }


    protected Node DeltreseWalkTo(Transform target)
    {
        return new Sequence(deltrese.GetComponent<BehaviorMecanim>().Node_GoTo(target.position));
    }


    protected Node Talk()
    {
            return new Sequence(HandShake(),
                                new SequenceShuffle(person1.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("beingcocky", 5000),
                                                    person1.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("cheer", 5000),
                                                    person2.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("clap", 5000),
                                                    person1.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("think", 5000),
                                                    new SequenceParallel(P1isBored(), person2.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("cry", 5000))));
    }


    protected Node Argue(Val<Vector3> P1Pos, Val<Vector3> P2Pos, Val<Vector3> P3Pos)
    {
        return new Sequence(new SequenceParallel(person1.GetComponent<BehaviorMecanim>().Node_OrientTowards(P3Pos), 
                                                 person2.GetComponent<BehaviorMecanim>().Node_OrientTowards(P3Pos), 
                                                 deltrese.GetComponent<BehaviorMecanim>().Node_OrientTowards(P1Pos)),
                            new SequenceShuffle(deltrese.GetComponent<BehaviorMecanim>().Node_HeadLook(P1Pos),
                                                new LeafWait(1000),
                                                deltrese.GetComponent<BehaviorMecanim>().Node_HeadLook(P2Pos),
                                                person1.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("Yawn", 3000),
                                                person2.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("lookaway", 3000),
                                                deltrese.GetComponent<BehaviorMecanim>().ST_PlayBodyGesture("headnodyes", 3000)));
    }

    protected Node eyeContact( Val<Vector3> P1Pos , Val<Vector3>P2Pos )
    {
        Vector3 height = new Vector3(0.0f, 1.8f, 0.0f);
       
        Val<Vector3> newP1Pos = Val.V(() => P1Pos.Value + height);
        Val<Vector3> newP2Pos = Val.V(() => P2Pos.Value + height);
        
        return new SequenceParallel(person1.GetComponent<BehaviorMecanim>().Node_HeadLook(newP2Pos),
                                    person2.GetComponent<BehaviorMecanim>().Node_HeadLook(newP1Pos));
    }
    #endregion
}
