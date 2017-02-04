using UnityEditor;
using UnityEngine;

public class QuitOnEnter : StateMachineBehaviour {

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
#if UNITY_EDITOR
	    EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
