using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Harmony;
using UnityEngine;

namespace AgentCTRL_MOD
{
public class Harmony_Patch
{
	private const int MinFunctionKey = 2;

	private const int MaxFunctionKey = 12;

	private const string PrefKeyPrefix = "Lobotomy.refracta.AgentCTRL2.slot.";

	private static bool _loadedFromPrefs;

	private static readonly Dictionary<int, List<string>> SlotNamesByFunctionKey = new Dictionary<int, List<string>>();

	private static readonly bool[] CtrlHandledByHold = new bool[13];

	private static readonly bool[] ShiftHandledByHold = new bool[13];

	private static readonly bool[] SelectHandledByHold = new bool[13];

	public Harmony_Patch()
	{
		try
		{
			HarmonyInstance obj = HarmonyInstance.Create("Lobotomy.refracta.AgentCTRL2");
			MethodInfo method = typeof(Harmony_Patch).GetMethod("UnitMouseEventManager_Update");
			obj.Patch((MethodBase)typeof(UnitMouseEventManager).GetMethod("Update", AccessTools.all), (HarmonyMethod)null, new HarmonyMethod(method), (HarmonyMethod)null);
		}
		catch (Exception)
		{
		}
	}

	private static void EnsureSlotsLoaded()
	{
		if (_loadedFromPrefs)
		{
			return;
		}
		SlotNamesByFunctionKey.Clear();
		for (int i = MinFunctionKey; i <= MaxFunctionKey; i++)
		{
			SlotNamesByFunctionKey[i] = LoadSlot(i);
		}
		_loadedFromPrefs = true;
	}

	private static string GetSlotPrefKey(int functionKeyNumber)
	{
		return PrefKeyPrefix + functionKeyNumber;
	}

	private static List<string> LoadSlot(int functionKeyNumber)
	{
		List<string> list = new List<string>();
		string @string = PlayerPrefs.GetString(GetSlotPrefKey(functionKeyNumber), string.Empty);
		if (string.IsNullOrEmpty(@string))
		{
			return list;
		}
		string[] array = @string.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			try
			{
				string @string2 = Encoding.UTF8.GetString(Convert.FromBase64String(array[i]));
				if (!string.IsNullOrEmpty(@string2))
				{
					list.Add(@string2);
				}
			}
			catch
			{
			}
		}
		return list;
	}

