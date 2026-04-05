using UnityEngine;
using UnityEditor;
using HaChiMiOhNameruDo.MiniGames.TissueGame;

namespace HaChiMiOhNameruDo.Editor
{
    /// <summary>
    /// 纸巾游戏场景设置工具
    /// 用于在 Unity 编辑器中快速创建纸巾游戏所需的 GameObject
    /// </summary>
    public class TissueGameSceneSetupTool : EditorWindow
    {
        [MenuItem("Tools/纸巾游戏/场景设置工具")]
        public static void ShowWindow()
        {
            GetWindow<TissueGameSceneSetupTool>("纸巾游戏场景设置");
        }

        private void OnGUI()
        {
            GUILayout.Label("纸巾游戏场景设置工具", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("创建 TissueBox（纸巾筒）"))
            {
                CreateTissueBox();
            }

            if (GUILayout.Button("创建 TissuePaper（纸巾条）"))
            {
                CreateTissuePaper();
            }

            if (GUILayout.Button("创建 TissuePileManager（纸巾堆管理器）"))
            {
                CreateTissuePileManager();
            }

            if (GUILayout.Button("创建 TissueInputHandler（输入处理器）"))
            {
                CreateTissueInputHandler();
            }

            GUILayout.Space(20);
            GUILayout.Label("说明", EditorStyles.boldLabel);
            GUILayout.Label("1. 点击上述按钮创建所需的 GameObject");
            GUILayout.Label("2. 在 Inspector 中配置每个组件的精灵引用");
            GUILayout.Label("3. 确保 TissueGameManager 引用了这些对象");
        }

        private static void CreateTissueBox()
        {
            // 创建 GameObject
            GameObject boxObj = new GameObject("TissueBox");
            
            // 添加纸巾架 SpriteRenderer
            SpriteRenderer spriteRenderer = boxObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Default";
            spriteRenderer.sortingOrder = 1;

            // 添加纸巾架 BoxCollider2D
            BoxCollider2D boxCollider = boxObj.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(2f, 3f);

            // 创建纸巾筒子对象
            GameObject rollObj = new GameObject("Roll");
            rollObj.transform.SetParent(boxObj.transform);
            rollObj.transform.localPosition = Vector3.zero;
            
            // 添加纸巾筒 SpriteRenderer（排序层级比纸巾架高 1，确保在上方渲染）
            SpriteRenderer rollSpriteRenderer = rollObj.AddComponent<SpriteRenderer>();
            rollSpriteRenderer.sortingLayerName = "Default";
            rollSpriteRenderer.sortingOrder = 2;

            // 添加纸巾筒 BoxCollider2D
            BoxCollider2D rollCollider = rollObj.AddComponent<BoxCollider2D>();
            rollCollider.size = new Vector2(1.5f, 2f);

            // 添加 TissueBox 脚本
            TissueBox tissueBox = boxObj.AddComponent<TissueBox>();

            // 设置位置
            boxObj.transform.position = new Vector3(0f, 2f, 0f);

            // 设置引用
            SerializedObject serializedObject = new SerializedObject(tissueBox);
            serializedObject.FindProperty("spriteRenderer").objectReferenceValue = spriteRenderer;
            serializedObject.FindProperty("boxCollider").objectReferenceValue = boxCollider;
            serializedObject.FindProperty("rollSpriteRenderer").objectReferenceValue = rollSpriteRenderer;
            serializedObject.FindProperty("rollCollider").objectReferenceValue = rollCollider;
            serializedObject.ApplyModifiedProperties();

            Debug.Log("[TissueGameSceneSetupTool] 已创建 TissueBox GameObject（包含纸巾架和纸巾筒）");
            
            // 选中创建的对象
            Selection.activeGameObject = boxObj;
        }

