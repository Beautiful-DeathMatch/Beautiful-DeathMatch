Shader "Custom/HalfLambertOutline"
{
    Properties
    {
        _BaseMap ("_BaseMap", 2D) = "white" {}
        [MainColor] _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineDistance("Outline Distance", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        UsePass "Custom/HalfLambert/HalfLambert"
        UsePass "Custom/Outline/OUTLINE"
    }
}