	private static void SaveSlot(int functionKeyNumber, List<string> names)
	{
		List<string> value = null;
		if (!SlotNamesByFunctionKey.TryGetValue(functionKeyNumber, out value))
		{
			value = new List<string>();
			SlotNamesByFunctionKey[functionKeyNumber] = value;
		}
		value.Clear();
		List<string> list = new List<string>();
		for (int i = 0; i < names.Count; i++)
		{
			if (!string.IsNullOrEmpty(names[i]))
			{
				value.Add(names[i]);
				list.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(names[i])));
			}
		}
		PlayerPrefs.SetString(GetSlotPrefKey(functionKeyNumber), string.Join("|", list.ToArray()));
		PlayerPrefs.Save();
	}

	private static string GetRuntimeSlotKey(int functionKeyNumber)
	{
		return "CTRLS_" + functionKeyNumber;
	}

	private static List<UnitMouseEventTarget> GetOrCreateRuntimeSlot(int functionKeyNumber)
	{
		if (Add_On.instance == null || Add_On.instance.ObjectList == null)
		{
			return null;
		}
		string runtimeSlotKey = GetRuntimeSlotKey(functionKeyNumber);
		if (!Add_On.instance.ObjectList.ContainsKey(runtimeSlotKey))
		{
			Add_On.instance.ObjectList.Add(runtimeSlotKey, new List<UnitMouseEventTarget>());
		}
		List<UnitMouseEventTarget> list = Add_On.instance.ObjectList[runtimeSlotKey] as List<UnitMouseEventTarget>;
		if (list == null)
		{
			list = new List<UnitMouseEventTarget>();
			Add_On.instance.ObjectList[runtimeSlotKey] = list;
		}
		return list;
	}

	private static bool IsCtrlHeld()
	{
		return Input.GetKey((KeyCode)306) || Input.GetKey((KeyCode)305) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
	}

	private static bool IsShiftHeld()
	{
		return Input.GetKey((KeyCode)304) || Input.GetKey((KeyCode)303) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
	}

	private static bool IsEnglishLanguage()
	{
		try
		{
			if (GlobalGameManager.instance != null)
			{
				string language = GlobalGameManager.instance.language;
				if (!string.IsNullOrEmpty(language) && string.Equals(language, SupportedLanguage.en, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if (GlobalGameManager.instance.Language == SystemLanguage.English)
				{
					return true;
				}
			}
		}
		catch
		{
		}
		return false;
	}

	private static string Localize(string korean, string english)
	{
		if (IsEnglishLanguage())
		{
			return english;
		}
		return korean;
	}

	private static KeyCode GetFunctionKeyCode(int functionKeyNumber)
	{
		switch (functionKeyNumber)
		{
		case 2:
			return KeyCode.F2;
		case 3:
			return KeyCode.F3;
		case 4:
			return KeyCode.F4;
		case 5:
			return KeyCode.F5;
		case 6:
			return KeyCode.F6;
		case 7:
			return KeyCode.F7;
		case 8:
			return KeyCode.F8;
		case 9:
			return KeyCode.F9;
		case 10:
			return KeyCode.F10;
		case 11:
			return KeyCode.F11;
		case 12:
			return KeyCode.F12;
		default:
			return KeyCode.None;
		}
	}

	private static void SendSystemMessage(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		try
		{
			if (Notice.instance != null)
			{
				Notice.instance.Send(NoticeName.AddSystemLog, text);
			}
		}
		catch
		{
		}
		try
		{
			Debug.Log("[AgentCTRL_MOD] " + text);
		}
		catch
		{
		}
	}

	private static WorkerModel GetWorkerFromTarget(UnitMouseEventTarget target)
	{
		if (target == null)
		{
			return null;
		}
		return target.GetCommandTargetModel() as WorkerModel;
	}

	private static string GetAgentNameFromTarget(UnitMouseEventTarget target)
	{
		WorkerModel workerFromTarget = GetWorkerFromTarget(target);
		if (workerFromTarget == null || string.IsNullOrEmpty(workerFromTarget.name))
		{
			return string.Empty;
		}
		return workerFromTarget.name;
	}

	private static List<string> GetSelectedAgentNames(UnitMouseEventManager manager)
	{
		List<string> list = new List<string>();
		if (manager == null || manager.seletedtargets == null)
		{
			return list;
		}
		foreach (UnitMouseEventTarget seletedtarget in manager.seletedtargets)
		{
			string agentNameFromTarget = GetAgentNameFromTarget(seletedtarget);
			if (!string.IsNullOrEmpty(agentNameFromTarget))
			{
				list.Add(agentNameFromTarget);
			}
		}
		return list;
	}

	private static List<string> GetAgentNamesFromTargets(List<UnitMouseEventTarget> targets)
	{
		List<string> list = new List<string>();
		if (targets == null)
		{
			return list;
		}
		for (int i = 0; i < targets.Count; i++)
		{
			string agentNameFromTarget = GetAgentNameFromTarget(targets[i]);
			if (!string.IsNullOrEmpty(agentNameFromTarget))
			{
				list.Add(agentNameFromTarget);
			}
		}
		return list;
	}

	private static string FormatNameList(List<string> names)
	{
		if (names == null || names.Count == 0)
		{
			return Localize("[비어 있음]", "[Empty]");
		}
		return string.Join(", ", names.ToArray());
	}

	private static void CleanupRuntimeSlot(List<UnitMouseEventTarget> targets)
	{
		if (targets == null)
		{
			return;
		}
		for (int num = targets.Count - 1; num >= 0; num--)
		{
			UnitMouseEventTarget unitMouseEventTarget = targets[num];
			if (unitMouseEventTarget == null || !unitMouseEventTarget.IsSelectable())
			{
				targets.RemoveAt(num);
				continue;
			}
			WorkerModel workerFromTarget = GetWorkerFromTarget(unitMouseEventTarget);
			if (workerFromTarget == null || workerFromTarget.IsDead())
			{
				targets.RemoveAt(num);
			}
		}
	}

	private static List<UnitMouseEventTarget> ResolveTargetsByNames(List<string> names)
	{
		List<UnitMouseEventTarget> list = new List<UnitMouseEventTarget>();
		List<UnitMouseEventTarget> list2 = new List<UnitMouseEventTarget>();
		if (names == null || names.Count == 0)
		{
			return list;
		}
		UnitMouseEventTarget[] array = UnityEngine.Object.FindObjectsOfType<UnitMouseEventTarget>();
		for (int i = 0; i < names.Count; i++)
		{
			if (string.IsNullOrEmpty(names[i]))
			{
				continue;
			}
			for (int j = 0; j < array.Length; j++)
			{
				UnitMouseEventTarget unitMouseEventTarget = array[j];
				if (unitMouseEventTarget == null || list2.Contains(unitMouseEventTarget) || !unitMouseEventTarget.IsSelectable())
				{
					continue;
				}
				WorkerModel workerFromTarget = GetWorkerFromTarget(unitMouseEventTarget);
				if (workerFromTarget != null && !workerFromTarget.IsDead() && workerFromTarget.name == names[i])
				{
					list.Add(unitMouseEventTarget);
					list2.Add(unitMouseEventTarget);
					break;
				}
			}
		}
		return list;
	}

	private static void TryOpenSingleAgentInfo(UnitMouseEventManager manager)
	{
		if (manager == null || manager.seletedtargets == null || manager.seletedtargets.Count != 1)
		{
			return;
		}
		UnitMouseEventTarget unitMouseEventTarget = manager.seletedtargets[0];
		if (unitMouseEventTarget == null)
		{
			return;
		}
		AgentModel val = unitMouseEventTarget.GetCommandTargetModel() as AgentModel;
		if (val != null)
		{
			AgentInfoWindow.CreateWindow(val, false);
		}
	}

	private static void SelectTargets(UnitMouseEventManager manager, List<UnitMouseEventTarget> targets)
	{
		if (manager == null || manager.seletedtargets == null || targets == null || targets.Count == 0)
		{
			return;
		}
		manager.UnselectAll();
		for (int i = 0; i < targets.Count; i++)
		{
			UnitMouseEventTarget unitMouseEventTarget = targets[i];
			if (unitMouseEventTarget == null || !unitMouseEventTarget.IsSelectable())
			{
				continue;
			}
			unitMouseEventTarget.OnSelect();
			manager.seletedtargets.Add(unitMouseEventTarget);
		}
		TryOpenSingleAgentInfo(manager);
	}

	public static void UnitMouseEventManager_Update(UnitMouseEventManager __instance)
	{
		EnsureSlotsLoaded();
		if (__instance == null)
		{
			return;
		}
		for (int i = MinFunctionKey; i <= MaxFunctionKey; i++)
		{
			KeyCode functionKeyCode = GetFunctionKeyCode(i);
			if (functionKeyCode == KeyCode.None || !Input.GetKey(functionKeyCode))
			{
				CtrlHandledByHold[i] = false;
				ShiftHandledByHold[i] = false;
				SelectHandledByHold[i] = false;
				continue;
			}
			List<UnitMouseEventTarget> orCreateRuntimeSlot = GetOrCreateRuntimeSlot(i);
			List<string> value = null;
			if (!SlotNamesByFunctionKey.TryGetValue(i, out value))
			{
				value = new List<string>();
				SlotNamesByFunctionKey[i] = value;
			}
			bool flag = IsCtrlHeld();
			bool flag2 = IsShiftHeld();
			if (flag)
			{
				if (CtrlHandledByHold[i])
				{
					continue;
				}
				CtrlHandledByHold[i] = true;
				ShiftHandledByHold[i] = false;
				SelectHandledByHold[i] = false;
				if (__instance.seletedtargets != null && __instance.seletedtargets.Count > 0)
				{
					if (orCreateRuntimeSlot != null)
					{
						orCreateRuntimeSlot.Clear();
						foreach (UnitMouseEventTarget seletedtarget in __instance.seletedtargets)
						{
							if (seletedtarget != null)
							{
								orCreateRuntimeSlot.Add(seletedtarget);
							}
						}
					}
					List<string> selectedAgentNames = GetSelectedAgentNames(__instance);
					SaveSlot(i, selectedAgentNames);
					SendSystemMessage(Localize("Ctrl + F" + i + " 저장: " + FormatNameList(selectedAgentNames), "Ctrl + F" + i + " saved: " + FormatNameList(selectedAgentNames)));
				}
				else
				{
					SendSystemMessage(Localize("Ctrl + F" + i + " 저장 실패: 선택된 직원이 없습니다.", "Ctrl + F" + i + " save failed: no selected agents."));
				}
				continue;
			}
			CtrlHandledByHold[i] = false;
			if (flag2)
			{
				if (ShiftHandledByHold[i])
				{
					continue;
				}
				ShiftHandledByHold[i] = true;
				SelectHandledByHold[i] = false;
				List<string> list = value;
				if ((list == null || list.Count == 0) && orCreateRuntimeSlot != null && orCreateRuntimeSlot.Count > 0)
				{
					list = GetAgentNamesFromTargets(orCreateRuntimeSlot);
				}
				SendSystemMessage(Localize("Shift + F" + i + ": " + FormatNameList(list), "Shift + F" + i + ": " + FormatNameList(list)));
				continue;
			}
			ShiftHandledByHold[i] = false;
			if (SelectHandledByHold[i])
			{
				continue;
			}
			SelectHandledByHold[i] = true;
			if (orCreateRuntimeSlot != null)
			{
				CleanupRuntimeSlot(orCreateRuntimeSlot);
				if (orCreateRuntimeSlot.Count == 0 && value.Count > 0)
				{
					orCreateRuntimeSlot.AddRange(ResolveTargetsByNames(value));
				}
				if (orCreateRuntimeSlot.Count > 0)
				{
					SelectTargets(__instance, orCreateRuntimeSlot);
				}
			}
			else if (value.Count > 0)
			{
				SelectTargets(__instance, ResolveTargetsByNames(value));
			}
		}
	}
}
}
