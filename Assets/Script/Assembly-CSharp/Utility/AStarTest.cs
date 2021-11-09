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

		private AStarGridManager.Node[] walkTos;

		public void Start() {
			oldPos      = target.transform.position;
			tokenSource = new CancellationTokenSource();

			task = gridManager.FindPathAsync( transform.position, oldPos, tokenSource.Token );
		}

		public void Update() {
			if ( oldPos != target.transform.position ) {
				if ( coroutine != null ) {
					StopCoroutine( coroutine );

					coroutine = null;
				}

				if ( task.Status == TaskStatus.Running ) {
					tokenSource.Cancel( false );
				}

				Start();

				return;
			}

			if ( task.Status == TaskStatus.RanToCompletion && coroutine == null ) {
				walkTos = task.Result;

				coroutine = StartCoroutine( WalkToTarget() );
			}
		}

		public IEnumerator WalkToTarget() {
			for ( var i = 0; i < walkTos.Length; ++i ) {
				var node = walkTos[i];

				while ( transform.position != node.worldPosition ) {
					yield return new WaitForEndOfFrame();

					var myTransform = transform;
					var position    = myTransform.position;
					var heading     = position - node.worldPosition;
					var distance    = heading.magnitude;
					var direction   = heading / distance;

					if ( distance < 0.5f ) {
						break;
					}

					position -= direction * 3f * Time.deltaTime;

					myTransform.position = position;
				}
			}
		}
	}
}
