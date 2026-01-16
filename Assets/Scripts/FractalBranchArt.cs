using UnityEngine;
using UnityEngine.UI;

public class FractalBranchArt : MonoBehaviour
{
    public enum FractalType { Tree, Coral, Lightning }
    public FractalType selectedFractal = FractalType.Tree;

    [Header("General Settings")]
    [Range(2, 12)]
    public int depth = 5;

    [Range(0.5f, 5.0f)]
    public float mainBranchScale = 1.0f;
    [Range(1.0f, 10.0f)]
    public float mainBranchThickness = 2.0f;

    [Range(0.2f, 1.0f)]
    public float subBranchScaleFactor = 0.7f;

    [Range(0, 90)]
    public float branchAngle = 30f;

    [Range(-360, 360)]
    public float rotationOffset = 0f;

    [Header("Branch Customization")]
    [Range(2, 10)]
    public int additionalBranches = 2;

    [Header("Position Offset Settings")]
    [Range(-0.5f, 0.5f)]
    public float xOffset = 0.0f;

    [Range(-0.5f, 0.5f)]
    public float yOffset = 0.0f;

    [Header("Color Settings")]
    public Gradient branchColors;

    [Range(0.1f, 3.0f)]
    public float colorIntensity = 1.0f;

    [Header("Coral and Lightning Specific Settings")]
    [Range(0.0f, 1.0f)]
    public float randomness = 0.3f;

    [Range(1.0f, 5.0f)]
    public float lightningThickness = 1.0f;

    private Texture2D fractalTexture;
    public RawImage displayImage;

    private void OnValidate()
    {
        GenerateFractalArt();
    }

    private void GenerateFractalArt()
    {
        fractalTexture = new Texture2D(512, 512);
        var pixels = new Color[fractalTexture.width * fractalTexture.height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.black;
        fractalTexture.SetPixels(pixels);

        float startX = fractalTexture.width / 2 + xOffset * fractalTexture.width;
        float startY = fractalTexture.height / 4 + yOffset * fractalTexture.height;
        DrawBranch(startX, startY, mainBranchScale * fractalTexture.height / 4, -90, depth, mainBranchThickness);

        fractalTexture.Apply();

        displayImage.texture = fractalTexture;
    }

    private void DrawBranch(float startX, float startY, float length, float angle, int currentDepth, float thickness)
    {
        if (currentDepth <= 0 || length <= 1f) return;

        float endX = startX + Mathf.Cos(Mathf.Deg2Rad * angle) * length;
        float endY = startY + Mathf.Sin(Mathf.Deg2Rad * angle) * length;

        Color branchColor = branchColors.Evaluate(1.0f - (float)currentDepth / depth) * colorIntensity;
        DrawLine((int)startX, (int)startY, (int)endX, (int)endY, branchColor, thickness);

        if (selectedFractal == FractalType.Coral || selectedFractal == FractalType.Lightning)
        {
            angle += Random.Range(-randomness * branchAngle, randomness * branchAngle);
            length *= Random.Range(subBranchScaleFactor * 0.9f, subBranchScaleFactor * 1.1f);
        }

        for (int i = 0; i < additionalBranches; i++)
        {
            float branchOffset = i * (branchAngle / additionalBranches);
            DrawBranch(endX, endY, length * subBranchScaleFactor, angle - branchOffset + rotationOffset, currentDepth - 1, thickness * subBranchScaleFactor);
            DrawBranch(endX, endY, length * subBranchScaleFactor, angle + branchOffset + rotationOffset, currentDepth - 1, thickness * subBranchScaleFactor);
        }

        if (selectedFractal == FractalType.Lightning)
        {
            DrawBranch(endX, endY, length * 0.5f, angle + Random.Range(-30, 30), currentDepth - 1, thickness * 0.5f);
        }
    }

    private void DrawLine(int x0, int y0, int x1, int y1, Color color, float thickness)
    {
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = (dx > dy ? dx : -dy) / 2, e2;

        for (int i = -Mathf.FloorToInt(thickness / 2); i <= Mathf.FloorToInt(thickness / 2); i++)
        {
            int xOffset = (Mathf.Abs(dx) > Mathf.Abs(dy)) ? 0 : i;
            int yOffset = (Mathf.Abs(dx) > Mathf.Abs(dy)) ? i : 0;

            int tx0 = x0 + xOffset;
            int ty0 = y0 + yOffset;

            while (true)
            {
                if (tx0 >= 0 && tx0 < fractalTexture.width && ty0 >= 0 && ty0 < fractalTexture.height)
                    fractalTexture.SetPixel(tx0, ty0, color);
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; tx0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; ty0 += sy; }
            }
        }
    }

}
