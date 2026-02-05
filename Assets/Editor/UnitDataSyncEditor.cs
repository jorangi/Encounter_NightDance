using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;
using Cysharp.Threading.Tasks;
using System.Text;

namespace Encounter.NightDance.Editor
{
    public class UnitDataSyncEditor : EditorWindow
    {
        private const string API_URL = "http://138.2.109.131:8001/api/units";
        private const string SAVE_PATH = "Assets/Script/Character/Data/UnitDataDTO/";

        [MenuItem("Encounter/NightDance/Sync Unit Data")]
        public static void ShowWindow()
        {
            GetWindow<UnitDataSyncEditor>("Entity Sync");
        }
        private void OnGUI()
        {
            GUILayout.Label("데이터 동기화", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if(GUILayout.Button("동기화 실행", GUILayout.Height(40)))
            {
                FetchDataFromServer().Forget();
            }
        }
        private async UniTaskVoid FetchDataFromServer()
        {
            Debug.Log("<color=cyan>[Sync]</color> 데이터베이스 접속 중");
            using UnityWebRequest request = UnityWebRequest.Get(API_URL);
            try
            {
                var operation = request.SendWebRequest().ToUniTask();
                var jsonData = request.downloadHandler.text;
                var unitList = JsonConvert.DeserializeObject<List<UnitDataDTO>>(jsonData);
                await UniTask.SwitchToMainThread();
                ProcessSync(unitList);
            }catch(UnityWebRequestException e){
                Debug.LogError($"[동기화 중 오류 발생]\n{e.Message}");
            }
        }
        private void ProcessSync(List<UnitDataDTO> loadedUnitList)
        {
            if(!Directory.Exists(SAVE_PATH)) Directory.CreateDirectory(SAVE_PATH);
            HashSet<string> serverIds = new();
            StringBuilder logs = new();
            foreach(UnitDataDTO data in loadedUnitList)
            {
                serverIds.Add(data.id);
                string assetPath = $"{SAVE_PATH}/{data.id}.asset";
                UnitData unitData = AssetDatabase.LoadAssetAtPath<UnitData>(assetPath);

                bool isNew = false;
                if (unitData == null)
                {
                    unitData = CreateInstance<UnitData>();
                    isNew = true;
                }
                UpdateUnitData(unitData, data);
                if (isNew)
                {
                    AssetDatabase.CreateAsset(unitData, assetPath);
                    logs.Append($"<color=green>[생성됨]</color> 새 유닛 데이터: {data.id}");
                }
                else
                {
                    EditorUtility.SetDirty(unitData);
                    logs.Append($"<color=yellow>[수정됨]</color> 유닛 데이터: {data.id}");
                }
            }
            DeleteOldUnitData(serverIds);
            Debug.Log(logs.ToString());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=cyan>[동기화 완료]</color> 모든 유닛 데이터 동기화 완료");
        }
        private void UpdateUnitData(UnitData unitData, UnitDataDTO dataFromServer)
        {
            unitData.Initialize(dataFromServer.id, dataFromServer.base_stats, dataFromServer.growth_stats);
        }
        private void DeleteOldUnitData(HashSet<string> serverIds)
        {
            string[] localGuids = AssetDatabase.FindAssets("t:UnitData", new[] {SAVE_PATH});
            StringBuilder logs = new();
            foreach(var guid in localGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path);
                if(!serverIds.Contains(fileName))
                {
                    AssetDatabase.DeleteAsset(path);
                    logs.Append($"<color=red>[삭제됨]</color> 유닛 데이터: {fileName}");
                }
                else
                {
                    serverIds.Remove(fileName);
                    logs.Append($"<color=blue>[유지됨]</color> 유닛 데이터: {fileName}");
                }
            }
            Debug.Log(logs.ToString());
        }
        // private void RegisterToAddessables(List<UnitData> unitDatas)
        // {
        //     AddessableSetting
        // }
    }
}