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

			if ( tokenSource != null ) {
				tokenSource.Cancel();
				
				Application.quitting -= tokenSource.Cancel;
			}
			
			tokenSource = new CancellationTokenSource();

			Application.quitting += tokenSource.Cancel;

			task = gridManager.FindPathAsync( transform.position, oldPos, tokenSource.Token );
		}

		private void Reset() {
			Application.quitting -= tokenSource.Cancel;
			
			tokenSource.Cancel();
			tokenSource = null;
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

		public void OnDrawGizmos() {
			if ( walkTos == null ) {
				return;
			}

			for ( var i = 1; i < walkTos.Length; ++i ) {
				Gizmos.color = Color.HSVToRGB( i / ( float )walkTos.Length, 1, 1 );

				Gizmos.DrawLine( walkTos[i - 1].worldPosition, walkTos[i].worldPosition );
			}
		}

		private IEnumerator WalkToTarget() {
			foreach ( var node in walkTos ) {
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

					position -= direction * 2.5f * Time.deltaTime;

					myTransform.position = position;
				}
			}
		}
	}
}
