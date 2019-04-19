using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
	[Header("Components")]
	// Components of the character
	public CharacterHealth health;
	public CharacterAttack attack;
	public CharacterMover mover;

	// The characters transform
	[HideInInspector]
	public Transform tf;
	// The Animation controller
	[HideInInspector]
	public Animator anim;

	[Header("Character Data")]
	public CharacterClass characterClass = CharacterClass.Knight;
	[Header("Movement")]
	// how fast the character will move (Used for out of combate)
	public float moveSpeed = 1;

	[Header("Health")]
	// The maximum health of the character
	public int maxHealth = 100;
	// The amount of health the character has
	[HideInInspector]
	public int currentHealth;
	[HideInInspector]
	public bool isDead;

	[Header("Combat Stats")]
	public List<Ability> abilitiesList;

	[Header("AI Stats")]
	public int aiArmor;
	public int aiEvasion;

	private Armor currentArmor;
	private Helmet currentHelmet;
	private Weapon currentWeapon;

	public Armor CurrentArmor
	{
		get { return this.currentArmor; }
		set
		{
			if (this.currentArmor != null)
			{
				this.currentArmor.OnUnequip(this);
			}
			this.currentArmor = value;
			this.currentArmor.OnEquip(this);
			this.ResetHealth();
		}
	}

	public Helmet CurrentHelmet
	{
		get { return this.currentHelmet; }
		set
		{
			if (this.currentHelmet != null)
			{
				this.currentHelmet.OnUnequip(this);
			}
			this.currentHelmet = value;
			this.currentHelmet.OnEquip(this);
			this.ResetHealth();
		}
	}

	public Weapon CurrentWeapon
	{
		get { return this.currentWeapon; }
		set
		{
			if (this.currentWeapon != null)
			{
				this.currentWeapon.OnUnequip(this);
			}
			this.currentWeapon = value;
			this.currentWeapon.OnEquip(this);
		}
	}

	public int Defense
	{
		get { return this.CurrentArmor.gearDefense + this.CurrentHelmet.gearDefense; }
		//private set { this.Defense = (value * 0) + this.CurrentArmor.gearDefense + this.CurrentHelmet.gearDefense; }
	}

	public int Evasion
	{
		get { return this.CurrentArmor.gearEvasion + this.CurrentHelmet.gearEvasion; }
		//private set { this.Evasion = (value * 0) + this.CurrentArmor.gearEvasion + this.CurrentHelmet.gearEvasion; }
	}

	private void Awake()
	{
		DontDestroyOnLoad(this.gameObject);

		if (this.mover == null)
		{
			this.mover = this.GetComponent<CharacterMover>();
		}
		if (this.health == null)
		{
			this.health = this.GetComponent<CharacterHealth>();
		}
		if (this.attack == null)
		{
			this.attack = this.GetComponent<CharacterAttack>();
		}

		this.tf = this.GetComponent<Transform>();
		this.anim = this.GetComponent<Animator>();

		this.currentHealth = this.maxHealth;
	}

	private void Start()
	{
		Armor tempArmor;
		Helmet tempHelmet;
		Weapon tempWeapon;

		switch (this.characterClass)
		{
			case CharacterClass.Knight:
				tempArmor = new Armor("Armor", 100, 0, 0);
				this.CurrentArmor = tempArmor;
				tempHelmet = new Helmet("Helmet", 100, 0, 0);
				this.CurrentHelmet = tempHelmet;
				tempWeapon = new Weapon("Weapon", Element.None, 100, 100);
				this.CurrentWeapon = tempWeapon;
				break;
			case CharacterClass.Assassin:
				tempArmor = new Armor("Armor", 100, 0, 0);
				this.CurrentArmor = tempArmor;
				tempHelmet = new Helmet("Helmet", 100, 0, 0);
				this.CurrentHelmet = tempHelmet;
				tempWeapon = new Weapon("Weapon", Element.None, 100, 100);
				this.CurrentWeapon = tempWeapon;
				break;
			case CharacterClass.Druid:
				tempArmor = new Armor("Armor", 100, 0, 0);
				this.CurrentArmor = tempArmor;
				tempHelmet = new Helmet("Helmet", 100, 0, 0);
				this.CurrentHelmet = tempHelmet;
				tempWeapon = new Weapon("Weapon", Element.None, 100, 100);
				this.CurrentWeapon = tempWeapon;
				break;
			case CharacterClass.Sorcerer:
				tempArmor = new Armor("Armor", 100, 0, 0);
				this.CurrentArmor = tempArmor;
				tempHelmet = new Helmet("Helmet", 100, 0, 0);
				this.CurrentHelmet = tempHelmet;
				tempWeapon = new Weapon("Weapon", Element.None, 100, 100);
				this.CurrentWeapon = tempWeapon;
				break;
			case CharacterClass.Alchemist:
				tempArmor = new Armor("Armor", 100, 0, 0);
				this.CurrentArmor = tempArmor;
				tempHelmet = new Helmet("Helmet", 100, 0, 0);
				this.CurrentHelmet = tempHelmet;
				tempWeapon = new Weapon("Weapon", Element.None, 100, 100);
				this.CurrentWeapon = tempWeapon;
				break;
			case CharacterClass.Enemy:
				tempArmor = new Armor("Armor", 0, this.aiArmor, this.aiEvasion);
				this.CurrentArmor = tempArmor;
				tempHelmet = new Helmet("Helmet", 10, 0, 0);
				this.CurrentHelmet = tempHelmet;
				tempWeapon = new Weapon("Weapon", Element.None, 10, 100);
				this.CurrentWeapon = tempWeapon;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void ResetHealth()
	{
		this.currentHealth = this.maxHealth;
	}

	[ContextMenu("TestBasicAttack")]
	public void TestBasicAttack()
	{
		var targets = new CharacterData[1];
		targets[0] = CombatInstance.CurrentInstance.enemyList[0];
		this.abilitiesList[0].OnActivate(this, targets);
	}
}
