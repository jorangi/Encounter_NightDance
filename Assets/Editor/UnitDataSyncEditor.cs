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
        private const string API_URL = "http://138.2.109.131:8001/api/units"; //OCI 서버
        private const string SAVE_PATH = "Assets/ScriptableObjects/UnitData";

        /// <summary>
        /// 메뉴와 GUI 생성
        /// </summary>
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
        /// <summary>
        /// API 통신해서 데이터 받아오는 함수
        /// </summary>
        /// <returns></returns>
        private async UniTaskVoid FetchDataFromServer()
        {
            Debug.Log("<color=cyan>[Sync]</color> 데이터베이스 접속 중");
            var request = UnityWebRequest.Get(API_URL);
            try
            {
                UnityWebRequest operation= await request.SendWebRequest().ToUniTask();
                if(request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"<color=red>[동기화 실패]</color>{request.error}");
                    return;
                }
                var jsonData = request.downloadHandler.text;
                if (string.IsNullOrEmpty(jsonData))
                {
                    Debug.LogWarning($"<color=orange>[동기화 경고]</color>서버로부터 빈 응답이 반환되었습니다.");
                    return;
                }
                var unitList = JsonConvert.DeserializeObject<List<UnitDataDTO>>(jsonData);
                if(unitList == null)
                {
                    Debug.LogError($"<color=red>[직렬화 실패]</color>데이터 직렬화에 실패했습니다.");
                }
                await UniTask.SwitchToMainThread();
                ProcessSync(unitList);
            }catch(UnityWebRequestException e){
                Debug.LogError($"[동기화 중 오류 발생]\n{e.Message}");
            }
            finally
            {
                request.Dispose();
            }
        }
        /// <summary>
        /// 유닛 데이터 동기화
        /// </summary>
        /// <param name="loadedUnitList"></param>
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
                unitData.name = data.id;
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
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AddressableEditorUtility.RegisterAsset(unitData, "UnitDataGroup");
            }
            DeleteOldUnitData(serverIds);
            if(logs.Length > 0) Debug.Log(logs.ToString());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=cyan>[동기화 완료]</color> 모든 유닛 데이터 동기화 완료");
        }
        /// <summary>
        /// 유닛 데이터 갱신 함수
        /// </summary>
        /// <param name="unitData"></param>
        /// <param name="dataFromServer"></param>
        private void UpdateUnitData(UnitData unitData, UnitDataDTO dataFromServer)
        {
            unitData.Initialize(dataFromServer.id, dataFromServer.base_stats, dataFromServer.growth_stats);
        }
        /// <summary>
        /// 업데이트 후 남은 기존 데이터 제거 함수
        /// </summary>
        /// <param name="serverIds"></param>
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
            if(string.IsNullOrEmpty(logs.ToString())) Debug.Log(logs.ToString());
        }
    }
}