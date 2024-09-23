using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Manages the tavel board UI.
/// </summary>
public class TravelMapManager : MonoBehaviour
{
	public static TravelMapManager Current { private set; get; }

	public GameObject TravelBoardUI;

	[Serializable]
	public struct TravelMapCountryData
	{
		public string Name;

		public int ID;

		public GameObject CountryLockedImage;

		public GameObject[] Hubs;

		public bool IsUnlocked;
	}

	[SerializeField]
	public TravelMapCountryData[] travelMapCountryData;

	[SerializeField]

	void Awake()
	{
		if (Current != null && Current != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			Current = this;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		UpdateCountries();

		CloseUI();
	}

	// Update is called once per frame
	void Update()
	{
		if (TravelBoardUI.activeSelf)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				CloseUI();
			}
		}
	}

	public void LoadLevel()
	{

	}

	public void OpenUI()
	{
		TravelBoardUI.SetActive(true);
	}

	public void CloseUI()
	{
		TravelBoardUI.SetActive(false);
	}

	public void HideHubs(TravelMapCountryData countryData)
	{
		foreach (GameObject go in countryData.Hubs)
		{
			go.SetActive(false);
		}
	}

	public void ShowHubs(TravelMapCountryData countryData)
	{
		foreach (GameObject go in countryData.Hubs)
		{
			go.SetActive(true);
		}
	}

	public void UpdateCountries()
	{
		for (int i = 0; i < travelMapCountryData.Length; i++)
		{
			if (travelMapCountryData[i].IsUnlocked)
			{
				travelMapCountryData[i].CountryLockedImage.SetActive(false);

				ShowHubs(travelMapCountryData[i]);
			}
		}
	}

	public void UnlockCountry(string name)
	{
		for (int i = 0; i < travelMapCountryData.Length; i++)
		{
			if (travelMapCountryData[i].Name == name)
			{
				travelMapCountryData[i].IsUnlocked = true;

				travelMapCountryData[i].CountryLockedImage.SetActive(false);

				ShowHubs(travelMapCountryData[i]);
			}
		}
	}

	public void UnlockCountry(int id)
	{
		for (int i = 0; i < travelMapCountryData.Length; i++)
		{
			if (travelMapCountryData[i].ID == id)
			{
				travelMapCountryData[i].IsUnlocked = true;

				travelMapCountryData[i].CountryLockedImage.SetActive(false);

				ShowHubs(travelMapCountryData[i]);
			}
		}
	}
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|