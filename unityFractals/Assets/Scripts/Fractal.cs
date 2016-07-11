using UnityEngine;
using System.Collections;

public class Fractal : MonoBehaviour {

	public Material material;

	public int maxDepth;
	public float childScale;
	public float spawnProbability;
	public float maxTwist;

	private int depth;
	private Material[,] materials;
	public Mesh[] meshes;

	public float maxRotationSpeed;

	private float rotationSpeed;

	private static Vector3[] childDirections = {
		Vector3.up,
		Vector3.right,
		Vector3.left,
		Vector3.forward,
		Vector3.back
	} ;

	private static Quaternion[] childOrientations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, -90f),
		Quaternion.Euler(0f, 0f, 90f),
		Quaternion.Euler(90f, 0f, 0f),
		Quaternion.Euler(-90f, 0f, 0f)
	} ;

	// Use this for initialization
	void Start () {
		rotationSpeed = Random.Range (-maxRotationSpeed, maxRotationSpeed);
		transform.Rotate (Random.Range (-maxTwist, maxTwist), 0f, 0f);
		if (materials == null) {
			InitializeMaterials ();
		}
		gameObject.AddComponent<MeshFilter> ().mesh = meshes[Random.Range(0, meshes.Length)];
		gameObject.AddComponent<MeshRenderer> ().material = materials[depth, Random.Range(0, 2)];

		if (depth < maxDepth) {
			StartCoroutine (CreateChildren ());
			if (depth < 1) {
				StartCoroutine (CreateBottom ());
			}
		}
	}

	private void Update(){
		transform.Rotate (0f, rotationSpeed * Time.deltaTime, 0f);
	}

	private void InitializeMaterials () {
		materials = new Material[maxDepth + 1, 2];
		for (int i = 0; i <= maxDepth; i++) {
			float t = i / maxDepth - 1f;
			t *= t;
			materials[i, 0] = new Material(material);
			materials[i, 0].color =
				Color.Lerp(Color.white, Color.yellow, t);
			materials [i, 1] = new Material (material);
			materials [i, 1].color = Color.Lerp (Color.white, Color.cyan, t);
		}
		materials [maxDepth, 0].color = Color.magenta;
		materials [maxDepth, 1].color = Color.red;
	}

	private void Initialize (Fractal parent, Vector3 direction, Quaternion orientation){
		meshes = parent.meshes;
		spawnProbability = parent.spawnProbability;
		maxRotationSpeed = parent.maxRotationSpeed;
		maxTwist = parent.maxTwist;
		materials = parent.materials;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		childScale = parent.childScale;
		transform.parent = parent.transform;
		transform.localScale = Vector3.one * childScale;
		transform.localPosition = direction * (0.5f + 0.5f * childScale);
		transform.localRotation = orientation;
	}

	private IEnumerator CreateChildren(){
		for (int i = 0; i < childDirections.Length; i++) {
			if (Random.value < spawnProbability) {
				yield return new WaitForSeconds (Random.Range (.1f, .5f));
				new GameObject ("Fractal Child").AddComponent<Fractal> ()
				.Initialize (this, childDirections [i], childOrientations [i]);
			}
		}
	}
	private IEnumerator CreateBottom(){
		yield return new WaitForSeconds (.5f);
		if (Random.value < spawnProbability) {
			new GameObject ("Fractal Child").AddComponent<Fractal> ()
			.Initialize (this, new Vector3 (0, -1, 0), Quaternion.Euler (0f, -90f, 0f));
		}
	}
}