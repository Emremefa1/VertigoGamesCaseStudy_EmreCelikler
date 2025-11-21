using System.Collections.Generic;
using UnityEngine;
using WheelGame.Data;

namespace WheelGame.Core
{
    /// <summary>
    /// Builds the wheel slices from a preset using 8 pre-configured position transforms.
    /// Instantiates slice prefabs at each position and fills with slice data.
    /// </summary>
    public class WheelBuilder : MonoBehaviour
    {
        [SerializeField] private GameObject slicePrefab;
        [SerializeField] private List<Transform> slicePositions = new List<Transform>();

        private const int SLICE_COUNT = 8;
        private List<SliceDefinitionSO> currentSOList = new List<SliceDefinitionSO>();
        private List<WheelSliceView> sliceViews = new List<WheelSliceView>();

        public int SetupSlices(List<SliceDefinitionSO> soList)
        {
            currentSOList = new List<SliceDefinitionSO>(soList);

            // ensure count equals SLICE_COUNT
            if (currentSOList.Count < SLICE_COUNT)
            {
                var placeholder = ScriptableObject.CreateInstance<SliceDefinitionSO>();
                for (int i = currentSOList.Count; i < SLICE_COUNT; i++)
                {
                    currentSOList.Add(placeholder);
                }
            }
            else if (currentSOList.Count > SLICE_COUNT)
            {
                currentSOList = currentSOList.GetRange(0, SLICE_COUNT);
            }

            BuildVisualSlices();
            return SLICE_COUNT;
        }

        private void BuildVisualSlices()
        {
            // Validate positions
            if (slicePositions == null || slicePositions.Count != SLICE_COUNT)
            {
                Debug.LogError($"WheelBuilder: Expected exactly {SLICE_COUNT} slice positions, but found {slicePositions?.Count ?? 0}");
                return;
            }

            // Clear existing slices at positions
            foreach (var pos in slicePositions)
            {
                for (int i = pos.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(pos.GetChild(i).gameObject);
                }
            }

            sliceViews.Clear();

            // Create slices at each position
            for (int i = 0; i < SLICE_COUNT; i++)
            {
                var go = Instantiate(slicePrefab, slicePositions[i]);
                go.name = $"Slice_{i:00}";
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                var view = go.GetComponent<WheelSliceView>();
                if (view == null) view = go.AddComponent<WheelSliceView>();
                
                var so = currentSOList[i];
                view.SetData(so);
                sliceViews.Add(view);
            }
        }

        public SliceDefinitionSO GetSliceSO(int index)
        {
            if (index < 0 || index >= currentSOList.Count) return null;
            return currentSOList[index];
        }

        public int GetSliceCount() => SLICE_COUNT;
    }
}

