// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#pragma once

#include "CoreMinimal.h"
#include "Engine/DataAsset.h"
#include "Enum/CFSEType.h"
#include "Enum/CFBGMType.h"
#include "CFSoundData.generated.h"

/**
 * 
 */
class UAtomComponent;
class UAtomSoundBase;

USTRUCT(BlueprintType)
struct FCFBGMBinding
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, Category = "Sound")
	CFBGMType BGMType = CFBGMType::Normal;

	UPROPERTY(EditAnywhere, Category = "Sound")
	TObjectPtr<UAtomSoundBase> Sound;
};

USTRUCT(BlueprintType)
struct FCFSEBinding
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, Category = "Sound")
	CFSEType SEType = CFSEType::AddCoin;

	UPROPERTY(EditAnywhere, Category = "Sound")
	TObjectPtr<UAtomSoundBase> Sound;
};


UCLASS()
class NEWGAMEPROJECT_API UCFSoundData : public UPrimaryDataAsset
{
	GENERATED_BODY()
// Variable
public:
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "SoundSetting")
	TArray<FCFBGMBinding> BGMBindings;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "SoundSetting")
	TArray<FCFSEBinding> SEBindings;
};
