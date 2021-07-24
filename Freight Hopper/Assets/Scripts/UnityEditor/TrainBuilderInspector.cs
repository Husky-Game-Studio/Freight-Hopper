using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(TrainBuilder))]
public class TrainBuilderInspector : Editor
{
    private TrainBuilder trainBuilder;
    private int cartIndex = 0;
    private int cargoIndex = 0;
    private bool isGUIenabled = true;

    public void OnEnable()
    {
        trainBuilder = (TrainBuilder)this.target;
        (int, int) settings = trainBuilder.LoadModelSettings();
        cartIndex = settings.Item1;
        cargoIndex = settings.Item2;
    }

    public override void OnInspectorGUI()
    {
        trainBuilder.SaveModelSettings(cartIndex, cargoIndex);

        EditorGUILayout.LabelField("Model Settings");
        string[] cartNames = System.Enum.GetNames(typeof(TrainBuilder.TrainCarts));
        string[] cargoNames = System.Enum.GetNames(typeof(TrainBuilder.TrainCargos));

        cartIndex = EditorGUILayout.Popup("Carts", cartIndex, cartNames);
        cargoIndex = EditorGUILayout.Popup("Cargos", cargoIndex, cargoNames);
        EditorGUILayout.Space(10);

        if (GUILayout.Button("Add"))
        {
            trainBuilder.AddCart((TrainBuilder.TrainCarts)cartIndex, (TrainBuilder.TrainCargos)cargoIndex);
            trainBuilder.OnValidate();
        }

        GUI.enabled = trainBuilder.CanRemoveOrClear;
        isGUIenabled = trainBuilder.CanRemoveOrClear;

        if (GUILayout.Button("Remove"))
        {
            trainBuilder.RemoveCart();
            trainBuilder.OnValidate();
        }
        if (GUILayout.Button("Replace"))
        {
            trainBuilder.ReplaceCart((TrainBuilder.TrainCarts)cartIndex, (TrainBuilder.TrainCargos)cargoIndex);
            trainBuilder.OnValidate();
        }
        if (GUILayout.Button("Clear"))
        {
            trainBuilder.Clear();
            trainBuilder.OnValidate();
        }
        EditorGUILayout.Space(10);

        if (cargoIndex == (int)TrainBuilder.TrainCargos.None)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("Add Cargo"))
        {
            trainBuilder.AddCargo((TrainBuilder.TrainCargos)cargoIndex);
            trainBuilder.OnValidate();
        }
        GUI.enabled = isGUIenabled;

        if (!trainBuilder.SelectedCartHasCargo)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("Remove Cargo"))
        {
            trainBuilder.RemoveCargo();
            trainBuilder.OnValidate();
        }
        GUI.enabled = isGUIenabled;

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Update Train"))
        {
            trainBuilder.UpdateTrain();
            trainBuilder.OnValidate();
        }
        if (GUILayout.Button("Update All Trains"))
        {
            trainBuilder.UpdateAllTrains();
        }
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Move Forward"))
        {
            trainBuilder.DecrementActionIndex();
            trainBuilder.OnValidate();
        }
        if (GUILayout.Button("Move Backwards"))
        {
            trainBuilder.IncrementActionIndex();
            trainBuilder.OnValidate();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Other Settings");
        GUI.enabled = true;
        base.OnInspectorGUI();
    }
}

#endif