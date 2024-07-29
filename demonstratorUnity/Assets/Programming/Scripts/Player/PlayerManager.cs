using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to manage the player.
/// </summary>
public class PlayerManager : MonoBehaviour
{
	public static PlayerManager Instance { get; private set; }

	private MovementController movementController;
	private LookController lookController;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		movementController = GetComponent<MovementController>();
		lookController = GetComponent<LookController>();
	}

	// Update is called once per frame
	void Update()
	{

	}


	/// <summary>
	/// Locks both the movement and look script if true.
	/// </summary>
	/// <param name="isLocked">Whether both scripts are locked.</param>
	public void LockPlayer(bool isLocked)
	{
		movementController.IsLocked = isLocked;
		lookController.IsLocked = isLocked;
	}


	/// <summary>
	/// Locks the specific scripts if true.
	/// </summary>
	/// <param name="lookLock">Locked the mouse.</param>
	/// <param name="movementLocked">Locks the movement.</param>
	public void LockPlayer(bool lookLock, bool movementLocked)
	{
		movementController.IsLocked = movementLocked;
		lookController.IsLocked = lookLock;
	}


	/// <summary>
	/// Disabled the camera and player model if true.
	/// </summary>
	/// <param name="isEnabled">Disables the camera and hides player model if true.</param>
	public void SetEnabledPlayer(bool isEnabled)
	{
		movementController.IsEnabled = isEnabled;
		lookController.IsEnabled = isEnabled;
	}

	/// <summary>
	/// Disabled specific aspects if true.
	/// </summary>
	/// <param name="isPlayerVisible">Hides the player model if true.</param>
	/// <param name="isCameraEnabled">Disables the camera if true.</param>
	public void SetEnabledPlayer(bool isPlayerVisible, bool isCameraEnabled)
	{
		movementController.IsEnabled = isPlayerVisible;
		lookController.IsEnabled = isCameraEnabled;
	}
}
