using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {
	[HideInInspector]
	public GameManager gm;
	[HideInInspector]
	public CharacterData data;

	//TODO: move to monster spawner script
	public bool isBoss;
	public GameObject[] PackData;

	//TODO: move to overworld monster script
	//place to put points for patrol routes
	public Transform[] patrolPoints;

	//The monster's transform
	private Transform tf;
	// what point on the patrol you are on
	private int PointnGo;
	//the navmesh agent for the patrol
	private NavMeshAgent agent;

	private void Awake() {
		//grab the Game manager
		this.gm = GameManager.GM;
		//grab the data from character data
		this.data = this.GetComponent<CharacterData>();
		//Agent component
		this.agent = this.GetComponent<NavMeshAgent>();
		this.tf = this.GetComponent<Transform>();
	}

	private void Start() {

		//start on a point
		this.NextPoint();
	}

	private void Update() {
		//tells if it is combat or not 
		if (CombatManager.inCombat == false) {
			/* TODO: enable and test
			//while not in combat, patrol
			if (!agent.pathPending && agent.remainingDistance < 0.5f) {
				//when you are close to the current point switch to a new point
				NextPoint();
			}*/
		}
	}

	private void NextPoint() {
		if (this.patrolPoints.Length == 0) {
			return;
		}
		//the end point of monster
		this.tf.position = Vector3.MoveTowards(this.tf.position, this.patrolPoints [this.PointnGo].position, this.data.moveSpeed);
		this.agent.destination = this.patrolPoints [this.PointnGo].position;
		//reset and do again change point
		this.PointnGo = (this.PointnGo + 1) % this.patrolPoints.Length;
	}
		
	//Automated state machine for combat
	public void TakeCombatTurn() {
		Debug.Log(this.name + ": Starting Combat Turn.", this.gameObject);

		//TODO: if(check for certain status effects)

		var currentTargets = new List<CharacterData> ();

		//Choose attack
		var currentAbility = this.data.abilitiesList [Random.Range(0, this.data.abilitiesList.Count)];

		if (currentAbility.targetingType == TargetingType.Self) {
			currentTargets.Add(this.data);
		} else {
			switch (currentAbility.abilityType) {
				case AbilityType.Damage:
					switch (currentAbility.targetingType) {
						case TargetingType.Single:
						case TargetingType.RandomSingle:
						//TODO: override target if taunted

							var targetIndex = 0;
							var targetCount = 0;

							foreach (var player in CombatInstance.CurrentInstance.alivePlayers) {
								if (player.isDead == false) {
									targetCount++;
								}
							}

							switch (targetCount) {
								case 0: 
									Debug.LogWarning("No players remain.");
									break;
								case 1:
									targetIndex = 0;
									break;
								case 2:
									targetIndex = Random.Range(0, 2);
									if (CombatInstance.CurrentInstance.alivePlayers [targetIndex].isDead) {
										switch (targetIndex) {
											case 0:
												targetIndex = 1;
												break;
											case 1:
												targetIndex = 2;
												break;
											case 2:
												targetIndex = 0;
												break;
										}
									}
									break;
								case 3:
									targetIndex = Random.Range(0, 2);
									break;
							}
							currentTargets.Add(CombatInstance.CurrentInstance.alivePlayers [targetIndex]);
							break;
						case TargetingType.Party:
							currentTargets.AddRange(CombatInstance.CurrentInstance.alivePlayers);
							break;
					}
					break;
				case AbilityType.Healing:
					switch (currentAbility.targetingType) {
						case TargetingType.Single:
						case TargetingType.RandomSingle:
							var targetIndex = 0;
							var targetCount = 0;

							foreach (var enemy in CombatInstance.CurrentInstance.enemyList) {
								if (enemy.isDead == false) {
									targetCount++;
								}
							}

							switch (targetCount) {
								case 1:
									targetIndex = 0;
									break;
								case 2:
									targetIndex = Random.Range(0, 1);
									break;
								case 3:
									targetIndex = Random.Range(0, 2);
									break;
							}
							currentTargets.Add(CombatInstance.CurrentInstance.enemyList [targetIndex]);
							break;
						case TargetingType.Party:
							currentTargets.AddRange(CombatInstance.CurrentInstance.enemyList);
							break;
					}
					break;
				case AbilityType.Utility:
				//TODO: AI Utility abilities
					break;
			}
		}

		currentAbility.OnActivate(this.data, currentTargets.ToArray());

		CombatInstance.CurrentInstance.NextTurn();
	}
}
