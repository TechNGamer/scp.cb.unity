using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SCPCB.Remaster.Utility {
	public class AStarTest : MonoBehaviour {
		public bool goToTarget;

		public GameObject                    target;
		public Vector3                       oldPos;
		public AStarGridManager              gridManager;
		public Task<AStarGridManager.Node[]> task;
		public CancellationTokenSource       tokenSource;
		public Coroutine                     coroutine;

		public void Start() {
			oldPos      = target.transform.position;
			tokenSource = new CancellationTokenSource();

			task = gridManager.FindPathAsync( transform.position, oldPos, tokenSource.Token );
		}

		public void Update() {
			if ( oldPos != target.transform.position || task.IsFaulted ) {
				tokenSource.Cancel( false );

				Start();
			}

			if ( !task.IsFaulted && task?.Result != null && task.IsCompleted && coroutine == null ) {
				coroutine = StartCoroutine( WalkToTarget( task.Result ) );
			}
		}

		public IEnumerator WalkToTarget( AStarGridManager.Node[] nodes ) {
			foreach ( var node in nodes ) {
				while ( transform.position != node.worldPosition ) {
					yield return new WaitForEndOfFrame();
					
					var myTransform = transform;
					var position    = myTransform.position;

					position += ( position - node.worldPosition ) * Time.deltaTime;

					myTransform.position = position;
				}
			}
		}
	}
}
