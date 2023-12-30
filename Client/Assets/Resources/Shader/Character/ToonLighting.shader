Shader "Custom/ToonLighting"
{
    Properties
    {
        _BaseMap ("_BaseMap", 2D) = "white" {}
        [MainColor] _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineDistance("Outline Distance", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        UsePass "Custom/CartoonLambert/CartoonLambert"
        UsePass "Custom/Outline/OUTLINE"
    }
}
