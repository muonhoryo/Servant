using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

namespace Servant.DevelopmentOnly
{
    [CustomEditor(typeof(GlobalConstantManager))]
    public sealed class GlobalConstantManagerEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            GlobalConstantManager.instance_.OnCreateInspector();
            return base.CreateInspectorGUI();
        }
    }
}
