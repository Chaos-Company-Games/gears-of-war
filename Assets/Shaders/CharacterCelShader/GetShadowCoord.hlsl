#if defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE) || defined(_MAIN_LIGHT_SHADOWS_SCREEN)
	ShadowCoord = TransformWorldToShadowCoord(WorldPos);
#else
	ShadowCoord = float4(0,0,0,0);
#endif

