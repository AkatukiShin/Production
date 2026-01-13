// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#pragma once

#include "CoreMinimal.h"
#include "Engine/DeveloperSettings.h"
#include "CFSoundDataSettings.generated.h"

/**
 * 
 */

class UCFSoundData;

UCLASS(Config = Game, DefaultConfig, meta = (DisplayName = "Sound Manager Settings"))
class NEWGAMEPROJECT_API UCFSoundDataSettings : public UDeveloperSettings
{
	GENERATED_BODY()
public:
	UPROPERTY(Config, EditAnywhere, BlueprintReadOnly, Category = "Data")
	TSoftObjectPtr<UCFSoundData> SoundDataAssetToLoad;
};