        private static void CreateTissuePaper()
        {
            // 创建 GameObject
            GameObject paperObj = new GameObject("TissuePaper");
            
            // 添加 SpriteRenderer
            SpriteRenderer spriteRenderer = paperObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Default";
            spriteRenderer.sortingOrder = 2;

            // 添加 TissuePaper 脚本
            TissuePaper tissuePaper = paperObj.AddComponent<TissuePaper>();

            // 设置位置
            paperObj.transform.position = new Vector3(0f, 0f, 0f);

            // 设置引用
            SerializedObject serializedObject = new SerializedObject(tissuePaper);
            serializedObject.FindProperty("spriteRenderer").objectReferenceValue = spriteRenderer;
            serializedObject.ApplyModifiedProperties();

            Debug.Log("[TissueGameSceneSetupTool] 已创建 TissuePaper GameObject");
            
            // 选中创建的对象
            Selection.activeGameObject = paperObj;
        }

        private static void CreateTissuePileManager()
        {
            // 创建 GameObject
            GameObject pileManagerObj = new GameObject("TissuePileManager");
            
            // 添加 TissuePileManager 脚本
            TissuePileManager pileManager = pileManagerObj.AddComponent<TissuePileManager>();

            // 创建 L2 子对象
            GameObject pileL2Obj = new GameObject("PileL2");
            pileL2Obj.transform.SetParent(pileManagerObj.transform);
            SpriteRenderer l2Renderer = pileL2Obj.AddComponent<SpriteRenderer>();
            l2Renderer.sortingLayerName = "Default";
            l2Renderer.sortingOrder = 0;

            // 创建 L1 子对象
            GameObject pileL1Obj = new GameObject("PileL1");
            pileL1Obj.transform.SetParent(pileManagerObj.transform);
            SpriteRenderer l1Renderer = pileL1Obj.AddComponent<SpriteRenderer>();
            l1Renderer.sortingLayerName = "Default";
            l1Renderer.sortingOrder = 1;

            // 设置位置
            pileManagerObj.transform.position = new Vector3(0f, -2f, 0f);

            // 设置引用
            SerializedObject serializedObject = new SerializedObject(pileManager);
            serializedObject.FindProperty("l2Renderer").objectReferenceValue = l2Renderer;
            serializedObject.FindProperty("l1Renderer").objectReferenceValue = l1Renderer;
            serializedObject.ApplyModifiedProperties();

            Debug.Log("[TissueGameSceneSetupTool] 已创建 TissuePileManager GameObject");
            
            // 选中创建的对象
            Selection.activeGameObject = pileManagerObj;
        }

        private static void CreateTissueInputHandler()
        {
            // 创建 GameObject
            GameObject inputHandlerObj = new GameObject("TissueInputHandler");
            
            // 添加 TissueInputHandler 脚本
            TissueInputHandler inputHandler = inputHandlerObj.AddComponent<TissueInputHandler>();

            // 创建 TissueBoxZone 子对象（判定区域）
            GameObject boxZoneObj = new GameObject("TissueBoxZone");
            boxZoneObj.transform.SetParent(inputHandlerObj.transform);
            BoxCollider2D boxCollider = boxZoneObj.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(4f, 4f);
            boxCollider.isTrigger = true;
            boxZoneObj.transform.position = new Vector3(0f, 2f, 0f);

            // 创建 TissuePileZone 子对象（判定区域）
            GameObject pileZoneObj = new GameObject("TissuePileZone");
            pileZoneObj.transform.SetParent(inputHandlerObj.transform);
            BoxCollider2D pileCollider = pileZoneObj.AddComponent<BoxCollider2D>();
            pileCollider.size = new Vector2(4f, 4f);
            pileCollider.isTrigger = true;
            pileZoneObj.transform.position = new Vector3(0f, -2f, 0f);

            // 设置引用
            SerializedObject serializedObject = new SerializedObject(inputHandler);
            serializedObject.FindProperty("tissueBoxZone").objectReferenceValue = boxCollider;
            serializedObject.FindProperty("tissuePileZone").objectReferenceValue = pileCollider;
            serializedObject.ApplyModifiedProperties();

            Debug.Log("[TissueGameSceneSetupTool] 已创建 TissueInputHandler GameObject");
            
            // 选中创建的对象
            Selection.activeGameObject = inputHandlerObj;
        }
    }
}
