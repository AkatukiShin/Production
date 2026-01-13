// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#pragma once

#include "CoreMinimal.h"
#include "Subsystems/GameInstanceSubsystem.h"
#include "Enum/CFSEType.h"
#include "Enum/CFBGMType.h"
#include "CFSoundManagerSubsystem.generated.h"

/**
 * 
 */

class UAtomComponent;
class UAtomSoundBase;
class UCFSoundData;

UCLASS(Blueprintable)
class NEWGAMEPROJECT_API UCFSoundManagerSubsystem : public UGameInstanceSubsystem
{
	GENERATED_BODY()
	
// Variable
private:
	UPROPERTY()
	TMap<CFBGMType, TObjectPtr<UAtomSoundBase>> BGMMap;
	UPROPERTY()
	TMap<CFSEType, TObjectPtr<UAtomSoundBase>> SEMap;
	//Component
	UPROPERTY(Transient)
	TObjectPtr<UAtomComponent> SEAtomComponent;
	UPROPERTY(Transient)
	TObjectPtr<UAtomComponent> BGMAtomComponent;

// Function
public:
	virtual void Initialize(FSubsystemCollectionBase& Collection) override;
	virtual void Deinitialize() override;

	UFUNCTION(BlueprintCallable)
	void CallBGM(CFBGMType BGMType);
	UFUNCTION(BlueprintCallable)
	void CallSE(CFSEType SEType);
private:
	void OnWorldInit(UWorld* World, const UWorld::InitializationValues IVS);
};
