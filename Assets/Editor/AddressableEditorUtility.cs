using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Encounter.NightDance.Editor
{
    public static class AddressableEditorUtility
    {
        /// <summary>
        /// Addressables에 에셋 등록하는 함수
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="groupName"></param>
        /// <param name="address"></param>
        public static void RegisterAsset(Object asset, string groupName, string address = null)
        {
            //기본 설정 불러오기
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if(settings == null) // 없을 경우 리턴하고 에러로그
            {
                Debug.LogError($"<color=red>[Addressables]</color>설정을 찾을 수 없습니다. Addressables 창을 열고 설정을 생성해주세요.");
                return;
            }
            if(asset == null)
            {
                Debug.LogError($"<color=red>[Addressables]</color>등록하려는 에셋이 Null입니다.");
                return;
            }
            //그룹 없을시 생성
            AddressableAssetGroup group = settings.FindGroup(groupName) ??
                settings.CreateGroup(groupName, false, false, false, settings.DefaultGroup.Schemas);
            
            //에셋 경로 기반으로 guid 불러옴
            string assetPath = AssetDatabase.GetAssetPath(asset);
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if(string.IsNullOrEmpty(guid))
            {
                Debug.LogError($"<color=yellow>[Addressables]</color>GUID를 찾을 수 없음: {asset.name}. DB를 강제 갱신합니다.");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                guid = AssetDatabase.AssetPathToGUID(assetPath);
            }
            //엔트리 생성 혹은 이동
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
            if(entry == null)
            {
                Debug.LogError($"<color=red>[Addressables]</color>엔트리 생성 실패");
                return;
            }
            if (string.IsNullOrEmpty(address))
            {
                if (!string.IsNullOrEmpty(asset.name))
                {
                    entry.address = asset.name;
                }
                else
                {
                    entry.address = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                }
            }
            else
            {
                entry.address = address;
            }
            //저장
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
        }
    }
}