using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RotateLetters : MonoBehaviour
{
    public float targetRotationAngle;
    private TMP_Text textMeshPro;

    void Start()
    {
        // Get the TextMeshPro component
        textMeshPro = GetComponent<TMP_Text>();

        // Ensure the text is parsed into individual characters (vertex data)
        textMeshPro.ForceMeshUpdate();

        // Rotate individual letters
        RotateIndividualLetters();
    }

    void RotateIndividualLetters()
    {
        TMP_TextInfo textInfo = textMeshPro.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            // Check if the character is visible (not a space or newline)
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            // Rotate each corner of the character's quad
            Quaternion rotation = Quaternion.Euler(0, 0, targetRotationAngle);
            for (int j = 0; j < 4; j++)
            {
                int currentVertexIndex = vertexIndex + j;
                Vector3 rotatedPosition = rotation * textInfo.meshInfo[0].vertices[currentVertexIndex];
                textInfo.meshInfo[0].vertices[currentVertexIndex] = rotatedPosition;
            }
        }

        // Update the mesh with the modified vertices
        textMeshPro.UpdateVertexData();
    }
}
