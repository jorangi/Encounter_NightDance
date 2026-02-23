using UnityEngine;
using UnityEditor;
using Encounter.NightDance.Status;
namespace Encounter.NightDance.Editor
{
    [CustomPropertyDrawer(typeof(Stat))]
    public class StatDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty baseValueProp = property.FindPropertyRelative("<BaseValue>k__BackingField");
            Stat targetStat = GetTargetObjectOfProperty(property) as Stat;
            string finalValueText = (targetStat != null) ? $"{label} (Final: {targetStat.Value})" : " (Err)";

            label.text = finalValueText;
            EditorGUI.PropertyField(position, baseValueProp, label, true);
            EditorGUI.EndProperty();
        }
        private object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[","[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach(var element in elements)
            {
                if(element.Contains('['))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }
        private object GetValue_Imp(object source, string name)
        {
            if(source == null) return null;
            var type = source.GetType();
            while(type != null)
            {
                var f = type.GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if(f != null) return f.GetValue(source);
                type = type.BaseType;
            }
            return null;
        }
        private object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if(enumerable == null) return null;
            var enm = enumerable.GetEnumerator();
            for(int i = 0; i < index; i++) if(!enm.MoveNext()) return null;
            return enm.Current;
        }
    }
}
